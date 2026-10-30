using System;
using System.IO;
using System.Linq;

namespace Microsoft.Msagl.UnitTests {
    internal static class DotFileTestCorpus {
        private static readonly string[] NonDotFixtures = { "bugs.dot" };

        internal static string[] GetDotFiles(string directory) {
            return Directory.GetFiles(directory, "*.dot", SearchOption.AllDirectories)
                .Where(file => !NonDotFixtures.Contains(Path.GetFileName(file), StringComparer.OrdinalIgnoreCase))
                .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }
}
