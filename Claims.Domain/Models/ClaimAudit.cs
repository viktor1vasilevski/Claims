using Claims.Domain.Enums;

namespace Claims.Domain.Models;

public class ClaimAudit
{
    public int Id { get; set; }
    public string? ClaimId { get; set; }
    public DateTime Created { get; set; }
    public HttpRequestType HttpRequestType { get; set; }
}
