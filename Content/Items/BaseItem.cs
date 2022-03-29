using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Deltarune.Content.Items
{
	public abstract class BaseItem : ModItem
	{
		public virtual bool UpdateInventoryOnPig => false;
		public virtual bool AutoLoadGlow => false;
	}
}