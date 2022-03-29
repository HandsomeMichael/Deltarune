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
	public class starwalkerStarFriendly : ModProjectile
	{
		public override string Texture => "Deltarune/Content/NPCs/Boss/starwalkerStar";
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Starwalker Star");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
		}
		public override void SetDefaults() {
			projectile.width = 16;
			projectile.height = 16;
			projectile.aiStyle = 0;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.scale = 1f;
			projectile.extraUpdates = 1;
			projectile.hostile = false;
			projectile.timeLeft = 360;
			projectile.tileCollide = false;
		}
		public override void AI() {
			projectile.scale -= 0.005f;
			if (projectile.scale <= 0f) {
				projectile.Kill();
			}
			int i = (int)(projectile.position.X/16);
			int j = (int)(projectile.position.Y/16);
			//Main.NewText($"{i} / {Main.maxTilesX} || {j} / {Main.maxTilesY}");
			// there is this funky bug with tmod where its just spam indexoutofbounds so i need to put this here :(
			if (i > Main.maxTilesX || j > Main.maxTilesY || i < 0 || j < 0) {
				return;
			}
			Tile tile = Framing.GetTileSafely(i,j);
			if (tile != null && tile.active() && Main.tileSolid[tile.type]) {
				int a = Helpme.NearestNPC(projectile.Center);
				if (a != -1) {
					projectile.velocity = Vector2.Lerp(projectile.velocity,projectile.DirectionTo(Main.npc[a].Center)*3f,0.05f);
				}
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			//Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++) {
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}
	}
}

