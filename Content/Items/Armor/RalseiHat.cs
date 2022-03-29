using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Deltarune.Helper;

namespace Deltarune.Content.Items.Armor
{
	// me when copy pasted example mod code :troll:
	[AutoloadEquip(EquipType.Head)]
	public class RalseiHat : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Dark Prince Hat");
			Tooltip.SetDefault("Increases life regen\nSlowly regenerates TP\n'It's fluffy'");
		}

		public override void SetDefaults() {
			item.width = 18;
			item.height = 18;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.defense = 2;
		}

		//public override bool IsArmorSet(Item head, Item body, Item legs) {
			//return body.type == ModContent.ItemType<ExampleBreastplate>() && legs.type == ModContent.ItemType<ExampleLeggings>();
		//}
		int timer;
		public override void UpdateEquip(Player player) {
			player.lifeRegen += 1;
			player.lifeRegenCount += 1;
			player.lifeRegenTime += 1;
			timer++;
			//no way !! tp regen
			if (timer >= 30 && player.GetDelta().TP <= player.GetDelta().TPMax/2) {
				player.GetDelta().TP += 1;
				timer = 0;
			}
		}
		public override void UpdateArmorSet(Player player) {
			//player.setBonus = "trollface.jpg";
			//player.allDamage -= 0.2f;
			/* Here are the individual weapon class bonuses.
			player.meleeDamage -= 0.2f;
			player.thrownDamage -= 0.2f;
			player.rangedDamage -= 0.2f;
			player.magicDamage -= 0.2f;
			player.minionDamage -= 0.2f;
			*/
		}
	}
}