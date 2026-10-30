using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Msagl.Core.DataStructures;
using Microsoft.Msagl.Core.GraphAlgorithms;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.ProjectionSolver;

namespace Microsoft.Msagl.Layout.Layered {
    public class ConstrainedOrdering {
        readonly GeometryGraph geometryGraph;
        readonly BasicGraph<Node, PolyIntEdge> intGraph;
        public ProperLayeredGraph ProperLayeredGraph;
        readonly int[] initialLayering;
        LayerInfo[] layerInfos;
        public LayerArrays LayerArrays;
        readonly HorizontalConstraintsForSugiyama horizontalConstraints;
        int numberOfNodesOfProperGraph;
        readonly Database database;
        double[][] xPositions;
        int[][] yetBestLayers;

        readonly List<PolyIntEdge> verticalEdges = new List<PolyIntEdge>();

        readonly AdjacentSwapsWithConstraints adjSwapper;

        SugiyamaLayoutSettings settings;


        int numberOfLayers = -1;
        int noGainSteps;
        const int MaxNumberOfNoGainSteps=5;

        int NumberOfLayers {
            get {
                if (numberOfLayers > 0)
                    return numberOfLayers;
                return numberOfLayers = initialLayering.Max(i => i + 1);
            }
        }

        public double NodeSeparation() {
            return settings.NodeSeparation;
        }

        public ConstrainedOrdering(
            GeometryGraph geomGraph,
            BasicGraph<Node, PolyIntEdge> basicIntGraph,
            int[] layering,
            Dictionary<Node, int> nodeIdToIndex,
            Database database,
            SugiyamaLayoutSettings settings) {

            this.settings = settings;
            horizontalConstraints = settings.HorizontalConstraints;

            horizontalConstraints.PrepareForOrdering(nodeIdToIndex, layering);

            geometryGraph = geomGraph;
            this.database = database;
            intGraph = basicIntGraph;
            initialLayering = layering;
            //this has to be changed only to insert layers that are needed
            if (NeedToInsertLayers(layering)) {
                for (int i = 0; i < layering.Length; i++)
                    layering[i] *= 2;
                LayersAreDoubled = true;
                numberOfLayers = -1;
            }

            PrepareProperLayeredGraphAndFillLayerInfos();

            adjSwapper = new AdjacentSwapsWithConstraints(
                LayerArrays,
                HasCrossWeights(),
                ProperLayeredGraph,
                layerInfos);
        }

        bool LayersAreDoubled { get; set; }

        public bool NeedToInsertLayers(int[] layering) {
            return ExistsShortLabeledEdge(layering, intGraph.Edges) ||
                   ExistsShortMultiEdge(layering, database.Multiedges);
        }

        public static bool ExistsShortMultiEdge(int[] layering, Dictionary<IntPair, List<PolyIntEdge>> multiedges) {
            return multiedges.Any(multiedge => multiedge.Value.Count > 2 && layering[multiedge.Key.x] == 1 + layering[multiedge.Key.y]);
        }

        public void Calculate() {
            AllocateXPositions();
            var originalGraph = intGraph.Nodes[0].GeometryParent as GeometryGraph;
            LayeredLayoutEngine.CalculateAnchorSizes(database, out database.anchors, ProperLayeredGraph, originalGraph, intGraph, settings);
            LayeredLayoutEngine.CalcInitialYAnchorLocations(LayerArrays, 500, geometryGraph, database, intGraph, settings, LayersAreDoubled);
            Order();
        }

        
        public ConstrainedOrderMeasure CreateMeasure() {
            return new ConstrainedOrderMeasure(Ordering.GetCrossingsTotal(ProperLayeredGraph, LayerArrays));
        }

        public bool HasCrossWeights() {
            return ProperLayeredGraph.Edges.Any(le => le.CrossingWeight != 1);
        }

        public static bool ExistsShortLabeledEdge(int[] layering, IEnumerable<PolyIntEdge> edges) {
            return edges.Any(edge => layering[edge.Source] == layering[edge.Target] + 1 && edge.Edge.Label != null);
        }

