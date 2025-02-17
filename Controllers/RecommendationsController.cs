using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

[Route("api/recommendations")]
[ApiController]
public class RecommendationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public RecommendationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{patientId}")]
    public async Task<IActionResult> GetRecommendations(int patientId)
    {
        var recommendations = await _context.Recommendations
                            .Where(r => r.PatientId == patientId)
                            .ToListAsync();
        return Ok(recommendations);
    }
}
