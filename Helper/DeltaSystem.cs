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
    public abstract class DeltaSystem 
    {
        public string Name;
        public virtual bool Autoload(ref string name) => true;
        public virtual void PreSaveAndQuit() {}
        public virtual void Load() {}
        public virtual void Unload() {}
    }
    public static class DeltaSystemLoader
    {
        public static List<DeltaSystem> systems = new List<DeltaSystem>();
        public static void Load(Mod mod) {
            if (mod.Code == null)
				return;

            //guh i forgor
            systems = new List<DeltaSystem>();
            foreach (Type type in mod.Code.GetTypes().OrderBy(type => type.FullName))
			{
				if (type.IsAbstract){continue;}
				if (type.GetConstructor(Type.EmptyTypes) != null && type.IsSubclassOf(typeof(DeltaSystem)))
				{
					var system = (DeltaSystem)Activator.CreateInstance(type);
					var name = type.Name;
					if (system.Autoload(ref name)) {
                        system.Name = name;
                        mod.Logger.InfoFormat($"{mod.Name} Loading System : {name}");
                        system.Load();
                        systems.Add(system);
                    }
				}
                
			}
        }
        public static void PreSaveAndQuit() {
            foreach (var item in systems){
                item.PreSaveAndQuit();
            }
        }
        public static void Unload() {
            foreach (var item in systems){
                item.Unload();
            }
            systems = null;
        }
    }
    /*
    public static class DeltaSystemLoader
    {
        public static List<ILoadable> loader;

        public static void Load(Mod mod) {
            if (mod.Code == null)
				return;

            //guh i forgor
            loader = new List<ILoadable>();

            foreach (Type type in mod.Code.GetTypes().OrderBy(type => type.FullName))
			{
                if (type.IsAbstract) {continue;}
                if (type is ILoadable load) {
                    mod.Logger.InfoFormat($"{mod.Name} Loading System : {type.Name}");
                    var foo = (ILoadable)Activator.CreateInstance(load);
                    loader.Load();
                    loader.Add(foo);
                }
                
			}
        }
        public static void PreSaveAndQuit() {
            foreach (var item in loader){
                item.PreSaveAndQuit();
            }
        }
        public static void Unload() {
            foreach (var item in loader){
                item.Unload();
            }
            musicLoad = null;
            loader = null;
        }
    }
    */
}
