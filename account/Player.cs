using System.Collections.Generic;
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

        /// <summary>以类型为键，下挂的钱包字典</summary>
        public readonly IDictionary<short, Wallet> Wallets = new Dictionary<short, Wallet>();

        public Wallet DefaultWallet {
            get { return Wallets[109]; }
        }
    }
}