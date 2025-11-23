using Imposter.Core.Domain.Entities;
using Imposter.Core.ServicesContracts;
using Imposter.UI.Extension_Methods;
using Microsoft.AspNetCore.SignalR;

namespace Imposter.UI.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameService _gameService;

        public GameHub(IGameService gameService)
        {
            _gameService = gameService;
        }
        public async override Task OnConnectedAsync()
        {
            // add the connection to the player
            var context = Context.GetHttpContext();
            var player = context?.Session.GetObject<Player>("PlayerName");
            if(player == null)
            {
                return;
            }
            var result = await _gameService.AddConnectionToPlayer(player.PlayerId.Value,Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId,player.RoomId.ToString());
            if (result == 1)
            {
                await Clients.Group(player.RoomId.Value.ToString()).SendAsync("UserJoined", player.Name);
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var context = Context.GetHttpContext();
            var player = context?.Session.GetObject<Player>("PlayerName");
            if (player == null)
            {
                return;
            }
            //wait 20s
            await Task.Delay(20000);
            // remove the connection from the player
            await _gameService.RemoveConnectionFromPlayer(player.PlayerId.Value,Context.ConnectionId);
            // if the number of the conns is 0 remove the player from the room
            int cons = await _gameService.GetConnectionsCount(player.PlayerId.Value);
            if(cons == 0)
            {
                await _gameService.RemovePlayerFromRoom(player, player.RoomId.Value);
                await _gameService.RemovePlayer(player.RoomId.Value);
            }
        }


        public async Task StartGame(Guid roomId)
        {
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", "Game started!");
        }
    }
}