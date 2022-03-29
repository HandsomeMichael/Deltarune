using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Deltarune.Content.Tiles
{
	public class CorruptedTinBar : ModItem
	{
		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Corrupted Tin Bar");
			ItemID.Sets.SortingPriorityMaterials[item.type] = ItemID.Sets.SortingPriorityMaterials[ItemID.TinBar];
		}
		public override void SetDefaults(){
			item.width = 20;
			item.height = 20;
			item.maxStack = 99;
			item.value = 750;
		}
		public override void AddRecipes(){
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<CorruptedTinOre>(), 4);
			recipe.AddTile(TileID.Furnaces);
			recipe.SetResult(this);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(this);
			recipe.AddIngredient(ItemID.DemoniteBar);
			recipe.AddTile(TileID.Furnaces);
			recipe.SetResult(ItemID.DemoniteBar,4);
			recipe.AddRecipe();
		}
	}
	public class CorruptedTinOre : ModItem
	{
		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Corrupted Tin Ore");
			ItemID.Sets.SortingPriorityMaterials[item.type] = ItemID.Sets.SortingPriorityMaterials[ItemID.TinBar];
		}
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 99;
			item.value = 750/4;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.consumable = true;
			item.createTile = ModContent.TileType<CorruptedTin>();
			item.placeStyle = 0;
		}
	}
	public class CorruptedTin : ModTile
	{
		public override void SetDefaults(){
			TileID.Sets.Ore[Type] = true;
			Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
			Main.tileValue[Type] = Main.tileValue[TileID.Tin]; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
			Main.tileShine2[Type] = Main.tileShine2[TileID.Tin]; // Modifies the draw color slightly.
			Main.tileShine[Type] = Main.tileShine[TileID.Tin]; // How often tiny dust appear off this tile. Larger is less frequently
			Main.tileMergeDirt[Type] = Main.tileMergeDirt[TileID.Tin];
			Main.tileSolid[Type] = Main.tileSolid[TileID.Tin];
			Main.tileBlockLight[Type] = Main.tileBlockLight[TileID.Tin];

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Corrupted Tin Ore");
			AddMapEntry(new Color(152, 171, 198), name);

			dustType = 84;
			drop = ModContent.ItemType<CorruptedTinOre>();
			soundType = SoundID.Tink;
			soundStyle = 1;
			//mineResist = 4f;
			//minPick = 200;
		}
	}
}
