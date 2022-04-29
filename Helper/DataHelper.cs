using AddressBook.Data;
using AddressBook.Enums;
using AddressBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AddressBook.Helper
{
    public static class DataHelper
    {
        private const string _tonysEmail = "tonystark@mailinator.com";
        private const string _stevesEmail = "steverogers@mailinator.com";
        private const string _hanksEmail = "hankmccoy@starktower.com";
        public static DateTime GetPostGresDate(DateTime datetime)
        {
            return DateTime.SpecifyKind(datetime, DateTimeKind.Utc);
        }
        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            //Service: An instance of RoleManager
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            //Service: An instance of RoleManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<AppUser>>();
            //Migration: This is the programmatic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();

            await SeedDefaultUserAsync(userManagerSvc);
            await SeedDefaultContacts(dbContextSvc);
            await SeedDefaultCategoriesAsync(dbContextSvc);
            await DefaultCategoryAssign(dbContextSvc);
        }
        private static async Task SeedDefaultUserAsync(UserManager<AppUser> _userManager)
        {
            var defaultUser = new AppUser
            {
                UserName = _tonysEmail,
                Email = _tonysEmail,
                FirstName = "Tony",
                LastName = "Stark",
                EmailConfirmed = true,
            };
            
            try
            {
                var user = await _userManager.FindByNameAsync(defaultUser.Email);
                if (user is null)
                {
                    await _userManager.CreateAsync(defaultUser, "Abc&123!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("**** ERROR ****");
                Console.WriteLine("Error Seeding Default User");
                Console.WriteLine(ex.Message);
                Console.WriteLine("**** END OF ERROR ****");
            }

            defaultUser = new AppUser
            {
                UserName = _stevesEmail,
                Email = _stevesEmail,
                FirstName = "Steve",
                LastName = "Rogers",
                EmailConfirmed = true,
            };

            try
            {
                var user = await _userManager.FindByNameAsync(defaultUser.Email);
                if (user is null)
                {
                    await _userManager.CreateAsync(defaultUser, "Abc&123!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("**** ERROR ****");
                Console.WriteLine("Error Seeding Default User");
                Console.WriteLine(ex.Message);
                Console.WriteLine("**** END OF ERROR ****");
            }

        }
        private static async Task SeedDefaultContacts(ApplicationDbContext context)
        {
            var userId = context.Users.FirstOrDefault(u => u.Email == _tonysEmail)!.Id;
            var defaultContact = new Contact
            {
                AppUserId = userId,
                Created = DateTime.UtcNow,
                Email = _hanksEmail,
                FirstName = "Henry",
                LastName = "McCoy",
                Address1 = "1407 Graymalkin Ln.",
                City = "Salem Center",
                ZipCode = 22010,
                PhoneNumber = "555-555-0101",
                State = States.NY
            };
            try
            {
                var contact = await context.Contacts.AnyAsync(c => c.Email == defaultContact.Email && c.AppUserId == userId);
                if (!contact)
                {
                    await context.AddAsync(defaultContact);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("**** ERROR ****");
                Console.WriteLine("Error Seeding Default Contact");
                Console.WriteLine(ex.Message);
                Console.WriteLine("**** END OF ERROR ****");
            }
        }
        private static async Task SeedDefaultCategoriesAsync(ApplicationDbContext context)
        {
            var userId = context.Users.FirstOrDefault(u => u.Email == _tonysEmail)?.Id;
            var defaultCategory = new Category
            {
                AppUserId = userId,
                Name = "X-Men"
            };
            try
            {
                var category = await context.Categories.AnyAsync(c => c.Name == defaultCategory.Name && c.AppUserId == userId);
                if (!category)
                {
                    await context.AddAsync(defaultCategory);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("**** ERROR ****");
                Console.WriteLine("Error Seeding Default Category");
                Console.WriteLine(ex.Message);
                Console.WriteLine("**** END OF ERROR ****");
            }
        }
        private static async Task DefaultCategoryAssign(ApplicationDbContext context)
        {
            var user = context.Users
                .Include(c => c.Categories)
                .Include(c => c.Contacts)
                .FirstOrDefault(u => u.Email == _tonysEmail);
            var contact = context.Contacts
                .FirstOrDefault(u => u.Email == _hanksEmail);

            if (contact != null)
            {
                foreach (var category in user?.Categories!)
                {
                    bool isInCategory = await context.Categories
                                         .Include(c => c.Contacts)
                                         .Where(c => c.Id == category.Id && c.Contacts.Contains(contact))
                                         .AnyAsync();
                    if (!isInCategory)
                    {
                        category.Contacts.Add(contact);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
