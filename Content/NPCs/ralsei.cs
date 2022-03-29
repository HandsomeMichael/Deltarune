using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Deltarune;
using Deltarune.Content.Dusts;
using Deltarune.Helper;
using Terraria.UI.Chat;
using Terraria.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;


namespace Deltarune.Content.NPCs
{
	// no way !! auto load head !!!
	[AutoloadHead]
	public class ralsei : ModNPC , ICustomHead
	{
		public static int expression;

		public void CustomHead(ref int num, ref float scale) {
			num = expression;
		}

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Ralsei");
		}
		public override void SetDefaults() {
			npc.width = 40;
			npc.height = 80;
			npc.aiStyle = -1;
			npc.lifeMax = 1000;
			npc.friendly = true;
			npc.townNPC = true;
			npc.knockBackResist = 0.2f;
			npc.defense = 5;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.scale = 0.8f;
		}
		const float state_reveal = -1;
		const float state_idle = 0;
		const float state_walk = 1;
		const float state_needrevive = 2;

		int KRISHELPMEIGOTHITKRISWTFAREUDOING;

		public float state {get => npc.ai[0]; set => npc.ai[0] = value;}
		public float timer {get => npc.ai[1]; set => npc.ai[1] = value;}
		public float dir {get => npc.ai[2]; set => npc.ai[2] = value;}

		public override void AI() {
			npc.spriteDirection = (npc.velocity.X > 0) ? 1 : -1;
			//guh??
			if (state == state_needrevive) {
				npc.spriteDirection = (npc.Center.X > Main.LocalPlayer.Center.X) ? 1 : -1;
			}
			if (npc.velocity.Y > 1 || npc.velocity.Y < -1) {
				npc.rotation += (npc.velocity.Y*0.01f)*npc.spriteDirection;
			}
			else {npc.rotation = npc.rotation.AngleLerp(0f,0.1f);}
			if (state != state_reveal && state != state_needrevive && Main.LocalPlayer.talkNPC == npc.whoAmI) {
				state = state_idle;
				frame = 0;
				timer = Main.rand.Next(60,120);
				dir = 0f;
				npc.velocity.X = 0;
				return;		
			}
			if (state == state_idle) {
				int b = npc.NearestNPC(100f);
				if (b > -1) {
					state = state_walk;
					timer = 60*8;
				}
				npc.velocity.X = MathHelper.Lerp(npc.velocity.X,0f,0.2f);
				timer--;
				if (timer <= 0) {
					frame = 0;
					state = state_walk;
					timer = Main.rand.Next(60*5,60*7);
				}
			}
			if (state == state_walk) {
				int b = npc.NearestNPC();
				if (npc.velocity.X == 0f && dir != 0f && (timer % 5 == 0)) {
					if (npc.velocity.Y == 0f) {
						npc.velocity.Y = -5f;
					}
				}
				if (b > -1) {
					npc.velocity.X = MathHelper.Lerp(npc.velocity.X,(npc.DirectionTo(Main.npc[b].Center)*-3f).X,0.1f);
				}
				else {
					if (npc.velocity.Y == 0f && dir != 0f) {
						int score = 0;
						for (int i = 0; i < 3; i++){
							for (int j = 0; j < 5; j++){
								Vector2 detectPos = npc.Center + new Vector2(npc.spriteDirection*(16*(1+i)),(j+2)*16);
								Tile tile = Framing.GetTileSafely((int)detectPos.X/16, (int)detectPos.Y/16);
								if (tile != null && !tile.active() || !Main.tileSolid[tile.type]) {
									score++;
								}
							}
						}
						if (score >= 13) {dir = dir*-1f;}
					}
					if (dir == 0 || (timer % 60 == 0 && Main.rand.NextBool(5))) {
						dir = (float)Main.rand.Next(-1,2);
					}
					npc.velocity.X = MathHelper.Lerp(npc.velocity.X,1.5f*dir,0.1f);
				}
				timer--;
				if (timer <= 0) {
					frame = 0;
					dir = 0f;
					state = state_idle;
					timer = Main.rand.Next(60*3,60*4);
				}
			}
			if (state == state_needrevive) {
				npc.velocity.X = 0f;
				npc.dontTakeDamage = true;
				timer++;
				if (timer % 60 == 0) {npc.velocity.Y -= 3f;}
				if (timer > 60*4) {
					var b = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_txtral");	
					string text = Main.rand.Next(new string[] { "Help !", "Anyone here ?", "Help me !", "Please Help Me !"});
					npc.GetDelta().textOverhead = new TypeWriter(text,b,3);
					timer = 0;
				}
			}
			else {npc.dontTakeDamage = false;}
			
		}

