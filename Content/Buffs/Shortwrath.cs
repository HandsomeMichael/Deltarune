using Terraria;
using Terraria.ModLoader;

namespace Deltarune.Content.Buffs
{
    public class Shortwrath : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Shortwrath");
            Description.SetDefault("Summon multiple shortswords when stabbing");
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false; //Add this so the nurse doesn't remove the buff when healing
        }
    }
    public class Stabwrath : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Stabwrath");
            Description.SetDefault("Summon multiple homing shortswords when stabbing");
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false; //Add this so the nurse doesn't remove the buff when healing
        }
        public override void Update(Player player, ref int buffIndex) {
            if (player.HasBuff(ModContent.BuffType<Shortwrath>())) {
                player.ClearBuff(ModContent.BuffType<Shortwrath>());
            }
		}
    }
    public class MuraDash : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Mura Dash Cooldown");
            Description.SetDefault("Cannot dash using mura weapon");
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;
        }
    }
    /*
    public class Stabber : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Stabber");
            Description.SetDefault("Increases shortsword damage by 30%\nRemoved when taking any damage");
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false; //Add this so the nurse doesn't remove the buff when healing
        }
    }
    public class Stabshield : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Stab shield");
            Description.SetDefault("Create damaging shortsword shield around the player");
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false; //Add this so the nurse doesn't remove the buff when healing
        }
    }*/
}
