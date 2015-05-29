using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader
{
    public class EventMessageList
    {
        private List<EventMessage> _messages = new List<EventMessage>();

        public void FireEvents(IsagEvents events)
        {
            lock (_messages)
            {
                foreach (EventMessage message in _messages)
                {
                    message.FireEvent(events);
                }

                _messages.Clear();
            }
        }

      

        public void AddMessage(string message, IsagEvents.IsagEventType eventType)
        {
            lock (_messages)
            {
                _messages.Add(new EventMessage() { Message = message, EventType = eventType });
            }
            
        }

        public void AddMessage(string message, string[]  parameter, IsagEvents.IsagEventType eventType)
        {
            lock (_messages)
            {
                _messages.Add(new EventMessage() { Message = message, Parameter = parameter, EventType = eventType });
            }

        }

    }


    public class EventMessage
    {
        public string Message { get; set; }
        public string[] Parameter { get; set; }
        public IsagEvents.IsagEventType EventType { get; set; }

        public void FireEvent(IsagEvents events)
        {
            if (Parameter != null && Parameter.Length > 0) events.Fire(EventType, Message, Parameter); 
            else events.Fire(EventType, Message);
        }      
    }
}
