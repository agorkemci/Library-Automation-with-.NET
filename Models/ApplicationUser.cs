using Microsoft.AspNetCore.Identity;

namespace Library_Automation.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; } = string.Empty;//Id, email, password hash gibi alanlar identity userdan kalıtıldı.
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
