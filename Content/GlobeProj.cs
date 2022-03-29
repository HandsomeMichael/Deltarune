using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.ModLoader.Config;
using Deltarune.Helper;
using Deltarune;
//this
using ReLogic.OS;

namespace Deltarune.Content
{
	public class GlobeProj : GlobalProjectile
	{
		public bool babagun;

		public override bool InstancePerEntity => true;

		public override void PostDraw(Projectile projectile,SpriteBatch spriteBatch, Color lightColor) {
			//Rectangle rec = Main.LocalPlayer.GetDelta().TPBox();
			//rec.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
			//spriteBatch.Draw(Main.magicPixel, rec, Color.Red);

			//rec = Main.LocalPlayer.Hitbox;
			//rec.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
			//spriteBatch.Draw(Main.magicPixel, rec, Color.Green);
		}

		public bool Graze;
		//public int[] Timer = new int[3];
		public override void PostAI(Projectile projectile) {
			Player owner = Main.player[projectile.owner];
			if (babagun) {
				projectile.velocity *= 0.95f;
			}
			if (owner.active && !owner.dead && babagun && owner.GetDelta().richochetBullet != null) {
				babagun = false;
				projectile.damage *= 2;
				projectile.tileCollide = true;
				//projectile.Center.DustLine(owner.GetDelta().richochetBullet ?? owner.Center,200);
				projectile.velocity = projectile.DirectionTo(owner.GetDelta().richochetBullet ?? owner.Center)*20f;
			}
			if (projectile.hostile) {
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					Player player = Main.player[i];
					if (player.active && !player.dead) {
						var p = player.GetDelta();
						if (p.TP < p.TPMax && p.TPCooldown < 1 && !projectile.Hitbox.Intersects(player.Hitbox) && projectile.Hitbox.Intersects(p.TPBox())) {
							p.Graze(projectile,projectile.damage,Graze);
							Graze = true;
							//CombatText.NewText(player.getRect(),Color.Yellow,((float)projectile.damage/(float)p.TPMax)*100f+"%");
						}
					}
				}
			}
			//AdditiveHandler.Proj(projectile);
		}
	}
}