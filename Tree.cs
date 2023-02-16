using System;
using System.Collections.Generic;
using System.Text;

namespace lab3
{
    class Tree
    {
        TreeNode Head;

        public Tree()
        {
            Head = new TreeNode('a', 'z');
        }

        public List<(int, int, int)> GetInfo(string word)
        {
            return Search(ref Head, ref word, 0);
        }

        public void InsertWord(string word, int fileIndex, int row, int col)
        {
            BuildTree(ref Head, ref word, 0, 'a', 'z', ref fileIndex, ref row, ref col);
        }

        void BuildTree(ref TreeNode node, ref string word, int pos, char leftBound, char rightBound,
                       ref int fileIndex, ref int row, ref int column)
        {
            if (node == null)
            {
                node = new TreeNode(leftBound, rightBound);
            }

            if (node.IsLeaf)
            {
                if (word.Length != pos + 1)
                {
                    BuildTree(ref node.GetLeft(), ref word, pos + 1, 'a', 'z',
                              ref fileIndex, ref row, ref column);
                }
                else
                {
                    node.GetInfo().Add((fileIndex, row, column));
                }
            }
            else
            {
                char middle = Convert.ToChar((leftBound + rightBound) / 2);
                if (word[pos] <= middle)
                {
                    BuildTree(ref node.GetLeft(), ref word, pos, leftBound, middle,
                              ref fileIndex, ref row, ref column);
                }
                else
                {
                    BuildTree(ref node.GetRight(), ref word, pos, Convert.ToChar(middle + 1), rightBound,
                              ref fileIndex, ref row, ref column);
                }
            }
        }

        List<(int, int, int)> Search(ref TreeNode node, ref string word, int pos)
        {
            if (node == null)
            {
                return null;
            }

            if (node.IsLeaf)
            {
                if (word.Length != pos + 1)
                {
                    return Search(ref node.GetLeft(), ref word, pos + 1);
                }
                else
                {
                    return node.GetInfo();
                }
            }
            else
            {
                char middle = Convert.ToChar((node.LeftBound + node.RightBound) / 2);
                if (word[pos] <= middle)
                {
                    return Search(ref node.GetLeft(), ref word, pos);
                }
                else
                {
                    return Search(ref node.GetRight(), ref word, pos);
                }
            }
        }
    }

    class TreeNode
    {
        TreeNode _left, _right;
        List<(int, int, int)> _info;

        public char LeftBound { get; }

        public char RightBound { get; }

        public bool IsLeaf { get; }

        public TreeNode(char leftBound, char rightBound)
        {
            LeftBound = leftBound;
            RightBound = rightBound;
            IsLeaf = (leftBound == rightBound);
            if (IsLeaf)
            {
                _info = new List<(int, int, int)>();
            }
        }

        public ref TreeNode GetLeft()
        {
            return ref _left;
        }

        public ref TreeNode GetRight()
        {
            return ref _right;
        }

        public ref List<(int, int, int)> GetInfo()
        {
            return ref _info;
        }
    }   
}
