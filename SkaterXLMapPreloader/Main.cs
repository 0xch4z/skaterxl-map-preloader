using UnityModManagerNet;
using UnityEngine;
using Harmony12;
using System.Reflection;

namespace SkaterXLMapPreloader
{
    class Main
    {
        public static bool enabled;

        private static MapPreloader mapPreloader;

        public static HarmonyInstance harmonyInstance;

        static void Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnToggle = OnToggle;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool flag)
        {
            if (flag == enabled) return true;
            enabled = flag;

            if (enabled)
            {
                mapPreloader = new GameObject().AddComponent<MapPreloader>();
                GameObject.DontDestroyOnLoad(mapPreloader);

                harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
                harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            } else
            {
                harmonyInstance.UnpatchAll(harmonyInstance.Id);
                if (mapPreloader != null)
                {
                    GameObject.DestroyImmediate(mapPreloader);
                }
            }
            return true;
        }
    }
}
