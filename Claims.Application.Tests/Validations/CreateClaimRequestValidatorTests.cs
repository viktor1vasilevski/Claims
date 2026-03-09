using Claims.Application.Requests.Claims;
using Claims.Application.Validations.Claims;
using Claims.Domain.Enums;
using FluentAssertions;

namespace Claims.Application.Tests.Validations;

public class CreateClaimRequestValidatorTests
{
    private readonly CreateClaimRequestValidator _sut = new();

    private CreateClaimRequest ValidRequest() => new()
    {
        CoverId = "c1",
        Name = "Test Claim",
        Type = ClaimType.Collision,
        DamageCost = 5000,
        Created = DateTime.UtcNow
    };

    [Fact]
    public async Task Validate_WhenRequestIsValid_ShouldPassValidation()
    {
        var result = await _sut.ValidateAsync(ValidRequest());
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WhenCoverIdIsEmpty_ShouldFailWithMessage(string coverId)
    {
        var request = ValidRequest();
        request.CoverId = coverId;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage == "CoverId is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WhenNameIsEmpty_ShouldFailWithMessage(string name)
    {
        var request = ValidRequest();
        request.Name = name;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage == "Name is required.");
    }

    [Fact]
    public async Task Validate_WhenTypeIsInvalid_ShouldFailWithMessage()
    {
        var request = ValidRequest();
        request.Type = (ClaimType)99;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage == "Claim type is invalid.");
    }

    [Fact]
    public async Task Validate_WhenDamageCostIsZero_ShouldFailWithMessage()
    {
        var request = ValidRequest();
        request.DamageCost = 0;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage == "DamageCost must be greater than 0.");
    }

    [Fact]
    public async Task Validate_WhenDamageCostIsNegative_ShouldFailWithMessage()
    {
        var request = ValidRequest();
        request.DamageCost = -1;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage == "DamageCost must be greater than 0.");
    }

    [Fact]
    public async Task Validate_WhenDamageCostExceedsLimit_ShouldFailWithMessage()
    {
        var request = ValidRequest();
        request.DamageCost = 100_001;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage == "DamageCost cannot exceed 100,000.");
    }

    [Fact]
    public async Task Validate_WhenCreatedIsEmpty_ShouldFailWithMessage()
    {
        var request = ValidRequest();
        request.Created = default;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage == "Created date is required.");
    }
}
