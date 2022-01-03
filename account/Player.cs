using Godot;

namespace Account
{
    /// <summary>
    /// 玩家数据
    /// </summary>
    public class Player : Node
    {
        public string Id { get; set; }
        public string Vendor { get; set; }

        public readonly Profile Profile = new Profile();
        public readonly Wallet Wallet = new Wallet();
    }
}