using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Deltarune.Content.Dusts
{
	public class SoulBreak : ModDust
	{
		public override void OnSpawn(Dust dust){
			int frame = 4;
			dust.frame = new Rectangle(0, Texture.Height / frame * Main.rand.Next(frame), Texture.Width, Texture.Height / frame);
			dust.scale = 1f;
		}
		public override bool Update(Dust dust){
			float speed = 1f;
			float gfx = 1f;
			Collision.StepUp(ref dust.position, ref dust.velocity, 5, 5, ref speed, ref gfx);
			dust.alpha += 1;
			dust.scale -= 0.01f;
			dust.rotation += dust.velocity.X > 0f ? 0.01f : -0.01f;
			dust.velocity.Y += 0.1f;
			dust.position += dust.velocity;
			Lighting.AddLight(dust.position, Color.Red.ToVector3() * 0.48f);
			if (dust.alpha >= 255)
				dust.active = false;
			return false;
		}
	}
}