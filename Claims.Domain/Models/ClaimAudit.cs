namespace Claims.Domain.Models;

public class ClaimAudit
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? ClaimId { get; set; }
    public DateTime Created { get; set; }
    public HttpRequestType HttpRequestType { get; set; }
}
