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
    // interfaces implemented, reject abstract classes return to interfaces
    public interface IPreSaveAndQuit{void PreSaveAndQuit();}
    public interface ILoadOnly{void Load();}
    public interface ILoadable {void Load();void Unload();}
    public interface ILoggable {string Log(Action<string> log);}

    public static class DeltaSystemLoader
    {
        // the caches , used for Unload and other hook
        public static List<ILoadable> systems = new List<ILoadable>();

        public static void Load(Mod mod) {
            // this is required
            if (mod.Code == null)
				return;

            //intialize systems
            systems = new List<ILoadable>();
            // loop over mod.Code
            foreach (Type type in mod.Code.GetTypes().OrderBy(type => type.FullName))
			{
                // dont do anything with abstract classes
				if (type.IsAbstract){continue;}

                // load ILoadable and cache it at systems
                if (type.GetInterfaces().Contains(typeof(ILoadable))) {
                    var instance = (ILoadable)Activator.CreateInstance(type);
                    mod.Logger.InfoFormat($"{mod.Name} Loading System : {type.Name}");
                    instance.Load();
                    systems.Add(instance);
                    continue;
                }

                // load ILoadOnly once
                if (type.GetInterfaces().Contains(typeof(ILoadOnly))) {
                    var instance = (ILoadOnly)Activator.CreateInstance(type);
                    mod.Logger.InfoFormat($"{mod.Name} Load Only System : {type.Name}");
                    instance.Load();
                }
                //type.GetConstructor(Type.EmptyTypes)
                
			}
        }
        public static void PreSaveAndQuit() {
            foreach (var item in systems){
                if (item is IPreSaveAndQuit hook) {
                    Deltarune.get.Logger.InfoFormat($"PreSaveQuit System : {hook.GetType().Name}");
                    hook.PreSaveAndQuit();
                }
            }
        }
        public static void Unload() {
            foreach (var item in systems){
                item.Unload();
            }
            systems = null;
        }
        public static void LogAll() {
            foreach (var item in systems){
                if (item is ILoggable hook) {
                    void CreateLog(string log) {
                        Deltarune.get.Logger.InfoFormat(log);
                        Main.NewText(log);
                    }
                    Action<string> log = CreateLog;
                    hook.Log(log);
                }
            }
        }
    }
}
