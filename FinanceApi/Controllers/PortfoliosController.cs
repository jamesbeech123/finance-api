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
    public class PortfoliosController : ControllerBase
    {
        private readonly WealthContext _context;

        public PortfoliosController(WealthContext context)
        {
            _context = context;
        }



        // GET: api/Portfolios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Portfolio>> GetPortfolio(int id)
        {
            var portfolio = await _context.Portfolios
                .Include(p => p.Client)          
                .Include(p => p.Investments)     
                .FirstOrDefaultAsync(p => p.Id == id); 

            if (portfolio == null)
            {
                return NotFound();
            }

            return portfolio;
        }


        // PUT: api/Portfolios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPortfolio(int id, Portfolio portfolio)
        {
            if (id != portfolio.Id)
            {
                return BadRequest();
            }

            _context.Entry(portfolio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PortfolioExists(id))
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

        // POST: api/Portfolios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Portfolio>> PostPortfolio(Portfolio portfolio)
        {
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPortfolio", new { id = portfolio.Id }, portfolio);
        }

        // DELETE: api/Portfolios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortfolio(int id)
        {
            var portfolio = await _context.Portfolios.FindAsync(id);
            if (portfolio == null)
            {
                return NotFound();
            }

            _context.Portfolios.Remove(portfolio);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("{id}/analytics")]
        public async Task<ActionResult<object>> GetPortfolioAnalytics(int id)
        {
            var portfolio = await _context.Portfolios
                .Include(p => p.Investments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (portfolio == null)
            {
                return NotFound();
            }

            decimal totalInvested = portfolio.Investments.Sum(i => i.Units * i.PurchasePrice);
            decimal currentValue = portfolio.Investments.Sum(i => i.Units * i.CurrentPrice);
            decimal roi = totalInvested > 0 ? ((currentValue - totalInvested) / totalInvested) * 100 : 0;

            //Diversification
            var diversification = portfolio.Investments
                .GroupBy(i => i.AssetType)
                .ToDictionary(
                g => g.Key,
                g => Math.Round((g.Sum(i => i.Units * i.CurrentPrice)/ currentValue) * 100, 2)
                );

            //Risk Rule: >70% stocks = high risk
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
                PortfolioId = portfolio.Id,
                PortfolioName = portfolio.Name,
                TotalInvested = totalInvested,
                CurrentValue = currentValue,
                RoiPercent = Math.Round(roi, 2),
                Diversification = diversification,
                RiskLevel = riskLevel
            });
        }

        private bool PortfolioExists(int id)
        {
            return _context.Portfolios.Any(e => e.Id == id);
        }


    }
}
