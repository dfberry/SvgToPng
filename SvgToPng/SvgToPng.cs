namespace SvgToPngLibrary
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// http://harriyott.com/2008/05/converting-svg-images-to-png-in-c.aspx
    /// </summary>
    public class SvgToPng
    {
        private string CmdLineArgs {get;set;}

        /// <summary>
        /// Use default config file
        /// </summary>
        public string ExeLocation {get;set;}

        /// <summary>
        /// Use specified config file
        /// </summary>
        /// <param name="nonDefaultConfigFileName"></param>
        public SvgToPng(string nonDefaultConfigFileName)
        {
            // find exe location from config file
            this.ExeLocation = this.GetInkScapeExeLocationFromConfigurationFile(nonDefaultConfigFileName);

            if (!System.IO.File.Exists(ExeLocation))
            {
                throw new Exception("InkscapeCmdLine from config file is empty. exeLocationFromConfigFile=" + this.ExeLocation);
            }
        }

        /// <summary>
        /// Create Png file with same prefix and png extension in same folder.
        /// Calls Inkscape command line so this is huge performance hit. 
        /// Eventually move this processing to background task.
        /// </summary>
        /// <param name="fileAndPathToSvg"></param>
        /// <returns>fileAndPath of new PNG</returns>
        public string Convert(string fileAndPathToSvg, string newFileAndPathToPng)
        {
            if (!System.IO.File.Exists(fileAndPathToSvg))
            {
                throw new Exception("fileAndPathToSvg file doesn't exist: " + fileAndPathToSvg);
            }

            if (!System.IO.File.Exists(this.ExeLocation))
            {
                throw new Exception("exeLocationFromConfigFile file doesn't exist: " + this.ExeLocation);
            }

            // figure out final file name
            newFileAndPathToPng = string.IsNullOrEmpty(newFileAndPathToPng) ? fileAndPathToSvg.Replace("svg", "png") : newFileAndPathToPng;

            if (System.IO.File.Exists(newFileAndPathToPng))
            {
                throw new Exception("newFileAndPathToPng file already exists: " + newFileAndPathToPng);
            }

            // build InkScape command line 
            this.CmdLineArgs = "-f " + fileAndPathToSvg + " -e " + newFileAndPathToPng;

            // Spawn thread to control InkScape process via command line
            CommandLineThread(this.ExeLocation, this.CmdLineArgs);

            if (!System.IO.File.Exists(newFileAndPathToPng))
            {
                throw new Exception("newFileAndPathToPng - final fine not found. May indicate original svg file is invalid format. Location =" + newFileAndPathToPng);
            }

            return (newFileAndPathToPng);

        }
        /// <summary>
        /// Set Inkscape command line arguements.
        /// </summary>
        /// <param name="oldFile"></param>
        /// <param name="newFile"></param>
        /// <returns></returns>
        public string CommandLineArguments(string oldFile, string newFile)
        {
            // Full list of export options is available at
            // http://tavmjong.free.fr/INKSCAPE/MANUAL/html/CommandLine-Export.html
            //inkscape.StartInfo.Arguments =
            //String.Format("--file \"{0}\" --export-{1} \"{2}\" --export-width {3} --export-height {4}",
            //              svgFile, extension, outFile, WIDTH, HEIGHT);

            if (string.IsNullOrEmpty(oldFile))
            {
                throw new ArgumentNullException("oldFile is empty");
            }

            if (string.IsNullOrEmpty(newFile))
            {
                throw new ArgumentNullException("oldFile is empty");
            }

            return "-f " + oldFile + " -e " + newFile;
        }

        /// <summary>
        /// Creates process and starts shell to execute Inkscape command line.
        /// "inkscape pie.svg -e -pie.png
        /// </summary>
        /// <param name="cmdLineArgs"></param>
        private void CommandLineThread(string inkscapeCmdLine, string cmdLineArgs)
        {
            if ((string.IsNullOrEmpty(inkscapeCmdLine)) || (!inkscapeCmdLine.Contains(".exe")))
            {
                throw new Exception("command line empty or doesn't contain exe: " + inkscapeCmdLine);
            }

            Process inkscape = new Process();

            inkscape.StartInfo.FileName = inkscapeCmdLine;
            inkscape.StartInfo.Arguments = cmdLineArgs;
            inkscape.StartInfo.UseShellExecute = false;
            inkscape.Start();

            inkscape.WaitForExit();
        }
        /// <summary>
        /// Add system.configuration reference to project
        /// Get Inkscape location from web.config
        /// </summary>
        /// <param name="nonDefaultConfigFileName"></param>
        /// <returns></returns>
        private string GetInkScapeExeLocationFromConfigurationFile(string nonDefaultConfigFileName)
        {
            return  ConfigurationManager.AppSettings["ExeDirectoryForInkscape"];
        }
        public string Save(string svgData_HttpUtilityUrlDecode, string filePrefix, string fileNamePostpend, string serverMapPath)
        {
            if (string.IsNullOrEmpty(svgData_HttpUtilityUrlDecode))
            {
                throw new ArgumentNullException("svgData is empty");
            }

            if (string.IsNullOrEmpty(filePrefix))
            {
                throw new ArgumentNullException("sessionId is empty");
            }

            if (string.IsNullOrEmpty(fileNamePostpend))
            {
                throw new ArgumentNullException("fileNamePostpend is empty");
            }

            if (string.IsNullOrEmpty(serverMapPath))
            {
                throw new ArgumentNullException("serverMapPath is empty");
            }

            // complete file name
            string fileNameWithoutExtension = filePrefix + "-" + fileNamePostpend;

            string svgFileLocation = Path.Combine(serverMapPath, fileNameWithoutExtension + ".svg");

            // get rid of previous svg file
            if (System.IO.File.Exists(svgFileLocation))
            {
                System.IO.File.Delete(svgFileLocation);
            }

            // get rid of previous png file
            if (System.IO.File.Exists(svgFileLocation.Replace("svg", "png")))
            {
                System.IO.File.Delete(svgFileLocation.Replace("svg", "png"));
            }

            // write svg data to file
            System.IO.File.WriteAllText(svgFileLocation, svgData_HttpUtilityUrlDecode);

            if (!System.IO.File.Exists(svgFileLocation))
            {
                throw new Exception("svg file doesn't exist - " + svgFileLocation);
            }

            // convert svg to png
            string pngLocation = this.Convert(svgFileLocation, null);

            if (!System.IO.File.Exists(pngLocation))
            {
                throw new Exception("png file doesn't exist - " + pngLocation);
            }

            // get rid of svg
            if (System.IO.File.Exists(svgFileLocation))
            {
                System.IO.File.Delete(svgFileLocation);
            }

            return pngLocation;
        }
    }
}
