#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using NovaDawnStudios.MarkDialogue.Interfaces;
using System;
using System.Collections.Generic;

namespace NovaDawnStudios.MarkDialogue
{
    public class MarkDialoguePlayerState
    {
        /// <summary>The player that's running this dialogue.</summary>
        public BaseMarkDialoguePlayer Player { get; }

        /// <summary>
        ///     The variable store that existed when this script started running, if any. 
        ///     Don't forget to set this if your scripts run any logic!
        /// </summary>
        public IMarkDialogueVariableStore? VariableStore { get; set; }

        /// <summary>The current collection that we're running a script from.</summary>
        public MarkDialogueScriptCollection Collection { get; private set; }

        /// <summary>The current script we're running..</summary>
        public MarkDialogueScript Script { get; private set; }

        /// <summary>The current line index in the current <see cref="Script"/>.</summary>
        public int CurrentScriptLineNumber { get; set; }

        /// <summary><see langword="true"/> if the dialogue is about to complete. If <see langword="true"/>, any unparsed dialogue will be skipped.</summary>
        public bool IsComplete { get; set; }

        /// <summary>The character who last spoke last.</summary>
        public MarkDialogueCharacter? CurrentCharacter { get; set; }

        /// <summary>
        ///     <para>
        ///         The current list of available choices the player can choose when the script ends.
        ///     </para>
        ///     <para>
        ///         If this is empty, the script ends cleanly and <see cref="Player"/> ends execution. 
        ///     </para>
        ///     <para>
        ///         If there is only one entry in this when a script concludes, <see cref="Player"/> will jump to that script with no player intervention.
        ///     </para>
        /// </summary>
        public List<MarkDialogueLink> Choices { get; set; } = new List<MarkDialogueLink>();

        public MarkDialoguePlayerState(BaseMarkDialoguePlayer player, MarkDialogueScriptCollection initialCollection, MarkDialogueScript initialScript)
        {
            Player = player;
            Collection = initialCollection;
            Script = initialScript;
            CurrentScriptLineNumber = 0;
        }

        /// <summary>
        ///     Creates a standardized logging string for MarkDialogue use. Includes the currently running script and collection, the line number we're
        ///     at and the supplied <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message we're about to log.</param>
        /// <returns>The supplied <paramref name="message"/> with extra formatting details.</returns>
        public string CreateLoggingString(string message)
        {
            return $"[{Collection.name}/{Script.name} @ {CurrentScriptLineNumber + Script.StartLine}] {message}";
        }

        /// <summary>
        ///     Starts a new script, optionally from a new collection. This resets a number of values and marks the previous script as having been visited.
        /// </summary>
        /// <param name="newScript">The new script we're launching.</param>
        /// <param name="newCollection"></param>
        public void StartNewScript(MarkDialogueScript newScript, MarkDialogueScriptCollection? newCollection = null)
        {
            if (VariableStore != null)
            {
                VariableStore.SetMarkDialogueScriptVisited(Script.AssetPath);
            }

            if (newCollection != null)
            {
                Collection = newCollection;
            }

            Script = newScript;
            CurrentScriptLineNumber = 0;
            Choices.Clear();
        }

        public string? SkipToNextTag(string[] targetTags, string[]? scopeStartTags = null, string[]? scopeEndTags = null)
        {
            int scopeLevel = 0;
            while (++CurrentScriptLineNumber < Script.Lines.Count)
            {
                var match = MarkDialogueRegexCollection.tagRegex.Match(Script.Lines[CurrentScriptLineNumber].rawLine);
                if (!match.Success)
                {
                    continue;
                }

                var tag = match.Groups["tag"].Value;

                if (scopeLevel == 0 && Array.Exists(targetTags, s => s.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                {
                    return tag;
                }

                if (scopeStartTags != null && Array.Exists(scopeStartTags, s => s.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                {
                    ++scopeLevel;
                }

                if (scopeEndTags != null && Array.Exists(scopeEndTags, s => s.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                {
                    --scopeLevel;
                }
            }

            return null;
        }

        public string ResolveScriptPath(string relativePath)
        {
            if (relativePath.Equals("this", StringComparison.OrdinalIgnoreCase))
            {
                return Script.AssetPath;
            }

            // TODO: Implement
            return relativePath;
        }
    }
}
