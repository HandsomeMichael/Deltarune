using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Deltarune;
using Deltarune.Helper;

namespace Deltarune.Content.Items
{
	public class ToughGlove : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Increased melee damage by 5%\nReduces damage taken by 10%\n'It's tough to be around here, take this !'");
		}
		public override void SetDefaults() {
			item.width = 20;
			item.height = 20;
			item.accessory = true;
			item.value = 50000;
			item.rare = 2;
			item.defense = 1;
		}
		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.endurance = 0.1f;
			player.meleeDamage += 0.05f;			
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 5);
			recipe.AddIngredient(ItemID.Shackle, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
