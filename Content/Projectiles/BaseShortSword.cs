using System;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Deltarune;
using Deltarune.Helper;
using Deltarune.Content.Items;
using Deltarune.Content.Buffs;

namespace Deltarune.Content.Projectiles
{
	public class BaseShortSword : ModProjectile
	{
		public override string Texture => "Terraria/Item_"+ItemID.CopperShortsword;
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Shortsword"); 
		}
		public override void SetDefaults() {
			projectile.width = 18;
			projectile.height = 18;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.scale = 1f;
			projectile.ownerHitCheck = true;
			projectile.melee = true;
			projectile.extraUpdates = 1;
			projectile.timeLeft = 360;
			projectile.hide = true;
		}
		public override void AI() {
			Player player = Main.player[projectile.owner];
			if (player.HeldItem.IsAir || player.HeldItem.type < 1) {projectile.Kill();}
			if (player.HeldItem.modItem != null && player.HeldItem.modItem is ShortswordItem num) {
				if (projectile.ai[0] == 0f) {
					num.ProjFirstTick(projectile);
				}
				if (!num.PreAI(projectile)) {return;}
			}
			projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2f;
			projectile.ai[0] += 1f;
			float num2 = (projectile.Opacity = Helpme.GetLerpValue(0f, 7f, projectile.ai[0], clamped: true) * Helpme.GetLerpValue(16f, 12f, projectile.ai[0], clamped: true));
			projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + projectile.velocity * (projectile.ai[0] - 1f);
			projectile.spriteDirection = ((!(Vector2.Dot(projectile.velocity, Vector2.UnitX) < 0f)) ? 1 : (-1));
			projectile.rotation -= (float)Math.PI / 4f * (float)projectile.spriteDirection;
			if (projectile.ai[0] >= 16f)
			{
				projectile.Kill();
			}
			else
			{
				player.heldProj = projectile.whoAmI;
			}
			if (player.HeldItem.modItem != null && player.HeldItem.modItem is Content.Items.ShortswordItem num3) {
				num3.CustomAI(projectile);
			}
		}
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetRect) {
			float collisionPoint9 = 0f;
			if (Collision.CheckAABBvLineCollision(targetRect.TopLeft(), targetRect.Size(), projectile.Center, projectile.Center + projectile.velocity * 6f, 10f * projectile.scale, ref collisionPoint9)){
				return true;
			}
			return false;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			Player player = Main.player[projectile.owner];
			if (player.HeldItem.IsAir || player.HeldItem.type < 1) {return false;}
			Texture2D texture = Main.itemTexture[player.HeldItem.type];
			if (player.HeldItem.modItem != null) {
				if (player.HeldItem.modItem is Content.Items.ShortswordItem item) {
					if (!item.PreDrawProj(projectile,spriteBatch,lightColor)) {return false;}
				}
			}
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, 
			null, projectile.GetAlpha(lightColor), 
			projectile.rotation, 
			texture.Size()/2f, 
			projectile.scale, 
			spriteEffects, 0);

			if (player.HeldItem.modItem != null) {
				if (player.HeldItem.modItem is Content.Items.ShortswordItem item) {
					if (item.GlowMask) {
						texture = ModContent.GetTexture(player.HeldItem.modItem.Texture+"_Glow");
						spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, 
						null, Color.White, 
						projectile.rotation, 
						texture.Size()/2f, 
						projectile.scale, 
						spriteEffects, 0);
					}
					item.PostDrawProj(projectile,spriteBatch,lightColor);
				}
			}
			//spriteBatch.BeginNormal(true);

			/*
			int num36 = 0;
			int num37 = 0;
			float num38 = (float)(Main.projectileTexture[projectile.type].Width - projectile.width) * 0.5f + (float)projectile.width * 0.5f;

			spriteBatch.Draw(Main.projectileTexture[projectile.type], 
			new Vector2(projectile.position.X - Main.screenPosition.X + num38 + (float)num37,
			 projectile.position.Y - Main.screenPosition.Y + (float)(projectile.height / 2) + projectile.gfxOffY),
			  new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height),
			   projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(num38, projectile.height / 2 + num36), projectile.scale, spriteEffects, 0f);

			if (projectile.glowMask != -1)
			{
				Main.spriteBatch.Draw(Main.glowMaskTexture[projectile.glowMask], new Vector2(projectile.position.X - Main.screenPosition.X + num38 + (float)num37, projectile.position.Y - Main.screenPosition.Y + (float)(projectile.height / 2) + projectile.gfxOffY), new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height), new Color(250, 250, 250, projectile.alpha), projectile.rotation, new Vector2(num38, projectile.height / 2 + num36), projectile.scale, spriteEffects, 0f);
			}
			if (projectile.type == 473)
			{
				Main.spriteBatch.Draw(Main.projectileTexture[projectile.type], new Vector2(projectile.position.X - Main.screenPosition.X + num38 + (float)num37, projectile.position.Y - Main.screenPosition.Y + (float)(projectile.height / 2) + projectile.gfxOffY), new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height), new Color(255, 255, 0, 0), projectile.rotation, new Vector2(num38, projectile.height / 2 + num36), projectile.scale, spriteEffects, 0f);
			}
			ModProjectile modProjectile = projectile.modProjectile;
			if (modProjectile != null && ModContent.TryGetTexture(modProjectile.GlowTexture, out var glowTexture))
			{
				Main.spriteBatch.Draw(glowTexture, new Vector2(projectile.position.X - Main.screenPosition.X + num38 + (float)num37, projectile.position.Y - Main.screenPosition.Y + (float)(projectile.height / 2) + projectile.gfxOffY), new Rectangle(0, 0, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height), new Color(250, 250, 250, projectile.alpha), projectile.rotation, new Vector2(num38, projectile.height / 2 + num36), projectile.scale, spriteEffects, 0f);
			}*/
			return false;
		}

		// for mod support idk
		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockBack, ref bool crit, ref int hitDirection) {
			Player player = Main.player[projectile.owner];
			// calculate damage increment
			var p = player.GetDelta();
			damage /= 2;
			damage += (int)((float)damage*(p.Shortswordatt/5f));
			p.Shortswordatt += 1f;
			if (p.Shortswordatt > 5f) {p.Shortswordatt = 5f;}
			//allow armor penetration
			if (player.armorPenetration > 0){damage += target.checkArmorPenetration(player.armorPenetration);}
			// call modloader stuff because there are no vanilla modifyhitnpc code
			ItemLoader.ModifyHitNPC(player.HeldItem,player,target,ref damage, ref knockBack, ref crit);
			NPCLoader.ModifyHitByItem(target, player, player.HeldItem, ref damage, ref knockBack, ref crit);
			PlayerHooks.ModifyHitNPC(player, player.HeldItem, target, ref damage, ref knockBack, ref crit);
		}
		// for mod support idk
		public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit) {
			Player player = Main.player[projectile.owner];
			Item item = player.HeldItem;
			// run OnHitNPC for vanilla and modded
			VanillaMethod.OnHitNPC(item,player,target,projectile.Hitbox,damage,knockBack,true,crit);
		}
	}
	public class ItemProj : ModProjectile
	{
		public override string Texture => "Terraria/Item_0";
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Magical Item"); 
		}
		public override void SetDefaults() {
			projectile.width = 18;
			projectile.height = 18;
			projectile.aiStyle = 0;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.scale = 1f;
			projectile.extraUpdates = 1;
			projectile.timeLeft = 360;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = -1; 
		}
		public override void AI() {
			projectile.alpha += 10;
			if (projectile.alpha >= 255) {
				projectile.Recreate();
				projectile.Kill();
			}
			projectile.rotation = projectile.velocity.ToRotation();
			if (projectile.ai[1] == 1f || projectile.ai[1] == 2f ) {
				if (projectile.direction == -1) {
					projectile.rotation -= (float)Math.PI / 4f * (float)projectile.direction;
				}
				else {
					projectile.rotation += (float)Math.PI / 4f * (float)projectile.direction;
				}
			}
			if (projectile.ai[1] == 2f) {
				int index = -1;
				Vector2 targetCenter = projectile.Center;
				for (int i = 0; i < Main.maxNPCs; i++) {
					NPC npc = Main.npc[i];
					float between = Vector2.Distance(npc.Center, projectile.Center);
					bool closest = Vector2.Distance(projectile.Center, targetCenter) > between;
					if ((closest || index == -1) && npc.CanBeChasedBy() && npc.active && Vector2.Distance(npc.Center,projectile.Center) < 1000f) {
						index = i;
						targetCenter = npc.Center;
					}
				}
				if (index > -1) {
					projectile.velocity = Vector2.Lerp(projectile.velocity,projectile.DirectionTo(targetCenter)*4,0.02f);
				}
			}
			else {
				projectile.velocity *= 0.98f;
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			Texture2D texture = Main.itemTexture[(int)projectile.ai[0]];
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, texture.Size()/2f, projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}