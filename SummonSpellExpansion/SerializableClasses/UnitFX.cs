using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SummonSpellExpansion.SerializableClasses
{
	[Serializable]
	public class UnitFX
	{
		[JsonProperty]
		public string FXName;
		[JsonProperty]
		public string NewGuid;
		[JsonProperty]
		public string OldGuid;

		//public static void SaveUnitFXs(Dictionary<string, UnitFX> unitFXs)
		public static void SaveUnitFXs(List<UnitFX> unitFXs)
		{
			string path = $"{Main.ModPath}{"UnitFX.json"}";
			File.WriteAllText(path, JsonConvert.SerializeObject(unitFXs, Utilities.SerializerSettings));
		}

		//public static Dictionary<string, UnitFX> LoadUnitFXs()
		public static List<UnitFX> LoadUnitFXs()
		{
			string path = $"{Main.ModPath}{"UnitFX.json"}";
			//return JsonConvert.DeserializeObject<Dictionary<string, UnitFX>>(File.ReadAllText(path));
			return JsonConvert.DeserializeObject<List<UnitFX>>(File.ReadAllText(path));
		}
	}
}
