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
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10,string? searchTerm = null)
    {
        string? search = searchTerm?.Trim();
        
        PagedResult<Person> result = await _personService.GetPagedAsync(pageNumber, pageSize,search);

        ViewData["SearchTerm"] = search;

        return View(result);
    }
    
    
    [HttpGet]
    public IActionResult Create()
    {
        CreatePersonViewModel model = new CreatePersonViewModel();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePersonViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool created = await _personService.CreateAsync(model);

        if (!created)
        {
            ModelState.AddModelError(
                nameof(CreatePersonViewModel.IdNumber),
                "A person with this ID number already exists.");

            return View(model);
        }

        TempData["SuccessMessage"] = "Person created successfully.";

        return RedirectToAction(nameof(Index));
    }
    
}