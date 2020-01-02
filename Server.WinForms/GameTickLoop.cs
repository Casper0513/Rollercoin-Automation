using Rollercoin.API.Minigames;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.WinForms
{
    public class GameFinishedEventArgs : EventArgs
    {
        public IMinigame Game;
        public GameResult Result;

        public GameFinishedEventArgs(IMinigame game, GameResult result)
        {
            Game = game;
            Result = result;
        }
    }
    public class GameTickLoop
    {
        public IMinigame Game;
        public int DelayMs;
        public long LastTickMs;
        public Thread LoopThread;
        public bool AbortLoop;

        public delegate void GameFinishedEventHandler(object sender, GameFinishedEventArgs e);
        public event GameFinishedEventHandler GameFinished;

        public GameTickLoop(IMinigame game, int delayMs)
        {
            Game = game;
            DelayMs = delayMs;
        }

        public void Start()
        {
            if (LoopThread == null)
                CreateLoopThread();
            if (LoopThread.IsAlive) return;
            LoopThread.Start();
            AbortLoop = false;
        }

        public void Stop()
        {
            if (LoopThread == null) return;
            if (LoopThread.IsAlive) AbortLoop = true;
            LoopThread = null;
        }

        public void CreateLoopThread()
        {
            LoopThread = new Thread(new ThreadStart(() => 
            {
                while(!AbortLoop)
                {
                    if(Game.GameOngoing)
                    {
                        Stopwatch s = new Stopwatch();
                        s.Start();
                        Game.Solve_Tick();
                        s.Stop();
                        LastTickMs = s.ElapsedMilliseconds;
                        Console.WriteLine($"Process game tick end: {LastTickMs}ms");
                    }

                    if(Game.GameFinished)
                    {
                        Console.WriteLine($"Game finished. Stopping tick loop and firing events.");
                        GameFinished?.Invoke(this, new GameFinishedEventArgs(Game, Game.GameResult));
                        Stop();
                    }
                        
                    Thread.Sleep(DelayMs);
                }
            }));
            LoopThread.IsBackground = true;
        }
    }
}
