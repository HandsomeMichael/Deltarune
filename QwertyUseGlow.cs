using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Deltarune.Helper;

namespace Deltarune
{
    //interface used for custom glow drawing
    public interface IUseGlow{
        void UseGlow(ref Vector2 pos, ref Rectangle? rec, ref Color color,ref float rotation,ref Vector2 orig,ref float scale,ref int shader);
    }
    // the class that handle use glow
    public class ItemUseGlow : ILoadable
    {
        // the data type that will be stored as
        public class ItemGlow
        {
            public Vector2 offset;
            public int type;
            public string texture;
            public ItemGlow(int type, string texture,Vector2 offset) {
                this.type = type;
                this.texture = texture;
                this.offset = offset;
            }
            public ItemGlow(int type, string texture) {
                this.type = type;
                this.texture = texture;
                this.offset = Vector2.Zero;
            }
        }

        // the list of glow data
        public static List<ItemGlow> useGlow;

        // get data from an item type
        public static ItemGlow Get(int type) {
            foreach (var item in useGlow){
                if (item.type == type) {
                    return item;
                }
            }
            return null;
        }
        // get data from an item type
        public static bool TryGet(int type,out ItemGlow output) {
            output = Get(type);
            return output != null;
        }

        public void Load() => useGlow = new List<ItemGlow>();
        public void Unload() => useGlow = null;

        /// <summary>
		/// make item itself glow when used
		/// </summary>
        public static void SelfGlow(ModItem item,int x = 0, int y = 0,string glow = "_Glow") {
            if (!Main.dedServ){
                useGlow.Add(new ItemGlow(item.item.type,item.Texture+glow,new Vector2(x,y)));
            }
        }

        /// <summary>
		/// make item itself glow when used
		/// </summary>
        public static void SelfGlow(ModItem item,string glow) => SelfGlow(item,0,0,glow);

