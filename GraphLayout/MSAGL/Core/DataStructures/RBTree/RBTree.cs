using System;
using System.Collections.Generic;

namespace Microsoft.Msagl.Core.DataStructures {

    [Serializable]
    public class RbTree<T> : IEnumerable<T> {
  
        /// <summary>
        /// find the first, minimal, node in the tree such that predicate holds
        /// </summary>
        /// <param name="predicate">Has to be monotone in the sense that if it holds for t then it holds for any t' greater or equal than t
        /// so the predicate values have a form (false, false, ..., false, true, true, ..., true)
        /// </param>
        /// <returns>the first node where predicate holds or null</returns>
        public RBNode<T> FindFirst(Func<T, bool> predicate) {
            return FindFirst(root, predicate);
        }

        RBNode<T> FindFirst(RBNode<T> n, Func<T, bool> p) {
            if (n == nil)
                return null;
            RBNode<T> good = null;
            while (n != nil)
                n = p(n.Item) ? (good = n).left : n.right;

            return good;
        }

        /// <summary>
        /// find the last, maximal, node in the tree such that predicate holds
        /// </summary>
        /// <param name="predicate">Has to be monotone in the sense that if it holds for t then it holds for any t' less or equal than t
        /// so the predicate values on the tree have a form (true, true, ..., true, false, false, ..., false)
        /// </param>
        /// <returns>the last node where predicate holds or null</returns>
        public RBNode<T> FindLast(Func<T,bool> predicate) {
            return FindLast(root, predicate);
        }

        RBNode<T> FindLast(RBNode<T> n, Func<T,bool> p) {
            if (n == nil)
                return null;
            RBNode<T> good = null;
            while (n != nil)
                n = p(n.Item) ? (good = n).right : n.left;

            return good;
        }


        readonly IComparer<T> comparer;

        IComparer<T> Comparer {
            get { return comparer; }
        }

        public IEnumerator<T> GetEnumerator() { return new RBTreeEnumerator<T>(this); }

        RBNode<T> nil;

        /// <summary>
        /// The nil node of the tree.
        /// </summary>
        public RBNode<T> Nil { get { return nil; } }

        RBNode<T> root;
        /// <summary>
        /// The root node of the tree, or the nil node if the tree is empty.
        /// </summary>
        public RBNode<T> Root { get { return root; } }

        /// <summary>
        /// The successor of the node in the sorted order, or null if the node is the maximum.
        /// </summary>
        public RBNode<T> Next(RBNode<T> x) {
            if (x.right != nil)
                return TreeMinimum(x.right);
            RBNode<T> y = x.parent;
            while (y != nil && x == y.right) {
                x = y;
                y = y.parent;
            }
            return ToNull(y);
        }

        RBNode<T> ToNull(RBNode<T> y) {
            return y != nil ? y : null;
        }

        /// <summary>
        /// The predecessor of the node in the sorted order, or null if the node is the minimum.
        /// </summary>
        public RBNode<T> Previous(RBNode<T> x) {
            if (x.left != nil)
                return TreeMaximum(x.left);
            RBNode<T> y = x.parent;
            while (y != nil && x == y.left) {
                x = y;
                y = y.parent;
            }
            return ToNull(y);
        }

        RBNode<T> TreeMinimum(RBNode<T> x) {
            while (x.left != nil)
                x = x.left;
            return ToNull(x);
        }

        /// <summary>
        /// The minimal node of the tree, or null if the tree is empty.
        /// </summary>
        public RBNode<T> TreeMinimum() {
            return TreeMinimum(root);
        }


        RBNode<T> TreeMaximum(RBNode<T> x) {
            while (x.right != nil)
                x = x.right;
            return ToNull(x);
        }

        /// <summary>
        /// The maximal node of the tree, or null if the tree is empty.
        /// </summary>
        public RBNode<T> TreeMaximum() {
            return TreeMaximum(root);
        }


