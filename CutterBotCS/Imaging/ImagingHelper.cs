using CutterBotCS.Worker;
using SixLabors.Fonts;
using System;

namespace CutterBotCS.Imaging
{
    /// <summary>
    /// Imaging Helper
    /// </summary>
    public sealed class ImagingHelper
    {
        static string FONTS_DIR = AppDomain.CurrentDomain.BaseDirectory + @"/Resources/Fonts/";

        private static readonly object lockobj = new object ();  
        private static ImagingHelper instance = null;

        public FontCollection FontCollection { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        ImagingHelper() 
        {
            FontCollection = new FontCollection();
            try
            {
                FontCollection.Install(FONTS_DIR + "boldfont.ttf");
                FontCollection.Install(FONTS_DIR + "bolditalicfont.ttf");
                FontCollection.Install(FONTS_DIR + "regularitalicfont.ttf");
                FontCollection.Install(FONTS_DIR + "regularfont.ttf");
            }
            catch (Exception e)
            {
                DiscordWorker.Log(string.Format("LeaderboardError Installing Fonts: {0}", e.Message), LogType.Error);
            }
        }

        /// <summary>
        /// Instance
        /// </summary>
        public static ImagingHelper Instance
        {
            get
            {
                lock (lockobj)
                {
                    if (instance == null)
                    {
                        instance = new ImagingHelper();
                    }
                    return instance;
                }
            }
        }
    }
}
