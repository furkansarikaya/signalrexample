using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRExample.API.Models;

namespace SignalRExample.API.Hubs;

public class MyHub : Hub<IMyHub>
{
    private readonly AppDbContext _context;

    public MyHub(AppDbContext context)
    {
        _context = context;
    }

    private static List<string> Names { get; set; } = new();
    private static int ClientCount { get; set; } = 0;
    public static int TeamCount { get; set; } = 7;

    public async Task SendName(string name)
    {
        if (Names.Count >= TeamCount)
        {
            await Clients.Caller.Error($"Takım en fazla {TeamCount} kişi olabilir");
        }

        Names.Add(name);
        await Clients.All.ReceiveName(name);
    }

    public async Task GetNames()
    {
        await Clients.All.ReceiveNames(Names);
    }

    //Groups

    public async Task AddToGroup(string teamName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
    }

    public async Task SendNameByGroup(string name, string teamName)
    {
        var team = await _context.Teams.FirstOrDefaultAsync(x => x.Name == teamName);
        if (team != null)
        {
            team.Users.Add(new User { Name = name });
        }
        else
        {
            team = new Team { Name = teamName };
            team.Users.Add(new User { Name = name });
            _context.Teams.Add(team);
        }

        await _context.SaveChangesAsync();

        await Clients.Group(teamName).ReceiveMessageByGroup(name, team.Id);
    }

    public async Task GetNamesByGroup()
    {
        var teams = await _context.Teams.Include(x => x.Users).Select(x => new
        {
            teamId = x.Id,
            Users = x.Users.ToList()
        }).ToListAsync();

        await Clients.All.ReceiveNamesByGroup(teams);
    }

    public async Task RemoveToGroup(string teamName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
    }

    public override async Task OnConnectedAsync()
    {
        ClientCount++;
        await Clients.All.ReceiveClientCount(ClientCount);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        ClientCount--;
        await Clients.All.ReceiveClientCount(ClientCount);
        await base.OnDisconnectedAsync(exception);
    }
}