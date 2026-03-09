using Claims.Domain.Enums;

namespace Claims.Application.Requests.Cover;

public class ComputePremiumRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CoverType Type { get; set; }
}
