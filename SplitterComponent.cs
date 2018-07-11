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
		private int currentSplit = -1, lastLogCheck;
		private bool hasLog = false;
		private long lastVector;
		private LevelTitle lastLevel = LevelTitle.Null;

		public SplitterComponent(LiveSplitState state) {
			mem = new SplitterMemory();
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
				long vector = mem.AverageVector();
				shouldSplit = mem.NextLevel() == LevelTitle.HubWorldConfigurationIntro && mem.PlayerState() == PlayerState.Sleeping && lastVector != vector;
				lastVector = vector;
			} else {
				if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
					LevelTitle level = mem.NextLevel();

					if (Model.CurrentState.Run.Count <= 5) {
						shouldSplit = HandleAnyPercentWorlds(level);
					} else if (Model.CurrentState.Run.Count <= 25) {
						shouldSplit = HandleAnyPercent(level);
					} else {
						shouldSplit = HandleAllLevels(level);
					}

					lastLevel = level;
				}

				Model.CurrentState.IsGameTimePaused = mem.IsLoading();
			}

			HandleSplit(shouldSplit, false);
		}
		private bool HandleAnyPercentWorlds(LevelTitle level) {
			if (currentSplit + 1 < Model.CurrentState.Run.Count) {
				switch (lastLevel) {
					case LevelTitle.World1Level6: return level == LevelTitle.World1HubDone;
					case LevelTitle.World2Level6: return level == LevelTitle.World2HubDone;
					case LevelTitle.World3Level6: return level == LevelTitle.World3HubDone;
					case LevelTitle.World4Level6: return level == LevelTitle.World4HubDone;
				}
				return false;
			} else {
				return mem.HasWonGame();
			}
		}
		private bool HandleAnyPercent(LevelTitle level) {
			if (currentSplit + 1 < Model.CurrentState.Run.Count) {
				switch (lastLevel) {
					case LevelTitle.World1Level1: return level == LevelTitle.World1Level2;
					case LevelTitle.World1Level2: return level == LevelTitle.World1Level3;
					case LevelTitle.World1Level3: return level == LevelTitle.World1Level4;
					case LevelTitle.World1Level4: return level == LevelTitle.World1Level5;
					case LevelTitle.World1Level5: return level == LevelTitle.World1Level6;
					case LevelTitle.World1Level6: return level == LevelTitle.World1HubDone;

					case LevelTitle.World2Level1: return level == LevelTitle.World2Level2;
					case LevelTitle.World2Level2: return level == LevelTitle.World2Level3;
					case LevelTitle.World2Level3: return level == LevelTitle.World2Level4;
					case LevelTitle.World2Level4: return level == LevelTitle.World2Level5;
					case LevelTitle.World2Level5: return level == LevelTitle.World2Level6;
					case LevelTitle.World2Level6: return level == LevelTitle.World2HubDone;

					case LevelTitle.World3Level1: return level == LevelTitle.World3Level2;
					case LevelTitle.World3Level2: return level == LevelTitle.World3Level3;
					case LevelTitle.World3Level3: return level == LevelTitle.World3Level4;
					case LevelTitle.World3Level4: return level == LevelTitle.World3Level5;
					case LevelTitle.World3Level5: return level == LevelTitle.World3Level6;
					case LevelTitle.World3Level6: return level == LevelTitle.World3HubDone;

					case LevelTitle.World4Level1: return level == LevelTitle.World4Level2;
					case LevelTitle.World4Level2: return level == LevelTitle.World4Level3;
					case LevelTitle.World4Level3: return level == LevelTitle.World4Level4;
					case LevelTitle.World4Level4: return level == LevelTitle.World4Level5;
					case LevelTitle.World4Level5: return level == LevelTitle.World4Level6;
					case LevelTitle.World4Level6: return level == LevelTitle.World4HubDone;
				}
				return false;
			} else {
				return mem.HasWonGame();
			}
		}
		private bool HandleAllLevels(LevelTitle level) {
			if (currentSplit < 24) {
				switch (lastLevel) {
					case LevelTitle.World1Level1: return level == LevelTitle.World1Level2;
					case LevelTitle.World1Level2: return level == LevelTitle.World1Level3;
					case LevelTitle.World1Level3: return level == LevelTitle.World1Level4;
					case LevelTitle.World1Level4: return level == LevelTitle.World1Level5;
					case LevelTitle.World1Level5: return level == LevelTitle.World1Level6;
					case LevelTitle.World1Level6: return level == LevelTitle.World1HubDone;

					case LevelTitle.World2Level1: return level == LevelTitle.World2Level2;
					case LevelTitle.World2Level2: return level == LevelTitle.World2Level3;
					case LevelTitle.World2Level3: return level == LevelTitle.World2Level4;
					case LevelTitle.World2Level4: return level == LevelTitle.World2Level5;
					case LevelTitle.World2Level5: return level == LevelTitle.World2Level6;
					case LevelTitle.World2Level6: return level == LevelTitle.World2HubDone;

					case LevelTitle.World3Level1: return level == LevelTitle.World3Level2;
					case LevelTitle.World3Level2: return level == LevelTitle.World3Level3;
					case LevelTitle.World3Level3: return level == LevelTitle.World3Level4;
					case LevelTitle.World3Level4: return level == LevelTitle.World3Level5;
					case LevelTitle.World3Level5: return level == LevelTitle.World3Level6;
					case LevelTitle.World3Level6: return level == LevelTitle.World3HubDone;

					case LevelTitle.World4Level1: return level == LevelTitle.World4Level2;
					case LevelTitle.World4Level2: return level == LevelTitle.World4Level3;
					case LevelTitle.World4Level3: return level == LevelTitle.World4Level4;
					case LevelTitle.World4Level4: return level == LevelTitle.World4Level5;
					case LevelTitle.World4Level5: return level == LevelTitle.World4Level6;
					case LevelTitle.World4Level6: return level == LevelTitle.World4HubDone;
				}
				return false;
			} else if (currentSplit == 24) {
				return mem.HasWonGame();
			} else {
				switch (lastLevel) {
					case LevelTitle.World1Level1: return level == LevelTitle.World1Level2;
					case LevelTitle.World1Level2: return level == LevelTitle.World1Level3;
					case LevelTitle.World1Level3: return level == LevelTitle.World1Level4;
					case LevelTitle.World1Level4: return level == LevelTitle.World1Level5;
					case LevelTitle.World1Level5: return level == LevelTitle.World1Level6;
					case LevelTitle.World1Level6: return level == LevelTitle.World1HubDone;

					case LevelTitle.World2Level1: return level == LevelTitle.World2Level2;
					case LevelTitle.World2Level2: return level == LevelTitle.World2Level3;
					case LevelTitle.World2Level3: return level == LevelTitle.World2Level4;
					case LevelTitle.World2Level4: return level == LevelTitle.World2Level5;
					case LevelTitle.World2Level5: return level == LevelTitle.World2Level6;
					case LevelTitle.World2Level6: return level == LevelTitle.World2HubDone;

					case LevelTitle.World3Level1: return level == LevelTitle.World3Level2;
					case LevelTitle.World3Level2: return level == LevelTitle.World3Level3;
					case LevelTitle.World3Level3: return level == LevelTitle.World3Level4;
					case LevelTitle.World3Level4: return level == LevelTitle.World3Level5;
					case LevelTitle.World3Level5: return level == LevelTitle.World3Level6;
					case LevelTitle.World3Level6: return level == LevelTitle.World3HubDone;

					case LevelTitle.World4Level1: return level == LevelTitle.World4Level2;
					case LevelTitle.World4Level2: return level == LevelTitle.World4Level3;
					case LevelTitle.World4Level3: return level == LevelTitle.World4Level4;
					case LevelTitle.World4Level4: return level == LevelTitle.World4Level5;
					case LevelTitle.World4Level5: return level == LevelTitle.World4Level6;
					case LevelTitle.World4Level6: return level == LevelTitle.World4HubDone;
				}
				return false;
			}
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
						case LogObject.AverageVector: curr = mem.AverageVector().ToString(); break;
						case LogObject.WonGame: curr = mem.HasWonGame().ToString(); break;
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
			if (Model.CurrentState.Run.Count == 1 && string.IsNullOrEmpty(Model.CurrentState.Run[0].Name)) {
				Model.CurrentState.Run[0].Name = "1-1";
				Model.CurrentState.Run.AddSegment("1-2");
				Model.CurrentState.Run.AddSegment("1-3");
				Model.CurrentState.Run.AddSegment("1-4");
				Model.CurrentState.Run.AddSegment("1-5");
				Model.CurrentState.Run.AddSegment("1-6");
				Model.CurrentState.Run.AddSegment("2-1");
				Model.CurrentState.Run.AddSegment("2-2");
				Model.CurrentState.Run.AddSegment("2-3");
				Model.CurrentState.Run.AddSegment("2-4");
				Model.CurrentState.Run.AddSegment("2-5");
				Model.CurrentState.Run.AddSegment("2-6");
				Model.CurrentState.Run.AddSegment("3-1");
				Model.CurrentState.Run.AddSegment("3-2");
				Model.CurrentState.Run.AddSegment("3-3");
				Model.CurrentState.Run.AddSegment("3-4");
				Model.CurrentState.Run.AddSegment("3-5");
				Model.CurrentState.Run.AddSegment("3-6");
				Model.CurrentState.Run.AddSegment("4-1");
				Model.CurrentState.Run.AddSegment("4-2");
				Model.CurrentState.Run.AddSegment("4-3");
				Model.CurrentState.Run.AddSegment("4-4");
				Model.CurrentState.Run.AddSegment("4-5");
				Model.CurrentState.Run.AddSegment("4-6");
				Model.CurrentState.Run.AddSegment("End");
			}

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
		public Control GetSettingsControl(LayoutMode mode) { return null; }
		public void SetSettings(XmlNode document) { }
		public XmlNode GetSettings(XmlDocument document) { return document.CreateElement("Settings"); }
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