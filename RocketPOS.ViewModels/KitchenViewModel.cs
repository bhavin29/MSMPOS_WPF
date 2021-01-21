using Dapper;
using RocketPOS.Core.Configuration;
using RocketPOS.Core.Constants;
using RocketPOS.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RocketPOS.ViewModels
{
    public class KitchenViewModel
    {
        AppSettings appSettings = new AppSettings();

        public KitchenModel GetKitchenStaus()
        {
            KitchenModel kitchenModel = new KitchenModel();
            List<KitchenStatusDetail> kitchenStatusDetail = new List<KitchenStatusDetail>();
            kitchenModel.kotStatusList = new List<KOTStatusList>();

            using (var db = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "Select CO.Id AS OrderId,Co.CustomerOrderNo, CO.TableId, T.TableName, COKot.KOTNumber, COKot.Id AS KOTId, COKotItem.Id AS KOTItemId, FM.FoodMenuName, COKotItem.FoodMenuQty, " +
                           " Case When COKotItem.KOTStatus = 1 Then 'Pending' When COKotItem.KOTStatus = 2 Then 'Cooking' When COKotItem.KOTStatus = 3 Then 'Completed' Else 'None' End As KOTStatus " + 
                           " From CustomerOrder CO " +
                           " Inner Join CustomerOrderKOT COKot On COKot.CustomerOrderId = CO.Id " +
                           " Inner Join CustomerOrderKOTItem COKotItem On COKotItem.CustomerOrderKOTId = COKot.Id " +
                           " Inner Join FoodMenu FM On FM.Id = COKotItem.FoodMenuId " +
                           " Left Join[Tables] T On T.Id = CO.TableId Where COKot.KOTStatus = 1 AND CO.OrderStatus=1 And CO.OutletId = " + LoginDetail.OutletId +
                           " Order by Co.CustomerOrderNo,COKot.KOTNumber";

                kitchenStatusDetail = db.Query<KitchenStatusDetail>(query).ToList();

                kitchenModel.kotStatusList = kitchenStatusDetail.GroupBy(kot => new { kot.OrderId, kot.CustomerOrderNo, kot.TableId, kot.TableName, kot.KOTNumber, kot.KOTId }, (kots, mainElements) => new KOTStatusList
                {
                    OrderId = kots.OrderId,
                    CustomerOrderNo = kots.CustomerOrderNo,
                    TableId = kots.TableId,
                    TableName = kots.TableName,
                    KOTNumber = kots.KOTNumber,
                    KOTId = kots.KOTId,
                    kOTItems = mainElements.GroupBy(kotItem => new { kotItem.KOTItemId, kotItem.FoodMenuName, kotItem.FoodMenuQty, kotItem.KOTStatus },
                           (kotItems, subElements) => new KOTItem
                           {
                               KOTItemId = kotItems.KOTItemId,
                               FoodMenuName = kotItems.FoodMenuName,
                               FoodMenuQty = kotItems.FoodMenuQty,
                               KOTStatus = kotItems.KOTStatus
                           }).ToList(),
                }).ToList();
            }
            return kitchenModel;
        }

        public void ChangeKOTStatus(string KOTItemId, string KOTId, int status)
        {
            int kotStatus;
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var kotItemStatusUpdate = "Update CustomerOrderKOTItem Set KOTStatus=" + status + " Where Id=" + KOTItemId;
                connection.Query<bool>(kotItemStatusUpdate).FirstOrDefault();

                var kotStatusCount = "Select Count(*) From CustomerOrderKOTItem Where CustomerOrderKOTId=" + KOTId + " And KOTStatus in (1,2)";
                kotStatus = connection.Query<int>(kotStatusCount).FirstOrDefault();

                if (kotStatus == 0)
                {
                    var kotStatusUpdate= "Update CustomerOrderKOT Set KOTStatus=3 Where Id=" + KOTId;
                    connection.Query<bool>(kotStatusUpdate).FirstOrDefault();
                }
            }
        }

        public void ChangeAllKOTItemStatus(string KOTId, int status)
        {
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var kotItemStatusUpdate = "Update CustomerOrderKOTItem Set KOTStatus=" + status + " Where CustomerOrderKOTId=" + KOTId;
                connection.Query<bool>(kotItemStatusUpdate).FirstOrDefault();

                if (status == 3)
                {
                    var kotStatusUpdate = "Update CustomerOrderKOT Set KOTStatus=3 Where Id=" + KOTId;
                    connection.Query<bool>(kotStatusUpdate).FirstOrDefault();
                }
            }
        }
    }
}
