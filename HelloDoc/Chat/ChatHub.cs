using static ServiceStack.Diagnostics.Events;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HelloDoc.Chat
{
	public class ChatHub : Hub
	{
		public async Task SendMessage(string user, string message)
		{
			// Broadcast message to all clients
			await Clients.All.SendAsync("ReceiveMessage", user, message);
		}
	}
}
