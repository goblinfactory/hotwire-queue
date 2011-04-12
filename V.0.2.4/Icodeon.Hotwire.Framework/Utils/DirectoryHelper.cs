﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Utils
{
    public static class DirectoryHelper
    {
        public static IEnumerable<string> SortByDateAscending(this IEnumerable<string> filePaths)
        {
            var orderedFiles = filePaths.Select( fp => new  { DateTime =new  FileInfo(fp).CreationTime, File = fp }).OrderBy( o=> o.DateTime).Select( s=> s.File);
            return orderedFiles.ToList();
        }

        public static string ProjectRootFolder
        {
            get
            {
                // current directory is either bin/debug or bin/release folder or, if not running from visual studio it is the current direct 
                // that the exe is running in.
                var dir = Environment.CurrentDirectory;
                int lastPos = dir.LastIndexOf(@"/bin/") + dir.LastIndexOf(@"\bin\") + 1;
                if (lastPos<1)
                {
                    // not running in project environment, probably running as a utility on the server in production or live testing
                    return Environment.CurrentDirectory;
                }
                var projectRootFolder = dir.Remove(lastPos);
                return projectRootFolder;
            }
        }

        public static bool ContainsErrorFile(this IEnumerable<string> filePaths, string fileNoPath)
        {
            string errorExtentionedFile = EnqueueRequestDTO.AddErrorExtension(fileNoPath);
            string skippedExtensionedFile = EnqueueRequestDTO.AddSkippedExtension(fileNoPath);
            return filePaths.Any(fp => fp.EndsWith(errorExtentionedFile) || fp.EndsWith(skippedExtensionedFile));
        }


        public static bool ContainsFile(this IEnumerable<string> filePaths, string fileNoPath)
        {
            return filePaths.Any(fp=> fp.EndsWith(fileNoPath));
        }

        public static string GetSolutionRootPath(string solutionFolderMarkerFile)
        {
            return GetSolutionRoot(solutionFolderMarkerFile).FullName;
        }

        private static DirectoryInfo _solutionRoot;
        public static DirectoryInfo GetSolutionRoot(string solutionFolderMarkerFile)
        {
            if (_solutionRoot != null) return _solutionRoot;
            // running under IIS or Console exe?

            DirectoryInfo di = null;
            if (HttpContext.Current==null)
            {
                di = new DirectoryInfo(Environment.CurrentDirectory);    
            }
            else
            {
                // current directory 
                di = new DirectoryInfo(HttpContext.Current.Server.MapPath("~"));
            }
            
            while(di!=null)
            {
                if (File.Exists(Path.Combine(di.FullName, solutionFolderMarkerFile)))
                {
                    _solutionRoot = di;
                    return _solutionRoot;
                }
                // not found, so go up a folder and try again
                di = di.Parent;
            }
            throw new DirectoryNotFoundException("Could not find the solution Root. Hotwire requires that all console apps run as a subfolder of some root folder where relative paths to folders will resolve correctly.");
        }

        public static string SolutionRootFolder
        {
            get
            {
                var solutionRootFolder = new DirectoryInfo(ProjectRootFolder).Parent.FullName;
                return solutionRootFolder;
            }
        }

        private static IEnumerable<string> ExcludeSVN(this IEnumerable<string> src)
        {
            return src.Where(s => !s.EndsWith(".svn"));
        }

        public static void DeleteAllFilesExceptMarker(string folderPath, string markerFile)
        {
            // CHECK
            string markerFilePath = Path.Combine(folderPath, markerFile);
            if (!Directory.Exists(folderPath)) throw new DirectoryNotFoundException("Directory '" + folderPath + "' not found.");
            if (!File.Exists(markerFilePath)) throw new FileNotFoundException("File '" + markerFilePath + "' not found.");
            var firstsubdir = Directory.EnumerateDirectories(folderPath).ExcludeSVN().FirstOrDefault();
            if (firstsubdir != null) throw new ArgumentException("Directory '" + folderPath + "' is not empty. It contains subfolders. For safety, to prevent accidental deltion of operating system directories and files, deleting of files is limited to folders without child folders and folder must contain marker file. It's possible the path is invalid.");
            // DELETE
            Directory.EnumerateFiles(folderPath).Where( f=> !f.EndsWith(markerFile)).ToList().ForEach(File.Delete);
        }


        /// <summary>
        /// returns the physical path to the subfolder at the project level, and checks that a marker file exists. (use backslash for folder seperators)
        /// </summary>
        /// <param name="subfolder"></param>
        /// <param name="markerFile"></param>
        /// <returns></returns>
        public static string MapProjectSubfolder(string subfolder, string markerFile)
        {
            string newFolder = Path.Combine(ProjectRootFolder, subfolder);
            string markerFilePath = Path.Combine(newFolder, markerFile);
            if (!Directory.Exists(newFolder)) throw new DirectoryNotFoundException("Directory '" + newFolder + "' not found.");
            if (!File.Exists(markerFilePath)) throw new FileNotFoundException("File '" + markerFilePath + "' not found.");
            return newFolder;
        }

        /// <summary>
        /// returns the physical path to the subfolder at the project level (use backslash for folder seperators)
        /// </summary>
        public static string MapPath(string subfolder)
        {
            string newPath = Path.Combine(ProjectRootFolder, subfolder);
            if (!File.Exists(newPath)) throw new FileNotFoundException("File '" + newPath + "' not found.");
            return newPath;
        }


        public static string MapSubFolderPathFromSolutionRoot(string subfolder, string solutionFolderMarkerFile)
        {
            string fullPath = Path.Combine(GetSolutionRootPath(solutionFolderMarkerFile), subfolder);
            if (!Directory.Exists(fullPath)) throw new DirectoryNotFoundException(fullPath);
            return fullPath;
        }


    } 

    public enum RootStart
    {
        SolutionRoot,
        ProjectRoot
    }

}
