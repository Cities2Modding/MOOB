using Game.Simulation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace MOOB.Extensions
{
    /// <summary>
    /// Extension methods for Unity's Texture2D class.
    /// </summary>
    public static class Texture2DExtensions
    {
        /// <summary>
        /// Exports a Texture2D to a file in 16-bit RAW format.
        /// </summary>
        /// <param name="texture">The Texture2D to export. Expected to be in a 16-bit format.</param>
        /// <param name="filePath">The file path where the texture should be saved.</param>
        /// <returns>True if the texture was successfully saved, false otherwise.</returns>
        public static bool Save16BitRaw( this Texture2D texture, string filePath )
        {
            try
            {
                var rawData = new byte[texture.width * texture.height * 2]; // 2 bytes per pixel
                var pixels = texture.GetPixels( );
                int byteIndex = 0;

                for ( int i = 0; i < pixels.Length; i++ )
                {
                    // Convert to grayscale
                    float grayScale = ( pixels[i].r + pixels[i].g + pixels[i].b ) / 3;
                    ushort gray16Bit = ( ushort ) ( grayScale * ushort.MaxValue );

                    // Split 16-bit value into two bytes
                    rawData[byteIndex++] = ( byte ) ( gray16Bit & 0xFF );
                    rawData[byteIndex++] = ( byte ) ( ( gray16Bit >> 8 ) & 0xFF );
                }

                File.WriteAllBytes( filePath, rawData );
                Debug.Log( $"Heightmap exported successfully to {filePath}" );
                return true;
            }
            catch ( Exception e )
            {
                Debug.LogError( $"Error exporting heightmap: {e.Message}" );
                return false;
            }
        }


        /// <summary>
        /// Loads a 16-bit RAW file into a Texture2D.
        /// </summary>
        /// <param name="texture">The Texture2D to load the data into.</param>
        /// <param name="filePath">The file path of the 16-bit RAW file.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <returns>True if the texture was successfully loaded, false otherwise.</returns>
        public static bool Load16BitRaw( string filePath, int width, int height, out Texture2D texture )
        {
            texture = new Texture2D( width, height, TextureFormat.R16, false );

            try
            {
                byte[] rawData = File.ReadAllBytes( filePath );

                if ( rawData.Length != width * height * 2 )
                {
                    Debug.LogError( "Load16BitRaw failed: The size of the RAW file does not match the specified dimensions." );
                    return false;
                }

                texture.LoadRawTextureData( rawData );
                texture.Apply( );

                Debug.Log( "Texture loaded successfully from RAW file." );
                return true;
            }
            catch ( IOException e )
            {
                Debug.LogError( $"Load16BitRaw failed: Unable to read file at {filePath}. Error: {e.Message}" );
                return false;
            }
        }
    }
}
