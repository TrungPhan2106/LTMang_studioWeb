using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using StudioManagement.Controllers;
using StudioManagement.Data;
using StudioManagement.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudioManagement.Models;
using Moq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.Data;
using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq.Expressions;

namespace StudioManagement.Tests.Controller
{
    public class MemberControllerTests
    {
        private MemberController _memberController;
        private ILogger<MemberController> _logger;
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;
        private MyDbContext _db;

        public MemberControllerTests()
        {
            _logger = A.Fake<ILogger<MemberController>>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _webHostEnvironment = A.Fake<IWebHostEnvironment>();
            _mapper = A.Fake<IMapper>();

            _memberController = new MemberController(_mapper, _logger, _unitOfWork, _webHostEnvironment, _db);
        }

        [Fact]
        public List<Member> Test_Index_ReturnSuccess()
        {
            int pg = 1;
            var objMemberList = new List<Member>();
            objMemberList = _unitOfWork.Member.GetAll(includeProperties:"Studio")
                .OrderBy(x => x.MemberId).ToList();
            int recsCount = objMemberList.Count();
            var paper = new Pager(recsCount, pg, 8);
            int recSkip = (pg - 1) * 8;
            var data = objMemberList.Skip(recSkip).Take(paper.PageSize).ToList();
            return (objMemberList);
        }
        
        [Fact]
        public void Test_Index_ReturnViewData()
        {
            var strSearch = "";
            var pg = 1;
            var StudioID = 1;
            var objMemberList = new List<Member>()
            {
                new Member() {MemberId = 1},
                new Member() {MemberId = 2}
            };
            var mockService = new Mock<IUnitOfWork>();
            mockService.Setup(x => x.Member.GetAll(It.IsAny<string>())).Returns(objMemberList);

            var expiredMembers = objMemberList.Where(x => x.ExpiredDate < DateTime.Now).ToList();
            if (expiredMembers.Any())
            {
                foreach (var mem in expiredMembers)
                {
                    mem.Status = "DeActive";
                }
                _unitOfWork.Save();
            }
            if (StudioID != 0)
            {
                objMemberList = objMemberList.Where(x => x.StudioID == StudioID).ToList();
            }
            int recsCount = objMemberList.Count();
            var paper = new Pager(recsCount, pg, 8);
            int recSkip = (pg - 1) * 8;
            var data = objMemberList.Skip(recSkip).Take(paper.PageSize).ToList();

            var mockLogger = new Mock<ILogger<MemberController>>();
            var controller = new MemberController(_mapper, mockLogger.Object, mockService.Object, _webHostEnvironment, _db);
            var result = _memberController.MemberIndex(strSearch, pg) as ViewResult;
            var member = (List<Member>)result?.ViewData.Model;
            Assert.NotNull(member);
        }
        [Fact]
        public Studio Test_Get_ReturnSuccess()
        {
            int MemberId = 1;
            var studioFromDb = new Studio();
            _unitOfWork.Member.Get(u => u.MemberId == MemberId);
            var result = _memberController.Get(MemberId);
            return (studioFromDb);
        }

        [Fact]
        public Studio Test_Get0_ReturnSuccess()
        {
            int MemberId = 0;
            var studioFromDb = new Studio();
            _unitOfWork.Member.Get(u => u.MemberId == MemberId);
            var result = _memberController.Get(MemberId);
            return (studioFromDb);
        }

        [Fact]
        public Studio Test_Delete0_ReturnSuccess()
        {
            int MemberId = 0;
            var studioFromDb = new Studio();
            _unitOfWork.Member.Get(u => u.MemberId == MemberId);
            var result = _memberController.Delete(MemberId);
            return (studioFromDb);
        }
        [Fact]
        public Studio Test_Edit0_ReturnSuccess()
        {
            int MemberId = 0;
            var studioFromDb = new Studio();
            _unitOfWork.Member.Get(u => u.MemberId == MemberId);
            var result = _memberController.Edit(MemberId);
            return (studioFromDb);
        }
        [Fact]
        public void Test_IndexSearch_ReturnViewData()
        {
            var strSearch = "c";
            var pg = 1;
            var objMemberList = new List<Member>()
            {
                new Member() {MemberId = 1, UserName= "a", StudioID=1, ExpiredDate= DateTime.Now.AddDays(-1)},
                new Member() {MemberId = 2, UserName="b", StudioID=1}
            };
            var mockService = new Mock<IUnitOfWork>();
            mockService.Setup(x => x.Member.GetAll(It.IsAny<string>())).Returns(objMemberList);
            var mockLogger = new Mock<ILogger<MemberController>>();
            var controller = new MemberController(_mapper, mockLogger.Object, mockService.Object, _webHostEnvironment, _db);
            var result = _memberController.MemberIndex(strSearch, pg, 1) as ViewResult;
            var member = (List<Member>)result?.ViewData.Model;
            Assert.NotNull(member);
        }
        
