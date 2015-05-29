using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader
{

    public class StatusEventList
    {
        private List<StatusEvent> _eventStatusList = new List<StatusEvent>();

        public void LogStatusEvents(Status events)
        {
            lock (_eventStatusList)
            {
                foreach (StatusEvent statusEvent in _eventStatusList)
                {
                    statusEvent.LogStatusEvent(events);
                }

                _eventStatusList.Clear();
            }
        }



        public void AddStatusEvent(int nr, int count, Status.StatusType statusType, IsagEvents.IsagEventType eventType)
        {
            lock (_eventStatusList)
            {
                _eventStatusList.Add(new StatusEvent(nr, count, statusType, eventType));
            }
        }

        public void AddStatusEvent(int nr, int count, Status.StatusType statusType, string param1, IsagEvents.IsagEventType eventType)
        {
            lock (_eventStatusList)
            {
                StatusEvent statusEvent = new StatusEvent(nr, count, statusType, eventType);
                statusEvent.Param1 = param1;
                _eventStatusList.Add(statusEvent);
            }
        }

        public void AddStatusEvent(int nr, int count, Status.StatusType statusType, DateTime timestamp, IsagEvents.IsagEventType eventType)
        {
            lock (_eventStatusList)
            {
                _eventStatusList.Add(new StatusEvent(nr, count, statusType, timestamp, eventType));
            }

        }

    }

    public class StatusEvent
    {
        public int Nr { get; set; }
        public int Count { get; set; }
        public Status.StatusType StatusType { get; set; }
        public DateTime Timestamp { get; set; }
        public string Param1 { get; set; }
        public IsagEvents.IsagEventType EventType { get; set; }

        public StatusEvent(int nr, int count, Status.StatusType statusType, IsagEvents.IsagEventType eventType)
        {
            Nr = nr;
            Count = count;
            StatusType = statusType;
            Timestamp = DateTime.Now;
            EventType = eventType;
        }

        public StatusEvent(int nr, int count, Status.StatusType statusType, DateTime timestamp, IsagEvents.IsagEventType eventType)
        {
            Nr = nr;
            Count = count;
            StatusType = statusType;
            Timestamp = timestamp;
            EventType = eventType;
        }

        public void LogStatusEvent(Status status)
        {
            status.AddStatus(this);
        }
    }
}
