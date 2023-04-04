using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRExample.API.Hubs;

namespace SignalRExample.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
   private readonly IHubContext<MyHub> _hubContext;

   public NotificationController(IHubContext<MyHub> hubContext)
   {
      _hubContext = hubContext;
   }
   
   [HttpGet("{teamCount:int}")]
   public async Task<IActionResult> SetTeamCount(int teamCount)
   {
      MyHub.TeamCount = teamCount;
      await _hubContext.Clients.All.SendAsync("Notify", $"Arkadaşlar takımlar {teamCount} kişi olacaktır.");
      return Ok();
   }
}