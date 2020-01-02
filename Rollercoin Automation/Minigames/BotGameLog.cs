using Rollercoin.API.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollercoin.API.Minigames
{
    public class BotGameLog
    {
        public DateTime Date;
        public GameResult GameResult;
        public string GameType;
        public GainPowerResult GainPowerResult;

        public BotGameLog(DateTime date, GameResult gameResult, string gameType, GainPowerResult gainPowerResult)
        {
            Date = date;
            GameResult = gameResult;
            GameType = gameType;
            GainPowerResult = gainPowerResult;
        }
    }
}
