using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TournamentAppProject.DataModel;
using Windows.UI.Popups;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TournamentAppProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MatchesListPage : Page
    {
        private MobileServiceCollection<Game, Game> matches;
        private IMobileServiceTable<Game> matchesTable = Application.MobileService.GetTable<Game>();

        private MobileServiceCollection<Team, Team> teams;
        private IMobileServiceTable<Team> teamsTable = Application.MobileService.GetTable<Team>();

        private MobileServiceCollection<TournamentTeam, TournamentTeam> tournamentTeams;
        private IMobileServiceTable<TournamentTeam> tournamentTeamsTable = Application.MobileService.GetTable<TournamentTeam>();

        public MatchesListPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Dictionary<string, string> tournamentInfo = (Dictionary<string, string>)e.Parameter;
            TournamentIdTextBlock.Text = tournamentInfo["id"];
            TournamentNameTextBlock.Text = tournamentInfo["name"];
            await CreateMatches();
            await RefreshMatches();
        }

        private async Task CreateMatches()
        {
            matches = await matchesTable.Where(p => p.TournamentId == TournamentIdTextBlock.Text ).ToCollectionAsync();
            if (matches.Count == 0)
            {
                tournamentTeams = await tournamentTeamsTable.Where(p => p.TournamentId == TournamentIdTextBlock.Text).ToCollectionAsync();
                var teamsIds = tournamentTeams.Select(p => p.TeamId).ToArray();

                if (teamsIds.Length != 0)
                {
                    teams = await teamsTable.Where(p => teamsIds.Contains(p.Id)).ToCollectionAsync();

                    Game match0 = new Game { TeamOneScore = 0, TeamTwoScore = 0, TeamOneId = teamsIds[0], TeamTwoId = teamsIds[3], Ended = false,
                        TournamentId = TournamentIdTextBlock.Text, Nr = 1 };
                    await matchesTable.InsertAsync(match0);

                    Game match1 = new Game { TeamOneScore = 0, TeamTwoScore = 0, TeamOneId = teamsIds[1], TeamTwoId = teamsIds[2], Ended = false,
                        TournamentId = TournamentIdTextBlock.Text, Nr = 2 };
                    await matchesTable.InsertAsync(match1);

                    Game match2 = new Game { TeamOneScore = 0, TeamTwoScore = 0, TeamOneId = teamsIds[3], TeamTwoId = teamsIds[2], Ended = false,
                        TournamentId = TournamentIdTextBlock.Text, Nr = 3 };
                    await matchesTable.InsertAsync(match2);

                    Game match3 = new Game { TeamOneScore = 0, TeamTwoScore = 0, TeamOneId = teamsIds[0], TeamTwoId = teamsIds[1], Ended = false,
                        TournamentId = TournamentIdTextBlock.Text, Nr = 4 };
                    await matchesTable.InsertAsync(match3);

                    Game match4 = new Game { TeamOneScore = 0, TeamTwoScore = 0, TeamOneId = teamsIds[1], TeamTwoId = teamsIds[3], Ended = false,
                        TournamentId = TournamentIdTextBlock.Text, Nr = 5 };
                    await matchesTable.InsertAsync(match4);

                    Game match5 = new Game { TeamOneScore = 0, TeamTwoScore = 0, TeamOneId = teamsIds[2], TeamTwoId = teamsIds[0], Ended = false,
                        TournamentId = TournamentIdTextBlock.Text, Nr = 6 };
                    await matchesTable.InsertAsync(match5);
                }
            }
        }

        private async Task RefreshMatches()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems
                MyProgressRing.Visibility = Visibility.Visible;

                matches = await matchesTable.Where(p => p.TournamentId == TournamentIdTextBlock.Text).OrderBy(p => p.Nr).ToCollectionAsync();
                var matchesIds = matches.Select(p => p.Id).ToArray();
                teams = await teamsTable.ToCollectionAsync();

                if (matchesIds.Length != 0)
                    matches = await matchesTable.Where(p => matchesIds.Contains(p.Id)).ToCollectionAsync();                 

            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading matches.").ShowAsync();
            }
            else
            {
                var matchesTeams = matches.Join(teams,
                                                m => m.TeamOneId,
                                                t => t.Id,
                                                (m, t) => new
                                                {
                                                    Id = m.Id,
                                                    TeamOneId = m.TeamOneId,
                                                    TeamTwoId = m.TeamTwoId,
                                                    TeamOneScore = m.TeamOneScore,
                                                    TeamTwoScore = m.TeamTwoScore,
                                                    TeamOneName = t.Name,
                                                    Ended = m.Ended,
                                                    Nr = m.Nr,
                                                    TournamentId = m.TournamentId
                                                });

                var matchesTeams2 = matchesTeams.Join(teams,
                                                m => m.TeamTwoId,
                                                t => t.Id,
                                                (m, t) => new GameHelper
                                                {
                                                    Id = m.Id,
                                                    TeamOneId = m.TeamOneId,
                                                    TeamTwoId = m.TeamTwoId,
                                                    TeamOneScore = m.TeamOneScore,
                                                    TeamTwoScore = m.TeamTwoScore,
                                                    TeamOneName = m.TeamOneName,
                                                    TeamTwoName = t.Name,
                                                    Ended = m.Ended,
                                                    Nr = m.Nr,
                                                    TournamentId = m.TournamentId
                                                });

                MatchesListListView.ItemsSource = matchesTeams2;
                MyProgressRing.Visibility = Visibility.Collapsed;
            }
        }

        private void MatchesListListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = MatchesListListView.SelectedItem;
            var match = selectedItem as GameHelper;
            Dictionary<string, string> matchInfo = new Dictionary<string, string>();
            matchInfo["id"] = match.Id;
            matchInfo["teamOneId"] = match.TeamOneId;
            matchInfo["teamTwoId"] = match.TeamTwoId;
            matchInfo["teamOneName"] = match.TeamOneName;
            matchInfo["teamTwoName"] = match.TeamTwoName;
            matchInfo["tournamentId"] = match.TournamentId;
            Frame.Navigate(typeof(MatchDetailsPage), matchInfo);
        }
    }
}
