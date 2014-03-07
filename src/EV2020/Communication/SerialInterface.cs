using System;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;

namespace EV2020.Communication
{
    public class SerialInterface : ISerial, IDisposable
    {
        string port;
        int baudrate;
        SerialPort serialPort;
        public string lastError = "";
        public event EventHandler<SerialDataEventArgs> SerialDataEvent;
        public bool IsOpen
        {
            get
            {
                return serialPort.IsOpen;
            }
        }
        public int BytesInTBuffer
        {
            get
            {
                return serialPort.BytesToWrite;
            }
        }
        public int BytesInRBuffer
        {
            get
            {
                return serialPort.BytesToRead;
            }
        }
        public SerialInterface(string _port, int _baudrate)
        {
            port = _port;
            baudrate = _baudrate;
            serialPort = new SerialPort();
            serialPort.PortName = port;
            serialPort.BaudRate = baudrate;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Handshake = Handshake.RequestToSend;
			serialPort.ReadBufferSize = 128000;
			serialPort.WriteBufferSize = 128000;
            serialPort.ReadTimeout = 1000;
            serialPort.WriteTimeout = 1000;
            serialPort.DataReceived += serialPort_DataReceived;
            serialPort.ErrorReceived += serialPort_ErrorReceived;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }       
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) 
            {
                // free managed resources
                if (serialPort != null)
                {
                    serialPort.Close();
                    serialPort.Dispose();
                }
            }            
        }

        public int OpenPort()
        {
            try
            {
                serialPort.Open();
                return 0;
            }
            catch (IOException e)
            {
                //MessageBox.Show("SerialInterface Error:\n" + e.Message, "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
                lastError = e.Message;
                return -1;
            }
            catch (ArgumentException e)
            {
                //MessageBox.Show("SerialInterface Error:\n" + e.Message, "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
                lastError = e.Message;
                return -2;
            }
            catch (InvalidOperationException e)
            {
                //MessageBox.Show("SerialInterface Error:\n" + e.Message, "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
                lastError = e.Message;
                return -3;
            }
            catch (UnauthorizedAccessException e)
            {
                //MessageBox.Show("SerialInterface Error:\n" + e.Message, "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
                lastError = e.Message;
                return -4;
            }
            catch (Exception e)
            {
                //MessageBox.Show("SerialInterface Error:\n" + e.Message, "SerialInterface Error", MessageBoxButton.OK, MessageBoxImage.Error);
                lastError = e.Message;
                return 1;
            }
        }
        public void Write(Byte data)
        {
			if (!serialPort.IsOpen)
				return;
			byte[] buf = {data};
            serialPort.Write(buf,0,1);
			//System.Diagnostics.Debug.WriteLine("Serial byte sent: {0:X}", data);
        }
		public void WriteLine(String data)
		{
			if (!serialPort.IsOpen)
				return;
			
			serialPort.Write(data+'\n');
			//System.Diagnostics.Debug.WriteLine("Serial bytes sent: {0}", data+'\n');
		}        

        void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {            
            //MessageBox.Show();
            throw new NotImplementedException();
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
			string indata = serialPort.ReadExisting();
			System.Diagnostics.Debug.WriteLine("Serial bytes received: {0}", indata);
			DataSerial(indata, e);           
        }
    
        void DataSerial(String s, SerialDataReceivedEventArgs e)
        {
            // Do something before the event…
            OnSerialDataChanged(new SerialDataEventArgs(s,e));
            // or do something after the event. 
        }

        protected virtual void OnSerialDataChanged(SerialDataEventArgs e)
        {
            if (SerialDataEvent != null)
            {
                SerialDataEvent(this, e);
            }
        }
    }
}
