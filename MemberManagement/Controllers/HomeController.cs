using StudioManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics.Metrics;
using Syncfusion.Pdf;
using Syncfusion.HtmlConverter;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace StudioManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly MyDbContext _context;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, MyDbContext context)
        {
            _logger = logger;
            logger.LogWarning("This is a MEL warning on the privacy page");
            _webHostEnvironment = webHostEnvironment;
            _context = context;

        }

        public IActionResult Index()
        {
            var studios = _context.Studios.ToList();
            return View(studios);
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("About page privacy.");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Xem thông tin Studio
        public ActionResult Get(int? StudioID)
        {
            if (!StudioID.HasValue)
            {
                return BadRequest("StudioID is required.");
            }

            var studio = _context.Studios.Find(StudioID);
            if (studio == null)
            {
                return NotFound(); // Trả về NotFound nếu không tìm thấy studio
            }

            return View(studio);
        }
    }
}
