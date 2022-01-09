using System;
using Rx.Net.Plus;

namespace Account
{
    /// <summary>
    /// 玩家的个人资料
    /// </summary>
    public class Profile
    {
        public readonly RxVar<string> Nickname = "".ToRxVar();
        public readonly RxVar<string> Avatar = "".ToRxVar();

        public readonly RxVar<int> Level = 0.ToRxVar();
    }
}