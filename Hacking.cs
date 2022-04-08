using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.UI.Chat;
using Terraria.UI;
using Terraria.ModLoader;
using ReLogic.Graphics;
using Deltarune.Helper;
using Deltarune.Content;
using System.Threading;
using Terraria.ModLoader.Audio;
using Terraria.ModLoader.Config;
using Microsoft.Xna.Framework.Input;
//using On.Terraria;

namespace Deltarune
{
	// a lot more organized now
	public class UpdateHook : ILoadable
	{
		public void Load() {
			IL.Terraria.Main.UpdateAudio += Music_Patch;
			IL.Terraria.Main.UpdateAudio += Music_Autodisable_Patch;
			Main.OnTick += PostUpdate;
			On.Terraria.Player.AddBuff += AddBuffPatch;
			On.Terraria.Main.OnCharacterNamed += OnCharacterNamedPatch;
			On.Terraria.NPC.CanBeChasedBy += MinionChasingPrevent;
		}
		public void Unload() {
			Main.OnTick -= PostUpdate;
			IL.Terraria.Main.UpdateAudio -= Music_Patch;
			IL.Terraria.Main.UpdateAudio -= Music_Autodisable_Patch;
			On.Terraria.Player.AddBuff -= AddBuffPatch;
			On.Terraria.Main.OnCharacterNamed -= OnCharacterNamedPatch;
			On.Terraria.NPC.CanBeChasedBy -= MinionChasingPrevent;
		}
		public static void PostUpdate() {
			// these are all clientside and just for pure graphic
			if (!Main.dedServ) {
				CustomEntity.UpdateAll();
				if (Deltarune.intro < 0) {UpdateHook.UpdateHim();}
				if (MyConfig.get.showDebug) {UpdateHook.UpdateDebug();}
			}
		}
		// disable all minion targetting when you exit ur body.
		static bool MinionChasingPrevent(On.Terraria.NPC.orig_CanBeChasedBy orig,NPC self,object attacker , bool ignoreDontTakeDamage ) {
			if (Main.ProjectileUpdateLoopIndex > -1 && Main.ProjectileUpdateLoopIndex < Main.projectile.Length) {
				Projectile projectile = Main.projectile[Main.ProjectileUpdateLoopIndex];
				if (projectile.active && projectile.minion && projectile.owner > -1 && projectile.owner < Main.projectile.Length) {
					Player player = Main.player[projectile.owner];
					if (player.active && !player.dead) {
						if (player.GetDelta() != null && player.GetDelta().soulTimer > 0) {
							return false;
						}
					}
				}
			}
			return orig(self,attacker,ignoreDontTakeDamage);
		}
		static void OnCharacterNamedPatch(On.Terraria.Main.orig_OnCharacterNamed orig, Main self,string text) {
			Deltarune.selectedMenu = 0;
			if (MyConfig.get.mainMenu) {Main.PlaySound(Deltarune.get.GetLegacySoundSlot(SoundType.Custom, "Sounds/mus_f_newlaugh"));}
			//mus_f_newlaugh
			orig(self,text);
		}
		static void UpdateHim() {
			//reflection lets gooo
			if (!MyConfig.get.mainMenu) {return;}
			FieldInfo field = Main.instance.GetType().GetField("selectedMenu", BindingFlags.NonPublic | BindingFlags.Instance);
			int selectedMenu = (int)field.GetValue(Main.instance);
			if (selectedMenu != -1) {Deltarune.selectedMenu = selectedMenu;}
			bool him = false;
			if (Main.menuMode == 2 || (Main.menuMode > 16 && Main.menuMode < 25) || Main.menuMode == 222 || (Main.menuMode == 888 && Deltarune.selectedMenu == 6)) {him = true;}
			if (him) {
				Deltarune.TitleMusic = Deltarune.get.GetSoundSlot(SoundType.Music, "Sounds/Music/AUDIO_ANOTHERHIM");
				Deltarune.darkenBG += 0.01f;
				if (Deltarune.darkenBG > 1f) {
					Deltarune.darkenBG = 1f;
				}
			}
			else{
				Deltarune.TitleMusic = Deltarune.get.GetSoundSlot(SoundType.Music, "Sounds/Music/lancer");
				Deltarune.darkenBG -= 0.05f;
				if (Deltarune.darkenBG < 0f) {
					Deltarune.darkenBG = 0f;
				}
			}
		}
		static void AddBuffPatch(On.Terraria.Player.orig_AddBuff orig,Player self,int type, int time1, bool quiet = true) {
			if (type == BuffID.Confused && !self.buffImmune[BuffID.Confused] && !self.HasBuff(BuffID.Confused)) {Main.PlaySound(Deltarune.GetSound("hypnosis"),self.Center);}
			orig(self,type,time1,quiet);
		}
		static void UpdateDebug() {
			if (Main.keyState.IsKeyDown(Keys.Up) || Main.keyState.IsKeyDown(Keys.Right)) {Deltarune.debug[Deltarune.debugCur]++;}
			if (Main.keyState.IsKeyDown(Keys.Down) || Main.keyState.IsKeyDown(Keys.Left)) {Deltarune.debug[Deltarune.debugCur]--;}
			if (Main.keyState.IsKeyDown(Keys.D1)) {Deltarune.debugCur = 0;}
			if (Main.keyState.IsKeyDown(Keys.D2)) {Deltarune.debugCur = 1;}
			if (Main.keyState.IsKeyDown(Keys.D3)) {Deltarune.debugCur = 2;}
			if (Main.keyState.IsKeyDown(Keys.D4)) {Deltarune.debugCur = 3;}
			if (Main.keyState.IsKeyDown(Keys.L)) {Deltarune.debug[Deltarune.debugCur] = 0;}
			if (Deltarune.debugCur < 0) {Deltarune.debugCur = 3;}
			if (Deltarune.debugCur > 3) {Deltarune.debugCur = 0;}
		}
		#region music
		static void Music_Patch(ILContext il) {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(instr => instr.MatchLdcI4(6) && instr.Next.MatchStfld(typeof(Main).GetField("newMusic")))) {
                throw new Exception("Could not find instruction to patch (Music_Patch)");
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(MusicDelegate);
        }
        static int MusicDelegate(int defaultMusic) {
			return (Deltarune.TitleMusic == 0 ? defaultMusic : Deltarune.TitleMusic);
        }
		static int MusicDisableDelegate(int oldValue){
            return 0;
        }
        static void Music_Autodisable_Patch(ILContext il){
			// ported from red cloud mod
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(instr => instr.MatchLdsfld<Main>("musicError") && instr.Next.MatchLdcI4(100))){
                throw new Exception("Could not find instruction to patch (Music_Patch), haha you had a skill issue");
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(MusicDisableDelegate);
        }
		#endregion
	}
	public class DrawHook : ILoadable
	{
		public void Load() {
			On.Terraria.Main.DoDraw += drawAdd;
			On.Terraria.Main.DrawBG += newBackground;
			On.Terraria.Main.GUIChatDrawInner += GuiPatch;
			On.Terraria.Main.DrawInterface_35_YouDied += YouDiedPatch;
			On.Terraria.Main.DrawPlayers += DrawPlayersPatch;
			On.Terraria.GameContent.Events.MoonlordDeathDrama.DrawPieces += PreEntityDraw;
			//On.Terraria.Main.DrawTiles += DrawTilesPatch;
		}
		public void Unload() {
			On.Terraria.Main.DoDraw -= drawAdd;
			On.Terraria.Main.DrawBG -= newBackground;
			On.Terraria.Main.GUIChatDrawInner -= GuiPatch;
			On.Terraria.Main.DrawInterface_35_YouDied -= YouDiedPatch;
			On.Terraria.Main.DrawPlayers -= DrawPlayersPatch;
			On.Terraria.GameContent.Events.MoonlordDeathDrama.DrawPieces -= PreEntityDraw;
		}
		static void PreEntityDraw(On.Terraria.GameContent.Events.MoonlordDeathDrama.orig_DrawPieces orig ,SpriteBatch spriteBatch) {	
			SoulHandler.DrawBorder(spriteBatch);
			orig(spriteBatch);
		}
		static void DrawPlayersPatch(On.Terraria.Main.orig_DrawPlayers orig, Main self) {
			orig(self);
			Main.spriteBatch.BeginNormal();
			SoulHandler.Draw(Main.spriteBatch);
			Main.spriteBatch.End();
		}
		// Reboiled later
		//static void DrawTilesPatch(On.Terraria.Main.orig_DrawTiles orig,Main self ,bool solidOnly, int waterStyleOverride) {orig(self,solidOnly,waterStyleOverride);}
		static void drawAdd(On.Terraria.Main.orig_DoDraw orig,global::Terraria.Main self, GameTime gameTime) {
			orig(self,gameTime);
			IntroDraw();
			if (CustomEntity.bitList != null) {CustomEntity.DrawAll();}
			if (MyConfig.get.showDebug) {DrawDebug();}
		}
		static void GuiPatch(On.Terraria.Main.orig_GUIChatDrawInner orig,Main self) {
			if (MyConfig.get.DialogBackground) {
				Main.fontMouseText = Deltarune.tobyFont;
				DrawDialogHead(Main.spriteBatch);
			}
			orig(self);
			if (MyConfig.get.DialogBackground) {Main.fontMouseText = TextureCache.fontMouseText;}
		}
		static void newBackground(On.Terraria.Main.orig_DrawBG orig,global::Terraria.Main self) {
			orig(self);
			Vector2 centerScreen = new Vector2(Main.screenWidth/2,Main.screenHeight/2);
			if (Deltarune.darkenBG > 0f) {
				//me when funky shader
				Main.spriteBatch.End();
				//float rot = centerScreen.AngleTo(new Vector2(Main.mouseX,Main.mouseY));
				//float deg = MathHelper.ToDegrees(rot);
				//float blur = Vector2.Distance(centerScreen,new Vector2(Main.mouseX,Main.mouseY));
				//Deltarune.darkenTime = MathHelper.Lerp(Deltarune.darkenTime,deg,0.1f);
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.instance.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);
				//GameShaders.Misc["Blurify"].UseColor(Deltarune.darkenTime, blur/1000f, 0).Apply();
				GameShaders.Misc["WaveWrap"].UseOpacity(Main.GameUpdateCount/500f).Apply();
				//GameShaders.Misc["funky"].UseImage(Deltarune.textureExtra+"Perlin").UseOpacity(0.5f).Apply();
				Rectangle hitbox = new Rectangle(0,0,(int)Main.screenWidth,(int)Main.screenHeight);
				Texture2D tex = ModContent.GetTexture(Deltarune.textureExtra+"depth");
				Main.spriteBatch.Draw(tex, hitbox, Color.White*Deltarune.darkenBG);
				//end begin bc shader
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.instance.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);
			}
			if (Main.menuMode == 2){DrawCharacterUI(Main.spriteBatch);}
			if (Main.LocalPlayer.active && !Main.gameMenu) {
				if (Deltarune.battleAlpha > 0f) {DrawBattleBackground();}
				//if (Helpme.AnyBoss()) {}
			}
		}
		static void DrawBattleBackground() {
			Rectangle hitbox = new Rectangle(0,0,(int)Main.screenWidth,(int)Main.screenHeight);
			Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Black*Deltarune.battleAlpha);
			Texture2D texture = ModContent.GetTexture(Deltarune.textureExtra+"battleAnim_glow");//_glow
			int width = 50;
			int height = 50;
			int num = (int)(Main.screenWidth / width) + 3;

			Deltarune.battleFrameCount++;
			if (Deltarune.battleFrameCount > 3) {
				Deltarune.battleFrameCount = 0;
				Deltarune.battleFrame++;
			}
			if (Deltarune.battleFrame >= 14) {Deltarune.battleFrame = 0;}
			//Main.spriteBatch.BeginGlow(true);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, Main.instance.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);
			for (int i = 0; i < num; i++)
			{
				int num3 = (int)(Main.screenHeight / height) + 3;
				for (int j = 0; j < num3; j++)
				{
					Vector2 pos = new Vector2((i*width),(j*height));
					Main.spriteBatch.Draw(texture, pos, texture.GetFrame(Deltarune.battleFrame,14), Color.White*Deltarune.battleAlpha, 0f, texture.GetFrame(Deltarune.battleFrame,14).Size() /2f, 1f, SpriteEffects.None, 0);
					int num2 = (Deltarune.battleFrame-14)*-1;
					//original alpha = 0.2f
					float alpha = Deltarune.battleAlpha;
					if (alpha > 0.2f) {alpha = 0.2f;}
					Main.spriteBatch.Draw(texture, pos, texture.GetFrame(num2,14), Color.White*alpha, 0f, texture.GetFrame(num2,14).Size() /2f, 1f, SpriteEffects.None, 0);
					
				}
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.instance.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);
		}
		static void DrawCharacterUI(SpriteBatch spriteBatch) {
			// haha "ui" moment :)

			Vector2 center = new Vector2(Main.screenWidth/2f,Main.screenHeight/2f);
			center += new Vector2(372,0);
			Vector2 pos = center;
			pos += new Vector2(-143,0);
			Texture2D texture = Main.cdTexture;
			// i dont like switch case
			if (Deltarune.playerClass == 1) {texture = Main.itemTexture[ItemID.WarriorEmblem];}
			if (Deltarune.playerClass == 2) {texture = Main.itemTexture[ItemID.RangerEmblem];}
			if (Deltarune.playerClass == 3) {texture = Main.itemTexture[ItemID.SorcererEmblem];}
			if (Deltarune.playerClass == 4) {texture = Main.itemTexture[ItemID.SummonerEmblem];}

			Rectangle rec = texture.getRect(pos);
			Rectangle mouseRec = new Rectangle(Main.mouseX,Main.mouseY,2,2);
			bool hover = mouseRec.Intersects(rec);
			
			Texture2D texture2 = Main.itemTexture[ItemID.CopperBar];
			// i dont like switch case
			if (Deltarune.playerClassType == 1) {texture2 = Main.itemTexture[ItemID.TinBar];}
			if (Deltarune.playerClassType == 2) {texture2 = Main.itemTexture[ItemID.IronBar];}
			if (Deltarune.playerClassType == 3) {texture2 = Main.itemTexture[ItemID.LeadBar];}
			if (Deltarune.playerClassType == 4) {texture2 = Main.itemTexture[ItemID.SilverBar];}
			if (Deltarune.playerClassType == 5) {texture2 = Main.itemTexture[ItemID.TungstenBar];}

			Rectangle rec2 = texture2.getRect(pos - new Vector2(50,0));
			bool hover2 = mouseRec.Intersects(rec2);

			Texture2D texture3 = Main.cdTexture;
			// i dont like switch case
			if (Deltarune.playerClassMisc == 1) {texture3 = Main.itemTexture[ItemID.WoodenCrate];}
			if (Deltarune.playerClassMisc == 2) {texture3 = Main.itemTexture[ItemID.IronCrate];}

			Rectangle rec3 = texture3.getRect(pos + new Vector2(50,0));
			bool hover3 = mouseRec.Intersects(rec3);
			
			if (Main.mouseLeft) {
				if (!Deltarune.click) {
					if (hover) {
						Main.PlaySound(Deltarune.GetSound("select"));
						Deltarune.playerClass++;
					}
					if (hover2) {
						Main.PlaySound(Deltarune.GetSound("select"));
						Deltarune.playerClassType++;
					}
					if (hover3) {
						Main.PlaySound(Deltarune.GetSound("select"));
						Deltarune.playerClassMisc++;
					}
					Deltarune.click = true;
				}
			}
			else {
				Deltarune.click = false;
			}
			if (Deltarune.playerClass > 4) {Deltarune.playerClass = 0;}
			if (Deltarune.playerClass < 0) {Deltarune.playerClass = 0;}
			if (Deltarune.playerClassType > 5) {Deltarune.playerClassType = 0;}
			if (Deltarune.playerClassMisc > 2) {Deltarune.playerClassMisc = 0;}
			if (Deltarune.playerClassMisc < 0) {Deltarune.playerClassMisc = 0;}

			Utils.DrawInvBG(spriteBatch, new Rectangle((int)center.X - 202,(int)center.Y - 65,131,33),Color.Violet*0.9f);
			Utils.DrawInvBG(spriteBatch, new Rectangle((int)center.X - 274,(int)center.Y - 33,263,140),Color.Violet*0.9f);

			spriteBatch.Draw(texture, pos, null, hover ? Color.LightYellow : Color.White , 0f, texture.Size()/2, hover ? 1.2f : 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(texture2, pos - new Vector2(50,0), null, hover2 ? Color.LightYellow : Color.White, 0f, texture2.Size()/2, hover2 ? 1.2f : 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(texture3, pos + new Vector2(50,0), null, hover3 ? Color.LightYellow : Color.White, 0f, texture3.Size()/2, hover3 ? 1.2f : 1f, SpriteEffects.None, 0f);

			pos = center + new Vector2(-139,-59);
			string textValue = "Starting Items";
			pos.X -= Helpme.MeasureString(textValue).X/2f;
			Utils.DrawBorderString(Main.spriteBatch, textValue, pos, Color.White, 1);
			textValue = "Choose your starting items !";
			string[] texts = new string[] {"None","Melee","Ranger","Mage","Summoner"};
			if (hover) {textValue = texts[Deltarune.playerClass] + " Class";}
			texts = new string[] {"Copper","Tin","Iron","Lead","Silver","Tungsten"};
			if (hover2) {textValue = texts[Deltarune.playerClassType] + " Tier";}
			texts = new string[] {"No","Basic","Extra"};
			if (hover3) {textValue = texts[Deltarune.playerClassMisc] + " Extra";}

			pos = center + new Vector2(-144,32);
			pos.X -= Helpme.MeasureString(textValue).X/2f;
			Utils.DrawBorderString(Main.spriteBatch, textValue, pos, Color.White, 1);
		}
		static void DrawDialogHead(SpriteBatch spriteBatch) {
			if (Main.LocalPlayer.talkNPC > -1 && !Main.playerInventory) {
				NPC npc = Main.npc[Main.LocalPlayer.talkNPC];
				Texture2D HeadTexture = null;
				int getHead = NPC.TypeToHeadIndex(npc.type);
				float headScale = 2f;
				bool rotate = true;
				if (getHead > -1) {HeadTexture = Main.npcHeadTexture[getHead];}
				GlobeNPC.CustomHeadTexture(npc,ref HeadTexture, ref headScale, ref rotate);
				if (MyConfig.get.DialogTypewriter) {
					Vector2 pos2 = new Vector2(0,0);
					if (Deltarune.chatNPC != null && !Deltarune.chatNPC.Done()) {
						string text = "Press Space To Skip";
						pos2 = new Vector2(Main.screenWidth / 2, 86);
						pos2.X -= Helpme.MeasureString(text,Deltarune.tobyFont).X/2f;
						ChatManager.DrawColorCodedStringWithShadow(spriteBatch,Deltarune.tobyFont,text,pos2,Color.White, 0, Vector2.Zero, Vector2.One);
					}
					if (HeadTexture == null) {
						return;
					}
					pos2 = new Vector2(Main.screenWidth / 2 - Main.chatBackTexture.Width / 2, 86);
					pos2.X -= Helpme.MeasureString(npc.GivenOrTypeName,Deltarune.tobyFont).X/2f;
					pos2 += new Vector2(-50,118);
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch,Deltarune.tobyFont,npc.GivenOrTypeName,pos2,Color.White, 0, Vector2.Zero, Vector2.One);
				}
				if (HeadTexture == null) {
					return;
				}
				Vector2 pos = new Vector2(Main.screenWidth / 2 - Main.chatBackTexture.Width / 2, 100f);
				Texture2D texture = ModContent.GetTexture(Deltarune.textureExtra+"Chat_Small");
				pos.X += -97;
				pos.Y += 9;
				Rectangle hitbox = new Rectangle((int)pos.X,(int)pos.Y,(int)97,(int)97);
			
				Main.spriteBatch.Draw(texture, hitbox, Color.White*0.8f);
				pos.X += 97/2;
				pos.Y += 97/2;
				texture = HeadTexture;
				SpriteEffects spriteEffects = SpriteEffects.None;
				float rotation = 0f;
				if (rotate) {
					if (npc.spriteDirection == 1) {
						//spriteEffects = SpriteEffects.FlipHorizontally;
						spriteEffects = SpriteEffects.FlipVertically;
					}
					rotation = npc.AngleTo(Main.LocalPlayer.Center);
					rotation -= MathHelper.ToRadians(180);
					if (npc.Distance(Main.LocalPlayer.Center) < 10f) {
						rotation = 0f;
						if (npc.spriteDirection == 1) {
							spriteEffects = SpriteEffects.FlipHorizontally;
							//spriteEffects = SpriteEffects.FlipVertically;
						}
					}
				}
				spriteBatch.Draw(texture, pos, null, Color.White, rotation, texture.Size()/2f, headScale, spriteEffects, 0);	
				//spriteBatch.Draw(texture, pos, null, Color.White, 0f, texture.Size()/2f, 1f, SpriteEffects.None, 0);
				//Main.spriteBatch.Draw(Main.chatBackTexture, new Vector2(Main.screenWidth / 2 - Main.chatBackTexture.Width / 2, 100f), new Rectangle(0, 0, Main.chatBackTexture.Width, (lineAmount + 1) * 30), color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
			}
		}
		static void YouDiedPatch(On.Terraria.Main.orig_DrawInterface_35_YouDied orig) {
			if (!MyConfig.get.newGameOver) {
				orig();
				return;
			}
			DrawNewGameOver();
		}
		static void DrawNewGameOver() {
			if (Main.player[Main.myPlayer].dead) {
				Deltarune.deathAlpha += 0.01f;
				if (Deltarune.deathAlpha > 1f) {Deltarune.deathAlpha = 1f;}
				float alpha = Deltarune.deathAlpha;
				if (alpha > 0f) {
					Rectangle hitbox = new Rectangle(0,0,(int)Main.screenWidth,(int)Main.screenHeight);
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Black*(alpha/2f));
				}
				//setup all the var needed
				Texture2D texture = ModContent.GetTexture(Deltarune.textureExtra+"gameoverbuddy");
				Vector2 size = texture.Size()/2f;
				string textValue = Deltarune.deathTips;
				var color = Main.player[Main.myPlayer].GetDeathAlpha(Color.Transparent);

				int num = 30;
				if (Main.player[Main.myPlayer].lostCoins > 0) {num = 60;}

				//pos for the tip
				Vector2 pos = new Vector2((float)(Main.screenWidth / 2) 
				- Helpme.MeasureString(textValue).X / 2f, Main.screenHeight / 2 + num);
				pos.Y += size.Y;

				//draw
				Main.spriteBatch.Draw(texture, new Vector2(Main.screenWidth/2,Main.screenHeight/2), null, 
				Color.White*alpha, 0f, size, 1f, SpriteEffects.None, 0f);

				//ChatManager.DrawColorCodedString(Main.spriteBatch, Deltarune.tobyFont, textValue, pos, Color.White*alpha,0f, Vector2.Zero, Vector2.One);

				Utils.DrawBorderString(Main.spriteBatch, textValue, pos, Color.White*alpha, 1);
				//funk
				//DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText,
				 //textValue, pos, Color.White*alpha, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

				//draw the coin drop. if had one
				if (Main.player[Main.myPlayer].lostCoins > 0){
					
					textValue = Language.GetTextValue("Game.DroppedCoins", Helpme.coinsText2(Main.player[Main.myPlayer].lostCoins));
					//text measuring
					Vector2 messageSize = Helpme.MeasureString(textValue);

					pos = new Vector2((float)(Main.screenWidth / 2) - messageSize.X / 2f, Main.screenHeight / 2 + 30);
					pos.Y += size.Y;

					//ChatManager.DrawColorCodedString(Main.spriteBatch, Deltarune.tobyFont, textValue, pos, Color.White*alpha,0f, Vector2.Zero, Vector2.One);

					Utils.DrawBorderString(Main.spriteBatch, textValue, pos, Color.White*alpha, 1);
					//funk
					//DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText,
					//textValue, pos, Color.Pink*alpha, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				}
				textValue = $"{Main.LocalPlayer.respawnTimer/60}";
				pos = new Vector2((float)(Main.screenWidth / 2) 
				- Helpme.MeasureString(textValue).X / 2f, Main.screenHeight / 2 + ((float)num*1.5f));
				pos.Y += size.Y;

				Utils.DrawBorderStringBig(Main.spriteBatch, textValue, pos, Color.White*alpha,1);
			}
			else {
				Deltarune.deathAlpha = 0f;
			}
		}
		static void DrawDebug() {
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, 
			Main.fontMouseText, $"curDeb = {Deltarune.debugCur}\ndeb0 = {Deltarune.debug[0]}\ndeb1 = {Deltarune.debug[1]}\ndeb2 = {Deltarune.debug[2]}\ndeb3 = {Deltarune.debug[3]}",
				new Vector2(Main.screenWidth/2,Main.screenHeight/2) + new Vector2(0,80),
				Color.White, 0, Vector2.Zero, Vector2.One);
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, 
			Main.fontMouseText, $"playerClassType = {Deltarune.playerClassType}\nplayerClassMisc = {Deltarune.playerClassMisc}\nplayerClass = {Deltarune.playerClass}",
				new Vector2(Main.screenWidth/2,Main.screenHeight/2) - new Vector2(0,50),Color.White, 0, Vector2.Zero, Vector2.One);
			Main.spriteBatch.End();
		}
		static void IntroDraw() {
			Vector2 centerScreen = new Vector2(Main.screenWidth/2,Main.screenHeight/2);
			if (Deltarune.intro < 0 && Deltarune.intro >= -30) {
				if (Deltarune.intro == -1) {
					if (MyConfig.get.mainMenu) {
						MyConfig.get.intro = false;
						Helpme.SaveConfig<MyConfig>();
						Explode.BoomScreen();
						Deltarune.TitleMusic = Deltarune.get.GetSoundSlot(SoundType.Music, "Sounds/Music/lancer");
						Main.logoTexture = ModContent.GetTexture(Deltarune.textureExtra+"Title"+Main.rand.Next(1,6));
						Main.logo2Texture = ModContent.GetTexture(Deltarune.textureExtra+"Title"+Main.rand.Next(1,6));
					}
				}
				Deltarune.intro--;
				int intro = Deltarune.intro*-1;
				if (intro == 30 && MyConfig.get.mainMenu) {
					Main.PlaySound(Deltarune.GetSound("lancerlaugh"));
				}
				Deltarune.intro = intro*-1;
			}
			if (Deltarune.intro > 0) {
				//normal
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
				Rectangle hitbox = new Rectangle(0,0,(int)Main.screenWidth,(int)Main.screenHeight);
				//hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
				Texture2D texture = ModContent.GetTexture(Deltarune.textureExtra+"Logo");
				Texture2D texture2 = ModContent.GetTexture(Deltarune.textureExtra+"Logo_glow");
				Deltarune.intro++;
				if (Deltarune.intro > 300) {
					float alpha = ((float)Deltarune.intro - 300f)/100f;
					alpha = 1f - alpha;
					if (alpha > 1f) {alpha = 1f;}
					if (alpha < 0f) {alpha = 0f;}
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Black*alpha);
					//glow
					Main.spriteBatch.BeginGlow(true,true);
					Main.spriteBatch.Draw(texture2, centerScreen, null, Color.White*alpha, 0f, texture2.Size()/2, 1f, SpriteEffects.None, 0f);
					//normal
					Main.spriteBatch.BeginNormal(true,true);
					Main.spriteBatch.Draw(texture, centerScreen, null, Color.White*alpha, 0f, texture.Size()/2, 1f, SpriteEffects.None, 0f);
					if (Deltarune.intro > 600) {
						LancerIntro.New();
						Deltarune.intro = 0;
					}
				}
				else {
					float alpha = (float)Deltarune.intro/100f;
					if (alpha > 1f) {alpha = 1f;}
					Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Black*alpha);
					if (Deltarune.intro > 200) {
						alpha = ((float)Deltarune.intro - 200f)/100f;
						if (alpha > 1f) {alpha = 1f;}
						//glow
						Main.spriteBatch.BeginGlow(true,true);
						Main.spriteBatch.Draw(texture2, centerScreen, null, Color.White*alpha, 0f, texture2.Size()/2, 1f, SpriteEffects.None, 0f);
						//normal
						Main.spriteBatch.BeginNormal(true,true);
						Main.spriteBatch.Draw(texture, centerScreen, null, Color.White*alpha, 0f, texture.Size()/2, 1f, SpriteEffects.None, 0f);
					}
				}
				//Main.fontMouseText
				//ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Deltarune.tobyFont, $"Loading {Deltarune.intro} / 1000", new Vector2(Main.screenWidth/2,Main.screenHeight/2) + new Vector2(0,80), Color.White, 0, Vector2.Zero, Vector2.One);
				Main.spriteBatch.End();
			}
		}
	}
	public class TmodHook : ILoadable
	{
		public void Load() {
			On_OnChatButtonClicked += PostOnChatButtonClicked;
			On_CanUseItem += PostCanUseItem;
		}
		public void Unload() {
			On_OnChatButtonClicked -= PostOnChatButtonClicked;
			On_CanUseItem -= PostCanUseItem;
		}
		static void PostOnChatButtonClicked(orig_OnChatButtonClicked orig,bool firstButton) {
			orig(firstButton);
			if (MyConfig.get.DialogTypewriter) {return;}
			Deltarune.chatNPC = null;
			GlobeNPC.ChatDialog();
		}
		
		public delegate void orig_OnChatButtonClicked(bool firstButton);
		public delegate void Hook_OnChatButtonClicked(orig_OnChatButtonClicked orig, bool firstButton);
		public static event Hook_OnChatButtonClicked On_OnChatButtonClicked {
			add => HookEndpointManager.Add<Hook_OnChatButtonClicked>(Helpme.GetModMethod("NPCLoader","OnChatButtonClicked"), value);
			remove => HookEndpointManager.Remove<Hook_OnChatButtonClicked>(Helpme.GetModMethod("NPCLoader","OnChatButtonClicked"), value);
		}

		static bool PostCanUseItem (orig_CanUseItem orig,Item item, Player player) {
			bool flag = orig(item,player);
			if (flag) {
				GlobeItem.PostCanUseItem(item,player);
			}
			return flag;
		}
		
		public delegate bool orig_CanUseItem(Item item, Player player);
		public delegate bool Hook_CanUseItem(orig_CanUseItem orig,Item item, Player player);
		public static event Hook_CanUseItem On_CanUseItem {
			add => HookEndpointManager.Add<Hook_CanUseItem>(Helpme.GetModMethod("ItemLoader","CanUseItem"), value);
			remove => HookEndpointManager.Remove<Hook_CanUseItem>(Helpme.GetModMethod("ItemLoader","CanUseItem"), value);
		}
	}
}
