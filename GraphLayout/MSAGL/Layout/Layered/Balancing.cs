using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Msagl.Core;
using Microsoft.Msagl.Core.DataStructures;
using Microsoft.Msagl.Core.GraphAlgorithms;
using Graph = Microsoft.Msagl.Core.GraphAlgorithms.BasicGraphOnEdges<Microsoft.Msagl.Layout.Layered.PolyIntEdge>;

namespace Microsoft.Msagl.Layout.Layered {
    /// <summary>
    /// balances the layers by moving vertices with
    /// the same number of input-output edges to feasible layers with fewer nodes
    /// </summary>
    public class Balancing : AlgorithmBase{

        public Set<int> jumpers = new Set<int>();

        public Dictionary<int, IntPair> possibleJumperFeasibleIntervals;
        /// <summary>
        /// numbers of vertices in layers 
        /// </summary>
        int[] vertsCounts;

        Graph dag;

        int[] layering;

        int[] nodeCount;

        /// <summary>
        /// balances the layers by moving vertices with
        /// the same number of input-output edges to feasible layers with fiewer nodes  /// </summary>
        /// <param name="dag">the layered graph</param>
        /// <param name="layering">the layering to change</param>
        /// <param name="nodeCount">shows how many nodes are represented be a node</param>
        /// <param name="cancelObj"></param>
        public static void Balance(Graph dag, int[] layering, int[] nodeCount, CancelToken cancelObj) {
            Balancing b = new Balancing(dag, layering, nodeCount);
            b.Run(cancelObj);
        }

        public Balancing(Graph dag, int[] layering, int[]nodeCount) {
            this.nodeCount = nodeCount;
            this.dag = dag;
            this.layering = layering;
            Init();
        }

        protected override void RunInternal() {
            while (jumpers.Count > 0)
                Jump(ChooseJumper());
        }

        public void Init() {
            CalculateLayerCounts();
            InitJumpers();
        }

        public void Jump(int jumper) {
            ProgressStep();
            this.jumpers.Delete(jumper);
            IntPair upLow = this.possibleJumperFeasibleIntervals[jumper];
            int jumperLayer, layerToJumpTo;
            if (this.CalcJumpInfo(upLow.x, upLow.y, jumper, out jumperLayer, out layerToJumpTo)) {
                this.layering[jumper] = layerToJumpTo;
                int jumperCount = nodeCount[jumper];
                this.vertsCounts[jumperLayer] -= jumperCount;
                this.vertsCounts[layerToJumpTo] += jumperCount;
                UpdateRegionsForPossibleJumpersAndInsertJumpers(jumperLayer, jumper);
            }
        }
    

        public bool IsJumper(int v) {
            return possibleJumperFeasibleIntervals.ContainsKey(v);
        }
        /// <summary>
        /// some other jumpers may stop being ones if the jump 
        /// was just in to their destination layer, so before the actual 
        /// jump we have to recheck if the jump makes sense
        /// 
        /// </summary>
        /// <param name="jumperLayer">old layer of jumper</param>
        /// <param name="jumper"></param>
        public void UpdateRegionsForPossibleJumpersAndInsertJumpers(int jumperLayer, int jumper) {
            Set<int> neighborPossibleJumpers = new Set<int>();
            //update possible jumpers neighbors
            foreach (int v in new Pred(dag, jumper))
                if (IsJumper(v)) {
                    this.CalculateRegionAndInsertJumper(v);
                    neighborPossibleJumpers.Insert(v);
                }

            foreach (int v in new Succ(dag, jumper))
                if (IsJumper(v)) {
                    this.CalculateRegionAndInsertJumper(v);
                    neighborPossibleJumpers.Insert(v);
                }

            List<int> possibleJumpersToUpdate = new List<int>();

            foreach (KeyValuePair<int, IntPair> kv in this.possibleJumperFeasibleIntervals) {
                if (!neighborPossibleJumpers.Contains(kv.Key))
                    if (kv.Value.x > jumperLayer && kv.Value.y < jumperLayer)
                        possibleJumpersToUpdate.Add(kv.Key);
            }

            foreach (int v in possibleJumpersToUpdate)
                this.CalculateRegionAndInsertJumper(v);
        }

