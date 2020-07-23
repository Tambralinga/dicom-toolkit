﻿namespace SimpleDICOMToolkit.ViewModels
{
    using Dicom;
    using Stylet;
    using StyletIoC;
    using System;
    using System.IO;
    using System.Windows;
    using Client;
    using Services;
    using Models;

    public class CStoreFileListViewModel : Screen, IHandle<ClientMessageItem>, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        private IWindowManager _windowManager;

        [Inject]
        private IViewModelFactory _viewModelFactory;

        [Inject]
        private IDialogServiceEx _dialogService;

        [Inject]
        private ICStoreSCU _cstoreSCU;

        public BindableCollection<CStoreItem> FileList { get; private set; } = new BindableCollection<CStoreItem>();

        public CStoreFileListViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this, nameof(CStoreFileListViewModel));
        }

        public async void Handle(ClientMessageItem message)
        {
            if (FileList.Count == 0)
                return;

            _eventAggregator.Publish(new BusyStateItem(true), nameof(CStoreFileListViewModel));

            try
            {
                await _cstoreSCU.StoreImageAsync(message.ServerIP, message.ServerPort, message.ServerAET, message.LocalAET, FileList);
            }
            finally
            {
                _eventAggregator.Publish(new BusyStateItem(false), nameof(CStoreFileListViewModel));
            }
        }

        public void AddFiles()
        {
            _dialogService.ShowOpenFileDialog("DICOM Image (*.dcm;*.dic)|*.dcm;*.dic", true, null, AddDcmFilesToList);
        }

        public void OnDragFilesOver(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        public void OnDropFiles(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            Array files = e.Data.GetData(DataFormats.FileDrop) as Array;

            foreach (object file in files)
            {
                string path = file as string;

                if (Directory.Exists(path))  // 文件夹
                {
                    DirectoryInfo dir = new DirectoryInfo(path);

                    FileInfo[] fileinfos = dir.GetFiles();

                    foreach (FileInfo info in fileinfos)
                    {
                        if (!info.Exists)
                            continue;

                        if (!DicomFile.HasValidHeader(info.FullName))
                            continue;

                        FileList.Add(new CStoreItem(FileList.Count, info.FullName));
                    }

                    continue;
                }

                if (File.Exists(path) && DicomFile.HasValidHeader(path))
                {
                    FileList.Add(new CStoreItem(FileList.Count, path));
                }
            }
        }

        public void PreviewCStoreItem(CStoreItem item)
        {
            var preview = _viewModelFactory.GetPreviewImageViewModel();
            preview.Initialize(item.File);

            _windowManager.ShowDialog(preview, this);
        }

        public void DeleteCStoreItem(CStoreItem item)
        {
            FileList.Remove(item);
            ReIndexItems();
        }

        public void ClearItems()
        {
            FileList.Clear();
        }

        private void AddDcmFilesToList(bool? result, string[] files)
        {
            foreach (string file in files)
            {
                FileList.Add(new CStoreItem(FileList.Count, file));
            }
        }

        private void ReIndexItems()
        {
            for (int i = 0; i < FileList.Count; i++)
            {
                FileList[i].Id = i;
            }
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }
    }
}
