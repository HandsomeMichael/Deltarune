using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI.Chat;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Deltarune.Helper;
using Terraria.Graphics.Shaders;
using System;

namespace Deltarune.Content.Items
{
	public class SacredRocks : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Sacred rock");
			Tooltip.SetDefault("Increases damage by 10%\nMultiply armor by 30%\namongus\n*insert vine boom sound effect*");
		}
		public override void SetDefaults() {
            item.width = 14;
            item.height = 14;
            item.rare = 3;
			item.accessory = true;
			item.expert = true;
		}
		public override void UpdateAccessory(Player player,bool hide) {
			player.allDamage += 0.1f;
			player.GetDelta().sacredrock = true;
		}
		public static bool hovering;
		public static bool hovering2;
		public static bool therock;
		public override void UpdateInventory(Player player) {
			if (hovering) {
				if (!hovering2) {
					if (Main.rand.NextBool(20) || !therock) {
						Main.PlaySound(Deltarune.GetSound("therock"));
						therock = true;
					}
					hovering2 = true;
				}
			}
			else {hovering2 = false;}
			hovering = false;
		}
		public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset) {
			hovering = true;
			if (line.mod == "Terraria" && line.Name == "ItemName") {
				TextSnippet[] snippets = ChatManager.ParseMessage(line.text, Color.White).ToArray();
				Vector2 messageSize = ChatManager.GetStringSize(Main.fontMouseText, snippets, Vector2.One);
				Rectangle rec = new Rectangle(line.X-40,line.Y-2,(int)messageSize.X + 88,(int)messageSize.Y);
				//Main.spriteBatch.Draw(ModContent.GetTexture(Deltarune.textureExtra+"yourballs"), rec, Color.Black);
				Main.spriteBatch.BeginImmediate(true,true);
				GameShaders.Misc["WaveWrap"].UseOpacity((float)Main.GameUpdateCount/500f).Apply();
				Main.spriteBatch.Draw(ModContent.GetTexture(Deltarune.textureExtra+"yourballs"), rec, Color.Black);
				Main.spriteBatch.BeginImmediate(true,true,true);
				// shdaers horay
				//GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(ItemID.SolarDye), item, null);
				GameShaders.Misc["WaveWrap"].UseOpacity((float)Main.GameUpdateCount/500f).Apply();
				//GameShaders.Misc["ShaderOverlay"].UseColor(Main.DiscoColor).Apply();
				Color color = Helpme.CycleColor(Color.Red,Color.Orange);
				Main.spriteBatch.Draw(ModContent.GetTexture(Deltarune.textureExtra+"yourballs"), rec, color);
				Main.spriteBatch.BeginImmediate(true,true);
				GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(ItemID.SolarDye), item, null);
				//ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.font, line.text, new Vector2(line.X, line.Y), Color.White, line.rotation, line.origin, line.baseScale, line.maxWidth, line.spread);
				Utils.DrawBorderString(Main.spriteBatch, line.text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
				Main.spriteBatch.BeginNormal(true,true);
				return false;
			}
			if (line.mod == "Terraria" && line.text.Contains("amongus")) {
				string amongus = "TP";
				string text1 = "Regenerate ";
				string text2 = " when hitting enemy";
				Vector2 Size = text1.MeasureString();
				Vector2 Size2 = amongus.MeasureString() + Size;
				line.Draw(text1,null,Colors.AlphaDarken(Color.White));

				//make Tension more fancy ( i laik fancy things )
				Main.spriteBatch.BeginImmediate(true,true);
				GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(ItemID.ShiftingSandsDye), item, null);
				line.Draw(amongus,new Vector2(line.X + Size.X ,line.Y));
				Main.spriteBatch.BeginNormal(true,true);

				line.Draw(text2,new Vector2(line.X + Size2.X ,line.Y),Colors.AlphaDarken(Color.White));
				return false;
			}
			return true;
		}
		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,Color itemColor, Vector2 origin, float scale) {
			spriteBatch.BeginGlow(true,true);
			spriteBatch.Draw(ModContent.GetTexture(Texture+"_Glow"), position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
			spriteBatch.BeginNormal(true,true);
			return true;
		}
	}
}