		//ralsei cant die, he is immortal
		public override bool CheckDead() {
			state = state_needrevive;
			timer = 0;
			npc.life = 1;
			return false;
		}

		// everyone : nooo thats not how you use npc.frame !!
		// me : HAHA GetFrame extension go brrrrrrrr
		public int frame {get => npc.frame.X; set => npc.frame.X = value;}

		public override void FindFrame(int frameHeight) {
			if (state == state_reveal) {
				npc.frameCounter++;
				if (npc.frameCounter >= 4) {
					frame += 1;
					npc.frameCounter = 0;
				}
				if (frame > 7) {frame = 7;}
			}
			if (state == state_walk) {
				npc.frameCounter++;
				if (npc.frameCounter >= 10) {
					frame += 1;
					npc.frameCounter = 0;
				}
				if (frame >= 4) {frame = 0;}
			}
		}
		public override string GetChat() {
			expression = 0;
			if (state == state_reveal) {
				return "Let me state this again, I am Ralsei the prince of dark and will you care to join me on a quest to save terraria from the evil lunar lord ?";
			}
			if (state == state_needrevive) {
				expression = 3;
				foreach (var item in Main.LocalPlayer.inventory){
					if (item.healLife > 0) {
						expression = 2;
						return $"{Main.LocalPlayer.name} please give me one of your healing potion !";
					}
				}
				return $"Pls help me {Main.LocalPlayer.name}, i need a potion !";
			}
			expression = 3;
			return "L + ratio + didn't ask + skill issue + you fell off";
		}
		public override void SetChatButtons(ref string button, ref string button2) {
			if (state == state_reveal) {
				button = "Ok, i will join u";
			}
			if (state == state_needrevive) {
				foreach (var item in Main.LocalPlayer.inventory){
					if (item.healLife > 0) {
						button = "Heal";
						break;
					}
				}
			}
		}
		public override void OnChatButtonClicked(bool firstButton, ref bool shop) {
			if (state == state_reveal) {
				expression = 1;
				frame = 0;
				MyWorld.hadRalsei = true;
				state = state_idle;
				Main.npcChatText = "Really ? Thanks !! I really appreciate it";
			}
			if (state == state_needrevive) {
				foreach (var item in Main.LocalPlayer.inventory){
					if (item.healLife > 0) {
						if (item.stack > 1) {item.stack--;}
						else {item.TurnToAir();}
						break;
					}
				}
				expression = 1;
				npc.rotation = 0f;
				npc.life = npc.lifeMax;
				state = state_idle;
				Main.npcChatText = "Thank you !";
				healdust.Spawn(npc.Center,8);
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom,"Sounds/snd_power"),npc.Center);
				CombatText.NewText(npc.getRect(),Color.LightGreen,npc.life);
				var b = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_txtral");
				npc.GetDelta().textOverhead = new TypeWriter("Thanks !",b,3);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {

			//sprite effect
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			if (KRISHELPMEIGOTHITKRISWTFAREUDOING > 0 && state != state_reveal && state != state_needrevive) {
				KRISHELPMEIGOTHITKRISWTFAREUDOING--;
				// 3 frame
				Texture2D texture = ModContent.GetTexture("Deltarune/Content/NPCs/ralsei_hurt");
				Rectangle rec = texture.GetFrame(0,3);
				spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(Main.rand.NextFloat(-3f,3f),Main.rand.NextFloat(-3f,3f)),
				rec, npc.GetAlpha(lightColor), npc.rotation, rec.Size()/2f, npc.scale, spriteEffects, 0f);
				return false;
			}
			if (state == state_reveal) {
				// 5 frame
				Texture2D texture = ModContent.GetTexture("Deltarune/Content/NPCs/ralsei_reveal");
				Rectangle rec = texture.GetFrame(frame,8);
				spriteBatch.Draw(texture, npc.Center - Main.screenPosition,
				rec, npc.GetAlpha(lightColor), npc.rotation, rec.Size()/2f, npc.scale, spriteEffects, 0f);
			}
			if (state == state_idle) {
				// 1 frame
				Texture2D texture = Main.npcTexture[npc.type];
				spriteBatch.Draw(texture, npc.Center - Main.screenPosition,
				null, npc.GetAlpha(lightColor), npc.rotation, texture.Size()/2f, npc.scale, spriteEffects, 0f);
			}
			if (state == state_walk) {
				// 4 frame
				Texture2D texture = null;
				if (npc.velocity.X == 0f) {
					texture = Main.npcTexture[npc.type];
					spriteBatch.Draw(texture, npc.Center - Main.screenPosition,
					null, npc.GetAlpha(lightColor), npc.rotation, texture.Size()/2f, npc.scale, spriteEffects, 0f);	
					return false;
				}
				texture = ModContent.GetTexture("Deltarune/Content/NPCs/ralsei_walk");
				Rectangle rec = texture.GetFrame(frame,4);
				spriteBatch.Draw(texture, npc.Center - Main.screenPosition,
				rec, npc.GetAlpha(lightColor), npc.rotation, rec.Size()/2f, npc.scale, spriteEffects, 0f);
				/*
				for (int i = 0; i < 3; i++){
					for (int j = 0; j < 5; j++){
						Vector2 detectPos = npc.Center + new Vector2(npc.spriteDirection*(16*(1+i)),(j+2)*16);
						Tile tile = Framing.GetTileSafely((int)detectPos.X/16, (int)detectPos.Y/16);
						detectPos -= Main.screenPosition;
						Rectangle hitbox = new Rectangle((int)(detectPos.X),(int)(detectPos.Y),16,16);
						Color color = Color.Orange;
						if (tile != null && !tile.active() || !Main.tileSolid[tile.type]) {
							color = Color.Red;
						}
						spriteBatch.Draw(Main.magicPixel, hitbox, color*0.5f);
					}
				}
				*/
			}
			if (state == state_needrevive) {
				// 3 frame
				Texture2D texture = ModContent.GetTexture("Deltarune/Content/NPCs/ralsei_hurt");
				Rectangle rec = texture.GetFrame(1,3);
				spriteBatch.Draw(texture, npc.Center - Main.screenPosition,
				rec, npc.GetAlpha(lightColor), npc.rotation, rec.Size()/2f, npc.scale, spriteEffects, 0f);
			}
			// haha custom draw go brrrr
			return false;
		}
		public override void HitEffect(int hitDirection, double damage) {
			KRISHELPMEIGOTHITKRISWTFAREUDOING = 10;
		}
	}
	public class ralseiHood : ModNPC, ICustomHead
	{
		public void CustomHead(ref int num, ref float scale) {
			num = 0;
		}
		public override string Texture => "Terraria/Projectile_0";
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("????");
		}
		public override void SetDefaults() {
			npc.width = 40;
			npc.height = 80;
			npc.aiStyle = -1;
			npc.lifeMax = 1000;
			npc.friendly = true;
			npc.townNPC = false;
			npc.knockBackResist = 0f;
			npc.defense = 10;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.scale = 0.8f;
		}
		public override bool CanChat() => true;
		public override string GetChat() {
			return "Are you one of the hero that want to save terraria ?";
		}
		public override void SetChatButtons(ref string button, ref string button2) {
			button = "Yes";
			button2 = "Who are you !?!";
		}
		public override void OnChatButtonClicked(bool firstButton, ref bool shop) {
			npc.Transform(ModContent.NPCType<ralsei>());
			npc.ai[0] = -1f;
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_equip"),npc.Center);
			if (firstButton) {
				Main.npcChatText = "Great ! Let me introduce myself first, I am Ralsei the prince of dark. i'm also here to save terraria from the evil lunar lord, will you care to join me ?";
			}
			else {
				Main.npcChatText = "Well let me introduce myself first, I am Ralsei the prince of dark. will you care join me on a quest to save terraria from the evil lunar lord?";
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			Texture2D texture = ModContent.GetTexture("Deltarune/Content/NPCs/ralsei_reveal");
			Rectangle rec = texture.GetFrame(0,8);
			spriteBatch.Draw(texture, npc.Center - Main.screenPosition,
			rec, npc.GetAlpha(lightColor), npc.rotation, rec.Size()/2f, npc.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}

