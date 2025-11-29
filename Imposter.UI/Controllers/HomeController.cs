using AutoMapper;
using Imposter.Core.Domain.Entities;
using Imposter.Core.ServicesContracts;
using Imposter.Core.ViewModels;
using Imposter.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Imposter.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGameService _gameService;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger,IGameService gameService,IMapper mapper)
        {
            _logger = logger;
            _gameService = gameService;
            _mapper = mapper;
        }
        [HttpGet("")]
        [HttpGet("/Index")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var room = await _gameService.CreateRoom();
            return RedirectToAction("Index", "Room", new { roomId = room.RoomId });
        }
        [HttpGet("Online")]
        public async Task<IActionResult> Online()
        {
            
            var rooms = await _gameService.GetRooms();
            var roomsVM = _mapper.Map<List<RoomViewModel>>(rooms);

            return View(roomsVM);
        }
        [HttpGet]
        public async Task<IActionResult> RemoveAllRooms()
        {
            await _gameService.RemoveAllRooms();
            return Ok("All rooms removed.");
        }



    }
}
