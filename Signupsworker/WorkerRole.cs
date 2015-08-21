using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace Signupsworker
{
    public class WorkerRole : RoleEntryPoint
    {
        private string ConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
        private string qname = "signups";
        private string tableConnectionString = CloudConfigurationManager.GetSetting("TableStorageConnection");

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("Signupsworker is running");

            while (true)
            {
                QueueClient qc = QueueClient.CreateFromConnectionString(ConnectionString, qname);
                BrokeredMessage msg = qc.Receive();

                if (msg != null)
                {
                    try
                    {
                        Trace.WriteLine("New Signup processed: " + msg.Properties["email"]);
                        msg.Complete();

                        // Log to table storage
                        SaveToStorage(msg.Properties["email"].ToString());
                    }
                    catch (Exception)
                    {
                        // Indicate a problem, unlock the queue
                        msg.Abandon();
                    }
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("Signupsworker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("Signupsworker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("Signupsworker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }

        private void SaveToStorage(string email)
        {
            string tableName = "signups";
            CloudStorageAccount account = CloudStorageAccount.Parse(tableConnectionString);

            //Client for Table storage
            CloudTableClient tableStorage = account.CreateCloudTableClient();
            var cloudTable = tableStorage.GetTableReference(tableName);
            cloudTable.CreateIfNotExists();

            //Create person entity and set email
            PersonEntity person = new PersonEntity();
            person.Email = email;
            person.PartitionKey = "signups";
            person.RowKey = email;

            //Save new person to the table
            cloudTable.Execute(TableOperation.Insert(person));
        }
    }
}
