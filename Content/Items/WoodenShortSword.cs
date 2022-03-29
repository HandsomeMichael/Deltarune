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
using Deltarune;
using Deltarune.Helper;
using Deltarune.Content;
using Deltarune.Content.Buffs;

namespace Deltarune.Content.Items
{
	public class WoodenShortSword : ShortswordItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Wooden Shortsword"); 
			Tooltip.SetDefault("Increase knockback based on player velocity");
		}
		public override void SetDefaults() {
			DefaultSet();
			item.damage = 5;
			item.knockBack = 1f;
		}
		public override void ModifyHitNPC(Player player,NPC  npc,ref int damage, ref float knockback, ref bool crit) {
			float vel = (player.velocity.X > 0 ? player.velocity.X : player.velocity.X*-1) + (player.velocity.Y > 0 ? player.velocity.Y : player.velocity.Y*-1);
			knockback += vel/4f;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Wood, 6);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class EbonWoodShortSword : ShortswordItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Ebonwood Shortsword"); 
			Tooltip.SetDefault("Increase stab effect speed");
		}
		public override void SetDefaults() {
			DefaultSet();
			item.damage = 8;
			item.knockBack = 1.2f;
		}
		public override void PreBleed(NPC npc,Projectile projectile,ref int damage, ref float knockback,ref bool crit) {
			npc.GetDelta().ShortswordBleed += 0.3;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Ebonwood, 6);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class ShadeWoodShortSword : ShortswordItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Shadewood Shortsword"); 
			Tooltip.SetDefault("Increase stab effect damage");
		}
		public override void SetDefaults() {
			DefaultSet();
			item.damage = 9;
			item.knockBack = 1.1f;
		}
		public override void PreBleed(NPC npc,Projectile projectile,ref int damage, ref float knockback,ref bool crit) {
			damage += (int)(npc.GetDelta().ShortswordBleed/2);
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Wood, 6);
			recipe.AddTile(TileID.Shadewood);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class RichMahoganyShortSword : ShortswordItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Rich Mahogany Shortsword"); 
			Tooltip.SetDefault("Stab effect has 50% chance to poison enemy");
		}
		public override void SetDefaults() {
			DefaultSet();
			item.damage = 8;
			item.knockBack = 1f;
		}
		public override void BleedEffect(NPC npc,Projectile projectile,ref int damage, ref float knockback,ref bool crit) {
			if (Main.rand.NextBool(2)) {
				npc.AddBuff(BuffID.Poisoned,60*3);
			}
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.RichMahogany, 6);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class ShortSpear : ShortswordItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Sparna Short Spear"); 
			Tooltip.SetDefault("Increased stab range\n'This. Is. SPARNA.'");
		}
		public override void SetDefaults() {
			DefaultSet();
			item.rare = 1;
			item.damage = 10;
			item.knockBack = 1f;
			item.shootSpeed = 4f;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Spear);
			recipe.AddIngredient(ItemID.IronBar, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe(); 

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Spear);
			recipe.AddIngredient(ItemID.LeadBar, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class murachan : ShortswordItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Murasan"); 
			Tooltip.SetDefault("Damage scaled by player velocity\n<right> to dash");
		}
		public override void SetDefaults() {
			DefaultSet();
			item.rare = 2;
			item.damage = 19;
			item.knockBack = 2f;
		}
		bool Dash;
		public override bool AltFunctionUse(Player player) => !player.HasBuff(ModContent.BuffType<MuraDash>());
		public override void CustomAI(Projectile projectile) {
			Player player = Main.player[projectile.owner];
			if (Main.rand.NextBool(4)) {
				Dust.NewDust(projectile.Center, projectile.width, projectile.height, 27, 0f, 0f, 0, new Color(255,255,255), 1f);	
			}
			if (Dash) {
				player.velocity = projectile.velocity*5f;
				for (int i = 0; i < 5; i++)
				{
					Dust.NewDust(projectile.Center, projectile.width, projectile.height, 27, 0f, 0f, 0, new Color(255,255,255), 1f);	
				}
			}
		}
		//public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit) {}
		public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat) {
			float vel = (player.velocity.X > 0 ? player.velocity.X : player.velocity.X*-1) + (player.velocity.Y > 0 ? player.velocity.Y : player.velocity.Y*-1);
			add += vel/6f;
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
			if (player.altFunctionUse == 2) {
				player.AddBuff(ModContent.BuffType<MuraDash>(),60*5);
				knockBack *= 1.5f;
				damage += (int)((float)damage*1.5f);
				Dash = true;
			}
			else {Dash = false;}
			return true;
		}
	}
	public class herolastpower : ShortswordItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Short Terra Energy"); 
			Tooltip.SetDefault("Still work in progress");
			//Tooltip.SetDefault("Inflict terrium on enemy\n'cool and green'");
		}
		public override void SetDefaults() {
			DefaultSet();
			item.rare = 3;
			item.damage = 50;
			item.knockBack = 2f;
		}
	}
	public class ToyKnife : ShortswordItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Toy Knife"); 
			Tooltip.SetDefault("'for children'");
		}
		public override void SetDefaults() {
			DefaultSet();
			item.damage = 10;
			item.rare = 1;
			item.knockBack = 1f;
		}
	}
	public class PokingStick : ShortswordItem
	{
		public override string Texture => "Terraria/Projectile_504";
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Poking Stick"); 
			Tooltip.SetDefault("Has a chance to summon a spark\n'Poke Poke'");
		}
		public override void SetDefaults() {
			DefaultSet();
			item.damage = 4;
			item.knockBack = 0.5f;
		}
		public override void ProjFirstTick(Projectile projectile) {
			if (Main.rand.NextBool(4)) {
				Projectile.NewProjectile(projectile.Center,projectile.velocity*2f, ProjectileID.Spark, 2, projectile.knockBack, projectile.owner);	
			}
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Wood, 10);
			recipe.AddIngredient(ItemID.Gel,5);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
}