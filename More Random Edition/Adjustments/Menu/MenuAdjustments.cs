using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace Randomizer;

/// <summary>
/// Makes menu adjustments
/// </summary>
public class MenuAdjustments
{
    /// <summary>
    /// Makes the actual menu adjustments
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">Contains the menu info - NewMenu means a menu was opened, OldMenu means one was closed</param>
    public static void AdjustMenus(object sender, MenuChangedEventArgs e)
    {
        // Quests - fix the name in multiplayer
        if (e.NewMenu is QuestLog questLog)
        {
            QuestLogAdjustments.FixQuestName(questLog);
        }

        // Pause menu - adjust the crab pot cost if the player has Trapper
        else if (e.NewMenu is GameMenu gameMenu)
        {
            CraftingMenuAdjustments.ReduceCrabPotCost(gameMenu);

#if DEBUG
            // This is to test shop menus quickly
            // Set a breakpoint here and move the code so CalculateAndInvalidateShopEdits is executed
            var test = false;
            if (test)
            {
                Globals.ModRef.CalculateAndInvalidateShopEdits();
            }
#endif
        }
    }
}
