using System;
using System.Collections.Generic;
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
using Deltarune.Content;

namespace Deltarune.Content.Items
{
	public abstract class ShortswordItem : ModItem
	{
		public virtual bool GlowMask => false;
		public virtual void CustomAI(Projectile projectile) {
		}
		public virtual bool PreAI(Projectile projectile) {
			return true;
		}
		public virtual bool PreDrawProj(Projectile projectile, SpriteBatch spriteBatch,Color lightColor) {
			return true;
		}
		public virtual void PostDrawProj(Projectile projectile, SpriteBatch spriteBatch,Color lightColor) {
		}
		public virtual void BleedEffect(NPC npc,Projectile projectile,ref int damage, ref float knockback,ref bool crit) {
		}
		public virtual void PreBleed(NPC npc,Projectile projectile,ref int damage, ref float knockback,ref bool crit) {
		}
		public virtual void ProjFirstTick(Projectile projectile) {
		}
		public void DefaultSet() {
			item.useTurn = false;
			item.GetDelta().Shortsword = true;
			item.useStyle = 5;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.scale = 0.8f;
			item.shoot = ModContent.ProjectileType<Content.Projectiles.BaseShortSword>();
			item.shootSpeed = 3f;
			item.autoReuse = true;
			item.width = 50;
			item.height = 18;
			item.UseSound = SoundID.Item1;
			item.melee = true;
			item.useAnimation = 13;
			item.useTime = 13;
		}
	}
}