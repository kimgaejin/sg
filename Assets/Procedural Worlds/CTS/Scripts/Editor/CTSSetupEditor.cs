﻿using System;
using UnityEditor;
using UnityEngine;
using PWCommon4;

namespace CTS
{
    public class CTSSetupEditor : Editor
    {
        [InitializeOnLoadMethod]
        static void Onload()
        {
            AppConfig conf = AssetUtils.GetConfig(Internal.PWApp.CONF_NAME, true);
            if (conf != null)
            {
                if (conf.Folder != null)
                {
                    Check(conf);
                }
            }
            else
            {
                // Need to wait for things to import before creating the common menu - Using delegates and only check menu when something gets imported
                AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
                AssetDatabase.importPackageCompleted += OnImportPackageCompleted;

                AssetDatabase.importPackageCancelled -= OnImportPackageCancelled;
                AssetDatabase.importPackageCancelled += OnImportPackageCancelled;

                AssetDatabase.importPackageFailed -= OnImportPackageFailed;
                AssetDatabase.importPackageFailed += OnImportPackageFailed;
            }
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
            //Reset the flag for the render pipeline message, could be set to true from an older project which would block the message then
            EditorPrefs.SetBool("CTS_RenderPipelineMessageShown", false);

            Check(Internal.PWApp.CONF);



            // No need for these anymore
            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
            AssetDatabase.importPackageCancelled -= OnImportPackageCancelled;
            AssetDatabase.importPackageFailed -= OnImportPackageFailed;
        }

        /// <summary>
        /// Checks the folder structure
        /// </summary>
        /// <param name="conf"></param>
        private static void Check(AppConfig conf)
        {
            string prefString = "PWShowNewsCTS";
            if (!EditorPrefs.HasKey(prefString))
            {
                if (EditorUtility.DisplayDialog("Install / Update detected", "It looks like you are installing CTS for the first time. Would you like CTS to display news and notify you when new versions are published? It will fetch the latest news and version information once per day from the Procedural Worlds website. This feature can be turned on and off in the CTS Component / CTS Profile anytime.", "Yes, allow News", "No thanks"))
                {
                    EditorPrefs.SetBool(prefString, true);
                }
                else
                {
                    EditorPrefs.SetBool(prefString, false);
                }
            }

            //Switching Autoupdate on initially after each load
            EditorPrefs.SetBool("CTS_AutoUpdateShaders", true);

            EditorApplication.update += CTSSetup.UpdatePipelineDefines;
            CTSSetup.UpdatePipelineDefines();

            if (conf != null)
            {
                int majorVersion, minorVersion, patchVersion = 0;
                bool parseFailed = false;

                if (!Int32.TryParse(CTS.Internal.PWApp.CONF.MajorVersion, out majorVersion))
                {
                    parseFailed = true;
                    Debug.LogWarning("Error when reading the CTS major version number!");
                }
                if (!Int32.TryParse(CTS.Internal.PWApp.CONF.MinorVersion, out minorVersion))
                {
                    parseFailed = true;
                    Debug.LogWarning("Error when reading the CTS minor version number!");
                }
                if (!Int32.TryParse(CTS.Internal.PWApp.CONF.PatchVersion, out patchVersion))
                {
                    parseFailed = true;
                    Debug.LogWarning("Error when reading the CTS patch version number!");
                }

                if (!parseFailed)
                {
                    if (majorVersion != CTSConstants.MajorVersion ||
                       minorVersion != CTSConstants.MinorVersion ||
                       patchVersion != CTSConstants.PatchVersion)
                    {
                        Debug.LogError("Version Mismatch between app config and CTS constants!");
                    }

                }
            }
        }
    }
}