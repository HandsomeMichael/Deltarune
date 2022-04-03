using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using MonoMod.Cil;
using ReLogic.Graphics;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.Localization; 
using Terraria.Utilities;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Deltarune.Helper;

namespace Deltarune
{
	public interface IAdditive{void DrawAdditive(SpriteBatch spritebatch);}
	
	public class CustomEntity : ILoadable , IPreSaveAndQuit , ILoggable
	{
		public void Log(Action<string> log) {
			int count = 0;
			foreach (var item in bitList){
				log($"CustomEntity, [{count}] {item}");
				count++;
			}
		}

		public static List<BitEntity> bitList;
		public void PreSaveAndQuit() => Load();
		public void Unload() => bitList = null;
		public void Load() => bitList = new List<BitEntity>();

		public static void New(BitEntity entity, Vector2 position, int timeLeft) {
			entity.timeLeft = timeLeft;
			New(entity,position);
		}
		public static void New(BitEntity entity, Vector2 position) {
			entity.position = position;
			New(entity);
		}
		public static void New(BitEntity entity) {
			if (entity.OnSpawn()) {
				bitList.Add(entity);
			}
		}
		public static void UpdateAll()
		{
			if (bitList == null) {return;}
			var ded = new List<BitEntity>();
			foreach (var bit in bitList){if (!bit.Update()) {ded.Add(bit);}}
			foreach (var dead in ded) {bitList.Remove(dead);}
		}
		public static void DrawAll() {
			if (bitList == null || bitList.Count < 1 || Main.mapFullscreen) {return;}
			Main.spriteBatch.BeginNormal();
			foreach (var bit in bitList){bit.Draw(Main.spriteBatch);}
			Main.spriteBatch.BeginGlow(true);
			foreach (var bit in bitList){
				if (bit is IAdditive hook) {hook.DrawAdditive(Main.spriteBatch);}
			}
			Main.spriteBatch.End();
		}
	}
	public class Glowers : BitEntity , IAdditive
	{
		public int type;
		public Color color;
		public Vector2 velocity;
		public float scale;
		public override bool OnSpawn() {
			if (type == 2) {
				velocity = new Vector2(Main.rand.NextFloat(-3f, 4f)*2f,Main.rand.NextFloat(-3f, 4f)*1.5f);
			}
			return true;
		}
		public override bool Update() {
			timeLeft--;
			position += velocity;
			if (type == 1) {
				velocity *= 0.98f;
				scale -= 0.01f;
				if (scale < 0f) {return false;}
			}
			return timeLeft > 0;
		}
		public void DrawAdditive(SpriteBatch spriteBatch){
			var texture = ModContent.GetTexture(Deltarune.textureExtra+"yourballs");
			float alpha = 1f;
			if (type == 1) {
				alpha = scale;
			}
			spriteBatch.Draw(texture, position - Main.screenPosition, null, color*alpha, 0f, texture.Size()/2f, scale, SpriteEffects.None, 0);
		}
		public Glowers(int type,Color color,float scale,Vector2 velocity) {
			this.type = type;
			this.color = color;
			this.scale = scale;
			this.velocity = velocity;
		}
		public static void New(int type,Vector2 pos,Vector2 velocity,Color color,float scale = 1f,int timeLeft = 60) {
			CustomEntity.New(new Glowers(type,color,scale,velocity),pos,timeLeft);
		}
	}
	public class LancerIntro : BitEntity
	{
		const int maxFrame = 6;
		const int frameCounter = 3;
		public override bool Update() {
			timeLeft++;
			position.X -= 20f;
			if (Vector2.Distance(position,new Vector2(Main.screenWidth/2,Main.screenHeight/2)) < 50f || Vector2.Distance(position,new Vector2(0,Main.screenHeight/2)) < 20f) {
				if (Deltarune.intro == 0) {Deltarune.intro = -1;}
				return false;
			}
			return true;
		}
		public override void Draw(SpriteBatch spriteBatch) {
			int frame = timeLeft % (frameCounter*maxFrame);
			frame = frame / frameCounter;
			Texture2D tt = ModContent.GetTexture("Deltarune/Content/NPCs/Lancer_Bike");
			spriteBatch.Draw(tt, position, tt.GetFrame(frame,maxFrame), Color.White, 0f, tt.GetFrame(frame,maxFrame).Size()/2, 2f, SpriteEffects.None, 0f);
			//Main.spriteBatch.Draw(tt, position, null, Color.White, 0, tt.Size()/2, 1f, SpriteEffects.None, 0f);
		}
		public static void New() {
			CustomEntity.New(new LancerIntro(),new Vector2(Main.screenWidth,Main.screenHeight/2));
			Main.PlaySound(Deltarune.GetSound("lancerwhistle"));
		}
	}
	public class Explode : BitEntity
	{
		float scale;
		const int maxFrame = 17;
		const int frameCounter = 5;
		public override bool Update() {
			timeLeft++;
			if (timeLeft >= maxFrame * frameCounter) {
				timeLeft = maxFrame * frameCounter;
				return false;
			}
			return true;
		}
		public override void Draw(SpriteBatch spriteBatch) {
			int frame = timeLeft / frameCounter;
			Texture2D tt = ModContent.GetTexture(Deltarune.textureExtra+"Explode_ButBad");
			if (scale == -1f) {
				spriteBatch.Draw(tt, new Vector2(Main.screenWidth/2,Main.screenHeight/2), tt.GetFrame(frame,maxFrame), Color.White, 0, tt.GetFrame(frame,maxFrame).Size()/2, 3f, SpriteEffects.None, 0f);
				return;
			}
			spriteBatch.Draw(tt, position - Main.screenPosition, tt.GetFrame(frame,maxFrame), Color.White, 0, tt.GetFrame(frame,maxFrame).Size()/2, scale, SpriteEffects.None, 0f);
		}
		public static void BoomScreen() {
			var bit = new Explode();
			bit.scale = -1f;
			CustomEntity.New(bit);
			Main.PlaySound(Deltarune.GetSound("badexplosion"));
		}
		public static void New(Vector2 position,float scale = 0.3f) {
			if (Vector2.Distance(Main.LocalPlayer.Center,position) > 3000f) {return;}
			var bit = new Explode();
			bit.scale = scale;
			CustomEntity.New(bit,position + new Vector2(Main.rand.Next(-10,11),Main.rand.Next(-10,11)));
			Main.PlaySound(Deltarune.GetSound("badexplosion"),position);
		}
	}
	// Bit Entity are entity but only has 2 property
	public abstract class BitEntity
	{
		public Vector2 position;
		public int timeLeft = 0;
		public override string ToString() {
			return $"time : {timeLeft}, position : {position}";
		}
		public virtual bool Update() => true;
		public virtual void Draw(SpriteBatch spriteBatch) {}
		public virtual bool OnSpawn() => true;
	}
	
