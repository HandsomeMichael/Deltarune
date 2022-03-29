using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Deltarune.Helper;

namespace Deltarune.Content.Items.Armor
{
	// me when copy pasted example mod code :troll:
	[AutoloadEquip(EquipType.Head)]
	public class terrariamanbox : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Terraria Man Box");
			Tooltip.SetDefault("Reduced movement speed by 10% when wet\n'funny terraria man box'");
		}

		public override void SetDefaults() {
			item.width = 18;
			item.height = 18;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.defense = 3;
		}
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ItemID.WoodBreastplate && legs.type == ItemID.WoodGreaves;
		}
		public override void UpdateEquip(Player player) {
			if (player.wet) {
				player.GetDelta().moveSpeed -= 0.1f;
			}
		}
		public override void DrawHands(ref bool drawHandsForLoserLmao, ref bool drawArmsForLoserLmao) {
			drawHandsForLoserLmao = false;
			drawArmsForLoserLmao = false;
		}
		public override void UpdateArmorSet(Player player) {
			player.setBonus = "Increases damage by 25%\nReduced movement speed by 50%";
			player.allDamage += 0.25f;
			player.GetDelta().moveSpeed -= 0.5f;
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