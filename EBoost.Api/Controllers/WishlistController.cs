using AutoMapper;
using EBoost.Api.Extensions;
using EBoost.Application.Common.Responses;
using EBoost.Application.DTOs.Wishlist.cs;
using EBoost.Application.Interfaces.Repositories;
using EBoost.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace EBoost.Api.Controllers
{
    [Authorize(Policy = "UserOnly")]
    [Route("api/wishlist")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistRepository _repo;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepo;

        public WishlistController(IWishlistRepository repo, IMapper mapper ,IProductRepository productRepo)
        {
            _repo = repo;
            _mapper = mapper;
            _productRepo = productRepo;
        }

        //WishList
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            int userId = User.GetUserId();

            var wishlist = await _repo.GetByUserIdAsync(userId);

            if (wishlist == null)
                return Ok(ApiResponse<WishlistDto>.Ok(
                    new WishlistDto(0, new List<WishlistItemDto>())
                ));
    
            var items = wishlist.Items
                .Where(i => i.Product.IsActive)
                .Select(i =>  _mapper.Map<WishlistItemDto>(i))
                .ToList();

            return Ok(ApiResponse<WishlistDto>.Ok(
                new WishlistDto(wishlist.Id, items)
            ));

        
        }

        //AddtoWishList
        [HttpPost("{productId:int}")]
        public async Task<IActionResult> Add(int productId)
        {
            int userId = User.GetUserId();

            var product = await _productRepo.GetByIdAsync(productId);

            if (product == null || !product.IsActive)
                return BadRequest("Product not found");

            var wishlist =await  _repo.GetByUserIdAsync(userId);

            if(wishlist == null)
            {
                wishlist = await _repo.CreateAsync(userId);
            }

            if (await _repo.ItemExistsAsync(wishlist.Id, productId))
                return BadRequest(ApiResponse<string>.Fail("Product Already in Wishlist"));

            await _repo.AddItemAsync(wishlist.Id , productId);

            return Ok(ApiResponse<string>.Ok("Added to Wishlist"));
        }

        //DeletefromWishlist
        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> Remove(int productId)
        {
            int userId = User.GetUserId();

            var wishlist = await _repo.GetByUserIdAsync(userId);

            if (wishlist == null)
                return BadRequest("Wishlist not found");

            var removed = await _repo.RemoveItemAsync(wishlist.Id, productId);

            if (!removed)
                return NotFound("Product not found in wishlist");

            return Ok("Removed from Wishlist");
        }
    }
}
