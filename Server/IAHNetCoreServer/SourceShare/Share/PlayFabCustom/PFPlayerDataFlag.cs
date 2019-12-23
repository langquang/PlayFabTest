namespace SourceShare.Share.PlayFabCustom
{
    public class PFPlayerDataFlag
    {
        public const int EMPTY      = 0;
        public const int ACCOUNT    = 1; // INTERNAL
        public const int TOURNAMENT = 1 << 1;

        public const int GROUP_INTERNAL = ACCOUNT;
    }
}