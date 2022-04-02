using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Deltarune.Helper;

namespace Deltarune.Content.Items
{
	public class StarAbsorber : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Charge a death star");
		}

		public override void SetDefaults() {
			item.channel = true; //Channel so that you can hold the weapon [Important]
			item.damage = 100;
			item.ranged = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 20;
			item.useAnimation = 20;
			item.shoot = ModContent.ProjectileType<StarAbsorberProj>();
			item.shootSpeed = 8f;
			item.rare = 8;
			item.value = Item.buyPrice(gold: 1);
			item.useStyle = 5;
		}
	}
	public class StarAbsorberProj : ModProjectile
	{
		public override string Texture => "Deltarune/Content/Items/StarAbsorber";
		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Star Absorber ( Sponsored by Raid Shadow Legend )");
		}

		public override void SetDefaults(){
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.ranged = true;
			projectile.width = 64;
			projectile.height = 64;
			projectile.aiStyle = -1;
			projectile.timeLeft = 60;
			projectile.tileCollide = false;
			projectile.extraUpdates = 1;
		}

		public float Charge {get => projectile.ai[0];set => projectile.ai[0] = value;}
		const float MaxCharge = 120f;
		public override void AI(){
			Player player = Main.player[projectile.owner];

			if (!player.active || player.dead || player.noItems || player.CCed || !player.channel) {
				projectile.Kill();
				return;
			}
			UpdateGraphic(player);
			Charge += 1f;
			projectile.timeLeft = 5;
			float num = (Charge/(MaxCharge/2f))+1f;
			if (num > 1.5f) {num = 1.5f;}
			projectile.scale = num;
			if (Charge > MaxCharge) {
				Charge = MaxCharge;
			}
		}

		void UpdateGraphic(Player player){
			player.heldProj = projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.direction = projectile.direction;
			projectile.Center = player.Center;
			if (projectile.owner == Main.myPlayer) {
				projectile.rotation = projectile.AngleTo(Main.MouseWorld);
				projectile.velocity = projectile.DirectionTo(Main.MouseWorld)*10f;
				player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * player.direction, projectile.velocity.X * player.direction);
				projectile.Center += projectile.velocity*2;
				projectile.netUpdate = true;
			}
			if (projectile.soundDelay <= 0) {
				projectile.soundDelay = 30;
				if (Charge > 1f) {Main.PlaySound(SoundID.Item15, projectile.position);}
			}
		}
		public override bool ShouldUpdatePosition() => false;
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			SpriteEffects spriteEffects = projectile.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
			Texture2D texture = Main.projectileTexture[projectile.type];
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(Color.White), projectile.rotation, texture.Size()/2f, projectile.scale, spriteEffects, 0);
			if (Charge < 60f) {return false;}
			float alpha = (Charge-60f)/60f;
			texture = ModContent.GetTexture(Texture+"_overlay");
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.White*alpha, projectile.rotation, texture.Size()/2f, projectile.scale, spriteEffects, 0);
			return false;
		}
	}
}
