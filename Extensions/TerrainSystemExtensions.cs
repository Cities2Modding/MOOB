using Game.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MOOB.Extensions
{
    /// <summary>
    /// Extension methods for TerrainSystem class.
    /// </summary>
    public static class TerrainSystemExtensions
    {
        // This field is used to access the 'm_CascadeReset' field in TerrainSystem, which is private.
        // Reflection is used here to access this non-public field.
        static FieldInfo _cascadeReset = typeof( TerrainSystem ).GetFields( BindingFlags.Instance | BindingFlags.NonPublic )
                                                        .FirstOrDefault( f => f.Name == "m_CascadeReset" );
        /// <summary>
        /// Applies a heightmap texture to a terrain system.
        /// </summary>
        /// <param name="terrainSystem">The terrain system to which the heightmap will be applied.</param>
        /// <param name="heightMap">The heightmap texture to apply.</param>
        public static void ApplyHeightMap( this TerrainSystem terrainSystem, Texture heightMap )
        {
            // Retrieve the RenderTexture from the terrain system.
            var hmRenderTexture = ( RenderTexture ) terrainSystem.heightmap;

            // Blit (copy) the heightMap texture onto the terrain system's render texture.
            Graphics.Blit( heightMap, hmRenderTexture );

            // Set the 'm_CascadeReset' field of the terrain system to true.
            // This might be necessary to update the internal state of the terrain system after changing the heightmap.
            _cascadeReset.SetValue( terrainSystem, true );

            // Update a global shader property with the new heightmap texture.
            // This is often necessary for shaders that render terrain to use the correct heightmap.
            Shader.SetGlobalTexture( "colossal_TerrainTexture", hmRenderTexture );
        }

        /// <summary>
        /// Exports the heightmap of a terrain system as a Texture2D.
        /// </summary>
        /// <param name="terrainSystem">The terrain system from which to export the heightmap.</param>
        /// <returns>A Texture2D containing the heightmap data.</returns>
        public static Texture ExportHeightMap( this TerrainSystem terrainSystem )
        {
            // Retrieve the RenderTexture from the terrain system.
            var hmRenderTexture = ( RenderTexture ) terrainSystem.heightmap;

            // Set the active RenderTexture to the terrain's heightmap render texture.
            RenderTexture.active = hmRenderTexture;

            // Create a new Texture2D to import the render texture data.
            var heightMap = new Texture2D( hmRenderTexture.width, hmRenderTexture.height, TextureFormat.RGBA32, false );

            // Read pixels from the render texture and apply them to the Texture2D.
            heightMap.ReadPixels( new Rect( 0, 0, hmRenderTexture.width, hmRenderTexture.height ), 0, 0 );
            heightMap.Apply( );

            // Reset the active RenderTexture to null to avoid any rendering conflicts or leaks.
            RenderTexture.active = null;

            // Return the Texture2D with the imported heightmap data.
            return heightMap;
        }
    }
}
