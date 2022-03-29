using Terraria;
using Terraria.ModLoader;
using Deltarune.Helper;
using Deltarune.Content;

namespace Deltarune.Content.Buffs
{
    public class fatalbleed : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Fatal Bleed");
            Description.SetDefault("Increased true melee damage taken by 100%");
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true; //Add this so the nurse doesn't remove the buff when healing
        }
        public override void Update(NPC npc, ref int buffIndex) {
            npc.GetDelta().fatalbleed = true;
		}
    }
}
