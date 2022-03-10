using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using CutterBotCS.Discord;
using CutterBotCS.Worker;
using Camille.RiotGames.MatchV5;
using CutterBotCS.Imaging;
using System.IO;

namespace CutterBotCS.Modules.Riot.History
{
    /// <summary>
    /// History Creator
    /// </summary>
    public class HistoryCreator
    {
        const string DEFEAT = "Defeat";
        const string VICTORY = "Victory";

        TextOptionsHelper m_TextOptionsHelper;
        Rgba64 BACKGROUND = new Rgba64(5000, 5000, 6000, 65535);

        RendererOptions m_H1BoldOptions;
        RendererOptions m_H2BoldOptions;
        RendererOptions m_H2Options;
        RendererOptions m_H3Options;
        RendererOptions m_H4Options;


        public HistoryCreator()
        {
            m_TextOptionsHelper = new TextOptionsHelper();
        }

        /// <summary>
        /// Try Create Match History
        /// </summary>
        public bool TryCreateMatchHistoryImage(HistoryMatchModel model, string path)
        {
            bool result = false;

            if (model != null)
            {
                try
                {
                    FontFamily defaultfont;
                    if (ImagingHelper.Instance.FontCollection.TryFind("Roboto", out defaultfont))
                    {
                        using (Image image = new Image<Rgba64>(2200, 1000, BACKGROUND))
                        {
                            m_H1BoldOptions = new RendererOptions(defaultfont.CreateFont(60, FontStyle.Bold));
                            m_H2BoldOptions = new RendererOptions(defaultfont.CreateFont(45, FontStyle.Bold));
                            m_H2Options = new RendererOptions(defaultfont.CreateFont(45, FontStyle.Regular));
                            m_H3Options = new RendererOptions(defaultfont.CreateFont(40, FontStyle.Regular));
                            m_H4Options = new RendererOptions(defaultfont.CreateFont(30, FontStyle.Regular));


                            bool blueteamwin = model.VictoryTeam == Camille.RiotGames.Enums.Team.Blue;

                            //
                            // Team 1 Victory / Defeat
                            //
                            string teamresult = blueteamwin ? VICTORY : DEFEAT;
                            Color resultcolor = blueteamwin ? Color.Green : Color.Red;
                            FontRectangle teambox = new FontRectangle(50, 50, 250, 30);
                            PointF teamresultpoint = DrawText(image, TextAlignment.Left, VerticalTextAlignment.Center, teamresult, m_H1BoldOptions, resultcolor, teambox);

                            //
                            // KDA Team 1 Total
                            //
                            int killsteam1 = 0;
                            int assists = 0;
                            int deaths = 0;

                            foreach(KeyValuePair<int, Participant> p in model.Team1)
                            {
                                killsteam1 += p.Value.Kills;
                                assists += p.Value.Assists;
                                deaths += p.Value.Deaths;
                            }

                            string kdatotal = string.Format("{0} / {1} / {2}", killsteam1, deaths, assists);
                            FontRectangle kdabox = new FontRectangle(teamresultpoint.X + 10, 50, 100, 30);
                            DrawText(image, TextAlignment.Left, VerticalTextAlignment.Center, kdatotal, m_H2Options, Color.Gray, kdabox);


                            // 
                            // Queue Type (Flex/Solo) Game Time
                            //
                            FontRectangle titlebox = new FontRectangle(0, 50, image.Width, 30);
                            PointF typepoint = DrawText(image, TextAlignment.Center, VerticalTextAlignment.Center, 
                                                        string.Format("{0} ({1})", model.RankType, model.GameTime.ToString(@"mm\:ss")),
                                                        m_H2BoldOptions, Color.Gray, titlebox);

                            //
                            // Team 2 Victory / Defeat
                            //
                            teamresult = blueteamwin ? DEFEAT : VICTORY;
                            resultcolor = blueteamwin ? Color.Red : Color.Green;
                            teambox = new FontRectangle(image.Width - 300, 50, 250, 30);
                            teamresultpoint = DrawText(image, TextAlignment.Right, VerticalTextAlignment.Center, teamresult, m_H1BoldOptions, resultcolor, teambox);

                            //
                            // KDA Team 2 Total
                            //
                            int killsteam2 = 0;
                            assists = 0;
                            deaths = 0;

                            foreach (KeyValuePair<int, Participant> p in model.Team2)
                            {
                                killsteam2 += p.Value.Kills;
                                assists += p.Value.Assists;
                                deaths += p.Value.Deaths;
                            }

                            kdatotal = string.Format("{0} / {1} / {2}", killsteam2, deaths, assists);
                            kdabox = new FontRectangle(teamresultpoint.X - teambox.Width - 110, 50, 100, 30);
                            DrawText(image, TextAlignment.Right, VerticalTextAlignment.Center, kdatotal, m_H2Options, Color.Gray, kdabox);

                            // Draw Top Line
                            DrawLine(image, Color.Snow, 5.0f, new PointF(30, 110), new PointF(image.Width - 30, 110));

                            for(int i = 0; i < 5; i++)
                            {
                                DrawLaneStats(image,(i + 1) * 150, model.Team1[i], model.Team2[i], killsteam1, killsteam2);
                            }
                   

                            image.SaveAsPng(path);
                            result = true;
                        };
                    }
                }
                catch (Exception e)
                {
                    DiscordWorker.Log(string.Format("HistoryCreator Error creating HistoryImage: {0}", e.Message), LogType.Error);
                }
            }


            return result;
        }

