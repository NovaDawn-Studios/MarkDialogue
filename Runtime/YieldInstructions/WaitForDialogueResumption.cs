#nullable enable

using UnityEngine;

namespace NovaDawnStudios.MarkDialogue
{
    /// <summary>
    ///     An yield instruction that holds up a <see cref="MDBaseRunner"/> until continued by the player.
    /// </summary>
    public sealed class WaitForDialogueResumption : CustomYieldInstruction
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
