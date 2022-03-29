using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Deltarune.Content.Projectiles;
using Deltarune.Helper;

namespace Deltarune.Content.Items
{
	public class DaintyScarf : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Immune to Chilled and Frostburn\nIncreases snowball damage by 70%\n'It's warm'");
		}
		public override void SetDefaults(){
			item.width = 40;
			item.height = 40;
            item.accessory = true;
            item.rare = 2;
            item.value = Item.sellPrice(gold: 1);
        }
        public override void UpdateAccessory(Player player, bool hideVisual) {
			//player.GetDelta().toughness += 0.2f;
            player.buffImmune[BuffID.Frostburn] = true;
            player.allDamage += 0.1f;
		}
	}
}
