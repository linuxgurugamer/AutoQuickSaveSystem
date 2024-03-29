﻿// just uncomment this line to restrict file access to KSP installation folder
#define _UNLIMITED_FILE_ACCESS
// for debugging
// #define _DEBUG

using System;
using System.IO;
using UnityEngine;

using static AutoQuickSaveSystem.AutoQuickSaveSystem;

namespace AutoQuickSaveSystem
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class FileOperations : MonoBehaviour
    {

        private static String ROOT_PATH;
        //private static String CONFIG_BASE_FOLDER;

        protected void Start()
        {
            ROOT_PATH = KSPUtil.ApplicationRootPath;
            //CONFIG_BASE_FOLDER = ROOT_PATH + "GameData/";
            FilePath = "PluginData/AutoQuickSaveSystem.cfg";
        }
        public static bool InsideApplicationRootPath(String path)
        {
            if (path == null) return false;
            try
            {
                String fullpath = Path.GetFullPath(path);
                return fullpath.StartsWith(Path.GetFullPath(ROOT_PATH));
            }
            catch
            {
                return false;
            }
        }

        public static bool ValidPathForWriteOperation(String path)
        {
#if (_UNLIMITED_FILE_ACCESS)
            return true;
#else
            String fullpath = Path.GetFullPath(path);
            return InsideApplicationRootPath(fullpath);
#endif
        }

        private static void CheckPathForWriteOperation(String path)
        {
            if (!ValidPathForWriteOperation(path))
            {
                Log.Error("invalid write path: " + path);
                throw new InvalidOperationException("write path outside KSP home folder: " + path);
            }
        }


        public static void DeleteFile(String file)
        {
            CheckPathForWriteOperation(file);
            Log.Info("deleting file " + file);
            File.Delete(file);
        }

        public static void CopyFile(String from, String to)
        {
            CheckPathForWriteOperation(to);
            Log.Info("copy file " + from + " to " + to);
            File.Copy(from, to);
        }

        public static void CopyDirectory(String from, String to, String excludemarkerfile = ".nobackup")
        {
            if (FileExists(from + "/" + excludemarkerfile))
            {
                Log.Info("directory '" + from + "' excluded from backup (marked by file)");
                return;
            }
            Log.Detail("no exclude marker file '" + excludemarkerfile + "' found in folder '" + from + "'");

            string dirName = new DirectoryInfo(from).Name;
            foreach (var e in ConfigNodeIO.excludes)
            {
                if (dirName == e)
                {
                    Log.Info("directory '" + dirName + "' excluded from backup (excluded by config)");
                    return;
                }
            }
            Log.Detail("folder '" + from + "' not in exclude list");

            CheckPathForWriteOperation(to);

            Log.Info("copy directory " + from + " to " + to);

            // create target directory if not existient
            if (!DirectoryExists(to))
            {
                CreateDirectoryRetry(to);
            }

            String[] files = GetFiles(from);
            foreach (String file in files)
            {
                String name = GetFileName(file);
                CopyFileRetry(file, to + "/" + name);
            }
            String[] folders = GetDirectories(from);
            foreach (String folder in folders)
            {
                String name = GetFileName(folder);
                CreateDirectoryRetry(to + "/" + name);
                CopyDirectory(folder, to + "/" + name);
            }
        }

        public static void CreateDirectory(String directory)
        {
            CheckPathForWriteOperation(directory);
            Log.Info("creating directory " + directory);
            Directory.CreateDirectory(directory);
        }

        public static void DeleteDirectory(String directory)
        {
            CheckPathForWriteOperation(directory);
            Log.Info("deleting directory " + directory);
            Directory.Delete(directory, true);
        }

        public static void CreateFile(String file)
        {
            CheckPathForWriteOperation(file);
            Log.Info("creating file " + file);
            File.Create(file);
        }

        public static void CreateDirectoryRetry(String directory, int retries = 3, int delayinMillis = 500)
        {
            do
            {
                try
                {
                    CreateDirectory(directory);
                    return;
                }
                catch (Exception e)
                {
                    Log.Exception("CreateDirectoryRetry", e);
                    if (retries > 0)
                    {
                        retries--;
                        Log.Info("retrying operation: create directory in " + delayinMillis + " ms");
                        //Thread.Sleep(delayinMillis);
                    }
                    else
                    {
                        throw e;
                    }
                }
            } while (retries > 0);
        }

        public static void CopyFileRetry(String from, String to, int retries = 6, int delayinMillis = 200)
        {
            do
            {
                try
                {
                    CopyFile(from, to);
                    return;
                }
                catch (Exception e)
                {
                    Log.Exception("CopyFileRetry", e);
                    if (retries > 0)
                    {
                        retries--;
                        Log.Info("retrying operation: copy file in " + delayinMillis + " ms");
                        //Thread.Sleep(delayinMillis);
                    }
                    else
                    {
                        throw e;
                    }
                }
            } while (retries > 0);
        }

        public static void DeleteFileRetry(String file, int retries = 6, int delayinMillis = 100)
        {
            do
            {
                try
                {
                    DeleteFile(file);
                    return;
                }
                catch (Exception e)
                {
                    Log.Exception("DeleteFileRetry", e);
                    if (retries > 0)
                    {
                        retries--;
                        Log.Info("retrying operation: delete file in " + delayinMillis + " ms");
                        //Thread.Sleep(delayinMillis);
                    }
                    else
                    {
                        throw e;
                    }
                }
            } while (retries > 0);
        }

        public static void DeleteDirectoryRetry(String directory, int retries = 6, int delayinMillis = 100)
        {
            do
            {
                try
                {
                    DeleteDirectory(directory);
                    return;
                }
                catch (Exception e)
                {
                    Log.Exception("DeleteDirectoryRetry", e);
                    if (retries > 0)
                    {
                        retries--;
                        Log.Info("retrying operation: delete directory in " + delayinMillis + " ms");
                        //Thread.Sleep(delayinMillis);
                    }
                    else
                    {
                        throw e;
                    }
                }
            } while (retries > 0);
        }


        public static String[] GetDirectories(String path)
        {
            return Directory.GetDirectories(path);
        }

        public static String[] GetFiles(String path)
        {
            return Directory.GetFiles(path);
        }

        public static bool FileExists(String file)
        {
            return File.Exists(file);
        }

        public static bool DirectoryExists(String file)
        {
            return Directory.Exists(file);
        }

        public static String GetFileName(String path)
        {
            return Path.GetFileName(path);
        }

        public static String ExpandBackupPath(String path)
        {
            if (path == null) return KSPUtil.ApplicationRootPath;
            path = path.Trim();
            if (path.StartsWith("./") || path.StartsWith(".\\"))
            {
                path = KSPUtil.ApplicationRootPath + path.Substring(2);
            }
            return path;
        }

#if (_DEBUG)
         /**
          * Used for debugging purposes only
          */
         public static void AppendText(String filename, String text)
         {
            using (StreamWriter sw = File.AppendText(filename))
            {
               sw.WriteLine(text);
               sw.Flush();
            }
         }
#endif





        internal static String AssemblyLocation
        { get { return System.Reflection.Assembly.GetExecutingAssembly().Location; } }


        internal static String AssemblyFolder
        { get { return System.IO.Path.GetDirectoryName(AssemblyLocation); } }

        private static String _FilePath;
        /// <summary>
        /// Location of file for saving and loading methods
        ///
        /// This can be an absolute path (eg c:\test.cfg) or a relative path from the location of the assembly dll (eg. ../config/test)
        /// </summary>
        public static String FilePath
        {
            get { return _FilePath; }
            set
            {
                //Combine the Location of the assembly and the provided string. This means we can use relative or absolute paths
                _FilePath = System.IO.Path.Combine(AssemblyFolder + "/../PluginData/", value).Replace("\\", "/");
            }
        }
        const string NODENAME = "AQSS";

        public static void SaveConfiguration( String file)
        {
            FilePath = file;

            ConfigNode f = new ConfigNode();
            ConfigNode node = new ConfigNode(NODENAME);
            node.AddValue("logLevel", ((int)Configuration.LogLevel).ToString());

            node.AddValue("quicksaveOnLaunch", Configuration.QuicksaveOnLaunch);
            node.AddValue("quicksaveOnSceneChange", Configuration.QuicksaveOnSceneChange);


            node.AddValue("quicksaveInterval", (int)Configuration.QuicksaveInterval);
            Log.Info("SaveConfiguration, Configuration.QuicksaveInterval: " + Configuration.QuicksaveInterval);
            node.AddValue("quickSaveNameTemplate", Configuration.QuickSaveNameTemplate);
            node.AddValue("launchNameTemplate", Configuration.LaunchNameTemplate);
            node.AddValue("sceneSaveNameTemplate", Configuration.SceneSaveNameTemplate);

            node.AddValue("customQuicksaveInterval", Configuration.CustomQuicksaveInterval);

            
            node.AddValue("minTimeBetweenQuicksaves", Configuration.MinTimeBetweenQuicksaves);

            node.AddValue("daysToKeepQuicksaves", Configuration.DaysToKeepQuicksaves);
            node.AddValue("minNumberOfQuicksaves", Configuration.MinNumberOfQuicksaves);
            node.AddValue("maxNumberOfQuicksaves", Configuration.MaxNumberOfQuicksaves);
            node.AddValue("maxNumberOfLaunchsaves", Configuration.MaxNumberOfLaunchsaves);
            node.AddValue("maxNumberOfScenesaves", Configuration.MaxNumberOfScenesaves);

            node.AddValue("saveVesselInEditor", Configuration.saveVesselInEditor);
            node.AddValue("editorTimeIntervalToSave", Configuration.editorTimeIntervalToSave);

            node.AddValue("soundOnSave", Configuration.SoundOnSave);
            node.AddValue("soundLocation", Configuration.SoundLocation);
            node.AddValue("minimumTimeBetweenSounds", Configuration.MinimumTimeBetweenSounds);

            f.AddNode(node);
            f.Save(FilePath);
        }

        public static void LoadConfiguration(String file)
        {
            FilePath = file;
            if (File.Exists(FilePath))
            {
                ConfigNode f = ConfigNode.Load(FilePath);
                ConfigNode node = f.GetNode(NODENAME);
                if (node != null)
                {
                    Configuration.Init();
                    Configuration.LogLevel = (KSP_Log.Log.LEVEL)int.Parse(SafeLoad(node, "logLevel", (int)Configuration.LogLevel));
                    Configuration.QuicksaveOnLaunch = bool.Parse(SafeLoad(node, "quicksaveOnLaunch", Configuration.QuicksaveOnLaunch));
                    Configuration.QuicksaveOnSceneChange = bool.Parse(SafeLoad(node, "quicksaveOnSceneChange", Configuration.QuicksaveOnSceneChange));

                    

                    Configuration.QuicksaveInterval = (Configuration.QuickSave_Interval)int.Parse(SafeLoad(node, "quicksaveInterval", (int)Configuration.QuicksaveInterval));
                    Log.Info("LoadConfiguration 1, Configuration.QuicksaveInterval: " + Configuration.QuicksaveInterval);
                    Configuration.LaunchNameTemplate = SafeLoad(node, "quickSaveNameTemplate", Configuration.QuickSaveNameTemplate);

                    Configuration.LaunchNameTemplate = SafeLoad(node, "launchNameTemplate", Configuration.LaunchNameTemplate);
                    Configuration.SceneSaveNameTemplate = SafeLoad(node, "sceneSaveNameTemplate", Configuration.SceneSaveNameTemplate);

                    Configuration.CustomQuicksaveInterval= int.Parse(SafeLoad(node, "customQuicksaveInterval", (int)Configuration.CustomQuicksaveInterval));

                    Configuration.MinTimeBetweenQuicksaves = int.Parse(SafeLoad(node, "minTimeBetweenQuicksaves", Configuration.MinTimeBetweenQuicksaves));

                    Configuration.DaysToKeepQuicksaves = int.Parse(SafeLoad(node, "daysToKeepQuicksaves", Configuration.DaysToKeepQuicksaves));
                    Configuration.MinNumberOfQuicksaves = int.Parse(SafeLoad(node, "minNumberOfQuicksaves", Configuration.MinNumberOfQuicksaves));
                    Configuration.MaxNumberOfQuicksaves = int.Parse(SafeLoad(node, "maxNumberOfQuicksaves", Configuration.MaxNumberOfQuicksaves));

                    Configuration.MaxNumberOfLaunchsaves = int.Parse(SafeLoad(node, "maxNumberOfLaunchsaves", Configuration.MaxNumberOfLaunchsaves));
                    Configuration.MaxNumberOfScenesaves = int.Parse(SafeLoad(node, "maxNumberOfScenesaves", Configuration.MaxNumberOfScenesaves));

                    Configuration.saveVesselInEditor = bool.Parse(SafeLoad(node, "saveVesselInEditor", Configuration.saveVesselInEditor));
                    Configuration.editorTimeIntervalToSave = int.Parse(SafeLoad(node, "editorTimeIntervalToSave", Configuration.editorTimeIntervalToSave));

                    Configuration.SoundOnSave = bool.Parse(SafeLoad(node, "soundOnSave", Configuration.SoundOnSave));
                    Configuration.SoundLocation = SafeLoad(node, "soundLocation", Configuration.SoundLocation);
                    Configuration.MinimumTimeBetweenSounds = int.Parse(SafeLoad(node, "minimumTimeBetweenSounds ", Configuration.MinimumTimeBetweenSounds));
                }
                else
                {
                    Log.Info("no config file: default Configuration");
                }
            }
        }

        static string SafeLoad(ConfigNode node, string name, string oldvalue)
        {
            string value = node.GetValue(name);
            if (value == null)
                return oldvalue;
            return value;
        }

        static string SafeLoad(ConfigNode node, string name, int oldvalue)
        {
            string value = node.GetValue(name);
            if (value == null)
                return oldvalue.ToString();
            return value;
        }
        static string SafeLoad(ConfigNode node, string name, bool oldvalue)
        {
            string value = node.GetValue(name);
            if (value == null)
                return oldvalue.ToString();
            return value;
        }

    }

}
