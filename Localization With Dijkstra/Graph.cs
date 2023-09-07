using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localization_With_Dijkstra
{
    class Graph
    {
        private DataTable _dt;
        public DataTable DT
        {
            get { return _dt; }
            set { _dt = value; }
        }

        int Size;
        int[][] a;
        bool[] Visited;
        bool[] ToBeChecked;
        int[] CurDist;
        int[] Pred;

        public Graph()
        {
            Size = 0;
        }
        public Graph(int s)
        {
            _dt = new DataTable();
            _dt.Columns.Add("node number", typeof(int));
            _dt.Columns.Add("distance", typeof(int));
            _dt.Columns.Add("pred", typeof(string));

            Size = s;

            a = new int[Size][];
            for (int i = 0; i < Size; i++)
            {
                a[i] = new int[Size];
            }
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                    a[i][j] = 0;
            }
            Visited = new bool[Size];
            ToBeChecked = new bool[Size];
            CurDist = new int[Size];
            Pred = new int[Size];
            for (int i = 0; i < Size; i++)
                Visited[i] = false;
        }

        public void AddEdge(int start, int end, int value)
        {
            a[start][end] = value;
        }

        public void RemoveEdge(int start, int end)
        {
            a[start][end] = 0;
        }

  

        public void Dijkstra(int first)
        {
            int SomeNodesLeft = Size;
            for (int i = 0; i < Size; i++)
            {
                CurDist[i] = int.MaxValue;
                ToBeChecked[i] = false;
            }
            CurDist[first] = 0;
            while (SomeNodesLeft != 1)
            {
                int i = 0;
                while (ToBeChecked[i] == true)
                    i++;

                int minDist = CurDist[i]; int minIndex = i;
                for (int j = i + 1; j < Size; j++)
                    if (ToBeChecked[j] == false && CurDist[j] < minDist)
                    {
                        minDist = CurDist[j];
                        minIndex = j;
                    }

                int v = minIndex;
                ToBeChecked[v] = true;
                for (int u = 0; u < Size; u++)
                    if (a[v][u] != 0 && ToBeChecked[u] == false)
                        if (CurDist[u] > CurDist[v] + a[v][u])
                        {
                            CurDist[u] = CurDist[v] + a[v][u];
                            Pred[u] = v;
                        }
                SomeNodesLeft--;
            }
        }

        public void Path(int number, string direction, string notes)
        {
            DataRow dr = _dt.NewRow();

            dr[0] = number;
            dr[1] = CurDist[number];
            string s = "";
            while (number != 0)
            {
                s += Pred[number] + ",";
                number = Pred[number];
            }
            dr[2] = s;


            _dt.Rows.Add(dr);
        }
    }
}