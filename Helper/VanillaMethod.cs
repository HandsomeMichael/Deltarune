using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using Terraria.Graphics.Shaders;
using Terraria.ObjectData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Deltarune;
using Deltarune.Content;
using System.Reflection;
using System.Threading.Tasks;
using Terraria.Graphics.Capture;
using Terraria.IO;
using Terraria.UI.Chat;
using Terraria.UI;
using System.Threading;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ReLogic.Graphics;
using Terraria.Chat;
using Terraria.GameContent.UI.Chat;
//using Terraria.ModLoader.Audio;
using Terraria.ModLoader.Config;

namespace Deltarune.Helper
{
	/// <summary>
	/// A static class that has extension for vanilla methods
	/// </summary>
	public static class VanillaMethod
	{
		/// <summary>
		/// Melee effects for item, can be found in ItemCheck method at Player.cs
		/// Plus some of it are getting id
		/// </summary>
		public static void MeleeEffects(Item item,Player player,Rectangle hitbox, bool modstuff = true) {

			#region Specific item type check

			if (item.type == ItemID.EnchantedSword && Main.rand.Next(5) == 0)
			{
				int pushYUp4 = Main.rand.Next(3);

				int dustType = 15;
				if (pushYUp4 == 1) {dustType = 57;}
				else if (pushYUp4 == 2) {dustType = 58;}

				int num186 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, dustType, player.direction * 2, 0f, 150, default(Color), 1.3f);
				Main.dust[num186].velocity *= 0.2f;
			}
			if (item.type == ItemID.InfluxWaver && Main.rand.Next(2) == 0)
			{
				int type18 = Utils.SelectRandom<int>(Main.rand, 226, 229);
				int num188 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, type18, player.direction * 2, 0f, 150);
				Main.dust[num188].velocity *= 0.2f;
				Main.dust[num188].noGravity = true;
			}
			if ((item.type == ItemID.DemonBow || item.type == ItemID.WarAxeoftheNight || item.type == ItemID.LightsBane || item.type == ItemID.NightmarePickaxe || item.type == ItemID.TheBreaker) && Main.rand.Next(15) == 0)
			{
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 14, player.direction * 2, 0f, 150, default(Color), 1.3f);
			}
			if (item.type == ItemID.NightsEdge || item.type == ItemID.TrueNightsEdge)
			{
				if (Main.rand.Next(5) == 0)
				{
					Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 14, player.direction * 2, 0f, 150, default(Color), 1.4f);
				}
				int num189 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 27, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 1.2f);
				Main.dust[num189].noGravity = true;
				Main.dust[num189].velocity.X /= 2f;
				Main.dust[num189].velocity.Y /= 2f;
			}
			if (item.type == ItemID.BeamSword && Main.rand.Next(2) == 0)
			{
				int num190 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 64, 0f, 0f, 150, default(Color), 1.2f);
				Main.dust[num190].noGravity = true;
			}
			if (item.type == ItemID.Starfury)
			{
				if (Main.rand.Next(5) == 0)
				{
					Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 58, 0f, 0f, 150, default(Color), 1.2f);
				}
				if (Main.rand.Next(10) == 0)
				{
					Gore.NewGore(new Vector2(hitbox.X, hitbox.Y), default(Vector2), Main.rand.Next(16, 18));
				}
			}
			if (item.type == ItemID.StarWrath)
			{
				int num191 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 58, 0f, 0f, 150, default(Color), 1.2f);
				Main.dust[num191].velocity *= 0.5f;
				if (Main.rand.Next(8) == 0)
				{
					int num192 = Gore.NewGore(new Vector2(hitbox.Center.X, hitbox.Center.Y), default(Vector2), 16);
					Main.gore[num192].velocity *= 0.5f;
					Main.gore[num192].velocity += new Vector2(player.direction, 0f);
				}
			}
			if (item.type == ItemID.BladeofGrass)
			{
				int num193 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 40, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 0, default(Color), 1.2f);
				Main.dust[num193].noGravity = true;
			}
			else if (item.type == ItemID.StaffofRegrowth)
			{
				int num194 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 3, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 0, default(Color), 1.2f);
				Main.dust[num194].noGravity = true;
			}
			if (item.type == ItemID.FieryGreatsword)
			{
				for (int num195 = 0; num195 < 2; num195++)
				{
					int num196 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 6, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 2.5f);
					Main.dust[num196].noGravity = true;
					Main.dust[num196].velocity.X *= 2f;
					Main.dust[num196].velocity.Y *= 2f;
				}
			}
			if (item.type == ItemID.MoltenPickaxe || item.type == ItemID.MoltenHamaxe)
			{
				int num197 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 6, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 1.9f);
				Main.dust[num197].noGravity = true;
			}
			if (item.type == ItemID.Muramasa)
			{
				int num199 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 172, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 0.9f);
				Main.dust[num199].noGravity = true;
				Main.dust[num199].velocity *= 0.1f;
			}
			if (item.type == ItemID.Frostbrand && Main.rand.Next(3) == 0)
			{
				int num200 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 67, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 90, default(Color), 1.5f);
				Main.dust[num200].noGravity = true;
				Main.dust[num200].velocity *= 0.2f;
			}
			if (item.type == ItemID.Meowmere)
			{
				int num201 = Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 66, 0f, 0f, 150, Color.Transparent, 0.85f);
				Main.dust[num201].color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f);
				Main.dust[num201].noGravity = true;
				Main.dust[num201].velocity /= 2f;
			}
			if (item.type == ItemID.DD2SquireDemonSword)
			{
				Dust dust2 = Dust.NewDustDirect(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 6, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, Color.Transparent, 0.7f);
				dust2.noGravity = true;
				dust2.velocity *= 2f;
				dust2.fadeIn = 0.9f;
			}
			if (item.type == ItemID.IceBlade && Main.rand.Next(5) == 0)
			{
				int num202 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 67, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 90, default(Color), 1.5f);
				Main.dust[num202].noGravity = true;
				Main.dust[num202].velocity *= 0.2f;
			}
			// basically every crimson items from BloodButcherer to TheRottedFork
			if (item.type >= 795 && item.type <= 802 && Main.rand.Next(3) == 0)
			{
				int num203 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 115, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 140, default(Color), 1.5f);
				Main.dust[num203].noGravity = true;
				Main.dust[num203].velocity *= 0.25f;
			}
			if (item.type == ItemID.Pwnhammer || item.type == ItemID.Excalibur || item.type == ItemID.TrueExcalibur)
			{
				int num204 = 0;
				if (Main.rand.Next(3) == 0)
				{
					num204 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 57, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 1.1f);
					Main.dust[num204].noGravity = true;
					Main.dust[num204].velocity.X /= 2f;
					Main.dust[num204].velocity.Y /= 2f;
					Main.dust[num204].velocity.X += player.direction * 2;
				}
				if (Main.rand.Next(4) == 0)
				{
					num204 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 43, 0f, 0f, 254, default(Color), 0.3f);
					Main.dust[num204].velocity *= 0f;
				}
			}
			// every phaseblade, and its upgrade
			if ((item.type >= 198 && item.type <= 203) || (item.type >= 3764 && item.type <= 3769))
			{
				float num205 = 0.5f;
				float num206 = 0.5f;
				float num207 = 0.5f;
				if (item.type == 198 || item.type == 3764)
				{
					num205 *= 0.1f;
					num206 *= 0.5f;
					num207 *= 1.2f;
				}
				else if (item.type == 199 || item.type == 3765)
				{
					num205 *= 1f;
					num206 *= 0.2f;
					num207 *= 0.1f;
				}
				else if (item.type == 200 || item.type == 3766)
				{
					num205 *= 0.1f;
					num206 *= 1f;
					num207 *= 0.2f;
				}
				else if (item.type == 201 || item.type == 3767)
				{
					num205 *= 0.8f;
					num206 *= 0.1f;
					num207 *= 1f;
				}
				else if (item.type == 202 || item.type == 3768)
				{
					num205 *= 0.8f;
					num206 *= 0.9f;
					num207 *= 1f;
				}
				else if (item.type == 203 || item.type == 3769)
				{
					num205 *= 0.9f;
					num206 *= 0.9f;
					num207 *= 0.1f;
				}
				Lighting.AddLight((int)((player.itemLocation.X + 6f + player.velocity.X) / 16f), (int)((player.itemLocation.Y - 14f) / 16f), num205, num206, num207);
			}
			#endregion

			#region Player field check

			if (player.frostBurn && item.melee && !item.noMelee && !item.noUseGraphic && Main.rand.Next(2) == 0)
			{
				int num208 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 135, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 2.5f);
				Main.dust[num208].noGravity = true;
				Main.dust[num208].velocity *= 0.7f;
				Main.dust[num208].velocity.Y -= 0.5f;
			}
			if (item.melee && !item.noMelee && !item.noUseGraphic && player.meleeEnchant > 0)
			{
				if (player.meleeEnchant == 1)
				{
					if (Main.rand.Next(3) == 0)
					{
						int num210 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 171, 0f, 0f, 100);
						Main.dust[num210].noGravity = true;
						Main.dust[num210].fadeIn = 1.5f;
						Main.dust[num210].velocity *= 0.25f;
					}
				}
				else if (player.meleeEnchant == 2)
				{
					if (Main.rand.Next(2) == 0)
					{
						int num211 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 75, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 2.5f);
						Main.dust[num211].noGravity = true;
						Main.dust[num211].velocity *= 0.7f;
						Main.dust[num211].velocity.Y -= 0.5f;
					}
				}
				else if (player.meleeEnchant == 3)
				{
					if (Main.rand.Next(2) == 0)
					{
						int num212 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 6, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 2.5f);
						Main.dust[num212].noGravity = true;
						Main.dust[num212].velocity *= 0.7f;
						Main.dust[num212].velocity.Y -= 0.5f;
					}
				}
				else if (player.meleeEnchant == 4)
				{
					int num213 = 0;
					if (Main.rand.Next(2) == 0)
					{
						num213 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 57, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 1.1f);
						Main.dust[num213].noGravity = true;
						Main.dust[num213].velocity.X /= 2f;
						Main.dust[num213].velocity.Y /= 2f;
					}
				}
				else if (player.meleeEnchant == 5)
				{
					if (Main.rand.Next(2) == 0)
					{
						int num214 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 169, 0f, 0f, 100);
						Main.dust[num214].velocity.X += player.direction;
						Main.dust[num214].velocity.Y += 0.2f;
						Main.dust[num214].noGravity = true;
					}
				}
				else if (player.meleeEnchant == 6)
				{
					if (Main.rand.Next(2) == 0)
					{
						int num215 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 135, 0f, 0f, 100);
						Main.dust[num215].velocity.X += player.direction;
						Main.dust[num215].velocity.Y += 0.2f;
						Main.dust[num215].noGravity = true;
					}
				}
				else if (player.meleeEnchant == 7)
				{
					if (Main.rand.Next(20) == 0)
					{
						int type19 = Main.rand.Next(139, 143);
						int num216 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, type19, player.velocity.X, player.velocity.Y, 0, default(Color), 1.2f);
						Main.dust[num216].velocity.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.dust[num216].velocity.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.dust[num216].velocity.X += (float)Main.rand.Next(-50, 51) * 0.05f;
						Main.dust[num216].velocity.Y += (float)Main.rand.Next(-50, 51) * 0.05f;
						Main.dust[num216].scale *= 1f + (float)Main.rand.Next(-30, 31) * 0.01f;
					}
					if (Main.rand.Next(40) == 0)
					{
						int type10 = Main.rand.Next(276, 283);
						int num217 = Gore.NewGore(new Vector2(hitbox.X, hitbox.Y), player.velocity, type10);
						Main.gore[num217].velocity.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.gore[num217].velocity.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.gore[num217].scale *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
						Main.gore[num217].velocity.X += (float)Main.rand.Next(-50, 51) * 0.05f;
						Main.gore[num217].velocity.Y += (float)Main.rand.Next(-50, 51) * 0.05f;
					}
				}
				else if (player.meleeEnchant == 8 && Main.rand.Next(4) == 0)
				{
					int num218 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 46, 0f, 0f, 100);
					Main.dust[num218].noGravity = true;
					Main.dust[num218].fadeIn = 1.5f;
					Main.dust[num218].velocity *= 0.25f;
				}
			}
			if (player.magmaStone && item.melee && !item.noMelee && !item.noUseGraphic && Main.rand.Next(3) != 0)
			{
				int num219 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 6, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 2.5f);
				Main.dust[num219].noGravity = true;
				Main.dust[num219].velocity.X *= 2f;
				Main.dust[num219].velocity.Y *= 2f;
			}

			#endregion

			if (modstuff) {
				ItemLoader.MeleeEffects(item, player, hitbox);
				PlayerHooks.MeleeEffects(player, item, hitbox);
			}
		}
		/// <summary>
		/// OnHitNPC for item, can be found in ItemCheck method at Player.cs
		/// Plus some of it are getting id
		/// </summary>
		public static void OnHitNPC(Item item,Player player,NPC npc,Rectangle r2,int damage,float knockBack,bool modstuff = true,bool crit = false) {
			bool isTargetMortal = !npc.immortal;

			#region Specific item type check

			if (item.type == ItemID.TheHorsemansBlade && (npc.value > 0f || (npc.damage > 0 && !npc.friendly))){
				PumpkinSword(player,npc.position, (int)((double)damage * 1.5), knockBack,321,npc.whoAmI);
			}
			if (item.type == ItemID.Bladetongue){
				Vector2 velocity = new Vector2(player.direction * 100 + Main.rand.Next(-25, 26), Main.rand.Next(-75, 76));
				velocity.Normalize();
				velocity *= (float)Main.rand.Next(30, 41) * 0.1f;
				Vector2 pos = new Vector2(r2.X + Main.rand.Next(r2.Width), r2.Y + Main.rand.Next(r2.Height));
				pos = (pos + npc.Center * 2f) / 3f;
				Projectile.NewProjectile(pos, velocity, 524, (int)((double)damage * 0.7), knockBack * 0.7f, player.whoAmI);
			}
			if (item.type == ItemID.PsychoKnife){
				player.stealth = 1f;
				if (Main.netMode == 1){
					NetMessage.SendData(84, -1, -1, null, player.whoAmI);
				}
			}
			if (item.type == ItemID.BeeKeeper && isTargetMortal){
				int count = Main.rand.Next(1, 4);
				if (player.strongBees && Main.rand.Next(3) == 0){count++;}
				for (int i = 0; i < count; i++){
					float speedX = (float)(player.direction * 2) + (float)Main.rand.Next(-35, 36) * 0.02f;
					float speedY = (float)Main.rand.Next(-35, 36) * 0.02f;
					speedX *= 0.2f;
					speedY *= 0.2f;
					Projectile.NewProjectile(r2.X + r2.Width / 2, r2.Y + r2.Height / 2, speedX, speedY, player.beeType(), player.beeDamage(damage / 3), player.beeKB(0f), i);
				}
			}

			#endregion

			#region Player field check

			if (player.beetleOffense && isTargetMortal)
			{
				player.beetleCounter += damage;
				player.beetleCountdown = 0;
			}
			if (player.meleeEnchant == 7){
				Projectile.NewProjectile(npc.Center, npc.velocity, 289, 0, 0f, player.whoAmI);
			}
			if (npc.value > 0f && player.coins && Main.rand.Next(5) == 0){
				int type12 = 71;
				if (Main.rand.Next(10) == 0){type12 = 72;}
				if (Main.rand.Next(100) == 0){type12 = 73;}

				int num240 = Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, type12);
				Main.item[num240].stack = Main.rand.Next(1, 11);
				Main.item[num240].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
				Main.item[num240].velocity.X = (float)Main.rand.Next(10, 31) * 0.2f * (float)player.direction;

				if (Main.netMode == 1){
					NetMessage.SendData(21, -1, -1, null, num240);
				}
			}

			#endregion

			if (modstuff) {
				ItemLoader.OnHitNPC(item, player, npc, damage, knockBack, crit);
				NPCLoader.OnHitByItem(npc, player, item, damage, knockBack, crit);
				PlayerHooks.OnHitNPC(player, item, npc, damage, knockBack, crit);
			}
		}
		/// <summary>
		/// Pumpkin sword method from Player.cs
		/// Rewrited aaaaah my hand hurt when typing this uhhh awa 
		/// what the hell happened to my hands help what
		/// ok. seems like it stopped after 1 minute. i should stop writing codes for a while now :pe:
		/// </summary>
		public static int PumpkinSword(Player player,Vector2 pos, int dmg, float kb,int type = 321,int i = 0){
			int logicCheckScreenHeight = Main.LogicCheckScreenHeight;
			int logicCheckScreenWidth = Main.LogicCheckScreenWidth;
			int x = Main.rand.Next(100, 300);
			int y = Main.rand.Next(100, 300);
			x = ((Main.rand.Next(2) != 0) ? (x + (logicCheckScreenWidth / 2 - x)) : (x - (logicCheckScreenWidth / 2 + x)));
			y = ((Main.rand.Next(2) != 0) ? (y + (logicCheckScreenHeight / 2 - y)) : (y - (logicCheckScreenHeight / 2 + y)));
			x += (int)player.position.X;
			y += (int)player.position.Y;
			float speedX = pos.X - x;
			float speedY = pos.Y - y;
			float speedFactor = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
			speedFactor = 8f / speedFactor;
			speedX *= speedFactor;
			speedY *= speedFactor;
			return Projectile.NewProjectile(x, y, speedX, speedY, type, dmg, kb, player.whoAmI, i);
		}
	}
}
