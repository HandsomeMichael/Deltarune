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
using Deltarune;
using System.Threading;
using System.Windows.Forms;
//this

namespace Deltarune.Content.Items
{
	public abstract class CheatItem : ModItem
	{
		public virtual string tt => "";
		public override string Texture => Deltarune.textureExtra+"susie";
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Test Item (hello cheater)\nthis are mean to be used on debugging / testing stuff\nusing this item may broke the game bc funky\n"+tt);
		}
		public override void SetDefaults() {
			item.useAnimation = 20;
			item.useTime = 20;
			item.autoReuse = true;
			item.useStyle = 1;
            item.width = 14;
            item.height = 14;
			item.accessory = true;
            item.rare = -12;
		}
		public bool acc;
		public virtual void UpdateUse(Player player) {}
		public virtual void Update(Player player) {}
		public override void UpdateAccessory(Player player, bool hide) {Update(player);}
		public override bool UseItem(Player player) {
			UpdateUse(player);
			Update(player);
			return true;
		}
	}
	public class CHUnlimitedTP : CheatItem{
		public override string tt => "set tp to max";
		public override void Update(Player player) {
			var p = player.GetDelta();
			p.TP = p.TPMax;
		}
	}
	public class CHNoitem : CheatItem{
		public override string tt => "clear inventory";
		public override void Update(Player player) {
			for (int i = 0; i < Main.maxItems; i++)
			{
				if (Main.item[i].type > 0 && Main.item[i].active) {
					Main.item[i].TurnToAir();
				}
			}
		}
	}
	public class CHReset : CheatItem{
		public override string tt => "Reset downed boss";
		public override void Update(Player player) {
			Main.hardMode = false;
			Main.bloodMoon = false;
			Main.snowMoon = false;
			Main.pumpkinMoon = false;
			Main.eclipse = false;
			NPC.savedTaxCollector = false;
			NPC.savedGoblin = false;
			NPC.savedWizard = false; 
			NPC.savedMech = false; 
			NPC.savedAngler = false; 
			NPC.savedStylist = false; 
			NPC.savedBartender = false;
			NPC.downedBoss1 = false;
			NPC.downedBoss2 = false;
			NPC.downedBoss3 = false;
			NPC.downedQueenBee = false;
			NPC.downedSlimeKing = false;
			NPC.downedGoblins = false;
			NPC.downedFrost = false;
			NPC.downedPirates = false; 
			NPC.downedClown = false; 
			NPC.downedPlantBoss = false; 
			NPC.downedGolemBoss = false; 
			NPC.downedMartians = false; 
			NPC.downedFishron = false;
			NPC.downedHalloweenTree = false; 
			NPC.downedHalloweenKing = false; 
			NPC.downedChristmasIceQueen = false; 
			NPC.downedChristmasTree = false; 
			NPC.downedChristmasSantank = false;
			NPC.downedAncientCultist = false; 
			NPC.downedMoonlord = false; 
			NPC.downedTowerSolar = false; 
			NPC.downedTowerVortex = false; 
			NPC.downedTowerNebula = false; 
			NPC.downedTowerStardust = false;
			NPC.downedMechBossAny = false; 
			NPC.downedMechBoss1 = false; 
			NPC.downedMechBoss2 = false; 
			NPC.downedMechBoss3 = false;
			MyWorld.downedStarWalker = false;
		}
	}
	public class CHRalsei : CheatItem{
		public override string tt => "reset ralsei";
		public override void Update(Player player) {
			MyWorld.hadRalsei = false;
		}
	}
	public class CHSandTest : CheatItem{
		public override string tt => "subeteyo ymir tachi";
		public override void Update(Player player) {
			Hacc.ToAllSubjectOfYmir = !Hacc.ToAllSubjectOfYmir;
		}
	}
	public class CHOneHP : CheatItem{
		public override string tt => "set hp to one";
		public override void Update(Player player) {
			player.statLife = 1;
		}
	}
	public class CHDeath : CheatItem{
		public override string tt => "kill the player (CRASH)";
		public override void UpdateUse(Player player) {
			player.statLife = 0;
			player.dead = true;
		}
	}
	public class CHWorldGen : CheatItem{
		public override string tt => "do some modded worldgen";
		public override void UpdateUse(Player player) {
			ModContent.GetInstance<MyWorld>().PostWorldGen();
		}
	}
	public class CHBattleBackground : CheatItem{
		public override string tt => "set battle background to true";
		public override void Update(Player player) {
			Deltarune.battleAlpha = MyConfig.get.BattleBackground;
		}
	}
	public class CHUseDebug : CheatItem{
		public override string tt => "show debug values (removed soon)";
		public override void Update(Player player) {
			var p = player.GetDelta();
			Main.NewText("====[ Debug ]=====");
			for (int i = 0; i < Deltarune.debug.Length; i++){
				Main.NewText($"deb{i} = {Deltarune.debug[i]}");
			}
			Main.NewText("=================");
		}
	}
	public class CHUsePlayerToString : CheatItem{
		public override string tt => "out put player values";
		public override void Update(Player player) {
			Main.NewText("====[ Stat ]=====");
			Main.NewText($"def {player.statDefense}",Color.LightGreen);
			Main.NewText($"life {player.statLife} / {player.statLifeMax2}",Color.LightGreen);
			Main.NewText($"sped {player.moveSpeed}",Color.LightGreen);
			Main.NewText($"endu {player.endurance}",Color.LightGreen);
			Main.NewText($"allDam {player.allDamage}",Color.LightGreen);
			Main.NewText($"TP {player.GetDelta().TP} / {player.GetDelta().TPMax}",Color.LightGreen);
			Main.NewText("=================");
		}
	}
	public class CHUseNPCToString : CheatItem{
		public override string tt => "out put nearest npc values";
		public override void Update(Player player) {
			int a = Helpme.NearestNPC(player.Center);
			if (a > -1) {
				NPC npc = Main.npc[a];
				Main.NewText("====[ Stat ]=====");
				Main.NewText($"name {npc.FullName}",Color.LightGreen);
				Main.NewText($"def {npc.defense}",Color.LightGreen);
				Main.NewText($"life {npc.life} / {npc.lifeMax}",Color.LightGreen);
				Main.NewText($"damage {npc.damage}",Color.LightGreen);
				Main.NewText($"value {npc.value}",Color.LightGreen);
				Main.NewText($"ai0 {npc.ai[0]}",Color.LightGreen);
				Main.NewText($"ai1 {npc.ai[1]}",Color.LightGreen);
				Main.NewText($"ai2 {npc.ai[2]}",Color.LightGreen);
				Main.NewText($"localAI0 {npc.localAI[0]}",Color.LightGreen);
				Main.NewText($"localAI1 {npc.localAI[1]}",Color.LightGreen);
				Main.NewText("=================");
			}
		}
	}
}