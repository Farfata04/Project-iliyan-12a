using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebChat.Data;
using WebChat.Models;

namespace WebChat.Hubs;

public class UserOnlineStatusModel
{
	public string UserId { get; set; }
	public bool IsOnline { get; set; }

	public UserOnlineStatusModel(string id, bool status)
	{
		this.UserId = id;
		this.IsOnline = status;
	}
}

[Authorize]
public class ChatHub : Hub
{
	private readonly WebChatContext _dbContext;
	
	public ChatHub(WebChatContext context)
	{
		_dbContext = context;
	}

	public override async Task OnConnectedAsync()
	{
		var user = await GetUserAsync(Context.UserIdentifier);
		string groupName = user.Department.DepartmentName;
		
		await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
		
		string groupMessage = $"{user.FirstName} has joined the {groupName} group.";
		await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("GroupMessage", groupMessage);
		
		await Clients
			.GroupExcept(groupName, Context.ConnectionId)
			.SendAsync("SetOnlineStatus", new UserOnlineStatusModel(user.Id, true));
		
		base.OnConnectedAsync();
	}
	
	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		var user = await GetUserAsync(Context.UserIdentifier);
		string groupName = user.Department.DepartmentName;
		
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
		
		string groupMessage = $"{user.FirstName} has left the {groupName} group.";
		await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("GroupMessage", groupMessage);
		
		await Clients
			.GroupExcept(groupName, Context.ConnectionId)
			.SendAsync("SetOnlineStatus", new UserOnlineStatusModel(user.Id, false));
		
		base.OnDisconnectedAsync(exception);
	}
	
	
	public async Task SendMessage(string message)
	{
		var user = await GetUserAsync(Context.UserIdentifier);
		string groupName = await GetDepartmentName(Context.UserIdentifier);

		var model = new MessageModel
		{
			Username = user.FirstName,
			ProfilePicture = $"https://robohash.org/{user.UserName}",
			Content = message,
			TimeStamp = DateTime.Now
		};
		
		await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveMessage", model);
	}

	private async Task<User> GetUserAsync(string id)
	{	
		return await _dbContext.Users.Include(u => u.Department).FirstAsync(u => u.Id == id);
	}
	
	private async Task<string> GetDepartmentName(string id)
	{
		var user = await GetUserAsync(id);
		return user.Department.DepartmentName;
	}
}
