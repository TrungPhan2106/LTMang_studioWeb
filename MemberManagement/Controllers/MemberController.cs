using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioManagement.Models;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudioManagement.Controllers
{
    [Authorize(Policy = "MemberOnly")]
    public class MemberController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MemberController(MyDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
       
        // Đăng ký studio
        [HttpGet]
        public async Task<IActionResult> RegisterStudio(int studioId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId.ToString() == userId);

            if (member == null)
            {
                return NotFound("Không tìm thấy thành viên.");
            }

            member.StudioID = studioId;
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewProfile");
        }

        // Xem và cập nhật thông tin cá nhân
        [HttpGet]
        public IActionResult EditProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return BadRequest("Invalid UserId"); // Trả về lỗi nếu UserId không hợp lệ
            }

            var member = _context.Members
                                 .Include(m => m.User)
                                 .FirstOrDefault(m => m.UserId == userId);

            if (member == null)
            {
                // Tạo mới thành viên nếu chưa tồn tại
                member = new Member
                {
                    UserId = userId,
                    DateOfBirth = DateTime.Now, // Gán giá trị mặc định
                    JoinedDate = DateTime.Now
                };
                _context.Members.Add(member);
                _context.SaveChanges();
            }

            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(Member model, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var existingMember = await _context.Members.Include(m => m.User).FirstOrDefaultAsync(m => m.UserId == model.UserId);

                if (existingMember == null)
                {
                    return NotFound();
                }

                // Handle file upload
                if (file != null && file.Length > 0)
                {
                    // Define the path to save the image
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "member");
                    Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Update the ImageUrl property
                    existingMember.ImageUrl = Path.Combine("member", uniqueFileName).Replace("\\", "/");
                }

                // Update other properties
                existingMember.DateOfBirth = model.DateOfBirth;
                existingMember.PhoneNumber = model.PhoneNumber;
                existingMember.Gender = model.Gender;
                existingMember.Address = model.Address;
                existingMember.JoinedDate = model.JoinedDate;

                await _context.SaveChangesAsync();
                return RedirectToAction("ViewProfile");
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult ViewProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var member = _context.Members
                .Include(m => m.User)
                .Include(m => m.Studio)
                .FirstOrDefault(m => m.UserId.ToString() == userId);

            if (member == null)
            {
                TempData["Message"] = "Bạn cần cập nhật thông tin cá nhân.";
                return RedirectToAction("EditProfile"); // Chuyển hướng đến EditProfile
            }

            return View(member);
        }
    }
}
