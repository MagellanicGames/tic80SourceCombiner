using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tic80SourceCombiner
{
    class Program
    {

        static int propertiesDefinePos = 0;
        static int propertiesEndDefinePos = 0;
        static int includeDefinePos = 0;
        static int includeDefineEndPos = 0;
        
        static List<string> outputCode = new List<string>();

        static List<string> includeNames = new List<string>();

        static void Main(string[] args)
        {
            string inputFile;
            string outputFile;

           
            List<string> arguments = ProcessArguments(args);
            if (arguments == null)
            {
                Console.WriteLine("Please enter main .lua file to process (file with includes).");
                inputFile = Console.ReadLine();
                Console.WriteLine("Enter an output file name (this will overwrite if file already exists).");
                outputFile = Console.ReadLine();            
            }
            else
            {
                inputFile = arguments[0];
                outputFile = arguments[1];
            }

            if (!inputFile.Contains(".lua"))
                inputFile = inputFile + ".lua";

            List<string> mainFileWithIncludes = RetrieveCode(inputFile);

            if (mainFileWithIncludes.Count < 1)
                return;

            if (RetrieveProperties(mainFileWithIncludes) == false) //return if tic80 properties are not defined
                return;


            includeNames.AddRange(CollectIncludeFilenames(inputFile,mainFileWithIncludes));

           for(int i = 0; i < includeNames.Count; i++)
            {
                List<string> includeCode = RetrieveCode(includeNames[i]);
                outputCode.AddRange(includeCode); //add included file's code/lines
            }

            mainFileWithIncludes.RemoveRange(0, includeDefineEndPos + 1); //remove include defines
            outputCode.AddRange(mainFileWithIncludes); //add remaining non-include code;            

            if (outputFile.Contains(".lua"))
                File.WriteAllLines(outputFile,outputCode);
            else
                File.WriteAllLines(outputFile+".lua", outputCode);

        }

        /// <summary>
        /// Get all of the defined includes.  All include files should be
        /// between a #defineIncludes and #endDefine.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="codeData"></param>
        /// <returns></returns>
        static List<string> CollectIncludeFilenames(string page, List<string> codeData)
        {
            bool includeDefineFound = false;
            List<string> result = new List<string>();

            for (int i = propertiesEndDefinePos; i < codeData.Count; i++) //search for line with "#define includes"
            {
                if (codeData[i] == "--#defineIncludes")
                {
                    includeDefineFound = true;
                    includeDefinePos = i + 1;
                    break;
                }

            }

            if (includeDefineFound == false) //return as if there are no includes, compiler isn't needed
            {
                Console.WriteLine("No includes found, no code generated.");
                return result;
            }
           
            Console.WriteLine("Includes found, processing...");

            int index = includeDefinePos;

            while (codeData[index] != "--#endDefine") //fine line with the #end define
            {
                if (index > codeData.Count - 1)
                {
                    Console.WriteLine("Error, no end define found for includes.  Exiting.");
                    return result;
                }

                index++;
            }

            includeDefineEndPos = index; //set the end define position
            int numIncludes = includeDefineEndPos - includeDefinePos; //work out number of files included             

            for (index = includeDefinePos; index < includeDefinePos + numIncludes; index++)//get all the include filenames
            {               
                string includePath = StringUtils.SubString(codeData[index],"--#include ",".lua") + ".lua";
               result.Add(includePath);
            }

            Console.WriteLine(result.Count + " includes found in " + page);          

            
            return result;
        }

        static List<string> ProcessArguments(string[] args)
        {
            List<string> arguments = new List<string>(args);
            
            if (arguments.Count < 1)
            {
                Console.WriteLine("No arguments, specified.");
                return null;
            }
            else
            {
                return arguments;
            }
            
        }

        /// <summary>
        /// Reads entire file and puts lines in a string list.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static List<string> RetrieveCode(string path)
        {
            List<string> code = new List<string>();

            if(!File.Exists(path))
                Console.WriteLine("File not found");

            code.AddRange(File.ReadAllLines(path));     
            
            return code;
        }


        /// <summary>
        /// These are the properties the tic80 needs at the 
        /// start of every rom/game file.
        /// </summary>
        /// <param name="codeData"></param>
        /// <returns></returns>
        static bool RetrieveProperties(List<string> codeData)
        {
            if (codeData[propertiesDefinePos] != "--#define tic80Properties")
            {
                Console.WriteLine("Error line 0: No properties defined.");
                return false;
            }                
            else
            {
                int i = propertiesDefinePos + 1;
                while (codeData[i] != "--#endDefine")
                {
                    outputCode.Add(codeData[i]);
                    i++;
                }
                propertiesEndDefinePos = i;
                return true;
            }
        }
       
    }
}
