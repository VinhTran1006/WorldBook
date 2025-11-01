using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WorldBook.Hubs
{
    public class ChatHub : Hub
    {
        // Lưu connection của từng user theo userName
        private static Dictionary<string, string> UserConnections = new();

        // Lưu connection của từng admin
        private static Dictionary<string, string> AdminConnections = new();

        // Lưu trạng thái: user nào đang chat với admin
        private static HashSet<string> UsersInAdminChat = new();

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var role = httpContext?.User?.IsInRole("Admin") == true ? "Admins" : "Users";
            var userName = httpContext?.User?.Identity?.Name ?? "Khách";

            await Groups.AddToGroupAsync(Context.ConnectionId, role);

            if (role == "Users")
            {
                UserConnections[userName] = Context.ConnectionId;
                System.Diagnostics.Debug.WriteLine($"✅ User '{userName}' connected: {Context.ConnectionId}");
            }
            else if (role == "Admins")
            {
                AdminConnections[userName] = Context.ConnectionId;
                System.Diagnostics.Debug.WriteLine($"✅ Admin '{userName}' connected: {Context.ConnectionId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var userName = httpContext?.User?.Identity?.Name ?? "Khách";

            UserConnections.Remove(userName);
            AdminConnections.Remove(userName);
            UsersInAdminChat.Remove(userName); // Xóa khỏi chat admin khi disconnect

            System.Diagnostics.Debug.WriteLine($" '{userName}' disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        // Khi user bấm "Liên hệ admin"
        public async Task StartAdminChat(string userName)
        {
            UsersInAdminChat.Add(userName);
            System.Diagnostics.Debug.WriteLine($"User '{userName}' started admin chat");

            // Thông báo cho admin rằng user này đang trong cuộc chat với admin
            await Clients.Group("Admins").SendAsync("UserStartedAdminChat", userName);
        }

        // Kiểm tra user có đang chat với admin không
        public bool IsUserInAdminChat(string userName)
        {
            return UsersInAdminChat.Contains(userName);
        }

        // User gửi tin nhắn cho admin (khi đã liên hệ admin)
        public async Task SendMessageToAdmin(string userName, string message)
        {
            System.Diagnostics.Debug.WriteLine($"User '{userName}' sending to admin: {message}");

            // Gửi tin nhắn đến tất cả admin
            await Clients.Group("Admins").SendAsync("ReceiveUserMessage", userName, message);
        }

        // Admin gửi tin nhắn lại user
        public async Task SendMessageToUser(string userName, string message)
        {
            System.Diagnostics.Debug.WriteLine($"Sending to user '{userName}': {message}");

            if (UserConnections.TryGetValue(userName, out var connId))
            {
                System.Diagnostics.Debug.WriteLine($"Found user connection: {connId}");
                await Clients.Client(connId).SendAsync("ReceiveAdminMessage", message);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"User '{userName}' not found in connections");
            }
        }

        // Gửi thông báo từ user đến admin (lần đầu liên hệ)
        public async Task NotifyAdmin(string userName, string message)
        {
            System.Diagnostics.Debug.WriteLine($"User '{userName}' notifying admin: {message}");
            await Clients.Group("Admins").SendAsync("ReceiveUserNotification", userName, message);
        }

        // Admin gửi tin nhắn đến tất cả user (broadcast)
        public async Task SendMessageToAll(string message)
        {
            await Clients.Group("Users").SendAsync("ReceiveAdminMessage", message);
        }


    }
}
