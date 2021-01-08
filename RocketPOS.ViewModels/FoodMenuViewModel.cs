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
        AppSettings appSettings = new AppSettings();
        public FoodMenuModel GetFoodMenu(int outLetId)
        {
            FoodMenuModel foodMenuModel = new FoodMenuModel();
            List<FoodMenu> foodMenus = new List<FoodMenu>();
            foodMenuModel.FoodList = new List<FoodList>();

            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                foodMenus = db.Query<FoodMenu>("SELECT FMC.Id,FM.Id AS FoodMenuId,FMC.IsFavourite, FMC.FoodMenuCategoryName As FoodCategory,FM.FoodCategoryId,SUBSTRING(FM.FoodMenuName,0,10) As SmallName,FM.FoodMenuCode,FM.SmallThumb,FM.SalesPrice,ISNULL(FM.FoodVat,0) AS FoodVat ,ISNULL(FM.Foodcess,0) AS Foodcess FROM [dbo].[FoodMenuCategory] FMC " +
                                                "Inner Join[dbo].[FoodMenu] FM " +
                                                "ON FMC.Id = FM.FoodCategoryId Where (',' + FM.OutletId + ',') LIKE '%," + outLetId + 
                                                ",%' And FM.IsActive=1 And FMC.IsActive=1 AND FMC.ISDeleted=0 AND FM.ISDeleted=0 order by FMC.position,FM.position").ToList();

                foodMenuModel.FoodList = foodMenus.GroupBy(menuCat => new { menuCat.Id, menuCat.FoodCategory, menuCat.IsFavourite }, (menuCategory, mainElements) => new FoodList
                {
                    Id = menuCategory.Id,
                    FoodCategory = menuCategory.FoodCategory,
                    IsFavourite = menuCategory.IsFavourite,
                    SubCategory = mainElements.GroupBy(subCat => new { subCat.FoodMenuId, subCat.FoodCategoryId, subCat.SmallName, subCat.SalesPrice, subCat.SmallThumb, subCat.FoodVat, subCat.Foodcess },
                         (subCategory, subElements) => new SubCategory
                         {
                             FoodMenuId = subCategory.FoodMenuId,
                             FoodCategoryId = subCategory.FoodCategoryId,
                             SmallName = subCategory.SmallName,
                             SalesPrice = subCategory.SalesPrice,
                             SmallThumb = subCategory.SmallThumb,
                             FoodVat = subCategory.FoodVat,
                             Foodcess = subCategory.Foodcess
                         }).ToList(),
                }).ToList();
            }
            return foodMenuModel;
        }
        public List<FoodMenu> GetFoodMenuPopUpList(int outLetId, string searchKey)
        {
            List<FoodMenu> foodMenus = new List<FoodMenu>();
            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "SELECT FMC.Id,FM.Id AS FoodMenuId,FMC.IsFavourite, FMC.FoodMenuCategoryName As FoodCategory,FM.FoodCategoryId,FM.FoodMenuName As SmallName,FM.FoodMenuCode,FM.SmallThumb,FM.SalesPrice,ISNULL(FM.FoodVat,0) AS FoodVat ,ISNULL(FM.Foodcess,0) AS Foodcess FROM [dbo].[FoodMenuCategory] FMC " +
                                                                 "Inner Join[dbo].[FoodMenu] FM " +
                                                                 "ON FMC.Id = FM.FoodCategoryId Where (',' + FM.OutletId + ',') LIKE '%," + outLetId + ",%' And FM.IsActive=1 And FMC.IsActive=1";

                if (!string.IsNullOrEmpty(searchKey))
                {
                    query += " AND (FM.FoodMenuName Like '%"+ searchKey+ "%' OR FMC.FoodMenuCategoryName Like '%" + searchKey + "%')";
                }
                query += " Order By FM.FoodMenuName";
                foodMenus = db.Query<FoodMenu>(query).ToList();
            }
            return foodMenus;
        }

        public bool UploadFoodImage(string fileName,int foodMenuId)
        {
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "Update FoodMenu Set SmallThumb=@FileName Where Id=@Id";
                //return connection.Query<int>(query).FirstOrDefault();
                var count = connection.Execute(query, new { FileName=fileName,Id= foodMenuId });
                return count > 0;
            }
        }
    }
}