        /// <summary>
		/// epic stolen code moment
		/// </summary>
        public static readonly PlayerLayer ItemUseGlowLayer = new PlayerLayer("Deltarune(PortedQwerty)", "ItemUseGlow", PlayerLayer.HeldItem, delegate (PlayerDrawInfo drawInfo)
        {
            //prevent shadow draw it
            if (drawInfo.shadow != 0f) {
				return;
			}
            Player drawPlayer = drawInfo.drawPlayer;
            Mod mod = Deltarune.get;
            // prevent draw for empty item and only when item is used
            if (!drawPlayer.HeldItem.IsAir && drawPlayer.itemAnimation > 0)
            {
                // get useglow data, and if it doesnt have one then dont assign any drawlayer
                Item item = drawPlayer.HeldItem;
                if (TryGet(item.type,out ItemGlow glowData))
                {
                    Texture2D texture = ModContent.GetTexture(glowData.texture);
                    float glowOffsetX = glowData.offset.X;
                    float glowOffsetY = glowData.offset.Y;
                    Vector2 location = drawInfo.itemLocation;
                    // Draw glow for style 5 ( gun / staff )
                    if (item.useStyle == 5)
                    {
                        Texture2D itemTexture = Main.itemTexture[item.type];
                        // Draw staff glow
                        if (Item.staff[item.type])
                        {
                            float rotation = drawPlayer.itemRotation + 0.785f * (float)drawPlayer.direction;
                            int width = 0;
                            Vector2 origin = new Vector2(0f, (float)itemTexture.Height);

                            if (drawPlayer.gravDir == -1f){
                                if (drawPlayer.direction == -1){
                                    rotation += 1.57f;
                                    origin = new Vector2((float)itemTexture.Width, 0f);
                                    width -= itemTexture.Width;
                                }
                                else{
                                    rotation -= 1.57f;
                                    origin = Vector2.Zero;
                                }
                            }
                            else if (drawPlayer.direction == -1){
                                origin = new Vector2((float)itemTexture.Width, (float)itemTexture.Height);
                                width -= itemTexture.Width;
                            }
                            Vector2 pos = new Vector2((float)((int)(location.X - Main.screenPosition.X + origin.X + (float)width)), (float)((int)(location.Y - Main.screenPosition.Y)));
                            Rectangle? rec = new Rectangle(0, 0, itemTexture.Width, itemTexture.Height);
                            Color color = Color.White;
                            float scale = item.scale;
                            int shader = 0;
                            if (item.modItem != null && item.modItem is IUseGlow glow) {
                                glow.UseGlow(ref pos,ref rec,ref color,ref rotation,ref origin, ref scale,ref shader);
                            }
                            var value = new DrawData(texture,pos, rec, color, rotation, origin, scale, drawInfo.spriteEffects, 0);
                            value.shader = shader;
                            Main.playerDrawData.Add(value);
                        }
                        // Draw gun
                        else
                        {
                            Vector2 pos = new Vector2((float)(itemTexture.Width / 2), (float)(itemTexture.Height / 2));

                            //Vector2 offset = this.DrawPlayerItemPos(drawPlayer.gravDir, item.type);
                            Vector2 offset = new Vector2(10, texture.Height / 2);
                            if (glowOffsetX != 0){offset.X = glowOffsetX;}
                            offset.Y += glowOffsetY * drawPlayer.gravDir;
                            pos.Y = offset.Y;
                            Vector2 orig = new Vector2((float)(-offset.X), (float)(itemTexture.Height / 2));
                            if (drawPlayer.direction == -1){
                                orig = new Vector2((float)(itemTexture.Width + (int)offset.X), (float)(itemTexture.Height / 2));
                            }
                            //value = new DrawData(itemTexture, new Vector2((float)((int)(value2.X - Main.screenPosition.X + pos.X)), (float)((int)(value2.Y - Main.screenPosition.Y + pos.Y))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, itemTexture.Width, itemTexture.Height)), item.GetAlpha(color37), drawPlayer.itemRotation, orig, item.scale, effect, 0);
                            //Main.playerDrawData.Add(value);
                            pos = new Vector2((float)((int)(location.X - Main.screenPosition.X + pos.X)), (float)((int)(location.Y - Main.screenPosition.Y + pos.Y)));
                            Rectangle? rec = new Rectangle(0, 0, itemTexture.Width, itemTexture.Height);
                            Color color = Color.White;
                            float rotation = drawPlayer.itemRotation;
                            float scale = item.scale;
                            int shader = 0;
                            if (item.modItem != null && item.modItem is IUseGlow glow) {
                                glow.UseGlow(ref pos,ref rec,ref color,ref rotation,ref orig, ref scale,ref shader);
                            }
                            var value = new DrawData(texture, pos, rec, color, rotation, orig, scale, drawInfo.spriteEffects, 0);
                            value.shader = shader;
                            Main.playerDrawData.Add(value);
                        }
                    }
                    //Draw glow for melee, eat and hold
                    else
                    {
                        Vector2 pos = new Vector2((float)((int)(location.X - Main.screenPosition.X)),
                            (float)((int)(location.Y - Main.screenPosition.Y)));
                        Rectangle? rec = new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height));
                        Color color = Color.White;
                        float rotation = drawPlayer.itemRotation;
                        Vector2 orig = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * (float)drawPlayer.direction, drawPlayer.gravDir == -1 ? 0f : texture.Height);
                        float scale = item.scale;
                        int shader = 0;
                        if (item.modItem != null && item.modItem is IUseGlow glow) {
                            glow.UseGlow(ref pos,ref rec,ref color,ref rotation,ref orig, ref scale,ref shader);
                        }
                        var value = new DrawData(texture,pos,rec,color,rotation,orig,scale,drawInfo.spriteEffects,0);
                        value.shader = shader;
                        Main.playerDrawData.Add(value);
                    }
                }
            }
        });
    }
}