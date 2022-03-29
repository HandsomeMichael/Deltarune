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
	/// epic frame struct , reject classes return to unnullable struct 
	/// </summary>
    public struct FrameHelp
    {
        /// <summary>
		/// the frame
		/// </summary>
        internal int frame;

        /// <summary>
		/// the frame counter
		/// </summary>
        internal int frameCounter;

        /// <summary>
		/// the method to get origin, requires Helpme class to work
		/// </summary>
        public Vector2 Origin(Texture2D texture, int maxFrame) => this.GetFrame(texture,maxFrame).Size()/2f;

        /// <summary>
		/// the method to get Rectangle frame, requires Helpme class to work
		/// </summary>
        public Rectangle GetFrame(Texture2D texture, int maxFrame) => texture.GetFrame(frame,maxFrame);

        /// <summary>
		/// the method to update the frame so its actually animated
		/// </summary>
        public void Update(int maxFrame,int maxFrameCounter = 5,int reset = 0,bool alwaysReset = true) {
            frameCounter++;
            if (frameCounter > maxFrameCounter) {
                frameCounter = 0;
                frame++;
                if (!alwaysReset) {if (frame >= maxFrame) {frame = reset;}}
            }
            if (alwaysReset) {if (frame >= maxFrame) {frame = reset;}}
        }
        /// <summary>
		/// the method to reset the frame to 0
		/// </summary>
        public void Reset() {
            frame = 0;
            frameCounter = 0;
        }

        public FrameHelp(int frame = 0, int frameCounter = 0) {
            this.frame = frame;
            this.frameCounter = frameCounter;
        }
    }
}
