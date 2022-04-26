using System.ComponentModel.DataAnnotations;

namespace AddressBook.Models
{
    public class Category
    {
        public int Id { get; set; } 
        public string? AppUserId { get; set; }
        
        [Required]
        [Display(Name = "Category Name")]
        public string? Name { get; set; }

        //TODO: Add virtual
        public virtual AppUser? AppUser { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; } = new HashSet<Contact>();

    }
}
