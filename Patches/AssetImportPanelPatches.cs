using Game.UI.Editor;
using HarmonyLib;
namespace MOOB.Patches
{
    [HarmonyPatch( typeof( AssetImportPanel ), "ImportNotReady" )]
    class AssetImportPanel_ImportNotReadyPatch
    {
        public static bool isReady = false;

        static bool Prefix( ref bool __result )
        {
            __result = !isReady;

            return false; // Ignore original function
        }
    }

    [HarmonyPatch( typeof( AssetImportPanel ), "ImportAssets" )]
    class AssetImportPanel_ImportAssetsPatch
    {
        static bool Prefix( )
        {

            return false; // Ignore original function
        }
    }
}
