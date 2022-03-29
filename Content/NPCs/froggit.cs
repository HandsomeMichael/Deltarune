using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Deltarune;
using Deltarune.Content.Projectiles;
using Deltarune.Helper;
using Terraria.UI.Chat;
using Terraria.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace Deltarune.Content.NPCs
{
	public class froggit : ModNPC
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Froggit");
			Main.npcFrameCount[npc.type] = 4;
		}
		public override void SetDefaults() {
			npc.CloneDefaults(NPCID.BlueSlime);
			aiType = NPCID.BlueSlime;
			animationType = NPCID.BlueSlime;
			npc.color = Color.White;
			npc.alpha = 0;
		}
		public override void AI() {
			if (npc.velocity.Y > 1f) {
				npc.velocity.Y += 0.1f;
				if (npc.velocity.Y > 10f) {
					npc.velocity.Y = 10f;
				}
			}
		}
	}
}

