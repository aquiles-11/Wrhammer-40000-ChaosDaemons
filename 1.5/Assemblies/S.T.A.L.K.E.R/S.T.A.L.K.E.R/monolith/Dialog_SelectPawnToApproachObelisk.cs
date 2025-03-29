using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

public class Dialog_SelectPawnToApproachObelisk : Window
{
    private readonly CompObelisk_Xenochanger obeliskComp;
    public override Vector2 InitialSize => new Vector2(500f, 600f);

    public Dialog_SelectPawnToApproachObelisk(CompObelisk_Xenochanger comp)
    {
        obeliskComp = comp;
        forcePause = true;
        absorbInputAroundWindow = true;
    }

    public override void DoWindowContents(Rect inRect)
    {
        Listing_Standard listingStandard = new Listing_Standard();
        listingStandard.Begin(inRect);

        listingStandard.Label("Выберите пешку, которая подойдет к обелиску:");

        foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonistsSpawned)
        {
            if (listingStandard.ButtonText(pawn.Name.ToStringFull))
            {
                obeliskComp.SendPawnToObelisk(pawn);
                Close();
            }
        }

        listingStandard.End();
    }
}

