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
	public class Portal : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Delta Portal");
		}
		public override void SetDefaults() {
			projectile.width = 18;
			projectile.height = 18;
			projectile.aiStyle = -1;
			projectile.friendly = false;
			projectile.scale = 1f;
			projectile.extraUpdates = 1;
			projectile.hostile = false;
			projectile.timeLeft = 600;
			projectile.tileCollide = false;
			projectile.scale = 0f;
			projectile.alpha = 255;
			projectile.hide = true;
		}
		public override void AI() {
			projectile.rotation += 0.05f;
			if (projectile.ai[1] == 0f) {
				if (projectile.localAI[0] > 0f) {
					if (Main.LocalPlayer.Distance(projectile.Center) < projectile.localAI[0]) {
						Main.LocalPlayer.GetDelta().CameraFocus(projectile.Center,3);
					}
				}
				projectile.scale += 0.005f;
				projectile.alpha -= 5;
				if (projectile.scale >= 1f && projectile.alpha <= 0) {
					if (projectile.ai[0] > 0f) {
						int a = NPC.NewNPC((int)projectile.Center.X, (int)projectile.Center.Y, (int)projectile.ai[0]);
						Main.npc[a].Center = projectile.Center;
					}
					projectile.ai[1] = 1f;
				}
				if (projectile.scale > 1f) {projectile.scale = 1f;}
				if (projectile.alpha < 0) {projectile.alpha = 0;}
			}
			if (projectile.ai[1] > 0f) {
				projectile.ai[1]++;
				if (projectile.ai[1] >= 60*5) {
					projectile.ai[1] = -1f;
				}
			}
			if (projectile.ai[1] == -1f) {
				projectile.scale -= 0.05f;
				projectile.alpha += 10;
				if (projectile.scale <= 0f && projectile.alpha >= 255) {
					projectile.Kill();
				}
				if (projectile.scale < 0f) {projectile.scale = 0f;}
				if (projectile.alpha > 255) {projectile.alpha = 255;}
			}

		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			Texture2D texture = ModContent.GetTexture(Texture);
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, 
			projectile.GetAlpha(Color.Black), projectile.rotation, texture.Size()/2f, projectile.scale, SpriteEffects.None, 0);	
			spriteBatch.BeginGlow(true);
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, 
			projectile.GetAlpha(Color.White), projectile.rotation, texture.Size()/2f, projectile.scale, SpriteEffects.None, 0);	
			spriteBatch.BeginNormal(true);
			return false;
		}
		//public override void PostDraw(SpriteBatch spriteBatch, Color lightColor) {}
		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI) {
			drawCacheProjsBehindNPCs.Add(index);
		}
	}
}

