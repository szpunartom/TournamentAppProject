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
    public sealed partial class MatchDetailsPage : Page
    {
        private MobileServiceCollection<Game, Game> matches;
        private IMobileServiceTable<Game> matchesTable = Application.MobileService.GetTable<Game>();

        private MobileServiceCollection<TournamentTeam, TournamentTeam> tournamentTeams;
        private IMobileServiceTable<TournamentTeam> tournamentTeamsTable = Application.MobileService.GetTable<TournamentTeam>();

        public MatchDetailsPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Dictionary<string, string> matchInfo = (Dictionary<string, string>)e.Parameter;
            MatchIdTextBlock.Text = matchInfo["id"];
            TeamOneIdTextBlock.Text = matchInfo["teamOneId"];
            TeamTwoIdTextBlock.Text = matchInfo["teamTwoId"];
            TeamOneNameTextBlock.Text = matchInfo["teamOneName"];
            TeamTwoNameTextBlock.Text = matchInfo["teamTwoName"];
            TournamentIdTextBlock.Text = matchInfo["tournamentId"];
            EndMatchButton.IsEnabled = false;
            await RefreshMatch();
        }

        private async Task RefreshMatch()
        {
            matches = await matchesTable.Where(p => p.Id == MatchIdTextBlock.Text).ToCollectionAsync();
            var matchesArray = matches.ToArray();
            var match = matchesArray[0];
            TeamOneScoreTextBlock.Text = match.TeamOneScore.ToString();
            TeamTwoScoreTextBlock.Text = match.TeamTwoScore.ToString();

            if (match.Ended == false)
                EndMatchButton.IsEnabled = true;

        }

        private async Task UpdateTeamOneScore(int score)
        {
            matches = await matchesTable.Where(p => p.Id == MatchIdTextBlock.Text).ToCollectionAsync();
            Game match = matches.First();
            match.TeamOneScore = score;

            await matchesTable.UpdateAsync(match);
        }

        private async Task UpdateTeamTwoScore(int score)
        {
            matches = await matchesTable.Where(p => p.Id == MatchIdTextBlock.Text).ToCollectionAsync();
            Game match = matches.First();
            match.TeamTwoScore = score;

            await matchesTable.UpdateAsync(match);
        }

        private int ConvertStringToInt(string text)
        {
            int x = 0;
            Int32.TryParse(text, out x);
            return x;
        }

        private async void TeamOneScoreMinus_Click(object sender, RoutedEventArgs e)
        {
            int score = ConvertStringToInt(TeamOneScoreTextBlock.Text);
            if (score != 0)
                score--;

            await UpdateTeamOneScore(score);
            TeamOneScoreTextBlock.Text = score.ToString();

        }

        private async void TeamOneScorePlus_Click(object sender, RoutedEventArgs e)
        {
            int score = ConvertStringToInt(TeamOneScoreTextBlock.Text);
            score++;

            await UpdateTeamOneScore(score);
            TeamOneScoreTextBlock.Text = score.ToString();
        }

        private async void TeamTwoScoreMinus_Click(object sender, RoutedEventArgs e)
        {
            int score = ConvertStringToInt(TeamTwoScoreTextBlock.Text);
            if (score != 0)
                score--;

            await UpdateTeamTwoScore(score);
            TeamTwoScoreTextBlock.Text = score.ToString();
        }

        private async void TeamTwoScorePlus_Click(object sender, RoutedEventArgs e)
        {
            int score = ConvertStringToInt(TeamTwoScoreTextBlock.Text);
            score++;

            await UpdateTeamTwoScore(score);
            TeamTwoScoreTextBlock.Text = score.ToString();
        }

        private async void EndMatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConvertStringToInt(TeamOneScoreTextBlock.Text) != ConvertStringToInt(TeamTwoScoreTextBlock.Text))
            {
                if (ConvertStringToInt(TeamOneScoreTextBlock.Text) > ConvertStringToInt(TeamTwoScoreTextBlock.Text))
                {
                    tournamentTeams = await tournamentTeamsTable.Where(p => p.TeamId == TeamOneIdTextBlock.Text &&
                                                                            p.TournamentId == TournamentIdTextBlock.Text)
                                                                            .ToCollectionAsync();
                    TournamentTeam tournamentTeam = tournamentTeams.FirstOrDefault();
                    tournamentTeam.TeamWins = tournamentTeam.TeamWins + 1;

                    await tournamentTeamsTable.UpdateAsync(tournamentTeam);
                }
                else if (ConvertStringToInt(TeamOneScoreTextBlock.Text) < ConvertStringToInt(TeamTwoScoreTextBlock.Text))
                {
                    tournamentTeams = await tournamentTeamsTable.Where(p => p.TeamId == TeamTwoIdTextBlock.Text &&
                                                                            p.TournamentId == TournamentIdTextBlock.Text)
                                                                            .ToCollectionAsync();
                    TournamentTeam tournamentTeam = tournamentTeams.FirstOrDefault();
                    tournamentTeam.TeamWins = tournamentTeam.TeamWins + 1;

                    await tournamentTeamsTable.UpdateAsync(tournamentTeam);
                }

                tournamentTeams = await tournamentTeamsTable.Where(p => p.TeamId == TeamOneIdTextBlock.Text &&
                                                                   p.TournamentId == TournamentIdTextBlock.Text)
                                                                   .ToCollectionAsync();
                TournamentTeam tournamentTeamOne = tournamentTeams.FirstOrDefault();
                tournamentTeamOne.TeamPoints = tournamentTeamOne.TeamPoints + ConvertStringToInt(TeamOneScoreTextBlock.Text);
                await tournamentTeamsTable.UpdateAsync(tournamentTeamOne);

                tournamentTeams = await tournamentTeamsTable.Where(p => p.TeamId == TeamTwoIdTextBlock.Text &&
                                                                   p.TournamentId == TournamentIdTextBlock.Text)
                                                                   .ToCollectionAsync();
                TournamentTeam tournamentTeamTwo = tournamentTeams.FirstOrDefault();
                tournamentTeamTwo.TeamPoints = tournamentTeamTwo.TeamPoints + ConvertStringToInt(TeamTwoScoreTextBlock.Text);
                await tournamentTeamsTable.UpdateAsync(tournamentTeamTwo);

                matches = await matchesTable.Where(p => p.Id == MatchIdTextBlock.Text).ToCollectionAsync();
                Game match = matches.First();
                match.Ended = true;
                await matchesTable.UpdateAsync(match);

                EndMatchButton.IsEnabled = false;
            }
            else
            {
                await new MessageDialog("Nie może być remisu").ShowAsync();
            }
        }
    }
}