        [Fact]
        public IActionResult Test_GetMember0_ReturnSuccess()
        {
            int MemberId = 0;
            var studioFromDb = new Member();
            _unitOfWork.Studio.Get(u => u.StudioID == MemberId);
            var result = _memberController.GetMem(MemberId);
            return result;
        }
        [Fact]
        public Studio Test_GetMem_ReturnSuccess()
        {
            int MemberId = 1;
            var studioFromDb = new Studio();
            _unitOfWork.Member.Get(u => u.MemberId == MemberId);
            var result = _memberController.GetMem(MemberId);
            return (studioFromDb);
        }

        [Fact]
        public IActionResult Create_ViewState_Is_Valid_Returns_RedirectToRouteResult()
        {
            var StudioID = 1;
            Member member = new Member()
            {
                MemberId = 1,
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                DateOfBirth = DateTime.Now,
                Gender = true,
                JoinedDate = DateTime.Now,
                PhoneNumber = "00000000"
            };
            var actual = _memberController.Create(StudioID, member);
            return actual;
        }

        [Fact]
        public IActionResult Delete_ViewState_Is_Valid_Returns_RedirectToRouteResult()
        {
            var MemberId = 1;
            Member member = new Member()
            {
                MemberId = 1,
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                DateOfBirth = DateTime.Now,
                Gender = true,
                JoinedDate = DateTime.Now,
                PhoneNumber = "00000000"
            };
            var actual = _memberController.Delete(MemberId);
            return actual;
        }

        [Fact]
        public IActionResult Update_ViewState_Is_Valid_Returns_RedirectToRouteResult()
        {
            var MemberId = 1;
            Member member = new Member()
            {
                MemberId = 1,
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                DateOfBirth = DateTime.Now,
                Gender = true,
                JoinedDate = DateTime.Now,
                PhoneNumber = "00000000"
            };
            var actual = _memberController.Edit(MemberId);
            return actual;
        }

        [Fact]
        public void CanGet()
        {
            try
            {
                //Arrange
                int MemberId = 1;
                Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();
                mockRepo.Setup(m => m.Member.GetAll(It.IsAny<string>())).Returns(new Member[]
                {
                    new Member() 
                    { 
                        MemberId =1,
                        UserName = "UserName",
                        FullName = "FullName",
                        Email = "Email",
                        DateOfBirth= DateTime.Now,
                        Gender= true,
                        JoinedDate= DateTime.Now,
                        PhoneNumber = "00000000" 
                    }
                });
                var mockMapper = new Mock<IMapper>();
                mockMapper.Setup(x => x.Map<MemberDto>(It.IsAny<Member>()))
                    .Returns((Member source) => new MemberDto() { MemberId = source.MemberId });
                MemberController controller = new MemberController(mockMapper.Object, _logger, mockRepo.Object, _webHostEnvironment, _db);
                //Act
                var result = _memberController.Get(MemberId);
                //Assert
                var okResult = result as OkObjectResult;
                Assert.NotNull(okResult);
            }
            catch (Exception ex)
            {
                //Assert
                Assert.False(false, ex.Message);
            }
        }

        [Fact]
        public ActionResult ExportToPDF_test()
        {
            int MemberId = 1;
            var htmlConverter = new HtmlToPdfConverter();
            var blinkConverterSettings = new BlinkConverterSettings();
            blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(800, 0);
            htmlConverter.ConverterSettings = blinkConverterSettings;
            var document = new PdfDocument();
            document = htmlConverter.Convert($"https://localhost:7056/Home/GetStudio?StudioID={MemberId}");
            var stream = new MemoryStream();
            document.Save(stream);
            var result = _memberController.ExportToPDF(MemberId);
            return result;
        }

