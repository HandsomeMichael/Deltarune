
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
using Deltarune.Content.NPCs.RalseiDummy;

namespace Deltarune
{
	public enum NetType
	{
		Player,
		Teleport,
		KillRalsei,
		//GlobalNPC
	}
	public class NetCode
	{
		public static ModPacket GetPacket() => Deltarune.get.GetPacket();

		public static void Send(NetType net) {
			var netMessage = GetPacket();
			netMessage.Write((byte)net);
			netMessage.Send();
		}

		public static void HandlePacket(BinaryReader reader, int whoAmI) {
			NetType msgType = (NetType)reader.ReadByte();
			switch (msgType) {
				case NetType.Player:
					Main.player[reader.ReadByte()].GetDelta().HandlePacket(reader);
					break;
				/*
				
				removed 

				case NetType.GlobalNPC:
					// honestly idk what am doing
					int index = reader.ReadByte();
					Main.npc[index].GetDelta().HandlePacket(reader);
					if (Main.netMode == NetmodeID.Server) {
						var packet = GetPacket();
						packet.Write((byte)NetType.GlobalNPC);
						Main.npc[index].GetDelta().SendPacket(packet);
						packet.Send(-1, whoAmI);
					}
					break;
				*/
				case NetType.Teleport:
					int playernumber = reader.ReadByte();
					Player player = Main.player[playernumber];
					player.position.X = reader.ReadSingle();
					player.position.Y = reader.ReadSingle();
					//Unlike SyncPlayer, here we have to relay/forward these changes to all other connected clients
					if (Main.netMode == NetmodeID.Server) {
						var packet = GetPacket();
						packet.Write((byte)NetType.Teleport);
						packet.Write(playernumber);
						packet.Write(player.position.X);
						packet.Write(player.position.Y);
						packet.Send(-1, playernumber);
					}
					break;
				case NetType.KillRalsei:
					if (Main.netMode == NetmodeID.Server){
                        for (int i = 0; i < Main.maxNPCs; i++){
                            if (Main.npc[i].active && Main.npc[i].modNPC != null && Main.npc[i].modNPC is ralseidummy ralsei){
                                ralsei.CanDie = true;
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i);
                            }
                        }
                    }
					break;
				default:
					ModContent.GetInstance<Deltarune>().Logger.WarnFormat("Deltarune: Unknown Message type: {0}", msgType);
					break;
			}
		}
	}
}
