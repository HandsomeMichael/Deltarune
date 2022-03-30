
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
	public class Deltarune : Mod
	{
		// readonly stuff
		public static string textureExtra => "Deltarune/Content/Texture/";
		public static bool HasMusic => ModLoader.GetMod("DeltaruneMusic") != null;
		public static Mod get => ModContent.GetInstance<Deltarune>();

		//Hotkey
		public static ModHotKey KeyMagic1;
		public static ModHotKey KeyMagic2;
		public static ModHotKey KeyMagic3;

		//Intro
		public static int intro;
		public static int TitleMusic;

		//haha random static field go brrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr
		public static int battleFrame;
		public static int battleFrameCount;
		public static float battleAlpha;
		public static int selectedMenu;
		public static string deathTips;
		public static float darkenBG;
		public static float deathAlpha;
		public static TypeWriter chatNPC;

		//my homemade "UI" fields
		public static int playerClass = -1;
		public static int playerClassType;
		public static int playerClassMisc = -1;
		public static bool click;

		//for debugging purpose
		public static int[] debug = new int[4];
		public static int debugCur;

		//bool
		public static bool Boss;

		//UI SHIT
		internal tensionBar tensionBar;
		private UserInterface _tensionBarUserInterface;

		//Font
		public static DynamicSpriteFont tobyFont;

		public override void PreUpdateEntities() {
			if (Main.LocalPlayer.talkNPC < 0) {
				Deltarune.chatNPC = null;
			}
			Boss = false;
			bool bossnear = false;
			for (int i = 0; i < Main.maxNPCs; i++){
                NPC n = Main.npc[i];
                if (n.active && (n.boss || n.type == NPCID.EaterofWorldsHead)){
					Boss = true;
					if (Main.LocalPlayer.Distance(n.Center) < 1500f) {
						bossnear = true;
						break;
					}
				}
			}
			if (bossnear) {
				if (MyConfig.get.BattleBackground > 0f) {
					battleAlpha += 0.01f;
				}
			}
			else {battleAlpha -= 0.01f;}
			if (battleAlpha > MyConfig.get.BattleBackground) {battleAlpha = MyConfig.get.BattleBackground;}
			if (battleAlpha < 0f) {battleAlpha = 0f;}
			SoulHandler.Update();
		}
		public override void PostUpdateEverything() => SoulHandler.Reset();

		//DynamicSpriteFont[] font = new DynamicSpriteFont[5];

		public override void Load() {

			Logger.InfoFormat("{0} loading epic mod. haha very epic", Name);

			KeyMagic1 = RegisterHotKey("Spell I", "Z");
			KeyMagic2 = RegisterHotKey("Spell II", "X");
			KeyMagic3 = RegisterHotKey("Spell III", "C");


			Logger.InfoFormat("{0} Loading Systems", Name);
			DeltaSystemLoader.Load(this);
			Logger.InfoFormat("{0} Done loading systems", Name);
			
			if (!Main.dedServ) {
				intro = 0;
				if (FontExists("Fonts/ExampleFont")) {tobyFont = GetFont("Fonts/ExampleFont");}
				else {
					tobyFont = Main.fontMouseText;
					Logger.InfoFormat("{0} Failed to load font, if this happen then u prob need to reinstall mod, loading vanilla font", Name);
				}

				//Assign ui

				tensionBar = new tensionBar();
				_tensionBarUserInterface = new UserInterface();
				_tensionBarUserInterface.SetState(tensionBar);

				//Shader ( no way )

				//how to use shader ( if i forgor how to )
				// Misc Shader = GameShaders.Misc["ShaderOverlay"].UseColor(color).Apply();
				// Screen Shader :
				//Filters.Scene.Activate("FilterName");
				// Updating a filter
				//Filters.Scene["FilterName"].GetShader().UseProgress(progress);
				//Filters.Scene["FilterName"].Deactivate();

				Ref<Effect> shaderRef = new Ref<Effect>(GetEffect("Effects/overlay"));
				GameShaders.Misc["ShaderOverlay"] = new MiscShaderData(shaderRef, "ShaderOverlayPass");

				shaderRef = new Ref<Effect>(GetEffect("Effects/blur"));
				GameShaders.Misc["Blurify"] = new MiscShaderData(shaderRef, "Blurify");
				Filters.Scene["Blurify"] = new Filter(new ScreenShaderData(shaderRef, "Blurify"), EffectPriority.Medium);
				//UseColor(Angle = 0, blur amount = 0.003, 0)

				shaderRef = new Ref<Effect>(GetEffect("Effects/Pixelate"));
				Filters.Scene["Pixelate"] = new Filter(new ScreenShaderData(shaderRef, "Pixelate"), EffectPriority.Medium);

				shaderRef = new Ref<Effect>(GetEffect("Effects/paperfold"));
				GameShaders.Misc["paperfold"] = new MiscShaderData(shaderRef, "paperfold");
				//UseOpacity

				//funky
				shaderRef = new Ref<Effect>(GetEffect("Effects/funky"));
				GameShaders.Misc["funky"] = new MiscShaderData(shaderRef, "funky");
				//UseOpacity (from 0 to 1 and then to 0)

				//WaveWrap
				shaderRef = new Ref<Effect>(GetEffect("Effects/WaveWrap"));
				GameShaders.Misc["WaveWrap"] = new MiscShaderData(shaderRef, "WaveWrap");
				Filters.Scene["WaveWrap"] = new Filter(new ScreenShaderData(shaderRef, "WaveWrap"), EffectPriority.Medium);
				//UseOpacity (from 0 to 2048 and then to 0)

				shaderRef = new Ref<Effect>(GetEffect("Effects/DeathAnimation"));
				GameShaders.Misc["DeathAnimation"] = new MiscShaderData(shaderRef, "DeathAnimation").UseImage("Images/Misc/Perlin");
				//UseOpacity (from 0 to 2048 and then to 0)
				//GameShaders.Misc["ExampleEffectDeath"].UseOpacity(color).Apply();
			}	
			Logger.InfoFormat("{0} Loading done", Name);
		}
		public override void Close(){
			if (Main.music.IndexInRange(TitleMusic) && (Main.music[TitleMusic]?.IsPlaying ?? false)){
				Main.music[TitleMusic].Stop(AudioStopOptions.Immediate);
			}
			base.Close();
		}
		public override void Unload() {

			DeltaSystemLoader.Unload();

			TitleMusic = 0;
			intro = 0;
			chatNPC = null;
			KeyMagic1 = null;
			KeyMagic2 = null;
			KeyMagic3 = null;
		}
		public override void PreSaveAndQuit() {
			Logger.InfoFormat($"Deltarune : debug value list [{debug[0]}] [{debug[1]}] [{debug[2]}] [{debug[3]}]");
		}
		public override void PostSetupContent() {
			if (MyConfig.get.intro) {
				Logger.InfoFormat("{0} Intro plays", Name);
				intro = 1;
				Main.PlaySound(GetSound("AUDIO_INTRONOISE",false));
			}
			else {
				intro = -1;
				if (MyConfig.get.mainMenu) {
					TitleMusic = GetSoundSlot(SoundType.Music, "Sounds/Music/lancer");
					Main.logoTexture = ModContent.GetTexture(textureExtra+"Title"+Main.rand.Next(1,6));
					Main.logo2Texture = ModContent.GetTexture(textureExtra+"Title"+Main.rand.Next(1,6));
				}
			}
		}
		public override void HandlePacket(BinaryReader reader, int whoAmI) => NetCode.HandlePacket(reader,whoAmI);
		public override void UpdateMusic(ref int music, ref MusicPriority priority) {
			if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active) {return;}
			if (Main.LocalPlayer.dead && MyConfig.get.newGameOver) {
				music = GetSoundSlot(SoundType.Music, "Sounds/Music/gameover_short");//gameover_short
				priority = MusicPriority.BossHigh;
			}
		}
		public override void UpdateUI(GameTime gameTime) {
			_tensionBarUserInterface?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			//foreach (var item in layers){Main.NewText(item.Name);}
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1) {
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"Deltarune: Tension bar",
					delegate {
						_tensionBarUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
			/*
			int dialogIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: NPC / Sign Dialog"));
			if (dialogIndex != -1) {
				layers.Insert(dialogIndex, new LegacyGameInterfaceLayer(
					"Deltarune: Chat head",
					delegate {
						DrawDialogHead(Main.spriteBatch);
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
			*/
		}
		//Method
		public static Terraria.Audio.LegacySoundStyle GetSound(string text,bool snd = true) {
			return Deltarune.get.GetLegacySoundSlot(SoundType.Custom, "Sounds/"+(snd?"snd_":"")+text);	
		}
		public static int NPCMusic(string mod,int type) {
			if (Deltarune.HasMusic) {
				int num = NewMusic(mod);
				if (num == 1) {
					Main.NewText($"'{mod}' music not found, Make sure you updated 'Deltarune Music Mod' !",Color.Red);
					return type;
				}
				return NewMusic(mod);
			}
			else {return type;}
		}
		public static int NewMusic(string type) {
			Mod musicMod = ModLoader.GetMod("DeltaruneMusic");
			if (musicMod != null) {
				int a = musicMod.GetSoundSlot(SoundType.Music, "Sounds/Music/"+type);
				if (a < 1){
					Deltarune.get.Logger.InfoFormat($"Deltarune : Error Music '{type}' Not Found");
					a = 1;
				}
				return a;
			}
			else {return 1;}
		}
	}
}
