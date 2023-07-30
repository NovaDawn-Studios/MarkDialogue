#nullable enable

using NovaDawnStudios.MarkDialogue.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Data
{
    /// <summary>
    ///     Represents a collection of one or more MarkDialogue scripts.
    /// </summary>
    public sealed class MDScriptCollectionAsset : ScriptableObject
    {
        /// <summary>
        ///     The raw script as supplied from the source file.
        /// </summary>
        /// <remarks>
        ///     DEVNOTE: This exists for testing.
        /// </remarks>
        [field: SerializeField, Multiline] public string RawScript { get; set; } = "";

        /// <summary>
        ///     The file path to this script collection. Used to find other scripts via <c>[[Link]]</c> tags.
        /// </summary>
        [field: SerializeField] public string AssetPath { get; set; } = "";

        /// <summary>
        ///     The collection of one or more MarkDialogue scripts found within the supplied script file.
        /// </summary>
        [field: SerializeField] public List<MDScriptAsset> Scripts { get; set; } = new List<MDScriptAsset>();

        /// <summary>
        ///     Returns the script that matches the supplied <paramref name="scriptName"/>, if any. If this collection only contains a single script,
        ///     that is always returned.
        /// </summary>
        /// <param name="scriptName">The name of the script to fetch. Ignored if this collection only contains a single script.</param>
        /// <returns>The found script, or <see langword="null"/> if no script matching <paramref name="scriptName"/> is found.</returns>
        public MDScriptAsset? GetDialogueScript(string? scriptName)
        {
            if (Scripts.Count == 1)
            {
                return Scripts[0];
            }

            if (string.IsNullOrWhiteSpace(scriptName))
            {
                scriptName = MDScriptAsset.DEFAULT_SCRIPT_NAME;
            }

            return Scripts.Find(s => s.name == scriptName);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="MDScriptCollectionAsset"/>, then parses the raw script file into one or more 
        ///     <see cref="MDScriptAsset"/> instances as stored in <see cref="Scripts"/>.
        /// </summary>
        /// <param name="assetPath">The path to the script collection to parse.</param>
        /// <returns>The newly created <see cref="MDScriptCollectionAsset"/>.</returns>
        public static MDScriptCollectionAsset ParseScript(string assetPath)
        {
            var rawScript = File.ReadAllText(assetPath);
            return ParseScript(rawScript, assetPath);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="MDScriptCollectionAsset"/>, then parses the raw script file into one or more 
        ///     <see cref="MDScriptAsset"/> instances as stored in <see cref="Scripts"/>.
        /// </summary>
        /// <param name="assetPath">The path to the script collection to parse.</param>
        /// <returns>The newly created <see cref="MDScriptCollectionAsset"/>.</returns>
        public static MDScriptCollectionAsset ParseScript(string rawScript, string assetPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(assetPath);

            var collection = ScriptableObject.CreateInstance<MDScriptCollectionAsset>();
            collection.name = fileName;
            collection.AssetPath = assetPath;
            collection.RawScript = rawScript;

            var cleanedScript = MDRegexCollection.commentRegex.Replace(rawScript, ""); //Remove comments.
            var splScript = cleanedScript.Split('\n');
            int line = 0;
            while (line < splScript.Length)
            {
                var script = ScriptableObject.CreateInstance<MDScriptAsset>();
                line = script.Parse(collection, splScript, line);

                if (collection.Scripts.Exists(s => s.name.Equals(script.name, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new DuplicateMarkDialogueScriptException(collection, script);
                }

                collection.Scripts.Add(script);
            }

            return collection;
        }

        public string ResolveRelativePathTo(string relativePath)
        {
            if (relativePath[0] == '#') // It's a local jump
            {
                return AssetPath + relativePath;
            }

            var (targetPath, targetScriptName) = SplitScriptPathAndScriptName(relativePath);
            targetPath = Path.Combine(Path.GetDirectoryName(AssetPath), targetPath);

            if (!targetPath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                targetPath += ".md";
            }

            if (targetScriptName.Length > 0)
            {
                targetPath += $"#{targetScriptName}";
            }

            return targetPath.Replace('\\', '/');
        }

        public static (string path, string scriptName) SplitScriptPathAndScriptName(string scriptPath)
        {
            var scriptName = "";
            if (scriptPath.Contains("#"))
            {
                var idx = scriptPath.IndexOf("#");
                scriptName = scriptPath.Substring(idx + 1);
                scriptPath = scriptPath.Substring(0, idx);
            }

            return (scriptPath, scriptName);
        }
    }
}
