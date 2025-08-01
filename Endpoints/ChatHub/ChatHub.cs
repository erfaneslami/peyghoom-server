using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;

namespace Peyghoom.Endpoints.ChatHub;

public class ChatHub: Hub
{
   public async Task SendMessageAsync(string message, ObjectId userId)
   {
      await Clients.All.SendAsync("ReceiveMessage", "test", message);
   }


   public override async Task OnConnectedAsync()
   {
     var connectionId = Context.ConnectionId;
     Console.WriteLine("connectionId", connectionId);

   }
}