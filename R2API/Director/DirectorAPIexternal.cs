using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Changing namespace to R2API.Director would be breaking
namespace R2API {

    /// <summary>
    /// API for modifying the monster and scene directors.
    /// </summary>
    public static partial class DirectorAPI {

        /// <summary>
        /// Return true if the submodule is loaded.
        /// </summary>
        public static bool Loaded {
            get => _loaded;
            internal set => _loaded = value;
        }

        private static bool _loaded;

        /// <summary>
        /// Event used to edit <see cref="StageSettings"/>.
        /// </summary>
        public static event Action<StageSettings, StageInfo>? StageSettingsActions;

        /// <summary>
        /// <para>
        /// Event used to edit the pool of monsters that can spawn in a given stage.
        /// </para>
        ///
        /// <para>
        /// First parameter is the <see cref="ClassicStageInfo.monsterDccsPool"/>,
        /// depending if the stage was updated or not to use the new <see cref="DccsPool"/> that field CAN BE NULL,
        /// if null, its the <see cref="ClassicStageInfo.monsterCategories"/> field that is used instead, which is always passed as
        /// the second parameter of this event : CAN BE NULL.
        /// One of those field is what is used to ultimately select the final <see cref="DirectorCardCategorySelection"/>.
        /// </para>
        ///
        /// <para>
        /// Make sure to guard with a if else null checks, either applying your changes to the first OR second parameter.
        /// </para>
        ///
        /// <para>
        /// The third parameter is the list of monster families and their cards. OLD SYSTEM AND CAN BE NULL. Its still here because some vanilla stages
        /// are still using it, very much like the second parameter above.
        /// Please refer to the paragraphs below for the new system, the families are now under a specific <see cref="DccsPool.Category"/>.
        /// </para>
        /// 
        /// <para>
        /// For the Artifact of Dissonance <see cref="RoR2Content.Artifacts.mixEnemyArtifactDef"/>,
        /// the original DCCS is located in <see cref="RoR2Content.mixEnemyMonsterCards"/>
        /// which is represented by the fourth parameter of this event.
        /// </para>
        ///
        /// <para>
        /// A <see cref="ClassicStageInfo.monsterDccsPool"/> usually contains, in a vanilla stage, the following :
        /// 3 <see cref="DccsPool.poolCategories"/>, written out in <see cref="Helpers.MonsterPoolCategories"/>.
        /// </para>
        ///
        /// <para>
        /// Below is an explanation of the <see cref="ClassicStageInfo.monsterDccsPool"/> categories.
        /// </para>
        /// 
        /// <para>
        /// The first category, <see cref="Helpers.MonsterPoolCategories.Standard"/> has, right now,
        /// a single <see cref="DccsPool.ConditionalPoolEntry"/> (contained in <see cref="DccsPool.Category.includedIfConditionsMet"/>)
        /// which contains the <see cref="DirectorCardCategorySelection"/> used for DLC1 SOTV.
        /// If the <see cref="DccsPool.ConditionalPoolEntry.requiredExpansions"/> is not enabled in the current lobby,
        /// they are not added to the pool of choice.
        /// Right now, there is a single PoolEntry in <see cref="DccsPool.Category.includedIfNoConditionsMet"/>,
        /// which is the vanilla (no expansion) dccs.
        /// </para>
        ///
        /// <para>
        /// The second category, <see cref="Helpers.MonsterPoolCategories.Family"/> has, right now,
        /// multiple <see cref="DccsPool.ConditionalPoolEntry"/> (contained in <see cref="DccsPool.Category.includedIfConditionsMet"/>)
        /// which contains the <see cref="DirectorCardCategorySelection"/>s used for family events.
        /// They don't have a corresponding <see cref="DccsPool.ConditionalPoolEntry.requiredExpansions"/>,
        /// so they are effectively always added to the pool of choice.
        /// </para>
        ///
        /// <para>
        /// The third category, <see cref="Helpers.MonsterPoolCategories.VoidInvasion"/> has, right now,
        /// a single <see cref="DccsPool.ConditionalPoolEntry"/> (contained in <see cref="DccsPool.Category.includedIfConditionsMet"/>)
        /// which contains the <see cref="DirectorCardCategorySelection"/> used for void invasion events.
        /// It has a corresponding <see cref="DccsPool.ConditionalPoolEntry.requiredExpansions"/>,
        /// so they are only added to the pool of choice if the DLC1 expansion is enabled.
        /// </para>
        /// </summary>
        public static event Action<DccsPool, List<DirectorCardHolder>, List<MonsterFamilyHolder>, List<DirectorCardHolder>, StageInfo>? MonsterActions;

