using LeagueReview.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace LeagueReview.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Microsoft.Extensions.Hosting.IHostEnvironment _hostingEnvironment;

        public HomeController(ILogger<HomeController> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var model = new LoadViewModel() { Teams = new List<LeagueTable>() };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(IFormFile resultsFile)
        {
            var model = new LoadViewModel() { Teams = new List<LeagueTable>()};

            try
            {
                string uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads");
                Directory.CreateDirectory(uploads);

                if (resultsFile != null  && resultsFile.Length > 0)
                {
                    model.CSVFile = resultsFile.FileName;

                    string filePath = Path.Combine(uploads, resultsFile.FileName);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        resultsFile.CopyTo(fileStream);
                    }

                    using (var streamReader = new StreamReader(resultsFile.OpenReadStream()))
                    {
                        using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                        {
                            var records = csvReader.GetRecords<GameResults>().ToList();

                            if (records == null || records.Count == 0)
                            {
                                ModelState.AddModelError("Teams", "There is no results data");
                            }
                            else
                            {
                                model.Teams = GetLeagueTables(records);                                
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return View(model);
        }

        public IActionResult Privacy()
            {
                return View();
            }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<LeagueTable> GetLeagueTables(List<GameResults> gameResults)
        {
            var leagueTable = new List<LeagueTable>();

            foreach (var gameResult in gameResults) 
            {
                //Home teams
                var leagueEntry = leagueTable.FirstOrDefault(x => x.TeamName == gameResult.HomeTeam);

                if (leagueEntry == null)
                {
                    leagueTable.Add(new LeagueTable() { TeamName = gameResult.HomeTeam,
                        GoalsScored = gameResult.FullTimeHomeTeamGoals,
                        GoalsConceded = gameResult.FullTimeAwayTeamGoals,
                        GoalDifference = gameResult.FullTimeHomeTeamGoals - gameResult.FullTimeAwayTeamGoals,
                        Points = gameResult.FullTimeResult == "H" ? 3 : 
                                 gameResult.FullTimeResult == "D" ? 1 : 0});
                }
                else
                {
                    leagueEntry.GoalsScored = leagueEntry.GoalsScored + gameResult.FullTimeHomeTeamGoals;
                    leagueEntry.GoalsConceded = leagueEntry.GoalsConceded + gameResult.FullTimeAwayTeamGoals;
                    leagueEntry.GoalDifference = leagueEntry.GoalDifference + 
                                            (gameResult.FullTimeHomeTeamGoals - gameResult.FullTimeAwayTeamGoals);
                    leagueEntry.Points = leagueEntry.Points + (gameResult.FullTimeResult == "H" ? 3 :
                                 gameResult.FullTimeResult == "D" ? 1 : 0);
                }

                // Away teams
                leagueEntry = leagueTable.FirstOrDefault(x => x.TeamName == gameResult.AwayTeam);

                if (leagueEntry == null)
                {
                    leagueTable.Add(new LeagueTable()
                    {
                        TeamName = gameResult.AwayTeam,
                        GoalsScored = gameResult.FullTimeAwayTeamGoals,
                        GoalsConceded = gameResult.FullTimeHomeTeamGoals,
                        GoalDifference = gameResult.FullTimeAwayTeamGoals - gameResult.FullTimeHomeTeamGoals,
                        Points = gameResult.FullTimeResult == "A" ? 3 :
                                 gameResult.FullTimeResult == "D" ? 1 : 0
                    });
                }
                else
                {
                    leagueEntry.GoalsScored = leagueEntry.GoalsScored + gameResult.FullTimeHomeTeamGoals;
                    leagueEntry.GoalsConceded = leagueEntry.GoalsConceded + gameResult.FullTimeAwayTeamGoals;
                    leagueEntry.GoalDifference = leagueEntry.GoalDifference + 
                                            (gameResult.FullTimeHomeTeamGoals - gameResult.FullTimeAwayTeamGoals);
                    leagueEntry.Points = leagueEntry.Points + (gameResult.FullTimeResult == "A" ? 3 :
                                 gameResult.FullTimeResult == "D" ? 1 : 0);
                }
            }

            // Get position in league
            var sortedLeague = leagueTable.OrderByDescending(x => x.Points).ThenByDescending(y => y.GoalDifference).
                                ThenByDescending(z => z.GoalsScored).ToList();

            int position = 1;
            LeagueTable prevTeam = new LeagueTable();

            foreach(var team in sortedLeague) 
            { 
                if (!string.IsNullOrEmpty(prevTeam.TeamName))
                {
                    if (prevTeam.Points != team.Points || prevTeam.GoalsConceded != team.GoalsConceded || prevTeam.GoalsScored != team.GoalsScored)
                    {
                        position++;
                    }
                }
                team.LeaguePosition = position;
                prevTeam = team;

            }

            return sortedLeague;
        }

    }
}