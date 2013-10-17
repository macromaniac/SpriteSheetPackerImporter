using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;

namespace Example
{
    class Example
    {
        static void Main(String[] Args)
        {
            string cd = System.IO.Directory.GetCurrentDirectory();
            //Cost: Disk read if filename hasn't been done before and a hash call
            List<SSPI.SpriteDims> dims = SSPI.Importer.getDims( cd + "\\12345.txt", "number" );
            //Empties hash
            SSPI.Importer.clean();
        }
    }
}