	/*

	oh the misery
	everybody wants to be my enemy
	spare the simpathy
	everybody wants to be
	my enemy
	wee oo wee oo wee
	my enemy
	wee oo wee oo wee
	my enemy

	public class Explode : DeltaSystem
	{
		public static List<Explode> explodeList = new List<Explode>();

		public override void Load() {
			explodeList = new List<Explode>();
		}
		public override void PreSaveAndQuit() {
			Load();
		}
		public override void Unload() {
			explodeList = null;
		}

		public int frame;
		public int maxFrame;
		public int frameCounter;
		public int type;
		public Vector2 position;
		public float scale;
		public int time;

		public bool Update(){
			if (type == 2) {this.maxFrame = 6;}
			else {this.maxFrame = 17;}
			if (type == 2) {
				position.X -= 20f;
				if (Vector2.Distance(position,new Vector2(Main.screenWidth/2,Main.screenHeight/2)) < 50f) {
					if (Deltarune.intro == 0) {Deltarune.intro = -1;}
					return false;
				}
				if (time > 120) {
					return false;
				}
				if (frame == maxFrame){frame = 0;}
			}
			else {
				if (frame == maxFrame){
					return false;
				}
			}
			time++;
			frameCounter++;
			if (frameCounter > 5) {
				frame++;
				frameCounter = 0;
			}
			return true;
		}

		public static void UpdateAll()
		{
			List<Explode> ded = new List<Explode>();
			foreach (var explode in explodeList){
				if (!explode.Update()) {ded.Add(explode);}
			}
			foreach (var explode in ded) {
				explodeList.Remove(explode);
			}
		}
		public static void DrawAll() {
			foreach (var explode in explodeList){
				explode.Draw();
			}
		}
		public static void BoomScreen(float scale = 1f) {
			explodeList.Add(new Explode(Vector2.Zero,scale,1));
			Main.PlaySound(ModContent.GetInstance<Deltarune>().GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_badexplosion"));
		}
		public Explode(Vector2 pos, float scale = 0.3f, int type = 0) {
			this.position = pos;
			this.scale = scale;
			this.type = type;
		}
		public static void New(Vector2 pos,float scale = 0.3f, bool rand = true, int type = 0) 
		{
			if (Main.netMode == 2){return;}
			if (Vector2.Distance(Main.LocalPlayer.Center,pos) > 1500f) {return;}

			Vector2 num = rand ? 
			new Vector2(Main.rand.Next(-10,11),Main.rand.Next(-10,11)) : new Vector2(0,0);

			explodeList.Add(new Explode(pos + num,
			scale,type));
			if (type == 2) {
				Main.PlaySound(ModContent.GetInstance<Deltarune>().GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_lancerwhistle"));
			}
			else {
				Main.PlaySound(ModContent.GetInstance<Deltarune>().GetLegacySoundSlot(SoundType.Custom, "Sounds/snd_badexplosion"),pos);
			}
		}
		public void Draw()
		{
			if (type == 2) {this.maxFrame = 6;}
			else {this.maxFrame = 17;}
			if (frame >= maxFrame && type != 2) {return;}
			string path = Deltarune.textureExtra+"Explode_ButBad";
			if (type == 2) {path = "Deltarune/Content/NPCs/Lancer_Bike";}
			Texture2D tt = ModContent.GetTexture(path);
			Main.spriteBatch.BeginNormal();
			Vector2 pos = position - Main.screenPosition;
			if (type == 1) {pos = new Vector2(Main.screenWidth/2,Main.screenHeight/2);}
			if (type == 2) {pos = position;}
			Main.spriteBatch.Draw(tt, pos, tt.GetFrame(frame,maxFrame), Color.White, 0, tt.GetFrame(frame,maxFrame).Size()/2, scale, SpriteEffects.None, 0f);
			Main.spriteBatch.End();

		}
	}
	*/
}