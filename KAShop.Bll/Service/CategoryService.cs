using KAShop.Dal.DTOs.Request;
using KAShop.Dal.DTOs.Response;
using KAShop.Dal.Models;
using KAShop.Dal.Repository;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAShop.Bll.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<CategoryResponse> CreateCategory(CategoryRequest request )
        {
            var category = request.Adapt<Category>();
            
            await _categoryRepository.Create(category);
            return category.Adapt<CategoryResponse>();
            
        }

        public async  Task<List<CategoryResponse>> GetAllCategoriesForAdmin()
        {
            var categories = await _categoryRepository.GetAll();
           
            var response = categories.Adapt<List<CategoryResponse>>();

            return response;
        }
        public async Task<List<CategoryUserResponse>> GetAllCategoriesForUser(string lang = "en") {
            var categories = await _categoryRepository.GetAll();

            //foreach (var category in categories)
            //{
            //    category.Translations = category.Translations.Where(t => t.Language == lang).ToList();
            //}
            var response = categories.BuildAdapter().AddParameters("lang", lang).AdaptToType<List<CategoryUserResponse>>();
            //var response = categories.Select(c => new CategoryUserResponse
            //{
            //    Id=c.Id,
            //    Name=c.Translations.Where(t=>t.Language == lang).Select(t=>t.Name).FirstOrDefault()
            //}).ToList();
            return response;
        }
        public async Task<BaseResponse> UpdateCategoryAsync(int id,CategoryRequest request)
        {
            try
            {
                var category = await _categoryRepository.FindByIdAsync(id);
                if (category is null)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Category Not Found"
                    };
                }

                if (request.Translations != null)
                {
                    foreach (var translation in request.Translations) {
                        var existing = category.Translations.FirstOrDefault(t => t.Language == translation.Language);
                        if (existing is not null) { 
                        existing.Name = translation.Name;
                        }
                        else
                        {
                            return new BaseResponse
                            {
                                Success=true,
                                Message=$"Language{translation.Language} not supported"
                            };
                        }
                    }
                    
                 }
                await _categoryRepository.UpdateAsync(category);
                return new BaseResponse
                {
                    Success = true,
                    Message = "Category Updated Successfully"
                };
            }
            catch (Exception ex) { 
            return new BaseResponse
            {
             Success = false,
             Message="Can't Update Category",
             Errors= new List<string> { ex.Message }
            };
            }
        }
        public async Task<BaseResponse> DeleteCategoryAsync(int id)
        {
            try
            {
               var category = await _categoryRepository.FindByIdAsync(id);
                if (category is null)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message="Category Not Found"
                    };
                }
                await _categoryRepository.DeleteAsync(category);
                return new BaseResponse
                {
                    Success = true,
                    Message = "Category Deleted Successfully"
                };
            }
            catch (Exception ex) {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Can't Delete Category",
                    Errors = new List<string> { ex.Message }
                };
            }

        }

        public async Task<BaseResponse> ToogleStatus(int id)
        {
            try
            {
                var category = await _categoryRepository.FindByIdAsync(id);
                if (category is null)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Category Not Found"
                    };
                }
                category.Status = category.Status == Status.Active ? Status.InActive : Status.Active;
                await _categoryRepository.UpdateAsync(category);
                return new BaseResponse
                {
                    Success = true,
                    Message = "Category status changed successfully",
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Can't Update Category",
                    Errors = new List<string> { ex.Message }
                };
            }

        }
    }
}
