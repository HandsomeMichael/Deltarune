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
	/// A struct that used for storing trail datas
	/// </summary>
	public struct TrailData
	{
		public Vector2 position;
		public float rotation;
		public int direction;
		public TrailData(Vector2 position,float rotation,int direction) {
			this.position = position;
			this.rotation = rotation;
			this.direction = direction;
		}
	}
    /// <summary>
	/// A class for trails that has positions , rotations and directions
	/// </summary>
	public class Trail
	{
		/// <summary>
		/// the trail data, contains the positions , rotations and directions
		/// </summary>
		public List<TrailData> values;

		public Trail() {
			this.values = new List<TrailData>();
		}

		/// <summary>
		/// get trail data easily, copied from List<T> class :trolleybus:
		/// </summary>
		public TrailData this[int index]{
			get{
				CheckIndex(index);
				return values[index];
			}
			set{
				CheckIndex(index);
				values[index] = value;
			}
		}

		/// <summary>
		/// Get trail data count
		/// </summary>
		public int Count => values.Count;


		/// <summary>
		/// check if index out of bounds or not
		/// </summary>
		public void CheckIndex(int index) {
			if ((uint) index < 0 || (uint)index >= (uint)values.Count){throw new ArgumentOutOfRangeException("index", "ur index is out of bounds lol");}
		}

        /// <summary>
		/// method to updates the trail
		/// <param name="pos">the position that it will add into values.</param>
		/// <param name="rot">the rotation that it will add into values.</param>
		/// <param name="dir">the direction that it will add into values.</param>
		/// <param name="maxTrail">the max trail.</param>
		/// <param name="speed">the speed or the max timer</param>
		/// </summary>
		public void Update(Vector2 pos,float rot,int dir, int maxTrail = 10,int speed = 5) {
			if (Main.GameUpdateCount % speed == 0) {
				values.Add(new TrailData(pos,rot,dir));
				if (values.Count > maxTrail) {values.RemoveAt(0);}
			}
		}
		/// <summary>
		/// method to updates the trail
		/// <param name="entity">the entity</param>
		/// <param name="maxTrail">the max trail.</param>
		/// <param name="speed">the speed or the max timer</param>
		/// </summary>
		public void Update(Entity entity,int maxTrail = 10,int speed = 5) {
			Vector2 pos = entity.Center;
			int dir = 0;
			float rot = 0f;

			// check if entity is npc , proj or player
			if (entity is NPC npc) {dir = npc.direction;rot = npc.rotation;}
			if (entity is Projectile proj) {dir = proj.direction;rot = proj.rotation;}
			if (entity is Player player) {dir = player.direction;rot = player.itemRotation;}

			Update(pos,rot,dir,maxTrail,speed);
		}

		/*
		public void UpdateWorm(Vector2 pos,float rot = 0f, float intensity = 0.1f,int maxTrail = 10,int speed = 5) {
			if (Main.GameUpdateCount % speed == 0) {
                values[values.Count-1] = new KeyValuePair<Vector2,float>(values[values.Count-1].Key,MathHelper.Lerp(values[values.Count-1].Value,values[values.Count-1].Key.AngleTo(pos),intensity));
				values.Add(new KeyValuePair<Vector2,float>(pos,rot));
				if (values.Count > maxTrail) {values.RemoveAt(0);}
			}
		}
		*/
	}
}
