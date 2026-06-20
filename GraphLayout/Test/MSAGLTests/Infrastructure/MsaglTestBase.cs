//-----------------------------------------------------------------------
// <copyright file="MsaglTestBase.cs" company="Microsoft">
//   (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Dot2Graph;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.DebugHelpers;
using Microsoft.Msagl.DebugHelpers.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Msagl.Routing;

namespace Microsoft.Msagl.UnitTests {
    [TestClass]
    public class MsaglTestBase {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext) {
            RedirectDefaultTraceListener();
            if (testContext != null) {
                runningUnitTests = true;
            }
        }

        private static bool runningUnitTests;

        // Note: Don't add [ClassInitialize] and [ClassCleanup] to MsaglTestBase because they are not called for base classes;
        // instead it is necessary to add them to the derived class.

        /// <summary>
        /// AssemblyInitialize as well as some commandline test apps call this.
        /// </summary>
        public static void RedirectDefaultTraceListener() {
            if (!Validate.InteractiveMode) {
                // If we are not in interactive mode, translate all Debug.Asserts to Assert.Fail
                // by replacing any default trace listeners with a redirecting listener.
                var defaultListeners = Trace.Listeners.OfType<DefaultTraceListener>().ToArray();
                foreach (var defaultListener in defaultListeners) {
                    Trace.Listeners.Remove(defaultListener);
                }
                Trace.Listeners.Add(new DebugAssertRedirector());
            }
        }

        public static void RestoreDefaultTraceListener() {
            if (!Validate.InteractiveMode) {
                // Restore a default trace listener.
                var redirectedListeners = Trace.Listeners.OfType<DebugAssertRedirector>().ToArray();
                foreach (var redirectedListener in redirectedListeners) {
                    Trace.Listeners.Remove(redirectedListener);
                }
                Trace.Listeners.Add(new DefaultTraceListener());
            }
        }

        [TestInitialize]
        public virtual void Initialize() {
        }

        [TestCleanup]
        public virtual void Cleanup() {
        }

        public TestContext TestContext { get; set; }

        public static void WriteLine(TestContext testContext, string line) {
            if (null != testContext) {
                testContext.WriteLine(line);
                return;
            }
            System.Diagnostics.Debug.WriteLine(line);
        }

        public void WriteLine(string line) {
            WriteLine(TestContext, line);
        }

        public void WriteLine() {
            WriteLine(String.Empty);
        }

        public void WriteLine(string format, params object[] args) {
            WriteLine(string.Format(format, args));
        }

        public static bool EnableDebugViewer() {
            return false;
        }

        internal static bool DontShowTheDebugViewer() {
            if (DebugViewerEnvVariableIsSet()) 
                return false;
            return runningUnitTests;
        }

