using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library_Automation.ViewModels
{
   
        public class CatalogIndexViewModel
        {
            public string? Search { get; set; }
            public int? CategoryId { get; set; }
            public bool OnlyAvailable { get; set; }

            public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
            public List<CatalogItemViewModel> Books { get; set; } = new List<CatalogItemViewModel>();
        }

  
}
