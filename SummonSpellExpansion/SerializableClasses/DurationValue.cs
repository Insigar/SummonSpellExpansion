using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Mechanics;
using Newtonsoft.Json;
using System;

namespace SummonSpellExpansion.SerializableClasses
{
	[Serializable]
	public class DurationValue
	{
		[JsonProperty]
		public DurationRate Rate;
		[JsonProperty]
		public DiceType DiceType;
		[JsonProperty]
		public int DiceCountValue;
		[JsonProperty]
		public DurationBonusValue BonusValue = new DurationBonusValue { ValueType = ContextValueType.Rank, Value = 1 };
		//[JsonProperty]
		//public bool ScaleWithLevel;
	}
	[Serializable]
	public class DurationBonusValue
	{
		[JsonProperty]
		public ContextValueType ValueType;
		[JsonProperty]
		public int Value;
	}
}
