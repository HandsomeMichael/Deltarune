using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Deltarune;
using Deltarune.Content.Projectiles;
using Deltarune.Helper;
using Terraria.UI.Chat;
using Terraria.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace Deltarune.Content.NPCs.Boss
{
	public class Sanesss : ModNPC
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Sanesss");
			Main.npcFrameCount[npc.type] = 4;
		}
		public override void SetDefaults() {
			npc.width = 100;
			npc.height = 100;
			npc.aiStyle = -1;
			npc.damage = 1000;
			npc.value = 1000;
			npc.boss = false;
			npc.lifeMax = 1000;
			npc.friendly = true;
			npc.knockBackResist = 0f;
			npc.defense = 70;
			npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/snd_hurt1");
			npc.DeathSound = SoundID.NPCDeath2;
			npc.scale = 0.5f;
		}
		public override Color? GetAlpha(Color drawColor) => Color.White;
		public override float SpawnChance(NPCSpawnInfo spawnInfo) => (Main.dayTime && (NPC.FindFirstNPC(npc.type) > -1)) ? 0f : 0.0000069f;
		public override void FindFrame(int frameHeight) {
			npc.frameCounter += 0.40f;
			npc.frameCounter %= Main.npcFrameCount[npc.type];
			int frame = (int)npc.frameCounter;
			npc.frame.Y = frame * frameHeight;
		}
		public override void AI() {
			npc.TargetClosest();
			Player player = Main.player[npc.target];
			if (npc.boss) {
				npc.noGravity = true;
				npc.noTileCollide = true;
				if (!player.active || player.dead) {
					npc.velocity.X = 0;
					npc.velocity.Y = -5;
					npc.rotation = 0f;
					if (npc.ai[0] != -1f) {
						Main.NewText("You suck, you bad hahaha suck",Color.Pink);
						npc.GetDelta().textOverhead = new TypeWriter("You suck, you bad hahaha suck",SoundID.Item16,4);
					}
					npc.ai[0] = -1f;
					return;
				}
				//if (player.velocity.Y < -5f) {player.velocity.Y = -5f;}
				//if (player.velocity.Y == 0f) {player.velocity.Y = 1f;}
				//else {player.velocity.X = 0f;}
				npc.velocity = Vector2.Lerp(npc.velocity,npc.DirectionTo(player.Center)*5f,0.1f);
				npc.ai[0]++;
				npc.ai[1]++;
				if (npc.ai[1] == 120f ) {
					if (Main.netMode != NetmodeID.MultiplayerClient) {
						Explode.New(npc.Center);
						Projectile.NewProjectile(npc.Center,npc.DirectionTo(player.Center)*20f,ProjectileID.Boulder,100,1,Main.myPlayer);
					}
				}
				if (npc.ai[1] > 200f) {
					if (Main.netMode != NetmodeID.MultiplayerClient) {
						int numberProjectiles = 4 + Main.rand.Next(2); // 4 or 5 shots
						Explode.New(npc.Center);
						for (int i = 0; i < numberProjectiles; i++)
						{
							Vector2 perturbedSpeed = (npc.DirectionTo(player.Center)*20f).RotatedByRandom(MathHelper.ToRadians(30)); // 30 degree spread.
							int a = Projectile.NewProjectile(npc.Center,perturbedSpeed,ProjectileID.Bone,69,1,Main.myPlayer);
							Main.projectile[a].hostile = true;
							Main.projectile[a].friendly = false;
							Main.projectile[a].tileCollide = false;
						}
					}
					npc.ai[1] = 0f;
				}
				if (npc.ai[0] > 60f) {
					if (Main.netMode != NetmodeID.MultiplayerClient) {
						int a = Projectile.NewProjectile(npc.Center,npc.DirectionTo(player.Center)*14f,ProjectileID.Bone,69,1,Main.myPlayer);
						Main.projectile[a].hostile = true;
						Main.projectile[a].friendly = false;
						Main.projectile[a].tileCollide = false;
					}
					npc.ai[0] = 0f;
				}
			}
			else if ((!player.active || player.dead) && Vector2.Distance(player.Center,npc.Center) > 500f) {music = -1;}

			npc.rotation = npc.velocity.X * 0.08f;
		}
		public override void SetChatButtons(ref string button, ref string button2) {
			button = "I did ur mom";
		}
		public override void OnChatButtonClicked(bool firstButton, ref bool shop) {
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player player = Main.player[i];
				if (player.active && !player.dead) {
					if (Vector2.Distance(player.Center,npc.Center) < 500f) {
						player.velocity = npc.DirectionTo(player.Center)*25f;
					}
				}
			}
			npc.GetDelta().textOverhead = new TypeWriter("Gaeming Time",SoundID.Item16,4);
			npc.boss = true;
			npc.friendly = false;
		}
		public override bool CanChat() => true;
		public override string GetChat() {
			string doin = "doin ur mom , doin ur mom , doin ur mom , doin ur mom , doin ur mom , doin ur mom , doin ur mom,doin ur mom , doin ur mom , doin ur mom , doin ur mom , doin ur mom , doin ur mom , doin ur mom";
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/sanss");
			Deltarune.chatNPC = new TypeWriter(doin,SoundID.Item16,4);
			return doin;
		}
	}
}

