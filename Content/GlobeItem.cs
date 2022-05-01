using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.ModLoader.Config;
using Deltarune.Helper;
using Deltarune.Content.Buffs;
using Deltarune.Content.Projectiles;
using Deltarune.Content.Items;
//this

namespace Deltarune.Content
{
	public class GlobeItem : GlobalItem
	{
		public override bool CloneNewInstances => true;
		public override bool InstancePerEntity => true;
		public bool Shortsword;
		//public override bool CanUseItem(Item item,Player player) {return base.CanUseItem(item,player);}
		bool IsModShortsword(Item item) {
			if (item.modItem != null && item.modItem is ShortswordItem) {
				return true;
			}
			return false;
		}
		public override void SetDefaults(Item item) {
			if (item.useStyle == 3 && !IsModShortsword(item)) {
				item.useTurn = false;
				Shortsword = true;
				item.useStyle = 5;
				item.noMelee = true;
				item.noUseGraphic = true;
				item.scale = 0.8f;
				item.shoot = ModContent.ProjectileType<Content.Projectiles.BaseShortSword>();
				item.shootSpeed = 3f;
			}
		}
		public override void ModifyWeaponDamage(Item item ,Player player, ref float add, ref float mult, ref float flat) {
			if (item.type == ItemID.Muramasa) {
				float vel = (player.velocity.X > 0 ? player.velocity.X : player.velocity.X*-1) + (player.velocity.Y > 0 ? player.velocity.Y : player.velocity.Y*-1);
				add += vel/8f;
			}
		}
		public bool requireResetMelee;
		public static void PostCanUseItem(Item item, Player player) {
			HeldSword.PostCanUseItem(item,player);
			if (item.type == ItemID.Muramasa) {
				if (player.altFunctionUse == 2) {
					player.velocity = player.DirectionTo(Main.MouseWorld)*20f;
					player.AddBuff(ModContent.BuffType<MuraDash>(),60*4);
				}
			}
		}
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
			Player player = Main.LocalPlayer;
			int speed = tooltips.FindIndex(tt => tt.Name.Equals("Speed") && tt.mod == "Terraria");
			int material = tooltips.FindIndex(tt => tt.Name.Equals("Material") && tt.mod == "Terraria");
			if (material > -1) {
				if (item.type == ItemID.Muramasa) {
					tooltips.Insert(material, new TooltipLine(mod, "MuraDash", "Damage scaled by player velocity\nRight Click to dash"));
				}
			}
		}
		public override bool AltFunctionUse(Item item,Player player) {
			if (item.type == ItemID.Muramasa) {
				return !player.HasBuff(ModContent.BuffType<MuraDash>());
			}
			return base.AltFunctionUse(item,player);
		}
		public override bool Shoot(Item item,Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
			if (Shortsword && (player.HasBuff(ModContent.BuffType<Shortwrath>()) ||
			 player.HasBuff(ModContent.BuffType<Stabwrath>()))) {//player.HasBuff(ModContent.BuffType<Shortwrath>()
				for (int i = 0; i < 4; i++)//Stabwrath
				{
					Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(20));
					float ai = (player.HasBuff(ModContent.BuffType<Stabwrath>()) ? 2f : 1f);
					Projectile.NewProjectile(position,perturbedSpeed*(1f+((float)player.GetDelta().Shortswordatt/10f)), ModContent.ProjectileType<ItemProj>(), damage/4, knockBack, player.whoAmI, item.type, ai);	
				}
			}
			return base.Shoot(item,player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}
	}
}