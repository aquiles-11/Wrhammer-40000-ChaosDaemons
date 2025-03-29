﻿using HarmonyLib;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace  ChickenCore.Enlistment
{
    class EnlistMod : Mod
    {
        public static EnlistSettings settings;
        public EnlistMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<EnlistSettings>();
            new Harmony("ChickenPlucker.Enlist").PatchAll();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            EnlistUtils.DoDefsRemoval();
        }
        public override string SettingsCategory()
        {
            return "Enlistment";
        }
    }
}
