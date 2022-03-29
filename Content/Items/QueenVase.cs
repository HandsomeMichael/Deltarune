using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace Deltarune.Content.Items
{
	public class QueenVase : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("'real'");
		}

		public override void SetDefaults() {
			item.damage = 100;
			item.thrown = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 20;
			item.shoot = ModContent.ProjectileType<QueenVaseProj>();
			item.shootSpeed = 8f;
			item.useAnimation = 20;
			item.knockBack = 6;
			item.value = Item.buyPrice(gold: 1);
			item.rare = 8;
			item.UseSound = SoundID.Item16;
			item.autoReuse = true;
			item.crit = 6; 
			item.useStyle = 5;
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ShroomiteBar, 10);
			recipe.AddIngredient(ItemID.SoulofFright, 5);
			recipe.AddIngredient(ItemID.SoulofMight, 5);
			recipe.AddIngredient(ItemID.SoulofSight, 5);
			recipe.AddIngredient(ItemID.PinkVase);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class QueenVaseProj : ModProjectile
	{//AiStyle = 24 for small and then gone
		public override string Texture => "Deltarune/Content/Items/QueenVase";
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Queen Vase ( Real )");     //The English name of the projectile
		}

		public override void SetDefaults() {
			projectile.CloneDefaults(ProjectileID.Shuriken);
			projectile.penetrate = 1;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			projectile.timeLeft = 600;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			projectile.ignoreWater = false;          //Does the projectile's speed be influenced by water?
			projectile.tileCollide = true;          //Can the projectile collide with tiles?
			projectile.extraUpdates = 1;            //Set to above 0 if you want the projectile to update multiple time in a frame
		}
		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
			target.AddBuff(BuffID.OnFire,60*3);
			if (!target.boss) {
				damage += target.life/20;
			}
			else {
				crit = true;
			}
		}
		public override void AI() {
			projectile.rotation += 0.1f;
			if (Deltarune.Boss) {
				projectile.rotation += 0.1f;
			}
		}
		public override void Kill(int timeLeft)
		{
			Explode.New(projectile.Center,0.5f);
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
		}
	}
}
