#nullable enable

namespace NovaDawnStudios.MarkDialogue.Interfaces
{
    public interface IMarkDialogueVariableStore
    {
        /// <summary>
        ///     Fetches the supplied MarkDialogue variable as a <see langword="string"/>. 
        ///     Returns <see langword="null"/> if the variable cannot be found.
        /// </summary>
        /// <param name="variableName">The name of the variable to fetch, case-insensitive. Should not include any whitespace.</param>
        /// <returns>The requested variables value, or <see langword="null"/> if the variable doesn't exist.</returns>
        public string? GetMarkDialogueVariable(string variableName);

        /// <summary>
        ///     Sets the supplied variable's value, creating the variable if it didn't exist before. 
        ///     If <paramref name="value"/> is <see langword="null"/>, then the variable will be deleted.
        /// </summary>
        /// <param name="variableName">The name of the variable to set, case-insensitive. Should not include any whitespace.</param>
        /// <param name="value">The new value to set the variable to. If <see langword="null"/>, the variable is deleted instead.</param>
        public void SetMarkDialogueVariable(string variableName, string? value);

        /// <summary>
        ///     Increments the visit count for a particular MarkDialogue script.
        /// </summary>
        /// <param name="scriptAssetPath">
        ///     The asset path to the script that has ran to completion. 
        ///     The visit count for this script will increment by one.
        /// </param>
        public void SetMarkDialogueScriptVisited(string scriptAssetPath);

        /// <summary>
        ///     Fetches the visit count for a particular MarkDialogue script.
        /// </summary>
        /// <param name="scriptAssetPath">The asset path to the script that we're fetching a visit count for.</param>
        /// <returns>The number of times the script was visited. Will return 0 if the script hasn't been visited yet.</returns>
        public int GetMarkDialogueScriptVisitCount(string scriptAssetPath);
    }
}
