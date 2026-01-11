using KAShop.Dal.Data;
using KAShop.Dal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAShop.Dal.Repository
{
    public class CategoryRepository:ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category> Create(Category request)
        {
           await _context.AddAsync(request);
           await _context.SaveChangesAsync();
            return request;
        }

        public async Task<List<Category>> GetAll()
        {
            return await _context.Categories.Include(c=>c.Translations).Include(c=>c.User).ToListAsync();
        }

        public async Task<Category?> FindByIdAsync(int id)
        {
            return await _context.Categories.Include(c=>c.Translations)
                .FirstOrDefaultAsync(c=>c.Id == id);
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

        }

        public async Task<Category?> UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }
    }
}