        /// <summary>
        /// <para>
        /// Event used to edit the pool of interactables that can spawn in a given stage.
        /// </para>
        ///
        /// <para>
        /// First parameter is the <see cref="ClassicStageInfo.interactableDccsPool"/>,
        /// which is used to select a <see cref="DirectorCardCategorySelection"/>. IT CAN BE NULL
        /// </para>
        ///
        /// <para>
        /// Second parameter is the <see cref="ClassicStageInfo.interactableCategories"/>,
        /// which is used to select a <see cref="DirectorCardCategorySelection"/> in cases
        /// the new system with <see cref="ClassicStageInfo.interactableDccsPool"/> is not used. IT CAN BE NULL
        /// </para>
        ///
        /// <para>
        /// Make sure to guard with a if else null checks, either applying your changes to the first OR second parameter.
        /// </para>
        /// 
        /// <para>
        /// A <see cref="ClassicStageInfo.interactableDccsPool"/> usually contains, in a vanilla stage, the following :
        /// 1 <see cref="DccsPool.poolCategories"/>, written out in <see cref="Helpers.InteractablePoolCategories"/>.
        /// </para>
        ///
        /// <para>
        /// The first category, <see cref="Helpers.InteractablePoolCategories.Standard"/> has, right now,
        /// a single <see cref="DccsPool.ConditionalPoolEntry"/> (contained in <see cref="DccsPool.Category.includedIfConditionsMet"/>)
        /// which contains the <see cref="DirectorCardCategorySelection"/> used for DLC1 SOTV.
        /// If the <see cref="DccsPool.ConditionalPoolEntry.requiredExpansions"/> is not enabled in the current lobby,
        /// they are not added to the pool of choice.
        /// Right now, there is a single PoolEntry in <see cref="DccsPool.Category.includedIfNoConditionsMet"/>,
        /// which is the vanilla (no expansion) dccs.
        /// </para>
        /// </summary>
        public static event Action<DccsPool, List<DirectorCardHolder>, StageInfo>? InteractableActions;

        /// <summary>
        /// The categories for monsters.
        /// </summary>
        public enum MonsterCategory {

            /// <summary>
            /// An invalid default value. Anything with this value is ignored when dealing with monsters.
            /// </summary>
            Invalid = 0,

            /// <summary>
            /// Anything with this value will instead be checked through <see cref="DirectorCardHolder.CustomMonsterCategory"/>
            /// </summary>
            Custom = 1,

            /// <summary>
            /// Small enemies like Lemurians and Beetles.
            /// </summary>
            BasicMonsters = 2,

            /// <summary>
            /// Medium enemies like Golems and Beetle Guards.
            /// </summary>
            Minibosses = 3,

            /// <summary>
            /// Bosses like Vagrants and Titans.
            /// </summary>
            Champions = 4,

            Special = 5,
        }

        /// <summary>
        /// The categories for interactables.
        /// </summary>
        public enum InteractableCategory {

            /// <summary>
            /// An invalid default value. Anything with this value is ignored when dealing with interactables.
            /// </summary>
            Invalid = 0,

            /// <summary>
            /// Anything with this value will instead be checked through <see cref="DirectorCardHolder.CustomInteractableCategory"/>
            /// </summary>
            Custom = 1,

            /// <summary>
            /// Chests, such as basic chests, large chests, shops, equipment barrels, lunar pods, and category chests. NOT legendary chests or cloaked chests.
            /// </summary>
            Chests = 2,

            /// <summary>
            /// Barrels.
            /// </summary>
            Barrels = 3,

            /// <summary>
            /// Chance shrines, blood shrines, combat shrines, order shrines, mountain shrines, shrine of the woods. NOT shrine of gold.
            /// </summary>
            Shrines = 4,

            /// <summary>
            /// All types of drones such as TC-280, equipment drones, gunner drones, healing drones, and incinerator drones. NOT gunner turrets.
            /// </summary>
            Drones = 5,

