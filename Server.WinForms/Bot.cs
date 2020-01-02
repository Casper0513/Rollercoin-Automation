using CefSharp;
using CefSharp.WinForms;
using Rollercoin.API;
using Rollercoin.API.Core;
using Rollercoin.API.Databases;
using Rollercoin.API.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.WinForms
{
    public class Bot
    {
        public API_Instance API;
        public ChromiumWebBrowser WebBrowser;
        public PictureBox Debug_PictureBox;
        public GameTickLoop GameTickLoop;
        public IMinigame CurrentGame;
        public List<BotGameLog> GameLogs;
        public DatabaseInterface DatabaseInterface;

        public Bot(API_Instance aPI, ChromiumWebBrowser webBrowser, DatabaseInterface dbInterface, PictureBox debug_PictureBox = null)
        {
            API = aPI;
            WebBrowser = webBrowser;
            Debug_PictureBox = debug_PictureBox;
            DatabaseInterface = dbInterface;
            GameLogs = new List<BotGameLog>();
        }

        public void GameTickLoop_GameFinished(object sender, GameFinishedEventArgs e)
        {
            StopGame();
            CurrentGame.Gain_Power();
            GainPowerResult result = GainPowerResult.Unknown;
            while (result == GainPowerResult.Unknown || result == GainPowerResult.Pending)
            {
                result = Get_GainPowerResult();
                Thread.Sleep(1000);
            }
            BotGameLog gameLog = new BotGameLog(DateTime.Now, CurrentGame.GameResult, CurrentGame.GetType().Name, result);
            GameLogs.Add(gameLog);
            if(!DatabaseInterface.Write(gameLog))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[BotInstance] WARNING: Failed to save the game result log. It will show up in the process memory but will not be stored permanently.");
                Console.ResetColor();
            }
            Console.WriteLine($"[BotInstance] Finished game\nGameResult => {CurrentGame.GameResult}\nGainPowerResult => {result}");
            Thread th = new Thread(new ThreadStart(() =>
            {
                Console.WriteLine("[BotInstance] Queue game start...");
                WebBrowser.Load(@"https://rollercoin.com/game/choose_game");
                Thread.Sleep(TimeSpan.FromMinutes(5));
                Console.WriteLine("[BotInstance] Queue game end...");
                Run();
            }));
            th.IsBackground = true;
            th.Start();
            DestroyGame();
        }

        public void Run()
        {
            Thread th = new Thread(new ThreadStart(() =>
            {
                while (GetGameCooldown() != 0) Thread.Sleep(1000);
                if (!ConstructGame())
                {
                    Console.WriteLine("[BotInstance] Failed to construct the game. ConstructGame() => false");
                    return;
                }
                CurrentGame.SelectGame();
                Thread.Sleep(2000);
                if (!StartGame())
                {
                    Console.WriteLine("[BotInstance] Failed to start the game. StartGame() => false");
                    return;
                }
                Console.WriteLine("[BotInstance] Started a new game.");
            }));
            th.IsBackground = true;
            th.Start();
            return;
        }

        public bool ConstructGame()
        {
            if (CurrentGame != null) return false;
            if (Debug_PictureBox != null)
                CurrentGame = new MemoryGame(WebBrowser, "#game1 > canvas", Debug_PictureBox);
            else
                CurrentGame = new MemoryGame(WebBrowser, "#game1 > canvas");
            GameTickLoop = new GameTickLoop(CurrentGame, 1600);
            return true;
        }

        public bool StartGame()
        {
            if (CurrentGame == null) return false;
            if (GameTickLoop == null) return false;
            CurrentGame.StartGame();
            GameTickLoop.GameFinished += GameTickLoop_GameFinished;
            GameTickLoop.Start();
            return true;
        }

        public bool StopGame()
        {
            if (CurrentGame == null) return false;
            if (GameTickLoop == null) return false;
            GameTickLoop.Stop();
            GameTickLoop.GameFinished -= GameTickLoop_GameFinished;
            return true;
        }

        public bool DestroyGame()
        {
            if (CurrentGame == null) return false;
            if (GameTickLoop == null) return false;
            CurrentGame = null;
            GameTickLoop.Stop();
            GameTickLoop = null;
            return true;
        }

        public GainPowerResult Get_GainPowerResult()
        {
            if (!WebBrowser.CanExecuteJavascriptInMainFrame || !WebBrowser.IsBrowserInitialized) return GainPowerResult.Unknown;
            Task<JavascriptResponse> resp = WebBrowser.EvaluateScriptAsync(Properties.Resources.gainpower_result_script);
            resp.Wait();
            return (GainPowerResult)(int)resp.Result.Result;
        }

        public int GetGameCooldown()
        {
            if (!WebBrowser.CanExecuteJavascriptInMainFrame || !WebBrowser.IsBrowserInitialized) return 1;
            Task<JavascriptResponse> resp = WebBrowser.EvaluateScriptAsync(Properties.Resources.checkcooldown_script);
            resp.Wait();
            return (int)resp.Result.Result;
        }
    }
}
