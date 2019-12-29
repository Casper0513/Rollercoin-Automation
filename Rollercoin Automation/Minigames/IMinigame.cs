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
        bool GameStarted { get; set; }
        bool GameSolved { get; set; }
        void StartGame();
        void SelectGame();
        void Solve_Tick();
        void Finalize_Game();
    }
}
