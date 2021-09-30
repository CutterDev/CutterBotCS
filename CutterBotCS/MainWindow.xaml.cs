using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CutterBotCS
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
            m_DiscordBot = new DiscordBot(tokenPwdBox.Password);
            m_DiscordBot.connected += DiscordBot_Connected;

            await m_DiscordBot.Initialize();
        }

        /// <summary>
        /// Show Left Button Down
        /// </summary>
        private void Show_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tokenPwdBox.Visibility = Visibility.Collapsed;
            tokenTxtBox.Text = tokenPwdBox.Password;
            tokenTxtBox.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hide Left Button Up
        /// </summary>
        private void Hide_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tokenPwdBox.Visibility = Visibility.Visible;
            tokenTxtBox.Text = string.Empty;
            tokenTxtBox.Visibility = Visibility.Collapsed;
        }
    }
}
