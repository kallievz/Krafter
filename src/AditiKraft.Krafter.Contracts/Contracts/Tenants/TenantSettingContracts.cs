using FluentValidation;

namespace AditiKraft.Krafter.Contracts.Contracts.Tenants;

public sealed class TenantSettingDto
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsSensitive { get; set; }
}

public sealed class UpsertTenantSettingRequest
{
    public string? Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public bool IsSensitive { get; set; }
}

public sealed class UpsertTenantSettingRequestValidator : AbstractValidator<UpsertTenantSettingRequest>
{
    public UpsertTenantSettingRequestValidator()
    {
        RuleFor(request => request.Key)
            .NotEmpty().WithMessage("Setting key is required")
            .MaximumLength(256).WithMessage("Setting key cannot be longer than 256 characters");

        RuleFor(request => request.Value)
            .NotNull().WithMessage("Setting value is required")
            .MaximumLength(4096).WithMessage("Setting value cannot be longer than 4096 characters");
    }
}
