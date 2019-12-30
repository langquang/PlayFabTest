#if SERVER_SIDE
using PlayFab.ServerModels;
#else
using PlayFab.ClientModels;
#endif

namespace PlayFabShare.Models
{
    public class PFProfile
    {
        private string _displayName;

        public string DisplayName
        {
            get => _displayName;
            set => _displayName = value;
        }

        public void Import(PlayerProfileModel payload)
        {
            DisplayName = payload.DisplayName;
        }
    }
}