using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace Deltarune
{
	//[BackgroundColor(144, 252, 249)]
	[Label("Deltarune Client Config")]
	public class MyConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;
		// public override ConfigScope Mode => ConfigScope.ServerSide;
		public static MyConfig get => ModContent.GetInstance<MyConfig>();

		[Header("Graphic")]

		[Label("Screen Shake")]
		[DefaultValue(true)]
		public bool CameraShake;

		[Label("Deltarune intro")]
		[Tooltip("Play an intro everytime the mod is loaded")]
		[DefaultValue(true)]
		public bool intro;

		[Label("Deltarune Main Menu")]
		[Tooltip("Change the main menu to deltarune style\nincluding the lancer theme")]
		[DefaultValue(true)]
		public bool mainMenu;

		[Label("Classic Tension Style")]
		[Tooltip("Change tension bar position below the mana bar")]
		[DefaultValue(false)]
		public bool tensionStyle;

		[Label("Ralsei dummy damage source")]
		[Tooltip("Display damage source on ralsei dummy")]
		[DefaultValue(false)]
		public bool ralseidummySource;

		[Label("Battle Background Opacity")]
		[Tooltip("The opacity of battle background\nset to 0 to disable\n [default is 0.9]")]
		[Range(0f, 1f)]
		[Increment(0.1f)]
		[DefaultValue(0.9f)]
		[DrawTicks]
		[Slider] 
		public float BattleBackground;

		[Label("New Game Over Scene")]
		[Tooltip("Replace terraria game over scene with \nthe epic deltarune game over scene")]
		[DefaultValue(true)]
		public bool newGameOver;

		[Header("Town NPC Dialogue")]

		[Label("Town NPC Typewriter effect")]
		[Tooltip("Make town npc has typewriter effect in dialogue")]
		[DefaultValue(true)]
		public bool DialogTypewriter;

		[Label("Town NPC Deltarune dialog syle")]
		[Tooltip("Make town npc uses deltarune dialog style")]
		[DefaultValue(true)]
		public bool DialogBackground;

		[Header("Debugging")]

		[Label("Show debug values")]
		[Tooltip("Show the debug values on Deltarune mod class")]
		[DefaultValue(false)]
		public bool showDebug;

		[Label("Password")]
		[Tooltip("set to something funny, idk")]
		[DefaultValue("ralsei")]
		public string Password;

		public override void OnChanged() {
			if (Password == "ResetRalsei") {
				MyWorld.hadRalsei = false;
				Main.NewText("reseted ralsei");
			}
			base.OnChanged();
		}
	}
}
