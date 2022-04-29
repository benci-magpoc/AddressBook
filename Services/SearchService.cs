using AddressBook.Data;
using AddressBook.Models;
using Microsoft.EntityFrameworkCore;

namespace AddressBook.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;
        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Contact> SearchContacts(string searchString, string userId)
        {
            var result = _context.Contacts
                .Include(c => c.Categories)
                .Where(c => c.AppUserId == userId)
                .AsQueryable();
            if (searchString != null)
            {
                result = result.Where(c => c.Address1!.ToLower().Contains(searchString.ToLower())
                                           || c.Address2!.ToLower().Contains(searchString.ToLower())
                                           || c.City!.ToLower().Contains(searchString.ToLower())
                                           || c.Email!.ToLower().Contains(searchString.ToLower())
                                           || c.FirstName!.ToLower().Contains(searchString.ToLower())
                                           || c.LastName!.ToLower().Contains(searchString.ToLower())
                                           || c.Categories.Any(t => t.Name!.ToLower().Contains(searchString.ToLower())));
            }
            return result.OrderByDescending(c => c.LastName).ThenByDescending(c => c.FirstName);
        }
    }
}
