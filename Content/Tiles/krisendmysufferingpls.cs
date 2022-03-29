using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.World.Generation;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Deltarune.Content.Tiles
{
	public class krisendmysufferingpls : ModItem
	{
		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Susie Plush");
			Tooltip.SetDefault("Cute");
		}
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 99;
			item.value = Item.sellPrice(gold: 1);
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.consumable = true;
			item.createTile = ModContent.TileType<krisendmysufferingplsTile>();
			item.placeStyle = 0;
		}
	}
	public class krisendmysufferingplsTile : ModTile
	{
		public override void SetDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(Type);

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Cute susie plush");
			AddMapEntry(Color.Magenta, name);

			dustType = 11;
			disableSmartCursor = true;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			Item.NewItem(i * 16, j * 16, 32, 48, ModContent.ItemType<krisendmysufferingpls>());
		}
	}
}
