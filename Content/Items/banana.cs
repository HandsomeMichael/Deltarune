using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Deltarune.Content.Items
{
	public class banana : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Banana");
			Tooltip.SetDefault("'potasium'");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(10, 8));
			ItemID.Sets.ItemNoGravity[item.type] = true;
		}
		public override void SetDefaults() {
			item.width = 20;
			item.height = 20;
			item.value = 100;
			item.rare = 3;
		}
	}
}