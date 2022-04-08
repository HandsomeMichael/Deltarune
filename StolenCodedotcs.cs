using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.UI.Chat;
using Terraria.UI;
using Terraria.ModLoader;
using ReLogic.Graphics;
using Deltarune.Helper;
using Deltarune.Content;
using Deltarune.Content.Items;
using Deltarune.Content.NPCs;
using Deltarune.Content.NPCs.Boss;
using System.Threading;
using Terraria.ModLoader.Audio;
using Terraria.ModLoader.Config;
using Microsoft.Xna.Framework.Input;
//using On.Terraria;

namespace Deltarune
{
	// referenced from https://github.com/ProjectStarlight/StarlightRiver/blob/master/Compat/BossChecklist/PortraitPatch.cs
	// patches : added reflection , and also error check to prevent bosschecklist error
	public interface IBossInfo{
		void SendBossInfo(Mod mod);
	}
	public class BossChecklistPatch
	{
		public const float KingSlime = 1f;
		public const float EyeOfCthulhu = 2f;
		public const float EaterOfWorlds = 3f;
		public const float QueenBee = 4f;
		public const float Skeletron = 5f;
		public const float WallOfFlesh = 6f;
		public const float TheTwins = 7f;
		public const float TheDestroyer = 8f;
		public const float SkeletronPrime = 9f;
		public const float Plantera = 10f;
		public const float Golem = 11f;
		public const float DukeFishron = 12f;
		public const float LunaticCultist = 13f;
		public static void Add(IBossInfo info) {
			bossInfo.Add(info);
		}
		public static List<IBossInfo> bossInfo;
		public static void Init() => bossInfo = new List<IBossInfo>();
		public static void Load(){
			Mod mod = ModLoader.GetMod("BossChecklist");
			if (mod == null) return;
			Deltarune.Log("Bosschecklist detected , sending boss info ");
			if (bossInfo == null) {
				Deltarune.Log("Bosschecklist error, couldnt get any boss info, try reloading mods");
				return;
			}
			foreach (var item in bossInfo){item.SendBossInfo(mod);}
			bossInfo = null;

			Deltarune.Log("Bosschecklist , applying patches");

			var typeInfo = mod.Code.GetType("BossChecklist.UIElements.BossLogUIElements").GetNestedType("BossLogPanel", BindingFlags.NonPublic);
			if (typeInfo == null) {Deltarune.Log("Bosschecklist Couldnt get typeInfo from boss checklist, skipped");return;}
			method = typeInfo.GetMethod("Draw", BindingFlags.Public | BindingFlags.Instance);
			if (method == null) {Deltarune.Log("Bosschecklist Couldnt get method from boss checklist, skipped");return;}

			applyPatchDrawString = true;
			patchDrawString = false;

			SetupGetBossInfos(mod);

			if (success) {Deltarune.Log("Bosschecklist Reflection loaded succesfully, loading il edit");}
			else {Deltarune.Log("Bosschecklist Reflection failed to load. using manual texture check , loading il edit");}

			On.Terraria.Utils.DrawBorderString += CoolNameLol;
			HookEndpointManager.Modify(method, new ILContext.Manipulator(PatchDraw));
			Deltarune.Log("Bosschecklist patch complete");
		}
		public static MethodInfo method;
		public static void Unload() {
			HookEndpointManager.Unmodify(method, new ILContext.Manipulator(PatchDraw));
			On.Terraria.Utils.DrawBorderString -= CoolNameLol;
			field = null;
			method = null;
		}
		public static void Reset() {
			if (patchDrawString) {
				applyPatchDrawString = false;
				Deltarune.Log("Bosschecklist Error detected, patching unsuccess. disabling text patch");
				if (!Main.gameMenu) {
					Main.NewText("[Deltarune] sorry but there is some error lol, just ignore this and keep playing",Color.Pink);
				}
				patchDrawString = false;
			}
		}
		public static void PatchDraw(ILContext il){
			var c = new ILCursor(il);
			c.TryGotoNext(n => n.MatchStloc(107)); //Color3
			c.Index++;
			c.Emit(OpCodes.Ldloc, 92);
			c.Emit(OpCodes.Ldarg_0);
			c.EmitDelegate<Action<Texture2D, UIElement>>(newDraw);
		}
		public static bool patchDrawString;
		public static bool applyPatchDrawString;
		public static void newDraw(Texture2D drawnTexture, UIElement self)
		{
			var spriteBatch = Main.spriteBatch;

			string modSource = "";
			List<int> npcIDs = null;
			int type = -1;
			if (success) {GetBossInfos(out modSource,out npcIDs);}
			else {
				if (drawnTexture == ModContent.GetTexture(Deltarune.textureExtra+"Boss_starwalker")) {
					type = 1;
				}
			}
			//if (drawnTexture == ModContent.GetTexture("StarlightRiver/Assets/BossChecklist/VitricBoss"))

			if ((modSource != "Deltarune" || npcIDs == null) || type != -1) return;

			if (success) {
				if (npcIDs.Contains(ModContent.NPCType<starwalker>())) {type = 1;}
			}

			if (type == 1){
				patchDrawString = true;
				float sin = 0.6f + (float)Math.Sin(Main.GameUpdateCount / 100f) * 0.3f;
				var tex = ModContent.GetTexture(Deltarune.textureExtra+"Boss_starwalker_glow");
				var tex2 = ModContent.GetTexture(Deltarune.textureExtra+"myballs");
				Vector2 pos = self.GetDimensions().Center();
				Rectangle rec = self.GetDimensions().ToRectangle();
				spriteBatch.BeginImmediate(true,true);
				GameShaders.Misc["WaveWrap"].UseOpacity((float)Main.GameUpdateCount/500f).Apply();
				spriteBatch.Draw(tex2, rec, null, Color.Black * 0.6f, 0, Vector2.UnitY * 2, 0, 0);
				spriteBatch.BeginGlow(true,true);
				spriteBatch.Draw(tex, pos, null, Color.White * sin, 0,tex.Size()/2f, 1, 0, 0);
				spriteBatch.BeginNormal(true,true);
			}
		}
		public static FieldInfo[] field;
		public static bool success;
		public static void SetupGetBossInfos(Mod mod) {
			// reflection doom
			field = new FieldInfo[5];
			success = false;
			
			Type type = mod.Code.GetType("BossChecklist.BossLogUI");
			if (type == null) {Deltarune.Log("Reflection error type 1");return;}
			field[0] = type.GetField("PageNum", BindingFlags.Public | BindingFlags.Static);
			if (field[0] == null) {Deltarune.Log("Reflection error field 0");return;}

			Type type2 = mod.Code.GetType("BossChecklist.BossChecklist");
			if (type2 == null) {Deltarune.Log("Reflection error type 2");return;}
			field[1] = type2.GetField("bossTracker",BindingFlags.NonPublic | BindingFlags.Static);
			if (field[1] == null) {Deltarune.Log("Reflection error field 1");return;}

			Type type3 = mod.Code.GetType("BossChecklist.BossTracker");
			if (type3 == null) {Deltarune.Log("Reflection error type 3");return;}
			field[2] = type3.GetField("SortedBosses",BindingFlags.NonPublic | BindingFlags.Instance);
			if (field[2] == null) {Deltarune.Log("Reflection error type 2");return;}

			Type type4 = mod.Code.GetType("BossChecklist.BossInfo");
			if (type4 == null) {Deltarune.Log("Reflection error type 4");return;}
			field[3] = type4.GetField("modSource",BindingFlags.NonPublic | BindingFlags.Instance);
			field[4] = type4.GetField("npcIDs",BindingFlags.NonPublic | BindingFlags.Instance);
			if (field[3] == null) {Deltarune.Log("Reflection error field 3");return;}
			if (field[4] == null) {Deltarune.Log("Reflection error field 4");return;}
			success = true;
		}
		public static void GetBossInfos(out string modSource,out List<int> npcIDs) {
			npcIDs = null;
			modSource = "unknown";
			Mod mod = ModLoader.GetMod("BossChecklist");
			if (field == null) return;
			int PageNum = (int)field[0].GetValue(null);
			object bossTracker = field[1].GetValue(mod);
			object SortedBossesList = field[2].GetValue(bossTracker);
			if (SortedBossesList == null) {
				Main.NewText("failed to get boss bosseslist");
				success = false;
				return;
			}
			IList SortedBosses = (IList)SortedBossesList;
			// prevent index out of bounds
			bool flag = PageNum >= 0;
			if (!flag) return;
			object bossInfo = SortedBosses[PageNum];
			modSource = (string)field[3].GetValue(bossInfo);
			npcIDs = (List<int>)field[4].GetValue(bossInfo);
			// the goal of this reflection is to get the current boss info :
			//BossInfo selectedBoss = BossChecklist.bossTracker.SortedBosses[BossLogUI.PageNum];
		}
		public static Vector2 CoolNameLol(On.Terraria.Utils.orig_DrawBorderString orig,SpriteBatch sb, 
		string text, Vector2 pos, Color color, float scale , float anchorx , float anchory, int maxCharactersDisplayed ) {
			if (patchDrawString && applyPatchDrawString && text == "Starwalker") {
				sb.BeginDyeShader(ItemID.SolarDye, new Item(),true,true);
			}
			Vector2 piss = Vector2.Zero;
			if (orig != null) {piss = orig(sb,text,pos,color,scale,anchorx,anchory,maxCharactersDisplayed);}
			if (patchDrawString && applyPatchDrawString && text == "Starwalker") {
				sb.BeginNormal(true,true);
				patchDrawString = false;
			}
			return piss;
		}
	}
}
