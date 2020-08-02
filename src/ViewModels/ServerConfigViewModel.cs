﻿namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using Client;
    using Logging;
    using Models;
    using Services;
    using Helpers;

    public class ServerConfigViewModel : Screen, IHandle<BusyStateItem>, IHandle<ServerStateItem>, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;

        [Inject(Key = "filelogger")]
        private ILoggerService _logger;

        [Inject]
        private II18nService i18NService;

        [Inject]
        private INotificationService notificationService;

        [Inject]
        private ICEchoSCU _cechoSCU;

        private Action _doRequestAction;

        private string _serverIP = "localhost";
        private string _serverPort = "6104";
        private string _serverAET = "RIS";
        private string _localAET = "LOCAL_AET";
        private string _modality = "MR";

        private bool _isServerIPEnabled = true;
        private bool _isServerPortEnabled = true;
        private bool _isServerAETEnabled = true;
        private bool _isLocalAETEnabled = true;
        private bool _isModalityEnabled = true;

        private (bool isServerIPEnabled, bool isServerPortEnabled, bool isServerAETEnabled, bool isLocalAETEnabled, bool isModalityEnabled) _backupStatus;

        public string ServerIP
        {
            get => _serverIP;
            set
            {
                SetAndNotify(ref _serverIP, value);
                NotifyOfPropertyChange(() => CanDoEcho);
                NotifyOfPropertyChange(() => CanDoRequest);
            }
        }

        public string ServerPort
        {
            get => _serverPort;
            set
            {
                SetAndNotify(ref _serverPort, value);
                NotifyOfPropertyChange(() => CanDoEcho);
                NotifyOfPropertyChange(() => CanDoRequest);
            }
        }

        public string ServerAET
        {
            get => _serverAET;
            set
            {
                SetAndNotify(ref _serverAET, value);
                NotifyOfPropertyChange(() => CanDoEcho);
                NotifyOfPropertyChange(() => CanDoRequest);
            }
        }

        public string LocalAET
        {
            get => _localAET;
            set
            {
                SetAndNotify(ref _localAET, value);
                NotifyOfPropertyChange(() => CanDoEcho);
                NotifyOfPropertyChange(() => CanDoRequest);
            }
        }

        public string Modality
        {
            get => _modality;
            set => SetAndNotify(ref _modality, value);
        }

        public bool IsServerIPEnabled
        {
            get => _isServerIPEnabled;
            private set => SetAndNotify(ref _isServerIPEnabled, value);
        }

        public bool IsServerPortEnabled
        {
            get => _isServerPortEnabled;
            private set => SetAndNotify(ref _isServerPortEnabled, value);
        }

        public bool IsServerAETEnabled
        {
            get => _isServerAETEnabled;
            private set => SetAndNotify(ref _isServerAETEnabled, value);
        }

        public bool IsLocalAETEnabled
        {
            get => _isLocalAETEnabled;
            private set => SetAndNotify(ref _isLocalAETEnabled, value);
        }

        public bool IsModalityEnabled
        {
            get => _isModalityEnabled;
            private set => SetAndNotify(ref _isModalityEnabled, value);
        }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetAndNotify(ref _isBusy, value))
                {
                    NotifyOfPropertyChange(() => CanDoEcho);
                    NotifyOfPropertyChange(() => CanDoRequest);
                }
            }
        }

        private int _busyIndicatorColumn = 0;

        public int BusyIndicatorColumn
        {
            get => _busyIndicatorColumn;
            private set => SetAndNotify(ref _busyIndicatorColumn, value);
        }

        public ServerConfigViewModel(IEventAggregator eventAggregator, IModelValidator<ServerConfigViewModel> validator) : base(validator)
        {
            _eventAggregator = eventAggregator;
        }

        public bool CanDoRequest =>
            !string.IsNullOrEmpty(ServerIP) &&
            !string.IsNullOrEmpty(ServerPort) &&
            !string.IsNullOrEmpty(ServerAET) &&
            !string.IsNullOrEmpty(LocalAET) &&
            !IsBusy;

        public void DoRequest()
        {
            _doRequestAction?.Invoke();
        }

        public bool CanDoEcho =>
            !string.IsNullOrEmpty(ServerIP) &&
            !string.IsNullOrEmpty(ServerPort) &&
            !string.IsNullOrEmpty(ServerAET) &&
            !string.IsNullOrEmpty(LocalAET) &&
            !IsBusy;

        public async void DoEcho()
        {
            int port = ParseServerPort();
            if (port == 0)
                return;

            _backupStatus = (IsServerIPEnabled, IsServerPortEnabled, IsServerAETEnabled, IsLocalAETEnabled, IsModalityEnabled);
            IsServerIPEnabled = IsServerPortEnabled = IsServerAETEnabled = IsLocalAETEnabled = IsModalityEnabled = false;

            BusyIndicatorColumn = 0;

            IsBusy = true;

            bool result = await _cechoSCU.Echo(_serverIP, port, _serverAET, _localAET);

            IsBusy = false;

            (IsServerIPEnabled, IsServerPortEnabled, IsServerAETEnabled, IsLocalAETEnabled, IsModalityEnabled) = _backupStatus;

            if (result)
            {
                await notificationService.ShowToastAsync(i18NService.GetXmlStringByKey("TestSuccess"), new TimeSpan(0, 0, 3), Controls.ToastType.Info);
            }
            else
            {
                await notificationService.ShowToastAsync(i18NService.GetXmlStringByKey("TestFailed"), new TimeSpan(0, 0, 3), Controls.ToastType.Error);
            }
        }

        public void Init(IScreen parentViewModel)
        {
            Parent = parentViewModel;

            if (parentViewModel is WorklistViewModel)
            {
                _doRequestAction = WorklistQueryRequest;
                _eventAggregator.Subscribe(this, nameof(WorklistResultViewModel));
            }
            else if (parentViewModel is WorklistSCPViewModel)
            {
                _doRequestAction = StartWorklistServer;
                _eventAggregator.Subscribe(this, nameof(PatientsViewModel));
                ServerIP = SystemHelper.LocalIPAddress;
                LocalAET = ServerAET = "RIS";
                IsServerIPEnabled = IsServerAETEnabled = IsModalityEnabled = false;
            }
            else if (parentViewModel is QueryRetrieveViewModel)
            {
                _doRequestAction = QueryRetrieveRequest;
                _eventAggregator.Subscribe(this, nameof(QueryResultViewModel));
                ServerIP = "www.dicomserver.co.uk";
                ServerPort = "104";  // 104/11112
                ServerAET = "QRSCP";
            }
            else if (parentViewModel is CStoreViewModel)
            {
                _doRequestAction = CStoreRequest;
                _eventAggregator.Subscribe(this, nameof(CStoreFileListViewModel));
                ServerPort = "104";
                ServerAET = "CSTORESCP";
                IsModalityEnabled = false;
            }
            else if (parentViewModel is PrintViewModel)
            {
                _doRequestAction = PrintRequest;
                _eventAggregator.Subscribe(this, nameof(PrintPreviewViewModel));
                ServerPort = "7104";
                ServerAET = "PRINTSCP";
                IsModalityEnabled = false;
            }
            else if (parentViewModel is CStoreSCPViewModel)
            {
                _doRequestAction = StartCStoreServer;
                _eventAggregator.Subscribe(this, nameof(CStoreReceivedViewModel));
                ServerIP = SystemHelper.LocalIPAddress;
                ServerPort = "104";
                LocalAET = ServerAET = "CSTORESCP";
                IsServerIPEnabled = IsServerAETEnabled = IsModalityEnabled = false;
            }
            else if (parentViewModel is PrintSCPViewModel)
            {
                _doRequestAction = StartPrintServer;
                _eventAggregator.Subscribe(this, nameof(PrintJobsViewModel));
                ServerIP = SystemHelper.LocalIPAddress;
                ServerPort = "7104";
                LocalAET = ServerAET = "PRINTSCP";
                IsServerIPEnabled = IsServerAETEnabled = IsModalityEnabled = false;
            }
            else
            {
                // ...
            }
        }

        private void PublishClientRequest(string channel)
        {
            int port = ParseServerPort();
            if (port == 0)
                return;

            _eventAggregator.Publish(new ClientMessageItem(_serverIP, port, _serverAET, _localAET, _modality), channel);
        }

        private void PublishServerRequest(string channel)
        {
            int port = ParseServerPort();
            if (port == 0)
                return;

            _eventAggregator.Publish(new ServerMessageItem(port, _localAET), channel);
        }

        private void WorklistQueryRequest()
        {
            PublishClientRequest(nameof(WorklistResultViewModel));
        }

        private void QueryRetrieveRequest()
        {
            PublishClientRequest(nameof(QueryResultViewModel));
        }

        private void PrintRequest()
        {
            PublishClientRequest(nameof(PrintPreviewViewModel));
        }

        private void CStoreRequest()
        {
            PublishClientRequest(nameof(CStoreFileListViewModel));
        }

        private void StartWorklistServer()
        {
            PublishServerRequest(nameof(PatientsViewModel));
        }

        private void StartCStoreServer()
        {
            PublishServerRequest(nameof(CStoreReceivedViewModel));
        }

        private void StartPrintServer()
        {
            PublishServerRequest(nameof(PrintJobsViewModel));
        }

        public int ParseServerPort()
        {
            if (!int.TryParse(_serverPort, out int port))
            {
                _logger.Warn("非法的端口号：{0}，端口号必须为数字。", _serverPort);
                ServerPort = "";
            }

            return port;
        }

        public void Handle(BusyStateItem message)
        {
            BusyIndicatorColumn = 1;

            IsBusy = message.IsBusy;

            if (message.IsBusy)
            {
                _backupStatus = (IsServerIPEnabled, IsServerPortEnabled, IsServerAETEnabled, IsLocalAETEnabled, IsModalityEnabled);
                IsServerIPEnabled = IsServerPortEnabled = IsServerAETEnabled = IsLocalAETEnabled = IsModalityEnabled = false;
            }
            else
            {
                (IsServerIPEnabled, IsServerPortEnabled, IsServerAETEnabled, IsLocalAETEnabled, IsModalityEnabled) = _backupStatus;
            }
        }

        public void Handle(ServerStateItem message)
        {
            if (message.IsRuning)
            {
                _backupStatus = (IsServerIPEnabled, IsServerPortEnabled, IsServerAETEnabled, IsLocalAETEnabled, IsModalityEnabled);
                IsServerIPEnabled = IsServerPortEnabled = IsServerAETEnabled = IsLocalAETEnabled = IsModalityEnabled = false;
            }
            else
            {
                (IsServerIPEnabled, IsServerPortEnabled, IsServerAETEnabled, IsLocalAETEnabled, IsModalityEnabled) = _backupStatus;
            }
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }
    }
}
