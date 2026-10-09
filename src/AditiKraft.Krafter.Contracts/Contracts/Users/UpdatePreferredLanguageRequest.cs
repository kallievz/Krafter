using FluentValidation;

namespace AditiKraft.Krafter.Contracts.Contracts.Users;

public class UpdatePreferredLanguageRequest
{
    public string? PreferredLanguageCode { get; set; }
}

public class UpdatePreferredLanguageRequestValidator : AbstractValidator<UpdatePreferredLanguageRequest>
{
    public UpdatePreferredLanguageRequestValidator()
    {
        RuleFor(request => request.PreferredLanguageCode)
            .MaximumLength(10)
            .When(request => !string.IsNullOrWhiteSpace(request.PreferredLanguageCode));
    }
}
