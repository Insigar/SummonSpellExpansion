using HarmonyLib;
using System.Reflection;
using UnityModManagerNet;

namespace SummonSpellExpansion
{
	public class Main
	{
		public static bool Enabled;
		public static Harmony harmony;
		public static UnityModManager.ModEntry.ModLogger logger;

		public static string ModPath;
		public static string BlueprintsPath;
		public static string LocalizationPath;

		static bool Load(UnityModManager.ModEntry modEntry)
		{
			logger = modEntry.Logger;
			modEntry.OnToggle = OnToggle;

			ModPath = modEntry.Path;
			BlueprintsPath = ModPath + "Blueprints";
			LocalizationPath = ModPath + "Localization";

			var harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			return true;
		}

		static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			Enabled = value;
			return true;
		}
	}
}
