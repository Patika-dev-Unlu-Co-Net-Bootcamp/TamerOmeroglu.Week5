using HotelFinder.API.Exceptions;
using HotelFinder.API.Extensions;
using HotelFinder.API.Params;
using HotelFinder.DataAccess;
using HotelFinder.Entity;
using HotelFinder.Entity.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;

namespace HotelFinder.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly HotelFinderDbContext _context;
        private readonly IMemoryCache _memoryCache;
        public HotelController(HotelFinderDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        [HttpGet("GetAllFromQuery")]
        public IActionResult GetAllHotel(GetAllParams prm)
        {
            var hotels = _context.Hotels.ToListFromQueryParams(prm);

            if (hotels.Count == 0)
            {
                throw new NotFoundException("No registered hotel");
            }
            return Ok(hotels);
        }

        [HttpGet("GetAllHotel")]
        public IActionResult GetAllHotel()
        {
            _memoryCache.TryGetValue("Hotels", out Hotel hotelsCache);


            if (hotelsCache == null)
            {
                var hotels = _context.Hotels.Select(x => ItemToDTO(x)).ToList();

                if (hotels.Count == 0)
                {
                    throw new NotFoundException("No registered hotel");
                }

                _memoryCache.Set("Hotels", hotels, new MemoryCacheEntryOptions { AbsoluteExpiration = DateTime.UtcNow.AddMinutes(30), Priority = CacheItemPriority.Low });

                return Ok(hotels);
            }
            return Ok(hotelsCache);
        }

        [ResponseCache(Duration = 10000, Location = ResponseCacheLocation.Any, NoStore = false)]
        [HttpGet("{id}")]
        public IActionResult GetHotel(int id)
        {
            var hotel = _context.Hotels.FirstOrDefault(x => x.Id == id);

            if (hotel == null)
            {
                throw new NotFoundException("Hotel not found");
            }
            return Ok(ItemToDTO(hotel));
        }





        [HttpPost("create")]
        public IActionResult CreateHotel([FromBody] HotelCreateDto hotelDto)
        {
            var hotel = new Hotel()
            {
                Name = hotelDto.Name,
                City = hotelDto.City,
                PostCode = hotelDto.PostCode
            };

            _context.Hotels.Add(hotel);
            _context.SaveChanges();


            return CreatedAtAction(
                    nameof(GetHotel),
                    new { id = hotel.Id },
                    ItemToDTO(hotel));
        }


        [HttpPut("{id}")]
        public IActionResult UpdateHotel(int id, [FromBody] HotelCreateDto hotelDto)
        {
            var hotel = _context.Hotels.FirstOrDefault(x => x.Id == id);

            if (hotel == null)
            {
                throw new NotFoundException("Hotel Not Found");
            }

            hotel.Name = hotelDto.Name;
            hotel.City = hotelDto.City;
            hotel.PostCode = hotelDto.PostCode;

            _context.SaveChanges();

            return Ok();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteHotel(int id)
        {
            var hotel = _context.Hotels.FirstOrDefault(x => x.Id == id);

            if (hotel == null)
            {
                throw new NotFoundException("Hotel Not Found");
            }

            _context.Remove(hotel);

            return Ok();
        }

        private static HotelDto ItemToDTO(Hotel todoItem) =>
        new HotelDto
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            City = todoItem.City,
            PostCode = todoItem.PostCode
        };
    }
}
