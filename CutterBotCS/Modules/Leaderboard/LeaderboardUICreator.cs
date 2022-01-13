using Camille.RiotGames.LeagueV4;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;
using System;
using SixLabors.ImageSharp.Drawing.Processing;
using CutterBotCS.Modules.Leaderboard;

namespace CutterBotCS.Leaderboard
{
    /// <summary>
    /// Leaderboard UI Creator
    /// </summary>
    public class LeaderboardUICreator
    {
        TextOptionsHelper m_TextOptionsHelper;
        FontCollection m_FontCollection;
        Rgba64 BACKGROUND = new Rgba64(39064, 49858, 60395, 65535);

        float POS_X_SIZE = 60;
        float SUMMONER_BOX_WIDTH = 500;
        float TIER_BOX_WIDTH = 400;
        float LP_BOX_WIDTH = 200;
        float WINS_BOX_WIDTH = 125;
        float SLASH_BOX_WIDITH = 50;
        float LOSSES_BOX_WIDTH = 125;
        float TOTALGAMES_BOX_WIDTH = 200;
        float WINRATE_BOX_WIDTH = 150;

        /// <summary>
        /// Ctor
        /// </summary>
        public LeaderboardUICreator()
        {
            m_TextOptionsHelper = new TextOptionsHelper();
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize(string fontpath = "")
        {
            m_FontCollection = new FontCollection();
            if (string.IsNullOrEmpty(fontpath))
            {
                try
                {
                    m_FontCollection.Install(AppDomain.CurrentDomain.BaseDirectory + "Resources/Fonts/boldfont.ttf");
                    m_FontCollection.Install(AppDomain.CurrentDomain.BaseDirectory + "Resources/Fonts/bolditalicfont.ttf");
                    m_FontCollection.Install(AppDomain.CurrentDomain.BaseDirectory + "Resources/Fonts/regularitalicfont.ttf");
                    m_FontCollection.Install(AppDomain.CurrentDomain.BaseDirectory + "Resources/Fonts/regularfont.ttf");
                }
                catch(Exception e)
                {

                }
            }
        }

        /// <summary>
        /// Create Leaderboard Bitmap
        /// </summary>
        public void CreateLeaderboard(List<LeagueEntry> leagueentries, string path)
        {           
            Image image = new Image<Rgba64>(1900, 350 + (75 * leagueentries.Count), BACKGROUND);

            FontFamily defaultfamilyfont;
   
            if (leagueentries.Count > 0)
            {
                if (m_FontCollection.TryFind("Roboto", out defaultfamilyfont))
                {
                    try
                    {
                        Font titlefont = defaultfamilyfont.CreateFont(100, FontStyle.Bold);
                        Font font = defaultfamilyfont.CreateFont(60, FontStyle.Bold);

                        RendererOptions options = new RendererOptions(font, dpi: 72)
                        {
                            ApplyKerning = true,
                        };

                        // Logo
                        Image logo = Image.Load(AppDomain.CurrentDomain.BaseDirectory + "Resources/Images/logo1.png");
                        // Resize Loaded Image
                        logo.Mutate(x => x.Resize(new Size(240, 240)));
                        image.Mutate(x => x.DrawImage(logo, new Point(75, 75), 1));

                        // Benny
                        logo = Image.Load(AppDomain.CurrentDomain.BaseDirectory + "Resources/Images/logo2.png");
                        // Resize Loaded Image
                        logo.Mutate(x => x.Resize(new Size(240, 240)));
                        image.Mutate(x => x.DrawImage(logo, new Point(image.Width - logo.Width - 75, 75), 1));

                        // Title 
                        string drawstring = "Pearlsayah Leaderboard";
                        FontRectangle titlebox = new FontRectangle(0, 100, image.Width, 30);
                        DrawText(image, TextAlignment.Center, drawstring, titlefont, Color.White, titlebox);

                        int i = 1;

                        bool alternate = false;

                        string sampletext = "CUTTERHEALER";
                        float textheight = TextMeasurer.Measure(sampletext, options).Height;
                        float linexstartpos = 50;

                        foreach (LeagueEntry entry in leagueentries)
                        {

  
                            float lineYPos = 300 + (i * (textheight + 10));


                            if (alternate)
                            {
                                image.Mutate(x => x.FillPolygon(Color.FromRgb(132, 178, 224),
                                                                new PointF(20, lineYPos),
                                                                new PointF(20, lineYPos + (textheight + 5)),
                                                                new PointF(image.Width - 20, lineYPos + (textheight + 5)),
                                                                new PointF(image.Width - 20, lineYPos)));       
                            }

                            alternate = !alternate;

                            // Number
                            drawstring = i.ToString();
                            FontRectangle textbox = new FontRectangle(linexstartpos, lineYPos, POS_X_SIZE, textheight);
                            PointF nextdrawpos = DrawText(image, TextAlignment.Right, drawstring, font, Color.White, textbox);

                            // Summoner Name
                            drawstring = entry.SummonerName;
                            textbox = new FontRectangle(nextdrawpos.X, nextdrawpos.Y, SUMMONER_BOX_WIDTH, textheight);
                            nextdrawpos = DrawText(image, TextAlignment.Left, drawstring, font, Color.White, textbox, 20.0f);


                            // Tier Rank
                            drawstring = string.Format("{0} {1}", entry.Tier, entry.Rank);
                            textbox = new FontRectangle(nextdrawpos.X, nextdrawpos.Y, TIER_BOX_WIDTH, textheight);
                            nextdrawpos = DrawText(image, TextAlignment.Left, drawstring, font, Color.White, textbox);

                            // League Points
                            drawstring = string.Format("LP {0}", entry.LeaguePoints);
                            textbox = new FontRectangle(nextdrawpos.X, nextdrawpos.Y, LP_BOX_WIDTH, textheight);
                            nextdrawpos = DrawText(image, TextAlignment.Left, drawstring, font, Color.White, textbox);

                            // Wins
                            drawstring = string.Format("{0}", entry.Wins);
                            textbox = new FontRectangle(nextdrawpos.X, nextdrawpos.Y, WINS_BOX_WIDTH, textheight);
                            nextdrawpos = DrawText(image, TextAlignment.Right, drawstring, font, Color.DarkGreen, textbox);

                            // / Between wins and Losses
                            drawstring = string.Format("/");
                            textbox = new FontRectangle(nextdrawpos.X, nextdrawpos.Y, SLASH_BOX_WIDITH, textheight);
                            nextdrawpos = DrawText(image, TextAlignment.Center, drawstring, font, Color.White, textbox);

                            // Losses
                            drawstring = string.Format("{0}", entry.Losses);
                            textbox = new FontRectangle(nextdrawpos.X, nextdrawpos.Y, LOSSES_BOX_WIDTH, textheight);
                            nextdrawpos = DrawText(image, TextAlignment.Left, drawstring, font, Color.DarkRed, textbox);

                            // Total 
                            drawstring = string.Format("({0})", (entry.Losses + entry.Wins));
                            textbox = new FontRectangle(nextdrawpos.X, nextdrawpos.Y, TOTALGAMES_BOX_WIDTH, textheight);
                            nextdrawpos = DrawText(image, TextAlignment.Center, drawstring, font, Color.White, textbox);

                            float wr = (entry.Wins / (float)(entry.Losses + entry.Wins)) * 100.0f;

                            // WinRate 
                            drawstring = string.Format("{0:0.00}%", wr);
                            Color wrcolor = wr >= 50.0 ? Color.DarkGreen : Color.DarkRed;
                            textbox = new FontRectangle(nextdrawpos.X, nextdrawpos.Y, WINRATE_BOX_WIDTH, textheight);
                            nextdrawpos = DrawText(image, TextAlignment.Center, drawstring, font, wrcolor, textbox);

                            i++;
                        }

                        image.Save(path);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(String.Format("Error Creating Leaderboard: {0}", e.Message));
                    }                                
                }
            }
          
        }

        /// <summary>
        /// Draw Text
        /// </summary>
        /// <param name="rectangle">start position for drawing text</param>
        /// <returns> returns the end position of text bottom right corner of rectangle </returns>
        public PointF DrawText(Image image, TextAlignment alignment, string text, Font font, Color textcolor, FontRectangle rectangle, float margin = 0)
        {
            DrawingOptions dop;
            PointF drawpoint = GetDrawingTextPosition(alignment, rectangle, margin, out dop);
            image.Mutate(x => x.DrawText(dop, text, font, textcolor, drawpoint));

            return new PointF(rectangle.X + rectangle.Width, rectangle.Y);
        }

        /// <summary>
        /// Get Drawing Text Position
        /// </summary>
        public PointF GetDrawingTextPosition(TextAlignment alignment, FontRectangle rectangle, float margin, out DrawingOptions textoptions)
        {
            PointF point = new PointF(0, rectangle.Y);
            textoptions = new DrawingOptions();

            switch(alignment)
            {
                case TextAlignment.Left:
                    point.X = rectangle.X + margin;
                    textoptions = m_TextOptionsHelper.AlignLeft;
                    break;
                case TextAlignment.Right:
                    point.X = rectangle.X + rectangle.Width - margin;
                    textoptions = m_TextOptionsHelper.AlignRight;
                    break;
                case TextAlignment.Center:
                    point.X = rectangle.X + (rectangle.Width / 2.0f);
                    textoptions = m_TextOptionsHelper.AlignCenter;
                    break;
            }

            return point;
        }
    }
}
