
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
	/*

    I will be bacc and actualy implement this

    public enum DrawStage {
        BackGround,
        NPC,
        Tiles,
        Projectile,
        Dust,
        Player,
        Soul,
        UI
    }
    public enum DrawContext{
        PrePreDraw,
        PreDraw,
        AdditivePreDraw,
        Draw,
        AdditiveDraw,
        PostDraw,
        AdditivePostDraw,
        PostPostDraw
    }
    public interface IDrawable{
        Action<SpriteBatch> GetDrawAction(DrawContext context, DrawStage stage,bool requireReset);
    }

	Has a lot of errors, revisited soon when i am bored

	public interface IDrawAdditive
    {
        void AdditiveDraw(SpriteBatch spriteBatch,bool behind);
    }
    public interface IPreDrawAdditive
    {
        void AdditivePreDraw(SpriteBatch spriteBatch,bool behind);
    }
    public interface IPostDrawAdditive
    {
        void AdditivePostDraw(SpriteBatch spriteBatch,bool behind);
    }
	public class AdditiveHandler : DeltaSystem
	{
		public static List<int> PostDrawAdditiveNPC;
		public static List<int> PostDrawAdditiveProj;

		public static List<int> PreDrawAdditiveNPC;
		public static List<int> PreDrawAdditiveProj;

		public static List<int> DrawAdditiveNPC;
		public static List<int> DrawAdditiveProj;
		public static void Proj(Projectile projectile) {
			if (projectile.modProjectile != null) {
				if (projectile.modProjectile is IDrawAdditive) {
					DrawAdditiveProj.Add(projectile.whoAmI);
				}
				if (projectile.modProjectile is IPostDrawAdditive) {
					PostDrawAdditiveProj.Add(projectile.whoAmI);
				}
				if (projectile.modProjectile is IPreDrawAdditive) {
					PreDrawAdditiveProj.Add(projectile.whoAmI);
				}
			}
		}
		public static void NPC(NPC npc) {
			if (npc.modNPC != null) {
				if (npc.modNPC is IDrawAdditive) {
					DrawAdditiveProj.Add(npc.whoAmI);
				}
				if (npc.modNPC is IPostDrawAdditive) {
					PostDrawAdditiveProj.Add(npc.whoAmI);
				}
				if (npc.modNPC is IPreDrawAdditive) {
					PreDrawAdditiveNPC.Add(npc.whoAmI);
				}
			}
		}
		static void DrawProjectilesPatch(On.Terraria.Main.orig_DrawProjectiles orig,Main self) {
			if (PreDrawAdditiveProj != null && PreDrawAdditiveProj.Count > 0) {
				Main.spriteBatch.BeginNormal();
				foreach (int index in PreDrawAdditiveProj){
					Projectile proj = Main.projectile[index];
					if (proj.active && proj.modProjectile != null && proj.modProjectile is IPreDrawAdditive draw) {
						draw.AdditivePreDraw(Main.spriteBatch,false);
					}
				}
				Main.spriteBatch.End();
			}
			if (DrawAdditiveProj != null && DrawAdditiveProj.Count > 0) {
				Main.spriteBatch.BeginGlow();
				foreach (int index in DrawAdditiveProj){
					Projectile proj = Main.projectile[index];
					if (proj.active && proj.modProjectile != null && proj.modProjectile is IDrawAdditive draw) {
						draw.AdditiveDraw(Main.spriteBatch,false);
					}
				}
				Main.spriteBatch.End();
			}
			DrawAdditiveProj = new List<int>();
			orig(self);
			if (PostDrawAdditiveProj != null && PostDrawAdditiveProj.Count > 0) {
				Main.spriteBatch.BeginGlow();
				foreach (int index in PostDrawAdditiveProj){
					Projectile proj = Main.projectile[index];
					if (proj.active && proj.modProjectile != null && proj.modProjectile is IPostDrawAdditive draw) {
						draw.AdditivePostDraw(Main.spriteBatch,false);
					}
				}
				Main.spriteBatch.End();
			}
			PostDrawAdditiveProj = new List<int>();
		}
		static void DrawNPCsPatch(On.Terraria.Main.orig_DrawNPCs orig,Main self,bool behindTiles) {
			if (PreDrawAdditiveNPC != null && PreDrawAdditiveNPC.Count > 0) {
				foreach (int index in PreDrawAdditiveNPC){
					var npc = Main.npc[index];
					if (npc.active && npc.modNPC != null && npc.modNPC is IPreDrawAdditive draw) {
						draw.AdditivePreDraw(Main.spriteBatch,behindTiles);
					}
				}
			}
			if (DrawAdditiveNPC != null && DrawAdditiveNPC.Count > 0) {
				Main.spriteBatch.BeginGlow(true);
				foreach (int index in DrawAdditiveNPC){
					var npc = Main.npc[index];
					if (npc.active && npc.modNPC != null && npc.modNPC is IDrawAdditive draw) {
						draw.AdditiveDraw(Main.spriteBatch,behindTiles);
					}
				}
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
			}
			DrawAdditiveNPC = new List<int>();
			orig(self,behindTiles);
			if (PostDrawAdditiveNPC != null && PostDrawAdditiveNPC.Count > 0) {
				Main.spriteBatch.BeginGlow(true);
				foreach (int index in PostDrawAdditiveNPC){
					var npc = Main.npc[index];
					if (npc.active && npc.modNPC != null && npc.modNPC is IPostDrawAdditive draw) {
						draw.AdditivePostDraw(Main.spriteBatch,behindTiles);
					}
				}
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
			}
			PostDrawAdditiveNPC = new List<int>();
		}

		public override void Load() {
			PreDrawAdditiveNPC = new List<int>();
			PreDrawAdditiveProj = new List<int>();
			PostDrawAdditiveNPC = new List<int>();
			PostDrawAdditiveProj = new List<int>();
			DrawAdditiveProj = new List<int>();
			DrawAdditiveNPC = new List<int>();
			On.Terraria.Main.DrawNPCs += DrawNPCsPatch;
			On.Terraria.Main.DrawProjectiles += DrawProjectilesPatch;
		}
		public override void Unload() {
			On.Terraria.Main.DrawNPCs -= DrawNPCsPatch;
			On.Terraria.Main.DrawProjectiles -= DrawProjectilesPatch;
			PreDrawAdditiveNPC = null;
			PreDrawAdditiveProj = null;
			PostDrawAdditiveNPC = null;
			PostDrawAdditiveProj = null;
			DrawAdditiveProj = null;
			DrawAdditiveNPC = null;
		}
	}
	*/
}
