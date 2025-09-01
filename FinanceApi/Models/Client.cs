namespace FinanceApi.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int NetWorth { get; set; }
        public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
    }
}
