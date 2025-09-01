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
    public class InvestmentsController : ControllerBase
    {
        private readonly WealthContext _context;

        public InvestmentsController(WealthContext context)
        {
            _context = context;
        }

        

        // PUT: api/Investments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvestment(int id, Investment investment)
        {
            if (id != investment.Id)
            {
                return BadRequest();
            }

            _context.Entry(investment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvestmentExists(id))
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

        // POST: api/Investments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Investment>> PostInvestment(Investment investment)
        {
            _context.Investments.Add(investment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInvestment", new { id = investment.Id }, investment);
        }

        // DELETE: api/Investments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvestment(int id)
        {
            var investment = await _context.Investments.FindAsync(id);
            if (investment == null)
            {
                return NotFound();
            }

            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvestmentExists(int id)
        {
            return _context.Investments.Any(e => e.Id == id);
        }
    }
}
