using AditiKraft.Krafter.UI.Web.Client.Features.Localization;
using FluentValidation;

namespace AditiKraft.Krafter.UI.Web.Client.Common.Components.Dialogs;

public class DeleteRequestInputValidator : AbstractValidator<DeleteRequestInput>
{
    public DeleteRequestInputValidator(ILocalizer localizer)
    {
        RuleFor(x => x.DeleteReason)
            .NotEmpty().WithMessage(localizer.Localize("Notification_DeleteReason", "Delete Reason is required"))
            .Length(5, 500).WithMessage(localizer.Localize("Notification_DeleteReasonLength", "Delete Reason must be between 5 and 500 characters"));
    }
}
