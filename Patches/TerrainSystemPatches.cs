using MOOB.Extensions;
using Game.Simulation;
using HarmonyLib;
using System;
using UnityEngine;

namespace MOOB.Patches
{
    [HarmonyPatch( typeof( TerrainSystem ), "ReplaceHeightmap" )]
    class TerrainSystem_ReplaceHeightmap
    {
        static bool Prefix( TerrainSystem __instance, Texture srcHeightmap )
        {
            try
            {
                __instance.ApplyHeightMap( srcHeightmap );

                return false;
            }
            catch ( Exception e )
            {
                Debug.LogException( e );

            }
            return true; // Run default function
        }
    }
}
