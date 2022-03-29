using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.UI.Chat;
using Terraria.UI;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.ModLoader.Config;
using Deltarune.Helper;
using Deltarune;

namespace Deltarune.Content.NPCs.RalseiDummy
{
	public abstract class DummyAttachment : ModItem
	{
		public virtual string NewName => "None";
		public virtual string TT => "None";
		public virtual int type => 0;
		public override void SetStaticDefaults() {
			DisplayName.SetDefault(NewName);
			Tooltip.SetDefault($"{TT}\nRight Click on this item to equip it onto [i:{ModContent.ItemType<ralseidummyItem>()}] Ralsei Dummy\nDummy Attachment");
		}
		public override void SetDefaults() 
		{
            item.width = 14;
            item.height = 14;
            item.rare = 1;
		}
		public override bool CanRightClick() => true;
		public override bool ConsumeItem(Player player) => false;
        public override void RightClick(Player player){
			foreach (var i in player.inventory)
			{
				if (i != null) {
					if (i.type == ModContent.ItemType<ralseidummyItem>()) {
						if (i.modItem is ralseidummyItem s) {
							for (int a = 0; a < s.dummyItem.Length; a++)
							{
								if (s.dummyItem[a] < 1) {
									s.blink = 30f;
									s.dummyItem[a] = type;
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_equip"),player.Center);
									item.TurnToAir();
									return;
								}
							}
						}
					}
				}
			}
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_cantselect"),player.Center);
			CombatText.NewText(player.getRect(),Color.Pink,"No Empty Ralsei Dummy");
		}
		public float blink;
		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,Color itemColor, Vector2 origin, float scale) {
			if (blink > 0) {
				blink -= 1f;
				Texture2D texture2 = ModContent.GetTexture(Texture+"_glow");
				spriteBatch.Draw(texture2, position, frame, Color.White*(blink/30f), 0f, origin, scale, SpriteEffects.None, 0f);
			}
		}
	}
	public class RalseiShield1 : DummyAttachment
	{
		public override string NewName => "Dummy Shield Tier I";
		public override string TT => "Increases Ralsei Dummy's defense by 2";
		public override int type => 1;
	}
	public class RalseiShield2 : DummyAttachment
	{
		public override string NewName => "Dummy Shield Tier II";
		public override string TT => "Increases Ralsei Dummy's defense by 5";
		public override int type => 2;
	}
	public class RalseiShield3 : DummyAttachment
	{
		public override string NewName => "Dummy Shield Tier III";
		public override string TT => "Increases Ralsei Dummy's defense by 15";
		public override int type => 3;
	}
	public class RalseiFighterAI : DummyAttachment
	{
		public override string NewName => "Dummy Zombie AI";
		public override string TT => "When equipped, this will make the dummy use the Zombie AI";
		public override int type => 4;
	}//RalseiSlimeAI
	public class RalseiSlimeAI : DummyAttachment
	{
		public override string NewName => "Dummy Slime AI";
		public override string TT => "When equipped, this will make the dummy use the Slime AI";
		public override int type => 5;
		//ModContent.ItemType<ralseidummyItem>()
	}
	public class RalseiDamage1 : DummyAttachment
	{
		public override string NewName => "Dummy Damaging Tier I";
		public override string TT => "Increases Ralsei Dummy's damage by 10\nThis will damage you on contact with the Dummy!";
		public override int type => 6;
	}
	public class RalseiDamage2 : DummyAttachment
	{
		public override string NewName => "Dummy Damaging Tier II";
		public override string TT => "Increases Ralsei Dummy's damage by 50\nThis will damage you on contact with the Dummy!";
		public override int type => 7;
	}
	public class RalseiDamage3 : DummyAttachment
	{
		public override string NewName => "Dummy Damaging Tier III";
		public override string TT => "Increases Ralsei Dummy's damage by 100\nThis will damage you on contact with the Dummy!";
		public override int type => 8;
	}
	public class RalseiShieldX : DummyAttachment
	{
		public override string NewName => "Dummy Shield Tier X";
		public override string TT => "Increases Ralsei Dummy's defense by 100";
		public override int type => 9;
	}//RalseiEyeAI
	public class RalseiEyeAI : DummyAttachment
	{
		public override string NewName => "Dummy Eye AI";
		public override string TT => "When equipped, this will make the dummy use the Demon Eye AI";
		public override int type => 10;
	}
	public class RalseiKB3 : DummyAttachment
	{
		public override string NewName => "Dummy Knockback III";
		public override string TT => "Increases Ralsei Dummy's knockback taken by 100%";
		public override int type => 11;
	}
	public class RalseiKB2 : DummyAttachment
	{
		public override string NewName => "Dummy Knockback II";
		public override string TT => "Increases Ralsei Dummy's knockback taken by 50%";
		public override int type => 12;
	}
	public class RalseiKB1 : DummyAttachment
	{
		public override string NewName => "Dummy Knockback I";
		public override string TT => "Increases Ralsei Dummy's knockback taken by 25%";
		public override int type => 13;
	}

	public class ralseidummyItem : ModItem
	{
		//public override string Texture => "Deltarune/Content/NPCs/ralseidummy";
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Ralsei Dummy");
			Tooltip.SetDefault("Right Click in hand to explode all Dummies\n"+
				"Right Click in Inventory to un-equip any dummy attachments\n"+
				"Any active dummies will not be affected if any changes are made to the active upgrades\n"+
				"If changes are made to the upgrades, respawn the dummy to enable the changes");
		}
		public override void SetDefaults() {
			item.useAnimation = 20;
			item.useTime = 20;
			item.useStyle = 1;
			item.width = 10;
			item.height = 10;
			item.rare = 1;
		}
		public override ModItem Clone(Item itemClone) {
			ralseidummyItem myClone = (ralseidummyItem)base.Clone(itemClone);
			myClone.dummyItem = dummyItem;
			myClone.dummyCount = dummyCount;
			return myClone;
		}
		int dummyCount;
		public float blink;
		public int[] dummyItem = new int[5];
		public override void UpdateInventory(Player player) {
			dummyCount = 0;
			for (int i = 0; i < Main.maxNPCs; i++){
                NPC n = Main.npc[i];
                if (n.active && n.type == ModContent.NPCType<ralseidummy>()){dummyCount++;}
			}
		}
		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,Color itemColor, Vector2 origin, float scale) {
			if (blink > 0) {
				blink -= 1f;
				Texture2D texture2 = ModContent.GetTexture(Texture+"_glow");
				spriteBatch.Draw(texture2, position, frame, Color.White*(blink/30f), 0f, origin, scale, SpriteEffects.None, 0f);
			}
			if (Deltarune.Boss || dummyCount >= 20) {
				Texture2D texture2 = ModContent.GetTexture(Texture+"_no");
				spriteBatch.Draw(texture2, position, frame, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
			}
		}
		public override bool AltFunctionUse(Player player) => true;
		public override bool CanUseItem(Player player) {
			if (player.altFunctionUse == 2) {
				if (Main.netMode == NetmodeID.SinglePlayer) {
					for (int i = 0; i < Main.maxNPCs; i++){
						if (Main.npc[i].active && Main.npc[i].modNPC != null && Main.npc[i].modNPC is ralseidummy ralsei){
							ralsei.CanDie = true;
						}
					}
				}
				else if (Main.netMode == NetmodeID.MultiplayerClient){NetCode.Send(NetType.KillRalsei);}
				return true;
			}
			else if (!Deltarune.Boss && dummyCount < 20){
				int a = NPC.NewNPC((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, ModContent.NPCType<ralseidummy>());
				Main.npc[a].spriteDirection = (player.Center.X < Main.MouseWorld.X ? -1 : 1);
				int result = 0;
				int damage = 0;
				for (int i = 0; i < dummyItem.Length; i++)
				{
					if (dummyItem[i] == 1) {result += 2;}
					else if (dummyItem[i] == 2) {result += 5;}
					else if (dummyItem[i] == 3) {result += 15;}
					else if (dummyItem[i] == 4) {Main.npc[a].aiStyle = 3;}
					else if (dummyItem[i] == 5) {Main.npc[a].aiStyle = 1;}
					else if (dummyItem[i] == 6) {damage += 10;}
					else if (dummyItem[i] == 7) {damage += 50;}
					else if (dummyItem[i] == 8) {damage += 100;}
					else if (dummyItem[i] == 9) {result += 100;}
					else if (dummyItem[i] == 10) {Main.npc[a].aiStyle = 2;}
					else if (dummyItem[i] == 11) {Main.npc[a].knockBackResist += 1f;}
					else if (dummyItem[i] == 12) {Main.npc[a].knockBackResist += 0.5f;}
					else if (dummyItem[i] == 13) {Main.npc[a].knockBackResist += 0.25f;}
				}
				if (damage > 0) {Main.npc[a].damage = damage;}
				Main.npc[a].defense = result;
				return true;
			}
			return false;
		}
		public override bool CanRightClick() => true;
		public override bool ConsumeItem(Player player) => false;
		public override void RightClick(Player player){
			for (int i = 0; i < dummyItem.Length; i++)
			{
				if (dummyItem[i] > 0) {
					int type = GetType(dummyItem[i]);
					Item dummy = new Item();
					dummy.SetDefaults(type);
					if (dummy.modItem is DummyAttachment thing) {
						thing.blink = 30f;
						player.QuickSpawnClonedItem(dummy);
						dummyItem[i] = 0;
					}
				}
			}
		}
		int GetType(int num1) {
			if (num1 == 1) {return ModContent.ItemType<RalseiShield1>();}
			if (num1 == 2) {return ModContent.ItemType<RalseiShield2>();}
			if (num1 == 3) {return ModContent.ItemType<RalseiShield3>();}
			if (num1 == 4) {return ModContent.ItemType<RalseiFighterAI>();}//RalseiSlimeAI
			if (num1 == 5) {return ModContent.ItemType<RalseiSlimeAI>();}
			if (num1 == 6) {return ModContent.ItemType<RalseiDamage1>();}
			if (num1 == 7) {return ModContent.ItemType<RalseiDamage2>();}
			if (num1 == 8) {return ModContent.ItemType<RalseiDamage3>();}//RalseiShieldX
			if (num1 == 9) {return ModContent.ItemType<RalseiShieldX>();}//RalseiEyeAI
			if (num1 == 10) {return ModContent.ItemType<RalseiEyeAI>();}
			if (num1 == 11) {return ModContent.ItemType<RalseiKB3>();}
			if (num1 == 12) {return ModContent.ItemType<RalseiKB2>();}
			if (num1 == 13) {return ModContent.ItemType<RalseiKB1>();}
			return ModContent.ItemType<Removed>();
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			int defense = 0;
			int damage = 0;
			for (int i = 0; i < dummyItem.Length; i++)
			{
				if (dummyItem[i] == 1) {defense += 2;}
				else if (dummyItem[i] == 2) {defense += 5;}
				else if (dummyItem[i] == 3) {defense += 15;}
				else if (dummyItem[i] == 6) {damage += 10;}
				else if (dummyItem[i] == 7) {damage += 50;}
				else if (dummyItem[i] == 8) {damage += 100;}
				else if (dummyItem[i] == 9) {defense += 100;}
			}
			if (defense > 0) {
				tooltips.Insert(1,new TooltipLine(mod, "defense", $"{defense} defense"));
			}
			if (damage > 0) {
				tooltips.Insert(1,new TooltipLine(mod, "damage", $"{damage} damage"));
			}
			tooltips.Insert(1,new TooltipLine(mod, "dumm", $"{dummyCount} / 20 dummy slot"));
			string text = $"Attachment : ";
			for (int i = 0; i < dummyItem.Length; i++)
			{
				int num = GetType(dummyItem[i]);
				text += $"[i:{num}]";
			}
			tooltips.Add(new TooltipLine(mod, "based", text));
			tooltips.Add(new TooltipLine(mod, "num", "'fluffy'"));
		}
		public override TagCompound Save() {
			TagCompound tag = new TagCompound();
			tag.AddIntArray(dummyItem,"dummy");
			return tag;
		}
		public override void Load(TagCompound tag) {
			dummyItem = tag.GetIntArray(dummyItem.Length,"dummy");
		}
		public override void NetSend(BinaryWriter writer) {
			writer.WriteArrayInt32(dummyItem);
		}
		public override void NetRecieve(BinaryReader reader) {
			dummyItem = reader.ReadArrayInt32();
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.TargetDummy);
			recipe.AddIngredient(ModContent.ItemType<Content.Spell.Spell_>());
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class ralseidummy : ModNPC
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Ralsei Dummy");
		}
		public override void SetDefaults() {
			npc.width = 54;
			npc.height = 90;
			npc.aiStyle = -1;
			npc.damage = 0;
			npc.value = 0;
			npc.lifeMax = int.MaxValue;
			npc.friendly = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			//npc.immortal = true;
			npc.knockBackResist = 0f;
		}
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;
		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) {
			Texture2D GlowTexture = ModContent.GetTexture(Texture+"_glow");
			Texture2D tt = ModContent.GetTexture(Texture);
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			spriteBatch.Draw(tt, npc.Center - Main.screenPosition, null, npc.GetAlpha(drawColor), npc.rotation, tt.Size() * 0.5f, npc.scale, spriteEffects, 0f);
			spriteBatch.Draw(GlowTexture, npc.Center - Main.screenPosition, null, npc.color, npc.rotation, GlowTexture.Size() * 0.5f, npc.scale, spriteEffects, 0f);
			//use ui scale
			spriteBatch.BeginNormal(true , true);
			float num = damage;
			if (num == 0f) {
				num = damageOld;
			}
			string word = "";
			if (num > 999) {
				num /= 1000;
				num = (float)Math.Round(num);
				word = "K";
			}
			Vector2 messageSize = ChatManager.GetStringSize(Deltarune.tobyFont, $"{kb} KB\n{num}{word} DPS\n{npc.defense} DEF", Vector2.One);
			Vector2 pos = npc.Center - Main.screenPosition + new Vector2(0,60);
			pos.X -= messageSize.X/2f;
			Rectangle rec = new Rectangle((int)pos.X - 4,(int)pos.Y - 4,(int)messageSize.X + 8,(int)messageSize.Y + 8);
			spriteBatch.Draw(ModContent.GetTexture(Deltarune.textureExtra+"Chat_Medium"), rec, Color.White*0.5f);
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Deltarune.tobyFont, $"[c/d2cdfd:{kb}] KB", pos, Color.White, 0, Vector2.Zero, Vector2.One);
			pos.Y += messageSize.Y/3f;//d2cdfd
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Deltarune.tobyFont, $"[c/ffe5e5:{num}{word}] DPS", pos, Color.White, 0, Vector2.Zero, Vector2.One);
			pos.Y += messageSize.Y/3f;//d2cdfd
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Deltarune.tobyFont, $"{npc.defense} DEF", pos, Color.White, 0, Vector2.Zero, Vector2.One);
			if (MyConfig.get.ralseidummySource) {
				messageSize = ChatManager.GetStringSize(Deltarune.tobyFont, source, Vector2.One);
				pos = npc.Center - Main.screenPosition + new Vector2(0,140);
				pos.X -= messageSize.X/2f;
				rec = new Rectangle((int)pos.X,(int)pos.Y,(int)messageSize.X,(int)messageSize.Y);
				spriteBatch.Draw(ModContent.GetTexture(Deltarune.textureExtra+"Chat_Back"), rec, Color.White*0.5f);
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Deltarune.tobyFont, source, pos, Color.White, 0, Vector2.Zero,Vector2.One);
			}
			spriteBatch.BeginNormal(true);
			return false;
		}
		public override bool CheckDead() {
			npc.life = npc.lifeMax;
			return CanDie;
		}
		public override void SendExtraAI(BinaryWriter writer) {
			writer.Write(damage);
			writer.Write(damageOld);
			writer.Write(kb);
			writer.Write(timer);
			writer.Write(CanDie);
		}
		public override void ReceiveExtraAI(BinaryReader reader) {
			damage = reader.ReadSingle();
			damageOld = reader.ReadSingle();
			kb = reader.ReadSingle();
			timer = reader.ReadSingle();
			CanDie = reader.ReadBoolean();
		}
		string source = "Nothing";
		float damage;
		float damageOld;
		float kb;
		float timer;
		public bool CanDie;
		public override void AI() {
			if (npc.aiStyle == -1) {
				npc.velocity.X = MathHelper.Lerp(npc.velocity.X,0f,0.1f);
			}
			if (CanDie) {
				Explode.New(npc.Center,0.5f);
				npc.life = 0;
				npc.HitEffect();
				npc.checkDead();
			}
			if (Deltarune.Boss) {CanDie = true;}
			else {
				npc.dontTakeDamage = false;
				npc.friendly = false;
				if (npc.alpha > 0) {npc.alpha -= 5;}
				if (npc.alpha < 0) {npc.alpha = 0;}
			}
			timer++;
			if (timer > 60f) {
				damageOld = damage;
				kb = 0f;
				damage = 0f;
				timer = 0f;
			}
			if (damageOld == 0f && damage == 0f) {
				source = "Nothing";
			}
			if (npc.aiStyle != -1) {
				npc.TargetClosest(true);
				npc.spriteDirection = npc.direction;
			}
			npc.life = npc.lifeMax;
			npc.color = Color.Lerp(npc.color,Color.White,0.1f);
		}
		public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
			if (target.statLife > npc.damage) {
				return true;
			}
			return false;
		}
		public override bool? CanHitNPC(NPC target) {
			return false;
		}
		public override void OnHitByProjectile(Projectile projectile, int dmg, float knockback, bool crit) {
			source = projectile.Name;
		}
		public override void OnHitByItem(Player player, Item item, int dmg, float knockback, bool crit) {
			source = $"[i:{item.type}] {item.HoverName}";
		}
		public override void HitEffect(int hitDirection, double dmg) {
			npc.color = Color.Red;
			damage += (float)dmg;
		}
		public override bool StrikeNPC(ref double dmg, int defense, ref float knockback, int hitDirection, ref bool crit) {
			kb += knockback;
			npc.life = npc.lifeMax;
			return true;
		}
	}
}

