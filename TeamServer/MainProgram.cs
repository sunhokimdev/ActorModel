using TeamServer.ActorService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamServer
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            var actorServiceConfigurator = new ActorServiceConfigurator();
            actorServiceConfigurator.Execute();
        }
    }
}
