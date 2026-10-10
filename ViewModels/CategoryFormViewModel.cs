using System.ComponentModel.DataAnnotations;

namespace Library_Automation.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }


        [Required(ErrorMessage ="Category name is required")]
        [StringLength(100)]
        [Display(Name="Category Name")]
        public string Name { get; set; } = string.Empty;




    }
    
}
