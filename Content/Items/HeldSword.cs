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
			Tooltip.SetDefault("Quickly slashesh enemy\n'spooky'");
		}
		public override void SafeSetDefaults() {
			item.damage = 30;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = 3;
			item.useAnimation = 20;
		}
		public override void ChangeRotation(Player player,Projectile projectile,ref int minRot,ref int maxRot) {
			minRot = 80;
			maxRot = 80;
		}
		public override void Initialize(Player player,Projectile projectile) {
			projectile.extraUpdates = 1;
		}
	}
	// contains a smol amount of hooks
	public abstract class HeldSwordItem : ModItem
	{
		public override void SetDefaults() {
			item.melee = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.width = 40;
			item.height = 40;
			item.useStyle = 5;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.shootSpeed = 7f;
			item.shoot = ModContent.ProjectileType<HeldSword>();
			SafeSetDefaults();
			item.useTime = item.useAnimation;
		}
		/// <summary>
		/// for setdefaults
		/// </summary>
		public virtual void SafeSetDefaults() {
			
		}
		/// <summary>
		/// Called after center , rotation and velocity is updated
		/// </summary>
		/// <param name="player"> the projectile owner </param>
		/// <param name="projectile"> the held sword </param>
		public virtual void PostUpdateCenter(Player player,Projectile projectile) {

		}
		/// <summary>
		/// Called before center , rotation and velocity is updated
		/// </summary>
		/// <param name="player"> the projectile owner </param>
		/// <param name="projectile"> the held sword </param>
		/// <param name="speed"> the rotation speed </param>
		public virtual void PreUpdateCenter(Player player,Projectile projectile,ref float speed) {

		}
		/// <summary>
		/// Change projectile rotation direction. called after Initialize and rotation checking
		/// </summary>
		/// <param name="player"> the projectile owner </param>
		/// <param name="projectile"> the held sword </param>
		/// <param name="minRot"> the minimum swing rotation, in degree </param>
		/// <param name="maxRot"> the maximum swing rotation, in degree</param>
		public virtual void ChangeRotation(Player player,Projectile projectile,ref int minRot,ref int maxRot) {

		}
		/// <summary>
		/// Initialize stuff
		/// </summary>
		/// <param name="player"> the projectile owner </param>
		/// <param name="projectile"> the held sword </param>
		public virtual void Initialize(Player player,Projectile projectile) {

		}
		/// <summary>
		/// Should draw trails or not 
		/// </summary>
		/// <param name="player"> the projectile owner </param>
		/// <param name="projectile"> the held sword </param>
		public virtual bool ShouldDrawTrails(Player player,Projectile projectile) {
			return true;
		}
		/// <summary>
		/// Draw something after the sword is drawn
		/// </summary>
		/// <param name="spriteBatch"> the spriteBatch </param>
		/// <param name="projectile"> the held sword </param>
		/// <param name="pos"> the draw position </param>
		/// <param name="rot"> the draw rotation </param>
		public virtual void PostDrawSword(SpriteBatch spriteBatch,Projectile projectile,Vector2 pos, float rot) {

		}
		/// <summary>
		/// Draw something before the sword is drawn. return false to not draw sword
		/// </summary>
		/// <param name="spriteBatch"> the spriteBatch </param>
		/// <param name="projectile"> the held sword </param>
		/// <param name="color"> the sword color </param>
		/// <param name="texture"> the sword texture </param>
		/// <param name="spriteEffects"> the sword spriteEffects </param>
		/// <param name="pos"> the draw position </param>
		/// <param name="rot"> the draw rotation </param>
		public virtual bool PreDrawSword(SpriteBatch spriteBatch,Projectile projectile,
		ref Color color,ref Texture2D texture,ref float rot,ref Vector2 pos,ref SpriteEffects effect) {
			return true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack){
			// making the use of useless item field. just like weapon out mod
			item.isBeingGrabbed = !item.isBeingGrabbed;
			return true;
		}
	}
	// yes i like the idea of 1 projectile , 100 weapon :thiscat:
	public class HeldSword : ModProjectile
	{
		// no texture
		public override string Texture => "Terraria/Item_0";

		// just heldsword lol
		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Held Sword");
		}
		public override void SetDefaults(){
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.melee = false;
			projectile.width = 64;
			projectile.height = 64;
			projectile.aiStyle = -1;
			projectile.timeLeft = 60;
			projectile.tileCollide = false;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 35;
		}
		// for mod support idk
		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockBack, ref bool crit, ref int hitDirection) {
			Player player = Main.player[projectile.owner];
			//allow armor penetration
			if (player.armorPenetration > 0){damage += target.checkArmorPenetration(player.armorPenetration);}
			// call modloader stuff because there are no vanilla modifyhitnpc code
			ItemLoader.ModifyHitNPC(player.HeldItem,player,target,ref damage, ref knockBack, ref crit);
			NPCLoader.ModifyHitByItem(target, player, player.HeldItem, ref damage, ref knockBack, ref crit);
			PlayerHooks.ModifyHitNPC(player, player.HeldItem, target, ref damage, ref knockBack, ref crit);
		}
		// for mod support idk
		public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit) {
			Player player = Main.player[projectile.owner];
			Item item = player.HeldItem;
			// run OnHitNPC for vanilla and modded
			VanillaMethod.OnHitNPC(item,player,target,projectile.Hitbox,damage,knockBack,true,crit);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox){
			Player player = Main.player[projectile.owner];
			Vector2 end = projectile.Center + projectile.velocity*projectile.height;
			float point = 0f;
			if (!Collision.CanHit(player.Center, 1, 1, end, 1, 1)) {return false;}
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center, end);
			//return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center, projectile.Center + projectile.velocity*projectile.height, 24f,ref point);
		}

		// we do a huge amount of trolling in variable naming
		public float Balls {get => projectile.ai[0];set => projectile.ai[0] = value;} // the max rot
		Trail trailsDeezNuts = new Trail(); // trail deez nuts in your mouth
		
		public override void AI(){
			// player
			Player player = Main.player[projectile.owner];

			// kill if player is ded or rotation is reached the limit
			bool shouldDed = false;
			if (projectile.melee) {
				shouldDed = Helpme.RotationDistance(projectile.rotation,Balls) < 15f;
			}

			// all of the crap is handled in Helpme.cs 
			if (player.CanHeldProj(false) || shouldDed) {
				projectile.Kill();
				return;
			}

			HeldSwordItem item = player.HeldItem.modItem as HeldSwordItem;

			// initialize
			if (!projectile.melee) {
				projectile.melee = true;
				// setting direction
				projectile.spriteDirection = (player.HeldItem.isBeingGrabbed ? -1 : 1) * projectile.spriteDirection;

				//setting rotation
				projectile.rotation = projectile.velocity.ToRotation();
				int minRot = 80 + player.HeldItem.useAnimation;
				int maxRot = 80 + player.HeldItem.useAnimation;

				// use hook that modify rotation
				if (item != null) {
					item.ChangeRotation(player,projectile,ref minRot,ref maxRot);
					item.Initialize(player,projectile);
				}

				// setting max and min rot
				Balls = projectile.rotation + MathHelper.ToRadians(maxRot)*projectile.spriteDirection;
				projectile.rotation -= MathHelper.ToRadians(minRot)*projectile.spriteDirection;
				projectile.spriteDirection = projectile.velocity.X > 1f ? 1 : -1;
			}
			// update projectile
			UpdateGraphic(player,item);
			projectile.timeLeft = 5;
		}

		void UpdateGraphic(Player player,HeldSwordItem item){
			// update basic stuff
			player.heldProj = projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.direction = projectile.direction;

			// use hook
			float speed = 0.15f;
			// calculate speed for speedy weapon
			if (player.HeldItem.useAnimation < 25) {
				speed = (1f - ((float)player.HeldItem.useAnimation/25f));
			}
			if (item != null) {
				item.PreUpdateCenter(player,projectile,ref speed);
			}

			// scale based on item scale
			projectile.scale = player.HeldItem.scale;

			// make hitbox based on width and height
			float width = Main.itemTexture[player.HeldItem.type].Width;
			float height = Main.itemTexture[player.HeldItem.type].Height;
			var cache = projectile.Center;
			projectile.width = (int)(width*projectile.scale);
			projectile.height = (int)(height*projectile.scale);
			projectile.Center = cache;

			// run melee effects after hitbox is properly calculated
			VanillaMethod.MeleeEffects(player.HeldItem,player,projectile.Hitbox,true);

			// update rotation and pos
			projectile.rotation = MathHelper.Lerp(projectile.rotation,Balls,speed);
			projectile.velocity = projectile.rotation.ToRotationVector2()*0.9f;
			projectile.Center = player.Center + projectile.velocity*15f;

			// use hook
			if (item != null) {
				item.PostUpdateCenter(player,projectile);
			}

			// update player rotation based on velocity
			player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * player.direction, projectile.velocity.X * player.direction);

			//store deez nuts
			int dir = projectile.spriteDirection;
			if (player.HeldItem.isBeingGrabbed) {dir = -1;}
			float rot = projectile.velocity.ToRotation() + (MathHelper.ToRadians(30) * dir);
			if (dir == -1) {rot += MathHelper.ToRadians(180);}
			trailsDeezNuts.Update(player.Center + projectile.velocity,rot,dir,20,1);
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {

			//get player
			Player player = Main.player[projectile.owner];
			if (!player.active || player.dead || player.HeldItem.IsAir || player.noItems || player.CCed) {return false;}
			HeldSwordItem item = player.HeldItem.modItem as HeldSwordItem;

			// var setup
			int dir = (player.HeldItem.isBeingGrabbed ? -1 : 1) * projectile.spriteDirection;
			//if (player.HeldItem.isBeingGrabbed) {dir = dir*-1;}
			SpriteEffects spriteEffects = dir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Texture2D texture = Main.itemTexture[player.HeldItem.type];
			float rot = projectile.velocity.ToRotation() + (MathHelper.ToRadians(30) * dir);
			// rotate 180 degree if the sword is when its the right side.
			if (dir == -1) {rot += MathHelper.ToRadians(180);}
			Vector2 pos = player.Center + projectile.velocity - Main.screenPosition;
			Color color = projectile.GetAlpha(lightColor);

			// calculate orig
			Vector2 orig = dir == 1 ? new Vector2(0,texture.Height) : new Vector2(texture.Width,texture.Height);

			// draw trails
			bool shouldTrail = true;
			if (item != null) {shouldTrail = item.ShouldDrawTrails(player,projectile);}
			if (shouldTrail) {
				for (int k = 0; k < trailsDeezNuts.Count; k++) {
					float alpha = (float)k / (float)trailsDeezNuts.Count;
					Color amoogs = (projectile.GetAlpha(lightColor)*alpha)*0.2f;
					var eff = trailsDeezNuts[k].direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
					Vector2 orig2 = trailsDeezNuts[k].direction == 1 ? new Vector2(0,texture.Height) : new Vector2(texture.Width,texture.Height);
					spriteBatch.Draw(texture, trailsDeezNuts[k].position - Main.screenPosition, null, amoogs, trailsDeezNuts[k].rotation, orig2, projectile.scale, eff, 0f);
				}
			}

			// use hook
			if (item != null) {
				if (!item.PreDrawSword(spriteBatch,projectile,ref color,ref texture,ref rot,ref pos,ref spriteEffects)) {return false;}
			}

			// draw
			spriteBatch.Draw(texture, pos, null, color, rot, orig, projectile.scale, spriteEffects, 0);

			// use hoook
			if (item != null) {
				item.PostDrawSword(spriteBatch,projectile,pos,rot);
			}

			// dont draw vanilla way
			return false;
		}

		// custom method
		public static void PostCanUseItem(Item item,Player player) {
			// reset melee defaults
			if (item.GetDelta().requireResetMelee) {
				item.noUseGraphic = false;
				item.noMelee = false;
				item.useStyle = 1;
			}
			// call melee stuff
			if (item.melee && item.useStyle == 1 && !item.noUseGraphic && !item.noMelee) {
				item.GetDelta().requireResetMelee = true;
				item.isBeingGrabbed = !item.isBeingGrabbed;
				item.useStyle = 5;
				item.noUseGraphic = true;
				item.noMelee = true;
				float speed = 5f;
				Vector2 vel = player.DirectionTo(Main.MouseWorld)*speed;
				int damage = player.GetWeaponDamage(item);
				int type = ModContent.ProjectileType<HeldSword>();
				Projectile.NewProjectile(player.Center,vel, type, damage, player.GetWeaponKnockback(item,item.knockBack), player.whoAmI, 0, 0);
			}
		}
	}
}