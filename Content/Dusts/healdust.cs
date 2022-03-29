using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Deltarune.Content.Dusts
{
	public class healdust : ModDust
	{
		public static void Spawn(Vector2 pos, int amount = 10) {
			for (int a = 0; a < amount; a++) {
				Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
				Dust d = Dust.NewDustPerfect(pos, ModContent.DustType<healdust>(), speed * Main.rand.NextFloat(3f));
				d.noGravity = true;
			}
		}
		public override void OnSpawn(Dust dust){
			int frame = 5;
			dust.noGravity = true;
			dust.frame = new Rectangle(0, Texture.Height / frame * Main.rand.Next(frame), Texture.Width, Texture.Height / frame);
			dust.scale = 1.2f;
			dust.noLight = true;
		}
		public override bool Update(Dust dust){
			dust.alpha -= 5;
			dust.velocity *= 0.98f;
			dust.position += dust.velocity;
			dust.scale -= 0.03f;

			if (dust.scale <= 0)
				dust.active = false;
			Lighting.AddLight(dust.position, Color.Green.ToVector3() * 0.58f);
			return false;
		}
	}

	public class healdust2 : ModDust
	{
		public static void Spawn(Vector2 pos, int amount = 10) {
			for (int a = 0; a < amount; a++) {
				Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
				Dust d = Dust.NewDustPerfect(pos, ModContent.DustType<healdust2>(), speed * Main.rand.NextFloat(3f));
				d.noGravity = true;
			}
		}
		public override void OnSpawn(Dust dust){
			int frame = 5;
			dust.noGravity = true;
			dust.frame = new Rectangle(0, Texture.Height / frame * Main.rand.Next(frame), Texture.Width, Texture.Height / frame);
			dust.scale = 1.2f;
			dust.noLight = true;
		}
		public override bool Update(Dust dust){
			dust.alpha -= 5;
			dust.velocity *= 0.98f;
			dust.position += dust.velocity;
			dust.scale -= 0.03f;

			if (dust.scale <= 0)
				dust.active = false;
			Lighting.AddLight(dust.position, Color.Green.ToVector3() * 0.58f);
			return false;
		}
	}
}