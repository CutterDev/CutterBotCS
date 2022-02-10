using CutterDragon.Champions;
using CutterDragon.Helpers;
using System;
using System.IO;
using System.Net;

namespace CutterDragon
{
    /// <summary>
    /// Cutter Dragon Deals with grabbing all data through Community Dragon
    /// https://raw.communitydragon.org
    /// </summary>
    public class CutterDragonWorker
    {
        private const string CHAMPION_DIR = "Champions";

        private string CutterData_Dir;

        private string ChampionsDir;

        private string ChampionIconsDir;

        private string ChampionJsonFilesDir;

        #region CDragon URLS

        public const string LOL_GAME_PATH = "/lol-game-data/assets/";

        public const string CDATA_ASSET_PATH = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/";

        private const string CDATA_CHAMPIONSUMMARYJSON = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-summary.json";

        private const string CDATA_CHAMPIONJSON = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champions/{0}.json";

        #endregion

        /// <summary>
        /// ChampionSummaries
        /// </summary>
        private ChampionSummary[] m_ChampionSummaries { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CutterDragonWorker()
        {
            CreateDirectories();
        }

        public void Initialize()
        {
            GetChampionSummariesFromFile();
        }

        /// <summary>
        /// Create Directories
        /// </summary>
        private void CreateDirectories()
        {
            bool noerror = false;
            try
            {
                CutterData_Dir = @"/home/pi/CutterBot/CutterDragon";
                noerror = true;
            }
            catch (Exception e)
            {

            }

            if (noerror)
            {
                CreateDirectory(CutterData_Dir);

                // Champions Directory
                ChampionsDir = Path.Combine(CutterData_Dir, CHAMPION_DIR);
                CreateDirectory(ChampionsDir);

                // Champion Icons Directory
                ChampionIconsDir = Path.Combine(ChampionsDir, "Icons");
                CreateDirectory(ChampionIconsDir);

                // Champion Json Files Directory
                ChampionJsonFilesDir = Path.Combine(ChampionsDir, "ChampsJsonFiles");
                CreateDirectory(ChampionJsonFilesDir);
            }
        }

        /// <summary>
        /// Create Directory
        /// </summary>
        private void CreateDirectory(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch (Exception e)
            {

            }
        }

        private void GetChampionSummariesFromFile()
        {
            m_ChampionSummaries = new ChampionSummary[0];

            ChampionSummary[] champs;
            if(TryGetChampionStats(out champs))
            {
                m_ChampionSummaries = champs;
            }
        }

        /// <summary>
        /// Try Get Champion Stats
        /// </summary>
        private bool TryGetChampionStats(out ChampionSummary[] champions)
        {
            champions = null;
            bool result = false;

            try
            {
                using (WebClient wc = new WebClient())
                {
                    var json = wc.DownloadString(CDATA_CHAMPIONSUMMARYJSON);

                    File.WriteAllText(Path.Combine(ChampionsDir, "champions.json"), json);

                    result = !string.IsNullOrWhiteSpace(json) && JsonHelper.TryDeserialize(json, out champions);
                }
            }
            catch (Exception e)
            {

            }


            return result;
        }

        /// <summary>
        /// Grab Champion Icons
        /// </summary>
        private void GrabChampionIcons(ChampionSummary champion)
        {
            try
            {
                string lolpath = champion.SquarePortraitPath.Replace(LOL_GAME_PATH, string.Empty);
                string cdatapath = CDATA_ASSET_PATH + lolpath;
                using (WebClient client = new WebClient())
                {
                    string iconpath = Path.Combine(ChampionIconsDir, string.Format("{0}.png", champion.Id));
                    client.DownloadFile(new Uri(cdatapath), iconpath);
                }
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Grab Champion Json File from C Dragon 
        /// </summary>
        private void GrabChampionJson(int championid)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(string.Format(CDATA_CHAMPIONJSON, championid)), Path.Combine(ChampionJsonFilesDir, string.Format("{0}.json", championid)));
                }
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Get Assets
        /// </summary>
        public void GetAssets()
        {
            ChampionSummary[] champions;

            if (TryGetChampionStats(out champions))
            {
                for (int i = 0; i < champions.Length; i++)
                {
                    var champion = champions[i];

                    // Champion Icons
                    GrabChampionIcons(champion);

                    // Champion Json
                    GrabChampionJson(champion.Id);
                }
            }
        }

        /// <summary>
        /// Try Get Champion Info from JSON File
        /// </summary>
        public bool TryGetChampionModel(string championname, out ChampionModel model)
        {
            bool result = false;
            model = null;

            ChampionSummary champsumm = null;

            for(int i = 0; i < m_ChampionSummaries.Length; i++)
            {
                if (m_ChampionSummaries[i].Name.Equals(championname, StringComparison.InvariantCultureIgnoreCase))
                {
                    champsumm = m_ChampionSummaries[i];
                    break;
                }
            }

            if (champsumm != null)
            {
                string path = Path.Combine(ChampionJsonFilesDir, string.Format("{0}.json", champsumm.Id));
                ChampionInfo champinfo;
                result = JsonHelper.DeserializeFromFile(path, out champinfo);             

                if (result)
                {
                    model = new ChampionModel()
                    {
                        ChampionSummary = champsumm,
                        ChampionInfo = champinfo
                    };
                }
            }

            return result;
        }
    }
}

