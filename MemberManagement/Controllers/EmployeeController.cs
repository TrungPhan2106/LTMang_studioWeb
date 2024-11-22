using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using StudioManagement.Models;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudioManagement.Controllers
{
    [Authorize(Policy = "EmployeeOnly")]
    public class EmployeeController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(MyDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Xem và cập nhật thông tin bản thân
        [HttpGet]
        public IActionResult EditProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return BadRequest("Invalid UserId."); // Trả về lỗi nếu UserId không hợp lệ
            }

            // Tìm Employee trong database
            var employee = _context.Employees.FirstOrDefault(e => e.UserId == userId);

            if (employee == null)
            {
                // Tạo Employee mới nếu không tìm thấy
                employee = new Employees
                {
                    UserId = userId,
                    DateOfBirth = DateTime.Now // Gán giá trị mặc định
                };
                _context.Employees.Add(employee);
                _context.SaveChanges();
            }
            ViewBag.StudioList = new SelectList(_context.Studios, "StudioID", "StudioName");
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(Employees model, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var existingEmployee = await _context.Employees.FindAsync(model.EmployeeId);
                if (existingEmployee == null)
                {
                    return NotFound();
                }

                existingEmployee.PhoneNumber = model.PhoneNumber;
                existingEmployee.Gender = model.Gender;
                existingEmployee.DateOfBirth = model.DateOfBirth;
                existingEmployee.StudioID = model.StudioID;

                // Handle file upload
                if (file != null && file.Length > 0)
                {
                    // Define the path to save the image
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "employee");
                    Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Update the ImageUrl property
                    existingEmployee.ImageUrl = Path.Combine("member", uniqueFileName).Replace("\\", "/");
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("ViewProfile");
            }
            ViewBag.StudioList = new SelectList(_context.Studios, "StudioID", "StudioName");
            return View(model);
        }

        // Hàm phụ kiểm tra định dạng file ảnh
        private bool IsValidImageFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension);
        }

        // Hàm phụ lưu ảnh và xóa ảnh cũ nếu có
        private async Task<string> SaveImage(IFormFile file, string path, string existingImageUrl)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string fullPath = Path.Combine(path, fileName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!string.IsNullOrEmpty(existingImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return @"\images\employee\" + fileName;
        }

        public IActionResult ViewProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _context.Employees
            .Include(e => e.User)
            .Include(e => e.Studio) 
            .FirstOrDefault(e => e.UserId.ToString() == userId);
            if (employee == null)
            {
                TempData["Message"] = "Bạn cần cập nhật thông tin cá nhân.";
                return RedirectToAction("EditProfile"); // Chuyển hướng đến EditProfile
            }
            return View(employee);
        }

        // Quản lý member trong Studio
        public IActionResult MembersList(int? studioId)
        {
            if (studioId.HasValue)
            {
                var studio = _context.Studios.Find(studioId.Value);
                if (studio == null)
                {
                    return NotFound();
                }

                // Lấy các thành viên thuộc studioId
                var members = _context.Members
                    .Include(m => m.User) .Include(m => m.Studio)
                    .Where(m => m.StudioID == studioId.Value)
                    .ToList();

                return View(members);
            }

            // Lấy tất cả các thành viên nếu không có studioId
            var allMembers = _context.Members
                .Include(m => m.User)
                .Include(m => m.Studio)
                .ToList();
            return View(allMembers);
        }


        public IActionResult MemberDetails(int id)
        {
            var member = _context.Members
                .Include(m => m.User)
                .Include(m => m.Studio)
                .FirstOrDefault(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMember(int id, int studioId)
        {
            var member = _context.Members?.Find(id);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("MembersList", new { studioId = studioId });
        }

        // Chỉnh sửa thông tin Studio cho employee
        [HttpGet]
        public IActionResult EditStudio(int id)
        {
            var studio = _context.Studios?.Find(id);
            if (studio == null)
            {
                return NotFound();
            }
            return View(studio);
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> EditStudio(Studio model, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                var existingStudio = await _context.Studios.FindAsync(model.StudioID);
                if (existingStudio == null)
                {
                    return NotFound();
                }

                existingStudio.StudioName = model.StudioName;
                existingStudio.StudioAddress = model.StudioAddress;
                existingStudio.StudioPhone = model.StudioPhone;

                if (file != null && file.Length > 0)
                {
                    // Define the path to save the image
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "studio");
                    Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Update the ImageUrl property
                    existingStudio.StudioPic = Path.Combine("studio", uniqueFileName).Replace("\\", "/");
                }


                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}
