using System;

namespace CutterBotCS
{
    /// <summary>
    /// Program Constants
    /// </summary>
    public class ProgramConstants
    {
#if DEBUG
        /// <summary>
        /// Resource Directory
        /// </summary>
        public const string RESOURCE_DIR = @"D:/CutterBot/Resources";
#else
        /// <summary>
        /// Resource Directory
        /// </summary>
        private const string RESOURCE_DIR = "/home/pi/CutterBot/Resources";
#endif



        /// <summary>
        /// Resource Images Directory
        /// </summary>
        public static string RESOURCE_IMAGES_DIR
        {
            get { return RESOURCE_DIR + "/Images"; }
        }

        /// <summary>
        /// EPOCH TIME FOR GAME CREATION TIMES
        /// </summary>
        public static DateTime EPOCHTIME = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    }
}
