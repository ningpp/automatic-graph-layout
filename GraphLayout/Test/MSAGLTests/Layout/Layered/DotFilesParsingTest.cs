// -----------------------------------------------------------------------
// <copyright file="DotFilesParsingTest.cs" company="Microsoft">
//   (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dot2Graph;
using Microsoft.Msagl.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Msagl.UnitTests.Layout.Layered {
    /// <summary>
    /// Parses every valid .dot file shipped under Resources/DotFiles/LevFiles and
    /// reports which ones cannot be parsed (or cannot build a geometry graph).
    /// Unlike RandomDotFileTests this walks the whole corpus instead of a random
    /// subset, and it does not rely on the .NET Framework-only DeploymentItem
    /// mechanism (which is a no-op on .NET 8). It locates the source tree from
    /// the assembly location so it works from any test host.
    /// </summary>
    [TestClass]
    public class DotFilesParsingTest : MsaglTestBase {
        [TestMethod]
        public void Utf8BomPrefixedDotFile_Parses()
        {
            var file = Path.Combine(GetTestResourcePath("DotFiles", "LevFiles"), "deon.dot");
            int line, col;
            string msg;

            var graph = Parser.Parse(file, out line, out col, out msg);

            Assert.IsNotNull(graph, $"{file} failed at line {line}, col {col}: {msg}");
        }

        [TestMethod]
        public void CrOnlyLineContinuationDotFile_Parses()
        {
            var file = Path.Combine(GetTestResourcePath("DotFiles", "LevFiles"), "bug.dot");
            int line, col;
            string msg;

            var graph = Parser.Parse(file, out line, out col, out msg);

            Assert.IsNotNull(graph, $"{file} failed at line {line}, col {col}: {msg}");
        }

        [TestMethod]
        [Description("Parse every valid .dot file under Resources/DotFiles/LevFiles and report the ones that fail. " +
                     "This test only exercises the parser (Parser.Parse). It intentionally does NOT call " +
                     "CreateGeometryGraph, because geometry-graph construction can hit Debug.Assert (e.g. a node " +
                     "asserted to have a single cluster parent) which, on .NET 8, terminates the test host process " +
                     "via FailFast and would prevent the parse-analysis report from ever being produced.")]
        public void AllDotFiles_ParseWithoutError() {
            var levDir = GetTestResourcePath("DotFiles", "LevFiles");
            Assert.IsTrue(Directory.Exists(levDir), "LevFiles directory not found at " + levDir);

            string[] dotFiles = DotFileTestCorpus.GetDotFiles(levDir);
            Assert.IsTrue(dotFiles.Length > 0, "No .dot files found under " + levDir);
            dotFiles = new string[] { dotFiles[0] };

            var parseFailures = new List<string>();      // Parser.Parse returned null
            var exceptions = new List<string>();          // Parser.Parse itself threw

            int parsedOk = 0;
            foreach (string file in dotFiles.OrderBy(f => f, StringComparer.OrdinalIgnoreCase)) {
                int line, col;
                string msg;
                Drawing.Graph drawingGraph;
                try {
                    drawingGraph = Parser.Parse(file, out line, out col, out msg);
                } catch (Exception e) {
                    exceptions.Add(string.Format("{0}: {1}: {2}", Path.GetFileName(file), e.GetType().Name, e.Message));
                    continue;
                }

                if (drawingGraph == null) {
                    parseFailures.Add(string.Format("{0}: parse returned null (line {1}, col {2}, msg='{3}')",
                        Path.GetFileName(file), line, col, msg));
                    continue;
                }

                parsedOk++;
            }

            WriteLine("Dot file parsing summary for {0}:", levDir);
            WriteLine("  total   : {0}", dotFiles.Length);
            WriteLine("  ok      : {0}", parsedOk);
            WriteLine("  null    : {0}", parseFailures.Count);
            WriteLine("  thrown  : {0}", exceptions.Count);

            foreach (string f in parseFailures) {
                WriteLine("  PARSE-NULL : {0}", f);
            }
            foreach (string f in exceptions) {
                WriteLine("  EXCEPTION  : {0}", f);
            }

            if (parseFailures.Count + exceptions.Count > 0) {
                string detail = string.Join(Environment.NewLine, parseFailures.Concat(exceptions));
                Assert.Fail(string.Format(
                    "{0} of {1} .dot files failed to parse:{2}{3}",
                    parseFailures.Count + exceptions.Count,
                    dotFiles.Length,
                    Environment.NewLine,
                    detail));
            }
        }
    }
}