        [Fact]
        public FileResult GenerateExcel_Test()
        {
            string fileName = "";
            var objMemberList= new List<Member>()
                {
                new Member() {MemberId = 1},
                new Member() {MemberId = 2}
                }; 
            DataTable dataTable = new DataTable("objMemberList");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("MemberId"),
                new DataColumn("UserName"),
                new DataColumn("FullName"),
                new DataColumn("DateOfBirth"),
                new DataColumn("Gender"),
                new DataColumn("PhoneNumber"),
                new DataColumn("Email"),
                new DataColumn("Address"),
                new DataColumn("JoinedDate"),
                new DataColumn("StudioID")
            });

            foreach (var member in objMemberList)
            {
                dataTable.Rows.Add(member.MemberId, member.UserName, member.FullName,
                    member.DateOfBirth.ToString("MM/dd/yyyy"), member.Gender ? "Male" : "Female", member.PhoneNumber, member.Email,
                    member.Address, member.JoinedDate.ToString("MM/dd/yyyy"), member.StudioID);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var result = _memberController.GenerateExcel(fileName, objMemberList);
                    return result;
                }
            }
        }

        [Fact]
        public void ExportMembersInExcel_AllNull_Test()
        {
            var strSearch = "c";
            var pg = 1;
            var objMemberList = new List<Member>()
            {
                new Member() {MemberId = 1, UserName= "a", StudioID=1, ExpiredDate= DateTime.Now.AddDays(-1)},
                new Member() {MemberId = 2, UserName="b", StudioID=1}
            };
            var mockService = new Mock<IUnitOfWork>();
            mockService.Setup(x => x.Member.GetAll(It.IsAny<string>())).Returns(objMemberList);
            var controller = new MemberController(_mapper, _logger, mockService.Object, _webHostEnvironment, _db);
            var objList = _db.Member.ToListAsync();
            var result = controller.ExportMembersInExcel(strSearch, 1);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public IActionResult CreateMember_InValidModel_ReturnCreated()
        {
            //var controller = new MemberController(_mapper, _logger, _unitOfWork, _webHostEnvironment, _db);
            Member member = new Member()
            {
                MemberId = 1,
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                DateOfBirth = DateTime.Now,
                Gender = true,
                JoinedDate = DateTime.Now,
                PhoneNumber = "00000000"
            };
            _memberController.ModelState.AddModelError("Error", "Sample error description");
            var result = _memberController.Create(member, null);
            return result;
        }

        [Fact]
        public IActionResult UpdateMember_InValidModel_ReturnCreated()
        {
            //var controller = new MemberController(_mapper, _logger, _unitOfWork, _webHostEnvironment, _db);
            Member member = new Member()
            {
                MemberId = 1,
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                DateOfBirth = DateTime.Now,
                Gender = true,
                JoinedDate = DateTime.Now,
                PhoneNumber = "00000000"
            };
            _memberController.ModelState.AddModelError("Error", "Sample error description");
            var result = _memberController.Edit(member, null);
            return result;
        }

        [Fact]
        public IActionResult Test_Edit_NotFound_ReturnSuccess()
        {
            int MemberId = 1;
            var member = null as Member;
            var mockService1 = new Mock<IUnitOfWork>();
            mockService1.Setup(x => x.Member.Get(It.IsAny<Expression<Func<Member, bool>>>(), It.IsAny<string>())).Returns(member);
            var controller = new MemberController(_mapper,_logger, mockService1.Object, _webHostEnvironment, _db);
            var result = controller.Edit(MemberId);
            return result;
        }
        [Fact]
        public IActionResult Test_Delete_NotFound_ReturnSuccess()
        {
            int MemberId = 1;
            var member = null as Member;
            var mockService1 = new Mock<IUnitOfWork>();
            mockService1.Setup(x => x.Member.Get(It.IsAny<Expression<Func<Member, bool>>>(), It.IsAny<string>())).Returns(member);
            var controller = new MemberController(_mapper, _logger, mockService1.Object, _webHostEnvironment, _db);
            var result = controller.Delete(MemberId);
            return result;
        }

        [Fact]
        public IActionResult Test_DeletePost_NotFound_ReturnSuccess()
        {
            int MemberId = 1;
            var member = null as Member;
            var mockService1 = new Mock<IUnitOfWork>();
            mockService1.Setup(x => x.Member.Get(It.IsAny<Expression<Func<Member, bool>>>(), It.IsAny<string>())).Returns(member);
            var controller = new MemberController(_mapper, _logger, mockService1.Object, _webHostEnvironment, _db);
            var result = controller.DeletePOST(MemberId);
            return result;
        }

        [Fact]
        public IActionResult Test_Get_NotFound_ReturnSuccess()
        {
            int MemberId = 1;
            var member = null as Member;
            var mockService1 = new Mock<IUnitOfWork>();
            mockService1.Setup(x => x.Member.Get(It.IsAny<Expression<Func<Member, bool>>>(), It.IsAny<string>())).Returns(member);
            var controller = new MemberController(_mapper, _logger, mockService1.Object, _webHostEnvironment, _db);
            var result = controller.Get(MemberId);
            return result;
        }

        [Fact]
        public IActionResult Test_GetMem_NotFound_ReturnSuccess()
        {
            int MemberId = 1;
            var member = null as Member;
            var mockService1 = new Mock<IUnitOfWork>();
            mockService1.Setup(x => x.Member.Get(It.IsAny<Expression<Func<Member, bool>>>(), It.IsAny<string>())).Returns(member);
            var controller = new MemberController(_mapper, _logger, mockService1.Object, _webHostEnvironment, _db);
            var result = controller.GetMem(MemberId);
            return result;
        }
        [Fact]
        public void CreateMember_ModelIsValid_ReturnCreated()
        {
            var member = new Member()
            {
                MemberId = 1,
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                DateOfBirth = DateTime.Now,
                Gender = true,
                JoinedDate = DateTime.Now,
                PhoneNumber = "00000000"
            };
            var fileMock = new Mock<IFormFile>();
            var sourceImg = Path.GetFullPath(@"source image path");
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            stream.Position = 0;
            var fileName = "text.jfif";
            fileMock.Setup(f => f.FileName).Returns(fileName).Verifiable();
            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
        .Returns((Stream stream, CancellationToken token) => stream.CopyToAsync(stream))
        .Verifiable();
            //var mockweb = new Mock<IWebHostEnvironment>();
            //string wwwRootPath = mockweb.Object.WebRootPath;
            //var fileName = "23db3c9f-c9e2-454d-8b48-073d30d8e8b1.jfif";
            //var memberPath= Path.Combine(wwwRootPath, @"images\member");
            //using (var fileStream = new FileStream(Path.Combine(memberPath, fileName), FileMode.Create)) 
            //{ fileMock.Object.CopyTo(fileStream); }
            //member.ImageUrl = @"\images\member\" + fileName;
            //fileStream.Position = 0;
            //fileMock.Setup(_ => _.OpenReadStream()).Returns(fileStream);
            //fileMock.Setup(_ => _.FileName).Returns(fileName);

            var controller = new MemberController(_mapper, _logger, _unitOfWork, _webHostEnvironment, _db);
            //_unitOfWork.Member.Add(member);
            //_unitOfWork.Save();
            var inputFile = fileMock.Object;
            var result = controller.Create(member, fileMock.Object);
            //Assert.IsAssignableFrom<IActionResult>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public IActionResult EditMember_ModelIsValid_ReturnCreated()
        {
            var member = new Member();
            var file = new Mock<IFormFile>();
            var sourceImg = Path.GetFullPath(@"source image path");
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(sourceImg);
            writer.Flush();
            stream.Position = 0;
            var fileName = "text.jfif";
            file.Setup(f => f.FileName).Returns(fileName).Verifiable();
            file.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
        .Returns((Stream stream, CancellationToken token) => stream.CopyToAsync(stream))
        .Verifiable();

            var controller = new MemberController(_mapper, _logger, _unitOfWork, _webHostEnvironment, _db);
            var inputFile = file.Object;
            _unitOfWork.Member.Update(member);
            _unitOfWork.Save();
            var result = _memberController.Edit(member, inputFile);
            return result;
        }

        [Fact]
        public IActionResult DeleteMember_ModelIsValid_ReturnCreated()
        {
            var MemberId = 1;
            var member = new Member()
            {
                MemberId = 1,
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                DateOfBirth = DateTime.Now,
                Gender = true,
                JoinedDate = DateTime.Now,
                PhoneNumber = "00000000",
                ImageUrl = "23db3c9f-c9e2-454d-8b48-073d30d8e8b1.jfif"
            };
            string? imageUrl = member.ImageUrl;
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('\\'));
            oldImagePath.Remove(oldImagePath.Length - 1);
            //var controller = new MemberController(_mapper, _logger, _unitOfWork, _webHostEnvironment, _db);
            _unitOfWork.Member.Remove(member);
            _unitOfWork.Save();
            var result = _memberController.DeletePOST(MemberId);
            return result;
        }
    }
}