        public void AllocateXPositions() {
            xPositions = new double[NumberOfLayers][];
            for (int i = 0; i < NumberOfLayers; i++)
                xPositions[i] = new double[LayerArrays.Layers[i].Length];
        }

        public void Order() {
            CreateInitialOrderInLayers();
            TryPushingOutStrangersFromHorizontalBlocks();
            int n = 5;

            ConstrainedOrderMeasure measure = null;
        
            while (n-- > 0 && noGainSteps <= MaxNumberOfNoGainSteps) {
                
                SetXPositions();
                
                ConstrainedOrderMeasure newMeasure = CreateMeasure();
                if (measure == null || newMeasure < measure) {
                    noGainSteps = 0;
                    Ordering.CloneLayers(LayerArrays.Layers, ref yetBestLayers);
                    measure = newMeasure;
                } else {
                    noGainSteps++;
                    RestoreState();
                }
                
            }
        }

        public void SetXPositions() {
            SolverShell solver = InitSolverWithoutOrder();
            ImproveWithAdjacentSwaps();
            PutLayerNodeSeparationsIntoSolver(solver);
            solver.Solve();
            SortLayers(solver);
            for (int i = 0; i < LayerArrays.Y.Length; i++)
                database.Anchors[i].X = solver.GetVariableResolvedPosition(i);

        }

        public SolverShell InitSolverWithoutOrder() {
            var solver=new SolverShell();
            InitSolverVars(solver);
            
            PutLeftRightConstraintsIntoSolver(solver);
            PutVerticalConstraintsIntoSolver(solver);
            AddGoalsToKeepProperEdgesShort(solver);

            AddGoalsToKeepFlatEdgesShort(solver);
            return solver;
        }

        public void SortLayers(SolverShell solver) {
            for (int i = 0; i < LayerArrays.Layers.Length; i++)
                SortLayerBasedOnSolution(LayerArrays.Layers[i], solver);
        }

        public void AddGoalsToKeepFlatEdgesShort(SolverShell solver) {
            foreach (var layerInfo in layerInfos)
                AddGoalToKeepFlatEdgesShortOnBlockLevel(layerInfo, solver);
        }

        public void InitSolverVars(SolverShell solver) {
            for (int i = 0; i < LayerArrays.Y.Length; i++)
                solver.AddVariableWithIdealPosition(i, 0);
        }

        public void AddGoalsToKeepProperEdgesShort(SolverShell solver) {
            foreach (var edge in ProperLayeredGraph.Edges)
                solver.AddGoalTwoVariablesAreClose(edge.Source, edge.Target, PositionOverBaricenterWeight);

        }

        public void PutVerticalConstraintsIntoSolver(SolverShell solver) {
            foreach (var pair in horizontalConstraints.VerticalInts) {
                solver.AddGoalTwoVariablesAreClose(pair.Item1, pair.Item2, ConstrainedVarWeight);
            }
        }

        public void PutLeftRightConstraintsIntoSolver(SolverShell solver) {
            foreach (var pair in horizontalConstraints.LeftRighInts) {
                solver.AddLeftRightSeparationConstraint(pair.Item1, pair.Item2, SimpleGapBetweenTwoNodes(pair.Item1, pair.Item2));
            }
        }

        public void PutLayerNodeSeparationsIntoSolver(SolverShell solver) {
            foreach (var layer in LayerArrays.Layers) {
                for (int i = 0; i < layer.Length - 1; i++) {
                    int l = layer[i];
                    int r = layer[i + 1];
                    solver.AddLeftRightSeparationConstraint(l, r, SimpleGapBetweenTwoNodes(l, r));
                }
            }
        }

        public void ImproveWithAdjacentSwaps() {
            adjSwapper.DoSwaps();
        }

        public void TryPushingOutStrangersFromHorizontalBlocks() {

        }

        public void CreateInitialOrderInLayers() {
            //the idea is to topologically ordering all nodes horizontally, by using vertical components, then fill the layers according to this order
            Dictionary<int, int> nodesToVerticalComponentsRoots = CreateVerticalComponents();
            IEnumerable<IntPair> liftedLeftRightRelations = LiftLeftRightRelationsToComponentRoots(nodesToVerticalComponentsRoots).ToArray();
            int[] orderOfVerticalComponentRoots = TopologicalSort.GetOrderOnEdges(liftedLeftRightRelations);
            FillLayersWithVerticalComponentsOrder(orderOfVerticalComponentRoots, nodesToVerticalComponentsRoots);
            LayerArrays.UpdateXFromLayers();
        }

