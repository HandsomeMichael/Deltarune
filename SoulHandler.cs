
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

		public static List<PlayerHitboxData> playerThatHasNoSoulLmao = new List<PlayerHitboxData>();

		public static void Update() {
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
			// safe code bc imma do it in detour hook
			if (playerThatHasNoSoulLmao != null && playerThatHasNoSoulLmao.Count > 0) {
				foreach (var item in playerThatHasNoSoulLmao){
					Player player = Main.player[item.whoAmI];
					player.width = item.width;
					player.height = item.height;
					player.Center = item.pos;
				}
			}
			playerThatHasNoSoulLmao = new List<PlayerHitboxData>();
		}
		public void Load() {
			playerThatHasNoSoulLmao = new List<PlayerHitboxData>();
		}
		public void Unload() {
			playerThatHasNoSoulLmao = null;
		}
	}
}
