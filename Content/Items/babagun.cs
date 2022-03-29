using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Deltarune.Helper;
using Deltarune.Content.Projectiles;

namespace Deltarune.Content.Items
{
	public class babagun : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Babagun"); 
			Tooltip.SetDefault("Shoot out a spread of frozed bullet\n<right> to shoot bullet tracker");
		}

		public override void SetDefaults() 
		{
			item.damage = 10;
			item.ranged = true; 
			item.width = 40; 
			item.height = 20; 
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.HoldingOut; 
			item.noMelee = true;
			item.knockBack = 4;
			item.value = 10000; 
			item.rare = 2; 
			item.UseSound = SoundID.Item11;
			item.autoReuse = true; 
			item.shoot = 10; 
			item.shootSpeed = 10f; 
			item.useAmmo = AmmoID.Bullet; 
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack){
			player.GetDelta().richochetBullet = null;
			if (player.altFunctionUse == 2) {
				type = ModContent.ProjectileType<homingHart>();
				return true;
			}
			for (int i = -2; i < 2; i++)
			{
				Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(i*10)); // 30 degree spread.
				int a = Projectile.NewProjectile(position,perturbedSpeed,type,damage,knockBack,player.whoAmI);
				Main.projectile[a].GetDelta().babagun = true;
				Main.projectile[a].tileCollide = false;
			}
			return false; // return false because we don't want tmodloader to shoot projectile
		}
		public override bool CanUseItem(Player player) {
			if (player.altFunctionUse == 2){
				item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/RPGShoot").WithVolume(.3f);
			}
			else{
				item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/SIGP226Shoot").WithVolume(.3f);
			}
			return base.CanUseItem(player);
		}
		public override bool AltFunctionUse(Player player) => true;
		/*
		public override bool CanRightClick() => true;
		public override void RightClick(Player player) {}
		public override bool ConsumeItem(Player player) => false;

		public override void UpdateInventory(Player player) {}
		public override bool AltFunctionUse(Player player) {return true;}
		public override bool UseItem(Player player){
			if (player.altFunctionUse == 2){}
			return true;
		}
		public override bool CanUseItem(Player player) {
			if (player.altFunctionUse == 2){}
			else{}
			return base.CanUseItem(player);
		}
		// Shotgun style: Multiple Projectiles, Random spread 
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack){
			return true; // return false because we don't want tmodloader to shoot projectile
		}*/
	}
}