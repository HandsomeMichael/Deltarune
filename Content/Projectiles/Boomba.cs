using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Deltarune;
using Deltarune.Helper;
using Terraria.UI.Chat;
using Terraria.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace Deltarune.Content.Projectiles
{
	public class Boomba : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Boom Boom");
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults() {
			projectile.width = 64;
			projectile.height = 64;
			projectile.aiStyle = -1;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.timeLeft = 240;
			projectile.tileCollide = false;
			projectile.extraUpdates = 1;
		}
		public override void AI() {
			projectile.scale *= 1.01f;
			projectile.alpha += 10;
			if (projectile.alpha >= 255) {
				projectile.Kill();
			}
			int frameSpeed = 5;
			projectile.frameCounter++;
			if (projectile.frameCounter >= frameSpeed) {
				projectile.frameCounter = 0;
				projectile.frame++;
				if (projectile.frame >= Main.projFrames[projectile.type]) {
					projectile.frame = 0;
				}
			}
		}
	}
}

