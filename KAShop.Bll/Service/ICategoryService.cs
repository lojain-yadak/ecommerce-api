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

        Task<List<CategoryResponse>> GetAllCategoriesForAdmin();
        Task<List<CategoryUserResponse>> GetAllCategoriesForUser(string lang = "en");
        Task<CategoryResponse> CreateCategory(CategoryRequest request);
        Task<BaseResponse> DeleteCategoryAsync(int id);
        Task<BaseResponse> UpdateCategoryAsync(int id, CategoryRequest request);
        Task<BaseResponse> ToogleStatus(int id);
    }
}