            /// <summary>
            /// Gunner turrets only.
            /// </summary>
            Misc = 6,

            /// <summary>
            /// Legendary chests, cloaked chests, shrine of gold, and radio scanners.
            /// </summary>
            Rare = 7,

            /// <summary>
            /// All three tiers of printers.
            /// </summary>
            Duplicator = 8,

            VoidStuff = 9
        }

        /// <summary>
        /// A flags enum for the vanilla stages. Custom stages are handled with a string in StageInfo.
        /// </summary>
        [Flags]
        public enum Stage {

            /// <summary>
            /// When this is set to custom, check the string in StageInfo
            /// </summary>
            Custom = 1 << 0,

            /// <summary>
            /// External / Internal Name : Titanic Plains / golemplains
            /// </summary>
            TitanicPlains = 1 << 1,

            /// <summary>
            /// External / Internal Name : Distant Roost / blackbeach
            /// </summary>
            DistantRoost = 1 << 2,

            /// <summary>
            /// External / Internal Name : Wetland Aspect / foggyswamp
            /// </summary>
            WetlandAspect = 1 << 3,

            /// <summary>
            /// External / Internal Name : Abandoned Aqueduct / goolake
            /// </summary>
            AbandonedAqueduct = 1 << 4,

            /// <summary>
            /// External / Internal Name : Rallypoint Delta / frozenwall
            /// </summary>
            RallypointDelta = 1 << 5,

            /// <summary>
            /// External / Internal Name : Scorched Acres / wispgraveyard
            /// </summary>
            ScorchedAcres = 1 << 6,

            /// <summary>
            /// External / Internal Name : Abyssal Depths / dampcavesimple
            /// </summary>
            AbyssalDepths = 1 << 7,

            /// <summary>
            /// External / Internal Name : Siren's Call / shipgraveyard
            /// </summary>
            SirensCall = 1 << 8,

            /// <summary>
            /// External / Internal Name : Hidden Realm: Gilded Coast / goldshores
            /// </summary>
            GildedCoast = 1 << 9,

            /// <summary>
            /// External / Internal Name : Hidden Realm: A Moment, Fractured / mysteryspace
            /// </summary>
            MomentFractured = 1 << 10,

            /// <summary>
            /// External / Internal Name : Hidden Realm: Bazaar Between Time / bazaar
            /// </summary>
            Bazaar = 1 << 11,

            /// <summary>
            /// External / Internal Name : Void Fields / arena
            /// </summary>
            VoidCell = 1 << 12,

            /// <summary>
            /// External / Internal Name : Hidden Realm: A Moment, Whole / limbo
            /// </summary>
            MomentWhole = 1 << 13,

            /// <summary>
            /// External / Internal Name : Sky Meadow / skymeadow
            /// </summary>
            SkyMeadow = 1 << 14,

            /// <summary>
            /// External / Internal Name : Hidden Realm: Bulwark's Ambry / artifactworld
            /// </summary>
            ArtifactReliquary = 1 << 15,

            /// <summary>
            /// External / Internal Name : Commencement / moon2
            /// </summary>
            Commencement = 1 << 16,

            /// <summary>
            /// External / Internal Name : Sundered Grove / rootjungle
            /// </summary>
            SunderedGrove = 1 << 17,

            // New entries for DLC1 below

            /// <summary>
            /// External / Internal Name : Aphelian Sanctuary / ancientloft
            /// </summary>
            AphelianSanctuary = 1 << 18,

            /// <summary>
            /// External / Internal Name : Aphelian Sanctuary - The Simulacrum / itancientloft
            /// </summary>
            AphelianSanctuarySimulacrum = 1 << 19,

            /// <summary>
            /// External / Internal Name : Abyssal Depths - The Simulacrum / itdampcave
            /// </summary>
            AbyssalDepthsSimulacrum = 1 << 20,

            /// <summary>
            /// External / Internal Name : Rallypoint Delta - The Simulacrum / itfrozenwall
            /// </summary>
            RallypointDeltaSimulacrum = 1 << 21,

            /// <summary>
            /// External / Internal Name : Titanic Plains - The Simulacrum / itgolemplains
            /// </summary>
            TitanicPlainsSimulacrum = 1 << 22,