        public override string ToString() {
            string ret = "{";
            int i = 0;
            foreach (T p in this) {
                ret += p.ToString();
                if (i != count - 1) {
                    ret += ",";
                }

                i++;
            }

            return ret + "}";
        }


        /// <summary>
        /// Deletes the subtree rooted at the given node without adjusting the count.
        /// </summary>
        public RBNode<T> DeleteSubtree(RBNode<T> z) {
            System.Diagnostics.Debug.Assert(z != nil);

            RBNode<T> y;
            if (z.left == nil || z.right == nil) {
                /* y has a nil node as a child */
                y = z;
            } else {
                /* find tree successor with a nil node as a child */
                y = z.right;
                while (y.left != nil) y = y.left;
            }

            /* x is y's only child */
            RBNode<T> x = y.left != nil ? y.left : y.right;

            x.parent = y.parent;
            if (y.parent == nil)
                root = x;
            else {
                if (y == y.parent.left)
                    y.parent.left = x;
                else
                    y.parent.right = x;
            }
            if (y != z)
                z.Item = y.Item;
            if (y.color == RBColor.Black)
                DeleteFixup(x);

            //	checkTheTree();

            return ToNull(z);

        }

        int count;
        public int Count { get { return count; } }

        /// <summary>
        /// Removes the item from the tree, or returns null if the item is not present.
        /// </summary>
        public RBNode<T> Remove(T i) {
            RBNode<T> n = Find(i);
            if (n != null) {
                count--;
                return DeleteSubtree(n);
            }
            return null;
        }

        /// <summary>
        /// Deletes the given node from the tree and decrements the count.
        /// </summary>
        public void DeleteNodeInternal(RBNode<T> x) {
            count--;
            DeleteSubtree(x);
        }

        RBNode<T> Find(RBNode<T> x, T i) {
            int compareResult;
            while (x != nil && (compareResult = Comparer.Compare(i, x.Item)) != 0)
                x = compareResult < 0 ? x.left : x.right;

            return ToNull(x);
        }

        /// <summary>
        /// Finds the node with the given item, or null if the item is not present.
        /// </summary>
        public RBNode<T> Find(T i) {
            return Find(root, i);
        }

        void DeleteFixup(RBNode<T> x) {
            while (x != root && x.color == RBColor.Black) {
                if (x == x.parent.left) {
                    RBNode<T> w = x.parent.right;
                    if (w.color == RBColor.Red) {
                        w.color = RBColor.Black;
                        x.parent.color = RBColor.Red;
                        LeftRotate(x.parent);
                        w = x.parent.right;
                    }
                    if (w.left.color == RBColor.Black && w.right.color == RBColor.Black) {
                        w.color = RBColor.Red;
                        x = x.parent;
                    } else {
                        if (w.right.color == RBColor.Black) {
                            w.left.color = RBColor.Black;
                            w.color = RBColor.Red;
                            RightRotate(w);
                            w = x.parent.right;
                        }
                        w.color = x.parent.color;
                        x.parent.color = RBColor.Black;
                        w.right.color = RBColor.Black;
                        LeftRotate(x.parent);
                        x = root;
                    }
                } else {
                    RBNode<T> w = x.parent.left;
                    if (w.color == RBColor.Red) {
                        w.color = RBColor.Black;
                        x.parent.color = RBColor.Red;
                        RightRotate(x.parent);
                        w = x.parent.left;
                    }
                    if (w.right.color == RBColor.Black && w.left.color == RBColor.Black) {
                        w.color = RBColor.Red;
                        x = x.parent;
                    } else {
                        if (w.left.color == RBColor.Black) {
                            w.right.color = RBColor.Black;
                            w.color = RBColor.Red;
                            LeftRotate(w);
                            w = x.parent.left;
                        }
                        w.color = x.parent.color;
                        x.parent.color = RBColor.Black;
                        w.left.color = RBColor.Black;
                        RightRotate(x.parent);
                        x = root;
                    }
                }
            }
            x.color = RBColor.Black;
        }

