using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DesignReporter
{
    class Program
    {
		static Dictionary<string, string> extensions = new Dictionary<string,string>();
		static string srcPath;
		static string srcPathDefault = "src\\";
		static DirectoryInfo src;
		static string docsPath;
		static string docsPathDefault = "docs\\";
		static string outputFilename = "sourcecode.tex";
		static DirectoryInfo docs;
        static FileInfo outputPath;
		static FileInfo preamblePath = new FileInfo(@"docs/library/preamble.tex");
		static FileInfo stylePath = new FileInfo(@"docs/library/style.tex");
		static FileInfo bibliographyPath = new FileInfo(@"docs/library/bibliography.bib");
		static String start =
			@"%!TEX program=xelatex+makeindex+bibtex
\documentclass{report}";

		static StreamWriter file;
		static long hardwareLines = 0;
		static long softwareLines = 0;

        static void Main(string[] args)
        {
			//Build dictionary
			extensions.Add(".cs", "csharp");
			extensions.Add(".c", "c");
			extensions.Add(".cpp", "c");
			extensions.Add(".h", "c");
			extensions.Add(".xaml", "xaml");
			extensions.Add(".vhdl", "vhdl");
			extensions.Add(".vhd", "vhdl");
			extensions.Add(".m", "matlab");

			//Get input
            Console.WriteLine("DesignReporter started.");
			Console.WriteLine("Paths are absolute or relative to current directory:");
			Console.WriteLine(Directory.GetCurrentDirectory());

			Console.WriteLine("Enter path to source code directory (defaults to \"{0}\")", srcPathDefault);
			srcPath = Console.ReadLine();
			if (String.IsNullOrWhiteSpace(srcPath))
				src = new DirectoryInfo(srcPathDefault);
			else
				src = new DirectoryInfo(srcPath);

			Console.WriteLine("Enter path to documents directory (defaults to \"{0}\")", docsPathDefault);
			docsPath = Console.ReadLine();
			if (String.IsNullOrWhiteSpace(docsPath))
				docs = new DirectoryInfo(docsPathDefault);
			else
				docs = new DirectoryInfo(docsPath);
			outputPath = new FileInfo(Path.Combine(new string[] {docs.FullName, outputFilename}));
			//Open file
			file = outputPath.CreateText();
			file.WriteLine(start);
			file.WriteLine("\\input{{{0}}}", GetRelativePath(docs.FullName,preamblePath.FullName));
			file.WriteLine("\\input{{{0}}}", GetRelativePath(docs.FullName, stylePath.FullName));
			file.WriteLine("\\addbibresource{{{0}}}", GetRelativePath(docs.FullName, bibliographyPath.FullName));
			file.WriteLine(@"\start{document}");
			file.WriteLine("\r\n");
			//Start processing
			processDirectory(src, 0);
			
			//Finish up
            file.WriteLine(@"\end{document}");
            file.Close();
			Console.WriteLine("Done. Press any key to exit.");
            Console.WriteLine("Hardware Code Lines: {0}\r\nSoftware Code Lines: {1}", hardwareLines, softwareLines);
			Console.ReadKey();
        }

		/// <summary>
		/// Recursively process all directories contained in the give root directory
		/// </summary>
		/// <param name="d">The directory to start ing</param>
		/// <param name="l">The current recursivity level</param>
		static void processDirectory(DirectoryInfo d, int l)
		{
			bool containsCode = false;
			string bookmarkLevel = "";
			string bookmarkName = "";
			string labelPrefix = "";

			//Enumerate all code files
			List<FileInfo> files = new List<FileInfo>();
			files.AddRange(d.GetFiles("*", SearchOption.TopDirectoryOnly));
			files.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.FullName, y.FullName));
			IEnumerable<FileInfo> fileQuery =
			from fileQ in files
			where !fileQ.FullName.Contains("obj") && !fileQ.FullName.Contains("Visual Micro") && !fileQ.FullName.Contains("Properties") && !fileQ.FullName.Contains("quartus") && !fileQ.FullName.Contains("DesignReporter") && !fileQ.FullName.Contains("modules") && extensions.ContainsKey(fileQ.Extension)
			select fileQ;

			if (fileQuery.Count() > 0)
			{
				containsCode = true;
				if (l == 0) //currently in root of project
				{
					bookmarkLevel = "section";
					bookmarkName = d.Name;
					labelPrefix = "appsec";
				}
				else if (l == 1)
				{
					bookmarkLevel = "subsection";
					bookmarkName = d.Parent.Name + " - " + d.Name;
					labelPrefix = "appsubsec";
				}
				else
				{
					bookmarkLevel = "subsubsection";
					bookmarkName = d.Parent.Parent.Name + " - " + d.Parent.Name + " - " + d.Name;
					labelPrefix = "appsubsubsec";
				}

				//Add new section
				file.WriteLine(String.Format("\\{0}{{{1}}}", bookmarkLevel, bookmarkName.Replace("_", @"\_")));
				file.WriteLine(String.Format("\\label{{{0}:{1}}}", labelPrefix, bookmarkName.Replace(" ", String.Empty).Replace("_", "-")));

				foreach (FileInfo fi in fileQuery)
				{
					string ext = fi.Extension;
					string caption = fi.Directory.Name + '/' + fi.Name;
					caption = caption.Replace("_", "-");
					string filename = fi.Name;
					filename = filename.Replace("_", "-");
					//string pathescaped = fi.FullName.Replace(outputPath.Directory.FullName + "\\", "").Replace('\\', '/');
					string pathescaped = GetRelativePath(outputPath.Directory.FullName,fi.FullName);

					//Add source code to file
					file.WriteLine(String.Format("\\includecode[{1}]{{{2}}}{{{3}}}{{lst:{0}}}\r\n", caption.Replace('/', '-'), extensions[ext], caption, pathescaped));
					
					//Count lines
					if (ext == ".vhdl" || ext == ".vhd")
					{
						long lines = CountLinesInFile(fi.FullName);
						hardwareLines += lines;
						Console.WriteLine("Lines: {0} --> file: {1}", lines, fi.Name);
					}
					else
					{
						long lines = CountLinesInFile(fi.FullName);
						softwareLines += lines;
						Console.WriteLine("Lines: {0} --> file: {1}", lines, fi.Name);
					}
				}
			}

			//Enumerate directories
			DirectoryInfo[] directories = d.GetDirectories();

			foreach (DirectoryInfo directory in directories)
			{
				//Process directories in current directory
				if (containsCode)
					processDirectory(directory, l + 1); //increase recursivity level
				else
					processDirectory(directory, l);
			}
		}

		/// <summary>
		/// Count the number of lines in the file specified.
		/// </summary>
		/// <param name="f">The filename to count lines.</param>
		/// <returns>The number of lines in the file.</returns>
		static long CountLinesInFile(string f)
		{
			long count = 0;
			using (StreamReader r = new StreamReader(f))
			{
				string line;
				while (!r.EndOfStream)
				{
					line = r.ReadLine();
					line = line.Trim();
					if (line != null && line != String.Empty)
						count++;
				}
			}
			return count;
		}

		public static string GetRelativePath(string fromPath, string toPath)
		{
			int fromAttr = GetPathAttribute(fromPath);
			int toAttr = GetPathAttribute(toPath);

			StringBuilder path = new StringBuilder(260); // MAX_PATH
			if (PathRelativePathTo(
				path,
				fromPath,
				fromAttr,
				toPath,
				toAttr) == 0)
			{
				throw new ArgumentException("Paths must have a common prefix");
			}
			return path.ToString().Replace("\\","/");
		}

		private static int GetPathAttribute(string path)
		{
			DirectoryInfo di = new DirectoryInfo(path);
			if (di.Exists)
			{
				return FILE_ATTRIBUTE_DIRECTORY;
			}

			FileInfo fi = new FileInfo(path);
			if (fi.Exists)
			{
				return FILE_ATTRIBUTE_NORMAL;
			}

			throw new FileNotFoundException();
		}

		private const int FILE_ATTRIBUTE_DIRECTORY = 0x10;
		private const int FILE_ATTRIBUTE_NORMAL = 0x80;

		[DllImport("shlwapi.dll", SetLastError = true)]
		private static extern int PathRelativePathTo(StringBuilder pszPath,
			string pszFrom, int dwAttrFrom, string pszTo, int dwAttrTo);
    }
}
