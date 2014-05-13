using BlueWave.Interop.Asio;
using CircularBuffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EV2020.AudioSystem
{
    public class ASIO : IDisposable
    {
		const double Fs = 48000;
		const int ASIOBufferSize = 512;
		const int MaxSampleBufferSize = (int)Fs * 10;
		CircularBuffer<float> sampleBufferOut;
		CircularBuffer<float> sampleBufferIn;
		AsioDriver driver;
		bool _disposed;
		public ASIO(int DriverIndex, int QueueDepth = ASIOBufferSize*100)
		{
			InstalledDriver instdriver;
			if (DriverIndex >= 0 && DriverIndex < AsioDriver.InstalledDrivers.Length)
			{
				instdriver = AsioDriver.InstalledDrivers[DriverIndex];
			}
			else
			{
				throw new ArgumentException("The DriverIndex does not exist.", "DriverIndex");
			}
			System.Diagnostics.Debug.WriteLine("ASIO driver: {0}", instdriver);
			driver = AsioDriver.SelectDriver(instdriver);			
			driver.BufferUpdate += new EventHandler(AsioDriver_BufferUpdate);		

			System.Diagnostics.Debug.WriteLine("Version: {0}", driver.Version);
			System.Diagnostics.Debug.WriteLine("In: {0}; Out: {1}", driver.NumberInputChannels, driver.NumberOutputChannels);
			sampleBufferIn = new CircularBuffer<float>(QueueDepth,true);
			sampleBufferOut = new CircularBuffer<float>(QueueDepth,true);
			
		}
		~ASIO()
		{
			Dispose(false);
		}
		public void Init()
		{
			driver.SetSampleRate(Fs);
			driver.CreateBuffers(false);
			driver.Start();
		}
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				// free other managed objects that implement
				// IDisposable only
			}

			// release any unmanaged objects
			// set the object references to null
			driver.Stop();
			driver.Release();
			driver = null;

			_disposed = true;
		}
		private void AsioDriver_BufferUpdate(object sender, EventArgs e)
		{
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			// the driver is the sender
			AsioDriver driver = sender as AsioDriver;

			// get the input channel and the stereo output channels
			Channel input = driver.InputChannels[0];
			Channel leftOutput = driver.OutputChannels[0];
			Channel rightOutput = driver.OutputChannels[1];

			for (int i = 0; i < leftOutput.BufferSize; i++)
			{
				float sampleOut = sampleBufferOut.Get();
				leftOutput[i] = sampleOut;
				rightOutput[i] = sampleOut;
				sampleBufferIn.Put(input[i]);
			}
		}
		float[] getInputSamples()
		{
			float[] inputSamples = sampleBufferIn.Get(sampleBufferIn.Size);
			sampleBufferIn.Clear();
			return inputSamples;
		}
		void putOutputSamples(float[] OutputSamples)
		{
			sampleBufferOut.Put(OutputSamples);		

		}
		void ShowControlPanel()
		{
			driver.ShowControlPanel();
		}

    }
}
