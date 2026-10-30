#region Using directives

using System;
using System.Collections.Generic;

#endregion

namespace Microsoft.Msagl.Core.Geometry.Curves
{

    /// <summary>
    /// An internal node of ParallelogramNodeOverICurve hierarchy.
    /// Is used in curve intersections routines.
    /// </summary>
    [Serializable]
  public class ParallelogramInternalTreeNode : ParallelogramNodeOverICurve
  {
    List<ParallelogramNodeOverICurve> children = new List<ParallelogramNodeOverICurve>();

    public ParallelogramInternalTreeNode(ICurve seg, double leafBoxesOffset):base(seg,leafBoxesOffset)
    {
    }

    /// <summary>
    /// children of the node
    /// </summary>
    /// <value></value>
    public List<ParallelogramNodeOverICurve> Children
    {
      get { return children; }
    }

    public void AddChild(ParallelogramNodeOverICurve node)
    {
      children.Add(node);
    }
  }

}