        public void InitJumpers() {
            int[] deltas = new int[this.dag.NodeCount];
            foreach (PolyIntEdge ie in dag.Edges) {
                deltas[ie.Source] -= ie.Weight;
                deltas[ie.Target] += ie.Weight;
            }

            this.possibleJumperFeasibleIntervals = new Dictionary<int, IntPair>();

            for (int i = 0; i < dag.NodeCount; i++)
                if (deltas[i] == 0)
                    CalculateRegionAndInsertJumper(i);
        }

        public void CalculateRegionAndInsertJumper(int i) {
            IntPair ip = new IntPair(Up(i), Down(i));
            this.possibleJumperFeasibleIntervals[i] = ip;

            InsertJumper(ip.x, ip.y, i);
        }

        public void InsertJumper(int upLayer, int lowLayer, int jumper) {
            int jumperLayer;
            int layerToJumpTo;
            if (CalcJumpInfo(upLayer, lowLayer, jumper, out jumperLayer, out layerToJumpTo))
                this.jumpers.Insert(jumper);
        }

        

        /// <summary>
        /// layerToJumpTo is -1 if there is no jump
        /// </summary>
        /// <param name="upLayer"></param>
        /// <param name="lowLayer"></param>
        /// <param name="jumper"></param>
        /// <param name="jumperLayer"></param>
        /// <param name="layerToJumpTo"></param>
        public bool CalcJumpInfo(int upLayer, int lowLayer, int jumper, out int jumperLayer, out int layerToJumpTo) {
            jumperLayer = layering[jumper];
            layerToJumpTo = -1;
            int min = this.vertsCounts[jumperLayer] - 2*nodeCount[jumper];
            // jump makes sense if some layer has less than min vertices
            for (int i = upLayer - 1; i > jumperLayer; i--) 
                if (vertsCounts[i] < min) {
                    min = vertsCounts[i];
                    layerToJumpTo = i;
                }
 
            for (int i = jumperLayer - 1; i > lowLayer; i--)
                if (vertsCounts[i] < min) {
                    min = vertsCounts[i];
                    layerToJumpTo = i;
                }
            return layerToJumpTo != -1;
        }
        /// <summary>
        /// Up returns the first infeasible layer up from i that i cannot jump to
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int Up(int i) {
            int ret = Int32.MaxValue;
            //minimum of incoming edge sources layeres
            foreach (PolyIntEdge ie in dag.InEdges(i)) {
                int r = layering[ie.Source] - ie.Separation + 1;
                if (r < ret)
                    ret = r;
            }

            if (ret == Int32.MaxValue)
                ret = layering[i] + 1;//

            return ret;
        }
        /// <summary>
        /// Returns the first infeasible layer down from i that i cannot jump to
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int Down(int i) {
            int ret = -Int32.MaxValue;

            foreach (PolyIntEdge ie in dag.OutEdges(i)) {
                int r = layering[ie.Target] + ie.Separation - 1;
                if (r > ret)
                    ret = r;
            }

            if (ret == -Int32.MaxValue)
                ret = layering[i] - 1;

            return ret;
        }

        public void CalculateLayerCounts() {
            this.vertsCounts = new int[layering.Max() + 1];
            for (int i = 0; i < layering.Length; i++)
                vertsCounts[layering[i]] += nodeCount[i];
        }

        public int ChooseJumper() {
            //just return the first available
            foreach (int jumper in this.jumpers)
                return jumper;

            // Callers (RunInternal) guarantee jumpers.Count > 0 before calling.
            // Throw instead of Debug.Assert so the empty-set error path is
            // observable on all build configurations and cannot silently return a
            // fake node (which would corrupt the layering).
            throw new InvalidOperationException("there are no jumpers to choose");
        }
    }
}
