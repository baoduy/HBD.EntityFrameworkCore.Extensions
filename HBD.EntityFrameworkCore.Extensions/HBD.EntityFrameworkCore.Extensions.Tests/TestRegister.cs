using System;
using System.Linq.Expressions;
using DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using HBD.EntityFrameworkCore.Extensions.Abstractions;
using TestSupport.EfHelpers;

namespace HBD.EntityFrameworkCore.Extensions.Tests
{
    [TestClass]
    public class TestRegister
    {
        private const string ConnectionString = "Data Source=db.db";

        [TestMethod]
        public async Task TestCreateDb()
        {
            var options = SqliteInMemory.CreateOptions<MyDbContext>();

            using (var db = new MyDbContext(new DbContextOptionsBuilder(options)
                .RegisterEntities(op=>op.FromAssemblies(typeof(MyDbContext).Assembly))
                .Options))
            {
                await db.Database.EnsureCreatedAsync();

                //Create User with Address
                await db.Set<User>().AddAsync(new User
                {
                    FirstName = "Duy",
                    LastName = "Hoang",
                    Addresses = {
                        new Address
                        {
                            Street = "12"
                        }
                    }
                });

                await db.SaveChangesAsync();

                Assert.IsTrue(await db.Set<User>().CountAsync() == 1);
                Assert.IsTrue(await db.Set<Address>().CountAsync() == 1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public async Task TestCreateDb_Validate()
        {
            var options = SqliteInMemory.CreateOptions<MyDbContext>();

            using (var db = new MyDbContext(new DbContextOptionsBuilder(options)
                .RegisterEntities(op => op.FromAssemblies(typeof(MyDbContext).Assembly))
                .Options))
            {
                await db.Database.EnsureCreatedAsync();

                //Create User with Address
                await db.Set<User>().AddAsync(new User
                {
                    FirstName = "Duy",
                    LastName = "Hoang",
                    Addresses = {
                        new Address()
                    }
                });

                await db.SaveChangesAsync();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestWithCustomEntityMapper_Bad()
        {
            var options = SqliteInMemory.CreateOptions<MyDbContext>();

            var db = new MyDbContext(new DbContextOptionsBuilder(options)
                .RegisterEntities(op =>
                    op.FromAssemblies(typeof(MyDbContext).Assembly).WithDefaultMapperType(typeof(Entity<>)))
                .Options);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWithCustomEntityMapper_NullFilter_Bad()
        {
            var options = SqliteInMemory.CreateOptions<MyDbContext>();
            
            var db = new MyDbContext(new DbContextOptionsBuilder(options)
                .RegisterEntities(op =>
                    op.FromAssemblies(typeof(MyDbContext).Assembly).WithFilter(null))
                .Options);
        }
    }
}