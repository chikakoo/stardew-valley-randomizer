# Stardew Valley Randomizer (More Random Edition)

An update for cTooshi's Stardew Valley Randomizer to fix errors, add new features and make the existing features more random.
Nexus Mods page available here: https://www.nexusmods.com/stardewvalley/mods/5311

## Installation

Make sure you have SMAPI installed (https://stardewvalleywiki.com/Modding:Installing_SMAPI_on_Windows), then download the latest release of the Randomizer and unzip into your Stardew Valley Mods folder.

## Randomization and Custom Image Info

Note the following:
* This mod's randomization is seeded by the farm name. To generate a new set of random things, simply start a new farm with a different name.
* For details on how to create your own custom images for the mod, see the readme file in <ModFolder>/Assets/CustomImages
  * Several images will receive random hue-shifts - the readme has more details on how this works

## Settings

This mod supports the GenericModConfigMenuApi mod to more easily manage settings (https://www.nexusmods.com/stardewvalley/mods/5098).

If you do not wish to use this, you can modify the config.json file in the mod folder. This file is created after the first time you launch the game with the mod installed.

## Notes About Certain Events

The Biome Balance special order, for the sake of simplicity, is mapped to the following in the randomizer:
* River = Town
* Ocean = Beach
* Lake = Mountain

The [Trout Derby]([url](https://stardewvalleywiki.com/Trout_Derby)) works differently in the randomizer:
* _All_ fish caught in the forest, town, and mountains have a base 25% chance to award a Golden Tag
  * This 25% chance is configurable in the **Fish -> GoldenTagChance** setting
* The same multiplier for catching multiple fish still applies (e.g. catching 2 at once will have a 50% chance of receiving the Golden Tag)
* If the fish happens to have the same ID that the game normally checks for, that chance is higher

## Features

* Bundle randomization
  * New bundles for each room with random items selected from themed pools and random number of those items required
  * IMPORTANT: DO NOT use remixed bundles if using this feature! This will overwrite this mod's randomization and result in the incorrect images being used for the bundle pictures.
  * Some bundles are completely random and select from most items in the game.
  * Optionally (on by default), the community center now has tooltips over the possible things to put in a bundle to make it easier to identify where to get fish and other items.
* Crafting recipe randomization
  * Recipes are now created based on randomly selected items from a pool (not randomly selected premade recipes)
  * Crafting difficulty is balanced based on necessity of the item and difficulty of crafting the item in vanilla
  * Setting to choose to randomize levels you unlock crafting recipes at - must also randomize the crafting recipes themselves to have it do anything
* Crop randomization
  * Crops, including fruits, vegetables, and flowers, have randomized (made-up) names, descriptions, prices (for both seeds and crops), and attributes (trellises, scythe needed, etc.)
  * Crops will have their planting seasons listed on them, to make it easier to tell when they can be planted
  * Seeds, crops and growth stages have randomized images
  * Credits for the crop sprites not made by us (some images were modified from the originals):
    * Mizu - permission is assumed from this page: https://community.playstarbound.com/threads/mizus-sprites.136549/
    * Bonster - permission is assumed based on the bottom of the mod page for Bonster's Crops: https://www.nexusmods.com/stardewvalley/mods/3438
    * Marrorow
* Fish randomization
  * Fish have randomized (made-up) names, difficulty, and behavior
  * Locations, time-of-day, weather, and seasons are swapped as well
  * Fish have randomized images - most of them are from the More New fish mod: https://www.nexusmods.com/stardewvalley/mods/3578
  * Specific credits go to Hisame for the sprites: https://www.nexusmods.com/Users/51209496
* Forageable randomization
  * Forageables for every season and location are now randomly selected from all forageables + fruit (normally from trees)
  * Foragables will have the seasons they can be foraged in listed on them
  * Every forageable appears at least once per year, and some may appear more than once
* Fruit tree randomization
  * Fruit tree saplings are now item saplings that grow different items
  * Prices will be randomized and are loosely balanced based on the item they give
  * 2 trees give items from a random category
  * 1 tree gives a random item from the "Objects" list ("O" Sapling)
  * 1 tree gives a random item from the "Big Craftables" list ("BC" Sapling)
  * 2 trees will give only a single random item
* Weapon randomization
  * Weapon stats, types, etc. are randomized
  * Many weapons can now appear in mines containers
  * Weapon images can be randomized
* Boot randomization
  * Stats are randomized
  * Names are randomized
  * Descriptions are randomized
  * Images are randomized
* Monster randomization
  * Stats are randomized: HP / Resilience / Speed / Experience
  * The threshold before a monster moves toward you is randomized
  * The time a monster moves randomly is randomized
  * Up to a 5% chance to be able to miss an attack on a monster
  * Setting to shuffle unique monster drops among all monsters (Slime, Bat Wing, Solar Essence, Bug Meat, Void Essence, and Squid Ink)
  * Each monster can now drop a new random item
* Building randomization
  * Farm buildings that you get from Robin now choose from a more random pool of resources/items instead of a set list
  * This does not yet include anything you don't get from Robin (Obelisks, the Gold Clock, etc.)
* Shop randomization (credit to Vertigon for doing the initial work for this) - note that each of these have their own associated setting
  * Pierre's Seed Shop
    * Random limited-stock of the week that's more expensive than the base price
  * Joja Mart
    * Random limited-stock of the week that's more expensive than the base price (but cheaper than Pierre's would be) 
  * Blacksmith Shop - every day, a chance at one of the following
    * (50%) Discount an ore by 10-25%
    * (35%) A chance of a random bar being added to the shop, with limited stock depending on the bar
    * (12%) Add an artifact to the shop (stock of 1)
    * (3%) Add 5-15 iridium ore to the shop at 5x the base cost
  * Robin's Carpenter Shop (changes daily)
    * Add 20-40 clay to the stock at a random price between 25-75
    * Add a random item that's use to craft tappers, since they can be hard to get (stock limited by the amount to craft 1 tapper)
  * Saloon - changes weekly
    * Beer and Coffee always available
    * 3-5 random cooked foods
    * 3-5 random recipes (not added if the player has learned them)
  * Willy's Fishing Shop
    * Catch of the day - 1-3 of a random fish that's catchable this season
  * Desert Oasis Shop - random based on the day
    * Mondays - 1-5 of a random desert foragables
    * Tuesdays - 3-8 of a random crop that corresponds to the seeds sold here
    * Weekends - A random cooked item
    * Every week...
      * Adds a couple random craftable/resource items
      * Replaces all clothing and furniture items with random ones
  * Krobus' Sewer Shop - changes daily
    * Replace the Monster Fireplace with a random Furniture item
    * Replace the Sign of the Vessel with a random BigCraftable item
  * Hat Shop
    * Hat of the week
  * Club/Casino Shop - each week, entire shop is replaced with...
    * 3-5 random furniture
    * A random hat or clothing item
    * A random BigCraftable item
    * 2-3 misc items, such as resources
    * A random totem, always costing 500
* Randomize museum rewards
  * Each reward will be randomized to another one in the same category
  * Certain important rewards will remain the same, including the Dwarven Translation Guide, the Ancient Seeds, and the Stardrop
* Graphics randomization - some of these are modified from the Time Fantasy assets (https://www.timefantasy.net/)
  * Horse: chooses a random horse image from the files in CustomImages/Animals/Horses
  * Pets: chooses a random pet image from the files in CustomImages/Animals/Pets
  * Rain: every day chooses a random rain image from the files in CustomImages/TileSheets
  * Critters: every day, all critters will get a random hue shift
* Music randomization
  * Most in-game songs and ambience are now randomly swapped 1 to 1 with another in-game song or ambience
  * Option to play a completely random song each time an area is moved to
* Quest randomization
  * Quest givers, required items, and rewards are randomly selected.
  * Help Wanted quests are unaffected, but the randomized item names should appear as expected.
* NPC preference randomizer (credit to Vertigon)
  * Includes randomizing secret notes to give random loved item info
* NPC birthday randomization
  * Randomizes the season and day of each NPC's birthday
  * Does not assign birthdays to the same day
  * Does not assign birthdays on the same day of most festivals (excludes night market and the moonlight jellies)
* Garbage Can Randomizer
  * Places disliked/hated items in garbage cans from characters that are likely to use them
* Spoiler log
  * A spoiler log can be generated to see info about what was randomized
  * You must turn on this option in the settings to generate the log
* Bundle log (off by default)
  * Creates a file containing all the items required for bundles, but with shorthand hints at when they are available
  * Fish/Crops/Seeds/Foragables have a list of seasons
  * Fish have their locations and whether they are sun or rain only
  * Intended to copy/paste into a spreadsheet of some kind for easy reference
  
## Possible Future Features
* Enemy changes
  * Change where enemies can appear
  * More varied drop randomization (they currently have all the same base drops)
* NPC schedule shuffle and/or randomization
* Cooking recipe randomization
* Tea trees and the new items associated with it need to be randomized
* Randomize tool upgrade cost
* Randomize house upgrade cost
* Randomize tailoring recipes
* Add settings for more things, such as stats that can be modified on weapons, possible random names, etc.
* More things randomized on Ginger Island

## Known Issues
* This mod does not fully support other languages (but it does for the most part)
  * Randomly generated weapon/crop names are English
  * The mad-lib style crop descriptions are English
  * Everything else has been internationalized
