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
using SMSHelper.Common.Models;
using System.Runtime;
using Microsoft.VisualBasic;

namespace SMSHelper.Helper.Implementation
{
    internal class TaskHelper : ITaskHepler
    {
        IConfiguration _configuration;
        private readonly IMongoClient _mongoClient;
        IMongoCollection<Customer>? _customerCollection;
        IMongoCollection<Settings>? _settingsCollection;

        private readonly ILog _logger = LogManager.GetLogger(typeof(TaskHelper));
        List<Customer> _customersFromMongo = new();
        Settings _settingsFromMongo = new();
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

                    var Database = _mongoClient.GetDatabase(mongoDbSettings.DatabaseName);
                    _customerCollection = Database.GetCollection<Customer>(mongoDbSettings.CustomerCollectionName);
                    _settingsCollection = Database.GetCollection<Settings>(mongoDbSettings.SettingsCollectionName);
                    _logger.Info("Connection Has been established with mongo");

                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                #endregion

                #region Get Customers from MongoDb
                _customersFromMongo = _customerCollection.Find(Builders<Customer>.Filter.Empty).ToListAsync().Result;
                _settingsFromMongo = _settingsCollection.Find(Builders<Settings>.Filter.Empty).FirstAsync().Result;
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

                        if (expirationDate?.Subtract(now).Days <= 2 && (lastNotification == null || expirationDate != lastNotification))
                        {

                            _expiredCustomers.Add(customer);
                            continue;
                        }
                        if (lastSeen?.Subtract(now).Days == -15)
                        {
                            _inActiveCustomers15.Add(customer);
                            continue;
                        }
                        if (lastSeen?.Subtract(now).Days == -7)
                        {
                            _inActiveCustomers7.Add(customer);
                            continue;
                        }


                    }
                #endregion


                _logger.Info("_expiredCustomers Count= " + _expiredCustomers.Count);
                _logger.Info("_inActiveCustomers7 Count= " + _inActiveCustomers7.Count);
                _logger.Info("_inActiveCustomers15 Count= " + _inActiveCustomers15.Count);

                //var successNumbers_Expired = SendSMS();

                //if (successNumbers_Expired != null)
                //    updateCustomersLastNotification(successNumbers_Expired);


                var credit = Task.Run(() => _htdHelper.GetCredit()).Result;

                if (credit != null)
                {
                    _settingsFromMongo.AvailableSMSBalance = (int)Double.Parse(credit);
                    _settingsCollection?.DeleteMany(_ => true);
                    _settingsCollection?.InsertOne(_settingsFromMongo);

                }


            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
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

        public List<string>? SendSMS()
        {


            try
            {

                MessagesContent messagesContent = _configuration.GetSection("MessagesContent").Get<MessagesContent>();

                List<string>? successNumbers_Expired;
                successNumbers_Expired = Task.Run(() => _htdHelper.Send(GetNumbers(_expiredCustomers.Select(x => x.Phone.Trim()).ToList()), string.IsNullOrEmpty( _settingsFromMongo.SendToExpiredTxt)? messagesContent.Expired: _settingsFromMongo.SendToExpiredTxt)).Result;

                if (_settingsFromMongo.SendToInactive7 == true)
                {
                    var successNumbers_7 = Task.Run(() => _htdHelper.Send(GetNumbers(_inActiveCustomers7.Select(x => x.Phone.Trim()).ToList()), string.IsNullOrEmpty(_settingsFromMongo.SendToInactive7Txt) ? messagesContent.Inactive7 : _settingsFromMongo.SendToInactive7Txt)).Result;
                }
                if (_settingsFromMongo.SendToInactive15 == true)
                {
                    var successNumbers_15 = Task.Run(() => _htdHelper.Send(GetNumbers(_inActiveCustomers15.Select(x => x.Phone.Trim()).ToList()), string.IsNullOrEmpty(_settingsFromMongo.SendToInactive15Txt) ? messagesContent.Inactive15 : _settingsFromMongo.SendToInactive15Txt)).Result;
                }
                return successNumbers_Expired;

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return null;
            }
        }

        private void updateCustomersLastNotification(List<string>? successNumbers)
        {

            for (int i = 0; i < _customersFromMongo.Count; i++)
            {
                if (successNumbers!.Contains(_customersFromMongo[i].Phone))
                {
                    _customersFromMongo[i].LastNotification = _customersFromMongo[i].EndDate;

                }

            }

            _customerCollection?.DeleteMany(_ => true);
            _customerCollection?.InsertMany(_customersFromMongo);
        }
    }
}
