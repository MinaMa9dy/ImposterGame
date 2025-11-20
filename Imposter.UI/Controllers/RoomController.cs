using AutoMapper;
using Imposter.Core.Domain.Entities;
using Imposter.Core.ServicesContracts;
using Imposter.UI.Extension_Methods;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Imposter.UI.Controllers
{
    [Route("room/{roomId:guid}")]
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
        public async Task<IActionResult> Index()
        {
            var roomId = RouteData.Values["roomId"]?.ToString();
            var player = HttpContext.Session.GetObject<Player>("PlayerName");
            if (roomId == null)
            {
                return BadRequest("Room ID is required.");
            }
            var room = await _gameService.GetRoom(Guid.Parse(roomId));
            if (room == null)
            {
                return NotFound("Room not found.");
            }
            if (player == null)
            {
                return RedirectToAction("EnterName", "Room");
            }
            if (room.InGame)
            {
                if (!room.Players.Any(p => p.Name == player.Name))
                {
                    return View("NotFound");
                }
                
            }
            if (room.Players.Count == 0)
            {
                room.Host = player;
                room.HostId = player.PlayerId;
            }
            bool result = await _gameService.AddPlayerToRoom(player, Guid.Parse(roomId));
            if(result == false)
            {
                return BadRequest("Could not add player to room.");
            }
            return RedirectToAction(nameof(Lobby));

        }
        [HttpGet("enter-name")]
        public IActionResult EnterName()
        {
            return View();
        }
        [HttpPost("enter-name")]
        public IActionResult EnterName(string Name)
        {

            HttpContext.Session.SetObject("PlayerName", new Player { Name = Name, PlayerId = Guid.NewGuid() });
            return RedirectToAction("Index", new { roomId = RouteData.Values["roomId"] });
        }
        [HttpPost("lobby")]
        public async Task<IActionResult> Lobby()
        {
            var roomId = RouteData.Values["roomId"]?.ToString();
            var room = await _gameService.GetRoom(Guid.Parse(roomId));
            //TODO Get the Stage of the room
            //TODO GameLobby
            return View();
        }
    }
}
