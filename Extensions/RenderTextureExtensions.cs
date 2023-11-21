using UnityEngine;

namespace MOOB.Extensions
{

    public static class RenderTextureExtensions
    {
        /// <summary>
        /// Convert a RenderTexture to a Texture2D
        /// </summary>
        /// <param name="rTex"></param>
        /// <returns></returns>
        public static Texture2D ToTexture2D( this RenderTexture rTex )
        {
            Texture2D tex = new Texture2D( rTex.width, rTex.height, TextureFormat.R16, false );
            RenderTexture.active = rTex;
            tex.ReadPixels( new Rect( 0, 0, rTex.width, rTex.height ), 0, 0 );
            tex.Apply( );
            RenderTexture.active = null; // Added to avoid errors
            return tex;
        }
    }

}
