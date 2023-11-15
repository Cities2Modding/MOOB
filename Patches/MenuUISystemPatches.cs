using Game.UI.Menu;
using HarmonyLib;

namespace MOOB.Patches
{
    /// <summary>
    /// Enable editor options (These may not be fully functional yet)
    /// </summary>
    [HarmonyPatch( typeof( MenuUISystem ), "IsEditorEnabled" )]
    class MenuUISystems_IsEditorEnabledPatch
    {
        static bool Prefix( ref bool __result )
        {
            __result = true;

            return false; // Ignore original function
        }
    }
}