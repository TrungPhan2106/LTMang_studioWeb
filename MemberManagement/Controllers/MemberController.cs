using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioManagement.Models;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudioManagement.Controllers
{
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
        [HttpPost]
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var member = _context.Members.Include(m => m.User).FirstOrDefault(m => m.UserId.ToString() == userId);

            if (member == null)
            {
                return NotFound();
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

                // Cập nhật thông tin thành viên
                existingMember.DateOfBirth = model.DateOfBirth;
                existingMember.PhoneNumber = model.PhoneNumber;
                existingMember.Gender = model.Gender;
                existingMember.Address = model.Address;
                existingMember.JoinedDate = model.JoinedDate;

                // Xử lý ảnh nếu có
                if (file != null)
                {
                    string memberPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "member");
                    existingMember.ImageUrl = await SaveImage(file, memberPath, existingMember.ImageUrl);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("ViewProfile");
            }

            return View(model);
        }

        // Xem thông tin cá nhân
        public IActionResult ViewProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var member = _context.Members.Include(m => m.User).FirstOrDefault(m => m.UserId.ToString() == userId);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // Hàm phụ để lưu ảnh mới và xóa ảnh cũ nếu có
        private async Task<string> SaveImage(IFormFile file, string folderPath, string existingImagePath)
        {
            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Xóa ảnh cũ nếu có
            if (!string.IsNullOrEmpty(existingImagePath))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingImagePath.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // Tạo tên file mới và lưu ảnh
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string fullPath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Trả về đường dẫn ảnh để lưu vào cơ sở dữ liệu
            return Path.Combine("images", "member", fileName).Replace("\\", "/");
        }
    }
}
