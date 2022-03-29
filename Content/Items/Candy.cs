using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Deltarune.Content.Items
{
	public class Candy : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Candy"); 
			Tooltip.SetDefault("Did you know that most pig can't talk ?");
		}

		public override void SetDefaults() {
			item.width = 40;
			item.height = 40;
			item.maxStack = 9999;
		}
		public override bool OnPickup(Player player) {
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_power"),player.Center);
			player.HealEffect(item.stack);
			player.statLife += item.stack;
			return false;
		}
		public override bool ItemSpace(Player player) {
			return true;
		}
	}
}