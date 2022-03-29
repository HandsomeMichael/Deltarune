using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using Terraria.ModLoader.Config;
using Deltarune.Content;
using Deltarune.Content.Tiles;
using Deltarune.Helper;

namespace Deltarune
{
	public class MyWorld : ModWorld
	{
		public static bool downedStarWalker;
		public static bool hadRalsei;
		public override void NetSend(BinaryWriter writer) {
			writer.Write(downedStarWalker);
			writer.Write(hadRalsei);
		}
		public override void NetReceive(BinaryReader reader) {
			downedStarWalker = reader.ReadBoolean();
			hadRalsei = reader.ReadBoolean();
		}
		public override TagCompound Save() {
			TagCompound tag = new TagCompound();
			tag.Add(nameof(downedStarWalker),downedStarWalker);
			tag.Add(nameof(hadRalsei),hadRalsei);
			return tag;
		}
		public override void Load(TagCompound tag) {
			downedStarWalker = tag.GetBool(nameof(downedStarWalker));
			hadRalsei = tag.GetBool(nameof(hadRalsei));
		}
		public override void PostWorldGen() {
			//funny world gen bc why not
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					if (WorldGen.genRand.NextBool(20)) {
						if (Main.tile[i,j].type == TileID.Copper) {
							Main.tile[i,j].type = (ushort)ModContent.TileType<CorruptedCopper>();
						}
						if (Main.tile[i,j].type == TileID.Tin) {
							Main.tile[i,j].type = (ushort)ModContent.TileType<CorruptedTin>();
						}
					}
				}
			}
		}
	}
}
