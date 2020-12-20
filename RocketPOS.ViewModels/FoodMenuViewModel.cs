using RocketPOS.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Dapper;
using System.Linq;
using System.Configuration;

namespace RocketPOS.ViewModels
{
    public class FoodMenuViewModel
    {
        public FoodMenuModel GetFoodMenu()
        {
            FoodMenuModel foodMenuModel = new FoodMenuModel();
            List<FoodMenu> foodMenus = new List<FoodMenu>();
            FoodList foodList = new FoodList();
            foodMenuModel.FoodList = new List<FoodList>();
            using (var db = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]))
            {
                foodMenus = db.Query<FoodMenu>("SELECT FMC.Id,FM.Id AS FoodMenuId, FMC.FoodMenuCategoryName As FoodCategory,FM.FoodCategoryId,FM.FoodMenuName As SmallName,FM.FoodMenuCode,FM.SmallThumb,FM.SalesPrice FROM [dbo].[FoodMenuCategory] FMC " +
                                                                "Inner Join[dbo].[FoodMenu] FM "+
                                                                "ON FMC.Id = FM.FoodCategoryId").ToList();
                if (foodMenus!=null && foodMenus.Count>0)
                {
                    foreach (var foodItems in foodMenus.GroupBy(foodMenuId=> foodMenuId.Id))
                    {
                        foodList = new FoodList();
                        foodList.Id = foodItems.FirstOrDefault().Id;
                        foodList.FoodCategory = foodItems.FirstOrDefault().FoodCategory;
                        foodList.SubCategory = new List<SubCategory>();
                        foreach (var foodSubCategory in foodItems.Where(foodMenuId => foodMenuId.Id == foodItems.FirstOrDefault().Id).ToList())
                        {
                            var subCategoryDetail = new SubCategory();
                            subCategoryDetail.FoodMenuId = foodSubCategory.FoodMenuId;
                            subCategoryDetail.FoodCategoryId = foodSubCategory.FoodCategoryId;
                            subCategoryDetail.SmallName = foodSubCategory.SmallName;
                            subCategoryDetail.SalesPrice = foodSubCategory.SalesPrice;
                            subCategoryDetail.SmallThumb = foodSubCategory.SmallThumb;
                            foodList.SubCategory.Add(subCategoryDetail);
                        }
                        foodMenuModel.FoodList.Add(foodList);
                    }
                }
                else
                {
                    foodMenuModel.FoodList = new List<FoodList>();
                }
            }
            return foodMenuModel;
        }
    }
}
