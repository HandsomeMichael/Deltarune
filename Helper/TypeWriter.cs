using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.UI.Chat;
using Terraria.UI;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.ModLoader.Config;
using Deltarune.Helper;
using Deltarune.Content.Items;
//this
using ReLogic.OS;

namespace Deltarune.Helper
{
	public class TypeWriter : ILoggable
	{
		byte frameMax;
		int timeLeft;
		LegacySoundStyle sound = null;
		public string text = "";
		public string wanted = "";

		public bool active => wanted != "";

		public bool Done() => (text == wanted);
		public float GetTime() => (float)timeLeft;

		public void Update(bool priority = false,float x = -1, float y = -1) {
			if (text == wanted) {
				if (timeLeft != -1) {
					if (timeLeft > 0) {timeLeft--;}
					else {wanted = "";}
				}
				return;
			}
			if (Main.GameUpdateCount % frameMax == 0) {
				if (sound != null && Main.netMode != NetmodeID.Server) {
					if (priority) {Main.PlaySound(sound);}
					else {Main.PlaySound(sound,new Vector2(x,y));}
				}
				if (text.Length != wanted.Length) {text += wanted[text.Length];}
			}
		}
		public void Log(Action<string> log) {
			log($"TypeWriter , [m:{frameMax}][t:{timeLeft}][{text}/{wanted}]");
		}

		public override string ToString() => text;

		public TypeWriter(string wanted = "",LegacySoundStyle sound = null,int frameMax = 1,int timeLeft = 340) {
			this.wanted = wanted;
			this.timeLeft = timeLeft;
			this.sound = sound;
			this.frameMax = (byte)frameMax;
		}
	}
}