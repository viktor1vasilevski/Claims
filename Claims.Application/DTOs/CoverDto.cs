using Claims.Domain.Enums;

namespace Claims.Application.DTOs;

public class CoverDto
{
    public Guid? Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CoverType Type { get; set; }
    public decimal Premium { get; set; }
}
