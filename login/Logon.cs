using Godot;
using PinusClient;
using Account;
using GodotLogger;
using Newtonsoft.Json.Linq;
using Consts;

namespace Login
{
    public class Logon : MarginContainer
    {
        private static readonly Logger _log = LoggerHelper.GetLogger(typeof(Logon));

        private Client client;
        private Player player;

        private LineEdit nicknameInput;
        private TextureButton logonButton;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            client = GetNode<Client>("/root/Client");
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

                client.Request("connector.auth.logon", request, _On_Logon_Response);
            }
        }

        private void _On_Logon_Response(JObject result)
        {
            _log.Debug($"logoned with response: {result}");

            ResponseCode code = (ResponseCode)(int)result["code"];

            if (code == ResponseCode.SUCCESS)
            {
                JObject playerInfo = (JObject)result["player"];
                player.Id = (string)playerInfo["id"];
                JObject playerProfile = (JObject)playerInfo["profile"];
                JObject playerWallet = (JObject)playerInfo["wallet"];
                player.Profile.Nickname.Value = (string)playerProfile["nickname"];
                player.Profile.Avatar.Value = (string)playerProfile["avatar"];
                player.Profile.Level.Value = (int)playerProfile["level"];

                player.Wallet.Currency.Value = (int)playerWallet["currency"];
                player.Wallet.Coins.Value = (float)playerWallet["coins"];
            }
            else
            {
                _log.Warn("logon failed");
            }
        }
    }
}