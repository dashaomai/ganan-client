using System;
using Rx.Net.Plus;

namespace Account
{
    /// <summary>
    /// 玩家的个人资料
    /// </summary>
    public class Profile
    {
        public readonly RxVar<string> Nickname = new RxVar<string>("");
        public readonly RxVar<string> Avatar = new RxVar<string>("");

        public readonly RxVar<short> Level = new RxVar<short>(0);
    }
}