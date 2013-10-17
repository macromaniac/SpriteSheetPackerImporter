using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Resources;

namespace SSPI
{
    //This just lets us store frame data for sorting, this is non persistant data
    internal class SpriteDimsAndFrame : SpriteDims
    {
        public string name;
        public int frame;
        public SpriteDimsAndFrame(string name, int frame, int x, int y, int w, int h)
            : base(x, y, w, h)
        {
            this.name = name;
            this.frame = frame;
        }
    }
    public class SpriteDims
    {
        public int x, y, w, h;
        public SpriteDims(int x, int y, int w, int h)
        {
            this.x = x;     this.y = y;
            this.w = w;     this.h = h;
        }
    }

    internal class FileData
    {
        public Dictionary<string, List<SpriteDims>> fileData;

        private SpriteDimsAndFrame getSDAF(string toAdd)
        {
            //format of data: "name-frame = x y w h" 
            bool sanityCheck = Regex.IsMatch(toAdd,
                @"^.+(-)[0-9]+\s*=\s*[0-9]+\s+[0-9]+\s+[0-9]+\s+[0-9]+\s*" );
            if (!sanityCheck)
                throw new Exception("improperly formated text: " + toAdd);
            string name = Regex.Match(toAdd, "^.+(-)").ToString().TrimEnd('-');
            string withoutname = Regex.Replace(toAdd, "^.+-", "");
            string withoutequals = Regex.Replace(withoutname,@"\s*=\s*"," ");
            string[] numbers = Regex.Replace(withoutequals, @"\s+", " ").Trim().Split(' ');
            int frame = Int32.Parse(numbers[0]);
            int x = Int32.Parse(numbers[1]);
            int y = Int32.Parse(numbers[2]);
            int w = Int32.Parse(numbers[3]);
            int h = Int32.Parse(numbers[4]);
            return new SpriteDimsAndFrame(name, frame, x, y, w, h);
        }
        private static int compareNameFrame(SpriteDimsAndFrame a, SpriteDimsAndFrame b)
        {
            int strComp = String.Compare(a.name, b.name);
            if (strComp > 0)
                return 1;
            if (strComp < 0)
                return -1;
            if (a.frame > b.frame)
                return 1;
            if (a.frame < b.frame)
                return -1;
            return 0;
        }
        public FileData(string filename)
        {
            fileData = new Dictionary<string, List<SpriteDims>>();
            string totalFilename = filename;
            if (!System.IO.File.Exists(totalFilename))
                throw new Exception("File \"" + totalFilename +  "\" does not exist!");

            string[] lines = System.IO.File.ReadAllLines(filename);
            List<SpriteDimsAndFrame> rawData = new List<SpriteDimsAndFrame>();
            foreach (string line in lines)
            {
                if(line!="")
                    rawData.Add( getSDAF(line) );
            }
            rawData.Sort(compareNameFrame);
            for (int i = 0; i < rawData.Count; ++i)
            {
                if(!fileData.ContainsKey(rawData[i].name))
                    fileData.Add(rawData[i].name,new List<SpriteDims>());
                fileData[rawData[i].name].Add( 
                        new SpriteDims( rawData[i].x, rawData[i].y,rawData[i].w, rawData[i].h));
            }
        }
    }
    public static class Importer
    {
        private static Dictionary<string, FileData> data = new Dictionary<string, FileData>();
        private static ResourceManager rm;
        public static List<SpriteDims> getDims(string filename, string spritename)
        {
            if( !data.ContainsKey(filename) )
                data.Add(filename, new FileData(filename));

            if (!data[filename].fileData.ContainsKey(spritename))
                throw new Exception("Could not find sprite \"" + spritename + "\" in file \"" + filename + "\".");

            return data[filename].fileData[spritename];
        }
        /*
        public static List<SpriteDims> getDims(string resourcefilename, string resourcename, string spritename)
        {
            return new List<SpriteDims>();
        }*/
        public static void clean()
        {
            data.Clear();
        }
    }
}