        /// <summary>
        /// Draw Each Lane of stats
        /// </summary>
        private void DrawLaneStats(Image image, int refypoint, Participant bluep, Participant redp, int team1totalkills, int team2totalkills)
        {
            //
            // BLUE PLAYER
            //
            // Champion Icon
            string champicon = Path.Combine(CutterDragon.CutterDragonConsts.CHAMPION_ICONS_DIR, string.Format("{0}.png", (int)bluep.ChampionId));

            FontRectangle champdim = new FontRectangle(20, refypoint + 20, 140, 140);
            PointF champpoint = DrawImage(image, champdim, champicon);

            // Participant Name 
            PointF namepoint = DrawTextCenterLeft(image, new FontRectangle(champpoint.X + 10, champpoint.Y - champdim.Height, 400, champdim.Height), bluep.SummonerName, m_H3Options, Color.Gray);

            
            PointF textpoint = DrawTextCenter(image, new FontRectangle(namepoint.X, refypoint + 35, 50, 30), bluep.Kills.ToString(), m_H4Options, Color.Green);
            textpoint = DrawTextCenter(image, new FontRectangle(textpoint.X, refypoint + 35, 25, 30), "/", m_H4Options, Color.Gray);
            textpoint = DrawTextCenter(image, new FontRectangle(textpoint.X, refypoint + 35, 50, 30), bluep.Deaths.ToString(), m_H4Options, Color.Red);
            textpoint = DrawTextCenter(image, new FontRectangle(textpoint.X, refypoint + 35, 25, 30), "/", m_H4Options, Color.Gray);
            textpoint = DrawTextCenter(image, new FontRectangle(textpoint.X, refypoint + 35, 50, 30), bluep.Assists.ToString(), m_H4Options, Color.Yellow);
            textpoint = DrawTextCenter(image, new FontRectangle(namepoint.X, textpoint.Y + 5, 200, 30),
                                        string.Format("{0} CS - {1} gold", (bluep.TotalMinionsKilled + bluep.NeutralMinionsKilled), bluep.GoldEarned), m_H4Options, Color.Gray);
            PointF edgeofstats = DrawTextCenter(image, new FontRectangle(namepoint.X, textpoint.Y + 5, 200, 30),
                            string.Format("{0:P0} Kills P. - Vision: {1}", ((decimal)bluep.Kills + (decimal)bluep.Assists) / (decimal)team1totalkills, bluep.VisionScore), m_H4Options, Color.Gray);

            int itemiconsize = 45;
            int itempointstart = (int)edgeofstats.X + 100;

            // Items 1,2,3,4
            PointF itempoint = DrawImage(image, new FontRectangle(itempointstart, refypoint + 35, itemiconsize, itemiconsize),
                                    Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", bluep.Item0)));
            itempoint = DrawImage(image, new FontRectangle(itempoint.X + 5, refypoint + 35, itemiconsize, itemiconsize),
                                    Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", bluep.Item1)));
            itempoint = DrawImage(image, new FontRectangle(itempoint.X + 5, refypoint + 35, itemiconsize, itemiconsize),
                        Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", bluep.Item2)));
            itempoint = DrawImage(image, new FontRectangle(itempoint.X + 5, refypoint + 35, itemiconsize, itemiconsize),
                        Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", bluep.Item3)));

            // Items 5,6,7
            itempoint = DrawImage(image, new FontRectangle(itempointstart, refypoint + 35 + itemiconsize + 5, itemiconsize, itemiconsize),
                  Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", bluep.Item4)));
            itempoint = DrawImage(image, new FontRectangle(itempoint.X + 5, refypoint + 35 + itemiconsize + 5, itemiconsize, itemiconsize),
                  Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", bluep.Item5)));
            itempoint = DrawImage(image, new FontRectangle(itempoint.X + 5, refypoint + 35 + itemiconsize + 5, itemiconsize, itemiconsize),
                  Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", bluep.Item6)));

            //
            // RED PLAYER
            //

            // Champion Icon
            champicon = Path.Combine(CutterDragon.CutterDragonConsts.CHAMPION_ICONS_DIR, string.Format("{0}.png", (int)redp.ChampionId));

            champdim = new FontRectangle(image.Width - 160, refypoint + 20, 140, 140);
            champpoint = DrawImage(image, champdim, champicon);

            // Participant Name 
            namepoint = DrawTextCenterRight(image, new FontRectangle(champpoint.X - champdim.Width - 410, champpoint.Y - champdim.Height, 400, champdim.Height), 
                                                    redp.SummonerName, m_H3Options, Color.Gray);


            textpoint = DrawTextCenter(image, new FontRectangle(namepoint.X - 600, refypoint + 35, 50, 30), redp.Kills.ToString(), m_H4Options, Color.Green);
            textpoint = DrawTextCenter(image, new FontRectangle(textpoint.X, refypoint + 35, 25, 30), "/", m_H4Options, Color.Gray);
            textpoint = DrawTextCenter(image, new FontRectangle(textpoint.X, refypoint + 35, 50, 30), redp.Deaths.ToString(), m_H4Options, Color.Red);
            textpoint = DrawTextCenter(image, new FontRectangle(textpoint.X, refypoint + 35, 25, 30), "/", m_H4Options, Color.Gray);
            textpoint = DrawTextCenter(image, new FontRectangle(textpoint.X, refypoint + 35, 50, 30), redp.Assists.ToString(), m_H4Options, Color.Yellow);
            textpoint = DrawTextCenter(image, new FontRectangle(namepoint.X - 600, textpoint.Y + 5, 200, 30),
                                        string.Format("{0} CS - {1} gold", (redp.TotalMinionsKilled + redp.NeutralMinionsKilled), redp.GoldEarned), m_H4Options, Color.Gray);
            edgeofstats = DrawTextCenter(image, new FontRectangle(namepoint.X - 600, textpoint.Y + 5, 200, 30),
                            string.Format("{0:P0} Kills P. - Vision: {1}", (((decimal)redp.Kills + (decimal)redp.Assists) / (decimal)team2totalkills), redp.VisionScore), m_H4Options, Color.Gray);

            itempointstart = (int)edgeofstats.X - 500;

            // Items 1,2,3,4
            itempoint = DrawImage(image, new FontRectangle(itempointstart, refypoint + 35, itemiconsize, itemiconsize),
                                    Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", redp.Item0)));
            itempoint = DrawImage(image, new FontRectangle(itempoint.X + 5, refypoint + 35, itemiconsize, itemiconsize),
                                    Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", redp.Item1)));
            itempoint = DrawImage(image, new FontRectangle(itempoint.X + 5, refypoint + 35, itemiconsize, itemiconsize),
                        Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", redp.Item2)));
            itempoint = DrawImage(image, new FontRectangle(itempoint.X + 5, refypoint + 35, itemiconsize, itemiconsize),
                        Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", redp.Item3)));

            // Items 5,6,7
            itempoint = DrawImage(image, new FontRectangle(itempointstart, refypoint + 35 + itemiconsize + 5, itemiconsize, itemiconsize),
                  Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", redp.Item4)));
            itempoint = DrawImage(image, new FontRectangle(itempoint.X + 5, refypoint + 35 + itemiconsize + 5, itemiconsize, itemiconsize),
                  Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", redp.Item5)));
            itempoint = DrawImage(image, new FontRectangle(itempoint.X + 5, refypoint + 35 + itemiconsize + 5, itemiconsize, itemiconsize),
                  Path.Combine(CutterDragon.CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", redp.Item6)));
        }

        /// <summary>
        /// Draw Image
        /// </summary>
        /// <returns>Bottom right point of image</returns>
        private PointF DrawImage(Image image, FontRectangle dim, string path)
        {         
            if (File.Exists(path))
            {
                using (Image logo = Image.Load(path))
                {
                    // Resize Loaded Image
                    logo.Mutate(x => x.Resize((int)dim.Width, (int)dim.Height));
                    image.Mutate(x => x.DrawImage(logo, new Point((int)dim.X, (int)dim.Y), 1));
                };
            }

            return new PointF(dim.X + dim.Width, dim.Y + dim.Height);
        }

        /// <summary>
        /// Draw Text Top Left
        /// </summary>
        private PointF DrawTextTopLeft(Image image, FontRectangle rect, string text, RendererOptions ro, Color color)
        {
            return DrawText(image, TextAlignment.Left, VerticalTextAlignment.Top, text,
                            ro, color, rect);
        }

        /// <summary>
        /// Draw Text Top Center
        /// </summary>
        private PointF DrawTextTopCenter(Image image, FontRectangle rect, string text, RendererOptions ro, Color color)
        {
            return DrawText(image, TextAlignment.Center, VerticalTextAlignment.Top, text,
                            ro, color, rect);
        }

        /// <summary>
        /// Draw Text Top Right
        /// </summary>
        private PointF DrawTextTopRight(Image image, FontRectangle rect, string text, RendererOptions ro, Color color)
        {
            return DrawText(image, TextAlignment.Right, VerticalTextAlignment.Top, text,
                            ro, color, rect);
        }

        /// <summary>
        /// Draw Text Center Left
        /// </summary>
        private PointF DrawTextCenterLeft(Image image, FontRectangle rect, string text, RendererOptions ro, Color color)
        {
            return DrawText(image, TextAlignment.Left, VerticalTextAlignment.Center, text,
                            ro, color, rect);
        }

        /// <summary>
        /// Draw Text Center
        /// </summary>
        private PointF DrawTextCenter(Image image, FontRectangle rect, string text, RendererOptions ro, Color color)
        {
            return DrawText(image, TextAlignment.Center, VerticalTextAlignment.Center, text,
                            ro, color, rect);
        }

        /// <summary>
        /// Draw Text Center Right
        /// </summary>
        private PointF DrawTextCenterRight(Image image, FontRectangle rect, string text, RendererOptions ro, Color color)
        {
            return DrawText(image, TextAlignment.Right, VerticalTextAlignment.Center, text,
                            ro, color, rect);
        }

        /// <summary>
        /// Draw Text Bottom Left
        /// </summary>
        private PointF DrawTextBottomLeft(Image image, FontRectangle rect, string text, RendererOptions ro, Color color)
        {
            return DrawText(image, TextAlignment.Left, VerticalTextAlignment.Bottom, text,
                            ro, color, rect);
        }

        /// <summary>
        /// Draw Text Bottom Center
        /// </summary>
        private PointF DrawTextBottomCenter(Image image, FontRectangle rect, string text, RendererOptions ro, Color color)
        {
            return DrawText(image, TextAlignment.Center, VerticalTextAlignment.Bottom, text,
                            ro, color, rect);
        }


        /// <summary>
        /// Draw Text Bottom Right
        /// </summary>
        private PointF DrawTextBottomRight(Image image, FontRectangle rect, string text, RendererOptions ro, Color color)
        {
            return DrawText(image, TextAlignment.Right, VerticalTextAlignment.Bottom, text,
                            ro, color, rect);
        }

        /// <summary>
        /// Draw Text
        /// </summary>
        /// <param name="rectangle">start position for drawing text</param>
        /// <returns> returns the end position of text bottom right corner of rectangle </returns>
        private PointF DrawText(Image image, TextAlignment alignment, VerticalTextAlignment vta, string text,
                                RendererOptions options, Color textcolor, FontRectangle rectangle, float margin = 0)
        {
            DrawingOptions dop;

            PointF drawpoint = GetDrawingTextPosition(text, alignment, vta, rectangle, options, margin, out dop);
            image.Mutate(x => x.DrawText(dop, text, options.Font, textcolor, drawpoint));

            return new PointF(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
        }

        /// <summary>
        /// Get Drawing Text Position
        /// </summary>
        public PointF GetDrawingTextPosition(string text, TextAlignment alignment, VerticalTextAlignment vta, FontRectangle rectangle,
                                             RendererOptions ro, float margin, out DrawingOptions textoptions)
        {
            PointF point = new PointF(0, rectangle.Y);
            textoptions = new DrawingOptions();
            FontRectangle textsize = TextMeasurer.Measure(text, ro);

            switch (vta)
            {
                case VerticalTextAlignment.Top:
                    point.Y = rectangle.Y;
                    break;
                case VerticalTextAlignment.Center:
                    point.Y = rectangle.Y + (rectangle.Height / 2.0f) - (textsize.Height / 2.0f);
                    break;
                case VerticalTextAlignment.Bottom:
                    point.X = rectangle.Y + rectangle.Height - textsize.Height;
                    break;
                default:
                    break;
            }

            switch (alignment)
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

        /// <summary>
        /// Draw line
        /// </summary>
        public void DrawLine(Image image, Color color, float thickness, params PointF[] points)
        {
            image.Mutate(x => x.DrawLines(color, thickness, points));
        }
    }
}
