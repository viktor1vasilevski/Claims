using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Claims.Application.DTOs;
using Claims.Application.Requests.Claims;
using Claims.Application.Requests.Cover;
using Claims.Domain.Enums;
using FluentAssertions;

namespace Claims.Integration.Tests.Covers;

[Collection(nameof(IntegrationTestCollection))]
public class CoversEndpointsTests
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    public CoversEndpointsTests(ClaimsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private CreateCoverRequest ValidCoverRequest() => new()
    {
        StartDate = DateTime.UtcNow.Date,
        EndDate = DateTime.UtcNow.Date.AddDays(100),
        Type = CoverType.Yacht
    };

    private async Task<CoverDto> CreateCoverAsync()
    {
        var response = await _client.PostAsJsonAsync("/covers", ValidCoverRequest(), JsonOptions);
        return (await response.Content.ReadFromJsonAsync<CoverDto>(JsonOptions))!;
    }

    [Fact]
    public async Task GetCovers_ShouldReturn200()
    {
        var response = await _client.GetAsync("/covers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateCover_WhenValid_ShouldReturn201WithCover()
    {
        var response = await _client.PostAsJsonAsync("/covers", ValidCoverRequest(), JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var cover = await response.Content.ReadFromJsonAsync<CoverDto>(JsonOptions);
        cover.Should().NotBeNull();
        cover!.Id.Should().NotBeNullOrEmpty();
        cover.Type.Should().Be(CoverType.Yacht);
        cover.Premium.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetCoverById_WhenExists_ShouldReturn200WithCover()
    {
        var created = await CreateCoverAsync();

        var response = await _client.GetAsync($"/covers/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cover = await response.Content.ReadFromJsonAsync<CoverDto>(JsonOptions);
        cover!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetCoverById_WhenNotExists_ShouldReturn404()
    {
        var response = await _client.GetAsync($"/covers/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCover_WhenExists_ShouldReturn204()
    {
        var created = await CreateCoverAsync();

        var response = await _client.DeleteAsync($"/covers/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCover_WhenNotExists_ShouldReturn404()
    {
        var response = await _client.DeleteAsync($"/covers/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateCover_WhenStartDateInPast_ShouldReturn400()
    {
        var request = ValidCoverRequest();
        request.StartDate = DateTime.UtcNow.Date.AddDays(-1);

        var response = await _client.PostAsJsonAsync("/covers", request, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCover_WhenPeriodExceedsOneYear_ShouldReturn400()
    {
        var request = ValidCoverRequest();
        request.EndDate = request.StartDate.AddDays(366);

        var response = await _client.PostAsJsonAsync("/covers", request, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCover_WhenHasActiveClaims_ShouldReturn409()
    {
        var cover = await CreateCoverAsync();

        var claimRequest = new CreateClaimRequest
        {
            CoverId = cover.Id!,
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 5000,
            Created = DateTime.UtcNow.Date
        };
        await _client.PostAsJsonAsync("/claims", claimRequest, JsonOptions);

        var response = await _client.DeleteAsync($"/covers/{cover.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ComputePremium_WhenValid_ShouldReturn200WithAmount()
    {
        var start = DateTime.UtcNow.Date;
        var end = start.AddDays(30);
        var query = $"?StartDate={start:yyyy-MM-dd}&EndDate={end:yyyy-MM-dd}&Type=Yacht";

        var response = await _client.GetAsync($"/covers/premium{query}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var premium = await response.Content.ReadFromJsonAsync<decimal>();
        premium.Should().BeGreaterThan(0);
    }
}
