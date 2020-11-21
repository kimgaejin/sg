using UnityEngine;
using UnityEditor;
using PWCommon2;
using GeNa.Internal;
using System.IO;

namespace GeNa
{
    public class DirStructChecker
    {
        private const string PW_FOLDER_NAME = "/Procedural Worlds";
        static string incorrectPath = "";

        [InitializeOnLoadMethod]
        static void Onload()
        {
            // Need to wait for things to import before creating the common menu - Using delegates and only check menu when something gets imported
            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;

            AssetDatabase.importPackageCancelled -= OnImportPackageCancelled;
            AssetDatabase.importPackageCancelled += OnImportPackageCancelled;

            AssetDatabase.importPackageFailed -= OnImportPackageFailed;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;
        }

        /// <summary>
        /// Called when a package import is Completed.
        /// </summary>
        private static void OnImportPackageCompleted(string packageName)
        {
            OnPackageImport();
        }

        /// <summary>
        /// Called when a package import is Cancelled.
        /// </summary>
        private static void OnImportPackageCancelled(string packageName)
        {
            OnPackageImport();
        }

        /// <summary>
        /// Called when a package import fails.
        /// </summary>
        private static void OnImportPackageFailed(string packageName, string error)
        {
            OnPackageImport();
        }

        /// <summary>
        /// Used to run things after a package was imported.
        /// </summary>
        private static void OnPackageImport()
        {
            // No need for these anymore
            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
            AssetDatabase.importPackageCancelled -= OnImportPackageCancelled;
            AssetDatabase.importPackageFailed -= OnImportPackageFailed;

            Check(PWApp.CONF);
        }

        /// <summary>
        /// Checks the folder structure
        /// </summary>
        /// <param name="conf"></param>
        private static void Check(AppConfig conf)
        {
            string procWorldsPath;
            string oldStylePath = GetOldFolderPath(conf.Folder, out procWorldsPath);

            // No need to do this check again if no old style folder was found
            if (string.IsNullOrEmpty(oldStylePath))
            {
                SelfDestruct();
                return;
            }

            if (string.IsNullOrEmpty(procWorldsPath) == false)
            {
                string dialogText = string.Format("{0} {1} is now using Procedural Worlds' improved folder structure.\n\n" +
                    "Remnants of an older version of the product were found in the project. A clean upgrade is recommended.\n\n" +
                    "The path that triggered this warning is: '{3}'\n-----------------------------------------------------------------\n" +
                    "To do a clean upgrade, please follow these steps:\n\n" +
                    " 1. Save/back up any of your data that may be contained inside the {0} folder ({3}) (Note: it is not recommended to keep any of your own data in product folders).\n\n" +
                    " 2. Completely remove {0} from where it was originally installed ({3}) and also from the Procedural Worlds folder (Procedural Worlds/{2}).\n\n" +
                    " 3. Reimport the new {0} version.\n\n" +
                    "Note: It's best not to select/use any spawners until you have completed the clean upgrade.",
                    conf.Name, conf.Version, conf.Folder, incorrectPath);
                EditorUtility.DisplayDialog(conf.Name + " " + conf.Version + " - Warning!", dialogText, "Ok, I'll do a clean install");
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: Could not find the '{1}' folder.", conf.Name, PW_FOLDER_NAME);
            }
        }

        /// <summary>
        /// Get the path of the product folder.
        /// </summary>
        /// <param name="appConfig">Appconfig of the product.</param>
        /// <returns></returns>
        private static string GetOldFolderPath(string folderName, out string procWorldsPath)
        {
            procWorldsPath = null;
            bool prodFolderFound = false;

            foreach (var path in AssetDatabase.GetAllAssetPaths())
            {
                if (path.EndsWith(PW_FOLDER_NAME))
                {
                    procWorldsPath = path;
                }

                // If ends '/floderName' and contains the 'Scripts' folder
                if (path.EndsWith("/" + folderName) && Directory.Exists(Path.Combine(path, "Scripts")))
                {
                    prodFolderFound = true;

                    // Product not in the new folder structure and needs to be moved.
                    if (path.Contains(PW_FOLDER_NAME) == false)
                    {
                        incorrectPath = path;
                    }
                }
            }

            if (!prodFolderFound)
            {
                Debug.LogWarningFormat("[{0}]: Could not find the '{1}' folder.", PWApp.CONF.Name, folderName);
            }
            return incorrectPath;
        }

        /// <summary>
        /// Removes this script
        /// </summary>
        private static void SelfDestruct()
        {
            // No self destruction in the dev env
            if (Dev.Present)
            {
                return;
            }

            foreach (var path in AssetDatabase.GetAllAssetPaths())
            {
                // If found this script under this products folder
                if (path.EndsWith("DirStructChecker.cs") && path.Contains(PWApp.CONF.Folder))
                {
                    //Debug.LogFormat("This'd have removed '{0}'", path);
                    AssetDatabase.DeleteAsset(path);
                }
            }
        }
    }
}