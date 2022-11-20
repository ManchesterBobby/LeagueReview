namespace LeagueReview.Models
{
    public class LeagueTable
    {
        public int LeaguePosition { get; set; }

        public string TeamName { get; set; }

        public int GoalsScored { get; set; } 

        public int GoalsConceded { get; set; }

        public int GoalDifference { get; set; }

        public int Points { get; set; }
    }
}
