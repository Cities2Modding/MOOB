using MOOB.Extensions;
using MOOB.Helpers;
using Game.Simulation;
using Game.UI.Editor;
using HarmonyLib;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MOOB.Patches
{
    /// <summary>
    /// Disables error message and introduces working export heightmap functionality
    /// </summary>
    [HarmonyPatch( typeof( TerrainPanelSystem ), "OnSaveHeightmap" )]
    class TerrainPanelSystem_OnSaveHeightmapPatch
    {
        public static MethodInfo _closeSubPanel = typeof( TerrainPanelSystem ).GetMethods( BindingFlags.Instance | BindingFlags.NonPublic ).FirstOrDefault( m => m.Name == "CloseSubPanel" );

        static bool Prefix( ref TerrainPanelSystem __instance, string fileName, Colossal.Hash128? overwriteGuid )
        {
            _closeSubPanel.Invoke( __instance, null );

            var terrainSystem = __instance.World.GetExistingSystemManaged<TerrainSystem>( );
            var heightMapTexture = ( Texture2D ) terrainSystem.ExportHeightMap( );
            var documentsPath = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
            var savePath = Path.Combine( documentsPath, fileName + ".raw" );

            if ( heightMapTexture.Save16BitRaw( savePath ) )
                OpenExplorer( savePath );

            return false; // Skip original method
        }

        /// <summary>
        /// Opens explorer on Windows, selecting the exported heightmap
        /// </summary>
        /// <param name="filePath"></param>
        static void OpenExplorer( string filePath )
        {
            try
            {
                // The argument uses /select, to specify the file to highlight
                System.Diagnostics.Process.Start( "explorer.exe", $"/select,\"{filePath}\"" );
            }
            catch ( Exception ex )
            {
                Debug.LogError( "An error occurred: " + ex.Message );
            }
        }
    }

    [HarmonyPatch( typeof( TerrainPanelSystem ), "ShowImportHeightmapPanel" )]
    class TerrainPanelSystem_ShowImportHeightmapPanelPatch
    {
        static bool Prefix( ref TerrainPanelSystem __instance )
        {
            var file = OpenFileDialog.ShowDialog( "Image files\0*.raw;*.png;*.tiff\0", ".raw" );

            if ( !string.IsNullOrEmpty( file ) )
            {
                var terrainSystem = __instance.World.GetExistingSystemManaged<TerrainSystem>( );

                Texture2DExtensions.LoadHeightMap( file, terrainSystem.ApplyHeightMap );
            }
            return false; // Skip original method
        }
    }

    [HarmonyPatch( typeof( TerrainPanelSystem ), "OnLoadHeightmap" )]
    class TerrainPanelSystem_OnLoadHeightmapPatch
    {
        static bool Prefix( ref TerrainPanelSystem __instance, Colossal.Hash128 guid )
        {
            TerrainPanelSystem_OnSaveHeightmapPatch._closeSubPanel.Invoke( __instance, null );

            // We do nothing here as we use our own image open with no display

            return false; // Skip original method
        }
    }
}
