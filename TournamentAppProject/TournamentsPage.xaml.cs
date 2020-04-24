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
    public sealed partial class TournamentsPage : Page
    {
        private MobileServiceCollection<Tournament, Tournament> tournaments;
        private IMobileServiceTable<Tournament> tournamentsTable = Application.MobileService.GetTable<Tournament>();

        public TournamentsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //await InitLocalStoreAsync(); // offline sync
            RefreshTournamentsButton_Click(this, null);
        }

        private async Task InsertTournament(Tournament tournament)
        {
            // This code inserts a new Tournament into the database. When the operation completes
            // and Mobile Apps has assigned an Id, the item is added to the CollectionView
            await tournamentsTable.InsertAsync(tournament);
            tournaments.Add(tournament);

            //await App.MobileService.SyncContext.PushAsync(); // offline sync
        }

        private async Task DeleteTournament(Tournament tournament)
        {
            // This code inserts a new Tournament into the database. When the operation completes
            // and Mobile Apps has assigned an Id, the item is added to the CollectionView
            await tournamentsTable.DeleteAsync(tournament);
            tournaments.Remove(tournament);

            //await App.MobileService.SyncContext.PushAsync(); // offline sync
        }

        private async Task RefreshTournaments()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems
                tournaments = await tournamentsTable.ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading tournaments").ShowAsync();
            }
            else
            {
                TournamentsListItems.ItemsSource = tournaments;
                AddTorunamentButton.IsEnabled = true;
                MyProgressRing.Visibility = Visibility.Collapsed;
            }
        }

        private async void AddTorunamentButton_Click(object sender, RoutedEventArgs e)
        {
            var tournament = new Tournament { Name = TournamentNameTextBox.Text, Date = DateTime.Now };
            TournamentNameTextBox.Text = "";
            await InsertTournament(tournament);
        }

        private async void RefreshTournamentsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshTournamentsButton.IsEnabled = false;

            //await SyncAsync(); // offline sync
            await RefreshTournaments();

            RefreshTournamentsButton.IsEnabled = true;
        }

        private async void DeleteTournamentButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Tournament tournament = btn.DataContext as Tournament;
            await DeleteTournament(tournament);
        }

        private void TournamentsListItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = TournamentsListItems.SelectedItem;           
            var tournament = selectedItem as Tournament;
            Dictionary<string, string> tournamentInfo = new Dictionary<string, string>();
            tournamentInfo["id"] = tournament.Id;
            tournamentInfo["name"] = tournament.Name;
            tournamentInfo["date"] = tournament.Date.ToString();

            Frame.Navigate(typeof(TournamentLadderPage), tournamentInfo);
        }
    }
}
