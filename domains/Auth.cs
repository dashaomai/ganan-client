using Account;
using Consts;

namespace Auth
{
    readonly public struct LogonResponse {
        public readonly ResponseCode code;
        public readonly Player player;
    }
}