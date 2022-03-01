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

namespace CutterBotCS.Modules.Riot.History
{
    /// <summary>
    /// History Creator
    /// </summary>
    public class HistoryCreator
    {
        const string IMAGES_DIR = "/home/pi/CutterBot/Resources/Images/";
        const string FONTS_DIR = "/home/pi/CutterBot/Resources/Fonts/";

        TextOptionsHelper m_TextOptionsHelper;
        FontCollection m_FontCollection;
        Rgba64 BACKGROUND = new Rgba64(9252, 25443, 40863, 65535);

        public HistoryCreator()
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
            catch (Exception e)
            {
                DiscordWorker.Log(string.Format("HistoryCreator Error Installing Fonts: {0}", e.Message), LogType.Error);
            }
        }

        /// <summary>
        /// Try Create Match History
        /// </summary>
        public bool TryCreateMatchHistoryImage(HistoryMatchModel matchmodel, string path)
        {
            bool result = false;

            if (matchmodel != null)
            {
                try
                {
                    using (Image image = new Image<Rgba64>(2000, 800, BACKGROUND))
                    {


                        image.SaveAsPng(path);
                        result = true;
                    };
                }
                catch (Exception e)
                {
                    DiscordWorker.Log(string.Format("HistoryCreator Error creating HistoryImage: {0}", e.Message), LogType.Error);
                }
            }


            return result;
        }
    }
}
