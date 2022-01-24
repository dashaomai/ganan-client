using Godot;
using PinusClient;
using Account;
using GodotLogger;
using System.Linq;
using Newtonsoft.Json.Linq;
using Consts;
using Networking;

namespace Login
{
    public class Logon : MarginContainer
    {
        private static readonly Logger _log = LoggerHelper.GetLogger(typeof(Logon));

        private Client gameClient;
        private ApiClient apiClient;
        private Player player;

        private LineEdit nicknameInput;
        private TextureButton logonButton;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            apiClient = GetNode<ApiClient>("/root/ApiClient");
            gameClient = GetNode<Client>("/root/GameClient");
            player = GetNode<Player>("/root/Player");

            nicknameInput = GetNode<LineEdit>("HBox/Right/HBoxContainer/VBox/Panel/Logon/Input/LogonLineEdit");
            logonButton = GetNode<TextureButton>("HBox/Right/HBoxContainer/VBox/Bottom/LogonButton");

            logonButton.Connect("pressed", this, nameof(_On_Logon_Pressed));
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }

        private void _On_Logon_Pressed()
        {
            string nickname = nicknameInput.Text.Trim();

            _log.Info($"ready to logon with nickname: {nickname}");

            if (nickname != "")
            {
                var request = new { token = nickname, vcode = "ganan-dev" };

                apiClient.OnResultError += result => _log.Warn($"result error {result}");
                apiClient.OnResponseCodeError += code => _log.Warn($"response code error {code}");
                apiClient.OnLogined += _OnApiLoginResponse;
                apiClient.Login(request);
            }
        }

        private void _OnApiLoginResponse(in LoginResult result)
        {
            _log.Debug("api login success, now logon to game server");

            gameClient.Request("connector.auth.logon", result, _On_Logon_Response);

        }

        private void _On_Logon_Response(JObject result)
        {
            _log.Debug($"logoned with response: {result}");

            ResponseCode code = (ResponseCode)(int)result["code"];

            if (code == ResponseCode.SUCCESS)
            {
                JObject playerInfo = (JObject)result["player"];
                player.Id = (string)playerInfo["id"];
                player.Profile.Nickname.Value = (string)playerInfo["nickname"];
                player.Profile.Avatar.Value = (string)playerInfo["avatar"];
                player.Profile.Level.Value = (short)playerInfo["level"];

                JArray playerWallets = (JArray)playerInfo["wallets"];

                var wallets = from w in playerInfo["wallets"] select new { Id = (int)w["id"], Type = (short)w["type"], Amount = (long)w["amount"] };

                foreach (var w in wallets)
                {
                    Wallet wallet = new Wallet();
                    wallet.Id.Value = w.Id;
                    wallet.Type.Value = w.Type;
                    wallet.Amount.Value = w.Amount;

                    player.Wallets.Add(w.Type, wallet);
                }
            }
            else
            {
                _log.Warn("logon failed");
            }
        }
    }
}