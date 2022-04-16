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
			item.noUseGraphic = true;
			item.noMelee = true;
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
			item.shoot = ModContent.ProjectileType<SpookyswordProj>();
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
	public class SpookyswordProj : ModProjectile
	{
		public override string Texture => "Deltarune/Content/Items/Spookysword";
		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Spooky Sword Lmao");
		}

		public override void SetDefaults(){
			projectile.hostile = false;
			projectile.friendly = false;
			projectile.penetrate = -1;
			projectile.ranged = true;
			projectile.width = 64;
			projectile.height = 64;
			projectile.aiStyle = -1;
			projectile.timeLeft = 60;
			projectile.tileCollide = false;
			projectile.extraUpdates = 1;
		}

		// note to gabehaswon : shudup , i can name variable whatevver i want, look a me go hahahaahah
		public float Piss {get => projectile.ai[0];set => projectile.ai[0] = value;}
		public float Balls {get => projectile.ai[1];set => projectile.ai[1] = value;}
		const float MaxPiss = 60f;
		
		public override void AI(){
			Player player = Main.player[projectile.owner];
			bool shouldDed = Piss > MaxPiss;
			if (!player.active || player.dead || player.noItems || player.CCed || shouldDed) {
				projectile.Kill();
				return;
			}
			if (Piss == 0f) {
				projectile.rotation = projectile.AngleTo(Main.MouseWorld);
				projectile.rotation += MathHelper.ToRadians(90)*projectile.direction;
			}
			Piss++;
			UpdateGraphic(player);
			projectile.timeLeft = 5;
		}

		void UpdateGraphic(Player player){
			player.heldProj = projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.direction = projectile.direction;
			projectile.Center = player.Center;
			projectile.rotation += MathHelper.ToRadians(5)*player.direction;
			if (projectile.owner == Main.myPlayer) {
				projectile.velocity = projectile.rotation.ToRotationVector2()*10f;
				player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * player.direction, projectile.velocity.X * player.direction);
				projectile.Center += projectile.velocity*2;
				projectile.netUpdate = true;
			}
		}
		public override bool ShouldUpdatePosition() => false;
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			SpriteEffects spriteEffects = projectile.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
			Texture2D texture = Main.projectileTexture[projectile.type];
			Vector2 orig = texture.Size()/2f;
			//Vector2 orig = new Vector2(0,texture.Height);
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, orig, projectile.scale, spriteEffects, 0);
			return false;
		}
	}
}