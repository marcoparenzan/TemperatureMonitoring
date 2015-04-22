using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using ThermostatCore.Common;

namespace ThermostatCore.ViewModels
{
	#region Class ThermostatViewModel

	public partial class ThermostatViewModel: DataViewModel
    {
			
			#region Local Command Execute
			
			public partial class ExecuteArgs: BaseArgs
			{
			}
			
			public ExecuteArgs ExecuteCommandArgs
			{
				get
				{
					ExecuteArgs args = new ExecuteArgs {
					};
					return args;
				}
			}

			partial void OnExecute(ExecuteArgs args);

			partial void OnExecuteEnable(Action<bool> enableHandler);

			private RelayCommand<ExecuteArgs> _execute; // ICommand

			public RelayCommand<ExecuteArgs> Execute // ICommand
			{
				get
				{
					if (_execute == null)
					{
						_execute = new RelayCommand<ExecuteArgs>(new Action<ExecuteArgs>(
							_ =>
							{
								OnExecute(_);
							}
						),
						_ => {
							bool enabled = true;
							OnExecuteEnable(enable => {
								enabled = enable;
							});
							return enabled;
						});
					}
					return _execute;
				}
			}
			
			#endregion		
					
			
			#region Local Command Commit
			
			public partial class CommitArgs: BaseArgs
			{
			}
			
			public CommitArgs CommitCommandArgs
			{
				get
				{
					CommitArgs args = new CommitArgs {
					};
					return args;
				}
			}

			partial void OnCommit(CommitArgs args);

			partial void OnCommitEnable(Action<bool> enableHandler);

			private RelayCommand<CommitArgs> _commit; // ICommand

			public RelayCommand<CommitArgs> Commit // ICommand
			{
				get
				{
					if (_commit == null)
					{
						_commit = new RelayCommand<CommitArgs>(new Action<CommitArgs>(
							_ =>
							{
								OnCommit(_);
							}
						),
						_ => {
							bool enabled = true;
							OnCommitEnable(enable => {
								enabled = enable;
							});
							return enabled;
						});
					}
					return _commit;
				}
			}
			
			#endregion		
				
			
			#region Local Command PowerOn
			
			public partial class PowerOnArgs: BaseArgs
			{
			}
			
			public PowerOnArgs PowerOnCommandArgs
			{
				get
				{
					PowerOnArgs args = new PowerOnArgs {
					};
					return args;
				}
			}

			partial void OnPowerOn(PowerOnArgs args);

			partial void OnPowerOnEnable(Action<bool> enableHandler);

			private RelayCommand<PowerOnArgs> _powerOn; // ICommand

			public RelayCommand<PowerOnArgs> PowerOn // ICommand
			{
				get
				{
					if (_powerOn == null)
					{
						_powerOn = new RelayCommand<PowerOnArgs>(new Action<PowerOnArgs>(
							_ =>
							{
								OnPowerOn(_);
							}
						),
						_ => {
							bool enabled = true;
							OnPowerOnEnable(enable => {
								enabled = enable;
							});
							return enabled;
						});
					}
					return _powerOn;
				}
			}
			
			#endregion		
				
			#region Local Command PowerOff
			
			public partial class PowerOffArgs: BaseArgs
			{
			}
			
			public PowerOffArgs PowerOffCommandArgs
			{
				get
				{
					PowerOffArgs args = new PowerOffArgs {
					};
					return args;
				}
			}

			partial void OnPowerOff(PowerOffArgs args);

			partial void OnPowerOffEnable(Action<bool> enableHandler);

			private RelayCommand<PowerOffArgs> _powerOff; // ICommand

			public RelayCommand<PowerOffArgs> PowerOff // ICommand
			{
				get
				{
					if (_powerOff == null)
					{
						_powerOff = new RelayCommand<PowerOffArgs>(new Action<PowerOffArgs>(
							_ =>
							{
								OnPowerOff(_);
							}
						),
						_ => {
							bool enabled = true;
							OnPowerOffEnable(enable => {
								enabled = enable;
							});
							return enabled;
						});
					}
					return _powerOff;
				}
			}
			
			#endregion		
				
			#region Local Command HigherTemp
			
			public partial class HigherTempArgs: BaseArgs
			{
			}
			
			public HigherTempArgs HigherTempCommandArgs
			{
				get
				{
					HigherTempArgs args = new HigherTempArgs {
					};
					return args;
				}
			}

			partial void OnHigherTemp(HigherTempArgs args);

			partial void OnHigherTempEnable(Action<bool> enableHandler);

			private RelayCommand<HigherTempArgs> _higherTemp; // ICommand

			public RelayCommand<HigherTempArgs> HigherTemp // ICommand
			{
				get
				{
					if (_higherTemp == null)
					{
						_higherTemp = new RelayCommand<HigherTempArgs>(new Action<HigherTempArgs>(
							_ =>
							{
								OnHigherTemp(_);
							}
						),
						_ => {
							bool enabled = true;
							OnHigherTempEnable(enable => {
								enabled = enable;
							});
							return enabled;
						});
					}
					return _higherTemp;
				}
			}
			
			#endregion		
				
			#region Local Command LowerTemp
			
			public partial class LowerTempArgs: BaseArgs
			{
			}
			
