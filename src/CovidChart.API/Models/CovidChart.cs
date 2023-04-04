namespace CovidChart.API.Models;

public class CovidChart
{
    public string CovidDate { get; set; }

    public List<int> Counts { get; set; } = new();
}