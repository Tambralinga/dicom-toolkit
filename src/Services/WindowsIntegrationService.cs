﻿using System;
using System.Management;
using System.Security.Principal;
using SimpleDICOMToolkit.Logging;
using SimpleDICOMToolkit.Utils;

namespace SimpleDICOMToolkit.Services
{
    public class WindowsIntegrationService : IWindowsIntegrationService
    {
        private ILoggerService Logger => SimpleIoC.Get<ILoggerService>("filelogger");

        private ManagementEventWatcher systemUsesLightThemeWatcher;
        private ManagementEventWatcher windowPrevalenceAccentColorWatcher;

        public bool IsSystemUsesLightTheme
        {
            get
            {
                return SysUtil.SystemUsesLightTheme();
            }
        }

        public bool IsWindowPrevalenceAccentColor
        {
            get
            {
                return SysUtil.IsWindowPrevalenceAccentColor();
            }
        }

        public event EventHandler SystemUsesLightThemeChanged = delegate { };
        public event EventHandler WindowPrevalenceAccentColorChanged = delegate { };

        public void StartMonitoringSystemUsesLightTheme()
        {
            try
            {
                var currentUser = WindowsIdentity.GetCurrent();
                if (currentUser != null && currentUser.User != null)
                {
                    var wqlEventQuery = new EventQuery(string.Format(@"SELECT * FROM RegistryValueChangeEvent WHERE Hive='HKEY_USERS' AND KeyPath='{0}\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize' AND ValueName='SystemUsesLightTheme'", currentUser.User.Value));
                    this.systemUsesLightThemeWatcher = new ManagementEventWatcher(wqlEventQuery);
                    this.systemUsesLightThemeWatcher.EventArrived += this.AppsUseLightThemeWatcher_EventArrived;
                    this.systemUsesLightThemeWatcher.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not start monitoring system uses light theme. Exception: {0}", ex.Message);
            }
        }

        public void StopMonitoringSystemUsesLightTheme()
        {
            try
            {
                if (this.systemUsesLightThemeWatcher != null)
                {
                    this.systemUsesLightThemeWatcher.Stop();
                    this.systemUsesLightThemeWatcher.EventArrived -= this.AppsUseLightThemeWatcher_EventArrived;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not stop monitoring system uses light theme. Exception: {0}", ex.Message);
            }
        }

        public void StartMonitoringWindowPrevalenceAccentColor()
        {
            try
            {
                var currentUser = WindowsIdentity.GetCurrent();
                if (currentUser != null && currentUser.User != null)
                {
                    var wqlEventQuery = new EventQuery(string.Format(@"SELECT * FROM RegistryValueChangeEvent WHERE Hive='HKEY_USERS' AND KeyPath='{0}\\SOFTWARE\\Microsoft\\Windows\\DWM' AND ValueName='ColorPrevalence'", currentUser.User.Value));
                    windowPrevalenceAccentColorWatcher = new ManagementEventWatcher(wqlEventQuery);
                    windowPrevalenceAccentColorWatcher.EventArrived += WindowPrevalenceAccentColorWatcher_EventArrived;
                    windowPrevalenceAccentColorWatcher.Start();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Could not start monitoring window prevalence accent color. Exception: {0}", e.Message);
            }
        }

        public void StopMonitoringWindowPrevalenceAccentColor()
        {
            try
            {
                if (windowPrevalenceAccentColorWatcher != null)
                {
                    windowPrevalenceAccentColorWatcher.Stop();
                    windowPrevalenceAccentColorWatcher.EventArrived -= WindowPrevalenceAccentColorWatcher_EventArrived;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Could not stop monitoring window prevalence accent color. Exception: {0}", e.Message);
            }
        }

        private void AppsUseLightThemeWatcher_EventArrived(object s, EventArrivedEventArgs e)
        {
            this.SystemUsesLightThemeChanged(this, new EventArgs());
        }

        private void WindowPrevalenceAccentColorWatcher_EventArrived(object s, EventArrivedEventArgs e)
        {
            WindowPrevalenceAccentColorChanged(this, new EventArgs());
        }
    }
}
