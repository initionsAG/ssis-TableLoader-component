using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader
{
    
    /// <summary>
    /// Logging for TableLoader
    /// </summary>
    class Logging
    {
        /// <summary>
        /// Handles SSIS events
        /// </summary>
        public static IsagEvents Events;

        /// <summary>
        /// Fire event
        /// </summary>
        /// <param name="eventType">Isag event type</param>
        /// <param name="message">event message</param>
        public static void Log(IsagEvents.IsagEventType eventType, string message)
        {
            Events.Fire(eventType, message + DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString());
        }
    }
}
