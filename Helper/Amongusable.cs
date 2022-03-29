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
    /// <summary>
	/// a struct where you can only access it limited amount of time
    /// the funny "safe" code struct
    /// can be used as an 'advanced' timer
	/// </summary>
    public struct Amongusable<T> 
	{
		private T data;
		private int gettable;

		/// <summary>
        /// The Value
        /// </summary>
		public T Data {
			get {
				if (gettable > 0) {gettable--;}
				else {data = default(T);}
                return data;
            }
			set => data = value;
		}

		public Amongusable(T data, int gettable) {
			this.data = data;
			this.gettable = gettable;
		}

		public override string ToString() => (gettable > 0) ? "Sussy" : "Sus";
	}
}

