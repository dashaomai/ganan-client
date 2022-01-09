using Rx.Net.Plus;

namespace Account
{
    /// <summary>
    /// 玩家钱包数据 
    /// </summary>
    public class Wallet
    {
        public readonly RxVar<int> Currency = 0.ToRxVar();
        public readonly RxVar<float> Coins = 0f.ToRxVar();
    }
}