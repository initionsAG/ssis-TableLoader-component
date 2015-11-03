using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader.Log
{
    /// <summary>
    /// Event message list 
    /// 
    /// Message list is filled in threads. Main Process has to fire SSIS event messages
    /// </summary>
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

      
        /// <summary>
        /// Adds a message
        /// </summary>
        /// <param name="message">message text</param>
        /// <param name="eventType">event type</param>
        public void AddMessage(string message, IsagEvents.IsagEventType eventType)
        {
            lock (_messages)
            {
                _messages.Add(new EventMessage() { Message = message, EventType = eventType });
            }
            
        }

        /// <summary>
        /// Adds a message
        /// </summary>
        /// <param name="message">message text</param>
        /// <param name="parameter">message parameter</param>
        /// <param name="eventType">event type</param>

        public void AddMessage(string message, string[]  parameter, IsagEvents.IsagEventType eventType)
        {
            lock (_messages)
            {
                _messages.Add(new EventMessage() { Message = message, Parameter = parameter, EventType = eventType });
            }

        }

    }

    /// <summary>
    /// Definition for an SSIS event
    /// </summary>
    public class EventMessage
    {
        /// <summary>
        /// message text
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// message parameter
        /// </summary>
        public string[] Parameter { get; set; }

        /// <summary>
        /// event type
        /// </summary>
        public IsagEvents.IsagEventType EventType { get; set; }

        public void FireEvent(IsagEvents events)
        {
            if (Parameter != null && Parameter.Length > 0) events.Fire(EventType, Message, Parameter); 
            else events.Fire(EventType, Message);
        }      
    }
}
