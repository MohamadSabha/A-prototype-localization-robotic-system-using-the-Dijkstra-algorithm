using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localization_With_Dijkstra
{
    class node
    {
        private string notes;
        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }

        private double node_x;
        public double Node_X
        {
            get { return node_x; }
            set { node_x = value; }
        }

        private double node_y;
        public double Node_Y
        {
            get { return node_y; }
            set { node_y = value; }
        }

        private int node_oriant_counter;
        public int Node_Oriant_Counter
        {
            get { return node_oriant_counter; }
            set { node_oriant_counter = value; }
        }

        private string node_direction;
        public string Node_Direction
        {
            get { return node_direction; }
            set { node_direction = value; }
        }

        private int node_number;
        public int Node_Number
        {
            get { return node_number; }
            set { node_number = value; }
        }

        public node()
        {
            Node_X = 0;
            Node_Y = 0;
            Node_Oriant_Counter = 0;
            Node_Direction = "";
            Node_Number = 0;
            notes = "";
        }
    }
}
