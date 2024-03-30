using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyJamMadness
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class ModBase : BaseUnityPlugin
    {
        private const string modGUID = "JellyJam.TinyMod";
        private const string modName = "JellyJamTinyMod";
        private const string modVersion = "1.0.2.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static ModBase Instance;

        internal ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("The JellyJamTinyMod has started");

            harmony.PatchAll();
        }
    }
}
