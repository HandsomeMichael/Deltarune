using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Deltarune;
using Deltarune.Content;
using Deltarune.Content.Projectiles;
using Deltarune.Helper;
using Terraria.UI.Chat;
using Terraria.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace Deltarune.Content.NPCs.Boss
{
	public class TheCat : ModNPC
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("The Cat");
			NPCID.Sets.TrailCacheLength[npc.type] = 10; //Higher numbers mean longer trails
    		NPCID.Sets.TrailingMode[npc.type] = 0;
		}
		public override void SetDefaults() {
			npc.width = 200;
			npc.height = 200;
			npc.aiStyle = -1;
			npc.damage = 30;
			npc.value = 1000;
			npc.boss = false;
			npc.lifeMax = 5500;
			npc.friendly = true;
			npc.dontTakeDamage = true;
			npc.knockBackResist = 0f;
			npc.defense = 20;
			npc.HitSound = SoundID.NPCHit2;
			npc.DeathSound = SoundID.NPCDeath2;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.alpha = 255;
		}
		public float timer {get => npc.ai[0];set => npc.ai[0] = value;}
		public override void AI() {
			//startup
			npc.TargetClosest(true);
			Player player = Main.player[npc.target];
			Vector2 dir = npc.DirectionTo(player.Center);
			Vector2 proDir = npc.DirectionTo(player.Center+player.velocity);
			if (player.dead) {
				npc.TargetClosest(true);
				player = Main.player[npc.target];
				if (player.dead) {
					timer = -1;
				}
			}
			else {
				npc.Center = Vector2.Lerp(npc.Center,player.Center,0.1f);
			}
			//graphics
			npc.spriteDirection = (npc.velocity.X > 0 ? -1 : 1);
			npc.rotation = npc.velocity.X * 0.01f;
			if (npc.alpha > 255) {npc.alpha = 255;}
			if (npc.alpha < 0) {npc.alpha = 0;}
			if (timer == -1) {
				npc.alpha += 5;
				if (npc.alpha == 255) {
					npc.active = false;
				}
				return;
			}
			if (timer > -1) {
				timer++;
				npc.alpha -= 5;
				if (timer == 60) {
					npc.GetDelta().textOverhead = new TypeWriter("Look me in the eyes",Deltarune.GetSound("text"),3);
					Main.PlaySound(Deltarune.GetSound("cat1",false));
				}
				if (timer == 200) {
					npc.GetDelta().textOverhead = new TypeWriter("Do you feel it ?",Deltarune.GetSound("text"),3);
					Main.PlaySound(Deltarune.GetSound("cat2",false));
				}
				if (timer == 300) {
					npc.GetDelta().textOverhead = new TypeWriter("The Madness consuming your mind,",Deltarune.GetSound("text"),3);
					Main.PlaySound(Deltarune.GetSound("cat3",false));
				}
				if (timer == 480) {
					npc.GetDelta().textOverhead = new TypeWriter("The utter despair",Deltarune.GetSound("text"),3);
					Main.PlaySound(Deltarune.GetSound("cat4",false));
				}
				if (timer == 600) {
					npc.GetDelta().textOverhead = new TypeWriter("In the face of the inevitable",Deltarune.GetSound("text"),3);
					Main.PlaySound(Deltarune.GetSound("cat5",false));
				}
				if (timer == 760) {
					timer = -1;
				}
			}
			//sound
			//for syncing projectile if i forgor
			//NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			Texture2D texture = Main.npcTexture[npc.type];
			Vector2 orig = texture.Size()/2f;
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1) {spriteEffects = SpriteEffects.FlipHorizontally;}
			spriteBatch.Draw(texture, npc.Center - Main.screenPosition,null, npc.GetAlpha(Color.Black), npc.rotation, orig, npc.scale, spriteEffects, 0f);
			spriteBatch.BeginGlow(true);
			spriteBatch.Draw(texture, npc.Center - Main.screenPosition,null, npc.GetAlpha(Color.White), npc.rotation, orig, npc.scale, spriteEffects, 0f);
			spriteBatch.BeginNormal(true);
			return false;
		}
		//public override void PostDraw(SpriteBatch spriteBatch, Color lightColor) {}
	}
}

