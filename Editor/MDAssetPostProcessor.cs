#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Editor
{
    public class MDAssetPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            var assetsToRelink = new HashSet<MDScriptCollectionAsset>();

            foreach (var assetPath in importedAssets.Where(a => a.EndsWith(".dlg.md", StringComparison.OrdinalIgnoreCase)))
            {
                assetsToRelink.Add(AssetDatabase.LoadAssetAtPath<MDScriptCollectionAsset>(assetPath));
            }

            foreach (var assetPath in deletedAssets.Where(a => a.EndsWith(".dlg.md", StringComparison.OrdinalIgnoreCase)))
            {
                foreach (var depPath in AssetDatabase.GetDependencies(assetPath).Where(a => a.EndsWith(".dlg.md", StringComparison.OrdinalIgnoreCase)))
                {
                    assetsToRelink.Add(AssetDatabase.LoadAssetAtPath<MDScriptCollectionAsset>(depPath));
                }
            }

            // Need to do this? 
            /*
            for (int i = 0; i < movedAssets.Length; i++)
            {
                if (!movedAssets[i].EndsWith(".dlg.md", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            }
            */

            foreach (var collection in assetsToRelink)
            {
                Debug.Log($"Need to relink {collection.Scripts.Count} scripts from {collection.AssetPath}");

                foreach (var script in collection.Scripts)
                {
                    RelinkScript(collection, script);
                }
            }
        }

        private static void RelinkScript(MDScriptCollectionAsset collection, MDScriptAsset script)
        {
            foreach (var linkLine in script.Lines.Where(l => l is MDLink).Cast<MDLink>())
            {
                if (linkLine.TargetScript[0] == '#') // Is an internal link
                {
                    var newScript = collection.GetDialogueScript(linkLine.TargetScript.Substring(1));
                    if (newScript == null)
                    {
                        Debug.LogError($"Could not resolve internal link in script {script.AssetPath} to {linkLine.TargetScript}");
                    }
                    else if (!script.LinkedToScripts.Contains(newScript))
                    {
                        script.LinkedToScripts.Add(newScript);
                        Debug.Log($"Linked {script.AssetPath} to internal script {newScript.AssetPath}");
                    }
                }
                else
                {
                    var targetCollectionFullPath = collection.ResolveRelativePathTo(linkLine.TargetScript);
                    var (targetCollectionPath, targetScriptName) = MDScriptCollectionAsset.SplitScriptPathAndScriptName(targetCollectionFullPath);

                    var targetCollection = AssetDatabase.LoadAssetAtPath<MDScriptCollectionAsset>(targetCollectionPath);
                    if (targetCollection == null)
                    {
                        Debug.LogError($"Could not find target collection {targetCollectionPath}. Linked to from {collection.AssetPath}");
                        continue;
                    }

                    var targetScript = targetCollection.GetDialogueScript(targetScriptName);
                    if (targetScript == null)
                    {
                        Debug.LogError($"Could not find target script {targetScriptName} in collection {targetCollectionPath}. Linked to from {collection.AssetPath}");
                        continue;
                    }

                    if (!script.LinkedToScripts.Contains(targetScript))
                    {
                        script.LinkedToScripts.Add(targetScript);
                        Debug.Log($"Linked {script.AssetPath} to external script {targetScript.AssetPath}");
                    }
                }
            }
        }
    }
}
