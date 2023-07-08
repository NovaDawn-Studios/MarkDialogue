#nullable enable

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Data
{
    /// <summary>
    ///     Represents a collection of one or more MarkDialogue scripts.
    /// </summary>
    public sealed class MarkDialogueScriptCollection : ScriptableObject
    {
        /// <summary>
        ///     Matches an Obsidian comment in the form <c>%% some txt %%</c>. These comments are removed from the script before parsing.
        /// </summary>
        private static readonly Regex commentRegex = new Regex(@"%%.*?%%", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        ///     The raw script as supplied from the source file.
        /// </summary>
        /// <remarks>
        ///     DEVNOTE: This exists for testing.
        /// </remarks>
        [field: SerializeField, Multiline] public string RawScript { get; set; } = "";

        /// <summary>
        ///     The collection of one or more MarkDialogue scripts found within the supplied script file.
        /// </summary>
        [field: SerializeField] public List<MarkDialogueScript> Scripts { get; set; } = new List<MarkDialogueScript>();

        /// <summary>
        ///     Creates a new instance of <see cref="MarkDialogueScriptCollection"/>, then parses the raw script file into one or more 
        ///     <see cref="MarkDialogueScript"/> instances as stored in <see cref="Scripts"/>.
        /// </summary>
        /// <param name="rawScript">The raw script to parse.</param>
        /// <returns>The newly created <see cref="MarkDialogueScriptCollection"/>.</returns>
        public static MarkDialogueScriptCollection ParseScript(string rawScript)
        {
            var collection = ScriptableObject.CreateInstance<MarkDialogueScriptCollection>();
            collection.RawScript = rawScript;

            var cleanedScript = commentRegex.Replace(rawScript, ""); //Remove comments.
            var splScript = cleanedScript.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            int line = 0;
            while (line < splScript.Length)
            {
                var script = ScriptableObject.CreateInstance<MarkDialogueScript>();
                line = script.Parse(splScript, line);
                collection.Scripts.Add(script);
            }

            return collection;
        }
    }
}
