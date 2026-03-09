using Claims.Application.Requests.Cover;
using Claims.Application.Validations.Cover;
using Claims.Domain.Enums;
using FluentAssertions;

namespace Claims.Application.Tests.Validations;

public class CreateCoverRequestValidatorTests
{
    private readonly CreateCoverRequestValidator _sut = new();

    private CreateCoverRequest ValidRequest() => new()
    {
        StartDate = DateTime.UtcNow.Date,
        EndDate = DateTime.UtcNow.Date.AddDays(30),
        Type = CoverType.Yacht
    };

    [Fact]
    public async Task Validate_WhenRequestIsValid_ShouldPassValidation()
    {
        var result = await _sut.ValidateAsync(ValidRequest());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WhenTypeIsInvalid_ShouldFailWithMessage()
    {
        var request = ValidRequest();
        request.Type = (CoverType)99;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage == "Cover type is invalid.");
    }

    [Fact]
    public async Task Validate_WhenStartDateIsInThePast_ShouldFailWithMessage()
    {
        var request = ValidRequest();
        request.StartDate = DateTime.UtcNow.Date.AddDays(-1);

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage == "StartDate cannot be in the past.");
    }

    [Fact]
    public async Task Validate_WhenEndDateIsBeforeStartDate_ShouldFailWithMessage()
    {
        var request = ValidRequest();
        request.EndDate = request.StartDate.AddDays(-1);

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage == "EndDate must be after StartDate.");
    }

    [Fact]
    public async Task Validate_WhenInsurancePeriodExceedsOneYear_ShouldFailWithMessage()
    {
        var request = ValidRequest();
        request.EndDate = request.StartDate.AddDays(366);

        var result = await _sut.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.ErrorMessage == "Insurance period cannot exceed 1 year.");
    }
}
