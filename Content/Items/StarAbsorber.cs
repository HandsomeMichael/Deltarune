using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Deltarune.Helper;
using Deltarune.Content.Projectiles;

namespace Deltarune.Content.Items
{
	public class StarAbsorber : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Shoot a relatively fast and good looking star\n'The Stars Below'");
		}

		public override void SetDefaults() {
			item.autoReuse = true;
			item.channel = true; //Channel so that you can hold the weapon [Important]
			item.damage = 80;
			item.ranged = true;
			item.noMelee = true;
			item.knockBack = 1f;
			item.noUseGraphic = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 20;
			item.useAnimation = 20;
			item.shoot = ModContent.ProjectileType<StarAbsorberProj>();
			item.shootSpeed = 8f;
			item.rare = 2;
			item.value = Item.buyPrice(gold: 1);
			item.useStyle = 5;
		}
	}
	// reject cringe haha 20000 dps minishark, return to heldproj
	public class StarAbsorberProj : ModProjectile
	{
		public override string Texture => "Deltarune/Content/Items/StarAbsorber";
		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Star Absorber ( Sponsored by Raid Shadow Legend )");
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

		public float Charge {get => projectile.ai[0];set => projectile.ai[0] = value;}
		public float Phase {get => projectile.ai[1];set => projectile.ai[1] = value;}
		const float MaxCharge = 120f;
		public override void AI(){
			Player player = Main.player[projectile.owner];
			bool shouldDed = Phase == 1f ? false : !player.channel;
			if (!player.active || player.dead || player.noItems || player.CCed || shouldDed) {
				projectile.Kill();
				return;
			}
			UpdateGraphic(player);
			
			projectile.timeLeft = 5;

			if (Phase == 0f) {
				Charge += 1f;
				if (Charge > MaxCharge) {
					for (int i = -1; i < 2; i++){
						Vector2 pos = projectile.Center + projectile.velocity;
						Vector2 speed = (projectile.velocity*2f).RotatedBy(MathHelper.ToRadians(i*10));
						int a = Projectile.NewProjectile(pos,speed,ModContent.ProjectileType<starwalkerStarFriendly>(),projectile.damage,1,player.whoAmI);
						Main.projectile[a].penetrate = 1;
						NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, a);
					}
					Utils.PoofOfSmoke(projectile.Center);
					player.CameraShake(10);
					Main.PlaySound(Deltarune.GetSound("scytheburst"),projectile.Center);
					Phase = 1f;
				}
			}
			else {
				Charge -= 2f;
				if (Charge <= 0f) {
					projectile.Kill();
				}
			}

			float num = (Charge/(MaxCharge/2f))+1f;
			if (num > 1.5f) {num = 1.5f;}
			projectile.scale = num;
		}

		void UpdateGraphic(Player player){
			player.heldProj = projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.direction = projectile.direction;
			projectile.Center = player.Center;
			if (Phase == 0f) {
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
			else {
				player.itemRotation += player.direction == -1 ? 0.1f : -0.1f;
				projectile.rotation += player.direction == -1 ? 0.1f : -0.1f;
			}
		}
		public override bool ShouldUpdatePosition() => false;
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			SpriteEffects spriteEffects = projectile.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
			Texture2D texture = Main.projectileTexture[projectile.type];
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(Color.White), projectile.rotation, texture.Size()/2f, projectile.scale, spriteEffects, 0);
			float HalfCharge = MaxCharge/2f;
			if (Charge < HalfCharge) {return false;}
			float alpha = (Charge-HalfCharge)/HalfCharge;
			texture = ModContent.GetTexture(Texture+"_overlay");
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.White*alpha, projectile.rotation, texture.Size()/2f, projectile.scale, spriteEffects, 0);
			return false;
		}
	}
}
