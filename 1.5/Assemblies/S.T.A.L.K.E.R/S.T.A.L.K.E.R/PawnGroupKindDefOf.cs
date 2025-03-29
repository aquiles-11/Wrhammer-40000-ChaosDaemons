using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;

namespace S.T.A.L.K.E.R
{
    public static class PawnGroupKindDefOf
    {
        [MayRequireAnomaly]
        public static PawnGroupKindDef STLKR_FeralZombies;

        static PawnGroupKindDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PawnGroupKindDefOf));
        }
    }

}