            /// <summary>
            /// External / Internal Name : Abandoned Aqueduct - The Simulacrum / itgoolake
            /// </summary>
            AbandonedAqueductSimulacrum = 1 << 23,

            /// <summary>
            /// External / Internal Name : Commencement - The Simulacrum / itmoon
            /// </summary>
            CommencementSimulacrum = 1 << 24,

            /// <summary>
            /// External / Internal Name : Sky Meadow - The Simulacrum / itskymeadow
            /// </summary>
            SkyMeadowSimulacrum = 1 << 25,

            /// <summary>
            /// External / Internal Name : Siphoned Forest / snowyforest
            /// </summary>
            SiphonedForest = 1 << 26,

            /// <summary>
            /// External / Internal Name : Sulfur Pools / sulfurpools
            /// </summary>
            SulfurPools = 1 << 27,

            /// <summary>
            /// External / Internal Name : Void Locus / voidraid
            /// </summary>
            VoidLocus = 1 << 28,

            /// <summary>
            /// External / Internal Name : The Planetarium / voidstage
            /// </summary>
            ThePlanetarium = 1 << 29,
        }

        /// <summary>
        /// Struct for holding information about the stage.
        /// </summary>
        public struct StageInfo {

            /// <summary>
            /// The current stage. If set to custom, check <see cref="CustomStageName"/>.
            /// </summary>
            public Stage stage;

            /// <summary>
            /// This is set to the name of the custom stage. Is left blank for vanilla stages.
            /// </summary>
            public string CustomStageName;

            /// <summary>
            /// Returns true if the current stage matches any of the stages you specify.
            /// To match a custom stage, include <see cref="Stage.Custom"/> in your stage input and specify names in <see cref="CustomStageName"/>.
            /// </summary>
            /// <param name="stage">The stages to match with</param>
            /// <param name="customStageNames">Names of the custom stages to match. Leave blank to match all custom stages</param>
            /// <returns></returns>
            public bool CheckStage(Stage stage, params string[] customStageNames) {
                if (!stage.HasFlag(this.stage)) return false;
                return this.stage != Stage.Custom || customStageNames.Length == 0 || customStageNames.Contains(CustomStageName);
            }
        }

        /// <summary>
        /// A class passed to everything subscribed to <see cref="StageSettingsActions"/> that contains various settings for a stage.
        /// All mods will be working off the same settings, so operators like *=, +=, -=, and /= are preferred over directly setting values.
        /// </summary>
        public class StageSettings {

            /// <summary>
            /// How many credits the scene director has for monsters at the start of a stage.
            /// This scales with difficulty, and thus will always be zero on the first stage.
            /// </summary>
            public int SceneDirectorMonsterCredits;

            /// <summary>
            /// How many credits the scene director has for interactables at the start of a stage.
            /// </summary>
            public int SceneDirectorInteractableCredits;

            /// <summary>
            /// If the GameObject key of the dictionary is enabled, then the scene director gains the value in extra interactable credits.
            /// Used for things like the door in Abyssal Depths.
            /// </summary>
            public Dictionary<GameObject, int>? BonusCreditObjects;

            /// <summary>
            /// The weights for each monster category per possible DCCS on this stage.
            /// </summary>
            public Dictionary<DirectorCardCategorySelection, Dictionary<string, float>> MonsterCategoryWeightsPerDccs;

            /// <summary>
            /// The weights for each interactable category per possible DCCS on this stage.
            /// </summary>
            public Dictionary<DirectorCardCategorySelection, Dictionary<string, float>> InteractableCategoryWeightsPerDccs;
        }

        /// <summary>
        /// A wrapper class for DirectorCards.
        /// </summary>
        public class DirectorCardHolder {

            /// <summary>
            /// The director card. This contains the majority of the information for an interactable or monster, including the prefab.
            /// </summary>
            public DirectorCard? Card;

            /// <summary>
            /// The monster category the card belongs to. Will be set to <see cref="MonsterCategory.Invalid"/> for interactables,
            /// <see cref="MonsterCategory.Custom"/> for custom monster non-vanilla categories.
            /// </summary>
            public MonsterCategory MonsterCategory;

            /// <summary>
            /// Should be null for vanilla categories
            /// </summary>
            public string CustomMonsterCategory;

