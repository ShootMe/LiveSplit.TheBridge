using System.ComponentModel;
namespace LiveSplit.TheBridge {
	public enum LogObject {
		CurrentSplit,
		Pointers,
		PointerVersion,
		NextLevel,
		LastLevel,
		Loading,
		PlayerState,
		WonGame,
		DarkWorlds,
		InHypercube
	}
	public enum SplitName {
		[Description("Manual Split (Not Automatic)"), ToolTip("Does not split automatically. Use this for custom splits not yet defined.")]
		ManualSplit,

		[Description("I - I Loft (On Enter)"), ToolTip("Splits when entering level I - I")]
		Level_1_1_Enter,
		[Description("I - I Loft (Completed)"), ToolTip("Splits when finishing level I - I")]
		Level_1_1,
		[Description("I - II Library (Completed)"), ToolTip("Splits when finishing level I - II")]
		Level_1_2,
		[Description("I - III Menace (Completed)"), ToolTip("Splits when finishing level I - III")]
		Level_1_3,
		[Description("I - IV Courtyard (Completed)"), ToolTip("Splits when finishing level I - IV")]
		Level_1_4,
		[Description("I - V Spiral (Completed)"), ToolTip("Splits when finishing level I - V")]
		Level_1_5,
		[Description("I - VI Nook (Completed)"), ToolTip("Splits when finishing level I - VI")]
		Level_1_6,

		[Description("II - I Vortex (On Enter)"), ToolTip("Splits when entering level II - I")]
		Level_2_1_Enter,
		[Description("II - I Vortex (Completed)"), ToolTip("Splits when finishing level II - I")]
		Level_2_1,
		[Description("II - II Precipice (Completed)"), ToolTip("Splits when finishing level II - II")]
		Level_2_2,
		[Description("II - III Lion (Completed)"), ToolTip("Splits when finishing level II - III")]
		Level_2_3,
		[Description("II - IV Pillars (Completed)"), ToolTip("Splits when finishing level II - IV")]
		Level_2_4,
		[Description("II - V Mausoleum (Completed)"), ToolTip("Splits when finishing level II - V")]
		Level_2_5,
		[Description("II - VI Memorial (Completed)"), ToolTip("Splits when finishing level II - VI")]
		Level_2_6,

		[Description("III - I Inversion (On Enter)"), ToolTip("Splits when entering level III - I")]
		Level_3_1_Enter,
		[Description("III - I Inversion (Completed)"), ToolTip("Splits when finishing level III - I")]
		Level_3_1,
		[Description("III - II Timepiece (Completed)"), ToolTip("Splits when finishing level III - II")]
		Level_3_2,
		[Description("III - III Aftermath (Completed)"), ToolTip("Splits when finishing level III - III")]
		Level_3_3,
		[Description("III - IV Antique (Completed)"), ToolTip("Splits when finishing level III - IV")]
		Level_3_4,
		[Description("III - V Corridor (Completed)"), ToolTip("Splits when finishing level III - V")]
		Level_3_5,
		[Description("III - VI Garden (Completed)"), ToolTip("Splits when finishing level III - VI")]
		Level_3_6,

		[Description("IV - I Veil (On Enter)"), ToolTip("Splits when entering level IV - I")]
		Level_4_1_Enter,
		[Description("IV - I Veil (Completed)"), ToolTip("Splits when finishing level IV - I")]
		Level_4_1,
		[Description("IV - II Rook (Completed)"), ToolTip("Splits when finishing level IV - II")]
		Level_4_2,
		[Description("IV - III Bend (Completed)"), ToolTip("Splits when finishing level IV - III")]
		Level_4_3,
		[Description("IV - IV Triad (Completed)"), ToolTip("Splits when finishing level IV - IV")]
		Level_4_4,
		[Description("IV - V Intersection (Completed)"), ToolTip("Splits when finishing level IV - V")]
		Level_4_5,
		[Description("IV - VI Archway (Completed)"), ToolTip("Splits when finishing level IV - VI")]
		Level_4_6,

		[Description("End Game (Normal)"), ToolTip("Splits when ending the game by getting hit by an apple")]
		Level_End_Normal,
		[Description("End Game (Mirror)"), ToolTip("Splits when ending the game by falling into vortex at your house")]
		Level_End_Mirror
	}
	public enum PlayerState {
		Null,
		Idle,
		Walking,
		Trudging,
		Dying,
		WaitingToBeDrawn,
		BeingDrawn,
		Falling,
		Sliding,
		Landing,
		EnteringDoorway,
		Sleeping,
		WakingUp,
		BracingForImpact,
		Waiting
	}
	public enum LevelTitle {
		Null,
		StartScreen,
		HubWorldConfigurationIntro,
		World1Hub,
		World1Level1,
		World1Level2,
		World1Level3,
		World1Level4,
		World1Level5,
		World1Level6,
		World1HubDone,
		HubWorldConfigurationAfterWorld1,
		World2Hub,
		World2Level1,
		World2Level2,
		World2Level3,
		World2Level4,
		World2Level5,
		World2Level6,
		World2HubDone,
		HubWorldConfigurationAfterWorld2,
		World3Hub,
		World3Level1,
		World3Level2,
		World3Level3,
		World3Level4,
		World3Level5,
		World3Level6,
		World3HubDone,
		HubWorldConfigurationAfterWorld3,
		World4Hub,
		World4Level1,
		World4Level2,
		World4Level3,
		World4Level4,
		World4Level5,
		World4Level6,
		World4HubDone,
		HubWorldConfigurationAfterWorld4,
		TrialOverHallway,
		NumberOfLevelTitles
	}
}