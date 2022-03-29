using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Deltarune;
using Deltarune.Content;
using Deltarune.Helper;
using Terraria.UI.Chat;
using Terraria.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace Deltarune.Content.Projectiles
{
	public class homingHart : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Bullet Tracker");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 30;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
		}
		public override void SetDefaults() {
			projectile.CloneDefaults(ProjectileID.Bullet);
			projectile.scale = 0.8f;
			aiType = ProjectileID.Bullet;
		}
		public override void PostAI() {
			projectile.scale -= 0.001f;
			if (projectile.scale < 0f) {
				projectile.Kill();
			}
		}
		public override void Kill(int timeLeft)
		{
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
			Player player = Main.player[projectile.owner];
			if (player.active && !player.dead) {
				player.GetDelta().richochetBullet = projectile.Center;
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			//Redraw the projectile with the color not influenced by light
			/*
			for (int k = 0; k < projectile.oldPos.Length; k++) {
				Vector2 drawPos = projectile.Center(projectile.oldPos[k]) - Main.screenPosition;
				float alpha = (float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length;
				Color color = projectile.GetAlpha(Color.Black) * alpha;
				spriteBatch.Draw(texture, drawPos, null, color, projectile.rotation, texture.Size()/2f, projectile.scale*alpha, SpriteEffects.None, 0f);
			}
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(Color.Black), projectile.rotation, texture.Size()/2f, projectile.scale, SpriteEffects.None, 0f);
			*/
			spriteBatch.BeginGlow(true);
			var texture = ModContent.GetTexture(Texture+"_glow");
			for (int k = 0; k < projectile.oldPos.Length; k++) {
				Vector2 drawPos = projectile.Center(projectile.oldPos[k]) - Main.screenPosition;
				float alpha = (float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length;
				Color color = projectile.GetAlpha(Color.White) * alpha;
				spriteBatch.Draw(texture, drawPos, null, color, projectile.rotation, texture.Size()/2f, projectile.scale*alpha, SpriteEffects.None, 0f);
			}
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(Color.White), projectile.rotation, texture.Size()/2f, projectile.scale, SpriteEffects.None, 0f);
			spriteBatch.BeginNormal(true);
			texture = Main.projectileTexture[projectile.type];
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(Color.White), projectile.rotation, texture.Size()/2f, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}

