using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentAppProject.DataModel
{
    public class TournamentTeam
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "tournamentId")]
        public string TournamentId { get; set; }

        [JsonProperty(PropertyName = "teamId")]
        public string TeamId { get; set; }

        [JsonProperty(PropertyName = "teamWins")]
        public int TeamWins { get; set; }

        [JsonProperty(PropertyName = "teamPoints")]
        public int TeamPoints { get; set; }

    }
}
