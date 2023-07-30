#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue
{
    /// <summary>
    ///     An yield instruction that holds up a <see cref="MDBaseRunner"/> until continued by the player.
    /// </summary>
    public sealed class WaitForMarkDialogueChoices : CustomYieldInstruction
    {
        private bool _keepWaiting = true;
        public override bool keepWaiting => _keepWaiting;

        public IReadOnlyList<MDLink> PossibleChoices { get; }
        public MDLink SelectedChoice { get; private set; }

        internal WaitForMarkDialogueChoices(IReadOnlyList<MDLink> choices)
        {
            if (choices.Count == 0)
            {
                throw new InvalidOperationException($"Supplied choices for {nameof(WaitForMarkDialogueChoices)} was an empty list! This should never happen.");
            }

            PossibleChoices = choices;
            SelectedChoice = choices[0];
        }

        /// <summary>
        ///     Continues the dialogue script runner onto the next dialogue line.
        /// </summary>
        public void SelectChoice(MDLink link)
        {
            SelectedChoice = link;
            if (!PossibleChoices.Any(l => l == link))
            {
                // Someone's doing something funky.
                Debug.LogWarning($"{nameof(WaitForMarkDialogueChoices)} has received selected choice '{link.TargetScript}' but that wasn't one of the expected choices. This may be a bug with your code.");
            }
            _keepWaiting = false;
        }
    }
}
