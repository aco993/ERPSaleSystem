namespace ERPSalesSystem.Models.ViewModels
{
    // This Viewmodel should be used for Stats of Saled items
    public class SalesReportViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
