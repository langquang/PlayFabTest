using System.Collections.Generic;
using PlayFab.ClientModels;

namespace PlayFabCustom.Models
{
    public class PlayerStatistic
    {
        public const string SERVER = "server";

        public int Server { get; set; }

        public PlayerStatistic(List<StatisticValue> payload)
        {
            foreach (var element in payload)
            {
                switch (element.StatisticName)
                {
                    case SERVER:
                        Server = element.Value;
                        break;
                }
            }
        }
    }
}