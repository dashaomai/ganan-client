using Rx.Net.Plus;

namespace Account
{
    /// <summary>
    /// 玩家钱包数据 
    /// </summary>
    public class Wallet
    {
        public readonly RxVar<long> Id = new RxVar<long>(0L);
        public readonly RxVar<short> Type = new RxVar<short>(0);
        public readonly RxVar<long> Amount = new RxVar<long>(0L);
    }
}