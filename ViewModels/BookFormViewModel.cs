using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Library_Automation.ViewModels
{
        public class BookFormViewModel()
        {
            public int Id { get; set; }
            
            
            [Required(ErrorMessage = "Title is required.")]
            [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
            [Display(Name = "Book Title")]
            public string Title { get; set; }
            [Required(ErrorMessage = "Author is required.")]
            [StringLength(150, ErrorMessage = "Author cannot exceed 150 characters.")]
            [Display(Name = "Author Name")]
            public string Author { get; set; }
            [StringLength(150)]
            [Display(Name = "Publisher")]
            public string? Publisher { get; set; }

            [Required(ErrorMessage = "ISBN is required.")]
            [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters.")]
            [Display(Name = "ISBN")]
            public string ISBN { get; set; }
            [Range(0,20, ErrorMessage = "Floor must be between 0 and 20.")]
            [Display(Name = "Floor Number")]
            public int Floor { get; set; }
            [StringLength(50)]
            [Display(Name = "Shelf Location")]
            public string? Shelf { get; set; }
            [Range(1,int.MaxValue, ErrorMessage = "Category is required.")]
            [Display(Name = "Category")]
            public int CategoryId { get; set; }
            public IEnumerable<SelectListItem> Categories { get; set; }=new List<SelectListItem>();
        }


}

