namespace WebChat.Models;

public class MessageModel
{
    public string Username { get; set; }
    public string ProfilePicture { get; set; } = "https://ptetutorials.com/images/user-profile.png";
    public string Content { get; set; }
    public DateTime TimeStamp { get; set; }
}