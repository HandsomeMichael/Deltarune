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
	public class starbell : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Bell of Stars");
			Tooltip.SetDefault("Summon one of the starwalker\n'This thing pisses him off'");
			ItemID.Sets.SortingPriorityBossSpawns[item.type] = 2; // This helps sort inventory know this is a boss summoning item.
		}
		public override void SetDefaults() {
			item.width = 20;
			item.height = 20;
			item.maxStack = 20;
			item.value = 100;
			item.rare = ItemRarityID.Blue;
			item.useAnimation = 30;
			item.useTime = 30;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.consumable = true;
			item.shoot = ModContent.ProjectileType<Portal>();
			item.shootSpeed = 0f;
			item.value = Item.buyPrice(gold: 1);//snd_bell
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
			//Projectile.NewProjectile(player.Center + vel,vel, type, damage, knockback, player.whoAmI, 0, 0);
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_bell"),player.Center);
			int a = Projectile.NewProjectile(player.Center - new Vector2(0,200),Vector2.Zero,type,0,0,player.whoAmI,ModContent.NPCType<starwalker>(),0);
			Main.projectile[a].localAI[0] = 1000f;
			return false;
		}
		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,Color itemColor, Vector2 origin, float scale) {
			Texture2D texture = Main.itemTexture[item.type];
			if (NPC.AnyNPCs(ModContent.NPCType<starwalker>())) {
				spriteBatch.Draw(texture, position, frame, drawColor*0.5f, 0f, origin, scale, SpriteEffects.None, 0);	
			}
			else {
				spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0);	
			}
			return false;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Shatterbell>(), 3);
			recipe.AddIngredient(ItemID.GoldBar, 5);
			recipe.SetResult(this);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Shatterbell>(), 3);
			recipe.AddIngredient(ItemID.PlatinumBar, 5);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
		public override bool CanUseItem(Player player) {
			return !NPC.AnyNPCs(ModContent.NPCType<starwalker>());
		}
	}
}
