using AutoMapper;
using EBoost.Application.DTOs.Products;
using EBoost.Application.Interfaces.Repositories;
using EBoost.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EBoost.Api.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepo;

        public ProductsController(IProductRepository repo, IMapper mapper, ICategoryRepository categoryRepo)
        {
            _repo = repo;
            _mapper = mapper;
            _categoryRepo = categoryRepo;
        }

        //AddProduct
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> Create(
         [FromForm] CreateProductDto dto,
         [FromServices] ICloudinaryService cloudinary)
        {
            dto.Name = dto.Name.Trim();

            var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
            if (category == null || !category.IsActive)
                return BadRequest("Invalid category");

            if (await _repo.ExistsAsync(dto.Name, dto.CategoryId))
                return BadRequest("Product with same name already exists in this category");

            if (dto.ImageFiles == null || !dto.ImageFiles.Any())
                return BadRequest("At least one image is required");

            if (dto.ImageFiles != null && dto.ImageFiles.Any())
            {
                if (dto.ImageFiles.Count > 5)
                    return BadRequest("Maximum 5 images allowed");

                var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };

                foreach (var file in dto.ImageFiles)
                {
                    if (file.Length == 0)
                        return BadRequest("Image file is empty");

                    if (file.Length > 2 * 1024 * 1024) 
                        return BadRequest("Image must be under 2MB");

                    if (!allowedTypes.Contains(file.ContentType))
                        return BadRequest("Only JPG, PNG, WEBP images allowed");
                }
            }

            var product = _mapper.Map<Product>(dto);
            product.IsActive = true;

            await _repo.AddAsync(product);
            await _repo.SaveChangesAsync();

            if (dto.ImageFiles != null && dto.ImageFiles.Any())
            {
                bool isFirst = true;

                foreach (var file in dto.ImageFiles)
                {
                    var imageUrl = await cloudinary.UploadAsync(file);

                    product.Images.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = imageUrl,
                        IsPrimary = isFirst
                    });

                    isFirst = false;
                }

                await _repo.SaveChangesAsync();
            }

            return Ok("Product created successfully");
        }



        //GetProducts-User
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Products = await _repo.GetAllAsync();
            return Ok(_mapper.Map<List<ProductDto>>(Products));
            //return Ok(Products.Select(p => new ProductDto(
            //        p.Id,
            //        p.Name,
            //        p.Price,
            //        p.Stock,
            //        p.CategoryId,
            //        p.Category.Name,
            //        p.Images.Select(i => i.ImageUrl).ToList()
            //    )));

        }

        //GetProducts-Admin
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("Admin")]
        public async Task<IActionResult> GetAllForAdmin()
        {
            var products = await _repo.GetAllAsync(includeInactive: true);
            return Ok(_mapper.Map<List<ProductDto>>(products));
            //    return Ok(products.Select(p => new ProductDto(
            //    p.Id,
            //    p.Name,
            //    p.Price,
            //    p.Stock,
            //    p.CategoryId,
            //    p.Category.Name,
            //    p.Images.Select(i => i.ImageUrl).ToList()
            //)));

        }

        //GetById
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var Product = await _repo.GetByIdAsync(id);
            if (Product == null) return NotFound("Product Not Found");

            return Ok(_mapper.Map<ProductDto>(Product));
        }

        //Updateproduct
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id,[FromForm]UpdateProductDto dto)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null)
                return NotFound("Product not found");

            dto.Name = dto.Name.Trim();

            var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
            if (category == null || !category.IsActive)
                return BadRequest("Invalid category");

            if (await _repo.ExistsAsync(dto.Name, dto.CategoryId, id))
                return BadRequest("Product with same name already exists in this category");

            if (!product.IsActive)
                return BadRequest("Cannot update inactive product");


            _mapper.Map(dto, product);

            await _repo.SaveChangesAsync();

            return Ok("Product updated successfully");
        }

        //toggle
        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{id:int}/toggle")]
        public async Task<IActionResult> Toggle(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null)
                return NotFound("Product not found");

            product.IsActive = !product.IsActive;

            await _repo.SaveChangesAsync();

            return Ok($"Product is now {(product.IsActive ? "Active" : "Inactive")}");
        }

        //StockUpdate
        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{id:int}/stock")]
        public async Task<IActionResult> UpdateStock(int id,[FromForm] UpdateProductStockDto dto)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null)
                return NotFound("Product not found");

            if (!product.IsActive)
                return BadRequest("Cannot update stock of inactive product");

            if (product.Stock == dto.Stock)
                return BadRequest("Stock is already the same value");


            product.Stock = dto.Stock;

            await _repo.SaveChangesAsync();

            return Ok("Stock updated");
        }

        //UploadImage 
        [Authorize(Policy = "AdminOnly")]
        [HttpPost("{id:int}/images")]
        public async Task<IActionResult> AddImage(
        int id,
        IFormFile file,
        [FromServices] ICloudinaryService cloudinary)
        {
            var product = await _repo.GetByIdAsync(id);

            if (product == null)
                return NotFound("Product not found");

            var imageUrl = await cloudinary.UploadAsync(file);

            var productImage = new ProductImage
            {
                ProductId = id,
                ImageUrl = imageUrl,
                IsPrimary = false
            };

            product.Images.Add(productImage);

            await _repo.SaveChangesAsync();

            return Ok(imageUrl);
        }


        //Deleteimage
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("images/{imageId:int}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var image = await _repo.GetImageByIdAsync(imageId);

            if (image == null)
                return NotFound("Image not found");

            await _repo.DeleteImageAsync(image);
            await _repo.SaveChangesAsync();

            return Ok("Image deleted");
        }



        //SearchProduct
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            var products = await _repo.SearchAsync(q);
            return Ok(_mapper.Map<List<ProductDto>>(products));
        }

        //Filter
        [HttpGet("filter")]
        public async Task<IActionResult> FilterProducts(
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        int page = 1,
        int pageSize = 10)
        {
            var (items, totalCount) = await _repo.FilterAsync(
                   categoryId,
                   minPrice,
                   maxPrice,
                   page,
                   pageSize
               );
            return Ok(new
                {
                    items = _mapper.Map<List<ProductDto>>(items),
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        //pagination
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedProducts(int page = 1,int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var (items, totalCount) = await _repo.GetPagedAsync(page, pageSize);

            return Ok(new
            {
                items = _mapper.Map<List<ProductDto>>(items),
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }


    }
}
