using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceApi.Models;

namespace FinanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly WealthContext _context;

        public ClientsController(WealthContext context)
        {
            _context = context;
        }

        // GET: api/Clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            var clients = await _context.Clients
                .Include(c => c.Portfolios)
                    .ThenInclude(p => p.Investments)
                .ToListAsync();

            return Ok(clients);
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Portfolios)
                    .ThenInclude(p => p.Investments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return NotFound();

            return Ok(client);
        }


        // PUT: api/Clients/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest();
            }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Clients
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClient", new { id = client.Id }, client);
        }

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("{id}/analytics")]
        public async Task<ActionResult<object>> GetClientAnalytics(int id)
        {
            var client = await _context.Clients
        .Include(c => c.Portfolios)
            .ThenInclude(p => p.Investments)
        .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            // Flatten all investments across all portfolios
            var allInvestments = client.Portfolios.SelectMany(p => p.Investments).ToList();

            decimal totalInvested = allInvestments.Sum(i => i.Units * i.PurchasePrice);
            decimal currentValue = allInvestments.Sum(i => i.Units * i.CurrentPrice);
            decimal roi = totalInvested > 0 ? ((currentValue - totalInvested) / totalInvested) * 100 : 0;

            // Diversification breakdown across all portfolios
            var diversification = allInvestments
                .GroupBy(i => i.AssetType)
                .ToDictionary(
                    g => g.Key,
                    g => Math.Round((g.Sum(i => i.Units * i.CurrentPrice) / (currentValue == 0 ? 1 : currentValue)) * 100, 2)
                );

            // Simple risk rule: >70% stocks = high risk
            string riskLevel = "Low";
            if (diversification.ContainsKey("Stocks") && diversification["Stocks"] > 70)
            {
                riskLevel = "High";
            }
            else if (diversification.ContainsKey("Stocks") && diversification["Stocks"] > 40)
            {
                riskLevel = "Medium";
            }

            return Ok(new
            {
                ClientId = client.Id,
                ClientName = client.FullName,
                TotalPortfolios = client.Portfolios.Count,
                TotalInvested = totalInvested,
                CurrentValue = currentValue,
                RoiPercent = Math.Round(roi, 2),
                Diversification = diversification,
                RiskLevel = riskLevel
            });
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