        /// <summary>
        /// Returns true if the tree contains no elements.
        /// </summary>
        public bool IsEmpty() { return root == nil; }

        RBNode<T> TreeInsert(T z) {
            var y = nil;
            var x = root;
            var compareRes = 0;
            while (x != nil) {
                y = x;
#if SHARPKIT //https://code.google.com/p/sharpkit/issues/detail?id=368
                compareRes = Comparer.Compare(z, x.Item);
                x = compareRes < 0 ? x.left : x.right;
#else
                x = (compareRes = Comparer.Compare(z, x.Item)) < 0 ? x.left : x.right;
#endif
            }

            var nz = new RBNode<T>(RBColor.Black, z, y, nil, nil);

            if (y == nil)
                root = nz;
            else if (compareRes < 0)
                y.left = nz;
            else
                y.right = nz;

            return ToNull(nz);
        }

        void InsertPrivate(RBNode<T> x) {
            count++;
            x.color = RBColor.Red;
            while (x != root && x.parent.color == RBColor.Red) {
                if (x.parent == x.parent.parent.left) {
                    RBNode<T> y = x.parent.parent.right;
                    if (y.color == RBColor.Red) {
                        x.parent.color = RBColor.Black;
                        y.color = RBColor.Black;
                        x.parent.parent.color = RBColor.Red;
                        x = x.parent.parent;
                    } else {
                        if (x == x.parent.right) {
                            x = x.parent;
                            LeftRotate(x);
                        }
                        x.parent.color = RBColor.Black;
                        x.parent.parent.color = RBColor.Red;
                        RightRotate(x.parent.parent);
                    }
                } else {
                    RBNode<T> y = x.parent.parent.left;
                    if (y.color == RBColor.Red) {
                        x.parent.color = RBColor.Black;
                        y.color = RBColor.Black;
                        x.parent.parent.color = RBColor.Red;
                        x = x.parent.parent;
                    } else {
                        if (x == x.parent.left) {
                            x = x.parent;
                            RightRotate(x);
                        }
                        x.parent.color = RBColor.Black;
                        x.parent.parent.color = RBColor.Red;
                        LeftRotate(x.parent.parent);
                    }
                }

            }

            root.color = RBColor.Black;
        }

        /// <summary>
        /// Inserts the item into the tree and returns the node holding it.
        /// </summary>
        public RBNode<T> Insert(T v) {
            RBNode<T> x = TreeInsert(v);
            InsertPrivate(x);
            return ToNull(x);
        }

        void LeftRotate(RBNode<T> x) {
            RBNode<T> y = x.right;
            x.right = y.left;
            if (y.left != nil)
                y.left.parent = x;
            y.parent = x.parent;
            if (x.parent == nil)
                root = y;
            else if (x == x.parent.left)
                x.parent.left = y;
            else
                x.parent.right = y;

            y.left = x;
            x.parent = y;
        }

        void RightRotate(RBNode<T> x) {
            RBNode<T> y = x.left;
            x.left = y.right;
            if (y.right != nil)
                y.right.parent = x;
            y.parent = x.parent;
            if (x.parent == nil)
                root = y;
            else if (x == x.parent.right)
                x.parent.right = y;
            else
                x.parent.left = y;

            y.right = x;
            x.parent = y;

        }

        /// <summary>
        /// Constructs a tree with a comparison function.
        /// </summary>
        public RbTree(Func<T, T, int> func) : this(new ComparerOnDelegate<T>(func)) {}


        /// <summary>
        /// Constructs a tree with a comparer.
        /// </summary>
        public RbTree(IComparer<T> comparer) {
            Clear();
            this.comparer = comparer;
        }

        /// <summary>
        /// Removes all elements from the tree.
        /// </summary>
        public void Clear() {
            root = nil = new RBNode<T>(RBColor.Black);
            count = 0;
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return new RBTreeEnumerator<T>(this);
        }

        #endregion
    }
}
