using KAShop.Dal.DTOs.Request;
using KAShop.Dal.DTOs.Response;
using KAShop.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAShop.Bll.Service
{
    public interface ICategoryService
    {

        List<CategoryResponse> GetAllCategories();
        CategoryResponse CreateCategory(CategoryRequest request);
    }
}
