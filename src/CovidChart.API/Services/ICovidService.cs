using CovidChart.API.Models;

namespace CovidChart.API.Services;

public interface ICovidService
{
    IQueryable<Covid> GetList();
    Task SaveCovid(Covid covid);
    List<Models.CovidChart> GetCovidChartList();
}