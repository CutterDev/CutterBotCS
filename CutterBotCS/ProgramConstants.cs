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
    }
}
