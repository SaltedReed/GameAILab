using System;

namespace GameAILab.Core
{

	public class BinaryHeap<TEle>
	{
        public int Count { get; private set; }

        // Capacity = m_eles.Length - 1
        public int Capacity { get; private set; }

        public Comparison<TEle> comparison;

        // index starts from 1
        private TEle[] m_eles;


        public BinaryHeap(int capacity, Comparison<TEle> comparison)
        {
            Init(capacity, comparison);
        }

        private void Init(int capacity, Comparison<TEle> comparison)
        {
            this.comparison = comparison;
            Count = 0;
            Capacity = capacity;
            m_eles = new TEle[capacity + 1];
            if (m_eles is null)
            {
                throw new OutOfMemoryException();
            }
        }

        public void Add(TEle ele)
        {
            if (Count == Capacity)
            {
                GrowCapacity(Capacity);
            }

            m_eles[++Count] = ele;
            Swim(Count);
        }

        public TEle RemoveTop()
        {
            TEle result = m_eles[1];

            Exch(1, Count);
            Count--;
            Sink(1);

            return result;
        }

        public void Clear()
        {
            Count = 0;
        }

        private void Swim(int k)
        {
            while (k > 1 && comparison(m_eles[(int)(k*0.5f)], m_eles[k])<0)
            {
                Exch((int)(k * 0.5f), k);
                k = (int)(k * 0.5f);
            }
        }

        private void Sink(int k)
        {
            while (2 * k <= Count)
            {
                int j = 2 * k;
                if (j < Count && comparison(m_eles[j], m_eles[j + 1])<0)
                {
                    j++;
                }

                if (comparison(m_eles[k], m_eles[j])<0) 
                {
                    Exch(k, j); 
                    k = j; 
                }
                else
                {
                    break;
                }
            }
        }

        private void GrowCapacity(int count)
        {
            TEle[] arr = new TEle[Capacity + count + 1];
            if (arr is null)
            {
                throw new OutOfMemoryException();
            }

            m_eles.CopyTo(arr, 0);
            m_eles = arr;

            Capacity += count;
        }

        private void Exch(int i1, int i2)
        {
            TEle ele = m_eles[i1];
            m_eles[i1] = m_eles[i2];
            m_eles[i2] = ele;
        }
    
        public string DebugString()
        {
            string str = "";
            str += "capacity: " + Capacity + "\n";
            str += "count: " + Count + "\n";
            str += "elements: \n";
            for (int i = 1; i <= Count; ++i)
            {
                str += m_eles[i].ToString() + " | ";
            }
            return str;
        }

    }

}
