using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheHotelApp.Models;
using TheHotelApp.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace TheHotelApp.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IGenericHotelService<Booking> _hotelService;
        private readonly IGenericHotelService<Room> _hotelServiceRoom;
        private readonly IGenericHotelService<RoomType> _hotelServiceRoomType;
        private readonly IGenericHotelService<ApplicationUser> _activeUser;
        private readonly UserManager<ApplicationUser> _userManager;
        private Room _room;

        public BookingsController(IGenericHotelService<Booking> genericHotelService, 
                                  IGenericHotelService<Room> genericHotelServiceRoom, 
                                  IGenericHotelService<ApplicationUser> activeUser,
                                  IGenericHotelService<RoomType> genericHotelServiceRoomType,
                                  UserManager<ApplicationUser> userManager)
        {
            _hotelService = genericHotelService;
            _hotelServiceRoom = genericHotelServiceRoom;
            _hotelServiceRoomType = genericHotelServiceRoomType;
            _activeUser = activeUser;
            _userManager = userManager;
            _room = new Room();
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName

             ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userEmail = applicationUser?.Email; // will give the user's Email

            ViewData["userId"] = userId.ToString();

            return View(await _hotelService.GetAllItemsAsync());
        }

         // GET: Images/Create
        public async Task<IActionResult> Create(string roomId,string userId, string roomTypeId)
        {
            _room.ID = roomId;
            
            _room.RoomTypeID = roomTypeId;

            var roomDetails = await _hotelServiceRoom.GetItemByIdAsync(roomId);

            var roomType = new RoomType();

            roomType = await _hotelServiceRoomType.GetItemByIdAsync(roomTypeId);

            roomDetails.RoomType = roomType;

            ViewBag.Room = roomDetails;

            var rawUser = _activeUser.GetItemByIdAsync(userId);
            var user = rawUser.Result;

            var booking = new Booking()
            {
                CustomerName = user.FullName,
                CustomerAddress = user.Address,
                CustomerCity = user.City,
                CustomerEmail = user.Email,
                CustomerPhone = user.PhoneNumber,
                RoomID = roomId
            };

            //booking.ID = Guid.NewGuid().ToString();

            //await  _hotelService.Get(booking);

            return View(booking);
        }

        // POST: Images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

    }
}