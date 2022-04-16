using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader.IO;
using Terraria.Localization;
using Terraria.Graphics.Shaders;
using Terraria.ObjectData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Deltarune;
using Deltarune.Content;

namespace Deltarune.Helper
{
    public interface IAdditive{
        void DrawAdditive(SpriteBatch spritebatch);
    }
    public interface ILittleTrolling {
        void Troll();
    }
    public interface IProjBlinder{
        bool CanBeSeen(bool normalReturn,Projectile projectile);
    }
    public interface IProjhittable{
        bool OwnerMeleeCount(GlobeProj proj);
        bool CanMeleeCollide(GlobeProj proj);
        void OnMeleeCollide(GlobeProj proj,Player player,Item item);
    }
    public interface ISoundText{
        void CustomSound(ref int textsound);
    }

    public interface ICustomglow{
        void UseGlow(ref Vector2 pos, ref Rectangle? rec, ref Color color,ref float rotation,ref Vector2 orig,ref float scale);
    }

    public interface ICustomHead{
        void CustomHead(ref int expression,ref float scale);
    }
    
    [AttributeUsage(AttributeTargets.Class)]
	public class ExampleAttribute : Attribute
	{
	}
}
