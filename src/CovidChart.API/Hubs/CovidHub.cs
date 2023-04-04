using CovidChart.API.Services;
using Microsoft.AspNetCore.SignalR;

namespace CovidChart.API.Hubs;

public class CovidHub:Hub
{
    private readonly ICovidService _covidService;

    public CovidHub(ICovidService covidService)
    {
        _covidService = covidService;
    }

    public async Task GetCovidList()
    {
        await Clients.All.SendAsync("ReceiveCovidList", _covidService.GetCovidChartList());
    }
}