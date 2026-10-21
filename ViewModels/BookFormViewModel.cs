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
        [Range(1000, 2100, ErrorMessage = "Enter a valid year")]
        [Display(Name = "Publication Year")]
        public int? PublishYear { get; set; }

        [Range(1,10000,ErrorMessage ="Enter a valid page count")]
        [Display(Name ="Page Count")]
        public int? PageCount { get; set; }

        [Display(Name ="Cover Image")]
        public IFormFile? CoverFile { get; set; }//formdan gelen veriyi get ile yakalamışken request üzerinden alınan dosyanın kendisi

        public string? CoverFileName { get; set; }//edit üzerinde kitabın kapağını gösterebilmek için dosyanın yolu
        //CopyToAsync ile dosya adı boyutu ve içeriğini okuyup buraya dosya yolunu döneceğiz








        public IEnumerable<SelectListItem> Categories { get; set; }=new List<SelectListItem>();
    }


}

