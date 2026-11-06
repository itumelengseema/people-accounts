using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PeopleAccounts.web.Data;
using PeopleAccounts.web.Models;
using PeopleAccounts.web.ViewModels;

namespace PeopleAccounts.web.Services;

public class PersonService : IPersonService
{
    private readonly ApplicationDbContext _db;

    public PersonService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<Person>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        IQueryable<Person> query = _db.Persons.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string search = searchTerm.Trim();

            query = query.Where(person =>
                person.IdNumber.Contains(search) || (person.Surname) != null && person.Surname.Contains(search) ||
                person.Accounts.Any(account => account.AccountNumber.Contains(search)));
        }

        int totalCount = await query.CountAsync();

        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        pageNumber = Math.Min(
            pageNumber,
            Math.Max(1, totalPages));

        int recordsToSkip = (pageNumber - 1) * pageSize;

        List<Person> persons =
            await query.OrderBy(person => person.Code).Skip(recordsToSkip).Take(pageSize).ToListAsync();

        return new PagedResult<Person>
        {
            Items = persons,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<bool> CreateAsync(CreatePersonViewModel model)
    {
        Validator.ValidateObject(model,new ValidationContext(model),validateAllProperties:true);

        string idNumber = model.IdNumber.Trim();

        bool isExist = await _db.Persons.AnyAsync(person => person.IdNumber == idNumber);

        if (isExist)
        {
            return false;
        }

        Person person = new Person
        {
            Name = model.Name?.Trim(),
            Surname = model.Surname?.Trim(),
            IdNumber = idNumber
        };
        _db.Persons.Add(person);

        try
        {
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        when(e.InnerException is SqlException sqlException && (sqlException.Number == 2601 || sqlException.Number == 2627))
        {
            _db.Entry(person).State = EntityState.Detached;
            
    
            bool duplicateId = await _db.Persons
                .AnyAsync(existing => existing.IdNumber == idNumber);

            if (duplicateId)
            {
                return false;
            }

            throw;
        }
    }

    public async Task<EditPersonViewModel?> GetForEditAsync(int code)
    {
        return await _db.Persons
            .AsNoTracking()
            .Where(person => person.Code == code)
            .Select(person => new EditPersonViewModel
            {
                Code = person.Code,
                Name = person.Name,
                Surname = person.Surname,
                IdNumber = person.IdNumber
            })
            .SingleOrDefaultAsync();
    }

    public async Task<UpdatePersonResult> UpdateAsync(
        EditPersonViewModel model)
    {
        Validator.ValidateObject(
            model,
            new ValidationContext(model),
            validateAllProperties: true);

        Person? person = await _db.Persons
            .SingleOrDefaultAsync(person => person.Code == model.Code);

        if (person == null)
        {
            return UpdatePersonResult.NotFound;
        }

        string idNumber = model.IdNumber.Trim();

        bool duplicateId = await _db.Persons
            .AnyAsync(existing =>
                existing.IdNumber == idNumber &&
                existing.Code != model.Code);

        if (duplicateId)
        {
            return UpdatePersonResult.DuplicateIdNumber;
        }

        person.Name = model.Name?.Trim();
        person.Surname = model.Surname?.Trim();
        person.IdNumber = idNumber;

        try
        {
            await _db.SaveChangesAsync();

            return UpdatePersonResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            _db.Entry(person).State = EntityState.Detached;

            bool stillExists = await _db.Persons
                .AnyAsync(existing => existing.Code == model.Code);

            if (!stillExists)
            {
                return UpdatePersonResult.NotFound;
            }

            throw;
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException sqlException &&
                  (sqlException.Number == 2601 ||
                   sqlException.Number == 2627))
        {
            _db.Entry(person).State = EntityState.Detached;

            bool idNowExists = await _db.Persons
                .AnyAsync(existing =>
                    existing.IdNumber == idNumber &&
                    existing.Code != model.Code);

            if (idNowExists)
            {
                return UpdatePersonResult.DuplicateIdNumber;
            }

            throw;
        }
    }
}