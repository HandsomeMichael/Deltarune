using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Deltarune.Content.Dusts
{
	public class StarDust : ModDust
	{
		public override void OnSpawn(Dust dust){
			int frame = 5;
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, Texture.Width, Texture.Height);
			dust.scale = dust.scale/2f;
			dust.noLight = true;
		}
		public override bool Update(Dust dust){
			dust.alpha -= 10;
			dust.velocity *= 0.98f;
			dust.position += dust.velocity;
			dust.scale -= 0.03f;

			if (dust.scale <= 0)
				dust.active = false;
			return false;
		}
	}
}