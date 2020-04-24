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
    public sealed partial class TournamentLadderPage : Page
    {
        private MobileServiceCollection<Team, Team> teams;
        private IMobileServiceTable<Team> teamsTable = Application.MobileService.GetTable<Team>();

        private MobileServiceCollection<TournamentTeam, TournamentTeam> tournamentTeams;
        private IMobileServiceTable<TournamentTeam> tournamentTeamsTable = Application.MobileService.GetTable<TournamentTeam>();
        
        public TournamentLadderPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Dictionary<string, string> tournamentInfo = (Dictionary<string, string>)e.Parameter;
            TournamentIdTextBlock.Text = tournamentInfo["id"];
            TournamentNameTextBlock.Text = tournamentInfo["name"];
            TournamentDateTextBlock.Text = String.Format("Data turnieju: {0}.", tournamentInfo["date"] );
            await RefreshTeams();
            await RefreshComboBoxTeams();
        }

        private async Task RefreshTeams()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems
                MyProgressRing.Visibility = Visibility.Visible;

                //tournamentTeams = await tournamentTeamsTable.Where(p => p.TournamentId == TournamentIdTextBlock.Text).ToCollectionAsync();
                //var teamsIds = tournamentTeams.Select(p => p.TeamId).ToArray();

                //if (teamsIds.Length != 0)
                //    teams = await teamsTable.Where(p => teamsIds.Contains(p.Id)).ToCollectionAsync();

                tournamentTeams = await tournamentTeamsTable.Where(p => p.TournamentId == TournamentIdTextBlock.Text).ToCollectionAsync();
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
                var specificTeams = teams.Join(tournamentTeams,
                                            t => t.Id,
                                            tT => tT.TeamId,
                                            (t, tT) => new { Id = t.Id, Name = t.Name, Wins = tT.TeamWins, Points = tT.TeamPoints })
                                            .OrderByDescending(p => p.Points).OrderByDescending(p => p.Wins);

                TeamsListListView.ItemsSource = specificTeams;
                MyProgressRing.Visibility = Visibility.Collapsed;

                tournamentTeams = await tournamentTeamsTable.Where(p => p.TournamentId == TournamentIdTextBlock.Text).ToCollectionAsync();
                var numberOfTeams = tournamentTeams.Count;
                var teamsIds = tournamentTeams.Select(p => p.TeamId).ToArray();

                if (numberOfTeams < 4)
                {
                    AssignTeamsTextBlock.Visibility = Visibility.Visible;
                }
            }
        }

        private async Task RefreshComboBoxTeams()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems

                tournamentTeams = await tournamentTeamsTable.Where(p => p.TournamentId == TournamentIdTextBlock.Text).ToCollectionAsync();
                var teamsIds = tournamentTeams.Select(p => p.TeamId).ToArray();
                if (teamsIds.Length != 0)
                    teams = await teamsTable.Where(p => !teamsIds.Contains(p.Id)).ToCollectionAsync();
                else
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
                

                TeamsListComboBox.ItemsSource = teams;
                this.AssignTeamButton.IsEnabled = true;
            }
        }

        private async Task InsertTournamentTeam(string teamId)
        {
            string tournamentId = TournamentIdTextBlock.Text;
            TournamentTeam tournamentTeam = new TournamentTeam { TournamentId = tournamentId, TeamId = teamId, TeamWins = 0 };
            await tournamentTeamsTable.InsertAsync(tournamentTeam);
            tournamentTeams.Add(tournamentTeam);

            //await App.MobileService.SyncContext.PushAsync(); // offline sync
        }

        private async void AssignTeamButton_Click(object sender, RoutedEventArgs e)
        {
            tournamentTeams = await tournamentTeamsTable.Where(p => p.TournamentId == TournamentIdTextBlock.Text).ToCollectionAsync();
            var numberOfTeams = tournamentTeams.Count;
            var teamsIds = tournamentTeams.Select(p => p.TeamId).ToArray();

            if (numberOfTeams < 4)
            {
                var selectedItem = TeamsListComboBox.SelectedItem;
                var team = selectedItem as Team;
                string teamId = team.Id;

                await InsertTournamentTeam(teamId);
                await RefreshTeams();
                await RefreshComboBoxTeams();
            }
            else await new MessageDialog("Za dużo drużyn w turnieju.").ShowAsync();
        }

        private async void MatchesListPageButton_Click(object sender, RoutedEventArgs e)
        {
            tournamentTeams = await tournamentTeamsTable.Where(p => p.TournamentId == TournamentIdTextBlock.Text).ToCollectionAsync();
            var teamsIds = tournamentTeams.Select(p => p.TeamId).ToArray();
            if (teamsIds.Length != 4)
                await new MessageDialog("4 drużyny muszą zostać przypisane do turnieju!").ShowAsync();
            else
            {
                Dictionary<string, string> tournamentInfo = new Dictionary<string, string>();

                tournamentInfo["id"] = TournamentIdTextBlock.Text;
                tournamentInfo["name"] = TournamentNameTextBlock.Text;
                tournamentInfo["date"] = TournamentDateTextBlock.Text;

                TournamentIdTextBlock.Text = tournamentInfo["id"];
                TournamentNameTextBlock.Text = tournamentInfo["name"];
                TournamentDateTextBlock.Text = String.Format("Data turnieju: {0}.", tournamentInfo["date"]);

                Frame.Navigate(typeof(MatchesListPage), tournamentInfo);
            }
        }
    }
}
