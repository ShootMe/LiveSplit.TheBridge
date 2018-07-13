using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.TheBridge {
	public class SplitterComponent : IComponent {
		public string ComponentName { get { return "The Bridge Autosplitter"; } }
		public TimerModel Model { get; set; }
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		private static string LOGFILE = "_TheBridge.txt";
		private Dictionary<LogObject, string> currentValues = new Dictionary<LogObject, string>();
		private SplitterMemory mem;
		private SplitterSettings settings;
		private int currentSplit = -1, lastLogCheck;
		private bool hasLog = false;
		private long lastVector;
		private PlayerState lastState = PlayerState.Null;
		private LevelTitle lastLevel = LevelTitle.Null;

		public SplitterComponent(LiveSplitState state) {
			mem = new SplitterMemory();
			settings = new SplitterSettings();
			foreach (LogObject key in Enum.GetValues(typeof(LogObject))) {
				currentValues[key] = "";
			}

			if (state != null) {
				Model = new TimerModel() { CurrentState = state };
				Model.InitializeGameTime();
				Model.CurrentState.IsGameTimePaused = true;
				state.OnReset += OnReset;
				state.OnPause += OnPause;
				state.OnResume += OnResume;
				state.OnStart += OnStart;
				state.OnSplit += OnSplit;
				state.OnUndoSplit += OnUndoSplit;
				state.OnSkipSplit += OnSkipSplit;
			}
		}

		public void GetValues() {
			if (!mem.HookProcess()) { return; }

			if (Model != null) {
				HandleSplits();
			}

			LogValues();
		}
		private void HandleSplits() {
			bool shouldSplit = false;

			if (currentSplit == -1) {
				if (settings.Splits.Count != 0) {
					long vector = mem.AverageVector();
					shouldSplit = mem.NextLevel() == LevelTitle.HubWorldConfigurationIntro && mem.PlayerState() == PlayerState.Sleeping && lastVector != vector;
					lastVector = vector;
				} else {
					PlayerState state = mem.PlayerState();
					shouldSplit = lastState == PlayerState.BeingDrawn && state != PlayerState.BeingDrawn && InALevel(mem.NextLevel());
					lastState = state;
				}
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				if (currentSplit < Model.CurrentState.Run.Count && currentSplit < settings.Splits.Count) {
					SplitName split = settings.Splits[currentSplit];
					LevelTitle level = mem.NextLevel();

					switch (split) {
						case SplitName.Level_1_1_Enter: shouldSplit = lastLevel != LevelTitle.World1Level1 && level == LevelTitle.World1Level1; break;
						case SplitName.Level_1_1: shouldSplit = lastLevel == LevelTitle.World1Level1 && level == LevelTitle.World1Level2; break;
						case SplitName.Level_1_2: shouldSplit = lastLevel == LevelTitle.World1Level2 && level == LevelTitle.World1Level3; break;
						case SplitName.Level_1_3: shouldSplit = lastLevel == LevelTitle.World1Level3 && level == LevelTitle.World1Level4; break;
						case SplitName.Level_1_4: shouldSplit = lastLevel == LevelTitle.World1Level4 && level == LevelTitle.World1Level5; break;
						case SplitName.Level_1_5: shouldSplit = lastLevel == LevelTitle.World1Level5 && level == LevelTitle.World1Level6; break;
						case SplitName.Level_1_6: shouldSplit = lastLevel == LevelTitle.World1Level6 && level == LevelTitle.World1HubDone; break;

						case SplitName.Level_2_1_Enter: shouldSplit = lastLevel != LevelTitle.World2Level1 && level == LevelTitle.World2Level1; break;
						case SplitName.Level_2_1: shouldSplit = lastLevel == LevelTitle.World2Level1 && level == LevelTitle.World2Level2; break;
						case SplitName.Level_2_2: shouldSplit = lastLevel == LevelTitle.World2Level2 && level == LevelTitle.World2Level3; break;
						case SplitName.Level_2_3: shouldSplit = lastLevel == LevelTitle.World2Level3 && level == LevelTitle.World2Level4; break;
						case SplitName.Level_2_4: shouldSplit = lastLevel == LevelTitle.World2Level4 && level == LevelTitle.World2Level5; break;
						case SplitName.Level_2_5: shouldSplit = lastLevel == LevelTitle.World2Level5 && level == LevelTitle.World2Level6; break;
						case SplitName.Level_2_6: shouldSplit = lastLevel == LevelTitle.World2Level6 && level == LevelTitle.World2HubDone; break;

						case SplitName.Level_3_1_Enter: shouldSplit = lastLevel != LevelTitle.World3Level1 && level == LevelTitle.World3Level1; break;
						case SplitName.Level_3_1: shouldSplit = lastLevel == LevelTitle.World3Level1 && level == LevelTitle.World3Level2; break;
						case SplitName.Level_3_2: shouldSplit = lastLevel == LevelTitle.World3Level2 && level == LevelTitle.World3Level3; break;
						case SplitName.Level_3_3: shouldSplit = lastLevel == LevelTitle.World3Level3 && level == LevelTitle.World3Level4; break;
						case SplitName.Level_3_4: shouldSplit = lastLevel == LevelTitle.World3Level4 && level == LevelTitle.World3Level5; break;
						case SplitName.Level_3_5: shouldSplit = lastLevel == LevelTitle.World3Level5 && level == LevelTitle.World3Level6; break;
						case SplitName.Level_3_6: shouldSplit = lastLevel == LevelTitle.World3Level6 && level == LevelTitle.World3HubDone; break;

						case SplitName.Level_4_1_Enter: shouldSplit = lastLevel != LevelTitle.World4Level1 && level == LevelTitle.World4Level1; break;
						case SplitName.Level_4_1: shouldSplit = lastLevel == LevelTitle.World4Level1 && level == LevelTitle.World4Level2; break;
						case SplitName.Level_4_2: shouldSplit = lastLevel == LevelTitle.World4Level2 && level == LevelTitle.World4Level3; break;
						case SplitName.Level_4_3: shouldSplit = lastLevel == LevelTitle.World4Level3 && level == LevelTitle.World4Level4; break;
						case SplitName.Level_4_4: shouldSplit = lastLevel == LevelTitle.World4Level4 && level == LevelTitle.World4Level5; break;
						case SplitName.Level_4_5: shouldSplit = lastLevel == LevelTitle.World4Level5 && level == LevelTitle.World4Level6; break;
						case SplitName.Level_4_6: shouldSplit = lastLevel == LevelTitle.World4Level6 && level == LevelTitle.World4HubDone; break;

						case SplitName.Level_End_Normal: shouldSplit = mem.HasWonGame(); break;
						case SplitName.Level_End_Mirror: shouldSplit = mem.InHypercube(); break;
					}

					lastLevel = level;
				} else if (settings.Splits.Count == 0) {
					PlayerState state = mem.PlayerState();
					shouldSplit = state == PlayerState.EnteringDoorway && lastState != PlayerState.EnteringDoorway;
					lastState = state;
				}

				Model.CurrentState.IsGameTimePaused = mem.IsLoading();
			}

			HandleSplit(shouldSplit, settings.Splits.Count == 0 && lastState == PlayerState.WaitingToBeDrawn);
		}
		private bool InALevel(LevelTitle level) {
			switch (level) {
				case LevelTitle.World1Level1:
				case LevelTitle.World1Level2:
				case LevelTitle.World1Level3:
				case LevelTitle.World1Level4:
				case LevelTitle.World1Level5:
				case LevelTitle.World1Level6:
				case LevelTitle.World2Level1:
				case LevelTitle.World2Level2:
				case LevelTitle.World2Level3:
				case LevelTitle.World2Level4:
				case LevelTitle.World2Level5:
				case LevelTitle.World2Level6:
				case LevelTitle.World3Level1:
				case LevelTitle.World3Level2:
				case LevelTitle.World3Level3:
				case LevelTitle.World3Level4:
				case LevelTitle.World3Level5:
				case LevelTitle.World3Level6:
				case LevelTitle.World4Level1:
				case LevelTitle.World4Level2:
				case LevelTitle.World4Level3:
				case LevelTitle.World4Level4:
				case LevelTitle.World4Level5:
				case LevelTitle.World4Level6:
					return true;
			}
			return false;
		}
		private void HandleSplit(bool shouldSplit, bool shouldReset = false) {
			if (shouldReset) {
				if (currentSplit >= 0) {
					Model.Reset();
				}
			} else if (shouldSplit) {
				if (currentSplit == -1) {
					Model.Start();
				} else {
					Model.Split();
				}
			}
		}
		private void LogValues() {
			if (lastLogCheck == 0) {
				hasLog = File.Exists(LOGFILE);
				lastLogCheck = 300;
			}
			lastLogCheck--;

			if (hasLog || !Console.IsOutputRedirected) {
				string prev = string.Empty, curr = string.Empty;
				foreach (LogObject key in Enum.GetValues(typeof(LogObject))) {
					prev = currentValues[key];

					switch (key) {
						case LogObject.CurrentSplit: curr = currentSplit.ToString(); break;
						case LogObject.Pointers: curr = mem.RAMPointers(); break;
						case LogObject.PointerVersion: curr = mem.RAMPointerVersion(); break;
						case LogObject.NextLevel: curr = mem.NextLevel().ToString(); break;
						case LogObject.LastLevel: curr = mem.PreviousLevelCompleted().ToString(); break;
						case LogObject.Loading: curr = mem.IsLoading().ToString(); break;
						case LogObject.PlayerState: curr = mem.PlayerState().ToString(); break;
						case LogObject.WonGame: curr = mem.HasWonGame().ToString(); break;
						case LogObject.DarkWorlds: curr = mem.EnableDarkWorlds().ToString(); break;
						case LogObject.InHypercube: curr = mem.InHypercube().ToString(); break;
						default: curr = string.Empty; break;
					}

					if (string.IsNullOrEmpty(prev)) { prev = string.Empty; }
					if (string.IsNullOrEmpty(curr)) { curr = string.Empty; }
					if (!prev.Equals(curr)) {
						WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + key.ToString() + ": ".PadRight(16 - key.ToString().Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

						currentValues[key] = curr;
					}
				}
			}
		}
		public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
			GetValues();
		}
		public void OnReset(object sender, TimerPhase e) {
			currentSplit = -1;
			lastLevel = LevelTitle.Null;
			Model.CurrentState.IsGameTimePaused = true;
			WriteLog("---------Reset----------------------------------");
		}
		public void OnResume(object sender, EventArgs e) {
			WriteLog("---------Resumed--------------------------------");
		}
		public void OnPause(object sender, EventArgs e) {
			WriteLog("---------Paused---------------------------------");
		}
		public void OnStart(object sender, EventArgs e) {
			currentSplit = 0;
			WriteLog("---------New Game " + Assembly.GetExecutingAssembly().GetName().Version.ToString(3) + "-------------------------");
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			currentSplit--;
			WriteLog("---------Undo-----------------------------------");
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			currentSplit++;
			WriteLog("---------Skip-----------------------------------");
		}
		public void OnSplit(object sender, EventArgs e) {
			currentSplit++;
			WriteLog("---------Split-----------------------------------");
		}
		private void WriteLog(string data) {
			if (hasLog || !Console.IsOutputRedirected) {
				if (Console.IsOutputRedirected) {
					using (StreamWriter wr = new StreamWriter(LOGFILE, true)) {
						wr.WriteLine(data);
					}
				} else {
					Console.WriteLine(data);
				}
			}
		}
		public Control GetSettingsControl(LayoutMode mode) { return settings; }
		public void SetSettings(XmlNode document) { settings.SetSettings(document); }
		public XmlNode GetSettings(XmlDocument document) { return settings.UpdateSettings(document); }
		public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
		public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }
		public float HorizontalWidth { get { return 0; } }
		public float MinimumHeight { get { return 0; } }
		public float MinimumWidth { get { return 0; } }
		public float PaddingBottom { get { return 0; } }
		public float PaddingLeft { get { return 0; } }
		public float PaddingRight { get { return 0; } }
		public float PaddingTop { get { return 0; } }
		public float VerticalHeight { get { return 0; } }
		public void Dispose() { }
	}
}