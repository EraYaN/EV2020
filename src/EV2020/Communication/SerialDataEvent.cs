using System;
using System.IO.Ports;

namespace EV2020.Communication
{
	/// <summary>
	/// A custom event used for higher-level (String) serial messaging.
	/// </summary>
    public class SerialDataEventArgs : EventArgs
    {
        public readonly String Data;
        public SerialDataReceivedEventArgs innerEvent;
        public SerialDataEventArgs(String _Data, SerialDataReceivedEventArgs e)
        {
            Data = _Data;            
            innerEvent = e;
        }
    }
}
