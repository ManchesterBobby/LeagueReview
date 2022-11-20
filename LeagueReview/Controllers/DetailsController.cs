using CsvHelper;
using LeagueReview.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace LeagueReview.Controllers
{
    public class DetailsController : Controller
    {
        private readonly Microsoft.Extensions.Hosting.IHostEnvironment _hostingEnvironment;

        public DetailsController(IHostEnvironment hostEnvironment)
        {
            _hostingEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var model = new DetailsViewModel() { TeamResults = new List<GameResults>() };
            return View(model);
        }

        public IActionResult Display(string team, string file)
        {
            var model = new DetailsViewModel() { TeamResults = new List<GameResults>() };

            try
            {
                string uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads");

                if (!string.IsNullOrWhiteSpace(file))
                {
                    string filePath = Path.Combine(uploads, file);

                    model.CSVFile = file;
                    model.TeamName = team;
                    using (var streamReader = new StreamReader(filePath))
                    {
                        using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                        {
                            var records = csvReader.GetRecords<GameResults>().ToList();

                            if (records == null || records.Count == 0)
                            {
                                ModelState.AddModelError("TeamResults", "There is no results data");
                            }
                            else
                            {
                                model.TeamResults = records.Where(x => x.AwayTeam == team || x.HomeTeam == team).ToList();
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

    }
}
