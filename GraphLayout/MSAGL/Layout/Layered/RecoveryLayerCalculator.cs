﻿using Microsoft.Msagl.Core.GraphAlgorithms;

namespace Microsoft.Msagl.Layout.Layered {
    public class RecoveryLayerCalculator : LayerCalculator {
        LayerArrays layers;

        public RecoveryLayerCalculator(LayerArrays recoveredLayerArrays) {
            layers=recoveredLayerArrays;
        }
        public int[] GetLayers() {
            return layers.Y;
        }
    }
}