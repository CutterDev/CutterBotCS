using System.Threading;
using System.Windows;
using System.Windows.Media;
using CutterBotCS.Discord;

namespace CutterBotCS.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Discord Bot
        /// </summary>
        private DiscordBot m_DiscordBot;

        /// <summary>
        /// Ctor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            m_DiscordBot = new DiscordBot();
        }

        /// <summary>
        /// Discord Bot Connected
        /// </summary>
        private void DiscordBot_Connected()
        {
            Application.Current.Dispatcher.BeginInvoke((ThreadStart)delegate {
                txtStatus.Text = "Connected";
                txtStatus.Foreground = Brushes.DarkGreen;
            });
        }

        /// <summary>
        /// Login Click Event
        /// </summary>
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            m_DiscordBot.connected += DiscordBot_Connected;

            await m_DiscordBot.Initialize();
        }

        /// <summary>
        /// Config Click
        /// </summary>
        private void Config_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow cw = new ConfigWindow();

            if (cw.ShowDialog().Value)
            {

            }
        }
    }
}
