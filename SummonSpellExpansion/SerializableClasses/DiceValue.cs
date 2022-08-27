using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Mechanics;
using Newtonsoft.Json;
using System;

namespace SummonSpellExpansion.SerializableClasses
{
	[Serializable]
	public class DiceValue
	{
		[JsonProperty]
		public DiceType DiceType;
		[JsonProperty]
		public int DiceCountValue;
		[JsonProperty]
		public ContextValue BonusValue = new ContextValue { ValueType = ContextValueType.Rank, Value = 1 };
	}
}
