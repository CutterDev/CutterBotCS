using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using CutterBotCS.Worker;

namespace CutterBotCS.Discord
{
    /// <summary>
    /// Leaderboard Creator
    /// </summary>
    public class LeaderboardCreator
    {
        TextOptionsHelper m_TextOptionsHelper;
        FontCollection m_FontCollection;
        Rgba64 BACKGROUND = new Rgba64(39064, 49858, 60395, 65535);

        float POS_X_SIZE = 60;
        float SUMMONER_BOX_WIDTH = 600;
        float TIER_BOX_WIDTH = 550;
        float LP_BOX_WIDTH = 200;
        float WINS_BOX_WIDTH = 125;
        float SLASH_BOX_WIDITH = 50;
        float LOSSES_BOX_WIDTH = 125;
        float TOTALGAMES_BOX_WIDTH = 300;
        float WINRATE_BOX_WIDTH = 150;

        RendererOptions m_Options;

        const string IMAGES_DIR = "/home/pi/CutterBot/Resources/Images/";
        const string FONTS_DIR = "/home/pi/CutterBot/Resources/Fonts/";

        /// <summary>
        /// Ctor
        /// </summary>
        public LeaderboardCreator()
        {
            m_TextOptionsHelper = new TextOptionsHelper();  
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            m_FontCollection = new FontCollection();
            try
            {
                m_FontCollection.Install(FONTS_DIR + "boldfont.ttf");
                m_FontCollection.Install(FONTS_DIR + "bolditalicfont.ttf");
                m_FontCollection.Install(FONTS_DIR + "regularitalicfont.ttf");
                m_FontCollection.Install(FONTS_DIR + "regularfont.ttf");
            }
            catch(Exception e)
            {
                DiscordWorker.Log(string.Format("Error Installing Fonts: {0}", e.Message), LogType.Error);
            }            
        }

        /// <summary>
        /// Create Leaderboard Bitmap
        /// </summary>
        public bool TryCreateLeaderboard(List<LeaderboardEntry> leagueentries, string path, string id, string leaderboardtitle)
        {
            bool result = false;
            string errorformat = "Error Creating Leaderboard {0}";
            string message = string.Empty;
           

            if (leagueentries.Count > 0)
            {
                FontFamily defaultfamilyfont;
                if (m_FontCollection.TryFind("Roboto", out defaultfamilyfont))
                {
                    try
                    {
                        using (Image image = new Image<Rgba64>(2300, 500 + (75 * leagueentries.Count), BACKGROUND))
                        {
                            Font titlefont = defaultfamilyfont.CreateFont(100, FontStyle.Bold);
                            Font font = defaultfamilyfont.CreateFont(60, FontStyle.Bold);

                            m_Options = new RendererOptions(font, dpi: 72)
                            {
                                ApplyKerning = true,
                            };

                            // Logo
                            string logopath = string.Format(IMAGES_DIR + "{0}_L1.png", id);
                            if (File.Exists(logopath))
                            {
                                using (Image logo = Image.Load(logopath))
                                {
                                    // Resize Loaded Image
                                    logo.Mutate(x => x.Resize(new Size(240, 240)));
                                    image.Mutate(x => x.DrawImage(logo, new Point(75, 75), 1));
                                };

                            }

                            // Logo 2
                            string logo2path = string.Format(IMAGES_DIR + "{0}_L2.png", id);
                            if (File.Exists(logo2path))
                            {
                                using (Image logo = Image.Load(logo2path))
                                {
                                    // Resize Loaded Image
                                    logo.Mutate(x => x.Resize(new Size(240, 240)));
                                    image.Mutate(x => x.DrawImage(logo, new Point(image.Width - logo.Width - 75, 75), 1));
                                }
                            }

                            // Title 
                            string drawstring = leaderboardtitle;
                            FontRectangle titlebox = new FontRectangle(0, 100, image.Width, 30);
                            DrawText(image, TextAlignment.Center, drawstring, titlefont, Color.White, titlebox);


                            bool alternate = false;

                            string sampletext = "CUTTERHEALER";
                            float textheight = TextMeasurer.Measure(sampletext, m_Options).Height;
                            float linexstartpos = 50;
                            int i = 1;

                            foreach (LeaderboardEntry entry in leagueentries)
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
                                drawstring = string.Format("{0} {1}", entry.Tier, entry.Division);
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

                                // WinRate 
                                float wr = entry.WinRate;
                                drawstring = string.Format("{0:0.00}%", wr);
                                Color wrcolor = wr >= 50.0 ? Color.DarkGreen : Color.DarkRed;
                                textbox = new FontRectangle(nextdrawpos.X, nextdrawpos.Y, WINRATE_BOX_WIDTH, textheight);
                                nextdrawpos = DrawText(image, TextAlignment.Center, drawstring, font, wrcolor, textbox);

                                i++;
                            }

                            image.SaveAsPng(path);
                            result = true;
                        };                      
                    }
                    catch (Exception e)
                    {
                        message = string.Format(errorformat, e.Message);                       
                    }
                }
                else
                {
                    message = string.Format(errorformat, "Fonts were not installed");
                }
            }
            else
            {
                message = string.Format(errorformat, "No Players exist!");
            }

            DiscordWorker.Log(message, LogType.Error);

            return result;
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
