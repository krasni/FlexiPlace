﻿using Topshelf;

namespace FlexiPlaceWinServiceTopShelf
{
    public class Program
    {
        /// <summary>
        /// Configure TopShelf to construct service using HangFire
        /// Bootstrap class - has Hangfire configuration
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();

            HostFactory.Run(config =>
            {
                config.Service<BootStrap>(service =>
                {
                    service.ConstructUsing(s => new BootStrap());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                config.RunAsLocalSystem();
                config.SetDescription("FlexiPlaceHangFire");
                config.SetDisplayName("FlexiPlaceHangFire");

            });
        }
    }
}


//HostFactory.New(config =>
//{
//config.EnableServiceRecovery(x =>
//{
//    x.OnCrashOnly();
//    x.RestartService(0);
//});
//});