using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.UI.Chat;
using Terraria.UI;
using Terraria.Graphics.Shaders;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
//using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.ModLoader.Config;
using Deltarune;
using Deltarune.Helper;
using Deltarune.Content.Items;
using Deltarune.Content.Projectiles;
using Deltarune.Content.Buffs;
using Deltarune.Content.NPCs;
//this
using ReLogic.OS;

namespace Deltarune.Content
{
	public class GlobeNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		bool Graze;
		public double ShortswordBleed;
		int MegaShortswordBleed;
		public bool fatalbleed;
		bool hasBell;
		bool NewDefault;
		public TypeWriter textOverhead = new TypeWriter();
		//client side list for dialog and stuff
		public List<string> customCheck = new List<string>();

		public override void ResetEffects(NPC npc) {
			fatalbleed = false;
		}
		//public int[] Timer = new int[3];
		public override void SetDefaults(NPC npc) {
			customCheck = new List<string>();
		}
		public override bool PreAI(NPC npc) {
			if (!NewDefault) {
				if (!MyWorld.downedStarWalker && NPC.downedBoss3 && npc.lifeMax != int.MaxValue && npc.CanBeChasedBy() && !Deltarune.Boss && npc.life > 5 && !npc.boss &&!npc.friendly) {
					if (Main.rand.NextBool(20)) {
						hasBell = true;
						npc.scale += 0.3f;
						npc.damage += npc.damage/2;
						npc.life *= 2;
						npc.lifeMax *= 2;
						Sync();
					}
				}
				NewDefault = true;
			}
			return base.PreAI(npc);
		}
		public override bool PreDraw(NPC npc,SpriteBatch spriteBatch, Color drawColor) {
			/*
			if (npc.type == NPCID.Guide) {
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
				float intensity = (float)Main.LocalPlayer.GetDelta().debug/1000f;
				GameShaders.Misc["DeathAnimation"].UseOpacity(Main.LocalPlayer.GetDelta().debug).Apply();
				//Main.NewText("Int = "+intensity);
				//GameShaders.Misc["WaveWrap"].UseOpacity(Main.LocalPlayer.GetDelta().debug).Apply();
			}
			*/
			if (hasBell) {
				spriteBatch.BeginDyeShader(ItemID.ReflectiveGoldDye, npc, true);
				//ReflectiveGoldDye
			}
			return base.PreDraw(npc,spriteBatch, drawColor);
		}

