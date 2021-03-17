﻿namespace SimpleDICOMToolkit.Models
{
    public class BusyStateEvent
    {
        public bool IsBusy { get; }

        public BusyStateEvent(bool isBusy)
        {
            IsBusy = isBusy;
        }

        public override string ToString()
        {
            return string.Format("Is busy: {0}", IsBusy);
        }
    }
}
