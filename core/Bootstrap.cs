using Account;
using Godot;
using GodotLogger;
using Newtonsoft.Json.Linq;
using PinusClient;
using PinusClient.Transporter;

public class Bootstrap : Node
{
    private static readonly Logger _log = LoggerHelper.GetLogger(typeof(Bootstrap));

    [Export]
    private readonly string _url;

    private Client _client;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _log.Debug("ready to bootstrap");

        _client = GetNode<Client>("/root/Client");
        _client.OnHandshakeCompleted += _OnHandshakeCompleted;

        // 直接让客户端连接到服务器
        _client.ConnectTo(new WebSocketParameter(_url));
    }

    private void _OnHandshakeCompleted(JObject obj)
    {
        _log.Debug("handshake completed.");

        // 握手完成，视为服务器连接已就绪
        var logonScene = ResourceLoader.Load<PackedScene>("res://login/logon.tscn");
        GetTree().ChangeSceneTo(logonScene);
    }
}
