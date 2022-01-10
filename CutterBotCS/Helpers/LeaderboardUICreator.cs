using Camille.RiotGames.LeagueV4;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;
using System;
using SixLabors.ImageSharp.Drawing.Processing;

namespace CutterBotCS.Helpers
{
    /// <summary>
    /// Leaderboard UI Creator
    /// </summary>
    public class LeaderboardUICreator
    {
        FontCollection m_FontCollection;
        Rgba64 BACKGROUND = new Rgba64(65535, 32639, 20560, 65535);

        float POS_X_SIZE = 60;
        float NAME_X_SIZE = 400;
        float TIER_X_SIZE = 350;
        float LP_X_SIZE = 150;
        float W_X_SIZE = 100;
        float WL_X_SIZE = 50;
        float L_X_SIZE = 100;
        float T_X_SIZE = 150;

        /// <summary>
        /// Ctor
        /// </summary>
        public LeaderboardUICreator()
        {

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
            Image image = new Image<Rgba64>(1750, 200 + (75 * leagueentries.Count), BACKGROUND);

            FontFamily defaultfamilyfont;
   
            if (leagueentries.Count > 0)
            {
                if (m_FontCollection.TryFind("Roboto", out defaultfamilyfont))
                {
                    try
                    {
                        Font titlefont = defaultfamilyfont.CreateFont(60, FontStyle.Bold);
                        Font font = defaultfamilyfont.CreateFont(40, FontStyle.Bold);

                        RendererOptions options = new RendererOptions(font, dpi: 72)
                        {
                            ApplyKerning = true,
                        };

                        DrawingOptions alignright = new DrawingOptions()
                        {
                            TextOptions = new TextOptions()
                            {
                                HorizontalAlignment = HorizontalAlignment.Right,                              
                            }
                        };

                        DrawingOptions aligncenter = new DrawingOptions()
                        {
                            TextOptions = new TextOptions()
                            {
                                HorizontalAlignment = HorizontalAlignment.Center,
                            }
                        };

                        Image logo = Image.Load(AppDomain.CurrentDomain.BaseDirectory + "Resources/Images/logo.png");
                        logo.Mutate(x => x.Resize(new Size(120, 120)));

                        // Logo
                        image.Mutate(x => x.DrawImage(logo, new Point(10, 10), 1));

                        logo = Image.Load(AppDomain.CurrentDomain.BaseDirectory + "Resources/Images/benny.png");
                        logo.Mutate(x => x.Resize(new Size(60, 60)));
                        // Benny
                        image.Mutate(x => x.DrawImage(logo, new Point(1650, 30), 1));

                        // Title 
                        string drawstring = "Pearlsayah Leaderboard";
                        FontRectangle rect = TextMeasurer.Measure(drawstring, options);
                        image.Mutate(x => x.DrawText(drawstring, titlefont, Color.White, new PointF(170, 55)));

                        int i = 1;

                        bool alternate = false;

                        foreach(LeagueEntry entry in leagueentries)
                        {
                            drawstring = i.ToString();
                            rect = TextMeasurer.Measure(drawstring, options);
                            float lineYPos = 130 + (i * (rect.Height + 10));
                            float lineXPos = 50;

                            if (alternate)
                            {
                                image.Mutate(x => x.FillPolygon(Color.FromRgb(204, 112, 84),
                                                                new PointF(20, lineYPos),
                                                                new PointF(20, lineYPos + (rect.Height + 10)),
                                                                new PointF(image.Width - 20, lineYPos + (rect.Height + 10)),
                                                                new PointF(image.Width - 20, lineYPos)));       
                            }

                            alternate = !alternate;

                            FontRectangle fontrect = new FontRectangle(lineXPos, lineYPos, POS_X_SIZE, rect.Height);
                            image.Mutate(x => x.DrawText(drawstring, font, Color.White, new PointF(fontrect.X, fontrect.Y)));
                            drawstring = string.Format("{0}", entry.SummonerName);

                            lineXPos += POS_X_SIZE;
                            // Name
                            fontrect = TextMeasurer.Measure(drawstring, options);
                            fontrect = new FontRectangle(lineXPos, lineYPos, NAME_X_SIZE, rect.Height);
                            image.Mutate(x => x.DrawText(drawstring, font, Color.White, new PointF(fontrect.X, fontrect.Y)));

                            lineXPos += NAME_X_SIZE;


                            // Tier Rank
                            drawstring = string.Format("{0} {1}", entry.Tier, entry.Rank);
                            fontrect = TextMeasurer.Measure(drawstring, options);
                            fontrect = new FontRectangle(lineXPos, lineYPos, NAME_X_SIZE, rect.Height);
                            image.Mutate(x => x.DrawText(drawstring, font, Color.White, new PointF(fontrect.X, fontrect.Y)));

                            lineXPos += TIER_X_SIZE;


                            // League Points
                            drawstring = string.Format("LP {0}", entry.LeaguePoints);
                            fontrect = TextMeasurer.Measure(drawstring, options);
                            fontrect = new FontRectangle(lineXPos, lineYPos, NAME_X_SIZE, rect.Height);
                            image.Mutate(x => x.DrawText(drawstring, font, Color.White, new PointF(fontrect.X, fontrect.Y)));

                            lineXPos += LP_X_SIZE;

                            // Wins
                            drawstring = string.Format("{0}", entry.Wins);
                            fontrect = TextMeasurer.Measure(drawstring, options);
                            fontrect = new FontRectangle(lineXPos + W_X_SIZE - 20, lineYPos, W_X_SIZE, rect.Height);
                            image.Mutate(x => x.DrawText(alignright, drawstring, font, Color.DarkGreen, new PointF(fontrect.X, fontrect.Y)));

                            lineXPos += W_X_SIZE;


                            // /
                            drawstring = string.Format("/");
                            fontrect = TextMeasurer.Measure(drawstring, options);
                            fontrect = new FontRectangle(lineXPos, lineYPos, WL_X_SIZE, rect.Height);
                            image.Mutate(x => x.DrawText(aligncenter, drawstring, font, Color.White, new PointF(fontrect.X, fontrect.Y)));

                            lineXPos += 20;

                            // Losses
                            drawstring = string.Format("{0}", entry.Losses);
                            fontrect = TextMeasurer.Measure(drawstring, options);
                            fontrect = new FontRectangle(lineXPos, lineYPos, NAME_X_SIZE, rect.Height);
                            image.Mutate(x => x.DrawText(drawstring, font, Color.DarkRed, new PointF(fontrect.X, fontrect.Y)));

                            lineXPos += L_X_SIZE;

                            // Total 
                            drawstring = string.Format("({0})", (entry.Losses + entry.Wins));
                            fontrect = TextMeasurer.Measure(drawstring, options);
                            fontrect = new FontRectangle(lineXPos, lineYPos, NAME_X_SIZE, rect.Height);
                            image.Mutate(x => x.DrawText(drawstring, font, Color.White, new PointF(fontrect.X, fontrect.Y)));

                            lineXPos += T_X_SIZE;

                            float wr = (entry.Wins / (float)(entry.Losses + entry.Wins)) * 100.0f;

                            Color wrcolor = wr >= 50.0 ? Color.DarkGreen : Color.DarkRed;

                            // WinRate 
                            drawstring = string.Format("{0:0.00}%", wr);
                            fontrect = TextMeasurer.Measure(drawstring, options);
                            fontrect = new FontRectangle(lineXPos, lineYPos, NAME_X_SIZE, rect.Height);
                            image.Mutate(x => x.DrawText(drawstring, font, wrcolor, new PointF(fontrect.X, fontrect.Y)));

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
    }
}
