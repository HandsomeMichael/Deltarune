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
	/// idk , i copy pasted from System.Collections.Generic
	/// </summary>
	public struct KeyValueExtra<TKey, TValue,TExtra>
	{
		private TKey key;
		private TValue values;
		private TExtra extra;

		/// <summary>
		/// the key value
		/// </summary>
		public TKey Key {get => key;set => key = value;}
		/// <summary>
		/// the value
		/// </summary>
		public TValue Value {get => values;set => values = value;}
		/// <summary>
		/// the extra value
		/// </summary>
		public TExtra Extra {get => extra;set => extra = value;}

		public KeyValueExtra(TKey key ,TValue values,TExtra extra) {
			this.key = key;
			this.values = values;
			this.extra = extra;
		}
	}
}
