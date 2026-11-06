using Microsoft.AspNetCore.Mvc;
using PeopleAccounts.web.Services;
using PeopleAccounts.web.ViewModels;

namespace PeopleAccounts.web.Controllers;

public class AccountsController: Controller
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Create(int personCode)
    {
        CreateAccountViewModel? model =
            await _accountService.GetForCreateAsync(personCode);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        CreateAccountResult result =
            await _accountService.CreateAsync(model);

        switch (result)
        {
            case CreateAccountResult.Success:
                TempData["SuccessMessage"] = "Account created successfully.";

                return RedirectToAction(
                    "Details",
                    "Persons",
                    new { id = model.PersonCode });

            case CreateAccountResult.PersonNotFound:
                return NotFound();

            case CreateAccountResult.DuplicateAccountNumber:
                ModelState.AddModelError(
                    nameof(model.AccountNumber),
                    "This account number already exists.");

                return View(model);

            default:
                throw new InvalidOperationException(
                    "Unexpected account creation result.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        EditAccountViewModel? model =
            await _accountService.GetForEditAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        EditAccountViewModel model)
    {
        if (id != model.Code)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        UpdateAccountResult result =
            await _accountService.UpdateAsync(model);

        switch (result)
        {
            case UpdateAccountResult.Success:
                TempData["SuccessMessage"] = "Account updated successfully.";

                return RedirectToAction("Edit", new { id = model.Code });

            case UpdateAccountResult.NotFound:
                return NotFound();

            case UpdateAccountResult.DuplicateAccountNumber:
                ModelState.AddModelError(
                    nameof(model.AccountNumber),
                    "This account number already exists.");
                break;

            case UpdateAccountResult.InvalidStatus:
                ModelState.AddModelError(
                    nameof(model.StatusCode),
                    "Select a valid account status.");
                break;

            case UpdateAccountResult.NonZeroBalance:
                ModelState.AddModelError(
                    nameof(model.StatusCode),
                    "An account can only be closed when its balance is zero.");
                break;

            default:
                throw new InvalidOperationException(
                    "Unexpected account update result.");
        }

        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Details(
        int id,
        int pageNumber = 1,
        int pageSize = 10)
    {
        AccountDetailsViewModel? model =
            await _accountService.GetDetailsAsync(id, pageNumber, pageSize);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }
}