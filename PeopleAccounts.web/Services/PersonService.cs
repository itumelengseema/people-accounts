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
}