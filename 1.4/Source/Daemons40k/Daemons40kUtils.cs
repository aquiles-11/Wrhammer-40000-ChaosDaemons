using System.Collections.Generic;
using Verse;

namespace Daemons40k
{
    public class Daemons40kUtils
    {
        public struct ChaosInvasionParams
        {
            //Pawnkind to spawn
            public List<PawnKindDef> pawnKind;
            //Range of the amount of pawns there should spawn, can scale on stuff?
            public IntRange amountRange;
        }
    }
}