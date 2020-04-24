using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TournamentAppProject.DataModel;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TournamentAppProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TeamsPage : Page
    {
        private MobileServiceCollection<Team, Team> teams;
        private IMobileServiceTable<Team> teamsTable = Application.MobileService.GetTable<Team>();

        public TeamsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //await InitLocalStoreAsync(); // offline sync
            RefreshTeamsButton_Click(this, null);
        }

        private async Task InsertTeam(Team team)
        {
            // This code inserts a new Tournament into the database. When the operation completes
            // and Mobile Apps has assigned an Id, the item is added to the CollectionView
            await teamsTable.InsertAsync(team);
            teams.Add(team);

            //await App.MobileService.SyncContext.PushAsync(); // offline sync
        }

        private async Task DeleteTeam(Team team)
        {
            // This code inserts a new Tournament into the database. When the operation completes
            // and Mobile Apps has assigned an Id, the item is added to the CollectionView
            await teamsTable.DeleteAsync(team);
            teams.Remove(team);

            //await App.MobileService.SyncContext.PushAsync(); // offline sync
        }

        private async Task RefreshTeams()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems
                teams = await teamsTable.ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading teams").ShowAsync();
            }
            else
            {
                TeamsListItems.ItemsSource = teams;
                AddTeamButton.IsEnabled = true;
                MyProgressRing.Visibility = Visibility.Collapsed;
            }
        }

        private async void AddTeamButton_Click(object sender, RoutedEventArgs e)
        {
            var team = new Team { Name = TeamNameTextBox.Text };
            TeamNameTextBox.Text = "";
            await InsertTeam(team);
        }

        private async void RefreshTeamsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshTeamsButton.IsEnabled = false;

            //await SyncAsync(); // offline sync
            await RefreshTeams();

            RefreshTeamsButton.IsEnabled = true;
        }

        private async void DeleteTeamButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Team team = btn.DataContext as Team;
            await DeleteTeam(team);
        }

    }
}
