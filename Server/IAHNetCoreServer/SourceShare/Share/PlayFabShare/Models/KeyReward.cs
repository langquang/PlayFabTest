using SourceShare.Share.APIServer.Data;

namespace PlayFabShare.Models
{
    public class KeyReward
    {
        public readonly OnlineReward OnlineReward;

        public KeyReward()
        {
            OnlineReward = new OnlineReward();
        }
    }
}