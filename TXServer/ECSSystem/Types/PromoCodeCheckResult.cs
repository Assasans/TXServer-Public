namespace TXServer.ECSSystem.Types
{
    public enum PromoCodeCheckResult : byte
    {
        VALID,
        NOT_FOUND,
        USED,
        EXPIRED,
        INVALID,
        OWNED
    }
}
