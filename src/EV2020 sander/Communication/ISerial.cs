using System;
using System.IO.Ports;

namespace EV2020.Communication
{
    interface ISerial
    {
        event EventHandler<SerialDataEventArgs> SerialDataEvent;       
    }
}
