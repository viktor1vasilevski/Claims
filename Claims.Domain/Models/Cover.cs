using Claims.Domain.Enums;

namespace Claims.Domain.Models;

public class Cover
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CoverType Type { get; set; }
    public decimal Premium { get; set; }
}
