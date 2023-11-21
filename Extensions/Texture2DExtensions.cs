using Colossal.AssetPipeline.Importers;
using MOOB.Helpers;
using System;
using System.IO;
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
        /// <param name="buffer">The buffer of the 16-bit RAW file.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <returns>True if the texture was successfully loaded, false otherwise.</returns>
        public static Texture2D Load16BitRaw( byte[] buffer, int width, int height )
        {
            if ( buffer.Length != width * height * 2 )
            {
                Debug.LogError( "Load16BitRaw failed: The size of the RAW file does not match the specified dimensions." );
                return null;
            }

            var texture = new Texture2D( width, height, TextureFormat.R16, false );
            texture.LoadRawTextureData( buffer );
            texture.Apply( );

            Debug.Log( "Texture loaded successfully from RAW file." );
            return texture;
        }

        /// <summary>
        /// Loads an 8-bit RAW file into a Texture2D.
        /// </summary>
        /// <param name="texture">The Texture2D to load the data into.</param>
        /// <param name="buffer">The buffer of the 8-bit RAW file.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <returns>True if the texture was successfully loaded, false otherwise.</returns>
        public static Texture2D Load8BitRaw( byte[] buffer, int width, int height )
        {
            if ( buffer.Length != width * height )
            {
                Debug.LogError( "Load8BitRaw failed: The size of the RAW file does not match the specified dimensions." );
                return null;
            }

            var texture = new Texture2D( width, height, TextureFormat.R8, false );
            texture.LoadRawTextureData( buffer );
            texture.Apply( );

            Debug.Log( "Texture loaded successfully from RAW file." );
            return texture;
        }

        public static Texture LoadTextureCO( string filePath, Vector2Int? resizeTo = null )
        {
            var importer = new DefaultTextureImporter( );
            var texture = importer.Import( importer.GetDefaultSettings( ), filePath, null );

            if ( texture != null )
            {
                var unityTexture = texture.ToUnityTextureRaw( );

                Debug.Log( "Loaded unityTexture: " + filePath + " Format: " + unityTexture.graphicsFormat );

                return unityTexture;
            }

            return null;
        }

        /// <summary>
        /// Loads a RAW file into a Texture2D.
        /// </summary>
        /// <remarks>
        /// (Partly async, action is invoked when complete with the resulting texture.
        /// It will run a gaussian compute shader and handle 8-bit to 16-bit conversion.)
        /// </remarks>
        /// <param name="filePath">The file path of the 16-bit RAW file.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        public static void LoadRaw( string filePath, int width, int height, Action<Texture> onComplete )
        {
            try
            {
                var buffer = File.ReadAllBytes( filePath );

                if ( buffer.Length == width * height ) // It's an 8-bit raw, run a conversion and gaussian blur
                {
                    BitDepthTransformer.ConvertHeightMap( buffer, width, height, onComplete );
                }
                else if ( buffer.Length == width * height * 2 ) // It's a 16-bit raw
                {
                    onComplete?.Invoke( Load16BitRaw( buffer, width, height ) );
                }
                else
                {
                    Debug.LogError( "LoadRaw failed: The size of the RAW file does not match the specified dimensions." );
                }
            }
            catch ( IOException e )
            {
                Debug.LogError( $"LoadRaw failed: Unable to read file at {filePath}. Error: {e.Message}" );
            }
        }

        /// <summary>
        /// Converts an 8-bit grayscale RAW file to a 16-bit Texture2D in Unity.
        /// Optionally applies a smoothing algorithm to reduce terracing.
        /// </summary>
        /// <param name="buffer">The data buffer of the 8-bit RAW file.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <returns>A Texture2D in 16-bit format, or null if an error occurs.</returns>
        public static Texture2D Convert8BitTo16Bit( byte[] buffer, int width, int height )
        {
            if ( buffer.Length != width * height )
            {
                Debug.LogError( "File size does not match the expected dimensions." );
                return null;
            }

            var texture16Bit = new Texture2D( width, height, TextureFormat.R16, false );
            var pixels = new byte[width * height * 2]; // 2 bytes per pixel for R16

            for ( var i = 0; i < buffer.Length; i++ )
            {
                var scaledValue = ( ushort ) ( buffer[i] * 257 ); // Scale 8-bit to 16-bit
                pixels[i * 2] = ( byte ) ( scaledValue & 0xFF ); // Low byte
                pixels[i * 2 + 1] = ( byte ) ( scaledValue >> 8 ); // High byte
            }

            texture16Bit.LoadRawTextureData( pixels );
            texture16Bit.Apply( );

            return texture16Bit;
        }

        /// <summary>
        /// Load a PNG or RAW heightmap
        /// </summary>
        /// <remarks>
        /// (Converts 8-bit images accordingly, applying a gaussian blur compute shader
        /// to eliminate terracing.)
        /// </remarks>
        /// <param name="filePath"></param>
        /// <param name="onComplete"></param>
        public static void LoadHeightMap( string filePath, Action<Texture> onComplete )
        {
            if ( filePath.EndsWith( ".png" ) || filePath.EndsWith( ".tiff" ) )
            {
                var texture = LoadTextureCO( filePath );

                if ( texture == null )
                {
                    Debug.Log( "LoadHeightMap: Could not load heightmap " + filePath );
                    return;
                }

                // Already in 16-bit so we just do a blur instead in the BitDepthTransformer
                // If it's a CS1 heightmap convert it
                if ( texture.width == 1081 && texture.height == 1081 )
                {
                    var convertedHeightmap = texture.ConvertCS1ToCS2( ); 
                    BitDepthTransformer.ConvertHeightMap( convertedHeightmap, onComplete );
                }
                else
                    BitDepthTransformer.ConvertHeightMap( texture, onComplete );
            }
            else
            {
                LoadRaw( filePath, 4096, 4096, onComplete );
            }
        }

        /// <summary>
        /// Converts a CS1 heightmap to a CS2 heightmap, padding out the
        /// out of bounds area.
        /// </summary>
        /// <param name="originalTexture"></param>
        /// <param name="elevationOffset"></param>
        /// <returns></returns>
        public static Texture ConvertCS1ToCS2( this Texture originalTexture, float elevationOffset = 0.058f )
        {
            var cs1PixelArea = 298.5984f / ( 1081f * 1081f );
            var cs2PixelArea = 205.52f / ( 4096f * 4096f );
            var scaleFactor = Mathf.Sqrt( cs1PixelArea / cs2PixelArea );

            var newWidth = Mathf.RoundToInt( originalTexture.width * scaleFactor );
            var newHeight = Mathf.RoundToInt( originalTexture.height * scaleFactor );

            // Scale the texture
            RenderTexture scaledRT = RenderTexture.GetTemporary( newWidth, newHeight, 24, RenderTextureFormat.R16 );
            Graphics.Blit( originalTexture, scaledRT );

            // Crop the texture to 4096x4096
            RenderTexture croppedRT = RenderTexture.GetTemporary( 4096, 4096, 24, RenderTextureFormat.R16 );
            Graphics.Blit( scaledRT, croppedRT );

            // Convert the cropped RenderTexture to Texture2D
            Texture2D outputTexture = new Texture2D( 4096, 4096, TextureFormat.R16, false );
            RenderTexture.active = croppedRT;
            outputTexture.ReadPixels( new Rect( 0, 0, 4096, 4096 ), 0, 0 );

            // Adjust for elevation offset
            for ( int y = 0; y < outputTexture.height; y++ )
            {
                for ( int x = 0; x < outputTexture.width; x++ )
                {
                    float currentHeight = outputTexture.GetPixel( x, y ).r;
                    currentHeight += elevationOffset; // Adjust the elevation
                    currentHeight = Mathf.Clamp( currentHeight, 0f, 1f ); // Ensure the value stays within the valid range
                    outputTexture.SetPixel( x, y, new Color( currentHeight, currentHeight, currentHeight, 1 ) );
                }
            }
            outputTexture.Apply( );

            // Clean up
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary( scaledRT );
            RenderTexture.ReleaseTemporary( croppedRT );

            return outputTexture;
        }
    }
}
