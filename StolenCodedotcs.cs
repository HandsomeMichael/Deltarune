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
	// There is a lot of changes from there
	// i only take the il edit because i have absouletly no idea how to il edit

	// the interface used for custom boss drawing
	public interface IBossInfoExtra {
		void CustomDraw(SpriteBatch spritebatch,UIElement ui);
		bool TypeCheck(List<int> type);
		string GetInfoTexture();
	}
	// the interface used to send boss info to boss checklist mod
	public interface IBossInfo{
		object SendBossInfo(Mod mod,Mod caller);
	}
	// the class that will handle all boss checklist related patch
	public class BossChecklistPatch
	{
		// const of progression
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

		// handles interfaces loading
		public static void HandleInterface(Type type,Type[] interfaces,Mod mod) {
			if (interfaces.Contains(typeof(IBossInfo))) {
				IBossInfo instance = (IBossInfo)Activator.CreateInstance(type);
				bossInfo.Add(instance);
			}
			if (interfaces.Contains(typeof(IBossInfoExtra))) {
				IBossInfoExtra instance = (IBossInfoExtra)Activator.CreateInstance(type);
				bossInfoExtra.Add(instance);
			}
		}
		// boss infos , only contains method to send info to bc
		public static List<IBossInfo> bossInfo;
		// boss info extra, contains draw method
		public static List<IBossInfoExtra> bossInfoExtra;
		// method for il edit
		public static MethodInfo method;
		// array of field, used for getting boss infos fields
		public static FieldInfo[] field;
		// check if the reflection is succeed
		// if it doesnt then will return to the old texture checking way
		public static bool success;

		// initialize lists
		public static void Init() {
			bossInfo = new List<IBossInfo>();
			bossInfoExtra = new List<IBossInfoExtra>();
		}

		// load the system, this is seperate from ILoadable
		public static void Load(Mod caller){
			// get bc mod
			Mod mod = ModLoader.GetMod("BossChecklist");
			// if no bc then dont do patches
			if (mod == null) return;
			// log p much everything
			Deltarune.Log("Bosschecklist detected , sending boss info ");
			if (bossInfo == null) {
				Deltarune.Log("Bosschecklist error, couldnt get any boss info, try reloading mods");
				return;
			}
			// loop over bossinfo and send infos
			foreach (var item in bossInfo){
				string name = item.GetType().Name;
				object call = item.SendBossInfo(mod,caller);
				if (call is string p) {if (p != "Success")Deltarune.Log($"Bosschecklist {name} info failed to load");}
				else {Deltarune.Log($"Bosschecklist {name} info failed to load");}
			}

			// reset boss info
			bossInfo = null;

			// apply patches , if failed then just return
			Deltarune.Log("Bosschecklist , applying patches");
			var typeInfo = mod.Code.GetType("BossChecklist.UIElements.BossLogUIElements").GetNestedType("BossLogPanel", BindingFlags.NonPublic);
			if (typeInfo == null) {Deltarune.Log("Bosschecklist Couldnt get typeInfo from boss checklist, skipped");return;}
			method = typeInfo.GetMethod("Draw", BindingFlags.Public | BindingFlags.Instance);
			if (method == null) {Deltarune.Log("Bosschecklist Couldnt get method from boss checklist, skipped");return;}
			// initialize var
			applyPatchDrawString = true;
			patchDrawStringDye = -1;

			// setup boss info , reflection dark magik (real)
			SetupGetBossInfos(mod);

			// log
			if (success) {Deltarune.Log("Bosschecklist Reflection loaded succesfully, loading il edit");}
			else {Deltarune.Log("Bosschecklist Reflection failed to load. using manual texture check , loading il edit");}

			// apply detour
			On.Terraria.Utils.DrawBorderString += CoolNameLol;
			HookEndpointManager.Modify(method, new ILContext.Manipulator(PatchDraw));
			Deltarune.Log("Bosschecklist patch complete");
		}
		// unload the mod
		public static void Unload() {
			// unload hooks only if the method is cached correctly
			if (method != null) {
				HookEndpointManager.Unmodify(method, new ILContext.Manipulator(PatchDraw));
				On.Terraria.Utils.DrawBorderString -= CoolNameLol;
			}
			// nullify fields
			field = null;
			method = null;
			bossInfo = null;
			bossInfoExtra = null;
			PreText = null;
			PostText = null;
		}
		// reset custom string draw, check if there is any error. disable if there is error
		public static void Reset() {
			if (patchDrawString) {
				applyPatchDrawString = false;
				Deltarune.Log("Bosschecklist Error 1 detected, text patching unsuccess. disabling text patch");
				if (!Main.gameMenu) {
					Main.NewText("[Deltarune] sorry but there is some error[1] lol, just ignore this and keep playing",Color.Pink);
				}
				patchDrawStringDye = -1;
			}
			if (PreText != null || PostText != null) {
				Deltarune.Log("Bosschecklist Error 2 detected, text patching unsuccess. disabling text patch");
				if (!Main.gameMenu) {
					Main.NewText("[Deltarune] sorry but there is some error[2] lol, just ignore this and keep playing",Color.Pink);
				}
				PreText = null;
				PostText = null;
				applyPatchDrawString = false;
			}
		}
		// the il edit . i have no clue what is this suppose to do :skull:
		public static void PatchDraw(ILContext il){
			var c = new ILCursor(il);
			c.TryGotoNext(n => n.MatchStloc(107)); //Color3
			c.Index++;
			c.Emit(OpCodes.Ldloc, 92);
			c.Emit(OpCodes.Ldarg_0);
			c.EmitDelegate<Action<Texture2D, UIElement>>(newDraw);
		}
		// the delegate that emitted for the il edit
		public static void newDraw(Texture2D drawnTexture, UIElement self){
			var spriteBatch = Main.spriteBatch;
			patchDrawStringDye = -1;
			if (success) {
				// if reflection applied succesfully, manually check for types
				GetBossInfos(out string modSource,out List<int> npcIDs);
				if (modSource == "Deltarune" && npcIDs != null) {
					foreach (var item in bossInfoExtra){
						if (item.TypeCheck(npcIDs)) {
							item.CustomDraw(spriteBatch,self);
							return;
						}
					}
				}
			}
			else {
				// if reflection broke, manually check for textures
				foreach (var item in bossInfoExtra){
					if (drawnTexture == ModContent.GetTexture(item.GetInfoTexture())) {
						item.CustomDraw(spriteBatch,self);
						return;
					}
				}
			}
		}
		// reflection setup, contains a lot of error checking to prevent bug from happening
		public static void SetupGetBossInfos(Mod mod) {
			// reflection doom , at least i understand it more than il edit
			field = new FieldInfo[5];
			success = false;
			
			Type type = mod.Code.GetType("BossChecklist.BossLogUI");
			if (type == null) {Deltarune.Log("Reflection error type 1, failed getting BossLogUI");return;}
			field[0] = type.GetField("PageNum", BindingFlags.Public | BindingFlags.Static);
			if (field[0] == null) {Deltarune.Log("Reflection error field 0, failed getting PageNum");return;}

			Type type2 = mod.Code.GetType("BossChecklist.BossChecklist");
			if (type2 == null) {Deltarune.Log("Reflection error type 2, failed getting BossChecklist");return;}
			field[1] = type2.GetField("bossTracker",BindingFlags.NonPublic | BindingFlags.Static);
			if (field[1] == null) {Deltarune.Log("Reflection error field 1, failed getting bossTracker");return;}

			Type type3 = mod.Code.GetType("BossChecklist.BossTracker");
			if (type3 == null) {Deltarune.Log("Reflection error type 3, failed getting BossTracker");return;}
			field[2] = type3.GetField("SortedBosses",BindingFlags.NonPublic | BindingFlags.Instance);
			if (field[2] == null) {Deltarune.Log("Reflection error type 2, failed getting SortedBosses");return;}

			Type type4 = mod.Code.GetType("BossChecklist.BossInfo");
			if (type4 == null) {Deltarune.Log("Reflection error type 4, failed getting BossInfo");return;}
			field[3] = type4.GetField("modSource",BindingFlags.NonPublic | BindingFlags.Instance);
			field[4] = type4.GetField("npcIDs",BindingFlags.NonPublic | BindingFlags.Instance);
			if (field[3] == null) {Deltarune.Log("Reflection error field 3, failed getting modSource");return;}
			if (field[4] == null) {Deltarune.Log("Reflection error field 4, failed getting npcIDs");return;}
			success = true;
		}
		// get the current boss info. for the boss checklist book
		public static void GetBossInfos(out string modSource,out List<int> npcIDs) {
			// the goal of this reflection is to get the current boss info :
			// BossInfo selectedBoss = BossChecklist.bossTracker.SortedBosses[BossLogUI.PageNum];
			// selectedBoss.modSource && selectedBoss.npcIDs
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
			// List<> has IList interface, thanks c#
			IList SortedBosses = (IList)SortedBossesList;
			// prevent index out of bounds
			bool flag = PageNum >= 0;
			if (!flag) return;
			object bossInfo = SortedBosses[PageNum];
			modSource = (string)field[3].GetValue(bossInfo);
			npcIDs = (List<int>)field[4].GetValue(bossInfo);
		}

		// this used up just for some epic text shdaers
		public static int patchDrawStringDye = -1;
		public static bool patchDrawString => patchDrawStringDye > 0;
		public static bool applyPatchDrawString;
		// a function called before text is drawn
		public static Func<SpriteBatch,string,Vector2,Color,float,float,float,int,bool> PreText;
		// a action called after text is drawn
		public static Action<SpriteBatch,string,Vector2,Color,float,float,float,int> PostText;
		// the hook
		public static Vector2 CoolNameLol(On.Terraria.Utils.orig_DrawBorderString orig,SpriteBatch sb, 
		string text, Vector2 pos, Color color, float scale , float anchorx , float anchory, int maxCharactersDisplayed ) {
			Vector2 piss = Vector2.Zero;
			bool runOrig = true;
			if (PreText != null && applyPatchDrawString) {
				runOrig = PreText(sb,text,pos,color,scale,anchorx,anchory,maxCharactersDisplayed);
			}
			if (patchDrawString && applyPatchDrawString) {
				sb.BeginDyeShader(patchDrawStringDye, new Item(),true,true);
			}
			if (orig != null && runOrig) {piss = orig(sb,text,pos,color,scale,anchorx,anchory,maxCharactersDisplayed);}
			if (patchDrawString && applyPatchDrawString) {
				sb.BeginNormal(true,true);
				patchDrawStringDye = -1;
			}
			if (PostText != null && applyPatchDrawString) {
				PostText(sb,text,pos,color,scale,anchorx,anchory,maxCharactersDisplayed);
			}
			PreText = null;
			PostText = null;
			return piss;
		}
	}
}
