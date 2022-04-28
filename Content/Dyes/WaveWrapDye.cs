using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Deltarune.Content.Dyes
{
	public abstract class BaseDye : ModItem{
		public virtual int rare => 1;
		public virtual int value => Item.sellPrice(0, 1, 50, 0);
		public override void SetDefaults() {
			item.width = 20;
			item.height = 20;
			item.maxStack = 99;
			item.value = value;
			item.rare = rare;
		}
	}

	public class WaveWrapDye : BaseDye{public override int rare => 2;}
}
