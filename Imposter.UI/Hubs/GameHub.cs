using AutoMapper;
using Imposter.Core.Domain.Entities;
using Imposter.Core.Domain.Enums;
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
        public GameHub(IGameService gameService, IMapper mapper)
        {
            _gameService = gameService;
            _mapper = mapper;
        }
        /*
         * Category of the room ✔
         * the number of players in online page ✔
            discussion✔
            voting✔
            Wating
            results
            when he enter a room should reset his data
            Name of the room is the name of the host ✔
            the state of players in the lobby and in voting ✔
            IsInRoom when he join in InGame room ✔
            choices to the word game
         */
        public async override Task OnConnectedAsync()
        {
            // add the connection to the player
            var context = Context.GetHttpContext();
            var playerId = context?.Session.GetString("PlayerId");
            if (playerId == null)
            {
                return;
            }
            var player = await _gameService.GetPlayer(Guid.Parse(playerId));
            var result = await _gameService.AddConnectionToPlayer(player.PlayerId.Value, Context.ConnectionId, player.RoomId);
            await Groups.AddToGroupAsync(Context.ConnectionId, player.RoomId.ToString());

            if (await _gameService.IsPlayerInRoom(player.PlayerId,player.RoomId) == 1)
            {

                
                var playerVM = _mapper.Map<PlayerViewModel>(player);
                await Clients.OthersInGroup(player.RoomId.Value.ToString()).SendAsync("userJoined", playerVM);
            }
            await RoomState(player.RoomId.Value);

        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Task.Delay(10000);
            var connectionId = Context.ConnectionId;
            
            var connection = await _gameService.GetConnection(connectionId);
            var player = await _gameService.GetPlayer(connection.playerId);
            var room = await _gameService.GetRoom(connection.roomId);
            await _gameService.RemoveConnection(connectionId);
            if (await _gameService.IsPlayerInRoom(player.PlayerId,room.RoomId) == 0)
            {
                var playerVM = _mapper.Map<PlayerViewModel>(player);
                //playerVM.
                await Clients.Group(room.RoomId.Value.ToString()).SendAsync("userLeft", playerVM);
                await _gameService.RemovePlayerFromRoom(player.PlayerId, room.RoomId);
            }
            await RoomState(player.RoomId.Value);

        }



        public async Task StartGame(Guid roomId)
        {
            await _gameService.ResetStateOfAllPlayersInRoom(roomId);
            await _gameService.StartGame(roomId);
            
            await NextStage(roomId);
        }
        public async Task Waiting(Guid roomId, Guid PlayerId, Guid chosenPlayer)
        {
            await ready(roomId, PlayerId);
            await Clients.Caller.SendAsync("Waiting");
            var room = await _gameService.GetRoom(roomId);
            if (chosenPlayer != Guid.Empty && room.ImposterId == chosenPlayer)
            {
                await _gameService.AddScore(PlayerId);
            }
        }
        public async Task ChangeCategory(Guid roomId, string category)
        {
            CategoryOptions cat = (CategoryOptions)Enum.Parse(typeof(CategoryOptions), category);
            await _gameService.SetCategoryForRoom(roomId, cat);
            await Clients.Group(roomId.ToString()).SendAsync("CategoryChanged", category);
        }
        public async Task NextStage(Guid roomId)
        {
            List<string> pages = new List<string>
            {
                "Lobby",
                "SecretWord",
                "Discussion",
                "Voting",
                "Choosing",
                "Results"
            };
            
            var stage = await _gameService.NextStage(roomId);
            if(stage == 0)
            {
                await _gameService.StopGame(roomId);
                await _gameService.ResetStateOfAllPlayersInRoom(roomId);
                var room = await _gameService.GetRoom(roomId);
                await _gameService.MakePlayerReady(room.HostId);
            }
            await Clients.Group(roomId.ToString()).SendAsync("NextStage", pages[stage]);
        }
        public async Task ready(Guid roomId, Guid PlayerId)
        {
            await _gameService.MakePlayerReady(PlayerId);
            var player = await _gameService.GetPlayer(PlayerId);
            var playerVm = _mapper.Map<PlayerViewModel>(player);
            await Clients.Group(roomId.ToString()).SendAsync("PlayerReady",playerVm);
            await RoomState(roomId);
        }
        public async Task RoomState(Guid roomId)
        {
            if(await _gameService.IsAllPlayersReady(roomId))
            {
                await Clients.Group(roomId.ToString()).SendAsync("AllPlayersReady");
            }else
            {
                await Clients.Group(roomId.ToString()).SendAsync("NotAllPlayersReady");
            }
        }
        /*
         * connection.invoke("SendImpostorResult", {
                    roomId: '@Model.RoomId',
                    playerId: '@Model.PlayerId',
                    chosenWord: chosenWord,
                    isCorrect: correct
                }).catch(err => console.error(err.toString()));
         */
        public async Task SendImpostorResult(string playerId,bool isCorrect)
        {
            var player = await _gameService.GetPlayer(Guid.Parse(playerId));
            await _gameService.MakePlayerReady(Guid.Parse(playerId));
            if (player.State) return;
            if (isCorrect)
            {
                await _gameService.AddScore(Guid.Parse(playerId));
            }
        }
    }
}