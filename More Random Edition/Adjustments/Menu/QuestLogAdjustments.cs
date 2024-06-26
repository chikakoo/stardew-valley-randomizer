﻿using StardewValley.Menus;
using StardewValley.Quests;
using System.Collections.Generic;

namespace Randomizer
{
    /// <summary>
    /// In multiplayer, the non-host will see the old quest names by default
    /// This will change the quest names to the intended ones in the log
    /// </summary>
    public class QuestLogAdjustments
    {
        /// <summary>
        /// Fix the quest name so that it matches what we want it to be
        /// Based on QuestRandomizer.QuestReplacements
        /// </summary>
        /// <param name="menu"></param>
        public static void FixQuestName(QuestLog menu)
        {
            const int QuestNameIndex = 1;

            var questPages = Globals.ModRef.Helper.Reflection
                .GetField<List<List<IQuest>>>(menu, "pages", true)
                .GetValue();

            foreach (var questPage in questPages)
            {
                foreach (var quest in questPage)
                {
                    if (quest is ItemDeliveryQuest itemDeliveryQuest)
                    {
                        if (itemDeliveryQuest.id.Value == null)
                        {
                            continue; 
                        }

                        if (QuestRandomizer.QuestReplacements.TryGetValue(
                            itemDeliveryQuest.id.Value,
                            out string questString))
                        {
                            string modifedQuestName = questString.Split("/")[QuestNameIndex];
                            itemDeliveryQuest.questTitle = modifedQuestName;
                        }
                    }
                }
            }
        }
    }
}
