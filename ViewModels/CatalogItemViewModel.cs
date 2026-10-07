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
        public bool IsAvailable { get; set; }

    }
}