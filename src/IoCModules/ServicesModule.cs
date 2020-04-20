﻿namespace SimpleDICOMToolkit.IoCModules
{
    using StyletIoC;
    using Logging;
    using MQTT;
    using Services;

    internal class ServicesModule : StyletIoCModule
    {
        protected override void Load()
        {
            Bind<ILoggerService>().To<LoggerService>().InSingletonScope().AsWeakBinding();
            Bind<IDialogServiceEx>().To<DialogServiceEx>().InSingletonScope().AsWeakBinding();
            Bind<INotificationService>().To<NotificationService>().InSingletonScope().AsWeakBinding();
            Bind<ISimpleMqttService>().To<SimpleMqttService>().InSingletonScope().AsWeakBinding();
            Bind<IMessenger>().To<Messenger>().InSingletonScope().AsWeakBinding();
        }
    }
}
