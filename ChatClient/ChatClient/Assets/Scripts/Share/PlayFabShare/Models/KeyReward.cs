using Share.APIServer.Data.Entities;

namespace Share.PlayFabShare.Models
{
    public class KeyReward
    {
        public OnlineReward OnlineReward;

        public KeyReward()
        {
            OnlineReward = new OnlineReward();
        }
    }
}