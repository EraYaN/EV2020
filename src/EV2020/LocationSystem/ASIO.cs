﻿using BlueWave.Interop.Asio;
using CircularBuffer;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EV2020.LocationSystem
{
    public class ASIO : IDisposable
    {
		public const double Fs = 48000;
		public static double T
		{
			get { return 1 / Fs; }
		}
		public int InChannels
		{
			get
			{
				if (driver != null)
					return driver.NumberInputChannels;
				return -1;
			}
		}
		public int OutChannels
		{
			get
			{
				if (driver != null)
					return driver.NumberOutputChannels;
				return -1;
			}
		}
		const int DefaultQueueDepth = (int)Fs * 10;
		List<long> times = new List<long>();
		List<CircularBuffer<double>> sampleBuffersOut = new List<CircularBuffer<double>>();
		List<CircularBuffer<double>> sampleBuffersIn = new List<CircularBuffer<double>>();
		List<Channel> inputChannels = new List<Channel>();
		List<Channel> outputChannels = new List<Channel>();
		public bool IsOutputEnabled = true;
		public bool IsInputEnabled = false;
		AsioDriver driver;
		bool _disposed;
		public ASIO(InstalledDriver instdriver, int[] InputChannels, int[] OutputChannels, int QueueDepth = DefaultQueueDepth)
		{
				
			System.Diagnostics.Debug.WriteLine("ASIO driver: {0}", instdriver);
			driver = AsioDriver.SelectDriver(instdriver);			
			driver.BufferUpdate += new EventHandler(AsioDriver_BufferUpdate);		

			System.Diagnostics.Debug.WriteLine("Version: {0}", driver.Version);
			System.Diagnostics.Debug.WriteLine("In: {0}; Out: {1}", driver.NumberInputChannels, driver.NumberOutputChannels);
			Init();
			foreach(int ch_ind in InputChannels){
				if (ch_ind < driver.NumberInputChannels && ch_ind >= 0)
				{
					inputChannels.Add(driver.InputChannels[ch_ind]);
					sampleBuffersIn.Add(new CircularBuffer<double>(QueueDepth, true));
				}
			}
			foreach (int ch_ind in OutputChannels)
			{
				if (ch_ind < driver.NumberOutputChannels && ch_ind >= 0)
				{
					outputChannels.Add(driver.OutputChannels[ch_ind]);
					sampleBuffersOut.Add(new CircularBuffer<double>(QueueDepth, true));
				}
			}			
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
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
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

			try
			{
				driver.Stop();
				driver.Release();
			}
			catch(Exception e){
				System.Diagnostics.Debug.WriteLine("Dispose Ex: {0}",e.Message);
			}
			driver = null;

			_disposed = true;
		}
		[STAThread]
		private void AsioDriver_BufferUpdate(object sender, EventArgs e)
		{
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			if (IsInputEnabled)
			{
				for (int ch = 0; ch < inputChannels.Count; ch++)
				{
					
					for (int i = 0; i < inputChannels[ch].BufferSize; i++)
					{
						sampleBuffersIn[ch].Put(inputChannels[ch][i]);
					}
				}
			}
			if (IsOutputEnabled)
			{
				for (int ch = 0; ch < outputChannels.Count; ch++)
				{
					int count1 = Math.Min(sampleBuffersOut[ch].Size, outputChannels[ch].BufferSize);
					int count2 = Math.Max(0, outputChannels[ch].BufferSize - count1);
					for (int i = 0; i < count1; i++)
					{
						outputChannels[ch][i] = Convert.ToSingle(sampleBuffersOut[ch].Get());
					}
					if (count2 > 0)
					{
						for (int i = count1; i < count2; i++)
						{
							outputChannels[ch][i] = 0;
						}
					}
				}
			}
		}
		[STAThread]
		public double[] getInputSamples(int _size, int channel = 0)
		{
			double[] inputSamples = new double[_size];
			for(int i = 0; i<_size;i++){
				while (sampleBuffersIn[channel].Size == 0)
				{
					;
				}
				inputSamples[i] = sampleBuffersIn[channel].Get();
			}
			return inputSamples;
		}
		[STAThread]
		public Matrix<double> getAllInputSamplesMatrix(int nSamples = 0)
		{
			int maxsize = 0;
			Matrix<double> inputSamplesMatrix;
			if (nSamples == 0)
			{
				for (int i = 0; i < sampleBuffersIn.Count; i++)
				{
					if (sampleBuffersIn[i].Size > maxsize)
						maxsize = sampleBuffersIn[i].Size;
				}
				nSamples = maxsize;				
			}

			inputSamplesMatrix = new DenseMatrix(nSamples, sampleBuffersIn.Count);			
			for (int i = 0; i < sampleBuffersIn.Count; i++)
			{
				for (int j = 0; j < nSamples; j++)
				{
					if (sampleBuffersIn[i].Size == 0)
					{
						break;
					}
					try
					{
						inputSamplesMatrix[j, i] = sampleBuffersIn[i].Get();
					}
					catch
					{
						break;
					}
				}
			}
			return inputSamplesMatrix;
		}
		[STAThread]
		public double[][] getAllInputSamples()
		{
			double[][] inputSamples = new double[inputChannels.Count][];
			for (int i = 0; i < inputChannels.Count; i++)
			{
				inputSamples[i] = sampleBuffersIn[i].Get(sampleBuffersIn[i].Size);
				sampleBuffersIn[i].Clear();
			}
			return inputSamples;
		}
		[STAThread]
		public double[] getAllOutputSamples(int channel = 0)
		{
			double[] inputSamples = sampleBuffersOut[channel].Get(sampleBuffersOut[channel].Size);
			sampleBuffersOut[channel].Clear();
			return inputSamples;
		}
		[STAThread]
		public void putOutputSamples(double[] OutputSamples, int channel = 0)
		{
			sampleBuffersOut[channel].Put(OutputSamples);
		}
		[STAThread]
		public void ShowControlPanel()
		{
			driver.ShowControlPanel();
		}
		[STAThread]
		public void ClearInput()
		{
			foreach (CircularBuffer<double> buff in sampleBuffersIn)
				buff.Clear();
		}
		[STAThread]
		public void ClearOutput()
		{
			foreach(CircularBuffer<double> buff in sampleBuffersOut)
				buff.Clear();
		}
		[STAThread]
		public ASIOLatencies GetLatencies()
		{
			driver.UpdateLatencies();
			return new ASIOLatencies { Input = driver.InputLatency, Output = driver.OutputLatency };
		}
    }
	public struct ASIOLatencies
	{
		public long Input;
		public long Output;
	}
}
