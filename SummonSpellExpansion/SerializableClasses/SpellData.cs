using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Mechanics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;

namespace SummonSpellExpansion.SerializableClasses
{
	[Serializable]
	public class SpellData
	{
		// Strings
		[JsonProperty]
		public string SpellName = "";
		[JsonProperty]
		public string SpellGuid = "";
		[JsonProperty]
		public string DisplayName = "";
		[JsonProperty]
		public string Description = "";
		[JsonProperty]
		public string Icon = "";

		// Unit and Summon Pool

		[JsonProperty]
		public string SpawnUnit;
		[JsonProperty]
		public string SummonPool = "";
		[JsonProperty]
		public string[] UnitFX = new string[0];

		// Dice
		[JsonProperty]
		public DiceValue DiceValue = new DiceValue
		{
			DiceType = Kingmaker.RuleSystem.DiceType.Zero,
			DiceCountValue = 0,
			BonusValue = new ContextValue { ValueType = ContextValueType.Simple, Value = 1 }
		};
		[JsonProperty]
		public DurationValue DurationValue = new DurationValue
		{
			Rate = DurationRate.Minutes,
			DiceType = Kingmaker.RuleSystem.DiceType.Zero,
			DiceCountValue = 0,
			BonusValue = new DurationBonusValue { Value = 1, ValueType = ContextValueType.Simple },
		};

		[JsonProperty]
		public string Parent = "";
		[JsonProperty]
		public string[] AbilityVariants = new string[0];
		[JsonProperty]
		public int SpellLevel = 0;
		[JsonProperty]
		public SpellSchool SpellSchool = SpellSchool.Conjuration;

		public static void SaveSpellData(string fileName, SpellData spellData)
		{
			Directory.CreateDirectory(Main.BlueprintsPath);
			string path = $"{Main.BlueprintsPath}{Path.DirectorySeparatorChar}{fileName}";
			File.WriteAllText(path, JsonConvert.SerializeObject(spellData, Utilities.SerializerSettings));
		}

		public static SpellData LoadSpellData(string fileName)
		{
			string path = $"{Main.BlueprintsPath}{Path.DirectorySeparatorChar}{fileName}";
			return JsonConvert.DeserializeObject<SpellData>(File.ReadAllText(path));
		}
	}
}
