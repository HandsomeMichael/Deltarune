
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Deltarune.Content.UI;
using Deltarune.Helper;
using Deltarune.Content.Spell;

namespace Deltarune
{
	public class TextureCache : DeltaSystem
	{
		public static Texture2D logo1;
		public static Texture2D logo2;
		public static Texture2D chatBack;
		public static DynamicSpriteFont fontMouseText;

		public override void Load() {
			if (!Main.dedServ) {
				logo1 = Main.logoTexture;
				logo2 = Main.logo2Texture;
				chatBack = Main.chatBackTexture;
				fontMouseText = Main.fontMouseText;
				if (MyConfig.get.DialogBackground) {
					Main.chatBackTexture = ModContent.GetTexture("Deltarune/Content/Texture/Chat_Back");
				}
			}
		}
		public override void Unload() {
			if (!Main.dedServ) {
				Main.logoTexture = logo1;
				Main.logo2Texture = logo2;
				Main.chatBackTexture = chatBack;
				if (fontMouseText != null) {Main.fontMouseText = fontMouseText;}
				fontMouseText = null;
				logo1 = null;
				logo2 = null;
				chatBack = null;
			}
		}
	}
}
