﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Deltarune;
using Deltarune.Content;
using Deltarune.Content.Projectiles;
using Deltarune.Helper;
using Terraria.UI.Chat;
using Terraria.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace Deltarune.Content.NPCs.Boss
{
	public class starwalker : ModNPC
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Star Walker");
			Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailCacheLength[npc.type] = 10; //Higher numbers mean longer trails
    		NPCID.Sets.TrailingMode[npc.type] = 0;
		}
		public override void SetDefaults() {
			npc.width = 999;
			npc.height = 999;
			npc.aiStyle = -1;
			npc.damage = 30;
			npc.value = Item.sellPrice(gold : 1);
			npc.boss = true;
			npc.lifeMax = 5500;
			npc.friendly = false;
			npc.knockBackResist = 0f;
			npc.defense = 12;
			npc.HitSound = SoundID.NPCHit2;
			npc.DeathSound = SoundID.NPCDeath2;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.scale = 0.1f;
			music = Deltarune.NPCMusic("battle",MusicID.Boss1);
			npc.alpha = 255;
		}
		int frame;
		bool ignoreFrame;
		public override void FindFrame(int frameHeight) {
			if (!ignoreFrame) {
				npc.frameCounter += 1;
				npc.frameCounter++;
				if (npc.frameCounter >= 15) {
					frame++;
					npc.frameCounter = 0;
					if (frame >= 3) {frame = 0;}
				}
			}
			ignoreFrame = false;
			npc.frame.Y = frame * frameHeight;

			//if(npc.frameCounter % 30 == 0){
			//npc.frameCounter %= Main.npcFrameCount[npc.type];
		}
		//extra ai
		Vector2 targetPos;
		//send
		public override void SendExtraAI(BinaryWriter writer) {
			writer.Write(targetPos.X);
			writer.Write(targetPos.Y);
		}
		//receive
		public override void ReceiveExtraAI(BinaryReader reader) {
			targetPos.X = reader.ReadSingle();
			targetPos.Y = reader.ReadSingle();
		}

		//state
		const int state_retreat = -1;
		const int state_spawn = 0;
		const int state_attnormal = 1;
		const int state_attcircle = 2;
		const int state_attdash = 3;
		const int state_count = 4;

		const int state2_spawn = 4;
		const int state2_attnormal = 5;
		const int state2_attcircle = 6;
		const int state2_attdash = 7;
		const int state2_attcloudy = 8;

		const int state3_madbird = 9;
		const int state3_laugh = 10;

		public float state {get => npc.ai[0];set => npc.ai[0] = value;}
		public float timer {get => npc.ai[1];set => npc.ai[1] = value;}
		public float statetimer {get => npc.ai[2];set => npc.ai[2] = value;}
		//reset timer
		void NewState(int num) {
			state = num;
			timer = 0;
			statetimer = 0;
			targetPos = Vector2.Zero;
		}
		// Allows you to customize this NPC's stats in expert mode. 
		// This is useful because expert mode's doubling of damage and life might be too much sometimes 
		// (for example, with bosses). Also useful for scaling life with the number of players in the world.
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale) {
			npc.lifeMax = (int)(npc.lifeMax / Main.expertLife * 1.2f * bossLifeScale);
		}
		public override void BossLoot(ref string name, ref int potionType) {
			potionType = ItemID.HealingPotion;
		}
		public override void NPCLoot() {
			
			if (!MyWorld.downedStarWalker) {
				Main.NewText("Monster are starting to be normal",Color.LightGreen);
			}

			Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/starwalk0"), 1f);
			Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/starwalk1"), 1f);
			Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/starwalk2"), 1f);
			Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/starwalk3"), 1f);
			Gore.NewGore(npc.Center, npc.velocity, mod.GetGoreSlot("Gores/wing1"), 1f);
			Gore.NewGore(npc.Center, npc.velocity, mod.GetGoreSlot("Gores/wing2"), 1f);

			MyWorld.downedStarWalker = true;
		}

		// warning : spaghetti code
		// i dont want to give much comment on this one
		public override void AI() {
			//startup
			npc.TargetClosest(true);
			Player player = Main.player[npc.target];
			var sound = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/FakeStarwalker").WithVolume(.7f).WithPitchVariance(.5f);	
			Vector2 dir = npc.DirectionTo(player.Center);
			Vector2 proDir = npc.DirectionTo(player.Center+player.velocity);
			//graphics
			npc.spriteDirection = npc.direction;
			npc.rotation = npc.velocity.X * 0.01f;
			//sound
			if (frame == 1 && npc.frameCounter == 0) {
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_wing"),npc.Center);
			}
			//the ai of retreat
			if (state == state_retreat) {
				timer++;
				if (timer >= 120) {
					npc.alpha += 10;
					npc.velocity = Vector2.Lerp(npc.velocity,new Vector2(0,-300),0.1f);
				}
				//return so it doesnt do anything else. funky
				return;
			}
			//retreat
			if (player.dead) {
				npc.TargetClosest(true);
				player = Main.player[npc.target];
				string text = Main.rand.Next(new string[] {"HaHaHa You SUCK !","Thats what you get !","GG NO RE","Pathetic",
				"I'll piss you next time","lmao you suck a lot"});
				if (MyWorld.downedStarWalker) {
					text = Main.rand.Next(new string[] {"GG","I'm not even tryin rn lol","hahahahahahahahahah","you pissed me off","lol i pissed you off"});
				}
				if (player.dead && npc.GetDelta().textOverhead.wanted != text) {
					npc.GetDelta().textOverhead = new TypeWriter(text,sound,3);
					NewState(state_retreat);
					npc.velocity = Vector2.Zero;
				}
			}
			//teleport
			if (!player.dead && state != state_retreat) {
				if (player.Distance(npc.Center) > 1000f) {
					npc.position = Vector2.Lerp(npc.position,player.Center,0.05f);
				}
			}
			#region the actual ai
			//spawn and say lore stuff
			if (state == state_spawn) {
				if (!Main.LocalPlayer.dead && Main.LocalPlayer.Distance(npc.Center) < 1000f) {
					Main.LocalPlayer.GetDelta().CameraFocus(npc.Center,3);
				}
				string text = Main.rand.Next(new string[] {"Lightners !","Perish !","You LIGHTNERS make me SICK !","You are pissing me off !"});
				if (MyWorld.downedStarWalker) {
					text = Main.rand.Next(new string[] {
						"I AM BACK !","I WANT A REMATCH !","you are really pissing me off","You HUMANS make me SICK !",
						"Stop You Filthy HUMANS !"
					});
				}
				npc.dontTakeDamage = true;
				timer++;
				npc.alpha -= 5;
				npc.scale += 0.05f;
				if (npc.scale > 1f) {npc.scale = 1f;}
				if (npc.alpha < 0) {npc.alpha = 0;}
				if (timer == 60) {
					//Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/amogus"),npc.Center);
					npc.GetDelta().textOverhead = new TypeWriter(text,sound,3, timeLeft : 60);
				}
				if (timer >= 160) {
					npc.dontTakeDamage = false;
					NewState(state_attnormal);
				}
			}
			//attack normal. goes to player and shoot
			if (state == state_attnormal) {
				timer++;
				statetimer++;
				Vector2 pos = player.Center + (player.velocity*2f) - new Vector2(0,200);
				if (npc.Center.X > pos.X) {
					pos.X += 50f;
				}
				else {pos.X -= 50f;}
				npc.velocity = Vector2.Lerp(npc.velocity,npc.DirectionTo(pos)*10f,0.1f);
				if (timer == 40) {
					ignoreFrame = true;
					frame = 4;
					npc.frameCounter = -100;
				}
				if (timer >= 60) {
					ignoreFrame = true;
					frame = 5;
					npc.frameCounter = -10;
					if (Main.netMode != NetmodeID.MultiplayerClient) {
						for (int i = 0; i < 3; i++){
							Vector2 vel = (dir*6f).RotatedByRandom(MathHelper.ToRadians(30)); // 30 degree spread.
							Projectile.NewProjectile(npc.Center,vel,ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer);	
						}
					}
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_crow"),npc.Center);
					timer = 0;
				}
				if (statetimer >= 60*10) {
					if (Main.netMode != NetmodeID.MultiplayerClient) {
						for (int i = 0; i < 5; i++){
							Vector2 vel = (dir*6f).RotatedByRandom(MathHelper.ToRadians(360)); // 30 degree spread.
							Projectile.NewProjectile(npc.Center,vel,ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer);
						}
					}
					ignoreFrame = false;
					npc.frameCounter = 15;
					//state = Main.rand.Next(1,4);
					NewState(state_attdash);
				}
			}
			//attack dash state
			if (state == state_attdash) {
				timer++;
				statetimer++;
				if (timer < 60) {
					Vector2 pos = player.Center;
					if (npc.Center.X > pos.X) {
						pos.X += 250f;
					}
					else {pos.X -= 250f;}
					pos = npc.DirectionTo(pos)*6f;
					npc.velocity = Vector2.Lerp(npc.velocity,pos,0.1f);
				}
				if (timer == 60) {
					if (Main.netMode != NetmodeID.MultiplayerClient) {
						for (int i = 0; i < 5; i++){
							Vector2 vel = (dir*6f).RotatedByRandom(MathHelper.ToRadians(360)); // 30 degree spread.
							Projectile.NewProjectile(npc.Center,vel,ModContent.ProjectileType<starwalkerStar>(),npc.damage/2,0f,Main.myPlayer);
						}
					}
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_spearrise"),npc.Center);
					targetPos = npc.DirectionTo(player.Center + player.velocity)*28f;
				}
				if (timer > 60) {
					npc.velocity = Vector2.Lerp(npc.velocity,targetPos,0.1f);
				}
				if (timer >= 100) {
					timer = 0;
					targetPos = npc.Center;
				}
				if (statetimer >= 60*10) {
					NewState(state_attcircle);
				}
			}
			//circle around player and randomly shoot
			if (state == state_attcircle) {
				timer++;
				statetimer++;
				npc.velocity = Vector2.Lerp(npc.velocity,Vector2.Zero,0.1f);
				Vector2 pos = player.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(statetimer))*200f;
				npc.Center = Vector2.Lerp(npc.Center,pos,0.1f);
				if (timer == 70 - 20) {
					ignoreFrame = true;
					frame = 4;
					npc.frameCounter = -100;
				}
				if (timer >= 70) {
					ignoreFrame = true;
					frame = 5;
					npc.frameCounter = -10;
					if (Main.netMode != NetmodeID.MultiplayerClient) {
						for (int i = 0; i < 12; i++){
							Vector2 vel = (dir*6f).RotatedBy(MathHelper.ToRadians(30*i)); // 30 degree spread.
							Projectile.NewProjectile(npc.Center,vel,ModContent.ProjectileType<starwalkerStar>(),npc.damage/2,0f,Main.myPlayer);
						}
						//Projectile.NewProjectile(npc.Center,dir*6f,ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer);	
					}
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_crow"),npc.Center);
					timer = 0;
				}
				if (statetimer >= 60*10) {
					NewState(state_attnormal);
				}
			}
			//state 2 check . lets goo baby
			if (state > state_spawn && state < state_count && ((float)npc.life < (float)npc.lifeMax*0.65f)) {
				ignoreFrame = false;
				music = 0;
				string text = "FOOLISH !";
				npc.GetDelta().textOverhead = new TypeWriter(text,sound,3, timeLeft : 120);
				npc.velocity = Vector2.Zero;
				NewState(state2_spawn);
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/foolish2"));
				//foolish2
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_revival"),npc.Center);
				//foolish
			}
			//introduce
			if (state == state2_spawn) {
				timer++;
				npc.dontTakeDamage = true;
				if (Main.LocalPlayer.Distance(npc.Center) < 1000f) {
					Main.LocalPlayer.GetDelta().CameraFocus(npc.Center,10);
					Main.LocalPlayer.CameraShake((int)((timer/230f)*10f));
				}
				if (timer > 230f) {
					music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/creepychase");
					NewState(state2_attnormal);
					npc.dontTakeDamage = false;
				}
			}
			//the dread state. is no hit even possible :o
			if (state == state2_attnormal) {
				timer++;
				statetimer++;

				Vector2 pos = player.Center + (player.velocity*-1f) - new Vector2(0,180);
				if (npc.Center.X > pos.X) {pos.X += 200f;}
				else {pos.X -= 200f;}
				npc.velocity = Vector2.Lerp(npc.velocity,npc.DirectionTo(pos)*10f,0.2f);

				if (timer == 40) {
					ignoreFrame = true;
					frame = 4;
					npc.frameCounter = -100;
				}
				if (timer >= 60) {
					ignoreFrame = true;
					frame = 5;
					npc.frameCounter = -10;
					if (Main.netMode != NetmodeID.MultiplayerClient) {
						int type = Main.rand.Next(1,5);
						if (type == 1) {
							if (Main.expertMode) {
								for (int i = 0; i < 12; i++){
									Vector2 vel = (dir*6f).RotatedBy(MathHelper.ToRadians(30*i)); // 30 degree spread.
									Projectile.NewProjectile(npc.Center,vel,ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer,1f);
								}
							}
							else {
								for (int i = 0; i < 6; i++){
									Vector2 vel = (dir*4f).RotatedBy(MathHelper.ToRadians(60*i)); // 30 degree spread.
									Projectile.NewProjectile(npc.Center,vel,ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer,1f);
								}
							}
						}
						if (type == 2) {
							for (int i = 0; i < 6; i++){
								int funk = i - 3;
								Vector2 vel = new Vector2(funk*5f,-10);
								Projectile.NewProjectile(npc.Center,vel,ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer,2f);
							}
						}
						if (type == 3) {
							npc.velocity = dir*20f;
							for (int i = 0; i < 3; i++){
								Vector2 vel = (proDir*6f).RotatedByRandom(MathHelper.ToRadians(60)); // 30 degree spread.
								Projectile.NewProjectile(npc.Center,vel,ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer,3f);	
							}
						}
						if (type == 4) {
							Projectile.NewProjectile(npc.Center,proDir*10f,ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer,4f);	
						}
					}
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_crow"),npc.Center);
					timer = 0;
				}
				//Main.GameUpdateCount % 60
				if (statetimer % 240 == 0) {
					int a = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<starwalkerStarNPC>());
					Main.npc[a].velocity = new Vector2(Main.rand.NextFloat(-15,15),Main.rand.NextFloat(-15,15));
					//starwalkerStarNPC
				}
				if (statetimer >= 60*10) {
					if (Main.netMode != NetmodeID.MultiplayerClient) {
						//funk
						Projectile.NewProjectile(npc.Center,new Vector2(10,0),ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer,1f);
						Projectile.NewProjectile(npc.Center,new Vector2(-10,0),ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer,1f);
						Projectile.NewProjectile(npc.Center,new Vector2(0,10),ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer,1f);
						Projectile.NewProjectile(npc.Center,new Vector2(0,-10),ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer,1f);
					}
					NewState(state2_attdash);
				}
			}
			//the dash state 2
			if (state == state2_attdash) {
				timer++;
				statetimer++;
				if (timer < 30) {
					Vector2 pos = player.Center;

					if (npc.Center.X > pos.X) {pos.X += 250f;}
					else {pos.X -= 250f;}

					if (npc.Center.Y > pos.Y) {pos.Y -= 100f;}
					else {pos.Y += 100f;}

					pos = npc.DirectionTo(pos)*6f;
					npc.velocity = Vector2.Lerp(npc.velocity,pos,0.1f);
				}
				if (timer == 30) {
					if (Main.netMode != NetmodeID.MultiplayerClient && Main.expertMode) {
						for (int i = 0; i < 6; i++){
							Vector2 vel = (dir*6f).RotatedBy(MathHelper.ToRadians(60*i)); // 30 degree spread.
							Projectile.NewProjectile(npc.Center,vel,ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer,1f);
						}
					}
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_spearrise"),npc.Center);
					targetPos = npc.DirectionTo(player.Center + player.velocity)*38f;
				}
				if (timer > 30) {
					if (statetimer % 4 == 0) {
						if (Main.netMode != NetmodeID.MultiplayerClient) {
							Projectile.NewProjectile(npc.Center,dir*-3f,ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer,1f);
						}
					}
					npc.velocity = Vector2.Lerp(npc.velocity,targetPos,0.1f);
				}
				if (timer >= 80) {
					timer = 0;
					targetPos = npc.Center;
				}
				if (statetimer >= 60*10) {
					NewState(state2_attcloudy);
				}
			}
			//the most easiest state i guess
			if (state == state2_attcloudy) {
				timer++;
				statetimer++;

				Vector2 pos = player.Center + (player.velocity*-1f) - new Vector2(0,350);
				bool right = false;
				if (npc.Center.X > pos.X) {
					right = true;
					pos.X += 130f;
				}
				else {pos.X -= 130f;}

				if (timer == 60) {
					if (right) {targetPos = new Vector2(-30,0);}
					else {targetPos = new Vector2(30,0);}
				}
				if (timer > 60) {
					//Main.NewText($"x = {targetPos.X} / (ORIG) {npc.velocity.X}, y = {targetPos.Y} / (ORIG) {npc.velocity.Y}");
					npc.velocity = Vector2.Lerp(npc.velocity,targetPos,0.1f);
					if (statetimer % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient) {
						Projectile.NewProjectile(npc.Center,new Vector2(0,-2f),ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer,5f);
					}
				}
				else {npc.velocity = Vector2.Lerp(npc.velocity,npc.DirectionTo(pos)*10f,0.2f);}
				if (timer > 60*2) {
					timer = 0;
					targetPos = npc.Center;
				}
				if (statetimer >= 60*10) {
					NewState(state2_attcircle);
				}
			}
			//circle
			if (state == state2_attcircle) {
				timer += 3f;
				statetimer++;
				npc.velocity = Vector2.Lerp(npc.velocity,Vector2.Zero,0.1f);
				Vector2 pos = player.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(statetimer))*300f;
				npc.Center = Vector2.Lerp(npc.Center,pos,0.1f);
				if (statetimer % 5 == 0) {
					if (Main.netMode != NetmodeID.MultiplayerClient) {
						Projectile.NewProjectile(npc.Center,Vector2.Zero,ModContent.ProjectileType<starwalkerStar>(),npc.damage/2,0f,Main.myPlayer,7f);
					}
				}
				if (timer % (180 - 20) == 0) {
					ignoreFrame = true;
					frame = 4;
					npc.frameCounter = -100;
				}
				if (timer % 180 == 0) {
					ignoreFrame = true;
					frame = 5;
					npc.frameCounter = -10;
					if (Main.netMode != NetmodeID.MultiplayerClient) {
						for (int i = 0; i < 12; i++){
							Vector2 vel = (dir*6f).RotatedBy(MathHelper.ToRadians(30*i)); // 30 degree spread.
							Projectile.NewProjectile(npc.Center,vel,ModContent.ProjectileType<starwalkerStar>(),npc.damage/2,0f,Main.myPlayer,0f,1f);
						}
						//Projectile.NewProjectile(npc.Center,dir*6f,ModContent.ProjectileType<starwalkerStar>(),npc.damage,0f,Main.myPlayer);	
					}
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_crow"),npc.Center);
				}
				if (statetimer >= 60*10) {
					NewState(state2_attnormal);
				}
			}
			#endregion
			//for syncing projectile if i forgor
			//NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
		}
		
		public override Color? GetAlpha(Color drawColor) {
			if (state >= state_count) {return Color.White;}
			return null;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			Texture2D texture = Main.npcTexture[npc.type];
			Vector2 orig = texture.GetFrame(frame,6).Size()/2f;
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			if (state == state_attdash || state >= state_count) {
				if (state >= state_count) {
					texture = ModContent.GetTexture(Texture+"_red");
				}
				for (int k = 0; k < npc.oldPos.Length; k++){
					Vector2 oldCenter = new Vector2(npc.oldPos[k].X + (float)(npc.width / 2), npc.oldPos[k].Y + (float)(npc.height / 2));
					Vector2 drawPos = oldCenter - Main.screenPosition;
					Color color = npc.GetAlpha(lightColor) * ((float)(npc.oldPos.Length - k) / (float)npc.oldPos.Length);
					spriteBatch.Draw(texture, drawPos, npc.frame, color, npc.rotation, orig, npc.scale, spriteEffects, 0f);
				}
			}
			texture = Main.npcTexture[npc.type];
			if (state == state2_spawn) {
				float alpha = timer/230f;
				alpha = 1f-alpha;
				spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(Main.rand.NextFloat(-3,3),Main.rand.NextFloat(-3,3)),
				npc.frame, Color.White*alpha, npc.rotation, orig, npc.scale+((timer/230f)*2), spriteEffects, 0f);
			}
			spriteBatch.Draw(texture, npc.Center - Main.screenPosition,
			npc.frame, npc.GetAlpha(lightColor), npc.rotation, orig, npc.scale, spriteEffects, 0f);
			return false;
		}
		//public override void PostDraw(SpriteBatch spriteBatch, Color lightColor) {}
	}
	public class starwalkerStar : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Starwalker Star");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
		}
		public override void SetDefaults() {
			projectile.width = 16;
			projectile.height = 16;
			projectile.aiStyle = 0;
			projectile.friendly = false;
			projectile.penetrate = -1;
			projectile.scale = 1f;
			projectile.extraUpdates = 1;
			projectile.hostile = true;
			projectile.timeLeft = 600;
			projectile.tileCollide = false;
		}
		public override void AI() {
			//homes after 1 second
			if (projectile.ai[0] == 1f) {
				projectile.hostile = false;
				projectile.alpha = 100;
				projectile.ai[1]++;
				projectile.scale += 0.005f;
				if (projectile.ai[1] > 60f) {
					projectile.hostile = true;
					projectile.alpha = 0;
					int a = Helpme.NearestPlayer(projectile.Center);
					if (a != -1) {
						projectile.velocity = projectile.DirectionTo(Main.player[a].Center + Main.player[a].velocity*-1f)*6f;
						if (Main.expertMode) {
							projectile.velocity = projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15));
						}
						else {
							projectile.velocity = projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30));
						}
					}
					projectile.ai[0] = 0f;
				}
			}
			//gravity
			if (projectile.ai[0] == 2f) {
				projectile.velocity.Y += 0.5f;
				if (projectile.velocity.Y > 8f) {projectile.velocity.Y = 8f;}
			}
			//rapid homing
			if (projectile.ai[0] == 3f) {
				projectile.ai[1]++;
				if (projectile.ai[1] > 60f) {
					int a = Helpme.NearestPlayer(projectile.Center);
					if (a != -1) {
						projectile.velocity = projectile.DirectionTo(Main.player[a].Center + Main.player[a].velocity*-0.5f)*6f;
					}
					projectile.ai[0] = 0f;
				}
			}
			//explode on dead
			if (projectile.ai[0] == 4f) {
				projectile.velocity *= 0.98f;
				projectile.ai[1]++;
				if (projectile.ai[1] > 80f) {
					projectile.Kill();
				}
				return;
			}
			//go bounce after 2 second and gravity
			if (projectile.ai[0] == 5f) {
				projectile.scale += 0.001f;
				projectile.ai[1]++;
				projectile.velocity.Y += 0.5f;
				if (projectile.velocity.Y > 5f) {projectile.velocity.Y = 5f;}
				if (projectile.ai[1] > 60*2f) {
					projectile.velocity = new Vector2(0,-6);
					projectile.ai[0] = 0f;
				}
				projectile.scale -= 0.001f;
				if (projectile.scale <= 0f) {
					projectile.Kill();
				}
				return;
			}
			//pure visual 
			if (projectile.ai[0] == 6f) {
				projectile.hostile = false;
				projectile.scale += 0.1f;
				projectile.alpha += 3;
				if (projectile.alpha >= 255) {
					projectile.Kill();
				}
				return;
			}
			//pure among
			if (projectile.ai[0] == 7f) {
				if (projectile.ai[1] == 0f) {
					projectile.ai[1] = 1f;
					projectile.alpha = 255;
				}
				projectile.hostile = false;
				projectile.alpha -= 5;
				if (projectile.alpha <= 0 && projectile.ai[1] == 1f) {
					projectile.hostile = true;
					int a = Helpme.NearestPlayer(projectile.Center);
					if (a != -1) {
						projectile.velocity = projectile.DirectionTo(Main.player[a].Center + Main.player[a].velocity*-1f)*-2f;
					}
					projectile.ai[0] = 0f;
				}
				return;
			}
			//homes super inacuratly
			if (projectile.ai[0] == 8f) {
				projectile.hostile = false;
				projectile.alpha = 100;
				projectile.ai[1]++;
				projectile.scale += 0.005f;
				if (projectile.ai[1] > 60f) {
					projectile.hostile = true;
					projectile.alpha = 0;
					int a = Helpme.NearestPlayer(projectile.Center);
					if (a != -1) {
						projectile.velocity = projectile.DirectionTo(Main.player[a].Center + Main.player[a].velocity*-5f)*6f;
						projectile.velocity = projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30));
					}
					projectile.ai[0] = 0f;
				}
			}

			projectile.scale -= 0.005f;
			if (projectile.scale <= 0.45f) {
				projectile.hostile = false;
				projectile.alpha += 10;
				if (projectile.alpha >= 255) {
					projectile.Kill();
				}
			}
			if (projectile.ai[0] != 0f || projectile.ai[1] != 0f) {return;}
			int i = (int)(projectile.position.X/16);
			int j = (int)(projectile.position.Y/16);
			//Main.NewText($"{i} / {Main.maxTilesX} || {j} / {Main.maxTilesY}");
			// there is this funky bug with tmod where its just spam indexoutofbounds so i need to put this here :(
			if (i > Main.maxTilesX || j > Main.maxTilesY || i < 0 || j < 0) {
				return;
			}
			Tile tile = Framing.GetTileSafely(i,j);
			if (tile != null && tile.active() && Main.tileSolid[tile.type]) {
				int a = Helpme.NearestPlayer(projectile.Center);
				if (a != -1) {
					projectile.velocity = Vector2.Lerp(projectile.velocity,projectile.DirectionTo(Main.player[a].Center)*2f,0.05f);
				}
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			if (projectile.ai[0] != 0f || projectile.ai[1] != 0f) {lightColor = Color.Red;}
			//Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++) {
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}
		public override void Kill(int timeLeft) {
			if (projectile.ai[0] == 4f) {
				Projectile.NewProjectile(projectile.Center,Vector2.Zero,ModContent.ProjectileType<starwalkerStar>(),0,0f,Main.myPlayer,6f);
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_crow"),projectile.Center);
				if (!Main.expertMode) {
					for (int i = 0; i < 6; i++){
						Vector2 vel = projectile.velocity.RotatedBy(MathHelper.ToRadians(60*i)); // 30 degree spread.
						Projectile.NewProjectile(projectile.Center,vel,ModContent.ProjectileType<starwalkerStar>(),projectile.damage,0f,Main.myPlayer,8f);
					}
					return;
				}
				for (int i = 0; i < 12; i++){
					Vector2 vel = projectile.velocity.RotatedBy(MathHelper.ToRadians(30*i)); // 30 degree spread.
					Projectile.NewProjectile(projectile.Center,vel,ModContent.ProjectileType<starwalkerStar>(),projectile.damage,0f,Main.myPlayer,8f);
				}
			}
		}
	}
	public class starwalkerStarNPC : ModNPC
	{
		public override string Texture => "Deltarune/Content/NPCs/Boss/starwalkerStar";
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Star Runner");
		}
		public override void SetDefaults() {
			npc.width = 10;
			npc.height = 10;
			npc.aiStyle = -1;
			npc.damage = 30;
			npc.value = 0;
			npc.lifeMax = 20;
			npc.friendly = false;
			npc.knockBackResist = 1f;
			npc.defense = 2;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.alpha = 255;
		}
		public override void AI() {
			npc.TargetClosest(true);
			Player player = Main.player[npc.target];
			if (player.dead) {
				npc.TargetClosest(true);
				player = Main.player[npc.target];
				if (player.dead) {
					npc.life = 0;
					npc.active = false;
					npc.HitEffect();
				}
			}
			npc.alpha -= 10;
			if (npc.alpha < 0) {npc.alpha = 0;}
			npc.velocity += new Vector2(Main.rand.NextFloat(-3,3),Main.rand.NextFloat(-3,3));
			npc.velocity = Vector2.Lerp(npc.velocity,npc.DirectionTo(player.Center + player.velocity*-0.5f)*5f,0.05f);
			npc.rotation = npc.velocity.X * 0.001f;
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {

			Texture2D texture = ModContent.GetTexture(Texture+"_Gloow");
			Vector2 orig = texture.Size()/2f;
			spriteBatch.Draw(texture, npc.Center - Main.screenPosition,
			null, npc.GetAlpha(Color.Black), npc.rotation, orig, npc.scale, SpriteEffects.None, 0f);

			spriteBatch.BeginGlow(true);
			spriteBatch.Draw(texture, npc.Center - Main.screenPosition,
			null, npc.GetAlpha(Color.White), npc.rotation, orig, npc.scale, SpriteEffects.None, 0f);

			spriteBatch.BeginNormal(true);

			texture = Main.npcTexture[npc.type];
			orig = texture.Size()/2f;
			spriteBatch.Draw(texture, npc.Center - Main.screenPosition,
			null, npc.GetAlpha(lightColor), npc.rotation, orig, npc.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}
