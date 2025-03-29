using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

public class Gene_Maneater : Gene
{
    public override void PostAdd()
    {
        base.PostAdd();
        RemoveNegativeThoughts();
    }

    public override void Tick()
    {
        base.Tick();
        if (this.pawn.IsHashIntervalTick(200)) // Проверка раз в некоторое время
        {
            RemoveNegativeThoughts();
        }
    }

    private void RemoveNegativeThoughts()
    {
        // Список мыслей для подавления
        List<ThoughtDef> suppressedThoughts = new List<ThoughtDef>
        {
            DefDatabase<ThoughtDef>.GetNamed("AteHumanlikeMeatDirect"),
            DefDatabase<ThoughtDef>.GetNamed("AteHumanlikeMeatAsIngredient"),
            DefDatabase<ThoughtDef>.GetNamed("AteRawFood"),
            DefDatabase<ThoughtDef>.GetNamed("AteCorpse"),
            DefDatabase<ThoughtDef>.GetNamed("AteInsectMeatDirect"),
            DefDatabase<ThoughtDef>.GetNamed("AteKibble"),
            DefDatabase<ThoughtDef>.GetNamed("AteInsectMeatAsIngredient")
             };
        // Проверка наличия DLC Anomaly
        if (ModsConfig.IsActive("Ludeon.RimWorld.Anomaly"))
        {
            suppressedThoughts.Add(DefDatabase<ThoughtDef>.GetNamed("AteTwistedMeat"));
        }

        if (this.pawn.ideo != null)
        {
            // Удаление ограничений на поедание мяса
            foreach (var thought in suppressedThoughts)
            {
                var memory = this.pawn.needs.mood.thoughts.memories.GetFirstMemoryOfDef(thought);
                if (memory != null)
                {
                    this.pawn.needs.mood.thoughts.memories.RemoveMemory(memory);
                }
            }
        }
    }
}