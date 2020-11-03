﻿namespace SimpleDICOMToolkit.Client
{
    using StyletIoC;
    using Dicom.Network;
    using DicomClient = Dicom.Network.Client.DicomClient;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Logging;
    using Models;

    public class StoreSCU : IStoreSCU
    {
        private readonly ILoggerService Logger;

        public StoreSCU([Inject(Key = "filelogger")] ILoggerService loggerService)
        {
            Logger = loggerService;
        }

        public async ValueTask StoreImageAsync(string serverIp, int serverPort, string serverAET, string localAET, IEnumerable<CStoreItem> items)
        {
            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);

            foreach (CStoreItem item in items)
            {
                DicomCStoreRequest request = new DicomCStoreRequest(item.File)
                {
                    OnResponseReceived = (req, res) =>
                    {
                        if (res.Status != DicomStatus.Success)
                        {
                            Logger.Error("C-STORE send failed. Instance UID - [{0}]", req.SOPInstanceUID);
                            item.Status = CStoreItemStatus.Failed;
                        }
                        else
                        {
                            item.Status = CStoreItemStatus.Success;
                        }
                    }
                };

                await client.AddRequestAsync(request);
            }

            await client.SendAsync();
        }
    }
}