        public void FillLayersWithVerticalComponentsOrder(int[] order, Dictionary<int, int> nodesToVerticalComponentsRoots) {
            Dictionary<int, List<int>> componentRootsToComponents = CreateComponentRootsToComponentsMap(nodesToVerticalComponentsRoots);
            var alreadyInLayers = new bool[LayerArrays.Y.Length];
            var runninglayerCounts = new int[LayerArrays.Layers.Length];
            foreach (var vertCompRoot in order)
                PutVerticalComponentIntoLayers(EnumerateVertComponent(componentRootsToComponents, vertCompRoot), runninglayerCounts, alreadyInLayers);
            for (int i = 0; i < ProperLayeredGraph.NodeCount; i++)
                if (alreadyInLayers[i] == false)
                    AddVertToLayers(i, runninglayerCounts, alreadyInLayers);

        }

        public IEnumerable<int> EnumerateVertComponent(Dictionary<int, List<int>> componentRootsToComponents, int vertCompRoot) {
            List<int> compList;
            if (componentRootsToComponents.TryGetValue(vertCompRoot, out compList)) {
                foreach (var i in compList)
                    yield return i;
            } else
                yield return vertCompRoot;

        }


        public void PutVerticalComponentIntoLayers(IEnumerable<int> vertComponent, int[] runningLayerCounts, bool[] alreadyInLayers) {
            foreach (var i in vertComponent)
                AddVertToLayers(i, runningLayerCounts, alreadyInLayers);
        }

        public void AddVertToLayers(int i, int[] runningLayerCounts, bool[] alreadyInLayers) {
            if (alreadyInLayers[i])
                return;
            int layerIndex = LayerArrays.Y[i];

            int xIndex = runningLayerCounts[layerIndex];
            var layer = LayerArrays.Layers[layerIndex];

            layer[xIndex++] = i;
            alreadyInLayers[i] = true;
            List<int> block;
            if (horizontalConstraints.BlockRootToBlock.TryGetValue(i, out block))
                foreach (var v in block) {
                    if (alreadyInLayers[v]) continue;
                    layer[xIndex++] = v;
                    alreadyInLayers[v] = true;
                }
            runningLayerCounts[layerIndex] = xIndex;
        }

        public static Dictionary<int, List<int>> CreateComponentRootsToComponentsMap(Dictionary<int, int> nodesToVerticalComponentsRoots) {
            var d = new Dictionary<int, List<int>>();
            foreach (var kv in nodesToVerticalComponentsRoots) {
                int i = kv.Key;
                var root = kv.Value;
                List<int> component;
                if (!d.TryGetValue(root, out component)) {
                    d[root] = component = new List<int>();
                }
                component.Add(i);
            }
            return d;
        }

        public IEnumerable<IntPair> LiftLeftRightRelationsToComponentRoots(Dictionary<int, int> nodesToVerticalComponentsRoots) {
            foreach (var pair in horizontalConstraints.LeftRighInts)
                yield return new IntPair(GetFromDictionaryOrIdentical(nodesToVerticalComponentsRoots, pair.Item1),
                    GetFromDictionaryOrIdentical(nodesToVerticalComponentsRoots, pair.Item2));
            foreach (var pair in horizontalConstraints.LeftRightIntNeibs)
                yield return new IntPair(GetFromDictionaryOrIdentical(nodesToVerticalComponentsRoots, pair.Item1),
                    GetFromDictionaryOrIdentical(nodesToVerticalComponentsRoots, pair.Item2));
        }

        public static int GetFromDictionaryOrIdentical(Dictionary<int, int> d, int key) {
            int i;
            if (d.TryGetValue(key, out i))
                return i;
            return key;
        }

