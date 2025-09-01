namespace FinanceApi.Models
{
    public class Portfolio
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TotalInvestment { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; }
        public ICollection<Investment> Investments { get; set; } = new List<Investment>();
    }
}
