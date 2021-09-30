using Camille.RiotGames.LeagueV4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutterBotCS.Helpers
{
    /// <summary>
    /// Leaderboard UI Creator
    /// </summary>
    public class LeaderboardUICreator
    {
        Font TITLE_FONT = new Font("Helvetica", 42, FontStyle.Bold);
        Font RANK_FONT = new Font("Helvetica", 22, FontStyle.Bold);

        const float POS_X_SIZE = 60;
        const float NAME_X_SIZE = 300;
        const float TIER_X_SIZE = 220;
        const float LP_X_SIZE = 100;
        const float W_X_SIZE = 80;
        const float WL_X_SIZE = 25;
        const float L_X_SIZE = 100;
        const float T_X_SIZE = 100;
        const float WR_X_SIZE = 150;

        /// <summary>
        /// Create Leaderboard Bitmap
        /// </summary>
        public Bitmap CreateLeaderboard(List<LeagueEntry> leagueentries, string path)
        {           
            Bitmap bm = new Bitmap(1300, 200 + (50 * leagueentries.Count));
            bm.MakeTransparent(Color.Transparent);

            if (leagueentries.Count > 0)
            {
                Graphics g = Graphics.FromImage(bm);
                g.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#ffff9255")), new Rectangle(0, 0, bm.Width, bm.Height));
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.FillEllipse(Brushes.White, 10, 10, 140, 140);

                // Draw Logo
                g.DrawImage(Properties.Resources.logo, 20, 20, 120, 120);

                // Draw Logo
                g.DrawImage(Properties.Resources.benny, 1220, 30, 60, 60);

                // Title 
                string drawstring = "Pearlsayah Leaderboard";
                SizeF stringsize = g.MeasureString(drawstring, TITLE_FONT);
                g.DrawString(drawstring, TITLE_FONT, Brushes.White, new RectangleF(170, 55, stringsize.Width, stringsize.Height));

                int i = 1;
                foreach (LeagueEntry entry in leagueentries)
                {
                    drawstring = string.Format("{0})", i);
                    stringsize = g.MeasureString(drawstring, RANK_FONT);
                    float lineYPos = 130 + (i * stringsize.Height) + 5;
                    float lineXPos = 50;

                    RectangleF rectf = new RectangleF(lineXPos, lineYPos, POS_X_SIZE, stringsize.Height);
                    g.DrawString(drawstring, RANK_FONT, Brushes.White, rectf);
                    drawstring = string.Format("{0}", entry.SummonerName);

                    lineXPos += POS_X_SIZE;
                    // Name
                    stringsize = g.MeasureString(drawstring, RANK_FONT);
                    rectf = new RectangleF(lineXPos, lineYPos, NAME_X_SIZE, stringsize.Height);
                    g.DrawString(drawstring, RANK_FONT, Brushes.White, rectf);

                    lineXPos += NAME_X_SIZE;


                    // Tier Rank
                    drawstring = string.Format("{0} {1}", entry.Tier, entry.Rank);
                    rectf = new RectangleF(lineXPos, lineYPos, TIER_X_SIZE, stringsize.Height);
                    g.DrawString(drawstring, RANK_FONT, Brushes.White, rectf);
      
                    lineXPos += TIER_X_SIZE;


                    // League Points
                    drawstring = string.Format("LP {0}", entry.LeaguePoints);
                    rectf = new RectangleF(lineXPos, lineYPos, LP_X_SIZE, stringsize.Height);
                    g.DrawString(drawstring, RANK_FONT, Brushes.White, rectf);

                    lineXPos += LP_X_SIZE;

                    // Wins / 
                    drawstring = string.Format("{0}", entry.Wins);
                    rectf = new RectangleF(lineXPos, lineYPos, W_X_SIZE, stringsize.Height);
                    g.DrawString(drawstring, RANK_FONT, Brushes.DarkGreen, rectf);

                    lineXPos += W_X_SIZE;

                    // W
                    drawstring = string.Format("W");
                    rectf = new RectangleF(lineXPos, lineYPos, WL_X_SIZE + 15, stringsize.Height);
                    g.DrawString(drawstring, RANK_FONT, Brushes.DarkGreen, rectf);

                    lineXPos += WL_X_SIZE + 15;

                    // /
                    drawstring = string.Format("/");
                    rectf = new RectangleF(lineXPos, lineYPos, WL_X_SIZE, stringsize.Height);
                    g.DrawString(drawstring, RANK_FONT, Brushes.White, rectf);

                    lineXPos += WL_X_SIZE;

                    // L
                    drawstring = string.Format("L");
                    rectf = new RectangleF(lineXPos, lineYPos, WL_X_SIZE + 10, stringsize.Height);
                    g.DrawString(drawstring, RANK_FONT, Brushes.DarkRed, rectf);

                    lineXPos += WL_X_SIZE + 10;

                    // Losses
                    drawstring = string.Format("{0}", entry.Losses);
                    rectf = new RectangleF(lineXPos, lineYPos, L_X_SIZE, stringsize.Height);
                    g.DrawString(drawstring, RANK_FONT, Brushes.DarkRed, rectf);

                    lineXPos += L_X_SIZE;

                    // Total 
                    drawstring = string.Format("({0})", (entry.Losses + entry.Wins));
                    rectf = new RectangleF(lineXPos, lineYPos, T_X_SIZE, stringsize.Height);
                    g.DrawString(drawstring, RANK_FONT, Brushes.White, rectf);

                    lineXPos += T_X_SIZE;

                    float wr = (entry.Wins / (float)(entry.Losses + entry.Wins)) * 100.0f;

                    Brush wrbrush = wr >= 50.0 ? Brushes.DarkGreen : Brushes.DarkRed;

                    // WinRate 
                    drawstring = string.Format("{0:0.00}%", wr);
                    rectf = new RectangleF(lineXPos, lineYPos, WR_X_SIZE, stringsize.Height);
                    g.DrawString(drawstring, RANK_FONT, wrbrush, rectf);

                    i++;
                }

                g.Flush();

                bm.Save(path);
            }

            return bm;
        }
    }
}
