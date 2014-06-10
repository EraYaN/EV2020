using System;
using System.IO.Ports;

namespace EV2020.Communication
{
	/// <summary>
	/// A custom event used for higher-level (String) serial messaging.
	/// </summary>
    public class SerialDataEventArgs : EventArgs
    {
		/// <summary>
		/// The data to be saved in this eventArgs
		/// </summary>
        public readonly String Data;
		/// <summary>
		/// The eventArgs to be embedded
		/// </summary>
        public SerialDataReceivedEventArgs innerEvent;
		/// <summary>
		/// Wrapper for serial data event
		/// </summary>
		/// <param name="_Data">The actual data of this event</param>
		/// <param name="e">Embedded/inner SerialDataEventArgs in this SerialDataEventArgs</param>
        public SerialDataEventArgs(String _Data, SerialDataReceivedEventArgs e)
        {
            Data = _Data;            
            innerEvent = e;
        }
    }
}
