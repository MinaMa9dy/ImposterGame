using AutoMapper;
using Imposter.Core.Domain.Entities;
using Imposter.Core.ServicesContracts;
using Imposter.Core.ViewModels;
using Imposter.UI.Extension_Methods;
using Microsoft.AspNetCore.SignalR;

namespace Imposter.UI.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameService _gameService;
        private readonly IMapper _mapper;
        public GameHub(IGameService gameService,IMapper mapper)
        {
            _gameService = gameService;
            _mapper = mapper;
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
                var playerVM = _mapper.Map<PlayerViewModel>(player);
                await Clients.Group(player.RoomId.Value.ToString()).SendAsync("userJoined", playerVM);
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
            await Task.Delay(10000);
            // remove the connection from the player
            await _gameService.RemoveConnectionFromPlayer(player.PlayerId.Value,Context.ConnectionId);
            // if the number of the conns is 0 remove the player from the room
            int cons = await _gameService.GetConnectionsCount(player.PlayerId.Value);
            if(cons == 0)
            {
                await Clients.Group(player.RoomId.Value.ToString()).SendAsync("userLeft", player.PlayerId);
                await _gameService.RemovePlayerFromRoom(player.PlayerId, player.RoomId);
            }
        }
        


        public async Task StartGame(Guid roomId)
        {
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", "Game started!");
        }
    }
}