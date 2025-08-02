using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;

namespace Peyghoom.Endpoints.ChatHub;

[Authorize]
public class ChatHub: Hub
{
   private static readonly Dictionary<string, HashSet<string>> _usersConnections;// TODO:README ConcurrentDic
   public async Task SendMessageAsync(string message, ObjectId userId)
   {
      await Clients.All.SendAsync("ReceiveMessage", "test", message);
   }


   public override async Task OnConnectedAsync()
   {
     var connectionId = Context.ConnectionId;
     var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
     var userName = Context.User?.FindFirstValue("user_name");
     
     Console.WriteLine("connectionId {0}", connectionId);
     Console.WriteLine("userId: {0}", userId);
     Console.WriteLine("user name: {0}", userName);
     

   }
}