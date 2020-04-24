using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TournamentAppProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            MyFrame.Navigate(typeof(WelcomePage));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void NavigationMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = NavigationMenu.Items.Cast<ListBoxItem>()
                .Where(p => p.IsSelected)
                .Select(t => t.Name.ToString())
                .ToArray();

            if (selectedItems[0] == "TournamentsListBoxItem")
            {
                MyFrame.Navigate(typeof(TournamentsPage));
            }
            else if (selectedItems[0] == "TeamsListBoxItem")
            {
                MyFrame.Navigate(typeof(TeamsPage));
            }
            else if (selectedItems[0] == "HomeListBoxItem")
            {
                MyFrame.Navigate(typeof(WelcomePage));
            }
            else if (selectedItems[0] == "AboutListBoxItem")
            {
                MyFrame.Navigate(typeof(AboutPage));
            }
            else if (selectedItems[0] == "ExitListBoxItem")
            {
                Windows.UI.Xaml.Application.Current.Exit();
            }
        }
    }
}
