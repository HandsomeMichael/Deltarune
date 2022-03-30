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
	/// A struct for trails that has positions and rotations
	/// this struct doesnt have any constructor
	/// </summary>
	public class Trail
	{
		/// <summary>
		/// the trail data, contains the positions and rotations
		/// </summary>
		public List<KeyValuePair<Vector2,float>> values;

		public Trail() {
			this.values = new List<KeyValuePair<Vector2,float>>();
		}

        /// <summary>
		/// method to updates the trail
		/// </summary>
		public void Update(Vector2 pos,float rot, int maxTrail = 10,int speed = 5) {
			if (Main.GameUpdateCount % speed == 0) {
				values.Add(new KeyValuePair<Vector2,float>(pos,rot));
				if (values.Count > maxTrail) {values.RemoveAt(0);}
			}
		}

        /// <summary>
		/// method to updates the trail
		/// </summary>
		public void UpdateWorm(Vector2 pos,float rot = 0f, float intensity = 0.1f,int maxTrail = 10,int speed = 5) {
			if (Main.GameUpdateCount % speed == 0) {
                values[values.Count-1] = new KeyValuePair<Vector2,float>(values[values.Count-1].Key,MathHelper.Lerp(values[values.Count-1].Value,values[values.Count-1].Key.AngleTo(pos),intensity));
				values.Add(new KeyValuePair<Vector2,float>(pos,rot));
				if (values.Count > maxTrail) {values.RemoveAt(0);}
			}
		}
	}
}
