using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.ModLoader;
using Deltarune;
using Deltarune.Helper;
using Deltarune.Content.Spell;
using System;

namespace Deltarune.Content.UI
{
	
	internal class tensionBar : UIState
	{
		// For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
		// Once this is all set up make sure to go and do the required stuff for most UI's in the Mod class.
		private UIElement area;
		private UIImage barFrame;

		public override void OnInitialize() {
			// Create a UIElement for all the elements to sit on top of, this simplifies the numbers as nested elements can be positioned relative to the top left corner of this element. 
			// UIElement is invisible and has no padding. You can use a UIPanel if you wish for a background.
			area = new UIElement(); 
			//area.Left.Set(-area.Width.Pixels - 500, 1f); // Place the resource bar to the left of the hearts.
			//area.Top.Set(30, 0f); // Placing it just a bit below the top of the screen.
			area.Width.Set(30, 0f); // We will be placing the following 2 UIElements within this 182x60 area.
			area.Height.Set(200, 0f);

			barFrame = new UIImage(ModContent.GetTexture("Terraria/Projectile_0"));
			barFrame.Left.Set(22, 0f);
			barFrame.Top.Set(0, 0f);
			barFrame.Width.Set(25, 0f);
			barFrame.Height.Set(196, 0f);

			//area.Append(text);
			area.Append(barFrame);
			Append(area);
		}

		public override void Draw(SpriteBatch spriteBatch) {
			// This prevents drawing unless we are using an ExampleDamageItem
			//if (!(Main.LocalPlayer.HeldItem.modItem is ExampleDamageItem))
			//	return;
			base.Draw(spriteBatch);
		}
		float lerpHeight;
		float white1;
		float white2;
		int cooldown;
		public static int shake;
		public static Color color;
		protected override void DrawSelf(SpriteBatch spriteBatch) {
			//base.DrawSelf(spriteBatch);

			var p = Main.LocalPlayer.GetDelta();

			Rectangle box = barFrame.GetInnerDimensions().ToRectangle();

			Texture2D asdf = ModContent.GetTexture("Deltarune/Content/UI/tensionBarRed");

			//Main.NewText("cHECK");

			spriteBatch.Draw(position: barFrame.GetDimensions().Position() + asdf.Size() * (1f -barFrame.ImageScale) / 2f, texture:asdf, sourceRectangle: null, color: Color.White, rotation: 0f, origin: Vector2.Zero, scale:barFrame.ImageScale, effects: SpriteEffects.None, layerDepth: 0f);

			asdf = ModContent.GetTexture("Deltarune/Content/UI/tensionBar");

			int width = 25;
			int height = 196;

			Vector2 pos = barFrame.GetDimensions().Position();

			float math = p.TPDisplay*height;
			lerpHeight = MathHelper.Lerp(lerpHeight,math,0.1f);

			pos.Y += height;
			pos.Y -= lerpHeight;

			if (cooldown > 0) {cooldown--;}
			else {
				white1 = MathHelper.Lerp(white1,math,0.1f);
				white2 = barFrame.GetDimensions().Position().Y;
				white2 += height;
				white2 -= white1;
			}

			Rectangle hitbox = new Rectangle((int)pos.X,(int)white2,width-1,(int)white1-1);

			Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.White);

			hitbox = new Rectangle((int)pos.X,(int)pos.Y,width-1,(int)lerpHeight-1);

			Main.spriteBatch.Draw(Main.magicPixel, hitbox, p.TP == p.TPMax ? Color.Yellow : new Color(255,127,39));

			spriteBatch.Draw(position: barFrame.GetDimensions().Position() + asdf.Size() * (1f -barFrame.ImageScale) / 2f, texture:asdf, sourceRectangle: null, color: Color.White, rotation: 0f, origin: Vector2.Zero, scale:barFrame.ImageScale, effects: SpriteEffects.None, layerDepth: 0f);

			//spriteBatch.Draw(asdf, new Vector2(box.X,box.Y), null, Color.White, 0f, asdf.Size()/2, 1f, SpriteEffects.None, 0f);
			asdf = ModContent.GetTexture(Deltarune.textureExtra+"spr_tplogo_0");
			pos = barFrame.GetDimensions().Position() + new Vector2(-13,61);
			spriteBatch.Draw(asdf, pos, null, Color.White, 0f, asdf.Size()/2, 1f, SpriteEffects.None, 0f);

			int num = (int)((p.TPDisplay)*100f);

			Vector2 pepe = new Vector2(Main.rand.Next(-shake, shake + 1), Main.rand.Next(-shake, shake + 1));

