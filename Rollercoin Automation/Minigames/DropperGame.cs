using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging;
using CefSharp;
using CefSharp.WinForms;

namespace Rollercoin.API.Minigames
{
    public class DropperGame : IMinigame
    {
        public ChromiumWebBrowser WebBrowser { get; set; }
        public Canvas Canvas { get; set; }
        public DateTime GameStarted_Date { get; set; }
        public bool GameFinished { get; set; }
        public bool GameOngoing { get; set; }
        public GameResult GameResult { get; set; }
        public static readonly Dictionary<DroppingCoinType, System.Drawing.Image> CONST_KNOWN_COIN_TYPES = new Dictionary<DroppingCoinType, System.Drawing.Image>()
        {
            { DroppingCoinType.Bitcoin, Properties.Resources.bitcoin_droppingcoin },
            { DroppingCoinType.Dashcoin, Properties.Resources.dashcoin_droppingcoin },
            { DroppingCoinType.Dogecoin, Properties.Resources.dogecoin_droppingcoin },
            { DroppingCoinType.Lightcoin, Properties.Resources.lightcoin_droppingcoin }
        };
        public static readonly Rectangle CONST_SCAN_REGION = new Rectangle(20, 90, 479, 110);
        // Debug features
        public PictureBox Debug_PictureBox;
        public bool Debugging_Enabled;
        // Debug features
        public Bitmap LastGameFrame;
        public bool Solver_Initialized;
        public DateTime GameStarted_Time;
        public List<DroppingCoin> Detections;
        public DropperGame(ChromiumWebBrowser webBrowser, string canvasElementSelector)
        {
            WebBrowser = webBrowser;
            Canvas = new Canvas(webBrowser, canvasElementSelector);
        }
        public DropperGame(ChromiumWebBrowser webBrowser, string canvasElementSelector, PictureBox debugPictureBox)
        {
            WebBrowser = webBrowser;
            Canvas = new Canvas(webBrowser, canvasElementSelector);
            Debug_PictureBox = debugPictureBox;
            Debugging_Enabled = true;
        }
        public void Finalize_Game()
        {
            GameFinished = true;
            GameOngoing = false;
            Solver_Initialized = false;
            if (Debugging_Enabled)
                WebBrowser.Invoke(new Action(() => Debug_PictureBox.Image = Debug_DrawDetections()));
            if (GameResult == GameResult.Victory)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[DropperGame] The minigame has been FULLY SOLVED");
            }
            else if(GameResult == GameResult.Defeat)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[DropperGame] Failed to solve the minigame");
            }
            Detections.Clear();
        }
        private System.Drawing.Image Debug_DrawDetections()
        {
            if (!Solver_Initialized) return null;
            Bitmap bitmap = GetGameFrame_Global();
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawRectangle(new Pen(new SolidBrush(Color.BlueViolet), 4), CONST_SCAN_REGION);
                foreach(DroppingCoin coin in Detections)
                {
                    SolidBrush brush = new SolidBrush(Color.White);
                    Pen pen = new Pen(brush, 2);
                    g.DrawRectangle(pen, coin.Location);
                    Font font = new Font(FontFamily.GenericMonospace, 6, FontStyle.Regular);
                    g.DrawString(coin.CoinType.ToString(), font, brush, coin.Location.X, coin.Location.Y - 7);
                }
            }
            return bitmap;
        }
        public void SelectGame()
        {
            WebBrowser.ExecuteScriptAsync(@"document.querySelector('#root > div > div.content > div > div.react-wrapper > div > div > div.choose-game-container.col-12.col-lg-8 > div > div.choose-game-body > div > div.select-game-block.col-12.col-lg-8 > div > div > div.scrollContainer > div > div:nth-child(1) > div').click()
            document.querySelector('#root > div > div.content > div > div.react-wrapper > div > div > div.choose-game-container.col-12.col-lg-8 > div > div.choose-game-body > div > div.selected-game-block.col-12.col-lg-4 > div > div.game-stats-block > div.play-game-block > button').click()");
        }
        int ticksWithoutDetection = 0; // if ticksWithoutDetection == 15 then end game
        public void Solve_Tick()
        {
            if(!GameOngoing)
            {
                Console.WriteLine("[DropperGame] Game needs to be started first.");
                return;
            }

            if(!Solver_Initialized)
            {
                Solver_Initialized = true;
                Detections = new List<DroppingCoin>();
            }

            if (ticksWithoutDetection == 15090)
                Finalize_Game();

            Thread th = new Thread(new ThreadStart(() =>
            {
                FindDroppingCoins(GetGameFrame_Global());
                if (Debugging_Enabled)
                {
                    Debug_PictureBox.Image = Debug_DrawDetections();
                }
                if (Detections.Count == 0)
                    ticksWithoutDetection++;
                else
                    ticksWithoutDetection = 0;
                Click_Coins();
            }));
            th.IsBackground = true;
            th.Start();
        }

        private void FindDroppingCoins(Bitmap bitmap)
        {
            List<Thread> threads = new List<Thread>();
            foreach(KeyValuePair<DroppingCoinType, System.Drawing.Image> coinType in CONST_KNOWN_COIN_TYPES)
            {
                Bitmap bitmapCopy = bitmap.Clone(CONST_SCAN_REGION, bitmap.PixelFormat);
                Thread th = new Thread(new ThreadStart(() =>
                {
                    Stopwatch s = new Stopwatch();
                    s.Start();
                    TemplateMatch[] coinType_matches = Bitmap_CV.FindAllNeedles(bitmapCopy, (Bitmap)coinType.Value, 0.8f);
                    s.Stop();
                    Console.WriteLine($"SCAN TIME: {s.ElapsedMilliseconds}ms");
                    bitmapCopy.Dispose();
                    foreach (TemplateMatch coinType_match in coinType_matches)
                        Detections.Add(new DroppingCoin(coinType.Key, new Rectangle(CONST_SCAN_REGION.X + coinType_match.Rectangle.X, CONST_SCAN_REGION.Y + coinType_match.Rectangle.Y
                            , coinType_match.Rectangle.Width, coinType_match.Rectangle.Height)));
                    Console.WriteLine($"[DropperGame] Detected {coinType_matches.Length} coins of type {coinType.Key}");
                }));
                th.IsBackground = true;
                threads.Add(th);
                th.Start();
            }

            bool threadsExited = false;
            while (!threadsExited)
            {
                bool exited = true;
                foreach (Thread th in threads)
                    if (th.IsAlive) exited = false;
                threadsExited = exited;
                Thread.Sleep(50);
            }
        }

        public void Click_Coins()
        {
            try
            {
                int count = 0; // limit: 3 per tick
                List<DroppingCoin> clickedCoins = new List<DroppingCoin>();
                foreach (DroppingCoin coin in Detections)
                {
                    if (count == 5) return;
                    clickedCoins.Add(coin);
                    Canvas.Invoke_MouseClick_Safe(new Point(coin.Location.X + 12, coin.Location.Y + 45), MouseButton.Left);
                    count++;
                    Thread.Sleep(25);
                }

                foreach (DroppingCoin coin in clickedCoins)
                    Detections.Remove(coin);
            }
            catch(Exception e) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"[DropperGame] ERROR: Click_Coins() => {e.Message}"); Console.ResetColor(); }
        }

        public void StartGame()
        {
            GameOngoing = true;
            Solver_Initialized = false;
            GameStarted_Date = DateTime.Now;
            Detections = new List<DroppingCoin>();
        }
        public void GetGameFrame()
        {
            LastGameFrame = GetGameFrame_Global();
        }
        public Bitmap GetGameFrame_Global()
        {
            Bitmap frame = new Bitmap(WebBrowser.Size.Width, WebBrowser.Size.Height, PixelFormat.Format24bppRgb);
            Point screenCoords = new Point();
            WebBrowser.Invoke(new Action(() => screenCoords = WebBrowser.PointToScreen(new Point(0, 0))));
            using (Graphics g = Graphics.FromImage(frame))
            {
                g.CopyFromScreen(screenCoords, new Point(0, 0), WebBrowser.Size);
            }
            return frame;
        }
        public void Gain_Power()
        {
            throw new NotImplementedException();
        }
        public void Restart()
        {
            throw new NotImplementedException();
        }

        public GameResult GetGameResult()
        {
            throw new NotImplementedException();
        }
    }

    public enum DroppingCoinType
    {
        Bitcoin,
        Dogecoin,
        Lightcoin,
        Dashcoin,
        Unknown
    }

    public class DroppingCoin
    {
        public DroppingCoinType CoinType;
        public Rectangle Location;

        public DroppingCoin(DroppingCoinType coinType, Rectangle location)
        {
            CoinType = coinType;
            Location = location;
        }
    }
}
