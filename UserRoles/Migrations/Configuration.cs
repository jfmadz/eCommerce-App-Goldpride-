namespace UserRoles.Migrations
{
    using Microsoft.AspNet.Identity;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<UserRoles.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(UserRoles.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var passwordHash2 = new PasswordHasher();
            string password2 = passwordHash2.HashPassword("ctek2020");

            context.Users.AddOrUpdate(x => x.Id,
                new Models.ApplicationUser()
                {
                    Id = "1",
                    Email = "codetek1@outlook.com",
                    EmailConfirmed = true,
                    Name = "Admin",
                    Surname = "Admin",
                    PasswordHash = password2,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEndDateUtc = null,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    UserName = "codetek1@outlook.com"
                });

            var passwordHash = new PasswordHasher();
            string password = passwordHash.HashPassword("Test01");

            context.Users.AddOrUpdate(x => x.Id,
                new Models.ApplicationUser()
                {
                    Id = "2",
                    Email = "test@gmail.com",
                    EmailConfirmed = true,
                    Name = "Test",
                    Surname = "Tester",
                    PasswordHash = password,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEndDateUtc = null,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    UserName = "test@gmail.com"
                });

            var passwordHash3 = new PasswordHasher();
            string password3 = passwordHash3.HashPassword("Test02");

            context.Users.AddOrUpdate(x => x.Id,
                new Models.ApplicationUser()
                {
                    Id = "3",
                    Email = "test2@gmail.com",
                    EmailConfirmed = true,
                    Name = "Test2",
                    Surname = "Tester2",
                    PasswordHash = password3,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEndDateUtc = null,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    UserName = "test2@gmail.com"
                });

            var passwordHash4 = new PasswordHasher();
            string password4 = passwordHash4.HashPassword("Password1");

            context.Users.AddOrUpdate(x => x.Id,
                new Models.ApplicationUser()
                {
                    Id = "4",
                    Email = "josh.madurai@gmail.com",
                    EmailConfirmed = true,
                    Name = "Joshua",
                    Surname = "Madurai",
                    PasswordHash = password4,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEndDateUtc = null,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    UserName = "josh.madurai@gmail.com"
                });
        }
    }
}
