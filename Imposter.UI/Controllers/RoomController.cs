using AutoMapper;
using Imposter.Core.Domain.Entities;
using Imposter.Core.ServicesContracts;
using Imposter.Core.ViewModels;
using Imposter.UI.Extension_Methods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
            
            var theplayer = HttpContext.Session.GetObject<Player>("PlayerName");
            Player? player = null;
            if (theplayer is not null)
            {
                player = await _gameService.GetPlayer(theplayer.PlayerId.Value);
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
                await _gameService.AddPlayerToRoom(player.PlayerId, roomId);
                player.Room = null;
                player.Connections = new List<Connection>();
                HttpContext.Session.SetObject("PlayerName", player);
            }
            if (room.InGame)
            {
                if (!room.Players.Any(p => p.Name == player.Name))
                {
                    return View("NotFound");
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
                player.Room = null;
                player.Connections = new List<Connection>();
                HttpContext.Session.SetObject("PlayerName", player);
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
            var existingPlayer = HttpContext.Session.GetObject<Player>("PlayerName");
            var existingPlayerInDb = existingPlayer is not null ? await _gameService.GetPlayer(existingPlayer.PlayerId.Value) : null;
            Player? SessionPlayer = new Player();
            if (existingPlayerInDb is null)
            {
                var player = await _gameService.CreatePlayer(Name);
                await _gameService.AddPlayer(player);
                await _gameService.AddPlayerToRoom(player.PlayerId, roomId);
                SessionPlayer = player;
            }else { 
                await _gameService.UpdateNamePlayer(existingPlayerInDb.PlayerId, Name);
                var x = await _gameService.AddPlayerToRoom(existingPlayerInDb.PlayerId, roomId);
                SessionPlayer = await _gameService.GetPlayer(existingPlayerInDb.PlayerId);

            }

            SessionPlayer.Room = null;
            SessionPlayer.Connections = new List<Connection>();
            HttpContext.Session.SetObject("PlayerName", SessionPlayer);
            return RedirectToAction("Index", new { roomId = roomId });
        }
        #endregion
        [HttpGet("Lobby")]
        public async Task<IActionResult> Lobby(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if(FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var room = await _gameService.GetRoom(roomId);
            var players = _mapper.Map<List<PlayerViewModel>>(room.Players.ToList());
            var player = HttpContext.Session.GetObject<Player>("PlayerName");
            ViewBag.isHost = room.HostId == player.PlayerId;
            ViewBag.roomId = roomId;
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
            int stage = room.Stage;
            switch (stage)
            {
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
                    return RedirectToAction(nameof(Scores), new { roomId = roomId });
                default:
                    return RedirectToAction(nameof(Lobby), new { roomId = roomId });
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
            var player = HttpContext.Session.GetObject<Player>("PlayerName");
            var room = await _gameService.GetRoom(roomId);
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
            var player = HttpContext.Session.GetObject<Player>("PlayerName");
            var room = await _gameService.GetRoom(roomId);
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
            var player = HttpContext.Session.GetObject<Player>("PlayerName");
            var room = await _gameService.GetRoom(roomId);
            var players = room.Players.Where(p => p.PlayerId != player.PlayerId).ToList();
            var playersVM = _mapper.Map<List<PlayerViewModel>>(players);
            return View(playersVM);
        }

        [HttpGet("Choosing")]
        public async Task<IActionResult> Choosing(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var room = await _gameService.GetRoom(roomId);
            var player = HttpContext.Session.GetObject<Player>("PlayerName");
            bool IsImposter = room.HostId == player.PlayerId;
            return View(model: IsImposter);
        }
        [HttpGet("Scores")]
        public async Task<IActionResult> Scores(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var room = await _gameService.GetRoom(roomId);
            return View(room);
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
