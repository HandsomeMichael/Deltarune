
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
	public class SoulHandler : ILoadable
	{
		public struct PlayerHitboxData
		{	
			public int whoAmI;
			public int width;
			public int height;
			public Vector2 pos;

			public PlayerHitboxData(int whoAmI,int width,int height,Vector2 pos) {
				this.whoAmI = whoAmI;
				this.width = width;
				this.height = height;
				this.pos = pos;
			}
		}

		public static List<PlayerHitboxData> playerThatHasNoSoulLmao;
		public static List<int> needDraw;
		// something something crashes if you dont make it update atleast once
		public static bool atLeastUpdatedOnce;

		public static void Draw(SpriteBatch spriteBatch) {
			if (needDraw != null && needDraw.Count > 0) {
				Texture2D texture = ModContent.GetTexture(Deltarune.textureExtra+"Heart");
				foreach (var i in needDraw){
					Player player = Main.player[i];
					if (player.active && !player.dead) {
						Vector2 pos = player.GetDelta().soul;
						Utils.DrawLine(spriteBatch, player.Center, pos, Color.Red,Color.Red, (pos.Distance(player.Center)/300f)*3f);
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
		public static void Update() {
			if (!atLeastUpdatedOnce) return;
			for (int i = 0; i < Main.maxPlayers; i++){
				Player player = Main.player[i];
				if (player.active && !player.dead) {
					if (player.GetDelta().soulTimer > 0) {
						player.GetDelta().soulTimer--;
						playerThatHasNoSoulLmao.Add(new PlayerHitboxData(player.whoAmI,player.width,player.height,player.Center));
						player.width = 10;
						player.height = 10;
						player.Center = player.GetDelta().soul;
					}
				}
			}
		}
		public static void Reset() {
			if (!atLeastUpdatedOnce) return;
			// safe code bc imma do it in detour hook
			if (playerThatHasNoSoulLmao != null && playerThatHasNoSoulLmao.Count > 0) {
				foreach (var item in playerThatHasNoSoulLmao){
					needDraw.Add(item.whoAmI);
					Player player = Main.player[item.whoAmI];
					//player.Center.DustLine(item.pos,100,true);
					player.width = item.width;
					player.height = item.height;
					player.Center = item.pos;
				}
			}
			playerThatHasNoSoulLmao.Clear();
			atLeastUpdatedOnce = true;
		}
		public void Load() {
			atLeastUpdatedOnce = false;
			playerThatHasNoSoulLmao = new List<PlayerHitboxData>();
			needDraw = new List<int>();
		}
		public void Unload() {
			playerThatHasNoSoulLmao = null;
			needDraw = null;
		}
	}
}
