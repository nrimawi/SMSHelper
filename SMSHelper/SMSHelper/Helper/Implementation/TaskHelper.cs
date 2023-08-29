using SMSHelper.Common.DataModels;
using SMSHelper.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using log4net;
using static System.Net.Mime.MediaTypeNames;

namespace SMSHelper.Helper.Implementation
{
    internal class TaskHelper : ITaskHepler
    {
        IConfiguration _configuration;
        private readonly IMongoClient _mongoClient;
        IMongoCollection<Customer>? _customerCollection;
        private readonly ILog _logger = LogManager.GetLogger(typeof(TaskHelper));
        List<Customer> _customersFromMongo = new();
        IHTDHelper _htdHelper;
        List<Customer> _expiredCustomers = new();
        List<Customer> _inActiveCustomers7 = new();
        List<Customer> _inActiveCustomers15 = new();



        public TaskHelper(IMongoClient mongoClient, IConfiguration configuration, IHTDHelper htdHelper)
        {
            _mongoClient = mongoClient;
            _configuration = configuration;
            _htdHelper = htdHelper;

        }

        public void Execute(bool forceMode)
        {
            try
            {


                #region Establish connection to mongo
                try
                {
                    var mongoDbSettings = _configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
                    var x = _mongoClient.ListDatabaseNames().ToList();

                    var Database = _mongoClient.GetDatabase(mongoDbSettings.DatabaseName);
                    _customerCollection = Database.GetCollection<Customer>(mongoDbSettings.CustomerCollectionName);
                    _logger.Info("Connection Has been established with mongo");

                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                #endregion

                #region Get Customers from MongoDb
                var filter = Builders<Customer>.Filter.Empty;
                _customersFromMongo = _customerCollection.Find(filter).ToListAsync().Result;
                _logger.Info("Customers form mongo has been retrieved Count= " + _customersFromMongo.Count);

                #endregion

                #region SMS Sending Conditions
                if (_customersFromMongo != null)
                    foreach (Customer customer in _customersFromMongo)
                    {
                        DateTime? expirationDate = !String.IsNullOrEmpty(customer.EndDate.Trim()) ? DateTime.Parse(customer.EndDate) : null;
                        DateTime? lastNotification = !String.IsNullOrEmpty(customer.LastNotification.Trim()) ? DateTime.Parse(customer.LastNotification) : null;
                        DateTime? lastSeen = !String.IsNullOrEmpty(customer.LastSeen.Trim()) ? DateTime.Parse(customer.LastSeen) : null;

                        DateTime now = DateTime.Now;

                        if (expirationDate?.Subtract(now).Days <= 2 && (lastNotification==null || lastNotification?.Subtract(now).Days>=2))
                        {
                            
                            _expiredCustomers.Add(customer);
                            continue;
                        }
                        if (lastSeen?.Subtract(now).Days == 15 )
                        {
                            _inActiveCustomers15.Add(customer);
                            continue;
                        }
                        if (lastSeen?.Subtract(now).Days == 7)
                        {
                            _inActiveCustomers7.Add(customer);
                            continue;
                        }


                    }
                #endregion


                _logger.Info("_expiredCustomers Count= " + _expiredCustomers.Count);
                _logger.Info("_inActiveCustomers7 Count= " + _inActiveCustomers7.Count);
                _logger.Info("_inActiveCustomers15 Count= " + _inActiveCustomers15.Count);

                SendSMS();

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        public void SendSMS()
        {


            try
            {

                var SuccessNumbers1 = Task.Run(() => _htdHelper.Send(GetNumbers(_expiredCustomers.Select(x => x.Phone.Trim()).ToList()), "HELLO1")).Result;
                var SuccessNumbers2 = Task.Run(() => _htdHelper.Send(GetNumbers(_inActiveCustomers7.Select(x => x.Phone.Trim()).ToList()), "HELLO1")).Result;
                var SuccessNumbers3 = Task.Run(() => _htdHelper.Send(GetNumbers(_inActiveCustomers15.Select(x => x.Phone.Trim()).ToList()), "HELLO1")).Result;

            }
            catch
            {

            }
        }

        public string GetNumbers(List<string> numbers)
        {

            string res = "";

            foreach (string number in numbers)
            {
                res += "97" + number + ",";
            }
            res = res.Substring(0, res.Length - 1);
            return res;
        }
    }
}
