using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Deltarune.Content.NPCs;
using Deltarune.Content.NPCs.Boss;

namespace Deltarune.Content.Items
{
	public class Shatterbell : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Shattered Golder Star");
			Tooltip.SetDefault("Used by the starwalkers to create a bell\n'Shines like a star'");
		}
		public override void SetDefaults() {
			item.width = 20;
			item.height = 20;
			item.maxStack = 4;
			item.value = 100;
			item.rare = 1;
			item.value = Item.buyPrice(gold: 1);
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.FallenStar, 1);
			recipe.AddIngredient(ItemID.Bone, 5);
			recipe.SetResult(this);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(item.type, 2);
			recipe.SetResult(ItemID.FallenStar);
			recipe.AddRecipe();
		}
	}
}
