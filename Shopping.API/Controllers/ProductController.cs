using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Shopping.API.Models;
using Shopping.API.Data;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Shopping.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ProductContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            _logger.LogInformation("Fetching all products");
            var products = await GetProducts();
            return Ok(products);
        }
      
        private async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context
                .Products
                .Find(p => true)
                .ToListAsync();
        }

    }
}
