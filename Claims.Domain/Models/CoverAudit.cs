using Claims.Domain.Enums;

namespace Claims.Domain.Models;

public class CoverAudit
{
    public int Id { get; set; }
    public string? CoverId { get; set; }
    public DateTime Created { get; set; }
    public HttpRequestType HttpRequestType { get; set; }
}
