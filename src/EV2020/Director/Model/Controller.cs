using System;
using System.Diagnostics;
using System.Text;
using System.Timers;
using MathNet.Numerics.LinearAlgebra.Double;

namespace EV2020.Director
{
	/// <summary>
	/// The class that actually handles the in and output to the car.
	/// </summary>
	public class Controller
	{
		public const int BatteryThreshold = 12000;
		public const int DrivingMin = -15;
		public const int DrivingMax = 15;
		public const int SteeringMin = -50;
		public const int SteeringMax = 50;
		public const int SteeringNeutral = 153;
		public const int DrivingNeutral = 150;
		public const int DrivingOffsetUp = 3;
		public const int DrivingOffsetDown = -6;
		public const int CollisionThreshold = 0;
		public const int TimerPeriod = 100;
		public const int BatteryHistoryPoints = 600;
		public const int DistanceHistoryPoints = 300;
		public const int ControlHistoryPoints = 300;

		private bool _isReceivingLine = false;
		private int _linesReceived = 0;
		private StringBuilder _receivingBuffer;
		private Stopwatch _replyStopwatch;
		private long _lastPing = -1;
		private bool replyTimerRunning = false;
		private ControllerState state;
		private int _emergencyErrorCountThreshold = 2;
		Timer sendInstructions;
		private int driving = 0;
		private int steering = 0;
		private int audioState = 0;
		#region Public properties
		public ControllerState State
		{
			get
			{
				return state;
			}
			set
			{
				if (value != state)
				{
					state = value;
					Data.db.UpdateProperty("ControllerState");
				}
			}
		}

		/// <summary>
		/// The last measured delay in the communication with the car.
		/// </summary>
		public long LastPing
		{
			get { return _lastPing; }
		}
		private Sequence outputBatteryVoltageHistory = new Sequence(BatteryHistoryPoints);
		/// <summary>
		/// This is the sequence containing the battery history
		/// </summary>
		public Sequence OutputBatteryVoltageHistory
		{
			get { return outputBatteryVoltageHistory; }
		}
		private OutputSequence distanceHistory = new OutputSequence(DistanceHistoryPoints);
		/// <summary>
		/// This is the sequence containing the distance (ultrasound sensor) history
		/// </summary>
		public OutputSequence DistanceHistory
		{
			get { return distanceHistory; }
		}
		private OutputSequence controlHistory = new OutputSequence(ControlHistoryPoints);
		/// <summary>
		/// This is the sequence containing the control signals history
		/// </summary>
		public OutputSequence ControlHistory
		{
			get { return controlHistory; }
		}
		private InputSequence _fixedInputSequence;
		/// <summary>
		/// The current index in the fixed input sequence
		/// </summary>
		public long FixedInputSequenceExecutingIndex
		{
			get { return _fixedInputSequence.Index; }
		}
		/// <summary>
		/// The length of the current fixed input sequence
		/// </summary>
		public long FixedInputSequenceExecutingLength
		{
			get { return _fixedInputSequence.Length; }
		}
		private bool _fixedInputSequenceExecuting = false;
		/// <summary>
		/// Gets the fixed input sequence status
		/// </summary>
		public bool IsFixedInputSequenceExecuting
		{
			get { return _fixedInputSequenceExecuting; }
		}
		/// <summary>
		/// Gets the emergency stop status 
		/// </summary>
		public bool IsEmergencyStop
		{
			get { return state == ControllerState.EmergencyStop; }
		}
		private int _emergencyErrorCount = 0;
		/// <summary>
		/// The amount of emergency stop errors.
		/// </summary>
		public int EmergencyErrorCount
		{
			get { return _emergencyErrorCount; }
		}
		/// <summary>
		/// The driving signal
		/// </summary>
		public int Driving
		{
			get { return driving; }
		}
		/// <summary>
		/// The steering signal
		/// </summary>
		public int Steering
		{
			get { return steering; }
		}
		/// <summary>
		/// The audio beacon state
		/// </summary>
		public int AudioState
		{
			get { return audioState; }
		}

