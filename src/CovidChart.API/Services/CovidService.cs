using CovidChart.API.Data;
using CovidChart.API.Hubs;
using CovidChart.API.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CovidChart.API.Services;

public class CovidService : ICovidService
{
    private readonly AppDbContext _context;
    private readonly IHubContext<CovidHub> _hubContext;

    public CovidService(AppDbContext context, IHubContext<CovidHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public IQueryable<Covid> GetList()
    {
        return _context.Covids.AsQueryable();
    }

    public async Task SaveCovid(Covid covid)
    {
        await _context.Covids.AddAsync(covid);
        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveCovidList", GetCovidChartList());
    }

    public List<Models.CovidChart> GetCovidChartList()
    {
        List<Models.CovidChart> covidCharts = new();

        using var command = _context.Database.GetDbConnection().CreateCommand();
        command.CommandText = @"SELECT *
                                FROM CROSSTAB(
                                                'select cast(""CovidDate"" as date) as CovidDate,""City"",""Count"" from ""Covids""',
                                                'SELECT m FROM GENERATE_SERIES(1,5) m') AS (""covidDate"" date,
                                                ""1"" numeric, ""2"" numeric, ""3"" numeric, ""4"" numeric,
                                                ""5"" numeric) order by ""covidDate"";";

        command.CommandType = System.Data.CommandType.Text;

        _context.Database.OpenConnection();

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Models.CovidChart cc = new()
                {
                    CovidDate = reader.GetDateTime(0).ToShortDateString()
                };

                Enumerable.Range(1, 5).ToList().ForEach(x =>
                {
                    cc.Counts.Add(System.DBNull.Value.Equals(reader[x]) ? 0 : reader.GetInt32(x));
                });

                covidCharts.Add(cc);
            }
        }

        _context.Database.CloseConnection();

        return covidCharts;
    }
}