			public LowerTempArgs LowerTempCommandArgs
			{
				get
				{
					LowerTempArgs args = new LowerTempArgs {
					};
					return args;
				}
			}

			partial void OnLowerTemp(LowerTempArgs args);

			partial void OnLowerTempEnable(Action<bool> enableHandler);

			private RelayCommand<LowerTempArgs> _lowerTemp; // ICommand

			public RelayCommand<LowerTempArgs> LowerTemp // ICommand
			{
				get
				{
					if (_lowerTemp == null)
					{
						_lowerTemp = new RelayCommand<LowerTempArgs>(new Action<LowerTempArgs>(
							_ =>
							{
								OnLowerTemp(_);
							}
						),
						_ => {
							bool enabled = true;
							OnLowerTempEnable(enable => {
								enabled = enable;
							});
							return enabled;
						});
					}
					return _lowerTemp;
				}
			}
			
			#endregion		
				
			#region Local Command Exit
			
			public partial class ExitArgs: BaseArgs
			{
			}
			
			public ExitArgs ExitCommandArgs
			{
				get
				{
					ExitArgs args = new ExitArgs {
					};
					return args;
				}
			}

			partial void OnExit(ExitArgs args);

			partial void OnExitEnable(Action<bool> enableHandler);

			private RelayCommand<ExitArgs> _exit; // ICommand

			public RelayCommand<ExitArgs> Exit // ICommand
			{
				get
				{
					if (_exit == null)
					{
						_exit = new RelayCommand<ExitArgs>(new Action<ExitArgs>(
							_ =>
							{
								OnExit(_);
							}
						),
						_ => {
							bool enabled = true;
							OnExitEnable(enable => {
								enabled = enable;
							});
							return enabled;
						});
					}
					return _exit;
				}
			}
			
			#endregion		
				
			#region Local Command Reset
			
			public partial class ResetArgs: BaseArgs
			{
			}
			
			public ResetArgs ResetCommandArgs
			{
				get
				{
					ResetArgs args = new ResetArgs {
					};
					return args;
				}
			}

			partial void OnReset(ResetArgs args);

			partial void OnResetEnable(Action<bool> enableHandler);

			private RelayCommand<ResetArgs> _reset; // ICommand

			public RelayCommand<ResetArgs> Reset // ICommand
			{
				get
				{
					if (_reset == null)
					{
						_reset = new RelayCommand<ResetArgs>(new Action<ResetArgs>(
							_ =>
							{
								OnReset(_);
							}
						),
						_ => {
							bool enabled = true;
							OnResetEnable(enable => {
								enabled = enable;
							});
							return enabled;
						});
					}
					return _reset;
				}
			}
			
			#endregion		
			
					#region Temp Property
			
			private decimal _temp;
			
			partial void OnSetTemp(decimal lastTemp, Action<decimal> handleSet);
			partial void OnGetTemp(decimal value);
	
			private void SetTemp(decimal alternateValue)
			{
				_temp = alternateValue;
			}

			public decimal Temp
			{
				get
				{
					OnGetTemp(_temp);
					return _temp;
				}
				
				set
				{
					if (value == _temp) return;
					var lastTemp = _temp;
					_temp = value;
					OnSetTemp(
						lastTemp
						, new Action<decimal>(SetTemp));
					Notify("Temp");
				}
			}
			
			#endregion
						#region TempRef Property
			
			private decimal _tempRef;
			
			partial void OnSetTempRef(decimal lastTempRef, Action<decimal> handleSet);
			partial void OnGetTempRef(decimal value);
	
			private void SetTempRef(decimal alternateValue)
			{
				_tempRef = alternateValue;
			}

			public decimal TempRef
			{
				get
				{
					OnGetTempRef(_tempRef);
					return _tempRef;
				}
				
				set
				{
					if (value == _tempRef) return;
					var lastTempRef = _tempRef;
					_tempRef = value;
					OnSetTempRef(
						lastTempRef
						, new Action<decimal>(SetTempRef));
					Notify("TempRef");
				}
			}
			
			#endregion
						#region Power Property
			
			private bool _power;
			
			partial void OnSetPower(bool lastPower, Action<bool> handleSet);
			partial void OnGetPower(bool value);
	
			private void SetPower(bool alternateValue)
			{
				_power = alternateValue;
			}

			public bool Power
			{
				get
				{
					OnGetPower(_power);
					return _power;
				}
				
				set
				{
					if (value == _power) return;
					var lastPower = _power;
					_power = value;
					OnSetPower(
						lastPower
						, new Action<bool>(SetPower));
					Notify("Power");
				}
			}
			
			#endregion
						#region Resistor Property
			
			private bool _resistor;
			
			partial void OnSetResistor(bool lastResistor, Action<bool> handleSet);
			partial void OnGetResistor(bool value);
	
			private void SetResistor(bool alternateValue)
			{
				_resistor = alternateValue;
			}

			public bool Resistor
			{
				get
				{
					OnGetResistor(_resistor);
					return _resistor;
				}
				
				set
				{
					if (value == _resistor) return;
					var lastResistor = _resistor;
					_resistor = value;
					OnSetResistor(
						lastResistor
						, new Action<bool>(SetResistor));
					Notify("Resistor");
				}
			}
			
			#endregion
			
			}
	
	#endregion
}
