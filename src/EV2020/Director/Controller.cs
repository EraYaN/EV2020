using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EV2020.Director
{
	public class Controller
	{
		const int BatteryThreshold = 15000;
		const int DrivingMin = -15;
		const int DrivingMax = 15;
		const int SteeringMin = -50;
		const int SteeringMax = 50;
		const int SteeringNeutral = 150;
		const int DrivingNeutral = 150;
		const int CollisionThreshold = 100;

		private bool _isWaitingForReply = false;
		private bool _isReceivingLine = false;
		private int _linesReceived = 0;
		private StringBuilder _receivingBuffer;
		private Stopwatch _replyStopwatch;
		private long _lastPing = -1;

		Timer sendInstructions;

		private int driving = 0;
		private int steering = 0;
		private int audioState = 0;

		public int Driving
		{
			get { return driving; }
		}
		public int Steering
		{
			get { return steering; }
		}

		private int currentDriving = 0;
		private int currentSteering = 0;
		private int currentLeftDist = 0;
		private int currentRightDist = 0;
		private int currentBatteryVoltage = 0;
		private int currentAudioState = 0;

		public long LastPing
		{
			get { return _lastPing; }
		}

		public Controller()
		{
			Data.com.SerialDataEvent += com_SerialDataEvent;
			sendInstructions = new Timer(1000);
			sendInstructions.Elapsed+=sendInstructions_Elapsed;
			sendInstructions.Start();
		}

		void sendInstructions_Elapsed(object sender, ElapsedEventArgs e)
		{
			//TODO figure out audio.			
			sendDriveSteering();
			sendStatusRequest();
		}

		void com_SerialDataEvent(object sender, Communication.SerialDataEventArgs e)
		{
			if (!_isWaitingForReply)
				return;
			foreach (char c in e.Data)
			{
				if (!_isReceivingLine)
				{
					_receivingBuffer = new StringBuilder();
					if (c == 4)
					{
						//Block done (= EOT).
						_replyStopwatch.Stop();
						_lastPing = _replyStopwatch.ElapsedMilliseconds;
						Data.db.UpdateProperty("LastPing");
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
		}

		void processReply(String line)
		{
			//TODO
			System.Diagnostics.Debug.WriteLine("Line received: {0}", line);
			if (line == String.Empty)
				return;
			if (line.Substring(0, 1) == "D")
			{
				//Steering/Driving info (150 is neutral)
				if(!Int32.TryParse(line.Substring(1, 3), out currentSteering)){
					//TODO handle (bogus data)
				}
				if(!Int32.TryParse(line.Substring(5, 3), out currentDriving)){
					//TODO handle (bogus data)
				}
				if (currentSteering - SteeringNeutral != steering)
				{
					//TODO handle ('D' command not accepted)
				}
				if (currentDriving - DrivingNeutral != driving)
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
				else
				{
					//TODO handle (proper data)
					
				}
				if (currentRightDist == 999)
				{
					//TODO handle (too far)
				}
				else
				{
					//TODO handle (proper data)
				}
				if (currentRightDist < CollisionThreshold || currentLeftDist < CollisionThreshold)
				{
					Stop();
					Center();
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
		private void sendCommand(String line, bool waitForReply = false)
		{
			if (Data.com == null)
				return;
			Data.com.WriteLine(line);
			if (waitForReply)
			{
				_isWaitingForReply = true;
				_replyStopwatch = Stopwatch.StartNew();				
			}
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
			sendCommand(String.Format("D{0} {1}", steering + SteeringNeutral, driving + DrivingNeutral));
		}
		private void sendStatusRequest()
		{
			sendCommand("S", true);
		}
		#endregion

		#region Public command methods
		public void SetDrivingSteering(int d, int s){
			driving = d.Clamp(DrivingMin, DrivingMax);
			steering = s.Clamp(SteeringMin,SteeringMax);
			sendDriveSteering();
		}
		public void SetDriving(int d){
			driving = d;
			sendDriveSteering();
		}
		public void SetSteering(int s){
			steering = s;
			sendDriveSteering();
		}
		public void Stop()
		{
			driving = 0;
			sendDriveSteering();
		}
		public void Faster(int amount = 1)
		{
			driving+=amount;
			driving=driving.Clamp(DrivingMin, DrivingMax);
		}
		public void Slower(int amount = 1)
		{
			driving -= amount; 
			driving=driving.Clamp(DrivingMin, DrivingMax);
		}
		public void Left(int amount = 1)
		{
			steering += amount; 
			steering=steering.Clamp(SteeringMin, SteeringMax);
		}
		public void Right(int amount = 1)
		{
			steering -= amount; 
			steering=steering.Clamp(SteeringMin, SteeringMax);
		}
		public void Center()
		{
			steering = 0;
		}
		public void GetStatus()
		{
			sendStatusRequest();
		}
		#endregion
	}
}
