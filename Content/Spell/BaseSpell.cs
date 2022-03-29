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

namespace Deltarune.Content.Spell
{
	//the spell bag
	public class SpellBag : ModItem
	{
		public override ModItem Clone(Item itemClone) {
			SpellBag myClone = (SpellBag)base.Clone(itemClone);
			myClone.spell = spell;
			myClone.spellPrefix = spellPrefix;
			return myClone;
		}
		//the white overlay
		public float blink;
		// the item spell type
		public int[] spell = new int[3];
		// the prefix type
		public int[] spellPrefix = new int[3];

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Spell Bag");
			Tooltip.SetDefault($"Allows you to use spell when equiped\n<right> on a [i:{ModContent.ItemType<Spell_>()}] spell to equip it here\nShift and <right> to remove all equiped [i:{ModContent.ItemType<Spell_>()}] spell");
		}
		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,Color itemColor, Vector2 origin, float scale) {
			int b = 0;
			foreach (var a in spell){if (a > 0) {b += 1;}}
			Texture2D texture2 = ModContent.GetTexture(Texture+"_full"+b);
			spriteBatch.Draw(texture2, position, frame, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
			if (blink > 0) {
				blink -= 1f;
				texture2 = ModContent.GetTexture(Texture+"_white");
				spriteBatch.Draw(texture2, position, frame, Color.White*(blink/30f), 0f, origin, scale, SpriteEffects.None, 0f);
			}
		}
		public override void SetDefaults() {
			item.width = 20;
			item.height = 20;
			item.value = 10000;
			item.rare = ItemRarityID.Blue;
			item.accessory = true;
		}
		public override bool CanRightClick() => Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift);
		public override bool ConsumeItem(Player player) => false;
		public override void UpdateAccessory(Player player, bool hideVisual) {
			var p = player.GetDelta();
			p.spell = spell;
			for (int i = 0; i < spellPrefix.Length; i++)
			{
				int type = spellPrefix[i];
				if (type > 0) {
					SpellPrefix.Effect(type ,player);
				}
			}
		}
        public override void RightClick(Player player){
			for (int i = 0; i < spell.Length; i++)
			{
				if (spell[i] > 0) {
					Item dummy = new Item();
					dummy.SetDefaults(spell[i]);
					if (dummy.modItem is BaseSpell thing) {
						thing.spellPrefix = spellPrefix[i];
						player.QuickSpawnClonedItem(dummy);
						spellPrefix[i] = 0;
						spell[i] = 0;
					}
				}
			}
		}
		public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset) {
			if (line.Name.Contains("Invenb")) {
				yOffset += 4;
				Color color = Color.Red;
				string p = line.Name.Replace("Invenb","");
				int num2 = int.Parse(p);
				int num = spell[num2];
				if (num > 0) {
					Item i = new Item();
					i.SetDefaults(num);
					num = spellPrefix[num2];
					if (num > 0) {
						if (i.rare != -12) {
							SpellPrefix.Names(num,out string name, out string tooltip, out bool bad);
							i.rare += bad ? -1 : 1;
							//clamp rarity
							if (i.rare > 11) {i.rare = 11;}
							if (i.rare < -1) {i.rare = -1;}
						}
					}
					color = Helpme.rarityToColor(i.rare);
				}

				TextSnippet[] snippets = ChatManager.ParseMessage(line.text, Color.White).ToArray();
				Vector2 messageSize = ChatManager.GetStringSize(Main.fontMouseText, snippets, Vector2.One);
				Utils.DrawInvBG(Main.spriteBatch, new Rectangle(
					line.X,
					line.Y -2,
					(int)messageSize.X + 27,
					(int)messageSize.Y + 2),color*0.4f);
				Utils.DrawInvBG(Main.spriteBatch, new Rectangle(
					line.X - 7,
					line.Y - 4,
					34,
					(int)messageSize.Y + 5));
			}
			return true;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			Player player = Main.LocalPlayer;
			var flag = Deltarune.KeyMagic1.GetAssignedKeys();
			var flag2 = Deltarune.KeyMagic2.GetAssignedKeys();
			var flag3 = Deltarune.KeyMagic3.GetAssignedKeys();
			if (flag.Count == 0 || flag2.Count == 0 || flag3.Count == 0) {
				tooltips.Add(new TooltipLine(mod, "hotkey1", "Please assign spell hotkey at Settings/Controls"){overrideColor = Color.Red});
			}


			for (int b = 0; b < spell.Length; b++)
			{
				string flag4 = "None";
				if (b == 0 && flag.Count != 0) {flag4 = flag[0];}
				if (b == 1 && flag2.Count != 0) {flag4 = flag2[0];}
				if (b == 2 && flag3.Count != 0) {flag4 = flag3[0];}
				var a = spell[b];
				string text = "Error";
				Color color = Color.Red;
				if (a < 1) {
					text = $"[i:{ModContent.ItemType<Removed>()}]  [c/{Color.White.Hex3()}:( {flag4} )] No Spell";
				}
				else {
					Item i = new Item();
					i.SetDefaults(a);
					string name = "";
					if (spellPrefix[b] > SpellPrefix.Max) {spellPrefix[b] = 0;}
					if (spellPrefix[b] > 0) {
						SpellPrefix.Names(spellPrefix[b],out string bb, out string tooltip, out bool bad);
						if (i.rare != -12) {
							i.rare += bad ? -1 : 1;
							if (i.rare > 11) {i.rare = 11;}
							if (i.rare < -1) {i.rare = -1;}
						}
						name = bb+" ";
					}
					color = Helpme.rarityToColor(i.rare);
					if (i.modItem is BaseSpell fart) {
						text = $"[i:{a}]  [c/{Color.White.Hex3()}:( {flag4} )] "+name+fart.name+$" Spell";
					}
				}
				tooltips.Add(new TooltipLine(mod, "Invenb"+b, text){overrideColor = color});
			}
		}
		public override TagCompound Save(){
            TagCompound tag = new TagCompound();
			for (int i = 0; i < spell.Length; i++)
			{
				if (spell[i] > 0) {
					Item ilem = new Item();
					ilem.SetDefaults(spell[i]);
					tag.Add("spell"+i,ilem.modItem.Name);
				}
				else {tag.Add("spell"+i,"None");}
				tag.Add("spellPrefix"+i,spellPrefix[i]);
			}
            return tag;
        }
        public override void Load(TagCompound tag){
			for (int i = 0; i < spell.Length; i++)
			{
				string spellName = tag.GetString("spell"+i);
				if (spellName != "None") {
					spell[i] = mod.ItemType(tag.GetString("spell"+i));
				}
				else {spell[i] = 0;}
				spellPrefix[i] = tag.GetInt("spellPrefix"+i);
			}
        }
		public override void NetSend(BinaryWriter writer) {
			for (int i = 0; i < spell.Length; i++)
			{
				writer.Write(spell[i]);	
				writer.Write(spellPrefix[i]);	
			}
		}

		public override void NetRecieve(BinaryReader reader) {
			for (int i = 0; i < spell.Length; i++)
			{
				spell[i] = reader.ReadInt32();
				spellPrefix[i] = reader.ReadInt32();
			}
		}
		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.Leather,10);
			recipe.AddIngredient(ItemID.IronBar, 10);
			recipe.AddIngredient(ItemID.FallenStar, 2);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
        }
	}
	//abstract class for spell, see spell.cs for all the spell
	//this has tons of comments lol
	public abstract class BaseSpell : ModItem
	{
		/// <summary>
		/// The new texture path.
		/// </summary>
		public virtual string Icon => "";

		/// <summary>
		/// The Display Name of the item.
		/// </summary>
		public virtual string name => Name;

		/// <summary>
		/// The tooltip of the item.
		/// </summary>
		public string tip = "";

		/// <summary>
		/// The prefix of this spell.
		/// </summary>
		public int spellPrefix;

		/// <summary>
		/// new autoReuse bc omniswing sucks
		/// </summary>
		public bool autoReuse;

		/// <summary>
		/// the tension point cost
		/// </summary>
		public float TPCost = 0f;

		/// <summary>
		/// the cooldown of the item.
		/// </summary>
		public int cooldown = 0;
		
		/// <summary>
		/// Modify The tension point cost.
		/// <param name="player">The player.</param>
		/// <param name="type">The slot of the spell bag.</param>
		/// </summary>
		public virtual int ModifyTPCost(Player player, int type) {
			if (TPCost == 1) {
				return player.GetDelta().TPMax;
			}
			return (int)(player.GetDelta().TPMax*TPCost);
		}

		/// <summary>
		/// Called before player equip spell in spell bag.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="bag">The spell bag.</param>
		/// <param name="type">The slot of the spell bag.</param>
		/// <returns>Whether or not to equip bag in spell bag</returns>
		public virtual bool StoreInBag(Player player, SpellBag bag, int type) {
			return true;
		}

		/// <summary>
		/// Called when player doesn't have enough tp to use the spell, return true to use the spell anyway.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="type">The slot of the spell bag.</param>
		/// <returns>Whether or not player can cast this spell</returns>
		public virtual bool MissingTP(Player player, int type) {
			return false;
		}

		/// <summary>
		/// Called when player cast the spell when still on cooldown, return true to ignore cooldown.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="type">The slot of the spell bag.</param>
		/// <returns>Whether or not player can cast this spell</returns>
		public virtual bool OnCooldown(Player player, int type) {
			return false;
		}

		/// <summary>
		/// Called when player cast the spell (Spawn a proj or something)
		/// always remember to check Main.netMode for client side stuff like spawning proj
		/// and also the Y position of the spell is : player.Center.Y - 60
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="type">The slot of the spell bag.</param>
		public virtual void Spell(Player player,int type) {

		}

		/// <summary>
		/// New set defaults
		/// </summary>
		public virtual void NewDefault() {
			
		}

		/// <summary>
		/// Modify the spell type to be something else, i'm too lazy to hardcode deez
		/// <param name="type">The type of the spell (Basic, attack,healing).</param>
		/// <param name="color">The type color of the spell (yellow for basic,red for attack ...).</param>
		/// </summary>
		public virtual void ModifySpellType(ref string type, ref Color color) {
		}

		/// <summary>
		/// Called in inventory and postupdate , Override to change how spellPrefix applied.
		/// </summary>
		public virtual void Prefix() {
			if (spellPrefix > SpellPrefix.Max) {spellPrefix = 0;}
			
			if (spellPrefix == 0) {
				spellPrefix = -1;
				if (Main.rand.NextBool(10)) {
					spellPrefix = Main.rand.Next(1,SpellPrefix.Max+1);
				}
			}
			if (spellPrefix > 0) {
				if (item.rare != -12) {

					SpellPrefix.Names(spellPrefix,out string name, out string tooltip, out bool bad);

					item.rare += bad ? -1 : 1;

					//clamp rarity
					if (item.rare > 11) {item.rare = 11;}
					if (item.rare < -1) {item.rare = -1;}
				}
			}
		}

		public override ModItem Clone(Item itemClone) {
			BaseSpell myClone = (BaseSpell)base.Clone(itemClone);
			myClone.spellPrefix = spellPrefix;
			myClone.tip = tip;
			myClone.TPCost = TPCost;
			myClone.autoReuse = autoReuse;
			myClone.cooldown = cooldown;
			return myClone;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			string text = "Basic Spell";
			Color color = Color.Yellow;
			if (Icon == "dead") {
				text = "Debug Spell";
				color = Main.DiscoColor;
			}
			if (Icon.Contains("attack")) {
				color = Color.Red;
				color.R -= 5;
				text = "Attack Spell";
			}
			if (Icon == "life" || Icon == "mana" || Icon == "nurse") {
				text = "Recovery Spell";
				color = new Color(52,207,99);
			}
			if (autoReuse){tooltips.Insert(1,new TooltipLine(mod, "ligmaballs", "Auto Cast"){overrideColor = Color.White});}
			ModifySpellType(ref text, ref color);
			tooltips.Add(new TooltipLine(mod, "tt3", text){overrideColor = color});
			tooltips.Insert(1,new TooltipLine(mod, "tt2", $"Right Click to equip on a [i:{ModContent.ItemType<SpellBag>()}] spell bag"){overrideColor = Color.White});
			text = "Very Fast Cast Speed";
			if (cooldown < 10)  {
				text = "Insanely Fast Cast Speed";
			}
			if (cooldown > 30) {
				text = "Fast Cast Speed";
			}
			if (cooldown > 60) {
				text = "Slow Cast Speed";
			}
			if (cooldown > 120) {
				text = "Very Slow Cast Speed";
			}
			if (cooldown > 240) {
				text = $"{Math.Round((double)cooldown/60),1}s Cast Cooldown";
			}
			tooltips.Insert(1,new TooltipLine(mod, "tt1", (TPCost == 0f ? "Does not uses TP" : $"Uses {(TPCost*100f)}% TP") + $"\n{text}\n"+tip));
			if (spellPrefix > 0) {
				SpellPrefix.Names(spellPrefix,out string name, out string tooltip, out bool bad);
				TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.mod == "Terraria");
				if (tt != null) {
					string a = tt.text;
					tt.text = name+" "+a;
				}
				tooltips.Add(new TooltipLine(mod, "spellPrefix", tooltip){overrideColor = (bad ? Color.Pink : Color.LightGreen)});
			}
		}


		public override string Texture => "Deltarune/Content/Spell/Spell_"+Icon;

		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault(name+" Spell");
		}
		public override void SetDefaults() 
		{
            item.width = 14;
            item.height = 14;
            item.rare = 0;
			if (Icon.Contains("attack")) {autoReuse = true;}
			NewDefault();
		}

		public override void PostUpdate() => Prefix();
		public override void UpdateInventory(Player player) => Prefix();

		public override bool CanRightClick() => true;
		public override bool ConsumeItem(Player player) => false;
        public override void RightClick(Player player){
			foreach (var i in player.inventory)
			{
				if (i != null) {
					if (i.type == ModContent.ItemType<SpellBag>()) {
						if (i.modItem is SpellBag s) {
							for (int a = 0; a < s.spell.Length; a++){
								if (s.spell[a] < 1) {
									if (StoreInBag(player,s,a)) {
										s.blink = 30f;
										s.spell[a] = item.type;
										s.spellPrefix[a] = spellPrefix;
										CombatText.NewText(player.getRect(),Color.LightGreen,"Equipped");
										Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_equip"),player.Center);
										item.TurnToAir();
										return;
									}
								}
							}
						}
					}
				}
			}
			for (int i = 0; i < 8 + player.extraAccessorySlots; i++)
            {
                if (player.armor[i].type == ModContent.ItemType<SpellBag>()){
					if (player.armor[i].modItem is SpellBag s) {
						for (int a = 0; a < s.spell.Length; a++)
						{
							if (s.spell[a] < 1) {
								if (StoreInBag(player,s,a)) {
									s.blink = 30f;
									s.spell[a] = item.type;
									s.spellPrefix[a] = spellPrefix;
									CombatText.NewText(player.getRect(),Color.LightGreen,"Equipped");
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
			CombatText.NewText(player.getRect(),Color.Pink,"No Empty Spell Bag");
		}
		public override TagCompound Save(){
            TagCompound tag = new TagCompound();
			tag.Add("spellPrefix",spellPrefix);
            return tag;
        }
        public override void Load(TagCompound tag){
			spellPrefix = tag.GetInt("spellPrefix");
        }
		public override void NetSend(BinaryWriter writer) {
			writer.Write(spellPrefix);
		}
		public override void NetRecieve(BinaryReader reader) {
			spellPrefix = reader.ReadInt32();
		}
	}
}