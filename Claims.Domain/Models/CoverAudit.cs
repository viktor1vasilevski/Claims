namespace Claims.Domain.Models;

public class CoverAudit
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? CoverId { get; set; }
    public DateTime Created { get; set; }
    public HttpRequestType HttpRequestType { get; set; }
}
