using FluentValidation;

namespace AditiKraft.Krafter.Contracts.Contracts.Auth;

public class SelfRegisterRequest
{
    public string? TenantId { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
}

public class SelfRegisterRequestValidator : AbstractValidator<SelfRegisterRequest>
{
    public SelfRegisterRequestValidator()
    {
        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

        RuleFor(p => p.UserName)
            .NotEmpty().WithMessage("User name is required")
            .MaximumLength(100).WithMessage("User name cannot exceed 100 characters");

        RuleFor(p => p.PhoneNumber)
            .MaximumLength(20).When(p => string.IsNullOrWhiteSpace(p.PhoneNumber) == false)
            .WithMessage("Phone number cannot exceed 20 characters");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Password is required");

        RuleFor(p => p.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required")
            .Equal(p => p.Password).WithMessage("Password and confirm password must match");
    }
}
