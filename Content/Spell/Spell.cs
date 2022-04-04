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
using Deltarune.Content.Items;
using Deltarune.Content.Dusts;
using Deltarune.Content.Projectiles;
using Deltarune;
//this

namespace Deltarune.Content.Spell
{
	public class Spell_ : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Blank Spell");
			Tooltip.SetDefault("Used to make spell");
		}
		public override void SetDefaults() 
		{
            item.width = 14;
            item.height = 14;
            item.rare = 0;
			item.maxStack = 100;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.FallenStar,1);
			recipe.AddIngredient(ItemID.Silk,10);
			recipe.AddTile(TileID.Loom);
			recipe.SetResult(this,3);
			recipe.AddRecipe(); 
		}
	}
	public class Heal : BaseSpell
	{
		public override string name => "Heal";
		public override string Icon => "life";
		public override void NewDefault() {
			tip = "Heal player by 10%";
			TPCost = 0.1f;
			item.rare = 1;
			cooldown = 30;
		}
		public override void Spell(Player player, int type) {
			healdust.Spawn(player.Center,8);
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom,"Sounds/snd_power"),player.Center);
			player.HealEffect(player.statLifeMax2/10); 
			player.statLife += player.statLifeMax2/10;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.LifeCrystal,3);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class Nursing : BaseSpell
	{
		public override string name => "Nurse Magik";
		public override string Icon => "nurse";
		public override void NewDefault() {
			tip = "Heal 50% of player hp";
			TPCost = 1f;
			item.rare = 3;
			cooldown = 60*120;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Heal>());
			recipe.AddIngredient(ModContent.ItemType<SimpCard>());
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
		public override void Spell(Player player, int type) {
			healdust.Spawn(player.Center,15);
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_power"),player.Center);
			player.HealEffect(player.statLifeMax2/2);
			player.statLife += player.statLifeMax2/2;
		}
	}
	public class ManaUp : BaseSpell
	{
		public override string name => "Mana Up";
		public override string Icon => "mana";
		public override void NewDefault() {
			tip = "Increases mana by 10%";
			TPCost = 0.1f;
			item.rare = 1;
			cooldown = 30;
		}

		public override void Spell(Player player, int type) {
			healdust2.Spawn(player.Center);
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_power"),player.Center);
			player.ManaEffect(player.statManaMax2/10);
			player.statMana += player.statManaMax2/10;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.ManaCrystal,3);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class Fireball : BaseSpell
	{
		public override string name => "Fireboll I";
		public override string Icon => "attack1";
		public override void NewDefault() {
			tip = "Summon a fireball that deals 30 damage";
			TPCost = 0.1f;
			item.rare = 1;
			cooldown = 30;
		}
		public override void Spell(Player player, int type) {
			var p = player.GetDelta();
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_swing"),player.Center);

			for (int i = 0; i < 10; i++) {
				Vector2 vel = Main.rand.NextVector2CircularEdge(1f, 1f);
				Dust d = Dust.NewDustPerfect(player.Center - new Vector2(0,60), 6, vel * Main.rand.NextFloat(3f), Scale: Main.rand.NextFloat(2f,4f));
				d.noGravity = true;
			}

			Vector2 speed = (player.Center - new Vector2(0,60)).DirectionTo(Main.MouseWorld)*15f;
			if (Main.netMode != NetmodeID.MultiplayerClient) {
				float damage = 30f;
				damage *= player.allDamage;
				int a = Projectile.NewProjectile(player.Center - new Vector2(0,60),speed,ProjectileID.Fireball,(int)damage,1,player.whoAmI);
				Main.projectile[a].hostile = false;
				Main.projectile[a].friendly = true;
				Main.projectile[a].penetrate = 1;
				NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, a);
			}
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.Gel,5);
			recipe.AddIngredient(ItemID.MeteoriteBar,10);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class Fireball2 : BaseSpell
	{
		public override string name => "Fireboll II";
		public override string Icon => "attack2";
		public override void NewDefault() {
			tip = "Summon 2 fireball that deals 90 damage";
			TPCost = 0.1f;
			item.rare = 1;
			cooldown = 30;
		}
		public override void Spell(Player player, int type) {
			var p = player.GetDelta();
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_swing"),player.Center);

			for (int i = 0; i < 15; i++) {
				Vector2 vel = Main.rand.NextVector2CircularEdge(1f, 1f);
				Dust d = Dust.NewDustPerfect(player.Center - new Vector2(0,60), 6, vel * Main.rand.NextFloat(3f), Scale: Main.rand.NextFloat(2f,4f));
				d.noGravity = true;
			}

			Vector2 v = (player.Center - new Vector2(0,60)).DirectionTo(Main.MouseWorld)*15f;
			if (Main.netMode != NetmodeID.MultiplayerClient) {
				for (int i = 0; i < 2; i++){
					float damage = 90f;
					damage *= player.allDamage;
					Vector2 speed = v.RotatedByRandom(MathHelper.ToRadians(10));
					int a = Projectile.NewProjectile(player.Center - new Vector2(0,60),speed,ProjectileID.Fireball,(int)damage,1,player.whoAmI);
					Main.projectile[a].hostile = false;
					Main.projectile[a].friendly = true;
					Main.projectile[a].penetrate = 1;
					NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, a);
				}
			}
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Fireball>());
			recipe.AddIngredient(ItemID.SoulofNight,2);
			recipe.AddIngredient(ItemID.SoulofLight,2);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class Fireball3 : BaseSpell
	{
		public override string name => "Fireboll III";
		public override string Icon => "attack3";
		public override void NewDefault() {
			tip = "Summon 4 fireball that deals 100 damage";
			TPCost = 0.1f;
			item.rare = 1;
			cooldown = 30;
		}
		public override void Spell(Player player, int type) {
			var p = player.GetDelta();
			for (int i = 0; i < 20; i++) {
				Vector2 v = Main.rand.NextVector2CircularEdge(1f, 1f);
				Dust d = Dust.NewDustPerfect(player.Center - new Vector2(0,60), 6, v * Main.rand.NextFloat(3f), Scale: Main.rand.NextFloat(2f,4f));
				d.noGravity = true;
			}

			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_ultraswing"),player.Center);
			Vector2 vel = (player.Center - new Vector2(0,60)).DirectionTo(Main.MouseWorld)*15f;
			if (Main.netMode != NetmodeID.MultiplayerClient) {
				for (int i = 0; i < 4; i++)
				{
					Vector2 speed = vel.RotatedByRandom(MathHelper.ToRadians(20));
					float damage = 100f;
					damage *= player.allDamage;
					int a = Projectile.NewProjectile(player.Center - new Vector2(0,60),speed,ProjectileID.Fireball,(int)damage,1,player.whoAmI);
					Main.projectile[a].hostile = false;
					Main.projectile[a].friendly = true;	
					Main.projectile[a].penetrate = 1;
					NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, a);
				}
			}
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Fireball2>());
			recipe.AddIngredient(ItemID.FragmentSolar,4);
			recipe.AddIngredient(ItemID.SoulofMight,10);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class ArrowFireCircle : BaseSpell
	{
		public override string name => "Flaming Arrow Circle";
		public override string Icon => "attack2";
		public override void NewDefault() {
			tip = "Create a circle of flaming arrow";
			TPCost = 0.15f;
			item.rare = 2;
			cooldown = 10;
		}
		public override void Spell(Player player, int type) {
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_ultraswing"),player.Center);
			for (int i = 0; i < 15; i++) {
				Vector2 vel = Main.rand.NextVector2CircularEdge(1f, 1f);
				Dust d = Dust.NewDustPerfect(player.Center - new Vector2(0,60), 6, vel * Main.rand.NextFloat(3f), Scale: Main.rand.NextFloat(2f,4f));
				d.noGravity = true;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient) {	 
				for (int a = 0; a < Main.rand.Next(10,21); a++) {
					Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
					float damage = 10f;
					damage *= player.allDamage;
					int b = Projectile.NewProjectile(player.Center - new Vector2(0,60),speed*10f,ProjectileID.FlamingArrow,(int)damage,1,player.whoAmI);
					Main.projectile[b].friendly = true;
					Main.projectile[b].hostile = false;
					Main.projectile[b].arrow = true;
					Main.projectile[b].ranged = true;
					Main.projectile[b].noDropItem = true;
					Main.projectile[b].tileCollide = false;
					Main.projectile[b].timeLeft = 60*3;
					NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, b);
				}
			}
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.FallenStar,1);
			recipe.AddIngredient(ItemID.FlamingArrow,20);
			recipe.AddIngredient(ItemID.Gel,10);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class JumpFly : BaseSpell
	{
		public override string name => "Jump and Fly";
		public override string Icon => "fly";

		public override void NewDefault() {
			tip = "Allow you to jump and fly";
			TPCost = 0.05f;
			item.rare = 1;
			cooldown = 90;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.Cloud,20);
			recipe.AddIngredient(ItemID.Feather,2);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
		public override void Spell(Player player, int type) {
			if (player.velocity.Y == 0) {player.velocity.Y = -20;}
			else {player.velocity.Y = -15;}
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_jump"),player.Center);
		}
	}
	public class SandThrow : BaseSpell
	{
		public override string name => "Sand Thrower";
		public override string Icon => "attack1";

		public override void NewDefault() {
			tip = "Cast a sand";
			TPCost = 0.01f;
			item.rare = 2;
			cooldown = 2;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.SandBlock,10);
			recipe.AddIngredient(ItemID.Sandgun);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
		public override void Spell(Player player, int type) {
			Vector2 vel = (player.Center - new Vector2(0,60)).DirectionTo(Main.MouseWorld)*15f;
			//Main.PlaySound(SoundID.Item33,player.Center);
			if (Main.netMode != NetmodeID.MultiplayerClient) {	 
				int b = Projectile.NewProjectile(player.Center - new Vector2(0,60),vel,ProjectileID.SandBallFalling,5,1,player.whoAmI);
				Main.projectile[b].hostile = false;
				Main.projectile[b].friendly = true;
				NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, b);
			}
		}
	}
	public class PshychoDash : BaseSpell
	{
		public override string name => "Pshycho Dash";
		public override string Icon => "speed";

		public override void NewDefault() {
			tip = "Dash to nearby enemy\nif no enemy detected , Dash to cursor";
			TPCost = 0.05f;
			item.rare = 2;
			autoReuse = true;
			cooldown = 20;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.DemoniteBar,5);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe(); 

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.CrimtaneBar,5);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
		public override void Spell(Player player, int type) {
			int b = Helpme.NearestNPC(player.Center);
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_dtrans_heavypassing"),player.Center);
			if (b > -1) {
				player.velocity = player.DirectionTo(Main.npc[b].Center)*10f;
			}
			else {
				player.velocity = player.DirectionTo(Main.MouseWorld)*10f;
			}
			Main.SetCameraLerp(0.1f, 15);
			Main.screenPosition -= player.velocity*0.5f;
		}
	}
	public class Farting : BaseSpell
	{
		public override string name => "Whoopie Cushion";
		public override string Icon => "fart";

		public override void NewDefault() {
			tip = "No description";
			TPCost = 1f;
			item.rare = 3;
			autoReuse = true;
			cooldown = 60*5;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.WhoopieCushion);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
		public override void Spell(Player player, int type) {
			Main.PlaySound(SoundID.Item16);
			if (Main.rand.NextBool(69696969)) {
				player.QuickSpawnItem(ItemID.WhoopieCushion);
			}
		}
	}
	public class GoldFarting : BaseSpell
	{
		public override string name => "Omega Fard";
		public override string Icon => "fart";

		public override void NewDefault() {
			tip = "Reduces nearby enemy hp by 5%";
			TPCost = 1f;
			item.rare = 5;
			autoReuse = true;
			cooldown = 60*10;
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Farting>());
			recipe.AddIngredient(ItemID.SoulofLight,3);
			recipe.AddIngredient(ItemID.SoulofNight,3);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
		public override void Spell(Player player, int type) {
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/gold"));
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.active && !npc.friendly && npc.Distance(player.Center) < 400f) {
					npc.life -= (int)((float)npc.life*0.05f);

					Vector2 dustPos = player.Center - new Vector2(0,60);
					Vector2 pos = npc.Center;
					while (dustPos.Distance(pos) > 20f){
						Dust d = Dust.NewDustPerfect(dustPos, 159, Vector2.Zero);
						d.noGravity = true;
						dustPos += dustPos.DirectionTo(pos)*5f;
					}
				}
			}
		}
	}
	public class Lazer : BaseSpell
	{
		public override string name => "Quick Lazer";
		public override string Icon => "attack1";

		public override void NewDefault() {
			tip = "Shoot out lazer nearest enemy or cursor\neach laser deals 15 damage\ndamage reduced by player velocity";
			TPCost = 0.01f;
			item.rare = 1;
			cooldown = 9;
		}

		public override void Spell(Player player, int type) {
			Vector2 pos = player.Center - new Vector2(0,60);
			Vector2 vel = pos.DirectionTo(Main.MouseWorld)*15f;
			Main.PlaySound(SoundID.Item33,player.Center);
			int index = Helpme.NearestNPC(player.Center);
			if (index > -1) {
				vel = pos.DirectionTo(Main.npc[index].Center)*15f;
			}
			for (int a = 0; a < 5; a++) {
				Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
				Dust d = Dust.NewDustPerfect(pos, 131, speed * 2f, Scale: 0.6f);
				d.noGravity = true;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient) {
				float damage = 15;
				damage *= player.allDamage;
				damage -= damage*(player.velocity.FloatCount()/10f);
				Projectile.NewProjectile(pos,vel,ProjectileID.GreenLaser,(int)damage,1,player.whoAmI);
			}
			//Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_dtrans_heavypassing"),player.Center);
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.MeteoriteBar,20);
			recipe.AddIngredient(ItemID.FallenStar,1);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class TripleLazer : BaseSpell
	{
		public override string name => "Triple Lazer";
		public override string Icon => "attack1";

		public override void NewDefault() {
			tip = "Shoot out a spread of laser to nearest enemy or cursor\neach laser deals 30 damage\ndamage increased by player velocity";
			TPCost = 0.1f;
			item.rare = 1;
			cooldown = 60;
		}

		public override void Spell(Player player, int type) {
			Vector2 pos = player.Center - new Vector2(0,60);
			Vector2 vel = pos.DirectionTo(Main.MouseWorld)*15f;
			Main.PlaySound(SoundID.Item33,player.Center);
			int index = Helpme.NearestNPC(player.Center);
			if (index > -1) {
				vel = pos.DirectionTo(Main.npc[index].Center)*15f;
			}
			for (int a = 0; a < 10; a++) {
				Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
				Dust d = Dust.NewDustPerfect(pos, 131, speed * 2f, Scale: 0.6f);
				d.noGravity = true;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient) {
				for (int i = 0; i < 3; i++){
					Vector2 speed = vel.RotatedByRandom(MathHelper.ToRadians(20));
					float damage = 30;
					damage *= player.allDamage;
					damage += damage*(player.velocity.FloatCount()/8f);
					Projectile.NewProjectile(pos,speed,ProjectileID.GreenLaser,(int)damage,1,player.whoAmI);
				}
			}
			//Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_dtrans_heavypassing"),player.Center);
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.MeteoriteBar,15);
			recipe.AddIngredient(ItemID.DemoniteBar,2);
			recipe.AddIngredient(ItemID.FallenStar,1);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe(); 

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.MeteoriteBar,15);
			recipe.AddIngredient(ItemID.CrimtaneBar,2);
			recipe.AddIngredient(ItemID.FallenStar,1);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class CircleStar : BaseSpell
	{
		public override string name => "Circle star";
		public override string Icon => "attack3";

		public override void NewDefault() {
			tip = "Strike nearest enemy with circle of star";
			TPCost = 0.1f;
			item.rare = 1;
			cooldown = 120;
		}

		public override void Spell(Player player, int type) {
			int a = Helpme.NearestNPC(player.Center);
			if (a > -1) {
				Vector2 pos = Main.npc[a].Center;
				if (Main.netMode != NetmodeID.MultiplayerClient) {
					for (int i = 0; i < 10; i++){
						Vector2 center = pos + Vector2.One.RotatedBy(MathHelper.ToRadians(36*i))*100f;
						Projectile.NewProjectile(center,center.DirectionTo(pos)*10f,
						ModContent.ProjectileType<starwalkerStarFriendly>(),50,1,player.whoAmI);
					}
				}
				for (int i = 0; i < 10; i++) {
					Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
					Dust d = Dust.NewDustPerfect(pos, 159, speed * 2f, Scale: 0.6f);
					d.noGravity = true;
				}
				Vector2 dustPos = player.Center - new Vector2(0,60);
				//using while() for the first time :)
				while (dustPos.Distance(pos) > 20f){
					Dust d = Dust.NewDustPerfect(dustPos, 159, Vector2.Zero);
					d.noGravity = true;
					dustPos += dustPos.DirectionTo(pos)*5f;
				}
				//starwalkerStarFriendly
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_petrify"),player.Center);
				return;
			}
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_cantselect"),player.Center);
		}
		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Spell_>());
			recipe.AddIngredient(ItemID.MeteoriteBar,20);
			recipe.AddIngredient(ItemID.ManaCrystal,1);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe(); 
		}
	}
	public class DespawnNPC : BaseSpell
	{
		public override string Icon => "dead";
		public override void NewDefault() {
			tip = "Despawn Nearest NPC";
			TPCost = 0f;
			item.rare = -12;
			cooldown = 5;
		}

		public override void Spell(Player player, int type) {
			int a = Helpme.NearestNPC(player.Center,1000f,true);
			if (a > -1) {
				Vector2 pos = Main.npc[a].Center;
				Main.npc[a].active = false;
				for (int i = 0; i < 10; i++) {
					Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
					Dust d = Dust.NewDustPerfect(pos, 159, speed * 2f, Scale: 0.6f);
					d.noGravity = true;
				}
				Vector2 dustPos = player.Center - new Vector2(0,60);
				//using while() for the first time :)
				while (dustPos.Distance(pos) > 20f){
					Dust d = Dust.NewDustPerfect(dustPos, 159, Vector2.Zero);
					d.noGravity = true;
					dustPos += dustPos.DirectionTo(pos)*5f;
				}
				//starwalkerStarFriendly
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_petrify"),player.Center);
				return;
			}
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_cantselect"),player.Center);
		}
	}
	public class ClearProj : BaseSpell
	{
		public override string name => "Clear projectile";
		public override string Icon => "dead";
		public override void NewDefault() {
			tip = "Clear all projectiles";
			TPCost = 0f;
			item.rare = -12;
			cooldown = 5;
		}
		public override void Spell(Player player, int type) {
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				if (Main.projectile[i].active) {
					Explode.New(Main.projectile[i].Center);
					Main.projectile[i].damage = 0;
					Main.projectile[i].Kill();
					Vector2 pos = Main.projectile[i].Center;
					Vector2 dustPos = player.Center - new Vector2(0,60);
					while (dustPos.Distance(pos) > 20f){
						Dust d = Dust.NewDustPerfect(dustPos, 159, Vector2.Zero);
						d.noGravity = true;
						dustPos = Vector2.Lerp(dustPos,pos,0.05f);
					}
				}
			}
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_badexplosion"),player.Center);
		}
	}
	public class SetTPMAX : BaseSpell
	{
		public override string name => "Maxed Tension Point";
		public override string Icon => "dead";
		public override void NewDefault() {
			tip = "Set tension point to max";
			TPCost = 0f;
			item.rare = -12;
			cooldown = 5;
		}
		public override void Spell(Player player, int type) {
			player.GetDelta().TP = player.GetDelta().TPMax;
			Explode.New(player.Center,1f);
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_badexplosion"),player.Center);
		}
	}
	public class ClearNPC : BaseSpell
	{
		public override string name => "Clear Enemy";
		public override string Icon => "dead";
		public override void NewDefault() {
			tip = "Clear all Hostile NPC";
			TPCost = 0f;
			item.rare = -12;
			cooldown = 5;
		}
		public override void Spell(Player player, int type) {
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (!npc.friendly && npc.active) {
					npc.life = 0;
					npc.HitEffect(0, 0);
					npc.checkDead();
					Explode.New(npc.Center);
				}
			}
		}
	}
}