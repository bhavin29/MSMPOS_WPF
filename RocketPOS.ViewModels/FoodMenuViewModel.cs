using RocketPOS.Model;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using RocketPOS.Core.Configuration;
using RocketPOS.Core.Constants;
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
                foodMenus = db.Query<FoodMenu>("SELECT FMC.Id,FM.Id AS FoodMenuId,FMC.IsFavourite, FMC.FoodMenuCategoryName As FoodCategory,FM.FoodCategoryId,FM.FoodMenuName As SmallName,FM.FoodMenuCode,FM.SmallThumb,FMR.SalesPrice,ISNULL(FMR.FoodVat,0) AS FoodVat ,ISNULL(FMR.Foodcess,0) AS Foodcess,ISNULL(FM.IsPriceChange,0) AS IsPriceChange,ISNULL(T.TaxPercentage,0) As TaxPercentage,Case When ISNULL(T.TaxPercentage,0)>0 Then 1 Else 0 End AS IsVatable FROM [dbo].[FoodMenuCategory] FMC " +
                                                " Inner Join[dbo].[FoodMenu] FM ON FMC.Id = FM.FoodCategoryId " +
                                                " INNER JOIN  FoodMenuRate FMR ON FM.Id = FMR.FoodMenuId " +
                                                " Left Join Tax T On T.Id=FMR.FoodVatTaxId Where FMR.OutletId =" + outLetId + 
                                                " And FMR.IsActive=1 And FMC.IsActive=1 AND FMC.ISDeleted=0 AND FoodMenuType!=4 AND FM.ISDeleted=0 order by FMC.position,FM.position").ToList();

                foodMenuModel.FoodList = foodMenus.GroupBy(menuCat => new { menuCat.Id, menuCat.FoodCategory, menuCat.IsFavourite }, (menuCategory, mainElements) => new FoodList
                {
                    Id = menuCategory.Id,
                    FoodCategory = menuCategory.FoodCategory,
                    IsFavourite = menuCategory.IsFavourite,
                    SubCategory = mainElements.GroupBy(subCat => new { subCat.FoodMenuId, subCat.FoodCategoryId, subCat.SmallName, subCat.SalesPrice, subCat.SmallThumb, subCat.FoodVat, subCat.Foodcess , subCat.TaxPercentage, subCat.IsVatable, subCat.IsPriceChange },
                         (subCategory, subElements) => new SubCategory
                         {
                             FoodMenuId = subCategory.FoodMenuId,
                             FoodCategoryId = subCategory.FoodCategoryId,
                             SmallName = subCategory.SmallName,
                             SalesPrice = subCategory.SalesPrice,
                             SmallThumb = subCategory.SmallThumb,
                             FoodVat = subCategory.FoodVat,
                             Foodcess = subCategory.Foodcess,
                             TaxPercentage=subCategory.TaxPercentage,
                             IsVatable=subCategory.IsVatable,
                             IsPriceChange = subCategory.IsPriceChange
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
                var query = " SELECT FMC.Id,FM.Id AS FoodMenuId,FMC.IsFavourite, FMC.FoodMenuCategoryName As FoodCategory,FM.FoodCategoryId," + 
                            " FM.FoodMenuName As SmallName,FM.FoodMenuCode,FM.SmallThumb,FMR.SalesPrice,ISNULL(FMR.FoodVat,0) AS FoodVat , " + 
                            " ISNULL(FMR.Foodcess,0) AS Foodcess,ISNULL(FMR.FoodVat,0) AS FoodVat ,ISNULL(FMR.Foodcess,0) AS Foodcess, " +
                            " ISNULL(FM.IsPriceChange,0) AS IsPriceChange,ISNULL(T.TaxPercentage,0) As TaxPercentage, FMR.OpeningStock,FMR.StockQty," + 
                            " Case When ISNULL(T.TaxPercentage,0)>0 Then 1 Else 0 End AS IsVatable FROM [dbo].[FoodMenuCategory] FMC " +
                            " Inner Join[dbo].[FoodMenu] FM " +
                            " INNER JOIN  FoodMenuRate FMR ON FM.Id = FMR.FoodMenuId " +
                            " ON FMC.Id = FM.FoodCategoryId  Left Join Tax T On T.Id=FMR.FoodVatTaxId  Where FMR.OutletId=" + outLetId + " And FMR.IsActive=1 AND FoodMenuType!=4 And FMC.IsActive=1";

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
        public bool ChagePrice(int foodMenuId, decimal newPrice)
        {
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "Update FoodMenuRate Set SalesPrice=@NewPrice Where FoodMenuId=@FoodMenuId and outletid=" + LoginDetail.OutletId + ";";
                //return connection.Query<int>(query).FirstOrDefault();
                var count = connection.Execute(query, new { NewPrice = newPrice, FoodMenuId = foodMenuId });
                return count > 0;
            }
        }
    }
}
