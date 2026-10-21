using System.ComponentModel.DataAnnotations;

namespace Library_Automation.ViewModels
{
    public class CatalogItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Publisher { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int Floor { get; set; }
        public string? Shelf { get; set; }
        

       
        public int? PublishYear { get; set; }

        
        public int? PageCount { get; set; }

   
        public IFormFile? CoverFile { get; set; }//formdan gelen veriyi get ile yakalamışken request üzerinden alınan dosyanın kendisi

        public string? CoverFileName { get; set; }//edit üzerinde kitabın kapağını gösterebilmek için dosyanın yolu
        //CopyToAsync ile dosya adı boyutu ve içeriğini okuyup buraya dosya yolunu döneceğiz

        public bool IsAvailable { get; set; }

    }
}