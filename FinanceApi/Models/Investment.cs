namespace FinanceApi.Models
{
    public class Investment
    {
        public int Id { get; set; }
        public string AssetName { get; set; }
        public string AssetType { get; set; }
        public decimal Units { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal CurrentPrice { get; set; }
        public int PortfolioId { get; set; }
        public Portfolio Portfolio { get; set; } 
    }
}
