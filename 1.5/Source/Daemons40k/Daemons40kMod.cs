using HarmonyLib;
using Verse;


namespace Daemons40k
{
    public class Daemons40kMod : Mod
    {
        public static Harmony harmony;


        public Daemons40kMod(ModContentPack content) : base(content)
        {
            harmony = new Harmony("Daemons40k.Mod");
            harmony.PatchAll();
        }

    }
}