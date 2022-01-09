namespace Consts
{
    public enum ResponseCode
    {
        /** 成功 */
        SUCCESS = 0,

        /** 未知失败 */
        FAIL = -1,

        /** 参数错误 */
        INVALID_PARAM = -2,

        /** 尚未登录 */
        NOT_LOGIN = -10,
        /** 帐号不存在 */
        ACCOUNT_NOT_EXISTS = -11,
    }
}