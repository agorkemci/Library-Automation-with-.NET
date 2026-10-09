using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library_Automation.ViewModels
{
   
        public class CatalogIndexViewModel
        {
            public string? Search { get; set; }
            public int? CategoryId { get; set; }
            public bool OnlyAvailable { get; set; }
            public int Page { get; set; } = 1;//o anki page
            public int PageSize { get; set; } = 10;//bir page kaç record tutacak        
            public int TotalCount { get; set; }//db' den kaç kayıt geliyor
            //gösterilecek total sayfa sayısı: örnek 23 kayıt var 23/10.0=2.3 => 3 sayfa
            public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
            public bool HasPrevious => Page > 1;//birden fazla sayfa varsa true
            public bool HasNext => Page < TotalPages;//şuanki page total pageten küçükse next vardır

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
            public List<CatalogItemViewModel> Books { get; set; } = new List<CatalogItemViewModel>();
        }

  
}
