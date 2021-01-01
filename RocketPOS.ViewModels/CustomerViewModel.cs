using System.Collections.Generic;
using RocketPOS.Core.Configuration;
using RocketPOS.Model;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace RocketPOS.ViewModels
{
    public class CustomerViewModel
    {
        AppSettings appSettings = new AppSettings();

        public List<CustomerModel> GetCustomers()
        {
            List<CustomerModel> customers = new List<CustomerModel>();
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "SELECT Id,CustomerName,CustomerEmail,CustomerAddress1,CustomerPhone FROM Customer Where IsActive=1 Order By Id Desc";
                customers = connection.Query<CustomerModel>(query).ToList();
                return customers;
            }
        }

        public CustomerModel GetCustomerById(int id)
        {
            CustomerModel customer = new CustomerModel();
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                var query = "SELECT Id,CustomerName,CustomerEmail,CustomerAddress1,CustomerPhone FROM Customer Where Id=" + id;
                customer = connection.Query<CustomerModel>(query).FirstOrDefault();
                return customer;
            }
        }

        public int InsertUpdateCustomer(CustomerModel customerModel)
        {
            int insertedId = 0;
            using (var connection = new SqlConnection(appSettings.GetConnectionString()))
            {
                string query = string.Empty;
                if (string.IsNullOrEmpty(customerModel.Id.ToString()) || customerModel.Id == 0)
                {
                    query = @"Insert Into Customer (CustomerName,CustomerEmail,CustomerAddress1,CustomerPhone,UserIdInserted,DateInserted,IsActive) Values(@CustomerName,@CustomerEmail,@CustomerAddress1,@CustomerPhone,@UserIdInserted,@DateInserted,@IsActive);" 
                             +"SELECT CAST(SCOPE_IDENTITY() as int)";
                }
                else
                {
                    query = @"Update Customer Set CustomerName=@CustomerName,CustomerEmail=@CustomerEmail,CustomerAddress1=@CustomerAddress1,CustomerPhone=@CustomerPhone,UserIdUpdated=@UserIdInserted,DateUpdated=@DateInserted Where Id=@Id;
                               SELECT CAST(@Id as int)";
                }

                insertedId = connection.Query<int>(query, new
                {
                    Id = customerModel.Id,
                    CustomerName = customerModel.CustomerName,
                    CustomerEmail = customerModel.CustomerEmail,
                    CustomerAddress1 = customerModel.CustomerAddress1,
                    CustomerPhone = customerModel.CustomerPhone,
                    UserIdInserted = customerModel.UserId,
                    DateInserted= System.DateTime.Now,
                    IsActive = true
                }).Single();
            }
            return insertedId;
        }
    }
}
