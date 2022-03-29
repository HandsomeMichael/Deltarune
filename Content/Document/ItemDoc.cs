
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using Deltarune.Content.UI;
using Deltarune.Helper;
using Deltarune.Content.Spell;
using Microsoft.Xna.Framework.Input;

namespace Deltarune.Content.Document
{
	// with abstract classes code become ez
	public abstract class ItemDoc : ModItem
	{
		public static float pepe = 0.5f;
		public static bool flip = false;
		public static int readType = 0;
		public static float rotate = 0f;
		public static float size = 0f;

		public virtual string ItemName => "";
		public virtual string Description => "";
		public virtual int Rare => 0;
		public virtual float Size => 1f;
		public virtual bool TrueTooltip => false;
		public virtual bool Changable => true;

		public virtual string DocTexture() => Texture + (flip ? "_doc1":"_doc");
		public virtual void OnFlip() {}

		public virtual void Reset() {
			if (pepe != 0.5f ) {pepe = 0.5f;}
			flip = false;
			rotate = 0f;
			size = 0f;
		}

		public override void SetDefaults() {
			item.width = 10;
			item.height = 10;
			item.rare = Rare;
			item.consumable = false;
		}
		public override bool ConsumeItem(Player player) => false;
		//public override bool CanRightClick() => true;
		public override void RightClick(Player player) {
			flip = !flip;
			Main.PlaySound(Deltarune.GetSound("pageflip"+Main.rand.Next(1,5),false));
		}

		public override void UpdateInventory(Player player) {
			if (Main.myPlayer != player.whoAmI || !player.active || player.dead || Main.HoverItem == null || Main.HoverItem.modItem == null || !(Main.HoverItem.modItem is ItemDoc)) {
				Reset();
			}
		}

		public override void SetStaticDefaults() {
			if (ItemName != ""){DisplayName.SetDefault(ItemName);}
			Tooltip.SetDefault(Description+ (TrueTooltip ? "" : (CanRightClick() ? "\n<right> to flip": "")+(Changable ? "\nUse Arrow keys to change rotation & scale":"")));
		}

		public override void PostDrawTooltip(ReadOnlyCollection<DrawableTooltipLine> lines) {
			if (readType != item.type) {
				Reset();
				readType = item.type;
			}
			if (pepe == 0.5f) {
				Main.PlaySound(Deltarune.GetSound("pageflip"+Main.rand.Next(1,5),false));
				flip = false;
			}
			pepe -= 0.05f;
			if (pepe < 0f) {pepe = 0f;}
			if (Changable) {
				if (Main.keyState.IsKeyDown(Keys.Left)) {
					rotate -= 0.01f;
				}
				if (Main.keyState.IsKeyDown(Keys.Right)) {
					rotate += 0.01f;
				}
				if (Main.keyState.IsKeyDown(Keys.Down)) {
					size -= 0.01f;
				}
				if (Main.keyState.IsKeyDown(Keys.Up)) {
					size += 0.01f;
				}
			}
			Vector2 pos = new Vector2( lines[lines.Count-1].X,lines[lines.Count-1].Y + 30);
			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D texture = ModContent.GetTexture(DocTexture());
			Vector2 orig = texture.Size()/2f;
			pos += orig*(Size + size);
			spriteBatch.BeginImmediate(true,true);
			GameShaders.Misc["paperfold"].UseOpacity(pepe).Apply();
			spriteBatch.Draw(texture, pos, null, Color.White, rotate, orig, (Size + size), SpriteEffects.None, 0);
			spriteBatch.BeginNormal(true,true);
		}
	}
	#region Note

	// CAR IS THE KEY [ Code : A3C9B4D1 ] 

	public class NoteA : ItemDoc 
	{
		//car
		public override string ItemName => "Note A3";
		public override string Description => "Seems like a puzzle\nHint : Cars 2 is the best movie";
		public override int Rare => 1;
	}
	public class NoteB : ItemDoc 
	{
		//the
		public override string ItemName => "Note B4";
		public override string Description => "Seems like a puzzle\nHint : Ralsei can eat 10 muffin";
		public override int Rare => 3;
	}
	public class NoteC : ItemDoc 
	{
		//is
		public override string ItemName => "Note C9";
		public override string Description => "Seems like a puzzle\nHint : Sequel is the first";
		public override int Rare => 2;
	}
	public class NoteD : ItemDoc 
	{
		//key
		public override string ItemName => "Note D1";
		public override string Description => "Seems like a puzzle\nHint : Sans can't say the n word";
		public override int Rare => 4;
	}
	#endregion

	public class sussy : ItemDoc 
	{
		public override string ItemName => "Suspicious Drawing";
		public override string Description => "'its look pretty suspicious'";
		public override int Rare => 1;
	}

	public class document1 : ItemDoc 
	{
		public override string ItemName => "??????";
		public override string Description => "give it to him";
		public override int Rare => -12;
	}
	public class him : ItemDoc 
	{
		public override float Size => 0.4f;
		public override string ItemName => "His Note";
		public override string Description => "There is a key inside";
		public override int Rare => -12;
		public override bool CanRightClick() => true;
	}
	public class spr_rbook_ch1 : ItemDoc 
	{
		//public override float Size => 0.4f;

		public static int flippedPage = 1;

		public override string ItemName => "Ralsei book";
		public override bool TrueTooltip => true;
		public override string Description => "<right> to go to next page\n<right> and shift to go to previous page";
		public override int Rare => 1;
		public override bool CanRightClick() => true;
		public override void OnFlip() => flippedPage = 1;
		public override string DocTexture() => Texture+"_"+flippedPage;
		
		public override void RightClick(Player player) {
			if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift)) {
				flippedPage--;
				if (flippedPage < 0) {flippedPage = 0;}
			}
			else {
				flippedPage++;
				if (flippedPage > 7) {flippedPage = 7;}
			}
			Main.PlaySound(Deltarune.GetSound("pageflip"+Main.rand.Next(1,5),false));
		}
		public override void Reset() {
			flippedPage = 1;
			base.Reset();
		}
	}
}
