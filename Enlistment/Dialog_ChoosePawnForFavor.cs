using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ChickenCore.Enlistment
{
    public class Dialog_ChoosePawnForFavor : Window
    {
        private List<Pawn> pawns;
        private Action<Pawn> onPawnSelected;

        private Vector2 scrollPosition;

        public Dialog_ChoosePawnForFavor(List<Pawn> pawns, Action<Pawn> onPawnSelected)
        {
            this.pawns = pawns;
            this.onPawnSelected = onPawnSelected;
            doCloseX = true;
            closeOnClickedOutside = true;
        }

        public override Vector2 InitialSize => new Vector2(400f, 600f);

        public override void DoWindowContents(Rect inRect)
        {
            const float titleHeight = 40f;
            const float titleMargin = 10f;
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0, 0, inRect.width, titleHeight), "ChoosePawnForFavorTitle".Translate());
            Text.Font = GameFont.Small;
            Rect viewRect = new Rect(0, titleHeight + titleMargin, inRect.width, inRect.height - (titleHeight + titleMargin));
            Rect scrollRect = new Rect(0, 0, viewRect.width - 16f, pawns.Count * 30f);
            Widgets.BeginScrollView(viewRect, ref scrollPosition, scrollRect);

            float y = 0f;
            foreach (var pawn in pawns)
            {
                Rect pawnRect = new Rect(0, y, scrollRect.width, 30f);
                if (Widgets.ButtonText(pawnRect, pawn.Name.ToStringShort))
                {
                    onPawnSelected?.Invoke(pawn);
                    Close();
                }
                y += 30f;
            }

            Widgets.EndScrollView();
        }
    }
}
