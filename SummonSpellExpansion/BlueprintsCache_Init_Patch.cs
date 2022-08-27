using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;

namespace SummonSpellExpansion
{
	[HarmonyPatch(typeof(BlueprintsCache), "Init")]
	public static class BlueprintsCache_Init_Patch
	{
		static bool loaded = false;

		public static void Postfix()
		{
			if (loaded) return;
			loaded = true;

			// Call your code to edit blueprints here
			//Utilities.CreateFXBlueprints();
			Utilities.CreateUnitBlueprints();
			Utilities.CreateSpellBlueprints();
		}
	}
}
