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
	public class wingedCar : ModNPC
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Dangerous Car With Wing");
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
		int frame;
		int frameCounter;
		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) {
			frameCounter++;
			if (frameCounter > 5) {
				frameCounter = 0;
				frame++;
			}
			if (frame >= 5) {frame = 1;}
			Texture2D wing = ModContent.GetTexture("Deltarune/Content/NPCs/Boss/wing");
			Texture2D tt = ModContent.GetTexture(Texture);
			spriteBatch.Draw(wing, npc.Center - Main.screenPosition, wing.GetFrame(frame,5), Color.White, 0f, wing.GetFrame(frame,5).Size() * 0.5f, npc.scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(tt, npc.Center - Main.screenPosition, null, Color.White, npc.rotation, tt.Size() * 0.5f, npc.scale*2f, SpriteEffects.None, 0f);
			//ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Deltarune.tobyFont, $"Ai{npc.[1]} \n{num} DPS", npc.position - Main.screenPosition - new Vector2(0,50), Color.White, 0, Vector2.Zero, Vector2.One);
			return false;
		}
		public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit) {
			//secretly buff wooden sword for no reason at all
			if (item.type == ItemID.WoodenSword) {
				crit = true;
				damage *= 2;
			}
		}
		//public override void SendExtraAI(BinaryWriter writer) {}
		//public override void ReceiveExtraAI(BinaryReader reader) {}

		public float state {get => npc.ai[0];set => npc.ai[0] = value;}
		public float timer {get => npc.ai[1];set => npc.ai[1] = value;}
		public float statetimer {get => npc.ai[2];set => npc.ai[2] = value;}
		public override void AI() {
			npc.TargetClosest(true);
			Player player = Main.player[npc.target];
			npc.velocity = Vector2.Lerp(npc.velocity,npc.DirectionTo(player.Center)*5f,0.1f);
			npc.rotation = npc.velocity.X * 0.08f;

			statetimer--;
			if (state == 0) {
				if (statetimer <= 0) {
					state = 0;
					var b = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_bigcar_yelp");
					npc.GetDelta().textOverhead = new TypeWriter("Funny Car go vroom vroom",b,5);
					statetimer = 300f;
				}
				timer++;
				if (timer > 60) {
					if (Main.netMode != NetmodeID.MultiplayerClient) {
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_bigcar_yelp"),npc.Center);
						Projectile.NewProjectile(npc.Center,npc.DirectionTo(player.Center+player.velocity)*6f,ModContent.ProjectileType<carWalk>(),npc.damage,Main.myPlayer);
					}
					timer = 0;
				}
			}
		}
	}
	public class carWalk : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Walking Car");
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults() {
			projectile.width = 8;
			projectile.height = 16;
			projectile.aiStyle = 1;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.timeLeft = 600;
			projectile.alpha = 255;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.extraUpdates = 1;
			aiType = ProjectileID.Bullet;
		}
		public override void AI() {
			projectile.spriteDirection = projectile.direction;
			projectile.rotation += MathHelper.ToRadians(90f);
			int frameSpeed = 10;
			projectile.frameCounter++;
			if (projectile.frameCounter >= frameSpeed) {
				projectile.frameCounter = 0;
				projectile.frame++;
				if (projectile.frame >= Main.projFrames[projectile.type]) {
					projectile.frame = 0;
				}
			}
		}
		public override void Kill(int timeLeft){
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_hitcar"),projectile.Center);
			int a = Projectile.NewProjectile(projectile.Center,Vector2.Zero,ModContent.ProjectileType<Boomba>(),projectile.damage,projectile.knockBack,projectile.whoAmI);
			Main.projectile[a].Center = projectile.Center;
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}
	}
}

