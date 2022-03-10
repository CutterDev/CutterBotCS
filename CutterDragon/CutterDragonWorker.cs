using CutterDragon.Assets;
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
        #region CDragon URLS

        public const string LOL_GAME_PATH = "/lol-game-data/assets/";

        public const string CDATA_ASSET_PATH = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/";

        private const string CDATA_CHAMPIONSUMMARYJSON = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-summary.json";

        private const string CDATA_CHAMPIONJSON = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champions/{0}.json";

        private const string CDATA_ITEM_JSON = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/en_gb/v1/items.json";

        private const string CDATA_PERKS_JSON = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/en_gb/v1/perkstyles.json";

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
        /// Get Assets
        /// </summary>
        public void GetAssets()
        {
            GetChampionAssets();

            GetItemAssets();

            GetPerkStyleAssets();
        }

        /// <summary>
        /// Get Champion Assets
        /// 
        /// </summary>
        private void GetChampionAssets()
        {
            ChampionSummary[] champions;

            if (TryGetChampionSummaries(out champions))
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
        /// Create Directories
        /// </summary>
        private void CreateDirectories()
        {
            // DATA DIR
            CreateDirectory(CutterDragonConsts.CUTTERDRAGON_DIR);

            // Champions Directory
            CreateDirectory(CutterDragonConsts.CHAMPION_DIR);

            // Champion Icons Directory
            CreateDirectory(CutterDragonConsts.CHAMPION_ICONS_DIR);

            // Champion Json Files Directory
            CreateDirectory(CutterDragonConsts.CHAMPION_JSON_DIR);

            // Asset Directory
            CreateDirectory(CutterDragonConsts.ASSETS_DIR);

            // Asset Icons Dir
            CreateDirectory(CutterDragonConsts.ASSETS_ITEMS_DIR);

            // Asset Runes Dir
            CreateDirectory(CutterDragonConsts.ASSETS_RUNES_DIR);
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

        #region Champion Asset Methods
        private void GetChampionSummariesFromFile()
        {
            m_ChampionSummaries = new ChampionSummary[0];

            ChampionSummary[] champs;
            if (TryGetChampionSummaries(out champs))
            {
                m_ChampionSummaries = champs;
            }
        }

        /// <summary>
        /// Try Get Champion Stats
        /// </summary>
        private bool TryGetChampionSummaries(out ChampionSummary[] champions)
        {
            champions = null;
            bool result = false;

            try
            {
                using (WebClient wc = new WebClient())
                {
                    var json = wc.DownloadString(CDATA_CHAMPIONSUMMARYJSON);

                    File.WriteAllText(Path.Combine(CutterDragonConsts.CHAMPION_DIR, "champions.json"), json);

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
                    string iconpath = Path.Combine(CutterDragonConsts.CHAMPION_ICONS_DIR, string.Format("{0}.png", champion.Id));
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
                    client.DownloadFile(new Uri(string.Format(CDATA_CHAMPIONJSON, championid)), 
                                        Path.Combine(CutterDragonConsts.CHAMPION_JSON_DIR, string.Format("{0}.json", championid)));
                }
            }
            catch (Exception e)
            {
                
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

            for (int i = 0; i < m_ChampionSummaries.Length; i++)
            {
                if (m_ChampionSummaries[i].Name.Equals(championname, StringComparison.InvariantCultureIgnoreCase))
                {
                    champsumm = m_ChampionSummaries[i];
                    break;
                }
            }

            if (champsumm != null)
            {
                string path = Path.Combine(CutterDragonConsts.CHAMPION_JSON_DIR, string.Format("{0}.json", champsumm.Id));
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

        #endregion

        #region Assets Items

        private void GetItemAssets()
        {
            ItemSummary[] items;

            if (TryGetItemSummaries(out items))
            {
                for(int i = 0; i < items.Length; i++)
                {
                    DownloadItemIcon(items[i].IconPath, items[i].Id);
                }
            }
        }

        /// <summary>
        /// Try Get Item Summaries
        /// </summary>
        private bool TryGetItemSummaries(out ItemSummary[] itemsummaries)
        {
            bool result = false;

            itemsummaries = null;
            string itemjsonfile = Path.Combine(CutterDragonConsts.ASSETS_DIR, "items.json");

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(CDATA_ITEM_JSON), itemjsonfile);
                }

                if (File.Exists(itemjsonfile))
                {
                    result = JsonHelper.DeserializeFromFile(itemjsonfile, out itemsummaries);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Error: {0}", e.Message));
            }

            return result;
        }

        /// <summary>
        /// Download Item Icon
        /// </summary>
        private void DownloadItemIcon(string itempath, int iconid)
        {
            if (!string.IsNullOrWhiteSpace(itempath))
            {
                string cdatapath = itempath.Replace(LOL_GAME_PATH, string.Empty).ToLower();
                string cdataitempath = CDATA_ASSET_PATH + cdatapath;
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(new Uri(cdataitempath), Path.Combine(CutterDragonConsts.ASSETS_ITEMS_DIR, string.Format("{0}.png", iconid)));
                    }
                }
                catch(Exception e)
                {

                }
            }     
        }

        #region PerkStyles (Runes)

        /// <summary>
        /// Get Runes
        /// </summary>
        private void GetPerkStyleAssets()
        {
            PerkStyle[] perkstyles;

            if (GetPerkStyles(out perkstyles))
            {

            }
        }

        /// <summary>
        /// Get Perk Styles
        /// </summary>
        private bool GetPerkStyles(out PerkStyle[] perkstyles)
        {
            bool result = false;

            perkstyles = null;
            string itemjsonfile = Path.Combine(CutterDragonConsts.ASSETS_DIR, "stats.json");

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(CDATA_PERKS_JSON), itemjsonfile);
                }

                if (File.Exists(itemjsonfile))
                {
                    result = JsonHelper.DeserializeFromFile(itemjsonfile, out perkstyles);
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        #endregion

        #endregion
    }
}

