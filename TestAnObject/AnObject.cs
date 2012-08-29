using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace TestAnObject
{
    public class AnObject
    {
        public int Id { get; set; }
        public int Value { get; set; }
    }

    public class AnObjectService
    {
        private readonly AnObjectRepository List = new AnObjectRepository();
        private AnObjectEventPublisher _eventPublisher;

        public void Add(AnObject value)
        {
            List.Add(value);
            GetPublisher().PublishAdd(value);
        }

        private AnObjectEventPublisher GetPublisher()
        {
            return _eventPublisher ?? (_eventPublisher = new AnObjectEventPublisher());
        }

        public AnObject Change(int id, AnObject value)
        {
            var anObject = List.Find(id);
            anObject.Value = value.Value;
            GetPublisher().PublishChange(anObject);
            return anObject;
        }

        public void Delete(int id)
        {
            var anObject = List.Find(id);
            List.Remove(anObject);
            GetPublisher().PublishDelete(anObject);
        }
    }

    internal class AnObjectEventPublisher
    {
        private readonly CloudQueue _queue;

        internal AnObjectEventPublisher()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("Storage"));
            var queueClient = storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("anobjectqueue");
            _queue.CreateIfNotExist();
        }

        public void PublishAdd(AnObject value)
        {
            var anEvent = new AnObjectEvent
                {
                AnObject = value,
                Type = AnObjectEventType.Add
            };
            Send(anEvent);
        }


        public void PublishChange(AnObject anObject)
        {
            var anEvent = new AnObjectEvent
            {
                AnObject = anObject,
                Type = AnObjectEventType.Change
            };
            Send(anEvent);
        }

        public void PublishDelete(AnObject anObject)
        {
            var anEvent = new AnObjectEvent
            {
                AnObject = anObject,
                Type = AnObjectEventType.Delete
            };
            Send(anEvent);
        }

        private void Send(AnObjectEvent anEvent)
        {
            var serializedEvent = new JavaScriptSerializer().Serialize(anEvent);
            var message = new CloudQueueMessage(serializedEvent);
            _queue.AddMessage(message);
        }

    }

    public class AnObjectEvent
    {
        public AnObjectEventType Type { get; set; }
        public AnObject AnObject { get; set; }
    }

    public enum AnObjectEventType
    {
        Add, Change, Delete
    }

    public class AnObjectRepository
    {
        private static readonly IList<AnObject> List = new List<AnObject>();

        private static int _inc;

        public AnObject Find(int id)
        {
            return List.FirstOrDefault(o => o.Id == id);
        }

        public IEnumerable<AnObject> GetAll()
        {
            return List;
        }

        internal void Add(AnObject value)
        {
            value.Id = ++_inc;
            List.Add(value);
        }

        internal void Remove(AnObject value)
        {
            List.Remove(value);
        }
    }
}