        /// <summary>
        /// These blocks are connected components in the vertical constraints. They don't necesserely span consequent layers.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> CreateVerticalComponents() {
            var vertGraph = new BasicGraphOnEdges<PolyIntEdge>(from pair in horizontalConstraints.VerticalInts select new PolyIntEdge(pair.Item1, pair.Item2));
            var verticalComponents = ConnectedComponentCalculator<PolyIntEdge>.GetComponents(vertGraph);
            var nodesToComponentRoots = new Dictionary<int, int>();
            foreach (var component in verticalComponents) {
                var ca = component.ToArray();
                if (ca.Length == 1)
                    continue;
                int componentRoot = -1;
                foreach (var j in component) {
                    if (componentRoot == -1)
                        componentRoot = j;
                    nodesToComponentRoots[j] = componentRoot;
                }
            }
            return nodesToComponentRoots;
        }

        public void RestoreState() {
            LayerArrays.UpdateLayers(yetBestLayers);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledCode")]
        public void Show() {
            SugiyamaLayoutSettings.ShowDatabase(database);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Diagnostics.Debug.Write(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledCode")]
        public static void PrintPositions(double[] positions) {
            for (int j = 0; j < positions.Length; j++)
                System.Diagnostics.Debug.Write(" " + positions[j]);
            System.Diagnostics.Debug.WriteLine("");
        }



        public void SortLayerBasedOnSolution(int[] layer, SolverShell solver) {
            int length = layer.Length;
            var positions = new double[length];
            int k = 0;
            foreach (int v in layer)
                positions[k++] = solver.GetVariableResolvedPosition(v);

            Array.Sort(positions, layer);
            int i = 0;
            foreach (int v in layer)
                LayerArrays.X[v] = i++;
        }

        const double ConstrainedVarWeight = 10e6;
        const double PositionOverBaricenterWeight = 5;

        public static int NodeToBlockRootSoftOnLayerInfo(LayerInfo layerInfo, int node) {
            int root;
            return layerInfo.nodeToBlockRoot.TryGetValue(node, out root) ? root : node;
        }

        public static void AddGoalToKeepFlatEdgesShortOnBlockLevel(LayerInfo layerInfo, SolverShell solver) {
            if (layerInfo != null)
                foreach (var couple in layerInfo.flatEdges) {
                    int sourceBlockRoot = NodeToBlockRootSoftOnLayerInfo(layerInfo, couple.Item1);
                    int targetBlockRoot = NodeToBlockRootSoftOnLayerInfo(layerInfo, couple.Item2);
                    if (sourceBlockRoot != targetBlockRoot)
                        solver.AddGoalTwoVariablesAreClose(sourceBlockRoot, targetBlockRoot);
                }
        }

        public static bool NodeIsConstrainedBelow(int v, LayerInfo layerInfo) {
            if (layerInfo == null)
                return false;
            return layerInfo.constrainedFromBelow.ContainsKey(v);
        }

        public static bool NodeIsConstrainedAbove(int v, LayerInfo layerInfo) {
            if (layerInfo == null)
                return false;
            return layerInfo.constrainedFromAbove.ContainsKey(v);
        }

        public static bool BelongsToNeighbBlock(int p, LayerInfo layerInfo) {
            return layerInfo != null && (layerInfo.nodeToBlockRoot.ContainsKey(p) || layerInfo.neigBlocks.ContainsKey(p));
            //p is a root of the block
        }

        public static bool NodesAreConstrainedBelow(int leftNode, int rightNode, LayerInfo layerInfo) {
            return NodeIsConstrainedBelow(leftNode, layerInfo) && NodeIsConstrainedBelow(rightNode, layerInfo);
        }

        public static bool NodesAreConstrainedAbove(int leftNode, int rightNode, LayerInfo layerInfo) {
            return NodeIsConstrainedAbove(leftNode, layerInfo) && NodeIsConstrainedAbove(rightNode, layerInfo);
        }

        public double GetGapFromNodeNodesConstrainedBelow(int leftNode, int rightNode, LayerInfo layerInfo,
                                                          int layerIndex) {
            double gap = SimpleGapBetweenTwoNodes(leftNode, rightNode);
            leftNode = layerInfo.constrainedFromBelow[leftNode];
            rightNode = layerInfo.constrainedFromBelow[rightNode];
            layerIndex--;
            layerInfo = layerInfos[layerIndex];
            if (layerIndex > 0 && NodesAreConstrainedBelow(leftNode, rightNode, layerInfo))
                return Math.Max(gap, GetGapFromNodeNodesConstrainedBelow(leftNode, rightNode, layerInfo, layerIndex));
            return Math.Max(gap, SimpleGapBetweenTwoNodes(leftNode, rightNode));
        }

        public double GetGapFromNodeNodesConstrainedAbove(int leftNode, int rightNode, LayerInfo layerInfo,
                                                          int layerIndex) {
            double gap = SimpleGapBetweenTwoNodes(leftNode, rightNode);
            leftNode = layerInfo.constrainedFromAbove[leftNode];
            rightNode = layerInfo.constrainedFromAbove[rightNode];
            layerIndex++;
            layerInfo = layerInfos[layerIndex];
            if (layerIndex < LayerArrays.Layers.Length - 1 && NodesAreConstrainedAbove(leftNode, rightNode, layerInfo))
                return Math.Max(gap, GetGapFromNodeNodesConstrainedAbove(leftNode, rightNode, layerInfo, layerIndex));
            return Math.Max(gap, SimpleGapBetweenTwoNodes(leftNode, rightNode));
        }

        public double SimpleGapBetweenTwoNodes(int leftNode, int rightNode) {
            return database.anchors[leftNode].RightAnchor +
                   NodeSeparation() + database.anchors[rightNode].LeftAnchor;
        }

        
        public void PrepareProperLayeredGraphAndFillLayerInfos() {
            layerInfos = new LayerInfo[NumberOfLayers];
            CreateProperLayeredGraph();
            CreateExtendedLayerArrays();
            FillBlockRootToBlock();
            FillLeftRightPairs();
            FillFlatEdges();
            FillAboveBelow();
            FillBlockRootToVertConstrainedNode();
        }

        public void FillBlockRootToVertConstrainedNode() {
            foreach (LayerInfo layerInfo in layerInfos)
                foreach (int v in VertConstrainedNodesOfLayer(layerInfo)) {
                    int blockRoot;
                    if (TryGetBlockRoot(v, out blockRoot, layerInfo))
                        layerInfo.blockRootToVertConstrainedNodeOfBlock[blockRoot] = v;
                }
        }

        public static bool TryGetBlockRoot(int v, out int blockRoot, LayerInfo layerInfo) {
            if (layerInfo.nodeToBlockRoot.TryGetValue(v, out blockRoot))
                return true;
            if (layerInfo.neigBlocks.ContainsKey(v)) {
                blockRoot = v;
                return true;
            }
            return false;
        }

        public static IEnumerable<int> VertConstrainedNodesOfLayer(LayerInfo layerInfo) {
            if (layerInfo != null) {
                foreach (int v in layerInfo.constrainedFromAbove.Keys)
                    yield return v;
                foreach (int v in layerInfo.constrainedFromBelow.Keys)
                    yield return v;
            }
        }


        public void CreateExtendedLayerArrays() {
            var layeringExt = new int[numberOfNodesOfProperGraph];
            Array.Copy(initialLayering, layeringExt, initialLayering.Length);
            foreach (PolyIntEdge edge in ProperLayeredGraph.BaseGraph.Edges) {
                var ledges = (LayerEdge[])edge.LayerEdges;
                if (ledges != null && ledges.Length > 1) {
                    int layerIndex = initialLayering[edge.Source] - 1;
                    for (int i = 0; i < ledges.Length - 1; i++)
                        layeringExt[ledges[i].Target] = layerIndex--;
                }
            }
            LayerArrays = new LayerArrays(layeringExt);
        }

        public void CreateProperLayeredGraph() {
            IEnumerable<PolyIntEdge> edges = CreatePathEdgesOnIntGraph();
            var nodeCount = Math.Max(intGraph.NodeCount, BasicGraph<Node, PolyIntEdge>.VertexCount(edges));
            var baseGraph = new BasicGraph<Node, PolyIntEdge>(edges, nodeCount) { Nodes = intGraph.Nodes };
            ProperLayeredGraph = new ProperLayeredGraph(baseGraph);
        }

        public IEnumerable<PolyIntEdge> CreatePathEdgesOnIntGraph() {
            numberOfNodesOfProperGraph = intGraph.NodeCount;
            var ret = new List<PolyIntEdge>();
            foreach (PolyIntEdge ie in intGraph.Edges) {
                if (initialLayering[ie.Source] > initialLayering[ie.Target]) {
                    CreateLayerEdgesUnderIntEdge(ie);
                    ret.Add(ie);
                    if (horizontalConstraints.VerticalInts.Contains(new Tuple<int, int>(ie.Source, ie.Target)))
                        verticalEdges.Add(ie);
                }
            }

            return ret;
        }


        public void CreateLayerEdgesUnderIntEdge(PolyIntEdge ie) {
            int source = ie.Source;
            int target = ie.Target;

            int span = LayeredLayoutEngine.EdgeSpan(initialLayering, ie);
            ie.LayerEdges = new LayerEdge[span];
            Debug.Assert(span > 0);
            if (span == 1)
                ie.LayerEdges[0] = new LayerEdge(ie.Source, ie.Target, ie.CrossingWeight);
            else {
                ie.LayerEdges[0] = new LayerEdge(source, numberOfNodesOfProperGraph, ie.CrossingWeight);
                for (int i = 0; i < span - 2; i++)
                    ie.LayerEdges[i + 1] = new LayerEdge(numberOfNodesOfProperGraph++, numberOfNodesOfProperGraph,
                                                         ie.CrossingWeight);
                ie.LayerEdges[span - 1] = new LayerEdge(numberOfNodesOfProperGraph++, target, ie.CrossingWeight);
            }
        }


        public void FillAboveBelow() {
            foreach (PolyIntEdge ie in verticalEdges) {
                foreach (LayerEdge le in ie.LayerEdges) {
                    int upper = le.Source;
                    int lower = le.Target;
                    RegisterAboveBelowOnConstrainedUpperLower(upper, lower);
                }
            }

            foreach (var p in horizontalConstraints.VerticalInts)
                RegisterAboveBelowOnConstrainedUpperLower(p.Item1, p.Item2);
        }

        public void RegisterAboveBelowOnConstrainedUpperLower(int upper, int lower) {
            LayerInfo topLayerInfo = GetOrCreateLayerInfo(LayerArrays.Y[upper]);
            LayerInfo bottomLayerInfo = GetOrCreateLayerInfo(LayerArrays.Y[lower]);

            topLayerInfo.constrainedFromBelow[upper] = lower;
            bottomLayerInfo.constrainedFromAbove[lower] = upper;
        }

        public void FillFlatEdges() {
            foreach (PolyIntEdge edge in intGraph.Edges) {
                int l = initialLayering[edge.Source];
                if (l == initialLayering[edge.Target]) {
                    GetOrCreateLayerInfo(l).flatEdges.Insert(new Tuple<int, int>(edge.Source, edge.Target));
                }
            }
        }

        public void FillLeftRightPairs() {
            foreach (var p in horizontalConstraints.LeftRighInts) {
                LayerInfo layerInfo = GetOrCreateLayerInfo(initialLayering[p.Item1]);
                layerInfo.leftRight.Insert(p);
            }
        }

        /// <summary>
        /// when we call this function we know that a LayerInfo is needed
        /// </summary>
        /// <param name="layerNumber"></param>
        /// <returns></returns>
        public LayerInfo GetOrCreateLayerInfo(int layerNumber) {
            LayerInfo layerInfo = layerInfos[layerNumber] ?? (layerInfos[layerNumber] = new LayerInfo());
            return layerInfo;
        }

        public void FillBlockRootToBlock() {
            foreach (var p in horizontalConstraints.BlockRootToBlock) {
                LayerInfo layerInfo = GetOrCreateLayerInfo(initialLayering[p.Key]);
                layerInfo.neigBlocks[p.Key] = p.Value;
                foreach (int i in p.Value)
                    layerInfo.nodeToBlockRoot[i] = p.Key;
            }
        }
    }
}