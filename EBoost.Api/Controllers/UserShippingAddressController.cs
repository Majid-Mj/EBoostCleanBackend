using AutoMapper;
using EBoost.Api.Extensions;
using EBoost.Application.DTOs.Address;
using EBoost.Application.Interfaces.Repositories;
using EBoost.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EBoost.Api.Controllers
{
    [Authorize(Policy ="UserOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserShippingAddressController : ControllerBase
    {
        private readonly IShippingAddressRepository _SaddressRepo;
        private readonly IMapper _mapper;

        public UserShippingAddressController(IShippingAddressRepository SaddressRepo , IMapper mapper)
        {
            _SaddressRepo = SaddressRepo;
            _mapper = mapper;
        }

        //Add Address
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromForm]CreateAddressDto dto)
        {
            int userId = User.GetUserId();

            var address = _mapper.Map<ShippingAddress>(dto);

            address.UserId = userId;

            if (dto.IsDefault)
            {
                var existing = await _SaddressRepo.GetByUserIdAsync(userId);

                foreach (var addr in existing)
                    addr.IsDefault = false;
            }

            await _SaddressRepo.AddAsync(address);
            await _SaddressRepo.SaveChangesAsync();

            return Ok("Address added successfully");
        }

        //get all Adresses
        [HttpGet]
        public async Task<IActionResult> GetMyAddresses()
        {
            int userId = User.GetUserId();
            var addresses = await _SaddressRepo.GetByUserIdAsync(userId);

            return Ok(_mapper.Map<List<AddressDto>>(addresses));
        }


        //Update Address
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAddress(int id, UpdateAddressDto dto)
        {
            int userId = User.GetUserId();

            var address = await _SaddressRepo.GetByIdAsync(id);

            if (address == null)
                return NotFound("Address not found");

            if (address.UserId != userId)
                return Forbid();

          
            _mapper.Map(dto, address);

            await _SaddressRepo.SaveChangesAsync();

            return Ok("Address updated successfully");
        }


        //  Delete Address
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            int userId = User.GetUserId();

            var address = await _SaddressRepo.GetByIdAsync(id);

            if (address == null)
                return NotFound("Address not found");

            if (address.UserId != userId)
                return Forbid();

            bool wasDefault = address.IsDefault;

            await _SaddressRepo.DeleteAsync(address);
            await _SaddressRepo.SaveChangesAsync();

            if (wasDefault)
            {
                var remaining = await _SaddressRepo.GetByUserIdAsync(userId);
                var first = remaining.FirstOrDefault();

                if (first != null)
                {
                    first.IsDefault = true;
                    await _SaddressRepo.SaveChangesAsync();
                }
            }

            return Ok("Address deleted successfully");
        }

        //Set As default
        [HttpPut("{id:int}/set-default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            int userId = User.GetUserId();

            var addresses = await _SaddressRepo.GetByUserIdAsync(userId);

            var selected = addresses.FirstOrDefault(a => a.Id == id);

            if (selected == null)
                return NotFound("Address not found");

            foreach (var addr in addresses)
                addr.IsDefault = false;

            selected.IsDefault = true;

            await _SaddressRepo.SaveChangesAsync();

            return Ok("Default address updated successfully");
        }




    }
}
