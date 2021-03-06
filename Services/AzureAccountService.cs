﻿using System.Threading.Tasks;
using LaoS.Models;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using LaoS.Interfaces;
using System;

namespace LaoS.Services
{
    public class AzureAccountService : IAccountService
    {
        private string _storageKey;
        private string _storageAccount;

        public AzureAccountService(IAppSettings appSettings)
        {
            this._storageAccount = appSettings.Get("storageAccount");
            this._storageKey = appSettings.Get("storageKey");
        }

        public async Task<bool> SaveAccountForTeam(Account settings)
        {
            StorageCredentials credentials = new StorageCredentials(_storageAccount, _storageKey);
            CloudStorageAccount account = new CloudStorageAccount(credentials, true);
            var client = account.CreateCloudTableClient();

            var tableRef = client.GetTableReference("laosSettings");
            await tableRef.CreateIfNotExistsAsync();

            TableOperation ops = TableOperation.InsertOrMerge(settings);
            await tableRef.ExecuteAsync(ops);
            return true;
        }

        public async Task<Account> GetContractFromTableStorage(string laosID)
        {
            StorageCredentials credentials = new StorageCredentials(_storageAccount, _storageKey);
            CloudStorageAccount account = new CloudStorageAccount(credentials, true);
            var client = account.CreateCloudTableClient();

            var tableRef = client.GetTableReference("laosSettings");
            await tableRef.CreateIfNotExistsAsync();
            try
            {
                TableOperation ops = TableOperation.Retrieve<Account>("DEV", laosID);
                var tableResult = await tableRef.ExecuteAsync(ops);
                if (tableResult.HttpStatusCode == 200)
                    return (Account)tableResult.Result;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Account> GetAccountForTeam(string account)
        {
            return await GetContractFromTableStorage(account);
        }
    }
}
