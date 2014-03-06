using System;
using System.IO.Ports;

namespace EV2020.Communication
{
    public class SerialDataEventArgs : EventArgs
    {
        public readonly byte DataByte;
        public SerialDataReceivedEventArgs innerEvent;
        public SerialDataEventArgs(byte _DataByte, SerialDataReceivedEventArgs e)
        {
            DataByte = _DataByte;            
            innerEvent = e;
        }
    }
}
