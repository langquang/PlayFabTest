using System.Collections.Generic;
#if SERVER_SIDE
using PlayFab.ServerModels;
#else
using PlayFab.ClientModels;
#endif

namespace PlayFabShare.Models
{
    public class PFStatistic
    {
        public const string SERVER = "server";
        public const string LEVEL  = "level";

        public int Server { get; set; }
        public int Level { get; set; }

        public PFStatistic()
        {
        }

        public void Import(List<StatisticValue> payload)
        {
            foreach (var element in payload)
            {
                switch (element.StatisticName)
                {
                    case SERVER:
                        Server = element.Value;
                        break;
                    case LEVEL:
                        Level = element.Value;
                        break;
                }
            }
        }
    }
}