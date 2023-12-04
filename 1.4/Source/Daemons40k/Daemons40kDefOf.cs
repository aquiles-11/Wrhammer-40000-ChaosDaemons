using RimWorld;
using Verse;

namespace Daemons40k
{
    [DefOf]
    public static class Daemons40kDefOf
    {
        public static FactionDef BEWH_ChaosFactionHidden;

        public static MentalStateDef BEWH_ManhunterDaemon;

        static Daemons40kDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(Daemons40kDefOf));
        }
    }
}