			if (p.TP == p.TPMax) {
				pos = barFrame.GetDimensions().Position() + new Vector2(-19,97);
				pos += new Vector2(-11,-11) + pepe;
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Deltarune.tobyFont, "MAX", pos, Color.Yellow, 0, Vector2.Zero, Vector2.One);	
			}
			else {
				color = Color.Lerp(color,Color.White,0.1f);
				Vector2 pos2 = new Vector2(0,0);
				string text = $"{num}";
				foreach (var i in text)
				{
					if (int.TryParse(i.ToString(), out int num2)) {
						asdf = ModContent.GetTexture("Deltarune/Content/UI/Number");
						if (num == 0) {pos.X += 10;}
						pos = barFrame.GetDimensions().Position() + new Vector2(-19,97) + pos2 + pepe;
						spriteBatch.Draw(asdf, pos, asdf.GetFrame(num2,10), color, 0f, asdf.GetFrame(num2,10).Size()/2, 1f, SpriteEffects.None, 0f);
						pos2.X += 21;
					}
					else {
						Main.NewText("Error Failed to parse string, if you are seeing this message, then you are doomed :deathemoji:",Color.Red);
						ModContent.GetInstance<Deltarune>().Logger.InfoFormat($"Deltarune : Error Failed to parse string");
					}
				}
			}
			pos = barFrame.GetDimensions().Position() + new Vector2(-12,128);
			asdf = ModContent.GetTexture(Deltarune.textureExtra+"Font/spr_numbersfontbig_12");
			spriteBatch.Draw(asdf, pos, null, Color.White, 0f, asdf.Size()/2, 1f, SpriteEffects.None, 0f);

			pos = barFrame.GetDimensions().Position() + new Vector2(-51,39);
			for (int i = 0; i < p.spell.Length; i++){
				if (p.spell[i] > 0) {
					asdf = Main.itemTexture[p.spell[i]];
					//item.SetDefaults(p.spell[i]);
					//BaseSpell spell = (BaseSpell)item.modItem;
					spriteBatch.Draw(asdf, pos, null, Color.White, 0f, asdf.Size()/2, 0.8f, SpriteEffects.None, 0f);
					if (p.spellTimer[i] > 0 && p.spellItem[i] != null) {
						BaseSpell spell = p.spellItem[i];
						float alpha = (float)p.spellTimer[i]/(float)spell.cooldown;
						float frameHeight = (float)asdf.Height*alpha;
						spriteBatch.Draw(asdf, pos, null, Color.Black*alpha, 0f, asdf.Size()/2, 0.8f, SpriteEffects.None, 0f);
						if (spell.cooldown > 60 || p.spellTimer[i] > 60) {
							float num2 = (float)p.spellTimer[i] / 60f;
							alpha = (float)Math.Round((double)num2);
							Vector2 messageSize = ChatManager.GetStringSize(Main.fontMouseText, $"{alpha}s", Vector2.One);
							ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, $"{alpha}s",
							pos - new Vector2(messageSize.X/2f,0f), Color.White, 0, Vector2.Zero, Vector2.One);
						}
					}
					string text = "None";
					var flag = Deltarune.KeyMagic1.GetAssignedKeys();
					var flag2 = Deltarune.KeyMagic2.GetAssignedKeys();
					var flag3 = Deltarune.KeyMagic3.GetAssignedKeys();
					if (i == 0) {if (flag.Count != 0) {text = flag[0];}}
					if (i == 1) {if (flag2.Count != 0) {text = flag2[0];}}
					if (i == 2) {if (flag3.Count != 0) {text = flag3[0];}}
					Vector2 m = ChatManager.GetStringSize(Main.fontMouseText, text, Vector2.One);
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, text,
					pos + new Vector2(-34,1) - new Vector2(m.X,0f), (text == "None" ? Color.Red : Color.White), 0, Vector2.Zero, Vector2.One);
				}
				pos.Y -= -55;
			}


			if (shake > 0) {
				color = Color.Red;
				cooldown = 30;
				shake--;
			}

			//ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Deltarune.tobyFont, ((float)p.TP/(float)p.TPMax)*100f+"%", barFrame.GetDimensions().Position() + new Vector2(-30,80), Color.White, 0, Vector2.Zero, Vector2.One);
		}
		float areaTop;
		float areaLeft;
		public override void Update(GameTime gameTime) {
			//if (!(Main.LocalPlayer.HeldItem.modItem is ExampleDamageItem))
			//	return;


			var p = Main.LocalPlayer.GetDelta();

			if (barFrame.IsMouseHovering)
    			Main.hoverItemName = $"{p.TP} / {p.TPMax}";

			if (MyConfig.get.tensionStyle) {
				areaLeft = MathHelper.Lerp(areaLeft,29,0.1f);
				areaTop = MathHelper.Lerp(areaTop,314,0.1f);
				area.Left.Set(-area.Width.Pixels - areaLeft, 1f);
				area.Top.Set(areaTop, 0f); // Placing it just a bit below the top of the screen.
			}
			else {
				if (Main.mapStyle == 1 && Main.mapEnabled) {
					areaLeft = MathHelper.Lerp(areaLeft,330,0.1f);
					areaTop = MathHelper.Lerp(areaTop,71,0.1f);
					area.Left.Set(-area.Width.Pixels - areaLeft, 1f);
					area.Top.Set(areaTop, 0f); // Placing it just a bit below the top of the screen.
				}
				else {
					areaLeft = MathHelper.Lerp(areaLeft,70,0.1f);
					areaTop = MathHelper.Lerp(areaTop,93,0.1f);
					area.Left.Set(-area.Width.Pixels - areaLeft, 1f);
					area.Top.Set(areaTop, 0f); // Placing it just a bit below the top of the screen.
				}
			}
			// Setting the text per tick to update and show our resource values.
			//text.SetText($"TP {((float)p.TP/(float)p.TPMax)*100f}%");
			base.Update(gameTime);
		}
	}
}
