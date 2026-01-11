using KAShop.Dal.DTOs.Response;
using KAShop.Dal.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAShop.Bll.MapsterConfigurations
{
    public static class MapsterConfig
    {
        public static void MapsterConfRegister()
        {
            //TypeAdapterConfig<Category, CategoryResponse>.NewConfig().Map(dest => dest.Category_Id, source => source.Id);
            TypeAdapterConfig<Category, CategoryResponse>.NewConfig().Map(dest=>dest.CreatedBy,source=>source.User.UserName);
            TypeAdapterConfig<Category, CategoryUserResponse>.NewConfig()
                .Map(dest=>dest.Name,source=>source.Translations.
                Where(t => t.Language == MapContext.Current.Parameters["lang"].ToString())
                .Select(t => t.Name).FirstOrDefault());
        }
    }
}
