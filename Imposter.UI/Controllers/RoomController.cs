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
            }
            if (room.InGame)
            {
                if (!room.Players.Any(p => p.Name == player.Name))
                {
                    return View("NotFound");
                }
                else
                {
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
            var CurrentPlayer = HttpContext.Session.GetObject<Player>("PlayerName");
            var player = new Player();
            if (CurrentPlayer is null)
            {
                player.Name = Name;
                player.PlayerId = Guid.NewGuid();
                await _gameService.AddPlayer(player);
                await _gameService.AddPlayerToRoom(player.PlayerId, roomId);
            }
            else
            {
                player = CurrentPlayer;
                player.Name = Name;
                await _gameService.UpdatePlayer(player);
            }
            Player player1 = new Player();
            player1 = player;
            player1.Room = null;
            HttpContext.Session.SetObject("PlayerName", player);
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
        [HttpGet("SecretWord")]
        public async Task<IActionResult> SecretWord(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var room = await _gameService.GetRoom(roomId);
            return View(model:room.SecretWord);
        }
        [HttpGet("Discussion")]
        public async Task<IActionResult> Discussion(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            return View();
        }
        [HttpGet("Voting")]
        public IActionResult Voting(Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var player = HttpContext.Session.GetObject<Player>("PlayerName");
            return View(model:player);
        }
        [HttpPost("Voting")]
        public async Task<IActionResult> Voting(Player player,Guid roomId)
        {
            string? FromIndexString = TempData["FromIndex"]?.ToString();
            if (FromIndexString is null)
            {
                return RedirectToAction(nameof(Index), new { roomId = roomId });
            }
            var res = await _gameService.UpdatePlayer(player);
            if(res == 0)
            {
                return BadRequest("Could not update player.");
            }
            return View(model: player);
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



    }
}
