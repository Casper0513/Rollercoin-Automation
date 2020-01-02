using AForge.Imaging;
using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rollercoin.API.Minigames
{
    public class MemoryGame : IMinigame
    {
        public ChromiumWebBrowser WebBrowser { get; set; }
        public Canvas Canvas { get; set; }
        public DateTime GameStarted_Date { get; set; }
        public bool GameFinished { get; set; }
        public bool GameOngoing { get; set; }
        public GameResult GameResult { get; set; }
        public MemoryGame(ChromiumWebBrowser webBrowser, string canvasElementSelector)
        {
            WebBrowser = webBrowser;
            Canvas = new Canvas(webBrowser, canvasElementSelector);
            GridCards = new List<MemoryCard>();
        }
        public MemoryGame(ChromiumWebBrowser webBrowser, string canvasElementSelector, PictureBox debugPictureBox)
        {
            WebBrowser = webBrowser;
            Canvas = new Canvas(webBrowser, canvasElementSelector);
            GridCards = new List<MemoryCard>();
            Debug_PictureBox = debugPictureBox;
            Debugging_Enabled = true;
        }
        public void Gain_Power()
        {
            Rectangle button = Find_GainPowerButton();
            if (button == Rectangle.Empty) return;
            Canvas.Invoke_MouseClick_Safe(button.Location, MouseButton.Left);
        }
        public void Restart()
        {
            Rectangle button = Find_RestartButton();
            if (button == Rectangle.Empty) return;
            Canvas.Invoke_MouseClick_Safe(button.Location, MouseButton.Left);
        }
        public void Press_Start_Button()
        {
            Rectangle button = Find_StartButton();
            if (button == Rectangle.Empty) return;
            Canvas.Invoke_MouseClick_Safe(button.Location, MouseButton.Left);
        }
        public void Finalize_Game()
        {
            // Finalize everything and free resources
            if (GameFinished) return;
            GameResult = GetGameResult();
            GameFinished = true;
            GameOngoing = false;
            GridCards.Clear();
            Solver_Initialized = false;
            if (Debugging_Enabled)
                WebBrowser.Invoke(new Action(() => Debug_PictureBox.Image = Debug_DrawGrid()));
            if (GameResult == GameResult.Victory)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[MemoryGame] The minigame has been FULLY SOLVED");
            }
            else if (GameResult == GameResult.Defeat)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[MemoryGame] Failed to solve the minigame");
            }
            Console.ResetColor();
        }

        public static readonly Dictionary<RackType, Point[]> CONST_RACK_TYPE_IDENTIFIERS = new Dictionary<RackType, Point[]>()
        {
            { RackType.Rack_3x4, new[] { new Point(207, 40), new Point(305, 40) } }, // Uncertain
            { RackType.Rack_4x4, new[] { new Point(86, 40), new Point(427, 40) }  }, // Certain
            { RackType.Rack_5x4, new[] { new Point(58, 40), new Point(461, 40) } } // Uncertain
        };
        public static readonly Dictionary<RackType, Point> CONST_GRID_LEFT_OFFSET = new Dictionary<RackType, Point>()
        {
            { RackType.Rack_3x4, new Point(141, 57) }, // Unknown
            { RackType.Rack_4x4, new Point(99, 57) }, // Known
            { RackType.Rack_5x4, new Point(57, 57) } // Unknown
        };
        public static readonly Dictionary<RackType, Size> CONST_GRID_CARD_SIZE = new Dictionary<RackType, Size>()
        {
            { RackType.Rack_3x4, new Size(64, 66) }, // Certain
            { RackType.Rack_4x4, new Size(64, 66) }, // Certain
            { RackType.Rack_5x4, new Size(64, 66) } // Certain
        };
        public static readonly Dictionary<RackType, Point> CONST_GRID_MARGINS = new Dictionary<RackType, Point>()
        {
            { RackType.Rack_3x4, new Point(20, 20) }, // Uncertain
            { RackType.Rack_4x4, new Point(20, 20) }, // Certain
            { RackType.Rack_5x4, new Point(20, 20) } // Uncertain
        };
        public static readonly Dictionary<CurrencyType, System.Drawing.Image> CONST_KNOWN_CARD_TYPES = new Dictionary<CurrencyType, System.Drawing.Image>()
        {
            { CurrencyType.Binance, Properties.Resources.binance },
            { CurrencyType.Bitcoin, Properties.Resources.bitcoin },
            { CurrencyType.Eos, Properties.Resources.eos },
            { CurrencyType.Ethereum, Properties.Resources.ethereum },
            { CurrencyType.Lightcoin, Properties.Resources.lightcoin },
            { CurrencyType.Monero, Properties.Resources.monero },
            { CurrencyType.Ripple, Properties.Resources.ripple },
            { CurrencyType.Rollercoin, Properties.Resources.rollercoin },
            { CurrencyType.Tether, Properties.Resources.tether },
            { CurrencyType.Stellar, Properties.Resources.stellar },
            { CurrencyType.CardBackSide, Properties.Resources.card_back_side }
        };
        public static readonly Rectangle CONST_VICTORY_SCAN_REGION = new Rectangle(198, 75, 123, 71);
        public static readonly Rectangle CONST_DEFEAT_SCAN_REGION = new Rectangle(188, 68, 142, 78);
        // Debug features
        public PictureBox Debug_PictureBox;
        public bool Debugging_Enabled;
        // Debug features
        public List<MemoryCard> GridCards;
        public RackType Game_RackType;
        public bool Solver_Initialized;
        public void StartGame()
        {
            Press_Start_Button();
            Thread th = new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(4000);
                GameOngoing = true;
                Solver_Initialized = false;
                GameStarted_Date = DateTime.Now;
            }));
            th.IsBackground = true;
            th.Start();
        }
        public void SelectGame()
        {
            WebBrowser.ExecuteScriptAsync(@"document.querySelector('#root > div > div.content > div > div.react-wrapper > div > div > div.choose-game-container.col-12.col-lg-8 > div > div.choose-game-body > div > div.select-game-block.col-12.col-lg-8 > div > div > div.scrollContainer > div > div:nth-child(8) > div').click()
            document.querySelector('#root > div > div.content > div > div.react-wrapper > div > div > div.choose-game-container.col-12.col-lg-8 > div > div.choose-game-body > div > div.selected-game-block.col-12.col-lg-4 > div > div.game-stats-block > div.play-game-block > button').click()");
        }
        int cardGrid_offset = 0;
        int scan_attempt = 0;
        Tuple<MemoryCard, MemoryCard> SelectedCards;
        public void Solve_Tick()
        {
            if (!GameOngoing)
            {
                Console.WriteLine("[MemoryGame] Game needs to be started first.");
                return;
            }

            if (!Solver_Initialized)
            {
                Identify_RackType();
                Compute_Rack_Grid();
                Solver_Initialized = true;
                SelectedCards = new Tuple<MemoryCard, MemoryCard>(null, null);
                cardGrid_offset = 0;
                scan_attempt = 0;
            }

            if (Debugging_Enabled)
            {
                Debug_PictureBox.Image = Debug_DrawGrid();
            }


            if (GridCards.Where(c => !c.Solved).Count() == 0)
            {
                Finalize_Game();
            }

            if ((DateTime.Now - GameStarted_Date).TotalMinutes > 1)
            {
                Finalize_Game();
            }

            Thread gameStateUpdateTh = new Thread(new ThreadStart(() =>
            {
                if (GetGameResult() != GameResult.Unknown)
                    Finalize_Game();
            }));
            gameStateUpdateTh.IsBackground = true;
            gameStateUpdateTh.Start();

            if (SelectedCardsCount() == 2)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                SelectedCards = new Tuple<MemoryCard, MemoryCard>(null, null);
                Console.WriteLine("[MemoryGame] SelectedCardsCount was 2, clearing selected card buffer");
                Console.WriteLine($"[MemoryCard] SelectedCardsCount: {SelectedCardsCount()}");
                Console.ResetColor();
            }

            if (GridCards.Skip(cardGrid_offset).Count() == 0)
            {
                // Solve remaining cards
                if (GridCards.Where(c => !c.Solved && c.CurrencyType == CurrencyType.Unknown).Count() > 0)
                {
                    foreach (MemoryCard card in GridCards.Where(c => !c.Solved && c.CurrencyType == CurrencyType.Unknown))
                    {
                        card.CurrencyType = RecognizeCardType(GetCardImage(card, true));
                        if (GridCards.Where(c => !c.Solved && c.Pair == null && c.CurrencyType == card.CurrencyType).Count() > 0)
                        {
                            MemoryCard cardPair = GridCards.First(c => !c.Solved && c.Pair == null && c.CurrencyType == card.CurrencyType);
                            card.Pair = cardPair;
                            cardPair.Pair = card;
                        }

                        break;
                    }
                    return;
                }


                if (SelectedCardsCount() == 1)
                {
                    MemoryCard card = SelectedCards.Item1;
                    if(card.Solved && card.Pair.Solved)
                    {
                        SelectedCards = new Tuple<MemoryCard, MemoryCard>(null, null);
                        return;
                    }
                    CurrencyType scannedType = RecognizeCardType(GetCardImage(card.Pair, true));
                    if (scannedType == CurrencyType.Unknown)
                    {
                        CurrencyType pairType = RecognizeCardType(GetCardImage(card, true));
                        if (pairType == CurrencyType.Unknown)
                        {
                            card.Solve();
                            return;
                        }
                    }
                    if (scannedType == card.Pair.CurrencyType)
                    {
                        card.Solve();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[MemoryCard] Card pair has been SOLVED!");
                        Console.ResetColor();
                        SelectedCards = new Tuple<MemoryCard, MemoryCard>(card, card.Pair);
                    }
                }
                else if (SelectedCardsCount() == 0)
                {
                    foreach (MemoryCard card in GridCards.Where(c => !c.Solved && c.Pair != null))
                    {
                        CurrencyType scannedType;
                        scannedType = RecognizeCardType(GetCardImage(card, true));
                        if(scannedType == CurrencyType.Unknown && card.__EverScannedBackSide)
                        {
                            CurrencyType pairType = RecognizeCardType(GetCardImage(card.Pair, true));
                            if(pairType == CurrencyType.Unknown)
                            {
                                card.Solve();
                                return;
                            }
                        }
                        Console.WriteLine($"[MemoryGame] scannedType = {scannedType}");
                        if (scannedType != card.CurrencyType) return;
                        if (scannedType == CurrencyType.CardBackSide)
                        {
                            card.__EverScannedBackSide = true;
                            return;
                        }
                        Thread.Sleep(50);
                        Application.DoEvents();
                        SelectedCards = new Tuple<MemoryCard, MemoryCard>(card, null);
                        scannedType = RecognizeCardType(GetCardImage(card.Pair, true));
                        Console.WriteLine($"[MemoryGame] scannedType = {scannedType}");
                        if (scannedType != card.Pair.CurrencyType) return;
                        if (scannedType == CurrencyType.CardBackSide)
                        {
                            card.__EverScannedBackSide = true;
                            return;
                        }
                        card.Solve();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[MemoryCard] Card pair has been SOLVED!");
                        Console.ResetColor();
                        SelectedCards = new Tuple<MemoryCard, MemoryCard>(card, card.Pair);
                        break;
                    }
                }

                return;
            }

            foreach (MemoryCard card in GridCards.Skip(cardGrid_offset))
            {
                cardGrid_offset++;
                Select_Card(card);
                Thread th = new Thread(new ThreadStart(() =>
                {
                    Thread.Sleep(450);
                    Bitmap cardImage = GetCardImage(card, false);
                    Console.WriteLine("[MemoryCard] RecognizeCardType: Start!");
                    CurrencyType currencyType = RecognizeCardType(cardImage);
                    if (currencyType == CurrencyType.Unknown || currencyType == CurrencyType.CardBackSide)
                    {
                        cardImage.Save($"unknown-{Guid.NewGuid()}.png");
                        Console.WriteLine("[MemoryCard] Card was not recognized.");
                        Thread.Sleep(50);
                        Application.DoEvents();
                        if (scan_attempt == 10)
                        {
                            Console.WriteLine("[MemoryCard] Scan attempt = 10; skipping");
                            scan_attempt = 0;
                            return;
                        }

                        scan_attempt++;
                        Console.WriteLine("[MemoryCard] Queued up a new scan attempt.");
                        cardGrid_offset--;
                        return;
                    }
                    if(currencyType == CurrencyType.CardBackSide)
                        Console.WriteLine("CARDBACKSIDE AAAAAA");
                    card.CurrencyType = currencyType;
                    if (SelectedCards.Item1 == null)
                    {
                        SelectedCards = new Tuple<MemoryCard, MemoryCard>(card, null);
                    }
                    else if (SelectedCards.Item2 == null)
                    {
                        SelectedCards = new Tuple<MemoryCard, MemoryCard>(SelectedCards.Item1, card);
                    }
                    else
                    {
                        if (SelectedCards.Item1 == SelectedCards.Item2.Pair)
                        {
                            card.Solve();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[MemoryCard] Card pair has been SOLVED!");
                            Console.ResetColor();
                        }
                    }
                    if (GridCards.Where(c => c.CurrencyType == card.CurrencyType && c.GridLocation != card.GridLocation).Count() != 0)
                    {
                        MemoryCard card2 = GridCards.First(c => c.CurrencyType == card.CurrencyType && c.GridLocation != card.GridLocation);
                        card.Pair = card2;
                        card2.Pair = card;
                        Console.WriteLine($"[MemoryCard] Found a pair for card ({card.GridLocation}): Card of type {card.Pair.CurrencyType}, ({card.Pair.GridLocation})");

                        if (SelectedCardsCount() == 1)
                        {
                            if(card.Pair.CurrencyType == RecognizeCardType(GetCardImage(card.Pair, true)))
                            {
                                card.Solve();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"[MemoryCard] Card pair has been SOLVED!");
                                Console.ResetColor();
                                SelectedCards = new Tuple<MemoryCard, MemoryCard>(card, card.Pair);
                                return;
                            }
                            cardGrid_offset--;
                        }
                        else if (SelectedCardsCount() == 2)
                        {
                            if (SelectedCards.Item1 == SelectedCards.Item2.Pair)
                            {
                                card.Solve();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"[MemoryCard] Card pair has been SOLVED!");
                                Console.ResetColor();
                            }
                        }

                    }

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"[MemoryCard] SelectedCardsCount: {SelectedCardsCount()}");
                    Console.ResetColor();
                }));
                th.IsBackground = true;
                th.Start();
                break;
            }
        }
        public int SelectedCardsCount()
        {
            int i = 0;
            if (SelectedCards.Item1 != null) i++;
            if (SelectedCards.Item2 != null) i++;
            return i;
        }
        public CurrencyType RecognizeCardType(Bitmap cardImage) // cardImage = haystack
        {
            CurrencyType currencyType = CurrencyType.Unknown;
            foreach (KeyValuePair<CurrencyType, System.Drawing.Image> kvp in CONST_KNOWN_CARD_TYPES)
            {
                if (Bitmap_CV.HaystackContainsNeedle(cardImage, (Bitmap)kvp.Value))
                {
                    currencyType = kvp.Key;
                    break;
                }
            }
            Console.WriteLine($"[MemoryGame] RecognizeCardType: Recognized card as {currencyType}");
            return currencyType;
        }
        public Bitmap Debug_DrawGrid()
        {
            if (!Solver_Initialized) return null;
            Bitmap bitmap = GetGameFrame_Global();
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                foreach (MemoryCard card in GridCards)
                {
                    int rectangleX = CONST_GRID_LEFT_OFFSET[Game_RackType].X + (CONST_GRID_CARD_SIZE[Game_RackType].Width * card.GridLocation.X)
                        + (CONST_GRID_MARGINS[Game_RackType].X * card.GridLocation.X);
                    int rectangleY = CONST_GRID_LEFT_OFFSET[Game_RackType].Y + (CONST_GRID_CARD_SIZE[Game_RackType].Height * card.GridLocation.Y)
                        + (CONST_GRID_MARGINS[Game_RackType].Y * card.GridLocation.Y);
                    int rectangleWidth = CONST_GRID_CARD_SIZE[Game_RackType].Width;
                    int rectangleHeight = CONST_GRID_CARD_SIZE[Game_RackType].Height;
                    SolidBrush brush = new SolidBrush(Color.Red);
                    if (card.CurrencyType != CurrencyType.Unknown)
                        brush.Color = Color.Orange;
                    if (card.Pair != null)
                        brush.Color = Color.DeepSkyBlue;
                    if (card.Solved)
                        brush.Color = Color.Green;
                    Pen pen = new Pen(brush, 5);

                    g.DrawRectangle(pen, new Rectangle(rectangleX, rectangleY, rectangleWidth, rectangleHeight));
                    Font font = new Font(FontFamily.GenericMonospace, 8, FontStyle.Regular);
                    g.DrawString(card.CurrencyType.ToString(), font, brush, rectangleX, rectangleY);
                }
            }
            return bitmap;
        }
        public void Select_Card(Point gridLocation)
        {
            int rectangleX = CONST_GRID_LEFT_OFFSET[Game_RackType].X + (CONST_GRID_CARD_SIZE[Game_RackType].Width * gridLocation.X)
                        + (CONST_GRID_MARGINS[Game_RackType].X * gridLocation.X);
            int rectangleY = CONST_GRID_LEFT_OFFSET[Game_RackType].Y + (CONST_GRID_CARD_SIZE[Game_RackType].Height * gridLocation.Y)
                + (CONST_GRID_MARGINS[Game_RackType].Y * gridLocation.Y);

            Canvas.Invoke_MouseClick_Safe(new Point(rectangleX + 16, rectangleY + 16), MouseButton.Left);
        }
        public void Select_Card(MemoryCard card)
        {
            int rectangleX = CONST_GRID_LEFT_OFFSET[Game_RackType].X + (CONST_GRID_CARD_SIZE[Game_RackType].Width * card.GridLocation.X)
                        + (CONST_GRID_MARGINS[Game_RackType].X * card.GridLocation.X);
            int rectangleY = CONST_GRID_LEFT_OFFSET[Game_RackType].Y + (CONST_GRID_CARD_SIZE[Game_RackType].Height * card.GridLocation.Y)
                + (CONST_GRID_MARGINS[Game_RackType].Y * card.GridLocation.Y);

            Canvas.Invoke_MouseClick_Safe(new Point(rectangleX + 16, rectangleY + 16), MouseButton.Left);
        }
        public Bitmap GetCardImage(MemoryCard card, bool handleSelectCard)
        {
            if (handleSelectCard)
            {
                Select_Card(card);
                Thread.Sleep(450);
            }

            int rectangleX = CONST_GRID_LEFT_OFFSET[Game_RackType].X + (CONST_GRID_CARD_SIZE[Game_RackType].Width * card.GridLocation.X)
                        + (CONST_GRID_MARGINS[Game_RackType].X * card.GridLocation.X);
            int rectangleY = CONST_GRID_LEFT_OFFSET[Game_RackType].Y + (CONST_GRID_CARD_SIZE[Game_RackType].Height * card.GridLocation.Y)
                + (CONST_GRID_MARGINS[Game_RackType].Y * card.GridLocation.Y);
            int rectangleWidth = CONST_GRID_CARD_SIZE[Game_RackType].Width;
            int rectangleHeight = CONST_GRID_CARD_SIZE[Game_RackType].Height;
            Rectangle cardRect = new Rectangle(rectangleX, rectangleY, rectangleWidth, rectangleHeight);
            Bitmap gameFrame = GetGameFrame_Global();
            Bitmap cardImage = new Bitmap(rectangleWidth, rectangleHeight, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(cardImage))
            {
                g.DrawImage(gameFrame, new Rectangle(0, 0, rectangleWidth, rectangleHeight), rectangleX, rectangleY, rectangleWidth, rectangleHeight,
                    GraphicsUnit.Pixel, new ImageAttributes());
            }
            return cardImage;
        }
        private void Compute_Rack_Grid()
        {
            List<MemoryCard> grid = new List<MemoryCard>();
            int x = 0;
            int y = 0;
            if (Game_RackType == RackType.Rack_3x4) { x = 3; y = 4; }
            else if (Game_RackType == RackType.Rack_4x4) { x = 4; y = 4; }
            else if (Game_RackType == RackType.Rack_5x4) { x = 5; y = 4; }
            // if Game_RackType == RackType.Unknown then you're doing something... wrong.... buddyyyyyyyy
            for (int iy = 0; iy < y; iy++)
            {
                for (int ix = 0; ix < x; ix++)
                {
                    MemoryCard card = new MemoryCard(CurrencyType.Unknown, null, new Point(ix, iy));
                    grid.Add(card);
                }
            }

            GridCards = grid;
        }
        public void Identify_RackType()
        {
            // 5x4 rack test
            Color rack_color = Color.FromArgb(82, 85, 102);
            Color l_color;
            Color r_color;
            Bitmap frame = GetGameFrame_Global();
            l_color = frame.GetPixel(CONST_RACK_TYPE_IDENTIFIERS[RackType.Rack_5x4][0].X, CONST_RACK_TYPE_IDENTIFIERS[RackType.Rack_5x4][0].Y);
            r_color = frame.GetPixel(CONST_RACK_TYPE_IDENTIFIERS[RackType.Rack_5x4][1].X, CONST_RACK_TYPE_IDENTIFIERS[RackType.Rack_5x4][1].Y);
            if (l_color == rack_color && r_color == rack_color) { Game_RackType = RackType.Rack_5x4; return; }
            // 4x4 rack test
            l_color = frame.GetPixel(CONST_RACK_TYPE_IDENTIFIERS[RackType.Rack_4x4][0].X, CONST_RACK_TYPE_IDENTIFIERS[RackType.Rack_4x4][0].Y);
            r_color = frame.GetPixel(CONST_RACK_TYPE_IDENTIFIERS[RackType.Rack_4x4][1].X, CONST_RACK_TYPE_IDENTIFIERS[RackType.Rack_4x4][1].Y);
            if (l_color == rack_color && r_color == rack_color) { Game_RackType = RackType.Rack_4x4; return; }
            // 3x4 rack test
            l_color = frame.GetPixel(CONST_RACK_TYPE_IDENTIFIERS[RackType.Rack_3x4][0].X, CONST_RACK_TYPE_IDENTIFIERS[RackType.Rack_3x4][0].Y);
            r_color = frame.GetPixel(CONST_RACK_TYPE_IDENTIFIERS[RackType.Rack_3x4][1].X, CONST_RACK_TYPE_IDENTIFIERS[RackType.Rack_3x4][1].Y);
            if (l_color == rack_color && r_color == rack_color) { Game_RackType = RackType.Rack_3x4; return; }
            // unknown rack
            Game_RackType = RackType.Unknown;
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
        public GameResult GetGameResult()
        {
            Bitmap frame = GetGameFrame_Global();
            Bitmap victoryLoc = frame.Clone(CONST_VICTORY_SCAN_REGION, frame.PixelFormat);
            if (Bitmap_CV.HaystackContainsNeedle(victoryLoc, Properties.Resources.game8_victory))
            {
                victoryLoc.Dispose();
                frame.Dispose();
                return GameResult.Victory;
            }
            Bitmap defeatLoc = frame.Clone(CONST_DEFEAT_SCAN_REGION, frame.PixelFormat);
            if (Bitmap_CV.HaystackContainsNeedle(defeatLoc, Properties.Resources.game8_defeat))
            {
                defeatLoc.Dispose();
                frame.Dispose();
                return GameResult.Defeat;
            }

            frame.Dispose();
            victoryLoc.Dispose();
            defeatLoc.Dispose();
            return GameResult.Unknown;
        }
        public Rectangle Find_StartButton()
        {
            Bitmap frame = GetGameFrame_Global();
            TemplateMatch[] matches = Bitmap_CV.FindAllNeedles(frame, Properties.Resources.start_fullscreen, 0.925f);
            if (matches.Length == 0) return Rectangle.Empty;
            return matches[0].Rectangle;
        }

        public Rectangle Find_RestartButton()
        {
            Bitmap frame = GetGameFrame_Global();
            TemplateMatch[] matches = Bitmap_CV.FindAllNeedles(frame, Properties.Resources.restart_btn, 0.925f);
            if (matches.Length == 0) return Rectangle.Empty;
            return matches[0].Rectangle;
        }

        public Rectangle Find_GainPowerButton()
        {
            Bitmap frame = GetGameFrame_Global();
            TemplateMatch[] matches = Bitmap_CV.FindAllNeedles(frame, Properties.Resources.gain_power, 0.925f);
            if (matches.Length == 0) return Rectangle.Empty;
            return matches[0].Rectangle;
        }
    }

    public class MemoryCard
    {
        public CurrencyType CurrencyType;
        public MemoryCard Pair;
        public Point GridLocation;
        public bool Solved;
        public bool __EverScannedBackSide;

        public void Solve()
        {
            if(Pair != null)
                Pair.Solved = true;
            Solved = true;
        }

        public MemoryCard(CurrencyType currencyType, MemoryCard pair, Point gridLocation)
        {
            CurrencyType = currencyType;
            Pair = pair;
            GridLocation = gridLocation;
        }
    }

    public enum CurrencyType
    {
        Binance,
        Bitcoin,
        Eos,
        Ethereum,
        Lightcoin,
        Monero,
        Ripple,
        Rollercoin,
        Tether,
        Stellar,
        CardBackSide,
        Unknown
    }

    public enum RackType
    {
        Rack_3x4,
        Rack_4x4,
        Rack_5x4,
        Unknown
    }
}
