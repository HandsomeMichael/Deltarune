using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.ModLoader.Config;
using Deltarune.Helper;
using Deltarune.Content.Spell;
using System.Net;//le troll

namespace Deltarune
{
	public class DeltaPlayer : ModPlayer
	{
		public Rectangle TPBox() {
			int bonus = TPScale+TPScale2+(int)(TPDisplay*10f);
			int x = (int)player.position.X - ((bonus)/2);
			int y = (int)player.position.Y - ((bonus)/2);
			int width = player.width+bonus;
			int height = player.height+bonus; 
			return new Rectangle(x,y,width,height);
		}

		public const int TPScale = 120;
		public int TPScale2;
		public int TPCooldown;
		public int TPMax => player.statLifeMax2*5;
		public float TPDisplay => (float)TP/(float)TPMax;
		public int TP;

		public float Shortswordatt;

		public Vector2 soul;
		public Vector2 soulBox;
		public int soulBoxWidth;
		public int soulBoxHeight;
		public int soulTimer;
		// no soul ???
		public void UpdateSoulBox(Vector2 pos, int width = -1, int height = -1) {
			soulBox = pos;
			UpdateSoulBox(width,height);
		}
		public void UpdateSoulBox(int width = -1, int height = -1) {
			if (width != -1)soulBoxWidth = width;
			if (height != -1)soulBoxHeight = height;
		}
		public void ExitSoul(int timer, int width = 200, int height = 200) => ExitSoul(player.Center,timer,width,height);
		public void ExitSoul(Vector2 box, int timer, int width = 200, int height = 200) {
			soulBox = box;
			soulTimer = timer;
			soulBoxWidth = width;
			soulBoxHeight = height;
			player.immuneTime += 60;
		}

		public int spellAnim;
		public string spellAnimTex;
		public bool receiveBag;
		public bool sacredrock;
		public Vector2? richochetBullet;

		public BaseSpell[] spellItem = new BaseSpell[3];
		public int[] spell = new int[3];
		public int[] spellTimer = new int[3];

		public List<string> dialogTag = new List<string>();
		public static bool HasDialog(string dialog) => Main.LocalPlayer.GetDelta().dialogTag.Contains(dialog);
		public static void AddDialog(string dialog) => Main.LocalPlayer.GetDelta().dialogTag.Add(dialog);
		public static void RemDialog(string dialog, bool check = true) {
			if (!check || HasDialog(dialog)) {
				Main.LocalPlayer.GetDelta().dialogTag.Remove(dialog);
			}
		}

		public float moveSpeed;

