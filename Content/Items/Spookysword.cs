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
using Deltarune.Helper;

namespace Deltarune.Content.Items
{
	public class Spookysword : HeldSwordItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Spooky Sword"); 
			Tooltip.SetDefault("'spooky'");
		}
		public override void SafeSetDefaults() {
			item.damage = 30;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = 3;
		}
		public override void PreUpdateCenter(Player player,Projectile projectile) => AddAI(projectile,1);
	}
	public abstract class HeldSwordItem : ModItem
	{
		public override void SetDefaults() {
			item.melee = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = 5;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.shootSpeed = 7f;
			item.shoot = ModContent.ProjectileType<HeldSword>();
			SafeSetDefaults();
		}
		public void AddAI(Projectile projectile,float Piss = 0f,float Balls = 0f) {
			projectile.ai[0] += Piss;
			projectile.ai[1] += Balls;
		}
		public virtual void SafeSetDefaults() {
			
		}
		public virtual void PostUpdateCenter(Player player,Projectile projectile) {

		}
		public virtual void PreUpdateCenter(Player player,Projectile projectile) {

		}
		public virtual void InitializeRot(Player player,Projectile projectile,ref int minRot,ref int maxRot) {

		}
		public virtual void ModifyMaxPiss(Player player,Projectile projectile,ref float MaxPiss) {

		}
		public virtual void PostDrawSword(SpriteBatch spriteBatch,Projectile projectile,Vector2 pos, float rot) {

		}
		public virtual bool PreDrawSword(SpriteBatch spriteBatch,Projectile projectile,ref Color color,ref Texture2D texture,ref float rot,ref Vector2 pos,ref SpriteEffects spriteEffects) {
			return true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack){
			item.isBeingGrabbed = !item.isBeingGrabbed;
			return true;
		}
	}
	public class HeldSword : ModProjectile
	{
		// just heldsword lol
		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Held Sword");
		}

		public override void SetDefaults(){
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.melee = true;
			projectile.width = 64;
			projectile.height = 64;
			projectile.aiStyle = -1;
			projectile.timeLeft = 60;
			projectile.tileCollide = false;
			projectile.extraUpdates = 2;
			// projectile has a huge cooldown
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 35;
		}

		// for mod support idk
		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
			Player player = Main.player[projectile.owner];
			ItemLoader.ModifyHitNPC(player.HeldItem,player,target,ref damage, ref knockback, ref crit);
		}
		// for mod support idk
		public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit) {
			Player player = Main.player[projectile.owner];
			ItemLoader.OnHitNPC(player.HeldItem,player, target, damage, knockBack, crit);
		}

		// we do a huge amount of trolling
		public float Piss {get => projectile.ai[0];set => projectile.ai[0] = value;}
		public float Balls {get => projectile.ai[1];set => projectile.ai[1] = value;}
		// the maximum amount of piss
		const float maxPiss = 30f;
		
		public override void AI(){
			// player
			Player player = projectile.Owner();

			// kill if player is ded
			if (!player.active || player.dead || player.noItems || player.CCed || player.HeldItem.IsAir) {
				projectile.Kill();
				return;
			}

			HeldSwordItem item = player.HeldItem.modItem as HeldSwordItem;

			// check for maximum amount of piss
			float MaxPiss = maxPiss;
			// use hook
			if (item != null) {
				item.ModifyMaxPiss(player,projectile,ref MaxPiss);
			}
			// kill
			bool shouldDed = Piss > MaxPiss;
			if (shouldDed) {projectile.Kill();return;}

			// initialize
			if (Piss == 0f) {
				// setting direction
				projectile.spriteDirection = player.direction;
				projectile.spriteDirection = (item.HeldItem.isBeingGrabbed ? 1 : -1) * projectile.spriteDirection;

				//setting rotation
				projectile.rotation = projectile.velocity.ToRotation();

				int minRot = 90;
				int maxRot = 90;

				// use hook that modify rotation
				if (item != null) {
					item.InitializeRot(player,projectile,ref minRot,ref maxRot);
				}

				// setting max and min rot
				Balls = projectile.rotation + MathHelper.ToRadians(maxRot)*projectile.spriteDirection;
				projectile.rotation -= MathHelper.ToRadians(minRot)*projectile.spriteDirection;
				projectile.spriteDirection = projectile.velocity.X > 1f ? 1 : -1;
			}
			// update projectile
			Piss++;
			UpdateGraphic(player);
			projectile.timeLeft = 5;
		}

		void UpdateGraphic(Player player,HeldSwordItem item){
			// update basic stuff
			player.heldProj = projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.direction = projectile.direction;

			// use hook
			if (item != null) {
				item.PreUpdateCenter(player,projectile);
			}

			// update rotation and pos
			projectile.rotation = MathHelper.Lerp(projectile.rotation,Balls,0.1f);
			projectile.velocity = projectile.rotation.ToRotationVector2()*0.9f;
			projectile.Center = player.Center + projectile.velocity*15f;

			// use hook
			if (item != null) {
				item.PostUpdateCenter(player,projectile);
			}

			// update player rotation based on velocity
			player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * player.direction, projectile.velocity.X * player.direction);
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {

			//get player
			Player player = projectile.Owner();
			if (!player.active || player.dead || player.noItems || player.CCed) {return false;}
			Item item = player.HeldItem;

			// var setup
			SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Texture2D texture = Main.itemTexture[item.type];
			float rot = projectile.velocity.ToRotation() + (MathHelper.ToRadians(30) * projectile.spriteDirection);
			if (projectile.spriteDirection == -1) {rot += MathHelper.ToRadians(180);}
			Vector2 pos = player.Center + projectile.velocity - Main.screenPosition;
			Color color = projectile.GetAlpha(lightColor);

			// use hook
			if (item.modItem is HeldSwordItem) {
				var sword = item.modItem as HeldSwordItem;
				if (!sword.PreDrawSword(spriteBatch,projectile,ref color,ref texture,ref rot,ref pos,ref spriteEffects)) {
					return false;
				}
			}

			// draw
			Vector2 orig = projectile.spriteDirection == 1 ? new Vector2(0,texture.Height) : new Vector2(texture.Width,texture.Height);
			spriteBatch.Draw(texture, pos, null, color, rot, orig, projectile.scale, spriteEffects, 0);

			// use hoook
			if (item.modItem is HeldSwordItem) {
				var sword = item.modItem as HeldSwordItem;
				sword.PostDrawSword(spriteBatch,projectile,pos,rot);
			}

			// dont draw vanilla way
			return false;
		}
	}
}