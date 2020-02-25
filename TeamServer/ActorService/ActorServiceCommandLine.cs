using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamServer.ActorService
{
    public sealed class ActorServiceCommandLine
    {
        public string Path { get; set; }

        public int Frequency { get; set; }

        public bool Active { get; set; }

        public void Log(ILoggingAdapter logger)
        {
            logger.Info("Service Command Line - path: {0}", Path);
            logger.Info("Service Command Line - frequency: {0}", Frequency);
            logger.Info("Service Command Line - active: {0}", Active);
        }
    }
}