		public override void PostDraw(NPC npc,SpriteBatch spriteBatch, Color drawColor) {
			if (hasBell) {
				spriteBatch.BeginNormal(true);
			}
			/*
			if (npc.type == NPCID.Guide) {
				spriteBatch.BeginNormal(true);
			}
			*/
			if (textOverhead != null && textOverhead.active && Main.LocalPlayer.talkNPC != npc.whoAmI) {
				float max = 60f;
				float num = textOverhead.GetTime();
				if (num > max) {num = max;}
				Vector2 pos = default(Vector2);
				TextSnippet[] snippets = ChatManager.ParseMessage(textOverhead.ToString(), Color.White*(num/max)).ToArray();
				Vector2 messageSize = ChatManager.GetStringSize(Deltarune.tobyFont, snippets, Vector2.One);
				pos.X = npc.position.X + (float)(npc.width / 2) - messageSize.X / 2f;
				pos.Y = npc.position.Y - messageSize.Y - 2f;
				pos.Y += npc.gfxOffY;
				pos.Y -= 5f;
				pos = pos.Floor();
				if (Main.player[Main.myPlayer].gravDir == -1f)
				{
					pos.Y -= Main.screenPosition.Y;
					pos.Y = Main.screenPosition.Y + (float)Main.screenHeight - pos.Y;
					pos.Y += 5f;
				}
				else {pos.Y -= 5f;}
				pos -= Main.screenPosition;
				int hoveredSnippet = 0;
				Rectangle rec = new Rectangle((int)pos.X,(int)pos.Y,(int)messageSize.X,(int)messageSize.Y);
				rec.Resize(4);
				spriteBatch.Draw(Main.chatBackTexture, rec, (Color.Black*(num/max))*0.5f);
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Deltarune.tobyFont, snippets, pos, 0f, Vector2.Zero, Vector2.One, out hoveredSnippet);
			}
		}
		public override void ModifyHitByProjectile(NPC npc,Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
			Player player = Main.player[projectile.owner];
			Item playeritem = player.HeldItem;
			if (projectile.type == ModContent.ProjectileType<BaseShortSword>() || projectile.type == ModContent.ProjectileType<ItemProj>()) {
				if (projectile.type == ModContent.ProjectileType<ItemProj>()) {
					ShortswordBleed += 0.2;
				}
				else {
					ShortswordBleed++;
					if (playeritem.modItem != null && playeritem.modItem is Content.Items.ShortswordItem num3) {
						num3.PreBleed(npc,projectile,ref damage, ref knockback,ref crit);
					}
				}
				if (ShortswordBleed >= 6) {
					if (playeritem.modItem != null && playeritem.modItem is Content.Items.ShortswordItem num3) {
						num3.BleedEffect(npc,projectile,ref damage, ref knockback,ref crit);
					}
					for (int i = 0; i < 20; i++){
						Dust.NewDust(npc.position, npc.width, npc.height, 5, 0f, 0f, 0, new Color(255,255,255), 1f);	
					}
					MegaShortswordBleed++;
					damage *= 3;
					if (MegaShortswordBleed >= 4) {
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_damage"),npc.Center);
						player.CameraShake(5);
						CombatText.NewText(npc.getRect(),Color.Red,"Strike");
						crit = true;
						damage *= 2;
						knockback *= 1.5f;
						MegaShortswordBleed = 0;
						for (int i = 0; i < 35; i++)
						{
							Vector2 position = Main.LocalPlayer.Center;
							Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 183, 0f, 0f, 0, new Color(255,255,255), 0.872093f)];
							dust.shader = GameShaders.Armor.GetSecondaryShader(37, Main.LocalPlayer);
						}
					}
					else {
						Main.PlaySound(SoundID.NPCDeath1);
						CombatText.NewText(npc.getRect(),Color.Red,MegaShortswordBleed);
					}
					ShortswordBleed = 0;
				}
			}
		}
		public override void PostAI(NPC npc) {

			if (hasBell && Main.rand.NextBool(3)) {
				Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 87, 0f, 0f, 0, Color.White, 1f)];
				dust.noGravity = true;
			}
			if (fatalbleed) {
				Vector2 v = npc.velocity*0.3f;
				v.RotatedByRandom(MathHelper.ToRadians(90));
				Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 88, v.X, v.Y, 0, Color.White, 0.7f)];
				dust.noGravity = true;
			}
			if (ShortswordBleed > 6 && Main.rand.NextBool(3)) {
				Dust.NewDust(npc.position, npc.width, npc.height, 5, 0f, 0f, 0, new Color(255,255,255), 1f);	
			}
			if (!npc.friendly && npc.damage > 0 && npc.type != ModContent.NPCType<Content.NPCs.RalseiDummy.ralseidummy>()) {
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					Player player = Main.player[i];
					if (player.active && !player.dead) {
						var p = player.GetDelta();
						if (p.TP < p.TPMax && p.TPCooldown < 1 && !npc.Hitbox.Intersects(player.Hitbox) && npc.Hitbox.Intersects(p.TPBox())) {
							p.Graze(npc,npc.damage,Graze);
							Graze = true;
						}
					}
				}
			}
			if (textOverhead.active) {textOverhead.Update(npc.boss,npc.position.X,npc.position.Y);}
			Dialoging(npc);
			//AdditiveHandler.NPC(npc);
		}
		public void Sync() {
			var packet = mod.GetPacket();
			packet.Write((byte)NetType.GlobalNPC);
			SendPacket(packet);
			packet.Send();
		}
		public void SendPacket(BinaryWriter packet) {
			packet.Write(hasBell);
		}
		public void HandlePacket(BinaryReader reader) {
			hasBell = reader.ReadBoolean();
		}
		public int funnyDialog;
		void Dialoging(NPC npc) {
			if (npc.type == NPCID.BoundGoblin || npc.type == NPCID.BoundMechanic || npc.type == NPCID.BoundWizard) {
				funnyDialog++;
				if (funnyDialog > 360) {
					funnyDialog = 0;
					string text = Main.rand.NextString("Help !", "Hello ?", "Please :(", "Help me !", "Please Help Me !");
					textOverhead = new TypeWriter(text,Deltarune.GetSound("text"),3);
				}
			}
			if (npc.type == NPCID.BartenderUnconscious || npc.type == NPCID.SleepingAngler) {
				funnyDialog++;
				if (funnyDialog > 360) {
					funnyDialog = 0;
					string text = Main.rand.NextString("Zzz", "ZzzZz", "zzZzz", "*sleeping noises", "ZZZ");
					textOverhead = new TypeWriter(text,Deltarune.GetSound("text"),3);
				}
			}
			if (MyConfig.get.DialogTypewriter) {
				ChatDialog(npc);
			}
		}
		public static void ChatDialog() {
			if (Main.LocalPlayer.talkNPC > -1) {
				Main.npc[Main.LocalPlayer.talkNPC].GetDelta().ChatDialog(Main.npc[Main.LocalPlayer.talkNPC]);
			}
		}
		public void ChatDialog(NPC npc) {
			if (Main.LocalPlayer.talkNPC == npc.whoAmI) {
				if (Deltarune.chatNPC == null) {
					var b = Deltarune.GetSound("text");
					if (npc.type == ModContent.NPCType<ralsei>()) {
						b = Deltarune.GetSound("txtral");
					}
					if (customCheck.Contains("ThrowedEgg")) {
						string text = Main.rand.NextString("I hate rotten eggs");
						Deltarune.chatNPC = new TypeWriter(text,b,3);
						customCheck.Remove("ThrowedEgg");
					}
					Deltarune.chatNPC = new TypeWriter(frameMax : 2,wanted : Main.npcChatText,sound : b,timeLeft : -1);
					Main.npcChatText = Deltarune.chatNPC.ToString();
				}
				else {
					if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space)) {
						Deltarune.chatNPC.text = Deltarune.chatNPC.wanted;
					}
					Deltarune.chatNPC.Update();
					Main.npcChatText = Deltarune.chatNPC.ToString();
					//nullify chat npc
					if (Deltarune.chatNPC.Done() && Main.npcChatText != Deltarune.chatNPC.ToString()) {Deltarune.chatNPC = null;}
				}
			}
		}
		public override bool PreChatButtonClicked(NPC npc,bool firstButton) {
			Deltarune.chatNPC = null;
			return base.PreChatButtonClicked(npc,firstButton);
		}
		public static void CustomHeadTexture(NPC npc, ref Texture2D texture, ref float scale, ref bool rotate) {
			if (npc.modNPC != null) {
				if (npc.modNPC is ICustomHead head) {
					int expression = 0;
					scale = 0.8f;
					rotate = false;
					head.CustomHead(ref expression,ref scale);
					string path = Deltarune.textureExtra+"face/"+npc.modNPC.Name+"_"+expression;
					texture = ModContent.GetTexture(path);
				}
			}
		}
		public override void OnHitByProjectile(NPC npc,Projectile projectile, int damage, float knockback, bool crit) {
			Player player = Main.player[projectile.owner];
			if (npc.townNPC && projectile.type == ProjectileID.RottenEgg && !Main.player[projectile.owner].dead) {
				if (!customCheck.Contains("ThrowedEgg")){customCheck.Add("ThrowedEgg");}
				var b = Deltarune.GetSound("text");
				string text = Main.rand.Next(new string[] {"Stop","No","Bruh","I Hate you","GO AWAY"});
				textOverhead = new TypeWriter(text,b,3);
			}
		}
		//public override bool StrikeNPC(NPC npc,ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit) {return true;}
		public override void ModifyHitByItem(NPC npc,Player player, Item item, ref int damage, ref float knockback, ref bool crit) {
			if (fatalbleed) {
				damage *= 2;
				for (int i = 0; i < 10; i++)
				{
					Vector2 v = player.velocity;
					v.RotatedByRandom(MathHelper.ToRadians(90));
					Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 88, v.X, v.Y, 0, Color.White, 0.7f)];
					dust.noGravity = true;
				}
			}
		}
		public override void SetupShop(int type, Chest shop, ref int nextSlot) {
			if (type == NPCID.ArmsDealer && NPC.FindFirstNPC(NPCID.Nurse) > -1){
				shop.AddShop(ref nextSlot, ModContent.ItemType<SimpCard>(), Item.buyPrice(gold:5));
			}
		}
		public override void NPCLoot(NPC npc) {
			if (hasBell) {
				npc.DropLoot(-1,ModContent.ItemType<Shatterbell>());
			}
		}
	}
}