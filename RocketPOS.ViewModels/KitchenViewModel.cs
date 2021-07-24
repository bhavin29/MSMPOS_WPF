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

        public List<KitchenModel> GetKitchenStaus(List<int> KitchenOrder)
        {
            List<KitchenModel> kitchenModel = new List<KitchenModel>();
            List<KOTStatusList> kOTStatusLists = new List<KOTStatusList>();
            int KitchenOrderAll = 0;
            using (var db = new SqlConnection(LoginDetail.ConnectionString))
            {
                var query = " Select Id as TableId,TableName,PersonCapacity,Status,Position," +
                            " (select customerorder.id from customerorder where  tableid = t.id and orderstatus=1) as CustomerOrderId, " +
                            " (select Firstname from employee E inner join [User] U on U.Employeeid = E.Id   where U.Id = ( select customerorder.UserIdInserted from customerorder where  tableid = t.id and orderstatus=1)) as WaiterName, " + 
                            " (select customerorder.AllocatedPerson from customerorder where  tableid = t.id and orderstatus=1) as AllocatedPerson " +
                            " from tables t where t.outletid=" + LoginDetail.OutletId +
                            " Order by T.Position asc";
                kitchenModel = db.Query<KitchenModel>(query).ToList();

                foreach (var item in kitchenModel)
                {
                    KitchenOrderAll = 0;
                    for (var i = 0; i < KitchenOrder.Count; i++)
                    {
                        if (KitchenOrder[i] == item.TableId)
                        {
                            KitchenOrderAll = 1;
                        }
                    }

                    query = "Select CO.Id AS CustomerOrderId,Co.CustomerOrderNo, COKot.KOTNumber, COKot.Id AS KOTId, COKotItem.Id AS KOTItemId, FM.FoodMenuName, COKotItem.FoodMenuQty, " +
                              " Case When COKotItem.KOTStatus = 1 Then 'Pending' When COKotItem.KOTStatus = 2 Then 'Cooking' When COKotItem.KOTStatus = 3 Then 'Ready' " +
                              " When COKotItem.KOTStatus = 4 Then 'Served' When COKotItem.KOTStatus = 5 Then 'Completed' Else 'None' End As KOTStatus " +
                              " From CustomerOrder CO " +
                              " Inner Join CustomerOrderKOT COKot On COKot.CustomerOrderId = CO.Id " +
                              " Inner Join CustomerOrderKOTItem COKotItem On COKotItem.CustomerOrderKOTId = COKot.Id " +
                              " Inner Join FoodMenu FM On FM.Id = COKotItem.FoodMenuId " +
                              " Where " + //COKotItem.KOTStatus <> 5 AND " +
                              " CO.OrderStatus=1 And CO.OutletId = " + LoginDetail.OutletId + " And CO.Id =" + item.CustomerOrderId +
                              " And isnull(FM.foodmenutype,0) <> 1 ";

                    if (KitchenOrderAll == 0)
                    {
                        query = query + " and(CompletedDateTime >= dateadd(minute, -1, getdate()) or CompletedDateTime is null) ";
                    }

                    query = query + " Order by Co.CustomerOrderNo,COKot.KOTNumber";

                    item.kotStatusList = db.Query<KOTStatusList>(query).ToList();
                }
            }
            return kitchenModel;
        }

        public void ChangeKOTStatus(string KOTItemId, string KOTId, int status)
        {
            int kotStatus;
            using (var connection = new SqlConnection(LoginDetail.ConnectionString))
            {

                var kotItemStatusUpdate = "Update CustomerOrderKOTItem Set KOTStatus=" + status + ",CompletedDateTime=null Where Id=" + KOTItemId;
                if (status == 5)
                {
                    kotItemStatusUpdate = "Update CustomerOrderKOTItem Set KOTStatus=" + status + ",CompletedDateTime=GetDate() Where Id=" + KOTItemId;
                }

                connection.Query<bool>(kotItemStatusUpdate).FirstOrDefault();

                var kotStatusCount = "Select Count(*) From CustomerOrderKOTItem Where CustomerOrderKOTId= (Select top 1 CustomerOrderKOTId from CustomerOrderKotItem where Id =" + KOTItemId + ") And KOTStatus in (1,2,3,4)";
                kotStatus = connection.Query<int>(kotStatusCount).FirstOrDefault();

                if (kotStatus == 0)
                {
                    var kotStatusUpdate = "Update CustomerOrderKOT Set KOTStatus=3 Where Id= (Select top 1 CustomerOrderKOTId from CustomerOrderKotItem where Id =" + KOTItemId + ")";
                    connection.Query<bool>(kotStatusUpdate).FirstOrDefault();
                }
            }
        }

        public void ChangeAllKOTItemStatus(string KOTId, int status)
        {
            using (var connection = new SqlConnection(LoginDetail.ConnectionString))
            {
                var kotItemStatusUpdate = "Update CustomerOrderKOTItem Set KOTStatus=" + status + ",CompletedDateTime=null Where CustomerOrderKOTId in ( Select  Id from CustomerOrderKOT where CustomerOrderId= " + KOTId + ")";
                if (status == 5)
                {
                    kotItemStatusUpdate = "Update CustomerOrderKOTItem Set KOTStatus=" + status + ",CompletedDateTime=GetDate()  Where CustomerOrderKOTId in ( Select  Id from CustomerOrderKOT where CustomerOrderId= " + KOTId + ")";
                }

                connection.Query<bool>(kotItemStatusUpdate).FirstOrDefault();

                if (status == 5)
                {
                    var kotStatusUpdate = "Update CustomerOrderKOT Set KOTStatus=5 Where CustomerOrderId=" + KOTId;
                    connection.Query<bool>(kotStatusUpdate).FirstOrDefault();
                }
            }
        }

        public KOTCustomerOrderDetail GetCustomerOrderKOT(string orderId)
        {
            KOTCustomerOrderDetail kOTCustomerOrderDetail = new KOTCustomerOrderDetail();
            using (var db = new SqlConnection(LoginDetail.ConnectionString))
            {
                var query = "Select CO.Id, CO.CustomerOrderNo,CO.OrderDate,C.CustomerName,(E.FirstName+' '+E.LastName) As WaiterName,T.TableName, " +
                          " Case When CO.OrderType = 1 Then 'DineIN' When  CO.OrderType = 2 Then 'TakeAway' When  CO.OrderType = 3 Then 'Delivery' Else 'All' End As OrderType " +
                          " from CustomerOrder CO " +
                          " Inner Join Customer C ON C.Id = CO.CustomerId " +
                          " left Join Employee E On E.Id = CO.WaiterEmployeeId " +
                          " Left Join [Tables] T On T.Id = CO.TableId  " +
                          " Where CO.Id=" + orderId;
                kOTCustomerOrderDetail = db.Query<KOTCustomerOrderDetail>(query).FirstOrDefault();
            }
            return kOTCustomerOrderDetail;
        }

        public List<KOTHeaderDetail> GetKOTHeaderDetail(string orderId)
        {
            List<KOTHeaderDetail> kotHeaderDetail = new List<KOTHeaderDetail>();
            using (var db = new SqlConnection(LoginDetail.ConnectionString))
            {
                var query = "Select Id,KOTNumber,convert(char(5), KOTDateTime, 108) as KOTDateTime, " +
                          " Case When KOTStatus = 1 Then 'Pending' When  KOTStatus = 2 Then 'Cooking' When  KOTStatus = 3 Then 'Ready' When  KOTStatus = 4 Then 'Served' Else 'Completed' End As KOTStatus " +
                          " from CustomerOrderKOT where CustomerOrderId= " + orderId;
                kotHeaderDetail = db.Query<KOTHeaderDetail>(query).ToList();
            }
            return kotHeaderDetail;
        }

        public List<KOTItemDetail> GetKOTItemDetail(string kotId)
        {
            List<KOTItemDetail> kotItemDetail = new List<KOTItemDetail>();
            using (var db = new SqlConnection(LoginDetail.ConnectionString))
            {
                var query = "Select COKOTItem.Id AS KOTItemId,COKOT.KOTNumber,COKOTItem.KOTDateTime,Case When COKOTItem.KOTStatus = 1 Then 'Pending' When  COKOTItem.KOTStatus = 2 Then 'Cooking' When  COKOTItem.KOTStatus = 3 Then 'Ready' When  COKOTItem.KOTStatus = 4 Then 'Served' Else 'Completed' End As KOTItemStatus, " +
                          " FM.FoodMenuName,COKOTItem.FoodMenuQty from CustomerOrderKOT COKOT " +
                          " Inner Join CustomerOrderKOTItem COKOTItem ON COKOT.Id = COKOTItem.CustomerOrderKOTId "  +
                          " Inner Join FoodMenu FM On FM.Id = COKOTItem.FoodMenuId " +
                          " where COKOTItem.CustomerOrderKOTId= " + kotId;
                kotItemDetail = db.Query<KOTItemDetail>(query).ToList();
            }
            return kotItemDetail;
        }
    }
}
