using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using System.Threading.Tasks;

public class IncidentWorker_MonolithCrystal : IncidentWorker_Obelisk
{
    public override ThingDef ObeliskDef => ThingDef.Named("MonolithCrystal");
}

