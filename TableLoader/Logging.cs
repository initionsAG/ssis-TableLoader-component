using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader
{
    

    class Logging
    {
        public static IsagEvents Events;

        public static void Log(IsagEvents.IsagEventType eventType, string message)
        {
            Events.Fire(eventType, message + DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString());
        }
    }
}
