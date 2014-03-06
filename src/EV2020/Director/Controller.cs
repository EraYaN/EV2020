using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EV2020.Director
{
	public class Controller
	{
		const int BatteryThreshold = 15000;
		
		private bool _done = false;
		private bool _continue = false;
		private bool _isWaitingForReply = false;
		private bool _isReceivingLine = false;
		private int _linesReceived = 0;
		private StringBuilder _receivingBuffer;

		Timer sendInstructions;

		private int driving = 150;
		private int steering = 150;
		private int audioState = 0;

		private int currentDriving = 0;
		private int currentSteering = 0;
		private int currentLeftDist = 0;
		private int currentRightDist = 0;
		private int currentBatteryVoltage = 0;
		private int currentAudioState = 0;

		public Controller()
		{
			Data.com.SerialDataEvent += com_SerialDataEvent;
			sendInstructions = new Timer();
			sendInstructions.Elapsed+=sendInstructions_Elapsed;
		}

		void sendInstructions_Elapsed(object sender, ElapsedEventArgs e)
		{
			//TODO send current drive and steering values.
			//TODO figure out audio.
		}

		void com_SerialDataEvent(object sender, Communication.SerialDataEventArgs e)
		{
			if (!_isWaitingForReply)
				return;
			if (!_isReceivingLine)
			{
				_receivingBuffer = new StringBuilder();
				char c = (char)e.DataByte;
				if (c == 4)
				{
					//Block done (= EOT).
					processReply(_receivingBuffer.ToString());
					_isWaitingForReply = false;
				}
				else if (c != '\n')
				{
					_isReceivingLine = true;
					_receivingBuffer.Append(c);
				}
				else
				{
					//TODO handle this (received just \n)
				}
			}
			else
			{
				char c = (char)e.DataByte;
				if (c != '\n')
				{
					_isReceivingLine = true;
					_receivingBuffer.Append(c);
				}
				else
				{
					_isReceivingLine = false;
					processReply(_receivingBuffer.ToString());
					_receivingBuffer = null;
					_linesReceived++;					
				}
			}
		}

		void processReply(String line)
		{
			//TODO
			if (line.Substring(0, 1) == "D")
			{
				//Steering/Driving info (135 to 165 and 150 is neutral)
				if(!Int32.TryParse(line.Substring(1, 3), out currentSteering)){
					//TODO handle (bogus data)
				}
				if(!Int32.TryParse(line.Substring(5, 3), out currentDriving)){
					//TODO handle (bogus data)
				}
				if (currentSteering != steering)
				{
					//TODO handle ('D' command not accepted)
				}
				if (currentDriving != driving)
				{
					//TODO handle ('D' command not accepted)
				}
			}
			else if (line.Substring(0, 1) == "U")
			{
				//Ultrasonic sendor data (cm)
				if(!Int32.TryParse(line.Substring(1, 3), out currentLeftDist)){
					//TODO handle (bogus data)
				}
				if(!Int32.TryParse(line.Substring(5, 3), out currentRightDist)){
					//TODO handle (bogus data)
				}
				if (currentLeftDist == 999)
				{
					//TODO handle (too far)
				}
				if (currentRightDist == 999)
				{
					//TODO handle (too far)
				}
			}
			else if (line.Substring(0, 2) == "Au")
			{
				//Audio beacon status
				if (!Int32.TryParse(line.Substring(1, 1), out currentAudioState))
				{
					//TODO handle (bogus data)
				}
				if (currentAudioState != audioState)
				{
					//TODO handle ('A' command not accepted)
				}
			}
			else if (line.Substring(0, 1) == "A")
			{
				//Battery voltage (mV)
				if (!Int32.TryParse(line.Substring(1, 5), out currentBatteryVoltage))
				{
					//TODO handle (bogus data)
				}
				if (currentBatteryVoltage < BatteryThreshold)
				{
					//TODO hanlde voltage too low
				}
			}
		}

		#region Private command methods
		private void sendCommand(String line)
		{
			foreach (char c in line)
			{
				Data.com.SendByte((byte)c);
			}
			Data.com.SendByte((byte)'\n');
		}
		private void enableAudio()
		{
			sendCommand("A1");
			audioState = 1;
		}
		private void disableAudio()
		{
			sendCommand("A0");
			audioState = 0;
		}
		private void sendDriveSteering()
		{
			sendCommand(String.Format("D{0:3} {0:3}", steering.Clamp(135, 165), driving.Clamp(135, 165)));
		}
		private void sendStatusRequest()
		{
			sendCommand("S");
		}
		#endregion
	}
}
