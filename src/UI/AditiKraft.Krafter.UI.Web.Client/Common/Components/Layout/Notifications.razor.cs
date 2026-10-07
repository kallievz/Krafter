using AditiKraft.Krafter.UI.Web.Client.Infrastructure.SignalR;

namespace AditiKraft.Krafter.UI.Web.Client.Common.Components.Layout;

public partial class Notifications(SignalRService signalRService) : IDisposable
{
    private readonly List<string> _messages = new();
    private RadzenButton _button = null!;
    private RadzenPopup _popup = null!;

    protected override Task OnInitializedAsync()
    {
        signalRService.MessageReceived += OnMessageReceived;
        return Task.CompletedTask;
    }

    private Task OnOpen() => Task.CompletedTask;

    private Task OnNotificationClickAsync() => _popup.ToggleAsync(_button.Element);

    private void OnMessageReceived(string user, string message)
    {
        string encodedMsg = $"{user}: {message}";
        _messages.Add(encodedMsg);
        InvokeAsync(StateHasChanged);
    }

    public void Dispose() => signalRService.MessageReceived -= OnMessageReceived;
}
