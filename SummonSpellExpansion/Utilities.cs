using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.Configurators;
using BlueprintCore.Blueprints.CustomConfigurators;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Cheats;
using Kingmaker.Localization;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using Newtonsoft.Json;
using SummonSpellExpansion.SerializableClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SummonSpellExpansion
{
	public static class Utilities
	{
		public static void CreateSummonSpell(SpellData spellData)
		{
			AbilityConfigurator.New(spellData.SpellName, spellData.SpellGuid)
					  .SetDisplayName(LocalizationTool.GetString(spellData.DisplayName))
					  .SetDescription(LocalizationTool.GetString(spellData.Description))
					  .SetIsFullRoundAction(true)
					  .SetRange(AbilityRange.Close)
					  .SetCanTargetPoint(true)
					  .SetCanTargetSelf(true)
					  .SetAnimation(Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Point)
					  .SetActionType(Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Standard)
					  .SetAvailableMetamagic(Metamagic.Empower, Metamagic.Maximize, Metamagic.Quicken, Metamagic.Extend, Metamagic.Heighten, Metamagic.Reach, Metamagic.CompletelyNormal)
					  .SetLocalizedDuration(LocalizationTool.GetString("DurationRate.Rounds"))
					  .SetLocalizedSavingThrow(LocalizationTool.GetString("SavingThrow.None"))
					  .AddSpellComponent(spellData.SpellSchool, null, ComponentMerge.Replace)
					  .AddSpellDescriptorComponent(SpellDescriptor.Summoning, null, ComponentMerge.Replace)
					  .SetIcon(DefaultIcon(spellData))
					  .AddContextRankConfig(DefaultContextRankConfig(false))
					  .AddAbilityEffectRunAction(DefaultAbilityEffectRunAction(spellData))
					  .Configure();
		}

		public static ActionsBuilder DefaultAbilityEffectRunAction(SpellData spellData)
		{
			ActionsBuilder abilityEffectRunAction = ActionsBuilder.New().SpawnMonster(DiceValueSetup(spellData.DiceValue), DurationValueSetup(spellData.DurationValue), ResourcesLibrary.TryGetBlueprint<BlueprintUnit>(spellData.SpawnUnit).ToReference<BlueprintUnitReference>(), DefaultAfterSpawn(spellData), false, false, ContextValues.Property(Kingmaker.UnitLogic.Mechanics.Properties.UnitProperty.Level, false));
			if (!spellData.SummonPool.IsNullOrEmpty())
			{
				abilityEffectRunAction = ActionsBuilder.New().SpawnMonsterUsingSummonPool(DiceValueSetup(spellData.DiceValue), DurationValueSetup(spellData.DurationValue), ResourcesLibrary.TryGetBlueprint<BlueprintUnit>(spellData.SpawnUnit).ToReference<BlueprintUnitReference>(), spellData.SummonPool, DefaultAfterSpawn(spellData), false, false, ContextValues.Property(Kingmaker.UnitLogic.Mechanics.Properties.UnitProperty.Level, false));
			}
			return abilityEffectRunAction;
		}
		public static ActionsBuilder DefaultAfterSpawn(SpellData spellData)
		{
			ConditionsBuilder augmentSummonCheck = ConditionsBuilder.New().CasterHasFact(ResourcesLibrary.TryGetBlueprint<BlueprintUnitFact>("38155ca9e4055bb48a89240a2055dcc3").ToReference<BlueprintUnitFactReference>(), false);
			ActionsBuilder augmentSummonBuff = ActionsBuilder.New().ApplyBuffPermanent(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("169d03bbccdbdc542ae1a57d83673d80").ToReference<BlueprintBuffReference>(), true, false, true, false, false);
			ActionsBuilder afterSpawn =
				ActionsBuilder.New()
				.ApplyBuffPermanent(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("0dff842f06edace43baf8a2f44207045").ToReference<BlueprintBuffReference>(), false, false, true, false, false)
				.Conditional(augmentSummonCheck, augmentSummonBuff, null);

			if (spellData.UnitFX != null)
			{
				foreach (string fxGuid in spellData.UnitFX)
				{
					afterSpawn.ApplyBuffPermanent(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>(fxGuid).ToReference<BlueprintBuffReference>(), false, false, true, false, false);
				}
			}

			return afterSpawn;
		}

		public static ContextRankConfig DefaultContextRankConfig(bool isParentSpell)
		{
			ContextRankConfig rankConfig;

			if (isParentSpell)
			{
				rankConfig = new ContextRankConfig()
				{
					m_BuffRankMultiplier = 1,
					m_Max = 20,
				};
			}
			else
			{
				rankConfig = new ContextRankConfig()
				{
					m_Type = Kingmaker.Enums.AbilityRankType.ProjectilesCount,
					m_BaseValueType = ContextRankBaseValueType.FeatureListRanks,
					m_Feature = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("0477936c0f74841498b5c8753a8062a3").ToReference<BlueprintFeatureReference>(),
					m_FeatureList = new BlueprintFeatureReference[] { ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("de24d9e57d7bad24dbada7389eebcd65").ToReference<BlueprintFeatureReference>(), ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("0477936c0f74841498b5c8753a8062a3").ToReference<BlueprintFeatureReference>() },
					m_Progression = ContextRankProgression.OnePlusDivStep,
					m_StepLevel = 1,
					m_BuffRankMultiplier = 1,
					m_Max = 20,
				};
			}

			return rankConfig;
		}

		public static Sprite DefaultIcon(SpellData spellData)
		{
			Sprite icon;

			Type type = ResourcesLibrary.TryGetBlueprint(BlueprintGuid.Parse(spellData.Icon)).GetType();

			if (type.Equals(typeof(BlueprintAbility)))
			{
				icon = ResourcesLibrary.TryGetBlueprint<BlueprintAbility>(spellData.Icon).m_Icon;
			}
			else if (type.Equals(typeof(BlueprintBuff)))
			{
				icon = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>(spellData.Icon).m_Icon;
			}
			else if (type.Equals(typeof(BlueprintUnit)))
			{
				icon = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>(spellData.Icon).PortraitSafe.SmallPortrait;
			}
			else
			{
				switch (spellData.SpellSchool)
				{
					case SpellSchool.Abjuration:
						icon = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("7f8c1b838ff2d2e4f971b42ccdfa0bfd").m_Icon;
						break;
					case SpellSchool.Conjuration:
						icon = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("ca4a0d68c0408d74bb83ade784ebeb0d").m_Icon;
						break;
					case SpellSchool.Divination:
						icon = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("09595544116fe5349953f939aeba7611").m_Icon;
						break;
					case SpellSchool.Enchantment:
						icon = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("875fff6feb84f5240bf4375cb497e395").m_Icon;
						break;
					case SpellSchool.Evocation:
						icon = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("c3724cfbe98875f4a9f6d1aabd4011a6").m_Icon;
						break;
					case SpellSchool.Illusion:
						icon = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("6750ead44c0c034428c6509c68110375").m_Icon;
						break;
					case SpellSchool.Necromancy:
						icon = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("a9bb3dcb2e8d44a49ac36c393c114bd9").m_Icon;
						break;
					case SpellSchool.Transmutation:
						icon = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("fc519612a3c604446888bb345bca5234").m_Icon;
						break;
					case SpellSchool.None:
						icon = ResourcesLibrary.TryGetBlueprint<BlueprintAbility>("9e1ad5d6f87d19e4d8883d63a6e35568").m_Icon;
						break;
					default:
						icon = ResourcesLibrary.TryGetBlueprint<BlueprintAbility>("9e1ad5d6f87d19e4d8883d63a6e35568").m_Icon;
						break;
				}
			}

			return icon;
		}

		public static void CreateUnitFXList()
		{
			List<UnitFX> unitFXList = new List<UnitFX>();
			foreach (BlueprintList.Entry entry in Kingmaker.Cheats.Utilities.GetAllBlueprints().Entries.Where(entry => entry.Type.Equals(typeof(BlueprintBuff)) && entry != null))
			{
				try
				{
					BlueprintBuff buff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>(entry.Guid);
					try
					{
						if (!unitFXList.Select<UnitFX, string>(unitFx => unitFx.FXName).Contains(buff.FxOnStart.Load(false, false).name + "_FX"))
						{
							unitFXList.Add(new UnitFX { FXName = buff.FxOnStart.Load(false, false).name + "_FX", OldGuid = buff.AssetGuidThreadSafe, NewGuid = BlueprintGuid.NewGuid().ToString() });
						}
					}
					catch (Exception)
					{
						Main.logger.Error("Couldn't add blueprint " + buff.NameForAcronym);
					}
				}
				catch (Exception)
				{
					Main.logger.Log("Failed to get blueprint!");
				}
			}
			UnitFX.SaveUnitFXs(unitFXList);
		}

		public static void CreateUnitBlueprints()
		{
			try
			{
				foreach (SpawnUnit spawnUnit in SpawnUnit.LoadSpawnUnits())
				{
					BlueprintUnit unitBp = BlueprintTool.Create<BlueprintUnit>(spawnUnit.UnitName, spawnUnit.NewGuid);
					BlueprintUnit sourceUnit = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>(spawnUnit.OldGuid);

					SharedStringAsset localizedName = ScriptableObject.CreateInstance<SharedStringAsset>();
					localizedName.name = spawnUnit.LocalizedNameKey + ".Shared";
					localizedName.String = LocalizationTool.GetString(spawnUnit.LocalizedNameKey);

					UnitConfigurator.For(unitBp)
						.SetType(sourceUnit.m_Type)
						.SetRace(sourceUnit.m_Race)
						.SetPortrait(sourceUnit.m_Portrait)
						.SetFaction(ResourcesLibrary.TryGetBlueprint<BlueprintFaction>("1b08d9ed04518ec46a9b3e4e23cb5105")) // Faction: Summoned
						.SetCustomizationPreset(sourceUnit.m_CustomizationPreset)
						.SetLocalizedName(localizedName)
						.SetBrain(sourceUnit.m_Brain)
						.SetHasAssignedChunkId(false)
						.SetPS4ChunkId(sourceUnit.m_PS4ChunkId)
						.SetSize(sourceUnit.Size)
						.SetColor(sourceUnit.Color)
						.SetAlignment(sourceUnit.Alignment)
						.SetPrefab(sourceUnit.Prefab)
						.SetVisual(sourceUnit.Visual)
						.SetFactionOverrides(new FactionOverrides())
						.SetBody(sourceUnit.Body)
						.SetStrength(sourceUnit.Strength)
						.SetDexterity(sourceUnit.Dexterity)
						.SetConstitution(sourceUnit.Constitution)
						.SetIntelligence(sourceUnit.Intelligence)
						.SetWisdom(sourceUnit.Wisdom)
						.SetCharisma(sourceUnit.Charisma)
						.SetSpeed(sourceUnit.Speed)
						.SetSkills(sourceUnit.Skills)
						.SetDisplayName(localizedName.String)
						.SetDescription(sourceUnit.m_Description)
					 .Configure();

					unitBp.ComponentsArray = sourceUnit.ComponentsArray;
					unitBp.m_AddFacts = sourceUnit.m_AddFacts;
					Main.logger.Log("Created unit " + unitBp.NameForAcronym + " with Guid " + unitBp.AssetGuidThreadSafe);
				}
			}
			catch (Exception)
			{
				Main.logger.Error("Something went wrong with the unit blueprints");
			}
		}

		public static void CreateFXBlueprints()
		{
			foreach (UnitFX unitFX in UnitFX.LoadUnitFXs())
			{
				BlueprintBuff unitFXbp = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>(unitFX.OldGuid);
				BuffConfigurator.New(unitFX.FXName, unitFX.NewGuid)
					.SetFlags(new BlueprintBuff.Flags[] { BlueprintBuff.Flags.HiddenInUi, BlueprintBuff.Flags.StayOnDeath })
					.SetFxOnStart(unitFXbp.FxOnStart)
					.SetFxOnRemove(unitFXbp.FxOnRemove)
					.SetResourceAssetIds(unitFXbp.ResourceAssetIds)
					.Configure();
			}
		}

		public static void CreateSpellBlueprints()
		{
			Directory.CreateDirectory(Main.BlueprintsPath);
			List<SpellData> spellsList = new List<SpellData>();
			foreach (string fileName in Directory.GetFiles(Main.BlueprintsPath).Select(file => Path.GetFileName(file)))
			{
				try
				{
					SpellData spellData = SpellData.LoadSpellData(fileName);
					spellsList.Add(spellData);
					CreateSummonSpell(spellData);
				}
				catch (Exception)
				{
					Main.logger.Error("Failed to create spell from file " + fileName);
				}
			}
			foreach (SpellData spellData in spellsList)
			{
				AbilityConfigurator.For(spellData.SpellGuid)
					.AddAbilityVariants(spellData.AbilityVariants.Select<string, Blueprint<BlueprintAbilityReference>>(variant => ResourcesLibrary.TryGetBlueprint<BlueprintAbility>(variant).ToReference<BlueprintAbilityReference>()).ToList())
					.SetParent(spellData.Parent);

				if (spellData.Parent.IsNullOrEmpty())
				{
					BlueprintAbility spell = ResourcesLibrary.TryGetBlueprint<BlueprintAbility>(spellData.SpellGuid);
					spellLists.ForEach(list => AbilityConfigurator.For(spell).AddSpellListComponent(spellData.SpellLevel, list).Configure());
					spellLists.ForEach(list => list.SpellsByLevel[spellData.SpellLevel].Spells.Add(spell));
				}
			}


		}

		public static ContextDiceValue DiceValueSetup(DiceValue diceValue)
		{
			ContextDiceValue contextDiceValue = new ContextDiceValue
			{
				DiceCountValue = diceValue.DiceCountValue,
				DiceType = diceValue.DiceType,
				BonusValue = new ContextValue { ValueType = diceValue.BonusValue.ValueType, Value = diceValue.BonusValue.Value }
			};
			return contextDiceValue;
		}

		public static ContextDurationValue DurationValueSetup(DurationValue durationValue)
		{
			ContextDurationValue contextDurationValue = new ContextDurationValue
			{
				Rate = durationValue.Rate,
				DiceType = durationValue.DiceType,
				DiceCountValue = durationValue.DiceCountValue,
				BonusValue = new ContextValue { ValueType = durationValue.BonusValue.ValueType, Value = durationValue.BonusValue.Value }
			};
			return contextDurationValue;
		}

		public static List<BlueprintSpellList> spellLists = new List<BlueprintSpellList>
		{
			ResourcesLibrary.TryGetBlueprint<BlueprintSpellList>("8443ce803d2d31347897a3d85cc32f53"), // ClericSpellList
			ResourcesLibrary.TryGetBlueprint<BlueprintSpellList>("4d72e1e7bd6bc4f4caaea7aa43a14639"), // MagusSpellList
			ResourcesLibrary.TryGetBlueprint<BlueprintSpellList>("c0c40e42f07ff104fa85492da464ac69"), // ShamanSpelllist
			ResourcesLibrary.TryGetBlueprint<BlueprintSpellList>("e17df9977b879b64e8a8cbb4b3569f19"), // WitchSpellList
			ResourcesLibrary.TryGetBlueprint<BlueprintSpellList>("ba0401fdeb4062f40a7aa95b6f07fe89"), // WizardSpellList
		};

		public static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
		{
			PreserveReferencesHandling = PreserveReferencesHandling.None,
			CheckAdditionalContent = false,
			ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
			DefaultValueHandling = DefaultValueHandling.Include,
			FloatParseHandling = FloatParseHandling.Double,
			Formatting = Formatting.Indented,
			MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
			MissingMemberHandling = MissingMemberHandling.Ignore,
			NullValueHandling = NullValueHandling.Ignore,
			ObjectCreationHandling = ObjectCreationHandling.Replace,
			StringEscapeHandling = StringEscapeHandling.Default,
		};
	}
}