            /// <summary>
            /// This is only used in case the category didnt exist in the first place in the targeted DCCS.
            /// </summary>
            public int MonsterCategorySelectionWeight = 1;

            /// <summary>
            /// The interactable category the card belongs to. Will be set to <see cref="InteractableCategory.Invalid"/> for monsters,
            /// <see cref="InteractableCategory.Custom"/> for custom non-vanilla categories.
            /// </summary>
            public InteractableCategory InteractableCategory;

            /// <summary>
            /// Should be null for vanilla categories
            /// </summary>
            public string CustomInteractableCategory;

            /// <summary>
            /// This is only used in case the category didnt exist in the first place in the targeted DCCS.
            /// </summary>
            public int InteractableCategorySelectionWeight = 1;

            public bool IsMonster => MonsterCategory != MonsterCategory.Invalid;

            public bool IsInteractable => InteractableCategory != InteractableCategory.Invalid;

            public bool IsValid() {
                if (InteractableCategory == InteractableCategory.Invalid &&
                    MonsterCategory == MonsterCategory.Invalid) {
                    return false;
                }

                return true;
            }

            public void ThrowIfInvalid() {
                if (!IsValid()) {
                    throw new Exception("Both DirectorCardHolder.InteractableCategory and DirectorCardHolder.MonsterCategory are invalid");
                }
            }

            public string GetCategoryName() {
                ThrowIfInvalid();

                string categoryName;
                if (InteractableCategory == InteractableCategory.Invalid) {
                    if (MonsterCategory == MonsterCategory.Custom) {
                        categoryName = CustomMonsterCategory;
                    }
                    else {
                        categoryName = Helpers.GetVanillaMonsterCategoryName(MonsterCategory);
                    }
                }
                else {
                    if (InteractableCategory == InteractableCategory.Custom) {
                        categoryName = CustomInteractableCategory;
                    }
                    else {
                        categoryName = Helpers.GetVanillaInteractableCategoryName(InteractableCategory);
                    }
                }

                return categoryName;
            }
        }

        /// <summary>
        /// Add a <see cref="DirectorCardHolder"/> to a <see cref="DirectorCardCategorySelection"/>.
        /// If the category from the given card parameter is not in the given dccs parameter,
        /// the category is created and added to the dccs.
        /// Returns the card index in the category if successful.
        /// </summary>
        /// <param name="dccs"></param>
        /// <param name="cardHolder"></param>
        /// <returns></returns>
        public static int AddCard(this DirectorCardCategorySelection dccs, DirectorCardHolder cardHolder) {
            string categoryName;
            categoryName = cardHolder.GetCategoryName();

            for (int i = 0; i < dccs.categories.Length; i++) {
                if (dccs.categories[i].name == categoryName) {
                    HG.ArrayUtils.ArrayAppend<DirectorCard>(ref dccs.categories[i].cards, cardHolder.Card);
                    return dccs.categories[i].cards.Length - 1;
                }
            }

            var categoryWeight = cardHolder.IsMonster ? cardHolder.MonsterCategorySelectionWeight : cardHolder.InteractableCategorySelectionWeight;
            var categoryIndex = dccs.AddCategory(categoryName, categoryWeight);
            return dccs.AddCard(categoryIndex, cardHolder.Card);
        }

        /// <summary>
        /// A wrapper class for Monster Families.
        /// </summary>
        public class MonsterFamilyHolder {
            /// <summary>
            /// List of all monster per monster category name that can spawn during this family event.
            /// </summary>
            public Dictionary<string, List<DirectorCard>> MonsterCategoryToMonsterCards;

            /// <summary>
            /// The selection weight per monster category name during the family event.
            /// </summary>
            public Dictionary<string, float> MonsterCategoryToSelectionWeights;

            /// <summary>
            /// The minimum number of stages completed for this family event to occur.
            /// </summary>
            public int MinStageCompletion;

            /// <summary>
            /// The maximum number of stages for this family event to occur.
            /// </summary>
            public int MaxStageCompletion;

            /// <summary>
            /// The weight of this monster family relative to other monster families.
            /// Does NOT increase the chances of a family event occuring, just the chance that this will be chosen when one does occur.
            /// </summary>
            public float FamilySelectionWeight;

            /// <summary>
            /// The message sent to chat when this family is selected.
            /// </summary>
            public string? SelectionChatString;
        }
    }
}
