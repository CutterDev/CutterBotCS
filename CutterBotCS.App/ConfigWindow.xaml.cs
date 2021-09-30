using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CutterBotCS.Properties;

namespace CutterBotCS.App
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
            LoadUI();
        }

        /// <summary>
        /// Load Config To UI
        /// </summary>
        void LoadUI()
        {
            tbDiscordToken.Text = Settings.Default.DiscordToken;
            tbRiotToken.Text = Settings.Default.RiotApiToken;
            tbPrefix.Text = Settings.Default.CommandPrefix.ToString();
        }

        /// <summary>
        /// Save UI to Config
        /// </summary>
        void SaveUI()
        {
            Settings.Default.DiscordToken = tbDiscordToken.Text;
            Settings.Default.RiotApiToken = tbRiotToken.Text;
            Settings.Default.CommandPrefix = tbPrefix.Text.ToCharArray()[0];
        }

        /// <summary>
        /// Prefix PreviewKeyDown
        /// </summary>
        private void tbPrefix_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender != null && sender is TextBox)
            {
                TextBox tb = sender as TextBox;

                if (tb.Text.Length > 0 && e.Key != Key.Back)
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Cancel Click
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Ok Click
        /// </summary>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            SaveUI();
            DialogResult = true;
        }
    }
}
