/******************************************************************************
 * File: ScenesToBuildSettingsHelper.cs
 * Copyright (c) 2021 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 * 
 * Confidential and Proprietary - Qualcomm Technologies, Inc.
 *
 ******************************************************************************/

#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class ScenesToBuildSettingsHelper : MonoBehaviour
{
    [MenuItem("Window/Snapdragon Spaces/Add Scenes to Build Settings")]
    public static void AddScenesToBuildSettings() {
        var packageName = "com.qualcomm.snapdragon.spaces";
        var packageID = "0.5.0";
        var samplesName = "Core Samples";
        /* Get the core sample. */
        var sample = Sample.FindByPackage(packageName, packageID).Single(sample => sample.displayName == samplesName);
        /* Get the samples import path but remove the Unity project path from it. Also remove the first index afterwards, which is a slash. */
        var samplePath = sample.importPath.Replace(Directory.GetParent(Application.dataPath).FullName, "").Remove(0, 1);
        
        var sampleScenes = AssetDatabase.FindAssets("t:Scene", new[] {samplePath});
        var editorBuildSettingsScenes = sampleScenes.Select(scene => new EditorBuildSettingsScene(AssetDatabase.GUIDToAssetPath(scene), true));
        /* Order list of scenes by path length, because the Main Menu scene will have the shortest one. */
        var orderedScenes = editorBuildSettingsScenes.OrderByDescending(scene => scene.path.Contains("Main Menu"));
        if (!orderedScenes.Any()) {
            Debug.Log("Can't find all sample scenes to add to the Editor Build settings.");
        } else {
            EditorBuildSettings.scenes = orderedScenes.ToArray();
            EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
        }
    }
}

#endif