		private int currentDriving = 0;
		/// <summary>
		/// The current driving signal
		/// </summary>
		public int CurrentDriving
		{
			get { return currentDriving; }
		}
		private int currentSteering = 0;
		/// <summary>
		/// The current steering signal
		/// </summary>
		public int CurrentSteering
		{
			get { return currentSteering; }
		}
		private int currentLeftDistance = 0;
		/// <summary>
		/// The current left sensor distance
		/// </summary>
		public int CurrentLeftDistance
		{
			get { return currentLeftDistance; }
		}
		private int currentRightDistance = 0;
		/// <summary>
		/// The current right sensor distance
		/// </summary>
		public int CurrentRightDistance
		{
			get { return currentRightDistance; }
		}
		private int currentBatteryVoltage = 0;
		/// <summary>
		/// The current battery voltage
		/// </summary>
		public int CurrentBatteryVoltage
		{
			get { return currentBatteryVoltage; }
		}
		private int currentAudioState = 0;
		/// <summary>
		/// The current audio beacon state
		/// </summary>
		public int CurrentAudioState
		{
			get { return currentAudioState; }
		}	
		#endregion
		/// <summary>
		/// Initializes the Controller class
		/// </summary>
		public Controller()
		{
			Data.com.SerialDataEvent += com_SerialDataEvent;
			sendInstructions = new Timer(TimerPeriod);
			sendInstructions.Elapsed+=sendInstructions_Elapsed;
			sendInstructions.Start();
			_receivingBuffer = new StringBuilder();
			_replyStopwatch = new Stopwatch();
			state = ControllerState.Idle;
		}
		#region Internal Processing methods
		void sendInstructions_Elapsed(object sender, ElapsedEventArgs e)
		{
			//Check if we are sending a predefined sequence.
			if (state == ControllerState.Navigating)
			{
				if (_fixedInputSequenceExecuting)
				{
					if (_fixedInputSequence.IsEndOfSignal)
						_fixedInputSequenceExecuting = false;
					//advace the pointer in the sequence
					_fixedInputSequence.Forward();
					//Set the current values for sending to car.
					SetDrivingSteering((int)Math.Round(_fixedInputSequence.GetCurrentDriving()), (int)Math.Round(_fixedInputSequence.GetCurrentSteering()));
				}
				else
				{
					if (Data.nav != null)
					{
						CarCommand command = Data.nav.Tick(currentLeftDistance, currentRightDistance);

						//Set Driving and Steering for sending to car.
						SetDrivingSteering((int)Math.Round(command.Driving), (int)Math.Round(command.Steering));
					}

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
			}
			else if (state == ControllerState.Charging)
			{
				if (currentBatteryVoltage > Data.cfg.ChargeVoltageThreshold*1000)
				{
					double distance = 1;
					//Set target to one meter in front of the car.
					Data.nav.GoToPosition(DenseVector.OfArray(new double[] { Data.nav.CarX + Math.Cos(Data.nav.CarBearing) * distance, Data.nav.CarY + Math.Sin(Data.nav.CarBearing) * distance }));
					//Set public property to update UI
					State = ControllerState.Navigating;
				}
			}
			else if (state == ControllerState.EmergencyStop)
			{
				//nothing
			}
			else if (state == ControllerState.Error)
			{
				//nothing
			}
			else if (state == ControllerState.Unknown)
			{
				//nothing
			}
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
						if (replyTimerRunning)
						{
							_replyStopwatch.Stop();
							replyTimerRunning = false;
							_lastPing =(long)Math.Round((_lastPing*2.0+(double)_replyStopwatch.ElapsedMilliseconds)/3);
							Data.db.UpdateProperty("LastPing");
						}
						
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
						if (currentLeftDistance > 300) //instead of == 999
						{
							//TODO handle (too far)
							currentLeftDistance = 300;
						}
						else
						{
							//TODO handle (proper data)	
						}
						if (currentRightDistance > 300) //instead of == 999
						{
							//TODO handle (too far)
							currentRightDistance = 300;
						}
						else
						{
							//TODO handle (proper data)
						}

						CheckEmergencyStop();
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
							//TODO handle voltage too low
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
		#endregion

		#region Private command methods
		private void CheckEmergencyStop()
		{
			if ((currentRightDistance < CollisionThreshold || currentLeftDistance < CollisionThreshold) && currentDriving > 0)
			{
				_emergencyErrorCount++;
				if (_emergencyErrorCount > _emergencyErrorCountThreshold)
				{
					state = ControllerState.EmergencyStop;
					Data.db.UpdateProperty("EmergencyStop");
					//Set current position as target, makes the car stop.
					Data.nav.GoToPosition(Data.nav.CarPos);
					Brake();
					Center();
				}
			}
			else
			{
				if (_emergencyErrorCount > 0)
				{
					_emergencyErrorCount = 0;
					Data.db.UpdateProperty("EmergencyStop");
				}
			}
		}
		private void sendCommand(String line)
		{
			if (Data.com == null)
				return;
			Data.com.WriteLine(line);
			if (!replyTimerRunning)
			{
				_replyStopwatch = Stopwatch.StartNew();
				replyTimerRunning = true;
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
			if (IsEmergencyStop)				
			{
				driving = 0;
				steering = 0;					
			}
			//Send the sotred values to the car.
			sendCommand(String.Format("D{0} {1}", steering + SteeringNeutral, driving + DrivingNeutral));
		}
		private void sendStatusRequest()
		{
			sendCommand("S");
		}
		#endregion

		#region Public command methods
		/// <summary>
		/// Toggles the Audio beacon state
		/// </summary>
		public void ToggleAudio()
		{
			if (audioState == 0)
			{
				enableAudio();
			}
			else
			{
				disableAudio();
			}
		}
		/// <summary>
		/// Sets the new driving and steering signals
		/// </summary>
		/// <param name="_driving">The driving signal</param>
		/// <param name="_steering">The steering signal</param>
		public void SetDrivingSteering(int _driving, int _steering){
			if (!_fixedInputSequenceExecuting)
			{
				//anti deadzone
				if (_driving > 0)
					_driving += DrivingOffsetUp;
				else if (_driving < 0)
					_driving -= DrivingOffsetDown;
				driving = _driving.Clamp(DrivingMin, DrivingMax);
				steering = _steering.Clamp(SteeringMin, SteeringMax);				
			}
		}
		/// <summary>
		/// Sets the new driving signal
		/// </summary>
		/// <param name="_driving">The driving signal</param>
		public void SetDriving(int _driving){
			//Anti Deadzone	of the car.		
			if (_driving > 0)
				_driving += DrivingOffsetUp;
			else if (_driving < 0)
				_driving += DrivingOffsetDown;
		
			driving = _driving.Clamp(DrivingMin, DrivingMax);
			sendDriveSteering();
		}
		/// <summary>
		/// Sets the new steering signal
		/// </summary>
		/// <param name="_steering">The steering signal</param>
		public void SetSteering(int _steering){
			steering = _steering.Clamp(SteeringMin, SteeringMax);
			sendDriveSteering();
		}
		/// <summary>
		/// Stops the car.
		/// </summary>
		public void Stop()
		{
			driving = 0;
			sendDriveSteering();
		}
		/// <summary>
		/// Not Implemented, maybe later
		/// </summary>
		public void Brake()
		{
			//Disabled.
			/*if (currentDriving != 0)
			{
				System.Diagnostics.Debug.WriteLine("Braking with: {0}", -currentDriving);
				driving = 0;
				if (_fixedInputSequenceExecuting)
					StopFixedInputSequence();
				InputSequence FixedInputSequence = new InputSequence( Sequence.Pulse(20, 0, 10, (1.5*-currentDriving).Clamp(-DrivingMax, DrivingMax), 0),new Sequence(20));
				StartFixedInputSequence(ref FixedInputSequence);
			}
			else
			{
				driving = 0;
			}*/
		}
		/// <summary>
		/// Makes the current driving signal higher
		/// </summary>
		/// <param name="amount">The amount</param>
		public void Faster(int amount = 1)
		{
			driving+=amount;
			driving=driving.Clamp(DrivingMin, DrivingMax);
		}
		/// <summary>
		/// Makes the current driving signal lower
		/// </summary>
		/// <param name="amount">The amount</param>
		public void Slower(int amount = 1)
		{
			driving -= amount; 
			driving=driving.Clamp(DrivingMin, DrivingMax);
		}
		/// <summary>
		/// Makes the current steering signal higher
		/// </summary>
		/// <param name="amount">The amount</param>
		public void Left(int amount = 1)
		{
			steering += amount; 
			steering=steering.Clamp(SteeringMin, SteeringMax);
		}
		/// <summary>
		/// Makes the current steering signal lower
		/// </summary>
		/// <param name="amount">The amount</param>
		public void Right(int amount = 1)
		{
			steering -= amount; 
			steering=steering.Clamp(SteeringMin, SteeringMax);
		}
		/// <summary>
		/// Centers the steering signal
		/// </summary>
		public void Center()
		{
			steering = 0;
		}
		/// <summary>
		/// Retrieves the status
		/// </summary>
		public void GetStatus()
		{
			sendStatusRequest();
		}
		/// <summary>
		/// Resets the emergency stop
		/// </summary>
		public void ResetEmergencyStop()
		{
			_emergencyErrorCount = 0;
			State = ControllerState.Idle;
			Data.db.UpdateProperty("EmergencyStop");			
		}
		/// <summary>
		/// Starts a fixed input sequence
		/// </summary>
		/// <param name="FixedInputSequence">The sequence to be played back to the car.</param>
		public void StartFixedInputSequence(ref InputSequence FixedInputSequence)
		{
			_fixedInputSequence = FixedInputSequence;
			_fixedInputSequenceExecuting = true;
		}
		/// <summary>
		/// Halt the current fixed input sequence.
		/// </summary>
		public void StopFixedInputSequence()
		{			
			_fixedInputSequenceExecuting = false;
		}
		#endregion

	}
}
