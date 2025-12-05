using KAShop.Dal.Data;
using KAShop.Dal.Models;
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

        public Category Create(Category request)
        {
            _context.Categories.Add(request);
            _context.SaveChanges();
            return request;
        }

        public List<Category> GetAll()
        {
            return _context.Categories.Include(c=>c.Translations).ToList();
        }
    }
}
