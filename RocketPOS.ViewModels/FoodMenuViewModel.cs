using RocketPOS.Model;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using RocketPOS.Core.Configuration;

namespace RocketPOS.ViewModels
{
    public class FoodMenuViewModel
    {
        AppSettings AppSettings = new AppSettings();
        public FoodMenuModel GetFoodMenu()
        {
            FoodMenuModel foodMenuModel = new FoodMenuModel();
            List<FoodMenu> foodMenus = new List<FoodMenu>();
            foodMenuModel.FoodList = new List<FoodList>();

            using (var db = new SqlConnection(AppSettings.GetConnectionString()))
            {
                foodMenus = db.Query<FoodMenu>("SELECT FMC.Id,FM.Id AS FoodMenuId, FMC.FoodMenuCategoryName As FoodCategory,FM.FoodCategoryId,FM.FoodMenuName As SmallName,FM.FoodMenuCode,FM.SmallThumb,FM.SalesPrice FROM [dbo].[FoodMenuCategory] FMC " +
                                                                "Inner Join[dbo].[FoodMenu] FM " +
                                                                "ON FMC.Id = FM.FoodCategoryId").ToList();

                foodMenuModel.FoodList = foodMenus.GroupBy(menuCat => new { menuCat.Id, menuCat.FoodCategory }, (menuCategory, mainElements) => new FoodList
                {
                    Id = menuCategory.Id,
                    FoodCategory = menuCategory.FoodCategory,
                    SubCategory = mainElements.GroupBy(subCat => new { subCat.FoodMenuId, subCat.FoodCategoryId, subCat.SmallName, subCat.SalesPrice, subCat.SmallThumb },
                         (subCategory, subElements) => new SubCategory
                         {
                             FoodMenuId = subCategory.FoodMenuId,
                             FoodCategoryId = subCategory.FoodCategoryId,
                             SmallName = subCategory.SmallName,
                             SalesPrice = subCategory.SalesPrice,
                             SmallThumb = subCategory.SmallThumb
                         }).ToList(),
                }).ToList();
            }
            return foodMenuModel;
        }
    }
}
