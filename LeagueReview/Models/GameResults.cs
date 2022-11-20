using CsvHelper.Configuration.Attributes;

namespace LeagueReview.Models
{
    public class GameResults
    {
        [Name("Div")]
        public string Division { get; set; }

        [Name("Date")]
        public string DatePlayed { get; set; }

        [Name("Time")]
        public string TimePlayed { get; set; }

        [Name("HomeTeam")]
        public string HomeTeam { get; set; }

        [Name("AwayTeam")]
        public string AwayTeam { get; set; }

        [Name("FTHG")]
        public int FullTimeHomeTeamGoals { get; set; }

        [Name("FTAG")]
        public int FullTimeAwayTeamGoals { get; set; }

        [Name("FTR")]
        public string FullTimeResult { get; set; }

        [Name("HTHG")]
        public int HalfTimeHomeTeamGoals { get; set; }

        [Name("HTAG")]
        public int HalfTimeAwayTeamGoals { get; set; }

        [Name("HTR")]
        public string HalfTimeResult { get; set; }


        [Name("Referee")]
        public string Referee { get; set; }

    }
}
