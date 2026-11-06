using Microsoft.AspNetCore.Mvc;
using PeopleAccounts.web.Services;
using PeopleAccounts.web.ViewModels;

namespace PeopleAccounts.web.Controllers;

public class TransactionsController : Controller
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int accountCode)
    {
        CreateTransactionViewModel? model =
            await _transactionService.GetForCreateAsync(accountCode);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateTransactionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        CreateTransactionResult result =
            await _transactionService.CreateAsync(model);

        switch (result)
        {
            case CreateTransactionResult.Success:
                TempData["SuccessMessage"] =
                    "Transaction created successfully.";

                return RedirectToAction(
                    "Details",
                    "Accounts",
                    new { id = model.AccountCode });

            case CreateTransactionResult.AccountNotFound:
                return NotFound();

            case CreateTransactionResult.AccountClosed:
                ModelState.AddModelError(
                    string.Empty,
                    "Transactions cannot be posted to a closed account.");

                return View(model);

            default:
                throw new InvalidOperationException(
                    "Unexpected transaction creation result.");
        }
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        EditTransactionViewModel? model =
            await _transactionService.GetForEditAsync(id);

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
        EditTransactionViewModel model)
    {
        if (id != model.Code)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        UpdateTransactionResult result =
            await _transactionService.UpdateAsync(model);

        switch (result)
        {
            case UpdateTransactionResult.Success:
                TempData["SuccessMessage"] =
                    "Transaction updated successfully.";

                return RedirectToAction("Edit", new { id = model.Code });

            case UpdateTransactionResult.NotFound:
                return NotFound();

            case UpdateTransactionResult.AccountClosed:
                ModelState.AddModelError(
                    string.Empty,
                    "Transactions on a closed account cannot be edited.");

                return View(model);

            default:
                throw new InvalidOperationException(
                    "Unexpected transaction update result.");
        }
    }
}