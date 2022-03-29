using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Deltarune.Content.Items
{
	public class Spookysword : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Spooky Sword"); 
			Tooltip.SetDefault("'spooky'");
		}

		public override void SetDefaults() 
		{
			item.damage = 30;
			item.melee = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = 1;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = 3;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.shootSpeed = 7f;
		}
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