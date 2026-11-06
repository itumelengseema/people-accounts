using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PeopleAccounts.web.Data;
using PeopleAccounts.web.Models;
using PeopleAccounts.web.ViewModels;

namespace PeopleAccounts.web.Services;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _db;

    public AccountService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CreateAccountViewModel?> GetForCreateAsync(
        int personCode)
    {
        bool personExists = await _db.Persons
            .AnyAsync(person => person.Code == personCode);

        if (!personExists)
        {
            return null;
        }

        return new CreateAccountViewModel
        {
            PersonCode = personCode
        };
    }

    public async Task<CreateAccountResult> CreateAsync(
        CreateAccountViewModel model)
    {
        Validator.ValidateObject(
            model,
            new ValidationContext(model),
            validateAllProperties: true);

        bool personExists = await _db.Persons
            .AnyAsync(person => person.Code == model.PersonCode);

        if (!personExists)
        {
            return CreateAccountResult.PersonNotFound;
        }

        string accountNumber = model.AccountNumber.Trim();

        bool accountExists = await _db.Accounts
            .AnyAsync(account =>
                account.AccountNumber == accountNumber);

        if (accountExists)
        {
            return CreateAccountResult.DuplicateAccountNumber;
        }

        const int openStatusCode = 1;

        Account account = new Account
        {
            PersonCode = model.PersonCode,
            AccountNumber = accountNumber,
            OutstandingBalance = 0m,
            StatusCode = openStatusCode
        };

        _db.Accounts.Add(account);

        try
        {
            await _db.SaveChangesAsync();

            return CreateAccountResult.Success;
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException sqlException &&
                  (sqlException.Number == 2601 ||
                   sqlException.Number == 2627))
        {
            _db.Entry(account).State = EntityState.Detached;

            bool duplicateNumber = await _db.Accounts
                .AnyAsync(existing =>
                    existing.AccountNumber == accountNumber);

            if (duplicateNumber)
            {
                return CreateAccountResult.DuplicateAccountNumber;
            }

            throw;
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException sqlException &&
                  sqlException.Number == 547)
        {
            _db.Entry(account).State = EntityState.Detached;

            bool personStillExists = await _db.Persons
                .AnyAsync(person => person.Code == model.PersonCode);

            if (!personStillExists)
            {
                return CreateAccountResult.PersonNotFound;
            }

            throw;
        }
    }

    public async Task<EditAccountViewModel?> GetForEditAsync(int code)
    {
        return await _db.Accounts
            .AsNoTracking()
            .Where(account => account.Code == code)
            .Select(account => new EditAccountViewModel
            {
                Code = account.Code,
                AccountNumber = account.AccountNumber,
                StatusCode = account.StatusCode
            })
            .SingleOrDefaultAsync();
    }

    public async Task<UpdateAccountResult> UpdateAsync(
        EditAccountViewModel model)
    {
        const int openStatusCode = 1;
        const int closedStatusCode = 2;

        if (model.StatusCode != openStatusCode &&
            model.StatusCode != closedStatusCode)
        {
            return UpdateAccountResult.InvalidStatus;
        }

        Validator.ValidateObject(
            model,
            new ValidationContext(model),
            validateAllProperties: true);

        await using var transaction =
            await _db.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        Account? account = await _db.Accounts
            .SingleOrDefaultAsync(existing => existing.Code == model.Code);

        if (account == null)
        {
            return UpdateAccountResult.NotFound;
        }

        string accountNumber = model.AccountNumber.Trim();

        bool duplicateNumber = await _db.Accounts
            .AnyAsync(existing =>
                existing.AccountNumber == accountNumber &&
                existing.Code != model.Code);

        if (duplicateNumber)
        {
            return UpdateAccountResult.DuplicateAccountNumber;
        }

        bool statusExists = await _db.Set<Status>()
            .AnyAsync(status => status.Code == model.StatusCode);

        if (!statusExists)
        {
            return UpdateAccountResult.InvalidStatus;
        }

        if (model.StatusCode == closedStatusCode &&
            account.OutstandingBalance != 0m)
        {
            return UpdateAccountResult.NonZeroBalance;
        }

        account.AccountNumber = accountNumber;
        account.StatusCode = model.StatusCode;

        try
        {
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return UpdateAccountResult.Success;
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException sqlException &&
                  (sqlException.Number == 2601 ||
                   sqlException.Number == 2627))
        {
            await transaction.RollbackAsync();
            _db.Entry(account).State = EntityState.Detached;

            bool numberIsTaken = await _db.Accounts
                .AnyAsync(existing =>
                    existing.AccountNumber == accountNumber &&
                    existing.Code != model.Code);

            if (numberIsTaken)
            {
                return UpdateAccountResult.DuplicateAccountNumber;
            }

            throw;
        }
    }
    
    public async Task<AccountDetailsViewModel?> GetDetailsAsync(
    int code,
    int pageNumber,
    int pageSize)
{
    pageNumber = Math.Max(1, pageNumber);
    pageSize = Math.Clamp(pageSize, 1, 100);

    AccountDetailsViewModel? model = await _db.Accounts
        .AsNoTracking()
        .Where(account => account.Code == code)
        .Select(account => new AccountDetailsViewModel
        {
            Code = account.Code,
            PersonCode = account.PersonCode,
            AccountNumber = account.AccountNumber,
            OutstandingBalance = account.OutstandingBalance,
            StatusCode = account.StatusCode,
            StatusName = account.StatusCodeNavigation.Name
        })
        .SingleOrDefaultAsync();

    if (model == null)
    {
        return null;
    }

    IQueryable<Transaction> query = _db.Transactions
        .AsNoTracking()
        .Where(transaction => transaction.AccountCode == code);

    int totalCount = await query.CountAsync();

    int totalPages = (int)Math.Ceiling(
        totalCount / (double)pageSize);

    pageNumber = totalPages == 0
        ? 1
        : Math.Min(pageNumber, totalPages);

    List<TransactionListItemViewModel> transactions = await query
        .OrderByDescending(transaction => transaction.TransactionDate)
        .ThenByDescending(transaction => transaction.Code)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(transaction => new TransactionListItemViewModel
        {
            Code = transaction.Code,
            TransactionDate = transaction.TransactionDate,
            CaptureDate = transaction.CaptureDate,
            Amount = transaction.Amount,
            Description = transaction.Description
        })
        .ToListAsync();

    model.Transactions = new PagedResult<TransactionListItemViewModel>
    {
        Items = transactions,
        PageNumber = pageNumber,
        PageSize = pageSize,
        TotalCount = totalCount
    };

    return model;
}
}