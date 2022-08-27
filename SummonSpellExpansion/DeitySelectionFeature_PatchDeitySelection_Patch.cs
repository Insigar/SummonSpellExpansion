using ExpandedContent.Tweaks.Deities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace SummonSpellExpansion
{
	[HarmonyPatch(typeof(DeitySelectionFeature), nameof(DeitySelectionFeature.PatchDeitySelection))]
	public class DeitySelectionFeature_PatchDeitySelection_Patch
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> newInstructions = new List<CodeInstruction>(instructions);
			for (int i = 917; i <= 964; i++)
			{
				newInstructions[i].opcode = OpCodes.Nop;
			}
			return newInstructions.AsEnumerable();
		}
	}
}
