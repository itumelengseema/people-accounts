using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PeopleAccounts.web.Data;
using PeopleAccounts.web.Models;
using PeopleAccounts.web.ViewModels;

namespace PeopleAccounts.web.Services;

public class TransactionService: ITransactionService
{
    private readonly ApplicationDbContext _db;

    public TransactionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CreateTransactionViewModel?> GetForCreateAsync(
        int accountCode)
    {
        bool accountExists = await _db.Accounts
            .AnyAsync(account => account.Code == accountCode);

        if (!accountExists)
        {
            return null;
        }

        return new CreateTransactionViewModel
        {
            AccountCode = accountCode,
            TransactionDate = DateTime.Today
        };
    }

    public async Task<CreateTransactionResult> CreateAsync(
        CreateTransactionViewModel model)
    {
        Validator.ValidateObject(
            model,
            new ValidationContext(model),
            validateAllProperties: true);

        await using IDbContextTransaction databaseTransaction =
            await _db.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        Account? account = await _db.Accounts
            .SingleOrDefaultAsync(
                existing => existing.Code == model.AccountCode);

        if (account == null)
        {
            return CreateTransactionResult.AccountNotFound;
        }

        const int openStatusCode = 1;

        if (account.StatusCode != openStatusCode)
        {
            return CreateTransactionResult.AccountClosed;
        }

        Transaction transaction = new Transaction
        {
            AccountCode = account.Code,
            TransactionDate = model.TransactionDate!.Value.Date,
            CaptureDate = DateTime.Now,
            Amount = model.Amount,
            Description = model.Description.Trim()
        };

        _db.Transactions.Add(transaction);

        account.OutstandingBalance += model.Amount;

        await _db.SaveChangesAsync();
        await databaseTransaction.CommitAsync();

        return CreateTransactionResult.Success;
    }
    public async Task<EditTransactionViewModel?> GetForEditAsync(int code)
    {
        return await _db.Transactions
            .AsNoTracking()
            .Where(transaction => transaction.Code == code)
            .Select(transaction => new EditTransactionViewModel
            {
                Code = transaction.Code,
                TransactionDate = transaction.TransactionDate,
                Amount = transaction.Amount,
                Description = transaction.Description
            })
            .SingleOrDefaultAsync();
    }
    
    public async Task<UpdateTransactionResult> UpdateAsync(
        EditTransactionViewModel model)
    {
        Validator.ValidateObject(
            model,
            new ValidationContext(model),
            validateAllProperties: true);

        await using IDbContextTransaction databaseTransaction =
            await _db.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        Transaction? transaction = await _db.Transactions
            .Include(existing => existing.AccountCodeNavigation)
            .SingleOrDefaultAsync(existing => existing.Code == model.Code);

        if (transaction == null)
        {
            return UpdateTransactionResult.NotFound;
        }

        Account account = transaction.AccountCodeNavigation;

        const int openStatusCode = 1;

        if (account.StatusCode != openStatusCode)
        {
            return UpdateTransactionResult.AccountClosed;
        }

        decimal difference = model.Amount - transaction.Amount;

        account.OutstandingBalance += difference;

        transaction.TransactionDate = model.TransactionDate!.Value.Date;
        transaction.Amount = model.Amount;
        transaction.Description = model.Description.Trim();
        transaction.CaptureDate = DateTime.Now;

        await _db.SaveChangesAsync();
        await databaseTransaction.CommitAsync();

        return UpdateTransactionResult.Success;
    }
}