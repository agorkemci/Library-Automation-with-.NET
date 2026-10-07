namespace Library_Automation.ViewModels
{
    public class LoanListItemViewModel
    {
        public int Id { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string BookISBN { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public bool IsReturned=>ReturnDate!= null;
        public bool IsOverDue => ReturnDate == null && DueDate < DateTime.UtcNow;




    }
}
