using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SummonSpellExpansion.SerializableClasses
{
	[Serializable]
	public class SpawnUnit
	{
		[JsonProperty]          // CR_Job/Faction_Class_Race_Sex_VariantNumber
		public string UnitName; // CR4_Militia_Paladin_Human_Male_1
		[JsonProperty]
		public string LocalizedNameKey;
		[JsonProperty]
		public string NewGuid;
		[JsonProperty]
		public string OldGuid;

		public static void SaveSpawnUnits(List<SpawnUnit> spawnUnits)
		{
			string path = $"{Main.ModPath}{"SpawnUnits.json"}";
			File.WriteAllText(path, JsonConvert.SerializeObject(spawnUnits, Utilities.SerializerSettings));
		}

		public static List<SpawnUnit> LoadSpawnUnits()
		{
			string path = $"{Main.ModPath}{"SpawnUnits.json"}";
			return JsonConvert.DeserializeObject<List<SpawnUnit>>(File.ReadAllText(path));
		}
	}
}
