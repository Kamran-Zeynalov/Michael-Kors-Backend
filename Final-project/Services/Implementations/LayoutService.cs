using Final_project.Data;
using Final_project.Entities;
using Final_project.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Services.Implementations
{
    public class LayoutService : ILayoutService
    {
        private readonly MichaelDbContext _context;

        public LayoutService(MichaelDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> ShowCategories()
        {
            List<Category>? categories = await _context.Categories.Include(c => c.SubCategories)
                .ThenInclude(c => c.SubSubCategories).ToListAsync();

            return categories;
        }
    }
}
