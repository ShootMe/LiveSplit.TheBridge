using System;
using System.Diagnostics;
namespace LiveSplit.TheBridge {
	//.load C:\Windows\Microsoft.NET\Framework\v4.0.30319\SOS.dll
	public partial class SplitterMemory {
		private static ProgramPointer LevelManager = new ProgramPointer(AutoDeref.None, new ProgramSignature(PointerVersion.Steam, "558BECC705????????0200000033D28915", 5));
		private static ProgramPointer LoadingScreen = new ProgramPointer(AutoDeref.None, new ProgramSignature(PointerVersion.Steam, "558BEC575683EC08C605????????008D7DF00F57C0", 10));
		private static ProgramPointer GameWorld = new ProgramPointer(AutoDeref.None, new ProgramSignature(PointerVersion.Steam, "85C075B4833D????????007423A1????????3800B9", 6));
		private static ProgramPointer WonGame = new ProgramPointer(AutoDeref.None, new ProgramSignature(PointerVersion.Steam, "33C08BC833D28B018B402CFF10C605????????00B9", 15));

		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		public DateTime LastHooked;

		public SplitterMemory() {
			LastHooked = DateTime.MinValue;
		}
		public string RAMPointers() {
			return LevelManager.GetPointer(Program).ToString("X");
		}
		public string RAMPointerVersion() {
			return LevelManager.Version.ToString();
		}
		public LevelTitle NextLevel() {
			//TheBridge.Levels.LevelManager.NextLevel
			return (LevelTitle)LevelManager.Read<int>(Program, 0x0, 0x0);
		}
		public LevelTitle PreviousLevelCompleted() {
			//TheBridge.Levels.LevelManager.PreviousLevelCompleted
			return (LevelTitle)LevelManager.Read<int>(Program, 0x0, 0x8);
		}
		public bool IsLoading() {
			//TheBridge.Screens.LoadingScreen.IsLoadingScreenRunningThread
			return LoadingScreen.Read<bool>(Program, 0x0, 0x0);
		}
		public PlayerState PlayerState() {
			//TheBridge.GameElements.GameWorld.player.state
			return (PlayerState)GameWorld.Read<int>(Program, 0x0, 0xc, 0x198);
		}
		public long AverageVector() {
			//TheBridge.GameElements.GameWorld.player.AverageVector
			return GameWorld.Read<long>(Program, 0x0, 0xc, 0xa0);
		}
		public bool InHypercube() {
			//TheBridge.GameElements.GameWorld.player.body.Xf.Position.X
			float x = GameWorld.Read<float>(Program, 0x0, 0xc, 0x10, 0x80);
			//TheBridge.GameElements.GameWorld.player.body.Xf.Position.Y
			float y = GameWorld.Read<float>(Program, 0x0, 0xc, 0x10, 0x84);
			return x > 50000 && (65125f - x) * (65125f - x) + (3190f - y) * (3190f - y) < 7200f;
		}
		public bool HasWonGame() {
			//TheBridge.GameElements.GameWorld.HasWonGame
			return WonGame.Read<bool>(Program, 0x0, -0xa);
		}
		public bool EnableDarkWorlds() {
			//TheBridge.GameElements.GameWorld.EnableDarkWorlds
			return WonGame.Read<bool>(Program, 0x0, -0xe);
		}
		public bool HookProcess() {
			IsHooked = Program != null && !Program.HasExited;
			if (!IsHooked && DateTime.Now > LastHooked.AddSeconds(1)) {
				LastHooked = DateTime.Now;
				Process[] processes = Process.GetProcessesByName("The Bridge");
				Program = processes != null && processes.Length > 0 ? processes[0] : null;

				if (Program != null && !Program.HasExited) {
					MemoryReader.Update64Bit(Program);
					IsHooked = true;
				}
			}

			return IsHooked;
		}
		public void Dispose() {
			if (Program != null) {
				Program.Dispose();
			}
		}
	}
}