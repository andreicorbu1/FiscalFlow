namespace FiscalFlow.Contracts.Accounts;

public class PeriodOfTimeRequest
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}