#nullable enable

using UnityEngine;

namespace NovaDawnStudios.MarkDialogue
{
    /// <summary>
    ///     An yield instruction that holds up a <see cref="BaseMarkDialoguePlayer"/> until continued by the player.
    /// </summary>
    public class WaitForDialogueContinuation : CustomYieldInstruction
    {
        private bool _keepWaiting = true;
        public override bool keepWaiting => _keepWaiting;

        /// <summary>
        ///     Continues the dialogue script runner onto the next dialogue line.
        /// </summary>
        public void ContinueDialogue()
        {
            _keepWaiting = false;
        }
    }
}
