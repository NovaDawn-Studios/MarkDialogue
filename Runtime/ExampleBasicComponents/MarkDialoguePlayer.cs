#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue
{
    /// <summary>
    ///     An example player that runs the script, invoking the relevant Unity events as required.
    /// </summary>
    [AddComponentMenu("NovaDawn Studios/MarkDialogue/MarkDialogue Player", 182)]
    public class MarkDialoguePlayer : MDBaseRunner
    {
        protected override void OnDialogueStart(MDRunnerState state)
        {
            state.VariableStore = MarkDialogueVariableStore.CreateInstanceIfRequired();
            Debug.Log("Starting Dialogue");
        }

        protected override void OnDialogueEnd()
        {
            Debug.Log("Ending Dialogue");
        }

        protected override void OnDialogueLine(MDCharacter character, MDDialogueLine dialogueLine, WaitForDialogueResumption continuation)
        {
            Debug.Log($"{character.CharacterIdentifier}: {dialogueLine.LineText}");
            continuation.ContinueDialogue();
        }

        protected override void OnQuoteText(MDQuoteLine quoteLine)
        {
            Debug.LogWarning($"[Dialogue Comment] {quoteLine}");
        }

        protected override void OnDialogueChoices(WaitForMarkDialogueChoices continuation)
        {
            Debug.LogWarning($"[Dialogue Choice] Picking first choice");
            continuation.SelectChoice(continuation.PossibleChoices[0]);
        }
    }
}