        private static bool DebugViewerEnvVariableIsSet() {
            string s = Environment.GetEnvironmentVariable("debugviewer");
            return !String.IsNullOrEmpty(s) && String.Compare(s, "on", true, CultureInfo.CurrentCulture) == 0;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "graph")]
        protected static void ShowGraphInDebugViewer(GeometryGraph graph) {
        }

        protected GeometryGraph LoadGraph(string geometryGraphFileName) {
            LayoutAlgorithmSettings settings;
            return this.LoadGraph(geometryGraphFileName, out settings);
        }

        protected string GetDeploymentPath(params string[] relativePathSegments) {
            if (relativePathSegments == null || relativePathSegments.Length == 0) {
                throw new ArgumentException("A relative path is required.", nameof(relativePathSegments));
            }

            var baseDirectory = TestContext?.DeploymentDirectory;
            if (string.IsNullOrEmpty(baseDirectory)) {
                baseDirectory = Path.Combine(TestContext?.TestRunDirectory ?? AppContext.BaseDirectory, "Out");
            }

            return relativePathSegments.Aggregate(baseDirectory, Path.Combine);
        }

        string ResolveTestFilePath(string filePath) {
            if (Path.IsPathRooted(filePath) || File.Exists(filePath)) {
                return filePath;
            }

            var baseDirectories = new[] {
                TestContext?.DeploymentDirectory,
                string.IsNullOrEmpty(TestContext?.TestRunDirectory) ? null : Path.Combine(TestContext.TestRunDirectory, "Out"),
                AppContext.BaseDirectory,
            };

            foreach (var baseDirectory in baseDirectories.Where(directory => !string.IsNullOrEmpty(directory))) {
                var directPath = Path.Combine(baseDirectory, filePath);
                if (File.Exists(directPath)) {
                    return directPath;
                }

                var geometryGraphPath = Path.Combine(baseDirectory, "Resources", "MSAGLGeometryGraphs", filePath);
                if (File.Exists(geometryGraphPath)) {
                    return geometryGraphPath;
                }
            }

            return filePath;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        protected GeometryGraph LoadGraph(string geometryGraphFileName, out LayoutAlgorithmSettings settings) {
            if (string.IsNullOrEmpty(geometryGraphFileName)) {
                throw new ArgumentNullException("geometryGraphFileName");
            }

            var resolvedGraphFileName = ResolveTestFilePath(geometryGraphFileName);
            GeometryGraph graph = null;
            settings = null;
            if (resolvedGraphFileName.EndsWith(".geom", StringComparison.OrdinalIgnoreCase)) {
                graph = GeometryGraphReader.CreateFromFile(resolvedGraphFileName, out settings);
                SetupPorts(graph);
            }
            else if (resolvedGraphFileName.EndsWith(".dot", StringComparison.OrdinalIgnoreCase)) {
                int line;
                string msg;
                int col;
                Drawing.Graph drawingGraph = Parser.Parse(resolvedGraphFileName, out line, out col, out msg);
                drawingGraph.CreateGeometryGraph();
                graph = drawingGraph.GeometryGraph;
                settings = drawingGraph.CreateLayoutSettings();
                GraphGenerator.SetRandomNodeShapes(graph, new Random(1));
            }
            else {
                Assert.Fail("Unknown graph format for file: " + resolvedGraphFileName);
            }

            TestContext.WriteLine("Loaded graph: {0} Nodes, {1} Edges", graph.Nodes.Count, graph.Edges.Count);
            return graph;
        }

        protected static RelativeFloatingPort MakePort(Node node) {
            if (node is Cluster)
                return new ClusterBoundaryPort(() => node.BoundaryCurve, () => node.Center);

            return new RelativeFloatingPort(() => node.BoundaryCurve, () => node.Center);
        }

        protected static void SetupPorts(GeometryGraph graph) {
            if (graph == null) {
                throw new ArgumentNullException("graph");
            }

            foreach (var e in graph.Edges) {
                e.SourcePort = MakePort(e.Source);
                e.TargetPort = MakePort(e.Target);
            }

            FixHookPorts(graph);
        }

        static void FixHookPorts(GeometryGraph geometryGraph) {
            foreach (var edge in geometryGraph.Edges) {
                var s = edge.Source;
                var t = edge.Target;
                var sc = s as Cluster;
                if (sc != null && Ancestor(sc, t)) {
                    edge.SourcePort = new HookUpAnywhereFromInsidePort(() => s.BoundaryCurve);
                }
                else {
                    var tc = t as Cluster;
                    if (tc != null && Ancestor(tc, s)) {
                        edge.TargetPort = new HookUpAnywhereFromInsidePort(() => t.BoundaryCurve);
                    }
                }
            }
        }


        static bool Ancestor(Cluster root, Node node) {
            if (node.ClusterParent == root)
                return true;
            return node.AllClusterAncestors.Any(p => p.ClusterParent == root);

        }

        /// <summary>
        /// Create a clustered sample graph with rectangular boundary margins of 5
        /// </summary>
        /// <returns>a clustered sample graph with rectangular boundary margins of 5</returns>
        public static GeometryGraph CreateClusteredGraph() {
            return CreateClusteredGraph(5);
        }

        /// <summary>
        /// Creates a small, non-trivial clustered and disconnected graph for tests
        ///   ( A B-)-C
        ///   D-(-(-E F)-G)
        ///   H-I
        ///   J
        ///   (K L-)-(-M N)
        /// </summary>
        /// <returns>returns a disconnected clustered graph</returns>
        public static GeometryGraph CreateClusteredGraph(double padding) {
            var graph = new GeometryGraph();
            for (int i = 0; i < 14; ++i) {
                graph.Nodes.Add(new Node(CurveFactory.CreateRectangle(10, 10, new Point()), GetCharacter(i)));
            }
            AddEdge(graph, "B", "C");
            AddEdge(graph, "D", "E");
            AddEdge(graph, "H", "I");
            var root = AddRootCluster(graph, "C", "D", "H", "I", "J");
            AddCluster(padding, graph, root, "A", "B");
            var parent = AddCluster(padding, graph, root, "G");
            var child = AddCluster(padding, graph, parent, "E", "F");
            graph.Edges.Add(new Edge(graph.FindNodeByUserData("G"), child));
            graph.Edges.Add(new Edge(AddCluster(padding, graph, root, "K", "L"), AddCluster(padding, graph, root, "M", "N")));

            return graph;
        }

        private static string GetCharacter(int i) {
            string name = string.Empty;
            var c = (char)(65 + (i % 26));
            name += c;
            return name;
        }

        private static void AddEdge(GeometryGraph g, string u, string v) {
            var e = new Edge(g.FindNodeByUserData(u), g.FindNodeByUserData(v));
            e.EdgeGeometry.SourceArrowhead = new Arrowhead { Length = 5, Width = 5 };
            e.EdgeGeometry.TargetArrowhead = new Arrowhead { Length = 5, Width = 5 };
            g.Edges.Add(e);
        }

        private static Cluster AddRootCluster(GeometryGraph g, params string[] vs) {
            var c = new Cluster(from v in vs select g.FindNodeByUserData(v));
            g.RootCluster = c;
            return c;
        }

        private static Cluster AddCluster(double padding, GeometryGraph g, Cluster parent, params string[] vs) {
            var c = new Cluster(from v in vs select g.FindNodeByUserData(v)) {
                UserData = string.Concat(vs),
                RectangularBoundary =
                    new RectangularClusterBoundary { LeftMargin = padding, RightMargin = padding, BottomMargin = padding, TopMargin = padding },
                BoundaryCurve = CurveFactory.CreateRectangle(30, 30, new Point(15, 15))
            };
            parent.AddChild(c);
            return c;
        }
    }
}
