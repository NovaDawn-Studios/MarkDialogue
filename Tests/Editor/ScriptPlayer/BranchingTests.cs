using NovaDawnStudios.MarkDialogue.Data;
using NUnit.Framework;
using UnityEngine;

namespace NovaDawnStudios.MarkDialogue.Editor.Tests
{
    public class BranchingTests
    {
        readonly string SimpleIfBranchTestScript = @"
JOHN
Begin

#if eq(branch1, true)
    JOHN
    Branch
#else
    #throw Test failed
#endif

JOHN
End
";

        readonly string SimpleElseBranchTestScript = @"
JOHN
Begin

#if eq(branch1, true)

    #throw Test failed

#else

    JOHN
    Else

#endif

JOHN
End
";

        readonly string NestedBranchingTestScript = @"
JOHN
Begin

#if eq(branch1, true)

    #throw Test failed - Entered first #if block.

#else

    JOHN
    Else

    #if eq(branch2, true)

        JOHN
        Nested branch

    #else

        #throw Test failed - Entered nested #else block.

    #endif

#endif

JOHN
End
";

        [Test]
        public void HandlesIfBranchesCorrectly()
        {
            var collection = MDScriptCollectionAsset.ParseScript(SimpleIfBranchTestScript, "dummy/test.dlg.md");
            Assert.IsNotNull(collection.Scripts);
            // TODO: Test it actually runs.
        }

        [Test]
        public void HandlesElseBranchesCorrectly()
        {
            var collection = MDScriptCollectionAsset.ParseScript(SimpleElseBranchTestScript, "dummy/test.dlg.md");
            Assert.IsNotNull(collection.Scripts);
            // TODO: Test it actually runs.
        }

        [Test]
        public void HandlesNestedBranchesCorrectly()
        {
            var collection = MDScriptCollectionAsset.ParseScript(NestedBranchingTestScript, "dummy/test.dlg.md");
            Assert.IsNotNull(collection.Scripts);
            // TODO: Test it actually runs.
        }
    }
}
