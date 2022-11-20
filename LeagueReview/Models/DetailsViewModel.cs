namespace LeagueReview.Models
{
    public class DetailsViewModel
    {
        public string CSVFile { get; set; }

        public string TeamName { get; set; }

        public List<GameResults> TeamResults { get; set; }

    }
}
