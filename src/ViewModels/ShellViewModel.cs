﻿namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using Logging;
    using Utils;
    using MQTT;

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public static string WindowName = "Simple DICOM Toolkit";

        private readonly ILoggerService logger;
        private readonly ISimpleMqttService mqttService;

        public ShellViewModel(
            DcmItemsViewModel dcmItemsViewModel,
            WorklistViewModel worklistViewModel,
            QueryRetrieveViewModel queryRetrieveViewModel,
            CStoreViewModel cstoreViewModel,
            CStoreSCPViewModel cstoreSCPViewModel,
            PrintViewModel printViewModel,
            PrintSCPViewModel printSCPViewModel,
            ISimpleMqttService mqttService,
            [Inject(Key = "filelogger")] ILoggerService loggerService)
        {
            DisplayName = WindowName;
            logger = loggerService;
            this.mqttService = mqttService;

            string[] args = ApplicationUtil.CommandLineArgs;

            if (args.Length <= 1)
            {
                // 默认以完整模式运行
                AddAllItems(dcmItemsViewModel, worklistViewModel, queryRetrieveViewModel, cstoreViewModel, cstoreSCPViewModel, printViewModel, printSCPViewModel);
            }
            else
            {
                logger.Debug("Application startup with Args: [{0}]", string.Join(" ", System.Environment.GetCommandLineArgs()));
                // 第一个参数为应用程序路径
                // 只检测第2个参数
                if (ApplicationUtil.IsClientModeOnly(args[1]))
                {
                    AddClientModeItems(dcmItemsViewModel, worklistViewModel, queryRetrieveViewModel, cstoreViewModel, printViewModel);
                }
                else if (ApplicationUtil.IsServerModeOnly(args[1]))
                {
                    AddServerModeItems(cstoreSCPViewModel, printSCPViewModel);
                }
                else
                {
                    // 默认以完整模式运行
                    AddAllItems(dcmItemsViewModel, worklistViewModel, queryRetrieveViewModel, cstoreViewModel, cstoreSCPViewModel, printViewModel, printSCPViewModel);
                }
            }
        }

        protected override async void OnViewLoaded()
        {
            base.OnViewLoaded();

            await mqttService.StartAsync(9629);

            ActiveItem = Items.Count > 0 ? Items[0] : null;
        }

        private void AddClientModeItems(
            DcmItemsViewModel dcmItemsViewModel,
            WorklistViewModel worklistViewModel,
            QueryRetrieveViewModel queryRetrieveViewModel,
            CStoreViewModel cstoreViewModel,
            PrintViewModel printViewModel)
        {
            Items.Add(dcmItemsViewModel);
            Items.Add(worklistViewModel);
            Items.Add(queryRetrieveViewModel);
            Items.Add(cstoreViewModel);
            Items.Add(printViewModel);
        }

        private void AddServerModeItems(
            CStoreSCPViewModel cstoreSCPViewModel,
            PrintSCPViewModel printSCPViewModel)
        {
            Items.Add(cstoreSCPViewModel);
            Items.Add(printSCPViewModel);
        }

        private void AddAllItems(
            DcmItemsViewModel dcmItemsViewModel,
            WorklistViewModel worklistViewModel,
            QueryRetrieveViewModel queryRetrieveViewModel,
            CStoreViewModel cstoreViewModel,
            CStoreSCPViewModel cstoreSCPViewModel,
            PrintViewModel printViewModel,
            PrintSCPViewModel printSCPViewModel)
        {
            Items.Add(dcmItemsViewModel);
            Items.Add(worklistViewModel);
            Items.Add(queryRetrieveViewModel);
            Items.Add(cstoreViewModel);
            Items.Add(cstoreSCPViewModel);
            Items.Add(printViewModel);
            Items.Add(printSCPViewModel);
        }
    }
}
