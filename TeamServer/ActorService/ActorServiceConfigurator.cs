using Akka.Configuration;
using Akka.Configuration.Hocon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.HostConfigurators;

namespace TeamServer.ActorService
{
    public sealed class ActorServiceConfigurator
    {
        private readonly Config _config;
        private readonly ActorServiceCommandLine _commandLine;

        public ActorServiceConfigurator()
        {
            _config = ReadConfigurationFromFile();
            _commandLine = new ActorServiceCommandLine();
        }

        public void Execute()
        {
            HostFactory.Run(hc =>
            {
                SetService(hc);
                SetServiceName(hc);
                SetServiceIdentity(hc);
                SetServiceStartMode(hc);
                SetServiceLogger(hc);
                SetServiceCommandLine(hc);
            });
        }

        private void SetService(HostConfigurator hc)
        {
            hc.Service<ActorServiceHost>(sc =>
            {
                sc.ConstructUsing(settings => new ActorServiceHost(_config, _commandLine));
                sc.WhenStarted((s, c) => s.Start(c));
                sc.WhenStopped((s, c) => s.Stop(c));
            });
        }

        private void SetServiceName(HostConfigurator hc)
        {
            string serviceName = _config.GetString(HoconPaths.ServiceName);
            string serviceDescription = _config.GetString(HoconPaths.ServiceDescription);

            if (string.IsNullOrWhiteSpace(serviceName))
                throw new NullReferenceException(string.Format("{0} is empty in the configuration file.", HoconPaths.ServiceName));

            if (string.IsNullOrWhiteSpace(serviceDescription))
                throw new NullReferenceException(string.Format("{0} is empty in the configuration file.", HoconPaths.ServiceDescription));

            hc.SetServiceName(serviceName);
            hc.SetDisplayName(serviceName);
            hc.SetDescription(serviceDescription);
        }

        private void SetServiceIdentity(HostConfigurator hc)
        {
            //
            // TODO: 서비스 실행 계정을 설정한다.
            //

            // 사용자 계정과 패스워드로 실행시킨다.
            //hc.RunAs("username", "password");

            // 콘솔 앱에서 사용자 계정과 패스워드를 입력받아 실행시킨다.
            //hc.RunAsPrompt();      

            // 로컬 시스템 계정으로 실행시킨다.
            //hc.RunAsLocalSystem();           

            // 로컬 서비스 계정으로 실행시킨다.
            //hc.RunAsLocalService();

            // NETWORK_SERVICE 계정으로 실행 시킨다.
            //hc.RunAsNetworkService();
        }

        private void SetServiceStartMode(HostConfigurator hc)
        {
            //
            // TODO: 서비스 시작 모드를 설정한다.
            //

            //hc.StartAutomatically(); 
            //hc.StartAutomaticallyDelayed();
            //hc.StartManually();
            //hc.Disabled();
        }

        private void SetServiceLogger(HostConfigurator hc)
        {
            hc.UseNLog();
        }

        private void SetServiceCommandLine(HostConfigurator hc)
        {
            //
            // INFO: 서비스 Argument는 대/소문자를 구분한다.
            //

            // -path:"c:\temp"
            hc.AddCommandLineDefinition("path", ci =>   
            {
                _commandLine.Path = ci.Trim();
            });

            // -frequency:2 
            hc.AddCommandLineDefinition("frequency", ci =>
            {
                _commandLine.Frequency = int.Parse(ci);
            });

            // --active  
            hc.AddCommandLineSwitch("active", ci =>
            {
                _commandLine.Active = ci;
            });
        }

        public static Config ReadConfigurationFromFile()
        {
            var assemblyPath = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;            
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(assemblyPath);            

            return GetAkkaConfiguration(configuration, "akka/akka.app")
                .WithFallback(GetAkkaConfiguration(configuration, "akka/akka.logging"))
                .WithFallback(GetAkkaConfiguration(configuration, "akka/akka.actor"))
                .WithFallback(GetAkkaConfiguration(configuration, "akka/akka.remote"))
                .WithFallback(GetAkkaConfiguration(configuration, "akka/akka.cluster"))
                .WithFallback(GetAkkaConfiguration(configuration, "akka/akka.petabridge"));
        }

        public static Config GetAkkaConfiguration(Configuration configuration, string sectionName)
        {
            return ((AkkaConfigurationSection)configuration.GetSection(sectionName)).AkkaConfig;
        }
    }
}
