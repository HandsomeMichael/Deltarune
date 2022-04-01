using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

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
			projectile.scale = 1.5f;
		}

		public float Charge {get => projectile.ai[0];set => projectile.ai[0] = value;}
		public override void AI(){
			Player player = Main.player[projectile.owner];

			if (player.active || player.dead || player.noItems || player.CCed || !player.channel) {
				projectile.Kill();
				return;
			}
			UpdateGraphic(player);
			if (projectile.owner == Main.myPlayer){
				projectile.velocity = player.RotatedRelativePoint(player.MountedCenter, true).DirectionTo(Main.MouseWorld)*3f;
				projectile.netUpdate = true;
			}
			Charge += 1f;
			projectile.timeLeft = 2;
			if (Charge > 120f) {
				Charge = 120f;
			}
		}

		void UpdateGraphic(Player player){
			projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true);
			projectile.rotation = projectile.velocity.ToRotation();
			projectile.spriteDirection = projectile.direction;
			player.ChangeDir(projectile.direction);
			player.heldProj = projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();

			if (projectile.soundDelay <= 0) {
				projectile.soundDelay = 30;
				if (Charge > 1f) {Main.PlaySound(SoundID.Item15, projectile.position);}
			}
		}
		public override bool ShouldUpdatePosition() => false;
	}
}
