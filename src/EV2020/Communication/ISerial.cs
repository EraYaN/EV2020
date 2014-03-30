using System;
using System.IO.Ports;

namespace EV2020.Communication
{
	/// <summary>
	/// A simple interface for the SerialInterface.
	/// </summary>
    interface ISerial
    {
        event EventHandler<SerialDataEventArgs> SerialDataEvent;       
    }
}
