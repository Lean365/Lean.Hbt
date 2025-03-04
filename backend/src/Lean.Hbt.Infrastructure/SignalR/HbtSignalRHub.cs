//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtSignalRHub.cs
// 创建者 : Lean365
// 创建时间: 2024-01-24 10:00
// 版本号 : V1.0.0
// 描述    : SignalR实时通信Hub
//===================================================================

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Lean.Hbt.Common.Enums;
using Lean.Hbt.Domain.Models.SignalR;
using Lean.Hbt.Common.Exceptions;
using Lean.Hbt.Domain.IServices.SignalR;

namespace Lean.Hbt.Infrastructure.SignalR
{
    /// <summary>
    /// SignalR实时通信Hub
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-24
    /// </remarks>
    [Authorize]
    public class HbtSignalRHub : Hub<IHbtSignalRClient>
    {
        private readonly IHbtSignalRUserService _signalRUserService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HbtSignalRHub(IHbtSignalRUserService signalRUserService)
        {
            _signalRUserService = signalRUserService;
        }

        /// <summary>
        /// 连接建立时
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
                throw new HbtException("无法获取 HTTP 上下文");

            var uidClaim = httpContext.User?.FindFirst("uid");
            if (uidClaim == null)
                throw new HbtException("用户未认证");

            var userId = long.Parse(uidClaim.Value);
            var clientIp = httpContext.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown";

            await _signalRUserService.SaveOnlineUserAsync(new HbtOnlineUser
            {
                UserId = userId,
                ConnectionId = Context.ConnectionId,
                ClientIp = clientIp,
                UserAgent = userAgent,
                LastActiveTime = DateTime.Now
            });
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 连接断开时
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _signalRUserService.DeleteOnlineUserAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 加入群组
        /// </summary>
        public async Task JoinGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException(nameof(groupName));

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).JoinedGroup(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// 离开群组
        /// </summary>
        public async Task LeaveGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException(nameof(groupName));

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).LeftGroup(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// 发送消息给指定用户
        /// </summary>
        public async Task SendToUser(long userId, string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            var connections = await _signalRUserService.GetConnectionIdsAsync(userId);
            if (connections?.Any() == true)
            {
                await Clients.Clients(connections).ReceiveMessage(message);
            }
        }

        /// <summary>
        /// 发送消息给指定群组
        /// </summary>
        public async Task SendToGroup(string groupName, string message)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException(nameof(groupName));
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            await Clients.Group(groupName).ReceiveMessage(message);
        }
    }
} 