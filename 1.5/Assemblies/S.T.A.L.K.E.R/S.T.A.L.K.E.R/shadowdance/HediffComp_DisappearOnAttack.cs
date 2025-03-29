using RimWorld;
using Verse;
using Verse.Sound;


public class HediffComp_DisappearOnAttack : HediffComp
{
    private bool activationSoundPlayed = false;
    private readonly int ticksUntilRemoval = 900; // Пример: 15 секунд (900 тиков, если тик 60 кадров в секунду)
    private int ticksPassed = 0;
    private bool shouldRemoveHediff = false; // Флаг для отложенного удаления

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);

        // Увеличиваем количество тикеров, прошедших с момента активации
        ticksPassed++;

        // Проверяем, нужно ли удалить хеддиф
        if (shouldRemoveHediff)
        {
            // Удаляем хеддиф, если он существует
            if (parent != null)
            {
                Pawn.health.RemoveHediff(parent);

                // Сбрасываем состояние
                activationSoundPlayed = false; // Сбрасываем состояние звука
                ticksPassed = 0; // Сбрасываем счетчик
                shouldRemoveHediff = false; // Сбрасываем флаг
            }
            return; // Выходим из метода
        }

        // Проверяем, если хеддиф ещё не активирован
        if (!activationSoundPlayed)
        {
            // Проигрываем звук активации только один раз при запуске способности
            SoundDef.Named("ShadowDanceInvisability").PlayOneShot(new TargetInfo(Pawn.Position, Pawn.Map));
            activationSoundPlayed = true;

            // Добавляем хеддиф для невидимости, если он еще не добавлен
            var hediffDef = HediffDef.Named("SDance");
            if (!Pawn.health.hediffSet.HasHediff(hediffDef))
            {
                Pawn.health.AddHediff(hediffDef);
            }
        }

        // Проверяем, достаточно ли прошло времени для удаления хеддифа
        if (ticksPassed >= ticksUntilRemoval)
        {
            shouldRemoveHediff = true; // Устанавливаем флаг для удаления
        }
    }

    public override void Notify_PawnUsedVerb(Verb verb, LocalTargetInfo target)
    {
        base.Notify_PawnUsedVerb(verb, target);

        // Проверяем, что персонаж использует настоящий верб атаки, а не способность
        if (verb != null && target.Thing != null && verb.CasterPawn == Pawn && (verb is Verb_MeleeAttack || verb is Verb_LaunchProjectile))
        {
            shouldRemoveHediff = true; // Устанавливаем флаг для удаления
        }
    }


    public override void CompPostPostRemoved()
    {
        base.CompPostPostRemoved();

        // Проигрываем звук по окончании невидимости только если хеддиф был удален
        if (activationSoundPlayed)
        {
            SoundDef.Named("ShadowDanceInvisabilityEnd").PlayOneShot(new TargetInfo(Pawn.Position, Pawn.Map));
        }
    }
}
