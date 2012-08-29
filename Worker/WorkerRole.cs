using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using TestAnObject;

namespace Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("Worker entry point called", "Information");
            var eventConsumer = new AnObjectEventConsumer();

            while (true)
            {
                Thread.Sleep(1000);
                eventConsumer.Treat();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }

    public class AnObjectEventConsumer
    {
        private readonly CloudQueue _queue;
        private readonly JavaScriptSerializer _javaScriptSerializer = new JavaScriptSerializer();

        public AnObjectEventConsumer()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("Storage"));
            var queueClient = storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("anobjectqueue");
            _queue.CreateIfNotExist();
        }

        public void Treat()
        {
            foreach (var message in _queue.GetMessages(20, TimeSpan.FromMinutes(5)))
            {
                ProcessMessage(message);
                _queue.DeleteMessage(message);
            }
        }

        private void ProcessMessage(CloudQueueMessage message)
        {
            var anObjectEvent = _javaScriptSerializer.Deserialize<AnObjectEvent>(message.AsString);
            Trace.WriteLine(string.Format("Event {0} : {1}", anObjectEvent.Type, _javaScriptSerializer.Serialize(anObjectEvent.AnObject)));
        }
    }
}
