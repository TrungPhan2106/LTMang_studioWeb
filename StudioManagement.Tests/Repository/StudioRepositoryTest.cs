using Microsoft.EntityFrameworkCore;
using Moq;
using StudioManagement.Data;
using StudioManagement.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StudioManagement.Tests.Repository
{
    public class StudioRepositoryTest
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
        public async void StudioRepository_Add_ReturnBool()
        {
            //arrange
            var studio = new Studio()
            {
                StudioName = "Studio 1",
                StudioAddress = "TP.HCM",
                StudioPhone = "000000000",
                StudioPic = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640"
            };
            var dbContext = await GetDbContext();
            var studioRepository = new StudioRepository(dbContext);
            //act
            studioRepository.Add(studio);
            //assert
        }
        [Fact]
        public async void StudioRepository_Update_ReturnBool()
        {
            var studio = new Studio()
            {
                StudioName = "Studio 1",
                StudioAddress = "TP.HCM",
                StudioPhone = "000000000",
                StudioPic = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640"
            };
            var dbContext = await GetDbContext();
            var studioRepository = new StudioRepository(dbContext);
            studioRepository.Add(studio);
            var sut = new StudioRepository(dbContext);
            sut.Update(studio);
            Assert.Empty(dbContext.Studio);
        }

        [Fact]
        public async void StudioRepository_Delete_ReturnBool()
        {
            var studio = new Studio()
            {
                StudioName = "Studio 1",
                StudioAddress = "TP.HCM",
                StudioPhone = "000000000",
                StudioPic = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640"
            };
            var dbContext = await GetDbContext();
            var studioRepository = new StudioRepository(dbContext);
            studioRepository.Add(studio);
            var sut = new StudioRepository(dbContext);
            sut.Remove(studio);
            Assert.Empty(dbContext.Studio);
        }

        [Fact]
        public async void StudioRepository_Get_ReturnBool()
        {
            var StudioID = 1;
            var studio = new Studio()
            {
                StudioName = "Studio 1",
                StudioAddress = "TP.HCM",
                StudioPhone = "000000000",
                StudioPic = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640"
            };
            var dbContext = await GetDbContext();
            var studioRepository = new StudioRepository(dbContext);
            studioRepository.Add(studio);
            var sut = new StudioRepository(dbContext);
            sut.Get(x => x.StudioID == StudioID);
            Assert.Empty(dbContext.Studio);
        }

        [Fact]
        public async void StudioRepository_GetAll_ReturnBool()
        {
            var studio1 = new Studio()
            {

                StudioName = "Studio 1",
                StudioAddress = "TP.HCM",
                StudioPhone = "000000000",
                StudioPic = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640"
            };
            var studio2 = new Studio()
            {

                StudioName = "Studio 2",
                StudioAddress = "TP.HCM",
                StudioPhone = "000000001",
                StudioPic = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640"
            };
            var dbContext = await GetDbContext();
            var studioRepository = new StudioRepository(dbContext);
            studioRepository.Add(studio1);
            studioRepository.Add(studio2);
            var sut = new StudioRepository(dbContext);
            sut.GetAll();
            Assert.Empty(dbContext.Studio);
        }
    }
}
