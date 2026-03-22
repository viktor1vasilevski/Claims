using Claims.Domain.Exceptions;

namespace Claims.Domain.Models;

public class Cover
{
    public Guid Id { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public CoverType Type { get; private set; }
    public decimal Premium { get; private set; }

    private Cover() { }

    public static Cover Create(DateTime startDate, DateTime endDate, CoverType type, decimal premium)
    {
        if (endDate <= startDate)
            throw new InvalidCoverPeriodException("EndDate must be after StartDate.");

        if ((endDate - startDate).TotalDays > 365)
            throw new InvalidCoverPeriodException("Insurance period cannot exceed 1 year.");

        return new Cover
        {
            Id = Guid.NewGuid(),
            StartDate = startDate,
            EndDate = endDate,
            Type = type,
            Premium = premium
        };
    }

    public bool IsDateWithinPeriod(DateTime date) => date >= StartDate && date <= EndDate;
}
