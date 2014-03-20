using System;
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace EV2020.Director
{
	public class Controller
	{
		public const int BatteryThreshold = 12000;
		public const int DrivingMin = -15;
		public const int DrivingMax = 15;
		public const int SteeringMin = -50;
		public const int SteeringMax = 50;
		public const int SteeringNeutral = 153;
		public const int DrivingNeutral = 150;
		public const int CollisionThreshold = 60;
		public const int TimerPeriod = 100;
		public const int BatteryHistoryPoints = 600;
		public const int DistanceHistoryPoints = 300;
		public const int ControlHistoryPoints = 300;

		private Observer observer;

		private bool _isReceivingLine = false;
		private int _linesReceived = 0;
		private StringBuilder _receivingBuffer;
		private Stopwatch _replyStopwatch;
		private long _lastPing = -1;
		public long LastPing
		{
			get { return _lastPing; }
		}
		private Sequence outputBatteryVoltageHistory = new Sequence(BatteryHistoryPoints);
		public Sequence OutputBatteryVoltageHistory
		{
			get { return outputBatteryVoltageHistory; }
		}
		private OutputSequence distanceHistory = new OutputSequence(DistanceHistoryPoints);
		public OutputSequence DistanceHistory
		{
			get { return distanceHistory; }
		}
		private OutputSequence controlHistory = new OutputSequence(ControlHistoryPoints);
		public OutputSequence ControlHistory
		{
			get { return controlHistory; }
		}
		private InputSequence _fixedInputSequence;
		public long FixedInputSequenceExecutingIndex
		{
			get { return _fixedInputSequence.Index; }
		}
		public long FixedInputSequenceExecutingLength
		{
			get { return _fixedInputSequence.Length; }
		}
		private bool _fixedInputSequenceExecuting = false;
		public bool IsFixedInputSequenceExecuting
		{
			get { return _fixedInputSequenceExecuting; }
		}

		private bool _emergencyStop = false;
		public bool IsEmergencyStop
		{
			get { return _emergencyStop; }
		}
		private int _emergencyErrorCount = 0;
		public int EmergencyErrorCount
		{
			get { return _emergencyErrorCount; }
		}
		private int _emergencyErrorCountThreshold = 2;

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
		public int AudioState
		{
			get { return audioState; }
		}

		private int currentDriving = 0;
		public int CurrentDriving
		{
			get { return currentDriving; }
		}
		private int currentSteering = 0;
		public int CurrentSteering
		{
			get { return currentSteering; }
		}
		private int currentLeftDistance = 0;
		public int CurrentLeftDistance
		{
			get { return currentLeftDistance; }
		}
		private int currentRightDistance = 0;
		public int CurrentRightDistance
		{
			get { return currentRightDistance; }
		}
		private int currentBatteryVoltage = 0;
		public int CurrentBatteryVoltage
		{
			get { return currentBatteryVoltage; }
		}
		private int currentAudioState = 0;
		public int CurrentAudioState
		{
			get { return currentAudioState; }
		}

		public Controller()
		{
			Data.com.SerialDataEvent += com_SerialDataEvent;
			sendInstructions = new Timer(TimerPeriod);
			sendInstructions.Elapsed+=sendInstructions_Elapsed;
			sendInstructions.Start();
			observer = new Observer();
			_receivingBuffer = new StringBuilder();
			_replyStopwatch = new Stopwatch();
		}

		void sendInstructions_Elapsed(object sender, ElapsedEventArgs e)
		{
			//TODO figure out audio.	
			if (_fixedInputSequenceExecuting)
			{
				if (_fixedInputSequence.IsEndOfSignal) 
					_fixedInputSequenceExecuting = false;
				_fixedInputSequence.Forward();				
			}
			Data.db.UpdateProperty("FixedInputSequence");
			//Add data to graph
			outputBatteryVoltageHistory.AddToFront((double)currentBatteryVoltage / 1000.0);
			Data.db.UpdateProperty("BatteryGraphPoints");
			distanceHistory.AddToFront(currentLeftDistance, currentRightDistance);
			Data.db.UpdateProperty("DistanceLeftGraphPoints");
			Data.db.UpdateProperty("DistanceRightGraphPoints");
			//controlHistory.AddToFront(currentDriving, currentSteering);
			controlHistory.AddToFront(driving, steering);
			Data.db.UpdateProperty("ControlDrivingGraphPoints");
			Data.db.UpdateProperty("ControlSteeringGraphPoints");
			sendDriveSteering();
			sendStatusRequest();
			if (Data.vis != null)
				Data.vis.drawJoystick();
		}

		void com_SerialDataEvent(object sender, Communication.SerialDataEventArgs e)
		{			
			foreach (char c in e.Data)
			{
				if (!_isReceivingLine)
				{
					if (c == 4)
					{
						//Block done (= EOT).
						_replyStopwatch.Stop();
						_lastPing = _replyStopwatch.ElapsedMilliseconds;
						Data.db.UpdateProperty("LastPing");
						processReply(_receivingBuffer.ToString());
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
						_receivingBuffer.Clear();
						_linesReceived++;
					}
				}
			}
		}

		void processReply(String line)
		{
			if (line == String.Empty)
					return;
			if (line.Length < 5)
			{
				return;
			}
			String linet = line.Trim('%');
			if (line.Length < 6)
			{
				return;
			}
			try
			{
				//TODO
				if (linet.Substring(0, 2) == "Dr")
				{
					//Driving info (150 is neutral)

					if (!Int32.TryParse(line.Substring(7), out currentDriving))
					{
						//TODO handle (bogus data)
					}
					else
					{
						currentDriving -= DrivingNeutral;
					}
					if (currentDriving - DrivingNeutral != driving)
					{
						//TODO handle ('D' command not accepted)
					}
				}
				else if (linet.Substring(0, 2) == "L/")
				{
					//Steering info (150 is neutral)
					if (!Int32.TryParse(line.Substring(5), out currentSteering))
					{
						//TODO handle (bogus data)
					}
					else
					{
						currentSteering -= SteeringNeutral;
					}
					if (currentSteering - SteeringNeutral != steering)
					{
						//TODO handle ('D' command not accepted)
					}
				}
				else
					if (line.Substring(0, 1) == "D")
					{
						string[] tmps = line.Substring(1).Split(new string[] { " " }, 2, StringSplitOptions.RemoveEmptyEntries);
						//Steering/Driving info (150 is neutral)
						if (!Int32.TryParse(tmps[0], out currentSteering))
						{
							//TODO handle (bogus data)
						}
						else
						{
							currentSteering -= SteeringNeutral;
						}
						if (!Int32.TryParse(tmps[1], out currentDriving))
						{
							//TODO handle (bogus data)
						}
						else
						{
							currentDriving -= DrivingNeutral;
						}
						if (currentSteering - SteeringNeutral != steering)
						{
							//TODO handle ('D' command not accepted)
						}
						if (currentDriving - DrivingNeutral != driving)
						{
							//TODO handle ('D' command not accepted)
						}
						tmps = null;
					}
					else if (line.Substring(0, 1) == "U")
					{
						//Ultrasonic sendor data (cm)
						string[] tmps = line.Substring(1).Split(new string[] { " " }, 2, StringSplitOptions.RemoveEmptyEntries);
						if (!Int32.TryParse(tmps[0], out currentLeftDistance))
						{
							//TODO handle (bogus data)
						}
						if (!Int32.TryParse(tmps[1], out currentRightDistance))
						{
							//TODO handle (bogus data)
						}
						if (currentLeftDistance == 999)
						{
							//TODO handle (too far)
							currentLeftDistance = 300;
						}
						else
						{
							//TODO handle (proper data)	
						}
						if (currentRightDistance == 999)
						{
							//TODO handle (too far)
							currentRightDistance = 300;
						}
						else
						{
							//TODO handle (proper data)
						}
						
						if ((currentRightDistance < CollisionThreshold || currentLeftDistance < CollisionThreshold)&&currentDriving>0)
						{							
							_emergencyErrorCount++;
							if (_emergencyErrorCount > _emergencyErrorCountThreshold)
							{
								_emergencyStop = true;
								Brake();
								Center();
							}								
							Data.db.UpdateProperty("EmergencyStop");
						}
						else
						{
							if (_emergencyErrorCount > 0)
							{
								_emergencyErrorCount = 0;
								Data.db.UpdateProperty("EmergencyStop");
							}
						}

						// Update the observer, to brake OR just update the current velocity
						int brakeDriving = observer.Update(currentLeftDistance < currentRightDistance ? currentLeftDistance : currentRightDistance, driving);
						if (_emergencyStop)
							driving = brakeDriving;

						tmps = null;
					}
					else if (line.Substring(0, 2) == "Au")
					{
						//Audio beacon status
						if (!Int32.TryParse(line.Substring(6, 1), out currentAudioState))
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
						if (!Int32.TryParse(line.Substring(2), out currentBatteryVoltage))
						{
							//TODO handle (bogus data)
						}
						else
						{
							
						}
						if (currentBatteryVoltage < BatteryThreshold)
						{
							//TODO hanlde voltage too low
						}
					}
			}
			catch
			{
				System.Diagnostics.Debug.WriteLine(String.Format("Exception on line; tline: {0}; {1}", line, linet));
				Stop();
				Center();
			}
		}

		#region Private command methods
		private void sendCommand(String line)
		{
			if (Data.com == null)
				return;
			Data.com.WriteLine(line);
			_replyStopwatch = Stopwatch.StartNew();
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

			if (!_fixedInputSequenceExecuting)
			{
				// Already handled on U message (parking sensor) receive
				/*if (_emergencyStop)				
				{
					driving = 0;
					steering = 0;					
				}*/
			}
			else
			{
				driving = (int)Math.Round(_fixedInputSequence.GetCurrentDriving());
				steering = (int)Math.Round(_fixedInputSequence.GetCurrentSteering());				
			}
			sendCommand(String.Format("D{0} {1}", steering + SteeringNeutral, driving + DrivingNeutral));
		}
		private void sendStatusRequest()
		{
			sendCommand("S");
		}
		#endregion

		#region Public command methods
		public void SetDrivingSteering(int d, int s){
			if (!_fixedInputSequenceExecuting)
			{
				driving = d.Clamp(DrivingMin, DrivingMax);
				steering = s.Clamp(SteeringMin, SteeringMax);				
			}
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
		public void Brake()
		{
			/*if (currentDriving != 0)
			{
				System.Diagnostics.Debug.WriteLine("Braking with: {0}", -currentDriving);
				driving = 0;
				if (_fixedInputSequenceExecuting)
					StopFixedInputSequence();
				InputSequence fis = new InputSequence( Sequence.Pulse(20, 0, 10, (1.5*-currentDriving).Clamp(-DrivingMax, DrivingMax), 0),new Sequence(20));
				StartFixedInputSequence(ref fis);
			}
			else
			{
				driving = 0;
			}*/
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

		public void ResetEmergencyStop()
		{
			_emergencyErrorCount = 0;
			_emergencyStop = false;
			Data.db.UpdateProperty("EmergencyStop");			
		}
		public void StartFixedInputSequence(ref InputSequence fis)
		{
			_fixedInputSequence = fis;
			_fixedInputSequenceExecuting = true;
		}
		public void StopFixedInputSequence()
		{			
			_fixedInputSequenceExecuting = false;
		}
		#endregion
	}
}
