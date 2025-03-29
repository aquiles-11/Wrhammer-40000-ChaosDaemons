using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;

namespace S.T.A.L.K.E.R
{
    public static class PawnKindDefOf
    {
        [MayRequireAnomaly]
        public static PawnKindDef STLKR_FeralZombie;

        static PawnKindDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PawnKindDefOf));
        }
    }

}
