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

namespace Deltarune.Content.NPCs.Boss
{
	public class Tomat : ModNPC
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Tomat");
		}
		public override void SetDefaults() {
			npc.width = 60;
			npc.height = 60;
			npc.aiStyle = -1;
			npc.damage = 100;
			npc.value = 1000;
			npc.boss = true;
			npc.lifeMax = 1000;
			npc.friendly = false;
			npc.knockBackResist = 0f;
			npc.defense = 70;
			npc.HitSound = SoundID.NPCHit2;
			npc.DeathSound = SoundID.NPCDeath2;
			npc.noGravity = true;
			npc.noTileCollide = true;
			music = Deltarune.NPCMusic("battle",MusicID.Boss1);
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) {
			Texture2D texture = Main.npcTexture[npc.type];
			Vector2 size = texture.Size();
			Vector2 position = npc.position;
			Rectangle rec = position.QuickRec(size);
			return false;
		}
		
		//public override void SendExtraAI(BinaryWriter writer) {}
		//public override void ReceiveExtraAI(BinaryReader reader) {}

		public float state {get => npc.ai[0];set => npc.ai[0] = value;}
		public float timer {get => npc.ai[1];set => npc.ai[1] = value;}
		public float statetimer {get => npc.ai[2];set => npc.ai[2] = value;}
		
		public override void AI() {
			npc.TargetClosest(true);
			Player player = Main.player[npc.target];
		}
	}
}

