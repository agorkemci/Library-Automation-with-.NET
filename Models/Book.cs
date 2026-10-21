using System.ComponentModel.DataAnnotations;

namespace Library_Automation.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string Author { get; set; } = string.Empty;
        [StringLength(150)]
        public string? Publisher { get; set; }
        [Required,StringLength(20)]
        public string ISBN { get; set; } = string.Empty;

        public int Floor { get; set; }

        [StringLength(50)]
        public string? Shelf{ get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();

        [Range(1000,2100)]
        public int? PublishYear { get; set; }

        [Range(1,10000)]
        public int? PageCount { get; set; }

        [StringLength(100)]
        public string? CoverFileName { get; set; }

    }
}
