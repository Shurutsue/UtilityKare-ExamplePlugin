/*
 *      This is an example Mod for KoboldKare using UtilityKare 
 * 
 *   For this project, we require certain Assembly-references.
 *   While UnityEngine stuff is mostly provided by the NuGet, the UnityEngine.Localization is not.
 *   So Add Unity.Localization.dll to the Assembly references!
 *   Additionally, we will need UtilityKare as well, since we're working with it.
 *   And since we're working with KoboldKare things, we would also need Assembly-CSharp.dll
 *   Which can be found in the KoboldKare_Data/managed/ folder.
 *   
 */

using BepInEx;                  // We're using BepInEx to load our mods after all
using UtilityKare;              // In this situation, we use UtilityKare methods
using UnityEngine.Localization; // We require this to work with localizedStrings
using UnityEngine;              // In the current example we'll need to use UnityEngine for Color objects, as well as GameObjects.

namespace UtilityKare_ExamplePlugin
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]

    // With this, we ensure that UtilityKare plugin exists, so we don't run our stuff without it even present.
    [BepInDependency(UtilityKare.PluginInfo.PLUGIN_GUID)]
    // With this we ensure that the mod is only used in KoboldKare!
    [BepInProcess("KoboldKare.exe")]
    public class ExamplePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Localization Examples
            // Create a new Instance of Localization
            Localization MyLocalizations = new Localization();
            // We Add a new entry for the key REAGENT_SWEAT with a default value "Sweat", which returns a LocalizationKey
            LocalizationKey LOCALIZATION_REAGENT_SWEAT =  MyLocalizations.AddKeyEntry("REAGENT_SWEAT", "Sweat");
            // Now we can call this Key's AddTranslation Method to add a translation for a specific language
            LOCALIZATION_REAGENT_SWEAT.AddTranslation("de", "Schweiß"); // Language Code "de" refers to the german
            LOCALIZATION_REAGENT_SWEAT.AddTranslation("fr", "Sueur"); // Language Code "fr" refers to the french
            // Any other language not defined here would take the default value we defined when adding our Key.

            //Now we can request a LocalizedString (defined in UnityEngine.Localization, in the Unity.Localization.dll)
            LocalizedString localizedString = MyLocalizations.GetLocalizedString("REAGENT_SWEAT");

            // With that, we could now create a custom reagent!
            // We pass in "Sweat" as a unique name. Existing reagents would be modified with the values we define, otherwise a new reagent will be added!
            // The unique name is used to spawn in the reagents, so it's required!
            CustomReagent REAGENT_SWEAT = new CustomReagent("Sweat")
            {
                // The LocalizedString we got earlier can be used here!
                // This will display our default text or translations on the Scanner.
                LocalizedName = localizedString,

                // Here we set the fluids color (defined in UnityEngine)
                // Colors take (Red, Green, Blue) or (Red, Green, Blue, Alpha) values between 0f and 1f.
                FluidColor = new Color(1f, 1f, 1f, 0.2f),

                // Here we set the emission color (still not sure how/where it is used.
                Emission = new Color(1f, 1f, 1f, 1f),

                // Here we set the worth of the fluid (when sold, depending on how much of the fluid you sell times this value)
                // Here we put 0.25f, so 100 units of it would be worth 25!
                Value = 0.25f,

                // The metabolization half life refers to the time it takes to metabolize. The lower it is, the faster.
                MetabolizationHalfLife = 60f,

                // Whether or not it acts as a cleaning agent, like water. If true, it will not leave decals when spilled and instead cleans those up.
                CleaningAgent = false,

                // The Display refers to the model and materials to use. I did not test those yet, so I can't provide an example
                // on how this can be used with mods yet.
                Display = new GameObject(),

                // The ConsumptionEvent refers to how the fluid behaves when metabolized.
                // DefaultConsumption means it just fills energy (or fat if energy is full) with the used amount * calories.
                // You can create custom ReagentConsumptionEvents, though for this example, we use a default one.
                ConsumptionEvent = new DefaultConsumption(),
            };


            // Now that our own reagent is complete, we add it to the ReagentManager, with assists us with adding it to the game.
            ReagentManager.AddReagent(REAGENT_SWEAT);

            // Now however, we would like to be able to get the reagent in game somehow.
            // So for this example, we'll create a new ReagentReaction.
            CustomReagentReaction REACTION_SWEAT = new CustomReagentReaction();

            // Now we will define the two reagents required to receive our now reagent!
            // Here we use water and the required amount for the reaction would be 1.0f
            REACTION_SWEAT.AddReactant("Water", 1.0f);

            // Now we add "Love" to somehow justify it resulting in sweat.
            // In this case, only 0.1f would be required!
            REACTION_SWEAT.AddReactant("Love", 0.1f);

            // Now we set the Product, meaning the result of the combination!
            // in this example, it's our previously created Reagent with the unique name "Sweat"
            // so 1.0 of Water + 0.1 of Love will produce 1.0 of Sweat (1.1 to 1.0)
            // so there's a slight loss in total volume.
            REACTION_SWEAT.AddProduct("Sweat", 1.0f);

            // All that's left to do is to add it the ReagentReaction to the ReagentManager, so it manages adding it to the game.
            // Additionally it'll throw an error in case a reagent was not found (and skips the stuff)
            ReagentManager.AddReagentReaction(REACTION_SWEAT);

            // However, now that we added custom reagents, we should ensure that only people with our custom reagents can actually join our room
            // and that we only see multiplayer rooms that have this mod active.
            // For that, while a dirty workaround, we modify the version by adding our GUID to it!
            // The VersionController will take care of sorting it and adding it to the version, since not all mods are necessarily loaded in correct order.
            VersionController.AddModGUID(PluginInfo.PLUGIN_GUID);


            // Now that we finished what our mod is supposed to do (in this case a custom localization, reagent and reagentreaction) we're done!
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
