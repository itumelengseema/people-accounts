namespace PeopleAccounts.web.Services;

public enum UpdateAccountResult
{
    Success,
    NotFound,
    DuplicateAccountNumber,
    InvalidStatus,
    NonZeroBalance
}