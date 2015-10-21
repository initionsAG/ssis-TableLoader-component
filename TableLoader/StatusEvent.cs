using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader
{
    /// <summary>
    /// A list of status events
    /// </summary>
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

    /// <summary>
    /// A status that will be fired as an event
    /// </summary>
    public class StatusEvent
    {
        /// <summary>
        /// thread number
        /// </summary>
        public int Nr { get; set; }
        /// <summary>
        /// number of data rows
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// status type
        /// </summary>
        public Status.StatusType StatusType { get; set; }
        /// <summary>
        /// timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// event parameter (i.e. temporary tablename)
        /// Event messages will be formatted with string.format. 
        /// Param1 will will be the first parameter for string.Format.
        /// </summary>
        public string Param1 { get; set; }
        /// <summary>
        /// event type
        /// </summary>
        public IsagEvents.IsagEventType EventType { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="nr">thread number</param>
        /// <param name="count">number of data rows</param>
        /// <param name="statusType">status type</param>
        /// <param name="eventType">event type</param>
        public StatusEvent(int nr, int count, Status.StatusType statusType, IsagEvents.IsagEventType eventType)
        {
            Nr = nr;
            Count = count;
            StatusType = statusType;
            Timestamp = DateTime.Now;
            EventType = eventType;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="nr">thread number</param>
        /// <param name="count">number of data rows</param>
        /// <param name="statusType">status type</param>
        /// <param name="timestamp">timestamp</param>
        /// <param name="eventType">event type</param>
        public StatusEvent(int nr, int count, Status.StatusType statusType, DateTime timestamp, IsagEvents.IsagEventType eventType)
        {
            Nr = nr;
            Count = count;
            StatusType = statusType;
            Timestamp = timestamp;
            EventType = eventType;
        }

        /// <summary>
        /// Add this to a status
        /// </summary>
        /// <param name="status">status</param>
        public void LogStatusEvent(Status status)
        {
            status.AddStatus(this);
        }
    }
}
