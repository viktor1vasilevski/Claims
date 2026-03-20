using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Claims.Application.DTOs;
using Claims.Application.Requests.Claims;
using Claims.Application.Requests.Cover;
using Claims.Domain.Enums;
using FluentAssertions;

namespace Claims.Integration.Tests.Claims;

[Collection(nameof(IntegrationTestCollection))]
public class ClaimsEndpointsTests
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    public ClaimsEndpointsTests(ClaimsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<CoverDto> CreateCoverAsync()
    {
        var request = new CreateCoverRequest
        {
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(100),
            Type = CoverType.Yacht
        };
        var response = await _client.PostAsJsonAsync("/covers", request, JsonOptions);
        return (await response.Content.ReadFromJsonAsync<CoverDto>(JsonOptions))!;
    }

    private async Task<ClaimDto> CreateClaimAsync(Guid coverId)
    {
        var request = new CreateClaimRequest
        {
            CoverId = coverId,
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 5000,
            Created = DateTime.UtcNow.Date
        };
        var response = await _client.PostAsJsonAsync("/claims", request, JsonOptions);
        return (await response.Content.ReadFromJsonAsync<ClaimDto>(JsonOptions))!;
    }

    [Fact]
    public async Task GetClaims_ShouldReturn200()
    {
        var response = await _client.GetAsync("/claims");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateClaim_WhenValid_ShouldReturn201WithClaim()
    {
        var cover = await CreateCoverAsync();
        var request = new CreateClaimRequest
        {
            CoverId = cover.Id!.Value,
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 5000,
            Created = DateTime.UtcNow.Date
        };

        var response = await _client.PostAsJsonAsync("/claims", request, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var claim = await response.Content.ReadFromJsonAsync<ClaimDto>(JsonOptions);
        claim.Should().NotBeNull();
        claim!.Id.Should().NotBeNull();
        claim.CoverId.Should().Be(cover.Id!.Value);
        claim.DamageCost.Should().Be(5000);
    }

    [Fact]
    public async Task GetClaimById_WhenExists_ShouldReturn200WithClaim()
    {
        var cover = await CreateCoverAsync();
        var created = await CreateClaimAsync(cover.Id!.Value);

        var response = await _client.GetAsync($"/claims/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var claim = await response.Content.ReadFromJsonAsync<ClaimDto>(JsonOptions);
        claim!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetClaimById_WhenNotExists_ShouldReturn404()
    {
        var response = await _client.GetAsync($"/claims/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteClaim_WhenExists_ShouldReturn204()
    {
        var cover = await CreateCoverAsync();
        var created = await CreateClaimAsync(cover.Id!.Value);

        var response = await _client.DeleteAsync($"/claims/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteClaim_WhenNotExists_ShouldReturn404()
    {
        var response = await _client.DeleteAsync($"/claims/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateClaim_WhenCoverNotFound_ShouldReturn404()
    {
        var request = new CreateClaimRequest
        {
            CoverId = Guid.NewGuid(),
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 5000,
            Created = DateTime.UtcNow.Date
        };

        var response = await _client.PostAsJsonAsync("/claims", request, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateClaim_WhenCreatedDateOutsideCoverPeriod_ShouldReturn400()
    {
        var cover = await CreateCoverAsync();
        var request = new CreateClaimRequest
        {
            CoverId = cover.Id!.Value,
            Name = "Test Claim",
            Type = ClaimType.Collision,
            DamageCost = 5000,
            Created = DateTime.UtcNow.Date.AddDays(200)
        };

        var response = await _client.PostAsJsonAsync("/claims", request, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
