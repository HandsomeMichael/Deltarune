using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Deltarune;
using Deltarune.Helper;
using Deltarune.Content.Buffs;
using Terraria.UI.Chat;
using Terraria.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace Deltarune.Content.Projectiles
{
	public class SusieEpicSwing : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Susi Epic");
			Main.projFrames[projectile.type] = 6;
		}
		public override void SetDefaults() {
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.melee = true;
			projectile.width = 64;
			projectile.height = 64;
			projectile.aiStyle = -1;
			projectile.timeLeft = 60;
			projectile.tileCollide = false;
			projectile.extraUpdates = 1;
			projectile.scale = 1.5f;
		}
		public override void AI() {
			Player player = Main.player[projectile.owner];
			player.heldProj = projectile.whoAmI;
			projectile.scale -= 0.01f;
			//player.direction = projectile.direction;
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.direction = projectile.direction;
			projectile.Center = player.Center;
			if (projectile.owner == Main.myPlayer) {
				player.itemRotation = player.AngleTo(Main.MouseWorld);
				projectile.rotation = projectile.AngleTo(Main.MouseWorld);
				projectile.velocity = projectile.DirectionTo(Main.MouseWorld)*10f;
				projectile.Center += projectile.velocity;
				projectile.netUpdate = true;
			}
			// if i lose it all. lose it all. lose it all
			// i will never look away
			projectile.frameCounter++;
			if (projectile.frameCounter >= 4) {
				projectile.frameCounter = 0;
				projectile.frame++;
				if (projectile.frame >= Main.projFrames[projectile.type]) {
					projectile.Kill();
				}
			}
			Lighting.AddLight(projectile.Center, Color.Cyan.ToVector3() * 0.78f);
		}
		public override bool ShouldUpdatePosition() {
			return false;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.direction == -1) {
				//spriteEffects = SpriteEffects.FlipHorizontally;
				spriteEffects = SpriteEffects.FlipVertically;
			}
			Texture2D texture = Main.projectileTexture[projectile.type];
			Rectangle rec = texture.GetFrame(projectile.frame,Main.projFrames[projectile.type]);
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, rec, projectile.GetAlpha(Color.White), projectile.rotation, rec.Size()/2f, projectile.scale, spriteEffects, 0);
			spriteBatch.BeginGlow(true);
			texture = ModContent.GetTexture(Texture+"_glow");
			rec = texture.GetFrame(projectile.frame,Main.projFrames[projectile.type]);
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, rec, projectile.GetAlpha(Color.White)*0.5f, projectile.rotation, rec.Size()/2f, projectile.scale, spriteEffects, 0);
			spriteBatch.BeginNormal(true);
			return false;
		}
		public override void OnHitNPC(NPC npc, int damage, float knockback, bool crit) {
			if (crit) {
				npc.AddBuff(ModContent.BuffType<fatalbleed>(),60*2);
				for (int i = 0; i < Main.rand.Next(9,13); i++){
					Vector2 v = projectile.velocity;
					v.RotatedByRandom(MathHelper.ToRadians(20));
					Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 88, v.X, v.Y, 0, Color.White, 1f)];
					dust.noGravity = true;
				}
			}
			else {
				npc.AddBuff(ModContent.BuffType<fatalbleed>(),60);
				for (int i = 0; i < Main.rand.Next(7,10); i++){
					Vector2 v = projectile.velocity;
					v.RotatedByRandom(MathHelper.ToRadians(20));
					Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 88, v.X, v.Y, 0, Color.White, 0.8f)];
					dust.noGravity = true;
				}
			}
		}
	}
}

