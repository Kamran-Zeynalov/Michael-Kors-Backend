using Final_project.Entities;

namespace Final_project.Services.Interfaces
{
    public interface ILayoutService
    {
        public Task<List<Category>> ShowCategories();
    }
}
