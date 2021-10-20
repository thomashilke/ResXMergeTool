using System.Collections.Generic;
using System.Resources;

namespace ResXMergeTool
{
    internal class NodeComparer: IComparer<ResXDataNode>
    {
        private readonly IDictionary<string, int> _sortOrder;

        public NodeComparer(IDictionary<string, int> sortOrder)
        {
            this._sortOrder = sortOrder;
        }

        public int Compare(ResXDataNode node1, ResXDataNode node2)
        {
            var node1InDictionary = _sortOrder.ContainsKey(node1.Name);
            var node2InDictionary = _sortOrder.ContainsKey(node2.Name);

            if (node1InDictionary && node2InDictionary)
            {
                return _sortOrder[node1.Name].CompareTo(_sortOrder[node2.Name]);
            }
            else if (node1InDictionary && !node2InDictionary)
            {
                return -1;
            }
            else if (!node1InDictionary && node2InDictionary)
            {
                return 1;
            }
            else
            {
                return node1.Name.CompareTo(node2.Name);
            }
        }
    }
}