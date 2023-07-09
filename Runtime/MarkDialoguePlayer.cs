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
        protected override void OnDialogueStart(MarkDialoguePlayerState state)
        {
            Debug.Log("Starting Dialogue");
        }

        protected override void OnDialogueEnd()
        {
            Debug.Log("Ending Dialogue");
        }

        protected override void OnDialogueLine(MarkDialogueCharacter character, MarkDialogueLine dialogueLine, WaitForDialogueContinuation continuation)
        {
            Debug.Log($"{character.CharacterIdentifier}: {dialogueLine.LineText}");
            continuation.ContinueDialogue();
        }

        protected override void OnQuoteText(MarkDialogueLine quoteLine)
        {
            Debug.LogWarning($"[Dialogue Comment] {quoteLine}");
        }
    }
}
