
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Deltarune.Content.UI;
using Deltarune.Helper;
using Deltarune.Content.Spell;

namespace Deltarune
{
	// This is where hitbox got manipulated 
	public class SoulHandler : ILoadable , IPreSaveAndQuit
	{
		public struct PlayerHitboxData
		{
			public int whoAmI;
			public int width;
			public int height;
			public Vector2 pos;
			public Vector2 velocity;
			public float gfxOffY;
			public PlayerHitboxData(int whoAmI,int width,int height,Vector2 pos,float gfxOffY,Vector2 velocity) {
				this.whoAmI = whoAmI;
				this.width = width;
				this.height = height;
				this.pos = pos;
				this.gfxOffY = gfxOffY;
				this.velocity = velocity;
			}
		}

		public static List<PlayerHitboxData> playerThatHasNoSoulLmao;
		public static List<int> needDraw;
		public static float drawAlpha;

		public static void DrawBorder(SpriteBatch spriteBatch,Player player) {
			if (drawAlpha <= 0f) return;
			int length = 200;
			drawAlpha += 0.1f;
			int borderLength = (int)((float)length*drawAlpha);
			Vector2 border = player.Center - new Vector2(borderLength,borderLength) - Main.screenPosition;
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)border.X, (int)border.Y, borderLength*2, borderLength*2), Color.Black*0.8f);
			spriteBatch.DrawBorderedRect(border,new Vector2(borderLength*2,borderLength*2),Color.White,5);
		}

		public static void Draw(SpriteBatch spriteBatch) {
			if (needDraw != null && needDraw.Count > 0) {
				Texture2D texture = ModContent.GetTexture(Deltarune.textureExtra+"Heart");
				foreach (var i in needDraw){
					Player player = Main.player[i];
					if (player.active && !player.dead) {
						// draw border clientside
						if (i == Main.myPlayer) {
							drawAlpha += 0.1f;
							if (drawAlpha > 1f) {drawAlpha = 1f;}
						}
						Vector2 pos = player.GetDelta().soul;
						spriteBatch.Draw(texture, pos - Main.screenPosition, null, Color.White, 0f, texture.Size()/2f, 1f, SpriteEffects.None, 0);
					}
				}
				spriteBatch.BeginGlow(true);
				texture = ModContent.GetTexture(Deltarune.textureExtra+"Heart_glow1");
				foreach (var i in needDraw){
					Player player = Main.player[i];
					if (player.active && !player.dead) {
						spriteBatch.Draw(texture, player.GetDelta().soul - Main.screenPosition, null, Color.White, 0f, texture.Size()/2f, 1f, SpriteEffects.None, 0);
					}
				}
				spriteBatch.BeginNormal(true);
			}
			needDraw.Clear();
		}
		public static void Update(Player player , DeltaPlayer p) {
			if (player.active && !player.dead) {
				if (p.soulTimer > 0) {
					p.soulTimer--;
					if (p.soulTimer == 0) {
						for (int i = 0; i < 15; i++) {
							Vector2 vel = Main.rand.NextVector2CircularEdge(1f, 1f);
							Dust.NewDustPerfect(p.soul, 114, vel * Main.rand.NextFloat(2f), 1);
						}
						for (int i = 0; i < 15; i++) {
							Vector2 vel = Main.rand.NextVector2CircularEdge(1f, 1f);
							Dust.NewDustPerfect(player.Center, 114, vel * Main.rand.NextFloat(2f), 1);
						}
						player.Center.DustLine(p.soul,114,true);
					}
					p.originalBody = player.Center;
					playerThatHasNoSoulLmao.Add(new PlayerHitboxData(player.whoAmI,player.width,player.height,player.Center,player.gfxOffY,player.velocity));
					player.width = 10;
					player.height = 10;
					player.Center = p.soul;
					player.invis = true;
				}
			}
		}
		public static void PositionUpdate(Player player,DeltaPlayer p) {
			if (p.soulTimer > 0 && p.soul != Vector2.Zero) {
				// jesse, we need to do math jesse
				if (player.controlRight) {p.soul.X += 2f;}
				if (player.controlLeft) {p.soul.X -= 2f;}
				if (player.controlDown) {p.soul.Y += 2f;}
				if (player.controlJump || player.controlUp) {p.soul.Y -= 2f;}
				player.stepSpeed = 0f;
				player.velocity = Vector2.Zero;
				player.Center = p.soul;
				player.itemAnimation = 0;
			}
			else {
				p.soul = player.Center;
				p.originalBody = player.Center;
				drawAlpha = 0f;
			}
		}
		public static void Reset() {
			// safe code baby
			if (playerThatHasNoSoulLmao != null && playerThatHasNoSoulLmao.Count > 0) {
				foreach (var item in playerThatHasNoSoulLmao){
					needDraw.Add(item.whoAmI);
					Player player = Main.player[item.whoAmI];
					player.gfxOffY = item.gfxOffY;
					player.width = item.width;
					player.height = item.height;
					player.Center = item.pos;
					player.velocity = item.velocity;
				}
			}
			playerThatHasNoSoulLmao.Clear();
		}
		public void PreSaveAndQuit() {
			playerThatHasNoSoulLmao = new List<PlayerHitboxData>();
			needDraw = new List<int>();
			drawAlpha = 0f;
		}
		public void Load() {
			PreSaveAndQuit();
			On.Terraria.Player.QuickGrapple += GrapplePrevent;
			On.Terraria.Player.QuickMount += MountPrevent;
			On.Terraria.Player.FindPulley += PulleyPrevent;
		}
		public void Unload() {
			playerThatHasNoSoulLmao = null;
			needDraw = null;
			On.Terraria.Player.QuickGrapple -= GrapplePrevent;
			On.Terraria.Player.QuickMount -= MountPrevent;
			On.Terraria.Player.FindPulley -= PulleyPrevent;
		}
		static void PulleyPrevent(On.Terraria.Player.orig_FindPulley orig, Player player) {
			if (player.GetDelta().soulTimer > 0) {return;}
			orig(player);
		}
		static void MountPrevent(On.Terraria.Player.orig_QuickMount orig,Player player) {
			if (player.GetDelta().soulTimer > 0) {player.frozen = true;}
			orig(player);
			if (player.GetDelta().soulTimer > 0) {player.frozen = false;}
		}
		static void GrapplePrevent(On.Terraria.Player.orig_QuickGrapple orig,Player player) {
			if (player.GetDelta().soulTimer > 0) {return;}
			orig(player);
		}
	}
}
