namespace AditiKraft.Krafter.UI.Web.Client.Features.Users;

public partial class ForgotPassword(ApiCallService api, IUsersApi usersApi) : ComponentBase
{
    [Inject]
    private Radzen.ILocalizer L { get; set; } = null!;
    public ForgotPasswordRequest ForgotPasswordRequest { get; set; } = new();
    public bool IsBusy { get; set; }
    public bool MailSent { get; set; }

    private async Task SendForgotPasswordMailAsync(ForgotPasswordRequest requestInput)
    {
        IsBusy = true;
        Response response = await api.CallAsync(() => usersApi.ForgotPasswordAsync(requestInput));
        if (response is { IsError: false })
        {
            MailSent = true;
        }

        IsBusy = false;
    }
}
