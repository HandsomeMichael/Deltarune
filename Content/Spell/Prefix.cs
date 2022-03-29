using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
using Deltarune;
//this

namespace Deltarune.Content.Spell
{

	public static class SpellPrefix
	{
		/*
		public static void AddSpellPrefix(this Mod mod, SpellPrefix mc,string name) {
			if (Deltarune.spellPrefix.Count == 0) {
				Deltarune.spellPrefix.Add(null);
			}
			mc.Name = name;
			mc.mod = mod;
			mc.AutoStaticDefaults();
			mc.SetStaticDefaults();
			mc.type = Deltarune.spellPrefix.Count;
			Deltarune.spellPrefix.Add(mc);
		}
		public static void AutoloadSpellPrefix(this Mod mod)
		{
			if (mod.Code == null)
				return;

			foreach (Type type in mod.Code.GetTypes().OrderBy(type => type.FullName))
			{
				if (type.IsAbstract)
				{
					continue;
				}
				if (type.IsSubclassOf(typeof(SpellPrefix)))
				{
					var mc = (SpellPrefix)Activator.CreateInstance(type);
					mc.mod = mod;
					var name = type.Name;
					if (mc.Autoload(ref name))
						mod.AddSpellPrefix(mc,name);
				}
			}
		}*/
		public static int Max = 10;
		public static void Effect(int type ,Player player) {
			if (type == 1) {
				player.moveSpeed += 0.03f;
			}
			else if (type == 2) {
				player.moveSpeed += 0.01f;
			}
			else if (type == 3) {
				player.moveSpeed -= 0.02f;
			}
			else if (type == 4) {
				player.allDamage += 0.03f;
			}
			else if (type == 5) {
				player.allDamage += 0.01f;
			}
			else if (type == 6) {
				player.allDamage -= 0.01f;
			}
			else if (type == 7) {
				if (player.velocity.X > -1 && player.velocity.X < 1 && player.velocity.Y > -1 && player.velocity.Y < 1)
				{player.allDamage += 0.05f;}
			}
			else if (type == 8) {
				player.statDefense += 1;
			}
			else if (type == 9) {
				player.statDefense += 3;
			}
			else if (type == 10) {
				player.statDefense -= 1;
			}
		}
		public static void Names(int type,out string name, out string tooltip, out bool bad) {
			bad = false;
			name = "none";
			tooltip = "none";
			if (type == 1) {
				name = "Speedy";
				tooltip = "+3% movement speed";
			}
			else if (type == 2) {
				name = "Light";
				tooltip = "+1% movement speed";
			}
			else if (type == 3) {
				bad = true;
				name = "Heavy";
				tooltip = "-2% movement speed";
			}
			else if (type == 4) {
				name = "Super";
				tooltip = "+3% damage";
			}
			else if (type == 5) {
				name = "Damaging";
				tooltip = "+1% damage";
			}
			else if (type == 6) {
				name = "Sad";
				tooltip = "-1% damage";
				bad = true;
			}
			else if (type == 7) {
				name = "Stoned";
				tooltip = "+5% damage when standing still";
			}
			else if (type == 8) {
				name = "Wild";
				tooltip = "+1 defense";
			}
			else if (type == 9) {
				name = "Defensive";
				tooltip = "+3 defense";
			}
			else if (type == 10) {
				name = "Defenseless";
				tooltip = "-1 defense";
				bad = true;
			}
		}
	}
	/*
	public abstract class SpellPrefix
	{
		/// <summary>
		/// the mod associated with the spell prefix.
		/// </summary>
		public Mod mod;

		/// <summary>
		/// the internal name of the spell prefix.
		/// </summary>
		public string Name;

		/// <summary>
		/// the display name of the spell prefix.
		/// </summary>
		public string DisplayName;

		/// <summary>
		/// The tooltip of the spell prefix.
		/// </summary>
		public string tooltip;

		/// <summary>
		/// the rarity, default to 1.
		/// </summary>
		public int rare = 1;

		/// <summary>
		/// The color of the tooltip.
		/// </summary>
		public Color color;

		/// <summary>
		/// The id of the spell prefix. do not touch this pls
		/// </summary>
		public int type;

		/// <summary>
		/// Allows you to automatically load an item instead of using mod.AddPrefix(SpellPrefix,name). 
		/// Return true to allow autoloading; by default returns the mod's autoload property. 
		/// Use this method to force or stop an autoload or change the internal name.
		/// NOTE : use this.AutoloadSpellPrefix() on Mod.Load to autoload prefixes
		/// </summary>
		/// <param name="name">The name, initialized to the name of this type.</param>
		public virtual bool Autoload(ref string name) {
			return mod.Properties.Autoload;
		}

		/// <summary>
		/// Set display name , and tooltip here.
		/// </summary>
		public virtual void SetStaticDefaults() {

		}
		/// <summary>
		/// Auto load display name.
		/// </summary>
		public virtual void AutoStaticDefaults() {
			color = Color.White;
			DisplayName = Regex.Replace(Name, "([A-Z])", " $1").Trim();
		}

		/// <summary>
		/// the prefix update to player.
		/// </summary>
		public virtual void Update(Player player, int type) {

		}
	}
	//Bad prefix
	public class Cancer : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "-50 max life";
			color = Color.Pink;
			rare = -1;
		}
		public override void Update(Player player, int type) {
			player.statLifeMax2 -= 50;
		}
	}
	//good prefix
	public class Funky : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "+1% damage";
			color = Color.LightGreen;
			rare = 1;
		}
		public override void Update(Player player, int type) {
			player.allDamage += 0.01f;
		}
	}
	public class Farty : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "+6% increased damage";
			color = Color.LightGreen;
			rare = 1;
		}
		public override void Update(Player player, int type) {
			player.allDamage += 0.06f;
		}
	}
	public class Damaging : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "+10% increased damage";
			color = Color.LightGreen;
			rare = 1;
		}
		public override void Update(Player player, int type) {
			player.allDamage += 0.1f;
		}
	}
	public class Ultra : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "+15% damage";
			color = Color.LightGreen;
			rare = 2;
		}
		public override void Update(Player player, int type) {
			player.allDamage += 0.15f;
		}
	}
	public class Mega : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "+3 defense";
			color = Color.LightGreen;
			rare = 1;
		}
		public override void Update(Player player, int type) {
			player.statDefense += 3;
		}
	}
	public class Wild : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "+1 defense";
			color = Color.LightGreen;
			rare = 1;
		}
		public override void Update(Player player, int type) {
			player.statDefense += 1;
		}
	}
	public class Magical : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "+50 increased maximum tp";
			color = Color.LightGreen;
			rare = 1;
		}
		public override void Update(Player player, int type) {
			player.GetDelta().TPMax2 += 50;
		}
	}
	public class Worse : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "-50 maximum tp";
			color = Color.Pink;
			rare = -1;
		}
		public override void Update(Player player, int type) {
			player.GetDelta().TPMax2 -= 50;
		}
	}
	public class Damaged : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "-10% damage";
			color = Color.Pink;
			rare = -1;
		}
		public override void Update(Player player, int type) {
			player.allDamage -= 0.1f;
		}
	}
	public class Poopy : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "-25% damage";
			color = Color.Pink;
			rare = -2;
		}
		public override void Update(Player player, int type) {
			player.allDamage -= 0.25f;
		}
	}
	public class Superior : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "+1% damage\n+1 defense";
			color = Color.LightGreen;
			rare = 1;
		}
		public override void Update(Player player, int type) {
			player.allDamage += 0.01f;
			player.statDefense += 1;
		}
	}
	public class Educated : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "Immune to poison";
			color = Color.LightGreen;
			rare = 2;
		}
		public override void Update(Player player, int type) {
			player.buffImmune[BuffID.Poisoned] = true;
		}
	}
	public class Zoomer : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "+1% movement speed";
			color = Color.Pink;
			rare = 1;
		}
		public override void Update(Player player, int type) {
			player.moveSpeed += 0.01f;
		}
	}
	public class Speed : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "+5% movement speed";
			color = Color.LightGreen;
			rare = 1;
		}
		public override void Update(Player player, int type) {
			player.moveSpeed += 0.05f;
		}
	}
	public class Fantastic : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "+10% damage when immune";
			color = Color.LightGreen;
			rare = 1;
		}
		public override void Update(Player player, int type) {
			if (player.immune){player.allDamage += 0.1f;}
		}
	}
	public class Forgotten : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "-10% movement speed";
			color = Color.Pink;
			rare = -1;
		}
		public override void Update(Player player, int type) {
			player.moveSpeed -= 0.1f;
		}
	}
	public class Standing : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "+10% damage while standing still";
			color = Color.LightGreen;
			rare = 1;
		}
		public override void Update(Player player, int type) {
			if (player.velocity.X > -1 && player.velocity.X < 1 && player.velocity.Y > -1 && player.velocity.Y < 1)
			{player.allDamage += 0.1f;}
		}
	}
	public class Running : SpellPrefix
	{
		public override void SetStaticDefaults() {
			tooltip = "+5% damage while moving";
			color = Color.LightGreen;
			rare = 1;
		}
		public override void Update(Player player, int type) {
			if (player.velocity.X > 0 || player.velocity.X < 0){
				player.allDamage += 0.05f;
			}
		}
	}*/
}