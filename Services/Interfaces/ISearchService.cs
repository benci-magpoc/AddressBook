using AddressBook.Models;

namespace AddressBook.Services.Interfaces
{
    public interface ISearchService
    {
        public IEnumerable<Contact> SearchContacts(string searchString, string userId);

    }
}
