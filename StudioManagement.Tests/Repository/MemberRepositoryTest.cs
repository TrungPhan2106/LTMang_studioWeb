using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using StudioManagement.Data;
using StudioManagement.Repository;
using StudioManagement.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Member = StudioManagement.Data.Member;

namespace StudioManagement.Tests.Repository
{
    public class MemberRepositoryTest
    {
        private async Task<MyDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new MyDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }
        [Fact]
        public async void MemberRepository_Add_ReturnBool()
        {
            //arrange
            var member = new Data.Member()
            {
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                PhoneNumber = "00000000",
                DateOfBirth = DateTime.Now,
                Gender = true,
                Address = "tp.hcm",
                JoinedDate = DateTime.Now,
                ExpiredDate = DateTime.Now,
                Status = "",
                IsDeleted= false,
                ImageUrl = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640",
            };
            var dbContext = await GetDbContext();
            var memberRepository = new MemberRepository(dbContext);
            //act
            memberRepository.Add(member);
        }
        [Fact]
        public async void MemberRepository_Update_ReturnBool()
        {
            var member = new Data.Member()
            {
                MemberId =1,
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                DateOfBirth= DateTime.Now,
                Gender= true,
                JoinedDate= DateTime.Now,
                PhoneNumber = "00000000"
            };
            var dbContext = await GetDbContext();
            var memberRepository = new MemberRepository(dbContext);
            memberRepository.Add(member);
            var sut = new MemberRepository(dbContext);
            sut.Update(member);
            Assert.Empty(dbContext.Member);
        }

        [Fact]
        public async void MemberRepository_Delete_ReturnBool()
        {
            var member = new Data.Member()
            {
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                PhoneNumber = "00000000",
                ImageUrl = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640"
            };
            var dbContext = await GetDbContext();
            var memberRepository = new MemberRepository(dbContext);
            memberRepository.Add(member);
            var sut = new MemberRepository(dbContext);
            sut.Remove(member);
            Assert.Empty(dbContext.Member);
        }

        [Fact]
        public async void MemberRepository_Get_ReturnBool()
        {
            var MemberId = 1;
            var member = new Data.Member()
            {
                MemberId =1,
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                PhoneNumber = "00000000",
                ImageUrl = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640",
                StudioID=1
            };
            var dbContext = await GetDbContext();
            var memberRepository = new MemberRepository(dbContext);
            memberRepository.Add(member);
            var sut = new MemberRepository(dbContext);
            sut.Get(x => x.MemberId == MemberId);
            Assert.Empty(dbContext.Member);
        }

        [Fact]
        public async void MemberRepository_GetAll_ReturnBool()
        {
            
            var member1 = new Data.Member()
            {
                UserName = "UserName1",
                FullName = "FullName1",
                Email = "Email",
                PhoneNumber = "00000000",
                ImageUrl = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640",
                StudioID = 1
            };
            var member2 = new Data.Member()
            {
                UserName = "UserName2",
                FullName = "FullName2",
                Email = "Email",
                PhoneNumber = "00000000",
                ImageUrl = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640",
                StudioID = 2
            };
            
            var dbContext = await GetDbContext();
            var memberRepository = new MemberRepository(dbContext);
            memberRepository.Add(member1);
            memberRepository.Add(member2);
            var sut = new MemberRepository(dbContext);
            sut.GetAll();
            Assert.Empty(dbContext.Member);
        }
    }
}
