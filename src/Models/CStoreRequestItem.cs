﻿namespace SimpleDICOMToolkit.Models
{
    public class CStoreRequestItem
    {
        public string ServerIP { get; private set; }

        public int ServerPort { get; private set; }

        public string ServerAET { get; private set; }

        public string LocalAET { get; private set; }

        public CStoreRequestItem(
            string ip,
            int port,
            string serverAET,
            string localAET)
        {
            ServerIP = ip;
            ServerPort = port;
            ServerAET = serverAET;
            LocalAET = localAET;
        }
    }
}
