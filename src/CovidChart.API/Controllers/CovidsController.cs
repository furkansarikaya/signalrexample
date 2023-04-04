using CovidChart.API.Models;
using CovidChart.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CovidChart.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CovidsController : ControllerBase
{
    private readonly ICovidService _covidService;

    public CovidsController(ICovidService covidService)
    {
        _covidService = covidService;
    }

    [HttpGet]
    public IActionResult GetChartList()
    {
        return Ok(_covidService.GetCovidChartList());
    }

    [HttpPost]
    public async Task<IActionResult> SaveCovid(Covid covid)
    {
        await _covidService.SaveCovid(covid);
        return Ok(_covidService.GetCovidChartList());
    }
    
    [HttpPost("initializeCovid")]
    public IActionResult InitializeCovid()
    {
        var rnd = new Random();

        Enumerable.Range(1, 10).ToList().ForEach(x =>
        {
            foreach (ECity item in Enum.GetValues(typeof(ECity)))
            {
                var covid = new Covid { City = item, Count = rnd.Next(100, 1000), CovidDate = DateTime.Now.AddDays(x) };
                _covidService.SaveCovid(covid).Wait();
                Thread.Sleep(1000);
            }
        });

        return Ok("Covid19 dataları veritabanına kaydedildi");
    }
}