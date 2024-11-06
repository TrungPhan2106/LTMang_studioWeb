using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using StudioManagement.Models;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudioManagement.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(MyDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Xem thông tin Studio
        public IActionResult ViewStudio(int id)
        {
            var studio = _context.Studios.Find(id);
            if (studio == null)
            {
                return NotFound();
            }
            return View(studio);
        }

        // Xem và cập nhật thông tin bản thân
        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _context.Employees.FirstOrDefault(e => e.UserId.ToString() == userId);
            if (employee == null)
            {
                return NotFound();
            }
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

                if (file != null && IsValidImageFile(file))
                {
                    string employeePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\employee");
                    existingEmployee.ImageUrl = await SaveImage(file, employeePath, existingEmployee.ImageUrl);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("ViewProfile");
            }
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
            var employee = _context.Employees.FirstOrDefault(e => e.UserId.ToString() == userId);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // Quản lý member trong Studio
        public IActionResult MembersList(int studioId)
        {
            var studio = _context.Studios.Find(studioId);
            if (studio == null)
            {
                return NotFound();
            }
            var members = _context.Members.Where(m => m.StudioID == studioId).ToList();
            return View(members);
        }

        public IActionResult MemberDetails(int id)
        {
            var member = _context.Members.Find(id);
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
            var member = _context.Members.Find(id);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("MembersList", new { studioId = studioId });
        }

        // Chỉnh sửa thông tin Studio cho employee
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var studio = _context.Studios.Find(id);
            if (studio == null)
            {
                return NotFound();
            }
            return View(studio);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Studio model, IFormFile? file)
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

                if (file != null && IsValidImageFile(file))
                {
                    string studioPath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\studio");
                    existingStudio.StudioPic = await SaveImage(file, studioPath, existingStudio.StudioPic);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Get", "Studio", new { id = model.StudioID });
            }
            return View(model);
        }
    }
}
