using System;
using Account;
using Godot;
using GodotLogger;
using Newtonsoft.Json.Linq;
using PinusClient;
using PinusClient.Transporter;
using Rx.Net.Plus;

public class Bootstrap : Node
{
    private static readonly Logger _log = LoggerHelper.GetLogger(typeof(Bootstrap));

    [Export]
    private readonly string _url;

    private Label nickname;
    private Label avatar;
    private Label money;

    private Client _client;

    private Player player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _log.Debug("ready to bootstrap");

        _client = GetNode<Client>("/root/Client");
        _client.OnNetworkStatusChanged += _OnNetworkStatusChanged;
        _client.OnHandshakeCompleted += _OnHandshakeCompleted;

        _client.ConnectTo(new WebSocketParameter(_url));

        nickname = GetNode<Label>("MarginContainer/HBoxContainer/Nickname");
        avatar = GetNode<Label>("MarginContainer/HBoxContainer/Avatar");
        money = GetNode<Label>("MarginContainer/HBoxContainer/Money");

        player = GetNode<Player>("/root/Player");

        player.Profile.Nickname.Notify(nickname => this.nickname.Text = nickname);
        player.Profile.Avatar.Notify(avatar => this.avatar.Text = avatar);
        player.Wallet.Money.Notify(money => this.money.Text = money.ToString());

        player.Profile.Nickname.Value = "buddy-01";
        player.Profile.Avatar.Value = "https://www.baidu.com/";
        player.Wallet.Money.Value = 99999f;
    }

    private void _OnHandshakeCompleted(JObject obj)
    {
        _log.Debug("handshake completed.");

        var request = new {token = "buddy-01", vcode = "ganan-dev"};
        _client.Request("connector.auth.logon", request, _OnLogon);
    }

    private void _OnLogon(JObject result)
    {
        _log.Debug($"logoned with response: {result}");
    }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        player.Wallet.Money.Value -= 1f;
    }

    private void _OnNetworkStatusChanged(NetworkStatus status)
    {

    }
}
