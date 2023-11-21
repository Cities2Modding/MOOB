using MOOB.Extensions;
using MOOB.IO;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace MOOB.Helpers
{
    /// <summary>
    /// Runs a gaussian blur compute shader on our R16 heightmap.
    /// </summary>
    /// <remarks>
    /// (Reduces terracing effects due to conversion from R8.)
    /// </remarks>
    public class BitDepthTransformer : MonoBehaviour
    {
        public Texture texture;
        public Byte[] buffer;
        public int width;
        public int height;
        public float intensity = 0.5f;
        public Action<Texture> onComplete;

        private Texture workingTexture;
        private RenderTexture renderTexture;
        private GraphicsFence fence;
        private static ComputeShader gaussianBlurShader;
        private float startTime;
        private bool isFinished;
        private int currentIteration;

        const int ITERATIONS = 4;
        const float WAIT_DURATION = 3f; // Enforce a 3 second wait?

        private void Update( )
        {
            if ( isFinished || renderTexture == null ) // No job running
                return;

            // Check if our job is done
            //if ( fence.passed == true )
            if ( Time.time >= startTime + WAIT_DURATION )
            {
                if ( currentIteration + 1 < ITERATIONS )
                {
                    currentIteration++;
                    RunIteration( );
                }
                else
                {
                    Debug.Log( "BitDepthTransformer: Finished" );
                    isFinished = true;
                    onComplete?.Invoke( renderTexture );

                    // Remove the instance
                    Destroy( gameObject );
                }
            }
        }

        /// <summary>
        /// Run the compute shader
        /// </summary>
        public void Run( bool convertTo16Bit = true )
        {
            Debug.Log( "BitDepthTransformer: Running!" );

            // Preload our compute shader
            if ( gaussianBlurShader == null )
                gaussianBlurShader = LoadComputeShader( );

            if ( gaussianBlurShader == null )
            {
                Debug.LogError( "gaussianBlurShader is NULL" );
                return;
            }

            renderTexture = new RenderTexture( width, height, 0, RenderTextureFormat.R16 )
            {
                enableRandomWrite = true
            };

            renderTexture.Create( );

            if ( convertTo16Bit )
                texture = Texture2DExtensions.Convert8BitTo16Bit( buffer, width, height );

            workingTexture = texture;

            RunIteration( );
        }

        /// <summary>
        /// Run a blur iteration
        /// </summary>
        private void RunIteration()
        {
            // If we're past the first iteration pull the render texture back into the
            // working texture for the next iteration
            if ( currentIteration > 0 )
                workingTexture = renderTexture.ToTexture2D( );

            Debug.Log( "BitDepthTransformer: Running iteration: " + currentIteration );

            int kernelHandle = gaussianBlurShader.FindKernel( "CSMain" );
            gaussianBlurShader.SetTexture( kernelHandle, "InputTexture", workingTexture );
            gaussianBlurShader.SetTexture( kernelHandle, "OutputTexture", renderTexture );
            gaussianBlurShader.SetFloat( "Intensity", intensity );
            gaussianBlurShader.Dispatch( kernelHandle, workingTexture.width / 8, workingTexture.height / 8, 1 );

            // As Graphics Fence isn't supported by many platforms we'll just have to random guess via timer
            startTime = Time.time;

            // when it's done.
            //fence = Graphics.CreateAsyncGraphicsFence( );
        }

        /// <summary>
        /// Add a BitDepthTransformer instance
        /// </summary>
        /// <returns></returns>
        private static BitDepthTransformer AddInstance( )
        {
            var gameObject = new GameObject( nameof( BitDepthTransformer ) );
            return gameObject.AddComponent<BitDepthTransformer>( );
        }

        /// <summary>
        /// Queues up a heightmap conversion
        /// </summary>
        /// <remarks>
        /// (Defaults to also doing 8-bit to 16-bit conversion.)
        /// </remarks>
        /// <param name="buffer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="onComplete"></param>
        public static void ConvertHeightMap( byte[] buffer, int width, int height, Action<Texture> onComplete )
        {
            var transformer = AddInstance( );
            transformer.buffer = buffer;
            transformer.width = width;
            transformer.height = height;
            transformer.intensity = 1f;
            transformer.onComplete = onComplete;
            transformer.Run( true );
        }

        /// <summary>
        /// Queues up a heightmap conversion
        /// </summary>
        /// <remarks>
        /// (Expects an existing 16-bit Texture in R16.)
        /// </remarks>
        /// <param name="texture"></param>
        /// <param name="onComplete"></param>
        public static void ConvertHeightMap( Texture texture, Action<Texture> onComplete, bool convertTo16Bit = false )
        {
            var transformer = AddInstance( );
            transformer.texture = texture;
            transformer.width = texture.width;
            transformer.height = texture.height;
            transformer.intensity = 1f;
            transformer.onComplete = onComplete;
            transformer.Run( convertTo16Bit );
        }

        /// <summary>
        /// Loads the compute shader from our Unity compiled asset bundle
        /// </summary>
        /// <returns></returns>
        private static ComputeShader LoadComputeShader()
        {
            var computeShaderData = EmbeddedResource.Load( "MOOB.Unity.modblur" );
            
            if ( computeShaderData == null )
            {
                Debug.LogError( "Failed to load embedded resource." );
                return null;
            }

            var myLoadedAssetBundle = AssetBundle.LoadFromMemory( computeShaderData );

            if ( myLoadedAssetBundle == null )
            {
                Debug.LogError( "Failed to load AssetBundle from memory." );
                return null;
            }

            // Load an asset from the bundle
            var loadedShader = myLoadedAssetBundle.LoadAsset<ComputeShader>( "assets/modblur.compute" );

            if ( loadedShader == null )
            {
                Debug.LogError( "Failed to load the compute shader from the AssetBundle." );
                return null;
            }

            myLoadedAssetBundle.Unload( false );
            return loadedShader;
        }
    }
}
