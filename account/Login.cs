namespace Account
{
    public readonly struct LoginResult
    {
        public readonly string playerId;
        public readonly string sessionId;
        public readonly int vendorId;
        public readonly int codeId;

        public LoginResult(
            string playerId, string sessionId,
            int vendorId, int codeId
        )
        {
            this.playerId = playerId;
            this.sessionId = sessionId;
            this.vendorId = vendorId;
            this.codeId = codeId;
        }
    }
}