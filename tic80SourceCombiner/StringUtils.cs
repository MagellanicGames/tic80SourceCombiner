using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic80SourceCombiner
{
    struct StringPos
    {
        public int start;
        public int length;

        public StringPos(int s, int l)
        {
            start = s;
            length = l;
        }
    }

    class StringUtils
    {
        public static StringPos DataPosition(string text, string subStr1, string subStr2)
        {
            StringPos result;

            result.start = text.IndexOf(subStr1) + subStr1.Length;
            result.length = text.IndexOf(subStr2) - result.start;
            return result;
        }

        public static string SubString(string originalString, string subStr1, string subStr2)
        {
            StringPos dataPositions = DataPosition(originalString, subStr1, subStr2);            
            return originalString.Substring(dataPositions.start, dataPositions.length);
        }
    }
}
