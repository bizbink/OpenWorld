﻿using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace OpenWorld
{
    public class MP_WITabFind : WITab
    {
        private Vector2 scrollPosition;

        private static readonly Vector2 WinSize = new Vector2(432f, 540f);

        public override bool IsVisible => true;

        private string titleText = "Online Settlements List";

        public MP_WITabFind()
        {
            size = WinSize;
            labelKey = "Find";
        }

        protected override void FillTab()
        {
            Rect outRect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
            Rect rect = new Rect(10f, 10f, outRect.width - 16f, Mathf.Max(0f, outRect.height));

            float horizontalLineDif = Text.CalcSize(titleText).y + 3f + 10f;

            Text.Font = GameFont.Medium;
            Widgets.Label(rect, titleText);
            Widgets.DrawLineHorizontal(rect.x, horizontalLineDif, rect.width);

            try { GenerateList(new Rect(new Vector2(rect.x, rect.y + 30f), new Vector2(rect.width, rect.height - 30f))); }
            catch { }
        }

        private void GenerateList(Rect mainRect)
        {
            var orderedDictionary = Main._ParametersCache.onlineSettlements.OrderBy(x => x.Value[0]);
            if (!Main._ParametersCache.isPlayingOnline)
            {
                Main._MPWorld.FindOnlineFactionInWorld();
                Dictionary<string, List<string>> localOnlineSettlements = new Dictionary<string, List<string>>();
                foreach (Settlement st in Find.WorldObjects.Settlements)
                {
                    if (st.Faction == Main._ParametersCache.faction) localOnlineSettlements.Add(st.Tile.ToString(), new List<string>() { st.Name.Replace("'s Settlement", "") });
                }

                orderedDictionary = localOnlineSettlements.OrderBy(x => x.Value[0]);
            }

            float height = 6f + (float)orderedDictionary.Count() * 30f;
            Rect viewRect = new Rect(mainRect.x, mainRect.y, mainRect.width - 16f, height);

            Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);

            float num = 0;
            float num2 = scrollPosition.y - 30f;
            float num3 = scrollPosition.y + mainRect.height;
            int num4 = 0;

            foreach (KeyValuePair<string, List<string>> pair in orderedDictionary)
            {
                if (num > num2 && num < num3)
                {
                    Rect rect = new Rect(0f, mainRect.y + num, viewRect.width, 30f);
                    DrawCustomRow(rect, pair, num4);
                }

                num += 30f;
                num4++;
            }

            Widgets.EndScrollView();
        }

        private void DrawCustomRow(Rect rect, KeyValuePair<string, List<string>> pair, int index)
        {
            Text.Font = GameFont.Small;

            if (index % 2 == 0) Widgets.DrawLightHighlight(rect);
            Rect fixedRect = new Rect(new Vector2(rect.x + 10f, rect.y + 5f), new Vector2(rect.width - 52f, rect.height));

            if (pair.Value[0].Length > 1) pair.Value[0] = char.ToUpper(pair.Value[0][0]) + pair.Value[0].Substring(1);
            else pair.Value[0] = pair.Value[0].ToUpper();

            float buttonX = 47f;
            float buttonY = 30f;
            Widgets.Label(fixedRect, pair.Value[0] + "'s Settlement");
            if (Widgets.ButtonText(new Rect(new Vector2(rect.xMax - buttonX, rect.y), new Vector2(buttonX, buttonY)), "Jump"))
            {
                foreach(Settlement settlement in Find.World.worldObjects.Settlements)
                {
                    if (settlement.Faction == Main._ParametersCache.faction && settlement.Tile == int.Parse(pair.Key))
                    {
                        CameraJumper.TryJumpAndSelect(new GlobalTargetInfo(settlement));
                        break;
                    }
                }
            }
        }
    }
}
