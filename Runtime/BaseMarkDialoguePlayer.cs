#nullable enable

using NovaDawnStudios.MarkDialogue.Data;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue
{
    /// <summary>
    ///     Provides the core functionality for running a MarkDialogue script. 
    ///     Inherit from this class to provide your own implementation as require by your project. 
    /// </summary>
    public abstract class BaseMarkDialoguePlayer : MonoBehaviour
    {
        /// <summary>The script collection containing the MarkDialogue script we want to run.</summary>
        [field: SerializeField] public MarkDialogueScriptCollection? ScriptCollection { get; set; }

        /// <summary>
        ///     The name of the MarkDialogue script to run. 
        ///     This defaults to the default script. Not used if <see cref="ScriptCollection"/> only contains one script.
        /// </summary>
        [field: SerializeField] public string ScriptToRun { get; set; } = MarkDialogueScript.DEFAULT_SCRIPT_NAME;

        /// <summary>
        ///     <para>
        ///         If <see langword="true"/>, this dialogue is kicked off the moment this component's <see cref="Start"/> method is invoked.
        ///     </para>
        ///     <para>
        ///         If <see langword="false"/>, you'll need to call <see cref="PlayScript"/> from your own code.
        ///     </para>
        /// </summary>
        [field: SerializeField] public bool AutoStart { get; set; } = false;


        protected virtual void Start()
        {
            if (AutoStart)
            {
                PlayScript();
            }
        }

        /// <summary>
        ///     Starts the script from the beginning.
        /// </summary>
        public abstract void PlayScript();
    }
}
