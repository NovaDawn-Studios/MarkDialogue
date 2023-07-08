#nullable enable

using UnityEngine;

namespace NovaDawnStudios.MarkDialogue
{
    /// <summary>
    ///     An example player that runs the script, invoking the relevant Unity events as required.
    /// </summary>
    [AddComponentMenu("NovaDawn Studios/MarkDialogue/MarkDialogue Player", 182)]
    public class MarkDialoguePlayer : BaseMarkDialoguePlayer
    {
        /// <inheritdoc/>
        public override void PlayScript()
        {
            // TODO: Implement a proper player.
            // DEV: Debug code
            if (ScriptCollection == null)
            {
                return;
            }

            if (ScriptCollection.Scripts.Count == 0)
            {
                Debug.LogError($"Could not run script {ScriptToRun} in collection {ScriptCollection} - The supplied collection has no scripts! Maybe you need to reimport?");
                return;
            }

            foreach (var script in ScriptCollection.Scripts)
            {
                Debug.Log(script.ScriptName);
                foreach (var line in script.Lines)
                {
                    Debug.Log($"{line.type} -> {line.rawLine}");
                }
            }
        }
    }
}
