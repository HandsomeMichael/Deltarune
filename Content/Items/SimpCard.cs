using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace Deltarune.Content.Items
{
	public class SimpCard : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Nurse Unofficial Simp Card");
			Tooltip.SetDefault("'After seeing this card , you are filled with determination'");
		}
		public override void SetDefaults() 
		{
            item.width = 14;
            item.height = 14;
            item.rare = 3;
		}
	}
}
