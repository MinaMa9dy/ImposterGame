using AutoMapper;
using Imposter.Core.Domain.Entities;
using Imposter.Core.Domain.Enums;
using Imposter.Core.ServicesContracts;
using Imposter.Core.ViewModels;
using Imposter.UI.Extension_Methods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NuGet.Packaging.Signing;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Imposter.UI.Controllers
{
    /* Some Problems can occur
     *     1- If two players with the same name try to enter the room, it might cause confusion.
     *     2- There is no validation for the player name input, which could lead to empty or invalid names being used.
     *     3- he might enter in an old stage of the game if he left and rejoined (Or the connection lost seconds)
     *     4- if he make a refresh you should make him going to Index with Method Get ✔
     *     5- update all props for the room and the user 
     *     6- متنساش ترجع كل حاجه مع الconnections
     */
    [Route("room/{roomId:Guid}")]
    public class RoomController : Controller
    {
        private readonly IGameService _gameService;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;

        public RoomController(ILogger<HomeController> logger, IGameService gameService, IMapper mapper)
        {
            _logger = logger;
            _gameService = gameService;
            _mapper = mapper;
        }
        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index(Guid roomId)
        {
            
            string? PlayerId = HttpContext.Session.GetString("PlayerId");
            
            Player? player = null;
            if (PlayerId is not null)
            {
                player = await _gameService.GetPlayer(Guid.Parse(PlayerId));
            }
            var room = await _gameService.GetRoom(roomId);
            if (room == null)
            {
                return NotFound("Room not found.");
            }
            if (player == null)
            {
                return RedirectToAction(nameof(EnterName),new { roomId = roomId});
                
            }
            if(player.RoomId != room.RoomId)
            {
                await _gameService.RemovePlayerFromRoom(player.PlayerId,roomId);
                await _gameService.AddPlayerToRoom(player.PlayerId, roomId);
                
            }
            if (room.InGame)
            {
                if (await _gameService.IsPlayerInRoom(player.PlayerId, roomId) == 0)
                {
                    return Content("Room In Game");
                }
                else
                {
                    TempData["FromIndex"] = true;
                    return RedirectToAction(nameof(Game), new { roomId = roomId });
                }

            }
            
            if (room.HostId == null)
            {
                
                await _gameService.MakePlayerHost(player.PlayerId,roomId);
            }
            TempData["FromIndex"] = true;
            return RedirectToAction(nameof(Lobby), new { roomId = roomId });

        }
        #region enter-name
        [HttpGet("enter-name")]
        public IActionResult EnterName()
        {
            return View();
        }

        [HttpPost("enter-name")]
        public async Task<IActionResult> EnterName(string Name, Guid roomId)
        {
            var PlayerId = HttpContext.Session.GetString("PlayerId");
            var existingPlayerInDb = PlayerId is not null ? await _gameService.GetPlayer(Guid.Parse(PlayerId)) : null;
            Player? SessionPlayer = new Player();
            if (existingPlayerInDb is null)
            {
                var player = await _gameService.CreatePlayer(Name);
                await _gameService.AddPlayer(player);
                await _gameService.AddPlayerToRoom(player.PlayerId, roomId);
                HttpContext.Session.SetString("PlayerId",player.PlayerId.ToString());
            }else { 
                await _gameService.UpdateNamePlayer(existingPlayerInDb.PlayerId, Name);
                var x = await _gameService.AddPlayerToRoom(existingPlayerInDb.PlayerId, roomId);
            }
            return RedirectToAction("Index", new { roomId = roomId });
        }
        #endregion
        [HttpPost("Lobby")]
        public IActionResult LobbyNextPage(Guid roomId)
        {
            TempData["FromIndex"] = true;
            return RedirectToAction(nameof(Lobby), new { roomId = roomId });
        }
        [HttpGet("Lobby")]
        public async Task<IActionResult> Lobby(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if(FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var room = await _gameService.GetRoom(roomId);
            var playerId = HttpContext.Session.GetString("PlayerId");
            if (playerId is null || room is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var player = await _gameService.GetPlayer(Guid.Parse(playerId));
            if (room.HostId != player.PlayerId)
            {
                await _gameService.MakePlayerNotReady(player.PlayerId);
            }
            var players = _mapper.Map<List<PlayerViewModel>>(room.Players.ToList());
            ViewBag.isHost = room.HostId == player.PlayerId;
            ViewBag.roomId = roomId;
            ViewBag.PlayerId = player.PlayerId;
            ViewBag.selected = room.Category;          // already set by host
            ViewBag.categories = Enum.GetValues<CategoryOptions>();
            return View(players);
        }
        
        [HttpGet("Game")]
        public async Task<IActionResult> Game(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var room = await _gameService.GetRoom(roomId);
            if (room is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            int stage = room.Stage;
            switch (stage)
            {
                case 0:
                    return RedirectToAction(nameof(Index), new { roomId = roomId });
                case 1:
                    TempData["FromIndex"] = true;
                    return RedirectToAction(nameof(SecretWord), new { roomId = roomId });
                case 2:
                    TempData["FromIndex"] = true;
                    return RedirectToAction(nameof(Discussion), new { roomId = roomId });
                case 3:
                    TempData["FromIndex"] = true;
                    return RedirectToAction(nameof(Voting), new { roomId = roomId });
                case 4:
                    TempData["FromIndex"] = true;
                    return RedirectToAction(nameof(Choosing), new { roomId = roomId });
                case 5:
                    TempData["FromIndex"] = true;
                    return RedirectToAction(nameof(Results), new { roomId = roomId });  `   `
                default:
                    return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
        }
        [HttpPost("SecretWord")]
        public IActionResult SecretWordNextPage(Guid roomId)
        {
            
            TempData["FromIndex"] = true;
            return RedirectToAction(nameof(SecretWord), new { roomId = roomId });
        }
        [HttpGet("SecretWord")]
        public async Task<IActionResult> SecretWord(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var playerId = HttpContext.Session.GetString("PlayerId");
            var room = await _gameService.GetRoom(roomId);
            if (playerId is null || room is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var player = await _gameService.GetPlayer(Guid.Parse(playerId));
            ViewBag.isHost = room.HostId == player.PlayerId;
            ViewBag.roomId = roomId;
            ViewBag.IsImposter = room.ImposterId == player.PlayerId;
            return View(room.SecretWord);
        }
        [HttpPost("Discussion")]
        public IActionResult DiscussionNextPage(Guid roomId)
        {

            TempData["FromIndex"] = true;
            return RedirectToAction(nameof(Discussion), new { roomId = roomId });
        }
        [HttpGet("Discussion")]
        public async Task<IActionResult> Discussion(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var playerId = HttpContext.Session.GetString("PlayerId");
            var room = await _gameService.GetRoom(roomId);
            if (playerId is null || room is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var player = await _gameService.GetPlayer(Guid.Parse(playerId));
            ViewBag.isHost = room.HostId == player.PlayerId;
            ViewBag.roomId = roomId;
            return View();
        }
        [HttpPost("Voting")]
        public IActionResult VotingNextPage(Guid roomId)
        {
            TempData["FromIndex"] = true;
            return RedirectToAction(nameof(Voting), new { roomId = roomId });
        }
        [HttpGet("Voting")]
        public async Task<IActionResult> Voting(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var playerId = HttpContext.Session.GetString("PlayerId");
            var room = await _gameService.GetRoom(roomId);
            if (playerId is null || room is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var player = await _gameService.GetPlayer(Guid.Parse(playerId));
            if (player.State)
            {
                TempData["FromIndex"] = true;
                return RedirectToAction(nameof(Waiting), new { roomId = roomId });
            }
            var players = room.Players.Where(p => p.PlayerId != player.PlayerId).ToList();
            var playersVM = _mapper.Map<List<PlayerViewModel>>(players);
            ViewBag.roomId = roomId;
            ViewBag.myId = player.PlayerId;
            return View(playersVM);
        }
        [HttpPost("Waiting")]
        public IActionResult WaitingNextPage(Guid roomId)
        {
            TempData["FromIndex"] = true;
            return RedirectToAction(nameof(Waiting), new { roomId = roomId });
        }
        [HttpGet("Waiting")]
        public async Task<IActionResult> Waiting(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var playerId = HttpContext.Session.GetString("PlayerId");
            var room = await _gameService.GetRoom(roomId);
            if (playerId is null || room is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var player = await _gameService.GetPlayer(Guid.Parse(playerId));
            var players = room.Players.ToList();
            var playersVM = _mapper.Map<List<PlayerViewModel>>(players);
            ViewBag.IsHost = player.PlayerId == room.HostId;
            ViewBag.roomId = roomId;
            ViewBag.PlayerId = player.PlayerId;
            return View(playersVM);
        }
        [HttpPost("Choosing")]
        public IActionResult ChoosingNextPage(Guid roomId)
        {
            TempData["FromIndex"] = true;
            return RedirectToAction(nameof(Choosing), new { roomId = roomId });
        }
        [HttpGet("Choosing")]
        public async Task<IActionResult> Choosing(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var playerId = HttpContext.Session.GetString("PlayerId");
            var room = await _gameService.GetRoom(roomId);
            if (playerId is null || room is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var player = await _gameService.GetPlayer(Guid.Parse(playerId));
            if(player.PlayerId != room.ImposterId)
            {
                TempData["FromIndex"] = true;
                return RedirectToAction(nameof(TheImposterIs), new { roomId = roomId });
                
            }
            await _gameService.MakePlayerNotReady(player.PlayerId);
            return View((room.SecretWord.Choices,room.SecretWord.Text,player.PlayerId.ToString(),room.RoomId.ToString(),player.State));
            
        }
        [HttpGet("TheImposterIs")]
        public async Task<IActionResult> TheImposterIs(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var room = await _gameService.GetRoom(roomId);
            if (room is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var imposter = await _gameService.GetPlayer(room.ImposterId.Value);
            ViewBag.Name = imposter.Name;
            ViewBag.id = room.RoomId;
            return View(model:room.RoomId.ToString());
        }

        [HttpPost("Results")]
        public IActionResult ResultsNextPage(Guid roomId)
        {
            TempData["FromIndex"] = true;
            return RedirectToAction(nameof(Results), new { roomId = roomId });
        }
        [HttpGet("Results")]
        public async Task<IActionResult> Results(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var playerId = HttpContext.Session.GetString("PlayerId");
            var room = await _gameService.GetRoom(roomId);
            if (playerId is null || room is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var player = await _gameService.GetPlayer(Guid.Parse(playerId));
            var players = room.Players.ToList();
            var playersVM = _mapper.Map<List<PlayerViewModel>>(players);
            ViewBag.IsHost = player.PlayerId == room.HostId;
            ViewBag.roomId = roomId;
            return View(playersVM);
        }



        [HttpGet("Remove")]
        public async Task<IActionResult> RemoveRoom(Guid roomId)
        {
            var result = await _gameService.RemoveRoom(roomId);
            if (result)
            {
                return Ok("Room removed successfully.");
            }
            else
            {
                return BadRequest("Could not remove room.");
            }
        }

    }
}
