using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollercoin.API.Minigames
{
    public interface IMinigame
    {
        ChromiumWebBrowser WebBrowser { get; set; }
        Canvas Canvas { get; set; }
        bool GameOngoing { get; set; }
        GameResult GameResult { get; set; }
        bool GameFinished { get; set; }
        DateTime GameStarted_Date { get; set; }
        void StartGame();
        void SelectGame();
        void Solve_Tick();
        void Finalize_Game();
        void Gain_Power();
        void Restart();
        GameResult GetGameResult();
    }

    public enum GameResult
    {
        Unknown,
        Victory,
        Defeat
    }

    public enum GainPowerResult
    {
        Accepted = 0,
        Pending = 1,
        RejectedBot = 2,
        RejectedCaptcha = 3,
        Unknown = 4
    }
}
