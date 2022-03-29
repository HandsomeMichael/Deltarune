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
	public class SmollBraveAx : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Brave Ax"); 
			Tooltip.SetDefault("Summon a big axe\nStill work in progress");
		}

		public override void SetDefaults() {
			item.damage = 30;
			item.melee = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 20;
			item.useAnimation = 20;
			item.channel = true;
			item.useStyle = 1;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = 3;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.shootSpeed = 7f;
			timer = 600;
		}
		public int timer;
		public override void HoldItem(Player player) {
			timer++;
			if (timer == 599) {
				Projectile.NewProjectile(player.Center,Vector2.Zero,ModContent.ProjectileType<BraveAx>(),0,1f,player.whoAmI);
			}
			if (timer > 600) {timer = 600;}
		}
		public override bool AltFunctionUse(Player player) => true;
		public override bool CanUseItem(Player player) {
			if (player.altFunctionUse == 2){
				item.noUseGraphic = true;
				item.noMelee = true;
				item.useStyle = 5;
				timer = 0;
				//item.shoot = ModContent.ProjectileType<BraveAx>();
			}
			else{
				item.noMelee = false;
				item.noUseGraphic = false;
				item.useStyle = 1;
			}
			return base.CanUseItem(player);
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
	public class BraveAx : ModProjectile
	{
		public override void SetDefaults() {
			projectile.width = 100;
			projectile.height = 100;
			projectile.melee = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.hostile = false;
			projectile.aiStyle = -1;
		}
		public override void AI() {
			Player player = Main.player[projectile.owner];
			projectile.spriteDirection = projectile.direction;
			if (projectile.ai[0] == 2f) {
				projectile.alpha += 10;
				projectile.direction = projectile.velocity.X > 0 ? 1 : -1;
				if (projectile.alpha > 255) {
					projectile.Kill();
				}
				return;
			}
			if (projectile.ai[0] == 1f) {
				projectile.alpha += 10;
				if (projectile.alpha > 255) {
					projectile.Kill();
				}
				return;
			}
			projectile.alpha -= 10;
			if (projectile.alpha < 0) {projectile.alpha = 0;}
			if (player.dead) {projectile.ai[0] = 2f;}
			if (player.HeldItem.modItem != null && player.HeldItem.modItem is SmollBraveAx axe) {
				if (axe.timer < 599) {
					projectile.ai[0] = 2f;
					projectile.velocity = projectile.DirectionTo(Main.MouseWorld)*10f;
				}
			}
			else {
				projectile.ai[0] = 1f;
				return;
			}
			projectile.Center = Vector2.Lerp(projectile.Center,player.Center - new Vector2(0,50),0.1f);
			if (projectile.owner == Main.myPlayer) {
				projectile.direction = projectile.Center.X > Main.MouseWorld.X ? 1 : -1 ;
				projectile.rotation = MathHelper.Lerp(projectile.rotation,projectile.AngleTo(Main.MouseWorld),0.1f);
				projectile.rotation += MathHelper.ToRadians(30)*projectile.direction;
				projectile.netUpdate = true;
			}
		}
	}
}