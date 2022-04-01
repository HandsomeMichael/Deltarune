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
    public class Removed : ModItem
    {
        public override string Texture => "Terraria/CoolDown";
        public override void SetStaticDefaults() 
        {
            DisplayName.SetDefault("Removed");
            Tooltip.SetDefault("this item is removed");
        }
        public override void SetDefaults() 
        {
            item.width = 14;
            item.height = 14;
            item.rare = ItemRarityID.Red;
            item.value = 0;
        }
    }

	/// <summary>
	/// A static class that has many extension method. used in many of my mods.
	/// you can also use this if you want althought the extension here have little to no documentation about it ( cope )
	/// most stuff in here arent actually used lol
	/// </summary>
    public static class Helpme
    {
		#region Private Mod Extension
		
        public static GlobeItem GetDelta(this Item item) => item.GetGlobalItem<GlobeItem>();
        public static GlobeNPC GetDelta(this NPC npc) => npc.GetGlobalNPC<GlobeNPC>();
        public static GlobeProj GetDelta(this Projectile projectile) => projectile.GetGlobalProjectile<GlobeProj>();
        public static DeltaPlayer GetDelta(this Player player) => player.GetModPlayer<DeltaPlayer>();

		/// <summary>
		/// shake camera
		/// </summary>
        public static void CameraShake(this Player player,int num) {
            player.GetDelta().cameraShake = num;
        }

		/// <summary>
		/// shake camera with timer
		/// </summary>
        public static void CameraShake(this Player player,int num, int time) {
            player.GetDelta().cameraShakeForcedTimer = time;
            player.GetDelta().cameraShakeForced = num;
        }

		#endregion

        /// <summary>
		/// get player money
		/// </summary>
        public static long GetSavings(this Player player) { //Thanks Dire
            long inv = Utils.CoinsCount(out _, player.inventory, new int[]{
                58, /*Mouse item*/57, /*Ammo slots*/56,55,54
            });
            int[] empty = new int[0];
            long piggy = Utils.CoinsCount(out _, player.bank.item, empty);
            long safe = Utils.CoinsCount(out _, player.bank2.item, empty);
            long forge = Utils.CoinsCount(out _, player.bank3.item, empty);
            return Utils.CoinsCombineStacks(out _, new long[]{inv,piggy,safe,forge});
        }

		/// <summary>
		/// get player mouse item
		/// </summary>
		public static Item MouseItem(this Player player) => player.inventory[58];


        /// <summary>
		/// check if player has ammo
		/// </summary>
        public static bool HasAmmo(this Player player,int id) {
            foreach (var item in player.inventory){if (item.ammo == id) {return true;}}
            return false;
        }

        /// <summary>
		/// drop loot
		/// </summary>
        public static void DropLoot(this NPC npc,int npcid,int itemid, int stack = 1,int ran = 1, bool extraBool = true) {
			if (Main.rand.Next(ran) == 0 && extraBool) {
                if (npcid == -1) {Item.NewItem(npc.getRect(), itemid, stack);}
				else if (npc.type == npcid) {Item.NewItem(npc.getRect(), itemid, stack);}
			}
		}

        /// <summary>
		/// resize projectile hitbox
		/// </summary>
        public static void Resize(this Projectile projectile, int newWidth = 0, int newHeight = 0){
            Vector2 oldCenter = projectile.Center;
            projectile.width = newWidth;
            projectile.height = newHeight;
            projectile.Center = oldCenter;
        }

        /// <summary>
		/// create shop
		/// </summary>
        public static void AddShop(this Chest shop, ref int nextSlot, int item, int gold = -1){
            shop.item[nextSlot].SetDefaults(item);
            if (gold > -1) {shop.item[nextSlot].shopCustomPrice = gold;}
            nextSlot++;
        }
        /// <summary>
		/// check if there is any boss
		/// </summary>
        public static bool AnyBoss() {
			for (int i = 0; i < Main.maxNPCs; i++){
                NPC n = Main.npc[i];
                if (n.active && (n.boss || n.type == NPCID.EaterofWorldsHead)){return true;}
			}
			return false;
		}
        /// <summary>
		/// create coin text
		/// </summary>
        public static string coinsText(int price) {
            string coinsText = "";
            int[] coins = Utils.CoinsSplit(price);
            if (coins[3] > 0) {coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinPlatinum).Hex3() + ":" + coins[3] + " " + Language.GetTextValue("LegacyInterface.15") + "] ";}
            if (coins[2] > 0) {coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinGold).Hex3() + ":" + coins[2] + " " + Language.GetTextValue("LegacyInterface.16") + "] ";}
            if (coins[1] > 0) {coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinSilver).Hex3() + ":" + coins[1] + " " + Language.GetTextValue("LegacyInterface.17") + "] ";}
            if (coins[0] > 0) {coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinCopper).Hex3() + ":" + coins[0] + " " + Language.GetTextValue("LegacyInterface.18") + "] ";}
            return coinsText;
        }
		/// <summary>
		/// create coin text with icon
		/// </summary>
        public static string coinsText2(int price) {
            string coinsText = "";
            int[] coins = Utils.CoinsSplit(price);
            if (coins[3] > 0) {coinsText = coinsText + $"[i:{ItemID.PlatinumCoin}] [c/" + Colors.AlphaDarken(Colors.CoinPlatinum).Hex3() + ":" + coins[3] + " " + Language.GetTextValue("LegacyInterface.15") + "] ";}
            if (coins[2] > 0) {coinsText = coinsText + $"[i:{ItemID.GoldCoin}] [c/" + Colors.AlphaDarken(Colors.CoinGold).Hex3() + ":" + coins[2] + " " + Language.GetTextValue("LegacyInterface.16") + "] ";}
            if (coins[1] > 0) {coinsText = coinsText + $"[i:{ItemID.SilverCoin}] [c/" + Colors.AlphaDarken(Colors.CoinSilver).Hex3() + ":" + coins[1] + " " + Language.GetTextValue("LegacyInterface.17") + "] ";}
            if (coins[0] > 0) {coinsText = coinsText + $"[i:{ItemID.CopperCoin}] [c/" + Colors.AlphaDarken(Colors.CoinCopper).Hex3() + ":" + coins[0] + " " + Language.GetTextValue("LegacyInterface.18") + "] ";}
            return coinsText;
        }
        /// <summary>
		/// get best pick
		/// </summary>
        public static Item GetBestPick(this Player player){
            Item item = null;
            for (int i = 0; i < player.inventory.Length; i++){
                Item aitem = player.inventory[i];
                if (aitem.stack > 0 && aitem.pick > 0 && (item == null || aitem.pick > item.pick)){item = aitem;}
            }
            return item;
        }
		/// <summary>
		/// save int32 array
		/// </summary>
		public static void AddIntArray(this TagCompound tag, int[] array, string name = "array") {
			for (int i = 0; i < array.Length; i++){
				tag.Add(name+i,array[i]);
			}
		}
		/// <summary>
		/// load int32 array
		/// </summary>
		public static int[] GetIntArray(this TagCompound tag, int length, string name = "array") {
			int[] num = new int[length];
			for (int i = 0; i < length; i++){
				num[i] = tag.GetInt(name+i);
			}
			return num;
		}

		/// <summary>
		/// get rarity color. contains "wall of ifs"
		/// </summary>
		public static Color rarityToColor(int rare) {
			if (rare == -12){return Main.DiscoColor;}
			if (rare == -11){return new Color(255, 175, 0);}
			if (rare == -1){return new Color(130, 130, 130);}
			if (rare == 1){return new Color(150, 150, 255);}
			if (rare == 2){return new Color(150, 255, 150);}
			if (rare == 3){return new Color(255, 200, 150);}
			if (rare == 4){return new Color(255, 150, 150);}
			if (rare == 5){return new Color(255, 150, 255);}
			if (rare == 6){return new Color(210, 160, 255);}
			if (rare == 7){return new Color(150, 255, 10);}
			if (rare == 8){return new Color(255, 255, 10);}
			if (rare == 9){return new Color(5, 200, 255);}
			if (rare == 10){return new Color(255, 40, 100);}
			if (rare == 11){return new Color(180, 40, 255);}
			return Color.White;
		}
		/// <summary>
		/// Destroy chat tags , i hate chat tags
		/// <param name="text">the text</param>
		/// </summary>
		public static string DestroyChatTags(string text) {
			var snippetList = ChatManager.ParseMessage(text, Color.White);
			string funni = "";
			foreach (var i in snippetList){
				if (!i.TextOriginal.Contains("[i")) {funni += i.Text;}
			}
			return funni;
		}
		
		/// <summary>
		/// Shorthand for Vector2.Lerp
		/// </summary>
		public static Vector2 Lerp(this Vector2 pos,Vector2 pos2,float intensity) => Vector2.Lerp(pos,pos2,intensity);

		/// <summary>
		/// Get angle to a position
		/// </summary>
        public static float AngleTo(this Vector2 From,Vector2 Destination){
			return (float)Math.Atan2(Destination.Y - From.Y, Destination.X - From.X);
		}
		/// <summary>
		/// Get angle from a position
		/// </summary>
		public static float AngleFrom(this Vector2 From,Vector2 Source){
			return (float)Math.Atan2(From.Y - Source.Y, From.X - Source.X);
		}
		/// <summary>
		/// Distance Squared. short hand for Vector2.DistanceSquared
		/// </summary>
		public static float DistanceSQ(this Vector2 From,Vector2 Other){
			return Vector2.DistanceSquared(From, Other);
		}
		/// <summary>
		/// Get direction to a position
		/// </summary>
		public static Vector2 DirectionTo(this Vector2 From,Vector2 Destination){
			return Vector2.Normalize(Destination - From);
		}
		/// <summary>
		/// Get Direction from a position
		/// </summary>
		public static Vector2 DirectionFrom(this Vector2 From,Vector2 Source){
			return Vector2.Normalize(From - Source);
		}
		/// <summary>
		/// check if position is in range with another position
		/// </summary>
		public static bool InRange(this Vector2 From,Vector2 Target, float MaxRange){
			return Vector2.DistanceSquared(From, Target) <= MaxRange * MaxRange;
		}
		/// <summary>
		/// Resize Rectangle
		/// </summary>
		public static void Resize(this Rectangle rec, int x) {
			rec.X -= x;
			rec.Y -= x;
			rec.Width += x*2;
			rec.Height += x*2;
		}
		/// <summary>
		/// A quick way for getting item default. im too lazy to type
		/// </summary>
		public static Item ItemDefault(int id, int stack = 1,bool prefix = false) {
			Item item = new Item();
			item.SetDefaults(id);
			if (prefix) {
				item.Prefix(-1);
			}
			item.stack = stack;
			return item;
		}
		/// <summary>
		/// A quick way for getting npc default. im too lazy to type
		/// </summary>
		public static NPC NPCDefault(int id) {
			NPC item = new NPC();
			item.SetDefaults(id);
			return item;
		}
		/// <summary>
		/// get player lerp value. ported from 1.4 utils
		/// </summary>
		public static float GetLerpValue(float from, float to, float t, bool clamped = false){
			if (clamped){
				if (from < to){
					if (t < from){return 0f;}
					if (t > to){return 1f;}
				}
				else{
					if (t < to){return 1f;}
					if (t > from){return 0f;}
				}
			}
			return (t - from) / (to - from);
		}
		/// <summary>
		/// for those who are lazy to cast int :amogus:
		/// </summary>
		public static Rectangle QuickRec(this Vector2 pos, Vector2 size) {
			return new Rectangle((int)pos.X,(int)pos.Y,(int)size.X,(int)size.Y);
		}
		/// <summary>
		/// get the rectangle of a texture
		/// </summary>
		public static Rectangle getRect(this Texture2D texture2, Vector2 pos) {
			return new Rectangle((int)pos.X - texture2.Width/2,(int)pos.Y - texture2.Width/2,texture2.Width,texture2.Height);
		}
		/// <summary>
		/// save a config
		/// </summary>
		public static void SaveConfig<T>() where T : ModConfig{
			typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[1] { ModContent.GetInstance<T>()});
		}
		/// <summary>
		/// check if player is in forest.
		/// </summary>
		public static bool IsInForest(this Player player){
			return !player.ZoneJungle
				&& !player.ZoneDungeon
				&& !player.ZoneCorrupt
				&& !player.ZoneCrimson
				&& !player.ZoneHoly
				&& !player.ZoneSnow
				&& !player.ZoneUndergroundDesert
				&& !player.ZoneGlowshroom
				&& !player.ZoneMeteor
				&& !player.ZoneBeach
				&& player.ZoneOverworldHeight;
		}
		/// <summary>
		/// Center a pos from entity
		/// </summary>
		public static Vector2 Center(this Entity entity, Vector2 pos) {
			return new Vector2(pos.X + (float)(entity.width / 2), pos.Y + (float)(entity.height / 2));
		}
        
        /// <summary>
		/// creates random string
		/// </summary>
        public static string RandomString(int length,bool tag = false) {
			string value = "";
			// i smashed my keyboard for this
			string text = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890";
			//this maybe a bad idea . but i dont care, i can do whatever i want :troll:
			if (tag) {text += "!@#$%&*()-=_+~`[]{}|;'<>?,./";}
			for (int i = 0; i < length; i++){
				value += text[Main.rand.Next(0,text.Length+1)];
			}
			return value;
		}
        /// <summary>
		/// apply shader on tooltip, use it on PreDrawTooltipLine
		/// example code :
		/// if (item.ApplyShaderOnTooltip(line,dyeID)) {return false;}
		/// <param name="item">The item.</param>
		/// <param name="line">The tooltip line.</param>
		/// <param name="id">the dye id.</param>
		/// <param name="mod">the mod name of tooltip line (default to Terraria).</param>
		/// <param name="name">the name of tooltip line (default to ItemName)</param>
		/// </summary>
        public static bool ApplyShaderOnTooltip(this Item item,DrawableTooltipLine line,int id,string name = "ItemName",string mod = "Terraria") {
            if (line.mod == mod && line.Name == name){
				Main.spriteBatch.BeginDyeShader(id,item,true,true);
                Utils.DrawBorderString(Main.spriteBatch, line.text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
				Main.spriteBatch.BeginNormal(true,true);
                return true;
            }
            return false;
        }
		/// <summary>
		/// The sum of Vector2 X and Y without negative value
		/// </summary>
		public static float FloatCount(this Vector2 pos) {
			return (pos.X > 0 ? pos.X : pos.X*-1) + (pos.Y > 0 ? pos.Y : pos.Y*-1);
		}
		/// <summary>
		/// MeasureString with Chat tags support (no way)
		/// <param name="text">The text.</param>
		/// <param name="font">The font, set to null for Main.fontMouseText.</param>
		/// </summary>
		public static Vector2 MeasureString(this string text,DynamicSpriteFont font = null) {
			if (font == null) {font = Main.fontMouseText;}
			TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();
			return ChatManager.GetStringSize(font, snippets, Vector2.One);
		}

		/// <summary>
		/// spriteBatch begin but apply armor gameshader afterwards
		/// </summary>
		/// <param name="id">The item id of dye.</param>
		/// <param name="entity">The entity that will get shadered</param>
		/// <param name="end">Wether or not the sprite should be Ended before started.</param>
		/// <param name="ui">Wether it scaled by ui or zoom.</param>
		public static void BeginDyeShader(this SpriteBatch spriteBatch,int id, Entity entity, bool end = false,bool ui = false) {
			spriteBatch.BeginImmediate(end,ui);
			GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(id), entity, null);
		}

		/// <summary>
		/// spriteBatch begin but using immediate SpriteSortMode (for shader applying)
		/// </summary>
		/// <param name="end">Wether or not the sprite should be Ended before started.</param>
		/// <param name="ui">Wether it scaled by ui or zoom.</param>
		public static void BeginImmediate(this SpriteBatch spriteBatch,bool end = false, bool ui = false, bool additive = false) {
			if (end) {spriteBatch.End();}
			var scale = Main.GameViewMatrix.ZoomMatrix;
			if (ui) {scale = Main.UIScaleMatrix;}
			BlendState blend = null;
			if (additive) {blend = BlendState.Additive;}
			spriteBatch.Begin(SpriteSortMode.Immediate, blend, null, null, null, null, scale);
		}
        /// <summary>
		/// spriteBatch begin but using normal sort mode and effects
		/// </summary>
		/// <param name="end">Wether or not the sprite should be Ended before started.</param>
		/// <param name="ui">Wether it scaled by ui or zoom.</param>
		public static void BeginNormal(this SpriteBatch spriteBatch, bool end = false, bool ui = false) {
			if (end) {spriteBatch.End();}
			//Main.GameViewMatrix.TransformationMatrix
			var scale = Main.GameViewMatrix.ZoomMatrix;
			if (ui) {scale = Main.UIScaleMatrix;}
			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, scale);
		}

		/// <summary>
		/// spriteBatch begin but using blendstate.additive
		/// </summary>
		/// <param name="end">Wether or not the sprite should be Ended before started.</param>
		/// <param name="ui">Wether it scaled by ui or zoom.</param>
        public static void BeginGlow(this SpriteBatch spriteBatch, bool end = false,bool ui = false) {
			if (end) {spriteBatch.End();}
			var scale = Main.GameViewMatrix.ZoomMatrix;
			if (ui) {scale = Main.UIScaleMatrix;}
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, scale);
		}

		/// <summary>
		/// draw this line vanillaly
		/// </summary>
		public static void Draw(this DrawableTooltipLine line, string text = "",Vector2? pos = null, Color? color = null) {
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.font, text == "" ? line.text : text, pos ?? new Vector2(line.X,line.Y), color ?? Color.White, line.rotation, line.origin, line.baseScale, line.maxWidth, line.spread);
		}
		/// <summary>
		/// no more funny array haha
		/// </summary>
		public static string NextString(this UnifiedRandom rand,params string[] args) => rand.NextPart<string>(args);
		/// <summary>
		/// no more funny array haha
		/// </summary>
		public static T NextPart<T>(this UnifiedRandom rand,params T[] args) => rand.Next(args);
		/// <summary>
		/// Check for item stack and Delete item from inventory
		/// </summary>
		public static bool QuickRemoveItem(this Player player, int id, int amount = 1) {
			int stack = player.GetItemStack(id);
			if (stack < amount) {return false;}
			player.DeleteItem(id,amount);
			return true;
		}
		/// <summary>
		/// A helper method that draws a bordered rectangle, stolen from example mod
		/// </summary>
		public static void DrawBorderedRect(this SpriteBatch spriteBatch, Vector2 position, Vector2 size,Color borderColor,int borderWidth = 2,Texture2D texture = null) {
			if (texture == null) {texture = Main.magicPixel;}
			int posX = (int)position.X - borderWidth;
			spriteBatch.Draw(texture, new Rectangle(posX, (int)position.Y - borderWidth, (int)size.X + borderWidth * 2, borderWidth), borderColor);
			spriteBatch.Draw(texture, new Rectangle(posX, (int)position.Y + (int)size.Y, (int)size.X + borderWidth * 2, borderWidth), borderColor);
			spriteBatch.Draw(texture, new Rectangle(posX, (int)position.Y, (int)borderWidth, (int)size.Y), borderColor);
			spriteBatch.Draw(texture, new Rectangle((int)position.X + (int)size.X, (int)position.Y, (int)borderWidth, (int)size.Y), borderColor);
		}
		/// <summary>
		/// A helper method that draws a bordered rectangle with Rectangle as param, stolen from example mod
		/// </summary>
		public static void DrawBorderedRect(this SpriteBatch spriteBatch, Rectangle rec,Color borderColor,int borderWidth = 2,Texture2D texture = null) {
			spriteBatch.DrawBorderedRect(new Vector2(rec.X,rec.Y),new Vector2(rec.Width,rec.Height),borderColor,borderWidth,texture);
		}
		/// <summary>
		/// Make texture become black ( for overlay purpose of course. nothin sus here )
		/// </summary>
		public static void ToBlack(this Texture2D texture){
			Color[] c = new Color[texture.Width * texture.Height];
			texture.GetData(c);
			for (int a = 0; a < c.Length; a++){
				//int g = (c[a].R + c[a].G + c[a].B) / 3;
				int g = 0;
				c[a] = new Color(g, g, g);
			}
			texture.SetData(c);
		}
		/// <summary>
		/// Make texture color changed to a new color
		/// </summary>
		/// <param name="color"> the color </param>
		public static void ToColor(this Texture2D texture, Color color){
			Color[] c = new Color[texture.Width * texture.Height];
			texture.GetData(c);
			for (int a = 0; a < c.Length; a++){
				c[a] = new Color(color.R, color.G, color.B);
			}
			texture.SetData(c);
		}
		/// <summary>
		/// Pick something from an array, a perfect replacement for if statement
		/// </summary>
		public static T ArrayPick<T>(int index,params T[] args) {
			if (index > args.Length || index == -1) {return args[0];}
			return args[index];
		}
		/// <summary>
		/// a shorthand for AddBuff without using ModContent
		/// </summary>
		public static void AddBuff<T> (this Player player,int time) where T : ModBuff {
			player.AddBuff(ModContent.BuffType<T>(),time);
		}
		/// <summary>
		/// a shorthand for AddBuff without using ModContent
		/// </summary>
		public static void AddBuff<T> (this NPC player,int time) where T : ModBuff {
			player.AddBuff(ModContent.BuffType<T>(),time);
		}
		
		/// <summary>
		/// Get Item stack from player inventory
		/// </summary>
		public static int GetItemStack(this Player player, int id) {
			int stack = 0;
			foreach (var item in player.inventory){if (item.type == id) {stack += item.stack;}}
			return stack;
		}
		/// <summary>
		/// cut down floats , example : 8.9212 to 8.9
		/// thanks screen shdaers for making this :)
		/// </summary>
		public static float DecRound(this float f) {
			//f += 1f;
			float num = f*10f;
			int num2 = (int)f;
			num = (float)num2/10f;
			//num -= 1f;
			return num;
		}
		/// <summary>
		/// Get nearest npc from a position
		/// </summary>
		public static int NearestNPC(this Vector2 pos,float distanceFromTarget = 700f, bool friendly = false) {
			int b = -1;
			Vector2 targetCenter = pos;
			for (int i = 0; i < Main.maxNPCs; i++) {
				NPC npc = Main.npc[i];
				if (npc.CanBeChasedBy() || (friendly && npc.active && npc.friendly)) {
					float between = Vector2.Distance(npc.Center, pos);
					bool closest = Vector2.Distance(pos, targetCenter) > between;
					bool inRange = between < distanceFromTarget;
					if ((closest && inRange) || b == -1) {
						b = i;
						targetCenter = npc.Center;
					}
				}
			}
			return b;
		}
		/// <summary>
		/// Get nearest player from a position
		/// </summary>
		public static int NearestPlayer(this Vector2 pos,float distanceFromTarget = 700f) {
			int b = -1;
			Vector2 targetCenter = pos;
			for (int i = 0; i < Main.maxPlayers; i++) {
				Player npc = Main.player[i];
				if (npc.active && !npc.dead) {
					float between = Vector2.Distance(npc.Center, pos);
					bool closest = Vector2.Distance(pos, targetCenter) > between;
					bool inRange = between < distanceFromTarget;
					if ((closest && inRange) || b == -1) {
						b = i;
						targetCenter = npc.Center;
					}
				}
			}
			return b;
		}
		/// <summary>
		/// Get nearest projectile from a position
		/// </summary>
		public static int NearestProjectile(this Vector2 pos,float distanceFromTarget = 700f) {
			int b = -1;
			Vector2 targetCenter = pos;
			for (int i = 0; i < Main.maxProjectiles; i++) {
				Projectile npc = Main.projectile[i];
				if (npc.active) {
					float between = Vector2.Distance(npc.Center, pos);
					bool closest = Vector2.Distance(pos, targetCenter) > between;
					bool inRange = between < distanceFromTarget;
					if ((closest && inRange) || b == -1) {
						b = i;
						targetCenter = npc.Center;
					}
				}
			}
			return b;
		}
		
		public static int NearestProjectile(this Entity entity,float distance = 700f) => NearestProjectile(entity.Center,distance);
		public static int NearestPlayer(this Entity entity,float distance = 700f) => NearestPlayer(entity.Center,distance);
		public static int NearestNPC(this Entity entity,float distance = 700f) => NearestNPC(entity.Center,distance);

		/// <summary>
		/// a shorter short hand for calling Vector2.Distance ( i am the most lazy coder ever )
		/// </summary>
		public static float Distance(this Vector2 pos,Vector2 to) => Vector2.Distance(pos,to);
		/// <summary>
		/// Delete item from player inventory
		/// </summary>
		public static void DeleteItem(this Player player, int id, int amount = 1) {
			int deleted = 0;
			foreach(var item in player.inventory) {
				if (deleted >= amount) {
					break;
				}
				if (item != null && item.type == id) {
					if (amount == 1) {
						if (item.stack > 1) {item.stack -= 1;}
						else {item.TurnToAir();}
						deleted = amount;
						break;
					}
					else {
						if (item.stack == (amount - deleted)) {
							deleted += item.stack;
							item.TurnToAir();
							break;
						}
						else if (item.stack < (amount - deleted)) {
							deleted += item.stack;
							item.TurnToAir();
						}
						else if (item.stack > (amount - deleted)) {
							int a = amount - deleted;
							deleted += item.stack;
							item.stack -= a;
						}
					}
				}
				if (deleted >= amount) {
					break;
				}
			}
		}

        /// <summary>
		/// Get frame from texture
		/// </summary>
        public static Rectangle GetFrame(this Texture2D Texture, int frame, int maxframe) {
			int frameHeight = Texture.Height / maxframe;
			int startY = frameHeight * frame;
			return new Rectangle(0, startY, Texture.Width, frameHeight);
		}

        /// <summary>
		/// Read intreger array
		/// </summary>
		public static int[] ReadArrayInt32(this BinaryReader reader) {
			int length = reader.ReadInt32();
			int[] array = new int[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = reader.ReadInt32();
			}
			return array;
		}
		/// <summary>
		/// Write intreger array
		/// </summary>
		public static void WriteArrayInt32(this BinaryWriter writer, int[] array) {
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				writer.Write(array[i]);
			}
		}
		/// <summary>
		/// Cycle between color smoothly
		/// </summary>
		public static Color CycleColor(params Color[] color) {
			float fade = Main.GameUpdateCount % 60 / 60f;
			int index = (int)(Main.GameUpdateCount / 60 % color.Length);
			return Color.Lerp(color[index], color[(index + 1) % color.Length], fade);
		}
		/// <summary>
		/// Cycle between color smoothly with update parameter
		/// </summary>
		public static Color CycleColor(int update,params Color[] color) {
			float fade = Main.GameUpdateCount % update / (float)update;
			int index = (int)(Main.GameUpdateCount / update % color.Length);
			return Color.Lerp(color[index], color[(index + 1) % color.Length], fade);
		}
		/// <summary>
		/// create a line of dust
		/// </summary>
		public static void DustLine(this Vector2 dustPos,Vector2 pos,int dust, bool gravity = true) {
			while (dustPos.Distance(pos) > 20f){
				Dust d = Dust.NewDustPerfect(dustPos, dust, Vector2.Zero);
				d.noGravity = gravity;
				dustPos += dustPos.DirectionTo(pos)*5f;
			}
		}
		/// <summary>
		/// owner
		/// </summary>
		public static Player Owner(this Projectile projectile) {
			return Main.player[projectile.owner];
		}
		/// <summary>
		/// A quick way getting tmod methodbase
		/// </summary>
		public static MethodBase GetModMethod(string loader,string method) {
			return typeof(Mod).Assembly.GetType("Terraria.ModLoader."+loader).GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
		}
		/// <summary>
		/// Check if this type has an attribute
		/// </summary>
		public static bool HasAttribute<T>(this Type type) {
			return Attribute.GetCustomAttribute(type,typeof(T)) != null;
		}
        /// <summary>
		/// Draw Chains. ported from example mod
		/// </summary>
        public static void DrawChain(this SpriteBatch spriteBatch, Color lightColor,Vector2 from,Vector2 tothis, string Texture, bool nolight = false,float lengths = 25f, int spacing = 12) {
			Texture2D chainTexture = ModContent.GetTexture(Texture);
			var remainingVectorToPlayer = from - tothis;
			float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;
			// tothis while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
			while (true) {
				float length = remainingVectorToPlayer.Length();
				// Once the remaining length is small enough, we terminate the loop
				if (length < lengths || float.IsNaN(length))
					break;
				// tothis is advanced along the vector back to the player by 12 pixels
				// 12 comes from the height of ExampleFlailProjectileChain.png and the spacing that we desired between links
				tothis += remainingVectorToPlayer * spacing / length;
				remainingVectorToPlayer = from - tothis;
				// Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
				Color color = Lighting.GetColor((int)tothis.X / 16, (int)(tothis.Y / 16f));
                if (nolight) {color = lightColor;}
				spriteBatch.Draw(chainTexture, tothis - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
			}
        }
    }
}
