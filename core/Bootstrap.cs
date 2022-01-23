using Godot;
using GodotLogger;
using Networking;
using Newtonsoft.Json.Linq;
using PinusClient;
using PinusClient.Transporter;

public class Bootstrap : Node
{
    private static readonly Logger _log = LoggerHelper.GetLogger(typeof(Bootstrap));

    [Export]
    private readonly string _url;

    private ApiClient _apiClient;
    private Client _gameClient;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _log.Debug("ready to bootstrap");

        _apiClient = GetNode<ApiClient>("/root/ApiClient");
        _apiClient.BaseUrl = "http://localhost:13000";
        _apiClient.OnResultError += result => _log.Warn($"result error {result}");
        _apiClient.OnResponseCodeError += code => _log.Warn($"response code error {code}");
        _apiClient.Login(new { token = "buddy", vcode = "ganan-dev" });

        _gameClient = GetNode<Client>("/root/GameClient");
        _gameClient.OnHandshakeCompleted += _OnHandshakeCompleted;

        // 直接让客户端连接到服务器
        _gameClient.ConnectTo(new WebSocketParameter(_url));
    }

    private void _OnLogin(JObject loginResponse)
    {

    }

    private void _OnHandshakeCompleted(JObject obj)
    {
        _log.Debug("handshake completed.");

        // 握手完成，视为服务器连接已就绪
        var logonScene = ResourceLoader.Load<PackedScene>("res://login/logon.tscn");
        GetTree().ChangeSceneTo(logonScene);
    }
}
