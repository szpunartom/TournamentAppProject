using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentAppProject.DataModel
{
    public class Game
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "teamOneScore")]
        public int TeamOneScore { get; set; }

        [JsonProperty(PropertyName = "teamTwoScore")]
        public int TeamTwoScore { get; set; }

        [JsonProperty(PropertyName = "teamOneId")]
        public string TeamOneId { get; set; }

        [JsonProperty(PropertyName = "teamTwoId")]
        public string TeamTwoId { get; set; }

        [JsonProperty(PropertyName = "tournamentId")]
        public string TournamentId { get; set; }

        [JsonProperty(PropertyName = "ended")]
        public bool Ended { get; set; }

        [JsonProperty(PropertyName = "nr")]
        public int Nr { get; set; }
    }
}
