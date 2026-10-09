namespace Library_Automation.ViewModels
{
    public class CategoryStatViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percent { get; set; }
    }
    public class RecentBookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string CategoryName {  get; set; } = string.Empty;
    }

    public class ReportViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalMembers { get; set; }

        public int ActiveLoans { get; set; }
        public int OverdueCount { get; set; }

        public List<CategoryStatViewModel> CategoryStats { get; set; } = new List<CategoryStatViewModel>();
        public List<RecentBookViewModel> RecentBooks { get; set; } = new List<RecentBookViewModel>();
        public List<LoanListItemViewModel> OverdueLoans { get; set; } = new List<LoanListItemViewModel>();
    }
}







