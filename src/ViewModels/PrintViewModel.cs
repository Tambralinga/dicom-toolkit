﻿namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;

    public class PrintViewModel : Screen, IDisposable
    {
        [Inject]
        private IWindowManager _windowManager;

        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public PrintOptionsViewModel PrintOptionsViewModel { get; private set; }

        [Inject]
        public PrintPreviewViewModel PrintPreviewViewModel { get; private set; }

        public PrintViewModel()
        {
            DisplayName = "Print";
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();
            ServerConfigViewModel.Init(this);
        }

        public void ShowOptions()
        {
            _windowManager.ShowDialog(PrintOptionsViewModel);
        }

        public void Dispose()
        {
            // TODO
            ServerConfigViewModel.Dispose();
            PrintOptionsViewModel.Dispose();
            PrintPreviewViewModel.Dispose();
        }
    }
}
