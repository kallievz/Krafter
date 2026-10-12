using AditiKraft.Krafter.Contracts.Contracts.Auth;
using AditiKraft.Krafter.UI.Web.Client.Infrastructure.Auth;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Auth;

public partial class SelfRegister(
    IAuthApiService authApiService,
    NavigationManager navigationManager,
    NotificationService notificationService
) : ComponentBase
{
    [Inject]
    private Radzen.ILocalizer L { get; set; } = null!;
    [CascadingParameter] public bool IsMobileDevice { get; set; }

    public bool IsBusy { get; set; }

    public SelfRegisterRequest SelfRegisterRequestInput { get; set; } = new();

    private async Task SelfRegisterAsync(SelfRegisterRequest request)
    {
        IsBusy = true;
        Response response = await authApiService.SelfRegisterAsync(request, CancellationToken.None);
        IsBusy = false;

        if (response.IsError)
        {
            notificationService.Notify(NotificationSeverity.Error, "Self Register", response.Message ?? "Registration failed.");
            return;
        }

        notificationService.Notify(NotificationSeverity.Success, "Self Register", response.Message ?? "Registration successful.");
        navigationManager.NavigateTo("/login");
    }
}
