using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping.Client.Models;

namespace Shopping.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HomeController> _logger;      

        public HomeController(IHttpClientFactory httpClientFactory,ILogger<HomeController> logger)
        {            
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("ShoppingAPIClient");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/product");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var productList = JsonConvert.DeserializeObject<IEnumerable<Product>>(content);
                return View(productList);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling Shopping API");
                return View(new List<Product>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Index action");
                return View(new List<Product>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
