using Rx.Net.Plus;

namespace Account
{
    /// <summary>
    /// 玩家钱包数据 
    /// </summary>
    public class Wallet
    {
        public readonly RxVar<float> Money = 0f.ToRxVar();
    }
}