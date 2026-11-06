using Microsoft.AspNetCore.Mvc;
using PeopleAccounts.web.Models;
using PeopleAccounts.web.Services;
using PeopleAccounts.web.ViewModels;

namespace PeopleAccounts.web.Controllers;

public class PersonsController : Controller
{
    private readonly IPersonService _personService;

    public PersonsController(IPersonService personService)
    {
        _personService = personService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        PagedResult<Person> result = await _personService.GetPagedAsync(pageNumber, pageSize);

        return View(result);
    }
}