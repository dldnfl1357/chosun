using System.Collections.Generic;

namespace Roller.Pathfinding
{
    /// <summary>
    /// 작은 우선순위(min-heap) 큐. .NET PriorityQueue가 없는 환경 대비 자체 구현.
    /// 더 작은 priority가 먼저 Dequeue 됨.
    /// </summary>
    public class SimplePriorityQueue<TElement>
    {
        readonly List<(TElement element, int priority)> _heap = new List<(TElement, int)>();

        public int Count => _heap.Count;

        public void Enqueue(TElement element, int priority)
        {
            _heap.Add((element, priority));
            SiftUp(_heap.Count - 1);
        }

        public TElement Dequeue()
        {
            if (_heap.Count == 0) throw new System.InvalidOperationException("Queue empty");
            var top = _heap[0];
            int last = _heap.Count - 1;
            _heap[0] = _heap[last];
            _heap.RemoveAt(last);
            if (_heap.Count > 0) SiftDown(0);
            return top.element;
        }

        public void Clear() => _heap.Clear();

        void SiftUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (_heap[i].priority < _heap[parent].priority)
                {
                    (_heap[i], _heap[parent]) = (_heap[parent], _heap[i]);
                    i = parent;
                }
                else break;
            }
        }

        void SiftDown(int i)
        {
            int n = _heap.Count;
            while (true)
            {
                int left = 2 * i + 1;
                int right = 2 * i + 2;
                int smallest = i;
                if (left < n && _heap[left].priority < _heap[smallest].priority) smallest = left;
                if (right < n && _heap[right].priority < _heap[smallest].priority) smallest = right;
                if (smallest == i) break;
                (_heap[i], _heap[smallest]) = (_heap[smallest], _heap[i]);
                i = smallest;
            }
        }
    }
}
