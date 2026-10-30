﻿namespace Microsoft.Msagl.DebugHelpers.Persistence {
    public class CharStreamElement : CurveStreamElement {
        public CharStreamElement(char ch) {
            Value = ch;
        }

        public char Char { get { return (char)Value; } }
    }
}