		public override void ResetEffects() {
			moveSpeed = 0f;
			sacredrock = false;
			if (TPCooldown > 0){TPCooldown--;}
			for (int i = 0; i < spellTimer.Length; i++){if (spellTimer[i] > 0) {spellTimer[i]--;}}
			TPScale2 = 0;
			for (int i = 0; i < spell.Length; i++){
				if (spell[i] > 0) {
					Item item = new Item();
					item.SetDefaults(spell[i]);
					if (item.modItem is BaseSpell s) {
						spellItem[i] = s;
					}
				}
				else {spellItem[i] = null;}
			}
			spell = new int[3];
		}
		public override void PostUpdateRunSpeeds() {
			player.accRunSpeed += moveSpeed;
		}
		//public override void SetControls() {}
		public override void PostItemCheck() {
			if (player.HeldItem.IsAir || player.HeldItem.modItem == null || !player.HeldItem.GetDelta().Shortsword){Shortswordatt = 0;}
		}
		public override bool PreItemCheck() {
			if (soulTimer > 0) {return false;}
			return base.PreItemCheck();
		}
		public override void PreUpdate() {
			SoulHandler.Update(player,this);
		}
		public override void PostUpdate() {
			SoulHandler.PositionUpdate(player,this);

			if (sacredrock) {
				player.statDefense += (int)((float)player.statDefense * 0.3f);
			}
			if (TP > TPMax) {
				//snd_bell
				Main.PlaySound(Deltarune.GetSound("power"),player.Center);
				CombatText.NewText(player.getRect(),Color.Yellow,"Max TP");
				TP = TPMax;
			}
			//Clamp
			if (TP < 0) {TP = 0;}
			if (TP > TPMax) {TP = TPMax;}
		}
		public void Graze(Entity entity,int damage,bool Graze = true,int cooldown = 5,bool visual = true) {
			// 5 tick each graze
			TPCooldown = cooldown;
			//cannot be bigger than 1%
			if (damage > TPMax*0.01) {
				damage = (int)(TPMax*0.01);
			}
			if (entity is NPC npc) {
				if (npc.realLife > 0) {
					damage /= 2;
				}
			}
			Content.UI.tensionBar.color = Color.LightYellow;
			int num = damage/2;
			if (!Graze) {num = num/2;}
			TP += num+1;
			if (TP == TPMax) {TP += 1;}
			if (visual) {
				Main.PlaySound(Deltarune.GetSound("graze"),player.Center);
				for (int a = 0; a < Main.rand.Next(10,21); a++) {
					Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
					Dust d = Dust.NewDustPerfect(player.Center, 182, speed * Main.rand.NextFloat(2f,5f), Scale: 0.6f);
					d.noGravity = true;
				}
			}
			//Rectangle rec = TPBox();
			//Dust.QuickBox(new Vector2(rec.X,rec.Y),new Vector2(rec.Width+rec.X,rec.Height+rec.Y), 2, Color.Pink, null);
		}
		//Hooks
		public override void PostUpdateBuffs() {
			if (!player.immune) {
				if (Main.expertMode) {
					player.allDamage += TPDisplay/4f;
				}
				else {
					player.allDamage += TPDisplay/3f;
				}
			}
		}
		/*
		public override void PostUpdateEquips(){
			for (int i = 0; i < player.bank.item.Length; i++){
				Item item = player.bank.item[i];
			}
		}*/
		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit,ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource) {
			return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit,ref customDamage, ref playSound, ref genGore, ref  damageSource);
		}
		public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit) {
			player.CameraShake(2);
			Content.UI.tensionBar.shake = 3;
			Main.PlaySound(Deltarune.GetSound("hurt1"),player.Center);
			TPCooldown = 120;
			TP -= (int)damage*2;
			if (TP < 0) {TP = 0;}
		}
		public static readonly PlayerLayer MiscEffectsBack = new PlayerLayer("Deltarune", "MiscEffectsBack", PlayerLayer.MiscEffectsBack, delegate (PlayerDrawInfo drawInfo) {
			if (drawInfo.shadow != 0f) {return;}
			Player player = drawInfo.drawPlayer;
			Mod mod = Deltarune.get;
			var p = player.GetDelta();
			if (p.spellAnim > 0 && p.spellAnimTex != "") {
				p.spellAnim -= 3;
				Texture2D texture = ModContent.GetTexture(p.spellAnimTex);
				Vector2 pos = player.Center - Main.screenPosition;
				pos.Y -= p.spellAnim/2;
				var data = new DrawData(texture, pos, null, Color.White*((float)p.spellAnim/60f), 0f,texture.Size()/2f, (float)p.spellAnim/60f, SpriteEffects.None, 0);
				Main.playerDrawData.Add(data);
			}
			if (p.spellAnim < 0) {p.spellAnim = 0;}
		});
		/*
		public static readonly PlayerLayer MiscEffectsFront = new PlayerLayer("Deltarune", "MiscEffectsFront", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo drawInfo) {
			if (drawInfo.shadow != 0f) {return;}
			Player player = drawInfo.drawPlayer;
			Mod mod = Deltarune.get;
			var p = player.GetDelta();
		});
		*/

		public override void ModifyDrawLayers(List<PlayerLayer> layers) {
			MiscEffectsBack.visible = true;
			layers.Insert(0, MiscEffectsBack);
			//MiscEffectsFront.visible = true;
			//layers.Add(MiscEffectsFront);

			int itemLayer = layers.FindIndex(PlayerLayer => PlayerLayer.Name.Equals("HeldItem"));
            if (itemLayer != -1){
                ItemUseGlow.ItemUseGlowLayer.visible = true;
                layers.Insert(itemLayer + 1, ItemUseGlow.ItemUseGlowLayer);
            }
		}
		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
			if (soulTimer > 0) {
				SoulHandler.BreakSoul(soul);
				soulTimer = 0;
			}
			TP = 0;
			TPCooldown = 0;
			spellTimer = new int[3];
			Deltarune.deathTips = "Tips : "+Main.rand.Next(new string[] {
				"Get gud","Avoid getting hit","The more you death the more progress you had",
				"Do not give up yet","There is still hope","Believe in the power of terraria",
				"Use penetrating weapon for worm enemies","Only use health potion when in low hp",
				"Summoner is cringe","Use potion to get better at certain situation","Equip some mobility accesories to increase chance of dodging",
				"Imagine dying lol","Use dangersense potion to avoid traps","Use hunter potion to show location of enemies",
				"Health potion has cooldown so please use it carefully","Drinking mana potion causes mana sickness",
				"Store your coins in a chest before going out on a quest","Graze enemy and bullet to gain increased damage",
				"Talk to the guide for checking recipe","Jabibi Among us","Penetrating weapon can cause other weapon to not hit"});
		}
		void UseSpell(BaseSpell item,int type = -1) {
			if (type == -1) {
				bool flag2 = item.ModifyTPCost(player,type) <= TP ? true : item.MissingTP(player,type);
				if (flag2) {
					spellAnimTex = item.Texture;
					spellAnim = 60;
					TP -= item.ModifyTPCost(player,type);
					Content.UI.tensionBar.shake = 3;
					item.Spell(player,type);
					TPCooldown = 20;
				}
				else {
					Content.UI.tensionBar.shake = 3;
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_cantselect"),player.Center);
					TPCooldown += 1;
				}
				return;
			}
			bool flag1 = spellTimer[type] < 1 ? true : item.OnCooldown(player,type);
			if (flag1) {
				bool flag2 = item.ModifyTPCost(player,type) <= TP ? true : item.MissingTP(player,type);
				if (flag2) {
					spellAnimTex = item.Texture;
					spellAnim = 60;
					TP -= item.ModifyTPCost(player,type);
					Content.UI.tensionBar.shake = 3;
					item.Spell(player,type);
					TPCooldown = 20;
					spellTimer[type] = item.cooldown;
				}
				else {
					if ((type == 0 && Deltarune.KeyMagic1.JustPressed) ||
						(type == 1 && Deltarune.KeyMagic2.JustPressed) ||
						(type == 2 && Deltarune.KeyMagic3.JustPressed)) {
						//CombatText.NewText(player.getRect(),Color.Pink,"Not enough TP");
						Content.UI.tensionBar.shake = 3;
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_cantselect"),player.Center);
						TPCooldown += 1;
					}
				}
			}
		}
		public override void ProcessTriggers(TriggersSet triggersSet) {
			//JustPressed
			//Released
			if (player.GetDelta().soulTimer < 1) {
				if ((Deltarune.KeyMagic1.JustPressed || Deltarune.KeyMagic1.Current) && spellItem[0] != null) {
					BaseSpell item = spellItem[0];
					if (item.autoReuse) {if (Deltarune.KeyMagic1.Current) {UseSpell(item,0);}}
					else {if (Deltarune.KeyMagic1.JustPressed) {UseSpell(item,0);}}
				}
				if ((Deltarune.KeyMagic2.JustPressed || Deltarune.KeyMagic2.Current) && spellItem[1] != null) {
					BaseSpell item = spellItem[1];
					if (item.autoReuse) {if (Deltarune.KeyMagic2.Current) {UseSpell(item,1);}}
					else {if (Deltarune.KeyMagic2.JustPressed) {UseSpell(item,1);}}
				}
				if ((Deltarune.KeyMagic3.JustPressed || Deltarune.KeyMagic3.Current) && spellItem[2] != null) {
					BaseSpell item = spellItem[2];
					if (item.autoReuse) {if (Deltarune.KeyMagic3.Current) {UseSpell(item,2);}}
					else {if (Deltarune.KeyMagic3.JustPressed) {UseSpell(item,2);}}
				}
			}
			//if (Deltarune.KeyMagic2.JustPressed && spell[1] > 0) {UseSpell(spell[1],1);}
			//if (Deltarune.KeyMagic3.JustPressed && spell[2] > 0) {UseSpell(spell[2],2);}

			if (TP < 0) {TP = 0;}
		}
		//public override void OnHitByNPC(NPC npc, int damage, bool crit){}
		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit) {
			if (sacredrock && TP < TPMax && TPCooldown < 1) {
				Graze(target,damage/12,target.GetDelta().Graze,10,false);
			}
		}
		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit) {
			if (sacredrock && TP < TPMax && TPCooldown < 1) {
				Graze(target,damage/10,target.GetDelta().Graze,10,false);
				
			}
		}
		//public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack){return true;}
		//public override bool CanBuyItem(NPC vendor, Item[] shopInventory, Item item) {return base.CanBuyItem(vendor,shopInventory,item);}
		//public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit) {}

		//Sync
		public override void OnEnterWorld(Player client) {
			if (!Deltarune.HasMusic) {
				Main.NewText("Install Deltarune Music Mod For Morr Epicness",Color.LightGreen);
			}
			Mod mod = ModLoader.GetMod("CalamityMod");
			if (mod != null) {
				Main.NewText("Calamity detected, activating fard mode",Color.Pink);
			}

			mod = ModLoader.GetMod("ilovetheinternet");
			if (mod != null) {
				Main.NewText("I Love the internet",Color.Orange);
			}
			if (client.whoAmI == Main.myPlayer && !receiveBag) {
				receiveBag = true;
				if (Deltarune.playerClassMisc == 1) {
					player.QuickSpawnItem(ItemID.LesserHealingPotion,10);
					player.QuickSpawnItem(ItemID.Torch,10);
					player.QuickSpawnItem(ItemID.Wood,200);
					player.QuickSpawnItem(ItemID.Gel,20);
				}
				if (Deltarune.playerClassMisc == 2) {
					player.QuickSpawnItem(ItemID.GoldCoin);
					player.QuickSpawnItem(ItemID.LesserHealingPotion,30);
					player.QuickSpawnItem(ItemID.LesserManaPotion,10);
					player.QuickSpawnItem(ItemID.Torch,50);
					player.QuickSpawnItem(ItemID.Wood,400);
					player.QuickSpawnItem(ItemID.Gel,40);
					player.AddBuff(BuffID.Swiftness,60*60*3);
					player.AddBuff(BuffID.Ironskin,60*60*3);
					player.AddBuff(BuffID.Mining,60*60*3);
					player.QuickSpawnItem(ItemID.RecallPotion,10);
					player.QuickSpawnItem(ItemID.SlimeCrown,1);
				}
				//funk
				if (Deltarune.playerClassType < 1 && Deltarune.playerClass < 1) {return;}
				//replace player.inventory
				for (int i = 0; i < player.inventory.Length; i++){
					if (Deltarune.playerClassType > 0) {
						int[] pick = new int[] {ItemID.CopperPickaxe,ItemID.TinPickaxe,ItemID.IronPickaxe,ItemID.LeadPickaxe,ItemID.SilverPickaxe,ItemID.TungstenPickaxe};
						if (player.inventory[i].type == ItemID.CopperPickaxe) {player.inventory[i] = Helpme.ItemDefault(pick[Deltarune.playerClassType],1,true);}

						pick = new int[] {ItemID.CopperAxe,ItemID.TinAxe,ItemID.IronAxe,ItemID.LeadAxe,ItemID.SilverAxe,ItemID.TungstenAxe};
						if (player.inventory[i].type == ItemID.CopperAxe) {player.inventory[i] = Helpme.ItemDefault(pick[Deltarune.playerClassType],1,true);}
						if (Deltarune.playerClassType < 1) {
							pick = new int[] {ItemID.CopperShortsword,ItemID.TinShortsword,ItemID.IronShortsword,ItemID.LeadShortsword,ItemID.SilverShortsword,ItemID.TungstenShortsword};
							if (player.inventory[i].type == ItemID.CopperShortsword) {player.inventory[i] = Helpme.ItemDefault(pick[Deltarune.playerClassType],1,true);}
						}
					}
					if (Deltarune.playerClass > 0 && player.inventory[i].type == ItemID.CopperShortsword) {
						int type = ItemID.CopperShortsword;
						if (Deltarune.playerClass == 1) {
							int[] pick = new int[] {ItemID.CopperBroadsword,ItemID.TinBroadsword,ItemID.IronBroadsword,ItemID.LeadBroadsword,ItemID.SilverBroadsword,ItemID.TungstenBroadsword};
							type = pick[Deltarune.playerClassType];
						}
						if (Deltarune.playerClass == 2) {
							int[] pick = new int[] {ItemID.CopperBow,ItemID.TinBow,ItemID.IronBow,ItemID.LeadBow,ItemID.SilverBow,ItemID.TungstenBow};
							type = pick[Deltarune.playerClassType];
							player.QuickSpawnItem(ItemID.WoodenArrow,200);
						}
						if (Deltarune.playerClass == 3) {
							int[] pick = new int[] {ItemID.AmethystStaff,ItemID.TopazStaff,ItemID.SapphireStaff,ItemID.EmeraldStaff,ItemID.RubyStaff,ItemID.DiamondStaff};
							type = pick[Deltarune.playerClassType];
							player.QuickSpawnItem(ItemID.ManaCrystal,3);
						}
						if (Deltarune.playerClass == 4) {type = ItemID.SlimeStaff;}
						player.inventory[i] = Helpme.ItemDefault(type,1,true);
					}
				}
			}
			
		}
		public void HandlePacket(BinaryReader reader) {
			//read
			TP = reader.ReadInt32();
			TPCooldown = reader.ReadInt32();
			//spell
			for (int i = 0; i < spell.Length; i++){spell[i] = reader.ReadInt32();}
			for (int i = 0; i < spellTimer.Length; i++){spellTimer[i] = reader.ReadInt32();}
		}
		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
			ModPacket packet = mod.GetPacket();
			packet.Write((byte)NetType.Player);
			packet.Write((byte)player.whoAmI);
			//write
			packet.Write(TP);
			packet.Write(TPCooldown);
			//spell
			for (int i = 0; i < spell.Length; i++){packet.Write(spell[i]);}
			for (int i = 0; i < spellTimer.Length; i++){packet.Write(spellTimer[i]);}

			packet.Send(toWho, fromWho);
		}

		//Save and Load
		public override TagCompound Save() {
			TagCompound tag = new TagCompound();
			tag.Add("TP",TP);
			tag.Add("receiveBag",receiveBag);
			tag.Add("dialogTag",dialogTag);
			return tag;
		}
		public override void Load(TagCompound tag) {
			receiveBag = tag.GetBool("receiveBag");
			TP = tag.GetInt("TP");
			dialogTag = tag.GetList<string>("dialogTag").ToList();
			//CrystalClass = tag.GetInt("CrystalClass");
			//DeathAmount = tag.GetInt("DeathAmount");
			//test = tag.GetFloat("test");
			//nonStopParty = tag.GetBool("nonStopParty");
		}

		#region Camera
		
		public Vector2 cameraFocus;
		public Vector2 cameraFocusCache;
		public int cameraFocusTimer = -1;
		public int cameraShake;
		public int cameraShakeForced;
		public int cameraShakeForcedTimer;

		public void StopCamera() {
			cameraFocusTimer = -1;
		}
		public void CameraFocus(Vector2 pos,int time) {
			cameraFocus = pos;
			cameraFocusTimer = time;
		}

		public override void ModifyScreenPosition() {
			Vector2 centerScreen = new Vector2(Main.screenWidth/2,Main.screenHeight/2);
			SoulHandler.Camera(this);
			if (cameraFocusTimer > -1) {
				if (cameraFocusTimer == 0) {
					//Main.NewText(Vector2.Distance(cameraFocusCache,Main.screenPosition));
					if (Vector2.Distance(cameraFocusCache,Main.screenPosition) <= 10f) {
						cameraFocusTimer = -1;
					}
					cameraFocusCache = Vector2.Lerp(cameraFocusCache,Main.screenPosition,0.4f);
					Main.screenPosition = cameraFocusCache;
				}
				if (cameraFocusTimer > 0) {
					cameraFocusTimer--;
					cameraFocusCache = Vector2.Lerp(cameraFocusCache,cameraFocus - centerScreen,0.1f);
					Main.screenPosition = cameraFocusCache;
				}
			}
			else {cameraFocusCache = Main.screenPosition;}
			if (MyConfig.get.CameraShake) {
				if (cameraShake > 0)
				{
					Main.screenPosition += new Vector2(Main.rand.Next(-cameraShake, cameraShake + 1), Main.rand.Next(-cameraShake, cameraShake + 1));
					cameraShake -= 1;
				}
				if (cameraShakeForcedTimer > 0){
					Main.screenPosition += new Vector2(Main.rand.Next(-cameraShakeForced, cameraShakeForced + 1), Main.rand.Next(-cameraShakeForced, cameraShakeForced + 1));
					cameraShakeForcedTimer -= 1;
				}
				else if (cameraShakeForced > 0) {
					cameraShake += cameraShakeForced;
					cameraShakeForced = 0;
				}
			}
		}
		#endregion
	}
}
