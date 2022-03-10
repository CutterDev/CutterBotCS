using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutterDragon
{
    public class CutterDragonConsts
    {

#if DEBUG
        /// <summary>
        /// Cutter Dragon Dir
        /// </summary>
        public const string CUTTERDRAGON_DIR = @"D:/CutterDragon";
#else
        /// <summary>
        /// Cutter Dragon Dir
        /// </summary>
        public const string CUTTERDRAGON_DIR = @"/home/pi/CutterBot/CutterDragon";
#endif



        /// <summary>
        /// Champion Directory
        /// </summary>
        public static string CHAMPION_DIR
        {
            get
            { 
                return CUTTERDRAGON_DIR + "/Champions"; 
            }
        }

        /// <summary>
        /// Champion Icons Directory
        /// </summary>
        public static string CHAMPION_ICONS_DIR
        {
            get
            {
                return CHAMPION_DIR + "/Icons";
            }
        }

        /// <summary>
        /// Champion Json Directory
        /// </summary>
        public static string CHAMPION_JSON_DIR
        {
            get
            {
                return CHAMPION_DIR + "/ChampsJsonFiles";
            }
        }

        /// <summary>
        /// Assets Directory
        /// </summary>
        public static string ASSETS_DIR
        {
            get
            {
                return CUTTERDRAGON_DIR + "/Assets";
            }
        }

        /// <summary>
        /// Assets Items Directory
        /// </summary>
        public static string ASSETS_ITEMS_DIR
        {
            get
            {
                return ASSETS_DIR + "/Items";
            }
        }

        /// <summary>
        /// Assets Runes Directory
        /// </summary>
        public static string ASSETS_RUNES_DIR
        {
            get
            {
                return ASSETS_DIR + "/Runes";
            }
        }
    }
}
