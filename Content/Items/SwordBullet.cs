using Terraria;
using Terraria.ID;
using Deltarune;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deltarune.Content.Items
{
	public class SwordBulletItem : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Sword Bullet");
			Tooltip.SetDefault("Split into 3 swords");
		}
		public override void SetDefaults() {
			item.damage = 5;
			item.ranged = true;
			item.width = 8;
			item.height = 8;
			item.maxStack = 3996;
			item.consumable = true;
			item.knockBack = 1.5f;
			item.value = 10;
			item.rare = 1;
			item.shoot = ModContent.ProjectileType<SwordBullet>();
			item.shootSpeed = 16f;
			item.ammo = AmmoID.Bullet;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.TissueSample, 1);
			recipe.AddIngredient(ItemID.Leather, 1);
			recipe.AddIngredient(ItemID.ThrowingKnife, 10);
			recipe.AddIngredient(ItemID.MusketBall, 10);
			recipe.SetResult(this, 50);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ShadowScale, 1);
			recipe.AddIngredient(ItemID.Leather, 1);
			recipe.AddIngredient(ItemID.ThrowingKnife, 10);
			recipe.AddIngredient(ItemID.MusketBall, 10);
			recipe.SetResult(this, 50);
			recipe.AddRecipe();
		}
	}
	public class SwordBulletEndless : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Endless Sword Bullet");
			Tooltip.SetDefault("Split into 3 swords");
		}
		public override void SetDefaults() {
			item.CloneDefaults(ModContent.ItemType<SwordBulletItem>());
			item.consumable = false;
			item.rare += 1;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<SwordBulletItem>(), 3996);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
	public class SwordBullet : ModProjectile
	{
		//me when copy pasted from example mod
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Sharp Sword Bullet");     
		}
		public override void SetDefaults() {
			projectile.width = 8;               //The width of projectile hitbox
			projectile.height = 8;              //The height of projectile hitbox
			projectile.aiStyle = -1;             //The ai style of the projectile, please reference the source code of Terraria
			projectile.friendly = true;         //Can the projectile deal damage to enemies?
			projectile.hostile = false;         //Can the projectile deal damage to the player?
			projectile.ranged = true;           //Is the projectile shoot by a ranged weapon?
			projectile.alpha = 255;
			projectile.penetrate = -1;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			projectile.timeLeft = 600;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			//projectile.light = 0.5f;            //How much light emit around the projectile
			projectile.ignoreWater = true;          //Does the projectile's speed be influenced by water?
			projectile.tileCollide = true;          //Can the projectile collide with tiles?
			projectile.extraUpdates = 1;            //Set to above 0 if you want the projectile to update multiple time in a frame
		}
		public override void AI() {
			if (projectile.ai[1] == 0f) {
				projectile.position += projectile.velocity*2.5f;
				for (int i = 0; i < 3; i++){
					Vector2 vel = (projectile.velocity/3f).RotatedByRandom(MathHelper.ToRadians(30)); 
					Projectile.NewProjectile(projectile.Center, vel, projectile.type, projectile.damage/2, projectile.knockBack, projectile.owner,0f,1f);
				}
				projectile.Kill();
			}
			if (projectile.ai[0] == 1f) {
				projectile.velocity.Y += 0.1f;
				if (projectile.velocity.Y > 10f) {projectile.velocity.Y = 10f;}
				if (projectile.direction == 1) {projectile.rotation += MathHelper.ToRadians(Main.rand.Next(10,21));}
				else {projectile.rotation -= MathHelper.ToRadians(Main.rand.Next(10,21));}
				projectile.alpha += 15;
				if (projectile.alpha >= 255) {
					projectile.ai[1] = 0f;
					projectile.Kill();
				}
				return;
			}
			projectile.velocity.Y += 0.01f;
			projectile.alpha -= 80;
			// = projectile.spriteDirection = projectile.velocity.X > 0f ? 1 : -1;
			projectile.rotation = projectile.velocity.ToRotation();
			projectile.rotation += MathHelper.ToRadians(45);
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
			if (projectile.ai[0] == 0f) {
				projectile.alpha = 0;
				projectile.velocity /= 2f;
				target.immune[projectile.owner] = 0;
				projectile.friendly = false;
				projectile.ai[0] = 1f;
			}
		}
		public override void Kill(int timeLeft) {
			if (projectile.ai[1] == 0f) {
				return;
			}
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(SoundID.Item10, projectile.position);
		}
	}
}
