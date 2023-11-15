using System;
using System.Runtime.InteropServices;

namespace MOOB.Helpers
{
    public class OpenFileDialog
    {
        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
        private class OpenFileName
        {
            public int structSize = 0;
            public IntPtr dlgOwner = IntPtr.Zero;
            public IntPtr instance = IntPtr.Zero;
            public string filter = null;
            public string customFilter = null;
            public int maxCustFilter = 0;
            public int filterIndex = 0;
            public string file = null;
            public int maxFile = 0;
            public string fileTitle = null;
            public int maxFileTitle = 0;
            public string initialDir = null;
            public string title = null;
            public int flags = 0;
            public short fileOffset = 0;
            public short fileExtension = 0;
            public string defExt = null;
            public IntPtr custData = IntPtr.Zero;
            public IntPtr hook = IntPtr.Zero;
            public string templateName = null;
            public IntPtr reservedPtr = IntPtr.Zero;
            public int reservedInt = 0;
            public int flagsEx = 0;
        }

        [DllImport( "comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        private static extern bool GetOpenFileName( [In, Out] OpenFileName ofn );

        public static string ShowDialog( string filter = "All Files\0*.*\0", string defaultExt = "" )
        {
            OpenFileName ofn = new OpenFileName( );

            ofn.structSize = Marshal.SizeOf( ofn );
            ofn.filter = filter;
            ofn.file = new string( new char[256] );
            ofn.maxFile = ofn.file.Length;
            ofn.fileTitle = new string( new char[64] );
            ofn.maxFileTitle = ofn.fileTitle.Length;
            ofn.initialDir = UnityEngine.Application.dataPath;
            ofn.title = "Open File";
            ofn.defExt = defaultExt;

            var result = GetOpenFileName( ofn );

            if ( result )
            {
                return ofn.file;
            }
            else
            {
                return null;
            }
        }
    }
}
