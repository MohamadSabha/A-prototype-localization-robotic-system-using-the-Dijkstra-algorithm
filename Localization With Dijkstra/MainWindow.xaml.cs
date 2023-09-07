using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using System.Diagnostics;
using System.Data;

namespace Localization_With_Dijkstra
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Main Initialization
        Brick _Brik;                                    // To use the EV3 liprary 
        int Forword = 60;                               // The Power of motor (speed)
        int Backword = -60;                             // The Power of motor (speed)
        int Left_Obstacle;                              // Obstacle on the left
        int Right_Obstacle;                             // Obstacle on the right

        int Node_Distance = 0;                          // Distance between a node and the previous node for the Graph
        int Fire_Counter = 0;                           // Node number
        bool Looping;                                   // For the whil loop in the obstacle_avoidence method

        uint Time = 850;                                // Equals 90 degree on the left or the right

        int Obstacle;                                   // The distance to obstacle
        string Direction = "start";
        string Node_notes = "";
        int Orient_Direction = 90;                      // Orientation in Axis (Initial axis value)
        int Rotate_Angle = 90;                          // To calculate the orientation
        int Main_Orient_Counter = 0;                    // Distance between a node and the previous node to determine the value for the node_distance for each node
                                                        
        const double Deg = Math.PI / 180;               // (3.141F / 180)
        int COS;                                        
        int SIN;                                        
                                                        
        float Rotation;                                 // For scaling in the map
                                                        
        List<Point> Point_ArrList = new List<Point>();  // To Know the location of the robot
        ArrayList Node_ArrList = new ArrayList();       // To stor the nodes information
                                                        
        Map_Window M = new Map_Window();                // An object to use the map window        
        Graph G = new Graph();                          // An object to use Graph class
        Point[] p_array = new Point[1000000];           // To Know the location of the robot
        #endregion

        #region Startup Program
        private async void Windows_Loaded(object sender, RoutedEventArgs e)
        {
            _Brik = new Brick(new BluetoothCommunication("COM14"));
            _Brik.BrickChanged += _brik_BrickChanged;
            await _Brik.ConnectAsync();
            await _Brik.DirectCommand.PlayToneAsync(100, 1000, 300);
            await _Brik.DirectCommand.SetMotorPolarityAsync(OutputPort.B | OutputPort.C, Polarity.Forward);
            await _Brik.DirectCommand.StopMotorAsync(OutputPort.All, false);

            INERTIAL_textBox.Text = "0";
            await _Brik.DirectCommand.ClearAllDevicesAsync();
        }

        private async void _brik_BrickChanged(object sender, BrickChangedEventArgs e)
        {
            txtdistance.Text = e.Ports[InputPort.Four].SIValue.ToString();
        }
        #endregion

        #region All Buttons
        #region Manual Navigation Buttons
        private async void Forword_Butten(object sender, RoutedEventArgs e)
        {
            await _Brik.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, Forword, Time, true);
        }

        private async void Left_Button(object sender, RoutedEventArgs e)
        {
            move_left();
        }

        private async void right_button(object sender, RoutedEventArgs e)
        {
            move_right();
        }

        private async void Backword_Button(object sender, RoutedEventArgs e)
        {
            await _Brik.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, Backword, Time, true);
        }
        #endregion

        #region START Button (Automatic Navigation)
        private void Start_button_Click(object sender, RoutedEventArgs e)
        {
            Looping = true;
            obstacle_avoidence();
        }
        #endregion

        #region STOP Button
        private async void stop_button_Click(object sender, RoutedEventArgs e)
        {
            await _Brik.DirectCommand.StopMotorAsync(OutputPort.All, true);
            Looping = false;
            Fire_Counter++;

            Line End_Line = new Line();
            End_Line.Stroke = System.Windows.Media.Brushes.Red;
            End_Line.StrokeThickness = 20;
            End_Line.Fill = System.Windows.Media.Brushes.Red;
            End_Line.Opacity = 100;

            DrawNodeLine(COS, SIN, Main_Orient_Counter, End_Line);

            Direction = "Stop";


            Add_Node(Direction, Node_Distance, Fire_Counter, p_array[Main_Orient_Counter].X, p_array[Main_Orient_Counter].Y,"");
            node_text_drawing(Fire_Counter, Node_Distance);
            Node_Distance = 0;
        }
        #endregion

        #region Back Button
        private async void back_button_Click(object sender, RoutedEventArgs e)
        {
            #region Graph
            G = new Graph(Node_ArrList.Count);
            for (int i = 0; i < Node_ArrList.Count; i++)
            {
                node temp = (node)Node_ArrList[i];

                int distanc;
                if (i == Node_ArrList.Count - 1)
                {
                    node previos_node = (node)Node_ArrList[i-1];
                    int start = temp.Node_Number;
                    int end = previos_node.Node_Number;
                    distanc = temp.Node_Oriant_Counter;
                    G.AddEdge(start, end, distanc);
                }
                else
                {
                    node next_node = (node)Node_ArrList[i + 1];
                    int start = temp.Node_Number;
                    int end = next_node.Node_Number;
                    distanc = next_node.Node_Oriant_Counter;
                    G.AddEdge(start, end, distanc);
                }
            }
            #endregion

            move_right();
            System.Threading.Thread.Sleep(1000);
            move_right();
            int listcount = Node_ArrList.Count;
            Main_Orient_Counter--;


            for (int i = 0; i < listcount; i++)
            {
                node Prev_node = (node)Node_ArrList[i];


                if (p_array[Main_Orient_Counter].X == Prev_node.Node_X && p_array[Main_Orient_Counter].Y == Prev_node.Node_Y)
                {


                    if (Prev_node.Node_Direction == "left")
                    {
                        System.Threading.Thread.Sleep(1000);
                        move_left();
                        System.Threading.Thread.Sleep(1000);
                    }

                    else if (Prev_node.Node_Direction == "right")
                    {
                        System.Threading.Thread.Sleep(1000);
                        move_right();
                        System.Threading.Thread.Sleep(1000);
                    }

                 

                    else if (Prev_node.Node_Number == 0)
                    {
                        break;
                    }
                    else
                    {
                        while (Prev_node.Node_Oriant_Counter > 0)
                        {
                            await _Brik.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, Forword, 30, false);
                            System.Threading.Thread.Sleep(20);

                            Line backline = new Line();
                            backline.Stroke = System.Windows.Media.Brushes.Yellow;
                            backline.Fill = System.Windows.Media.Brushes.Yellow;
                            backline.Opacity = 100;
                            backline.StrokeThickness = 10;

                            backline.X1 = p_array[Main_Orient_Counter].X;
                            backline.Y1 = p_array[Main_Orient_Counter].Y;
                            backline.X2 = p_array[Main_Orient_Counter - 1].X;
                            backline.Y2 = p_array[Main_Orient_Counter - 1].Y;
                            M.grid.Children.Add(backline);

                            Prev_node.Node_Oriant_Counter--;
                            Main_Orient_Counter--;
                        }
                    }
                }
            }
        }
        #endregion

        #region Shortest_Path Button
        private void Shortest_Path_Click(object sender, RoutedEventArgs e)
        {
            G.Dijkstra(0);
            for (int i = 0; i < Node_ArrList.Count; i++)
            {
                node n = (node)Node_ArrList[i];
                G.Path(i, n.Node_Direction,n.Notes);
            }
            graph_grid.ItemsSource = G.DT.DefaultView;
        }
        #endregion

        #region PlaySound Button
        private async void PlaySound_Click(object sender, RoutedEventArgs e)
        {
            await _Brik.SystemCommand.CopyFileAsync("warning.rsf", "../prjs/new/warning.rsf");
            await _Brik.DirectCommand.PlaySoundAsync(100, "../prjs/new/warning");
        }
        #endregion
        #endregion

        #region All methods
        public async void obstacle_avoidence()
        {
            M.Show(); // Display the map
            while (Looping == true)
            {
                INERTIAL_textBox.Text = Convert.ToString(Main_Orient_Counter);
                Rotation = 50;
                Obstacle = Convert.ToInt32(_Brik.Ports[InputPort.Four].SIValue.ToString());

                if (Obstacle > 18) //check if obstacle within 18cm
                {
                    #region There is "NO" obstacle
                    await _Brik.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, Forword, 30, false);
                    System.Threading.Thread.Sleep(20);

                    Line drawingline = new Line();
                    drawingline.Stroke = System.Windows.Media.Brushes.Black;
                    drawingline.Fill = System.Windows.Media.Brushes.Black;
                    drawingline.Opacity = 100;
                    drawingline.StrokeThickness = 20;

                    #region Setting the default start point & Drawing the start node and add it to the list "One Time Use"
                    if (Main_Orient_Counter == 0)
                    {
                        Add_Point(Main_Orient_Counter, Rotation, Orient_Direction);
                        Add_Node(Direction, Node_Distance, Fire_Counter, p_array[Main_Orient_Counter].X, p_array[Main_Orient_Counter].Y,"");

                        // Drawing the first oriantation
                        Main_Orient_Counter++;
                        Node_Distance++;

                        Add_Point(Main_Orient_Counter, Rotation, Orient_Direction);

                        drawingline.X1 = p_array[Main_Orient_Counter - 1].X;
                        drawingline.Y1 = p_array[Main_Orient_Counter - 1].Y;
                        drawingline.X2 = p_array[Main_Orient_Counter].X;
                        drawingline.Y2 = p_array[Main_Orient_Counter].Y;
                        M.grid.Children.Add(drawingline);

                        // Drawing the first node 
                        Line Start_Line = new Line();
                        Start_Line.Stroke = System.Windows.Media.Brushes.Green;
                        Start_Line.StrokeThickness = 20;
                        Start_Line.Fill = System.Windows.Media.Brushes.Green;
                        Start_Line.Opacity = 100;
                        DrawNodeLine(0, -1, 1, Start_Line);

                        node_text_drawing(Fire_Counter, Node_Distance);
                        Node_Distance = 0;
                    }
                    #endregion

                    #region Drawing the path line
                    else
                    {
                        Main_Orient_Counter++;
                        Node_Distance++;
                        Add_Point(Main_Orient_Counter, Rotation, Orient_Direction);

                        drawingline.X1 = p_array[Main_Orient_Counter - 1].X;
                        drawingline.Y1 = p_array[Main_Orient_Counter - 1].Y;
                        drawingline.X2 = p_array[Main_Orient_Counter].X;
                        drawingline.Y2 = p_array[Main_Orient_Counter].Y;
                        M.grid.Children.Add(drawingline);

                        int listcount = Node_ArrList.Count;
                        for (int i = 0; i < listcount; i++)
                        {
                            node Prev_node = (node)Node_ArrList[i];


                            if (p_array[Main_Orient_Counter].X == Prev_node.Node_X && p_array[Main_Orient_Counter].Y == Prev_node.Node_Y)
                            {
                               
                                if (Prev_node.Node_Direction == "right")
                                {
                                    
                                    if (Prev_node.Notes == "crossway")
                                    {
                                        node Current_node = new node();
                                        Current_node.Node_Oriant_Counter = Node_Distance;
                                        Current_node.Node_Number = Prev_node.Node_Number;
                                        Current_node.Node_X = Prev_node.Node_X;
                                        Current_node.Node_Y = Prev_node.Node_Y;
                                        Prev_node.Node_Direction = "left";
                                        Current_node.Node_Direction = Prev_node.Node_Direction;
                                        Node_ArrList.Add(Current_node);
                                        Node_Distance = 0;

                                        break;
                                    }
                                    else
                                    {
                                        node Current_node = new node();
                                        Current_node.Node_Oriant_Counter = Node_Distance;
                                        Current_node.Node_Number = Prev_node.Node_Number;
                                        Current_node.Node_X = Prev_node.Node_X;
                                        Current_node.Node_Y = Prev_node.Node_Y;
                                        Prev_node.Node_Direction = Prev_node.Node_Direction;
                                        Current_node.Node_Direction = Prev_node.Node_Direction;
                                        Node_ArrList.Add(Current_node);
                                        Node_Distance = 0;

                                        Orient_Direction -= Rotate_Angle;
                                        System.Threading.Thread.Sleep(1000);
                                        move_right();
                                        System.Threading.Thread.Sleep(1000);
                                        break;

                                    }
                                }
                                else if (Prev_node.Node_Direction == "left")
                                {
                                    if (Prev_node.Notes == "crossway")
                                    {
                                        node Current_node = new node();
                                        Current_node.Node_Oriant_Counter = Node_Distance;
                                        Current_node.Node_Number = Prev_node.Node_Number;
                                        Current_node.Node_X = Prev_node.Node_X;
                                        Current_node.Node_Y = Prev_node.Node_Y;
                                        Prev_node.Node_Direction = "right";
                                        Current_node.Node_Direction = Prev_node.Node_Direction;
                                        Node_ArrList.Add(Current_node);
                                        Node_Distance = 0;

                                        break;
                                    }
                                    else
                                    {

                                        node Current_node = new node();
                                        Current_node.Node_Oriant_Counter = Node_Distance;
                                        Current_node.Node_Number = Prev_node.Node_Number;
                                        Current_node.Node_X = Prev_node.Node_X;
                                        Current_node.Node_Y = Prev_node.Node_Y;
                                        Prev_node.Node_Direction = Prev_node.Node_Direction;
                                        Current_node.Node_Direction = Prev_node.Node_Direction;
                                        Node_ArrList.Add(Current_node);
                                        Node_Distance = 0;

                                        Orient_Direction += Rotate_Angle;
                                        System.Threading.Thread.Sleep(1000);
                                        move_left();
                                        System.Threading.Thread.Sleep(1000);
                                        break;

                                    }

                                }
                               
                                else if (Prev_node.Node_Direction == "start")
                                {
                                    Looping = false;
                                }
                            }
                        }
                    }
                    #endregion
                    #endregion
                }
                else
                {
                    #region There is "AN" Obstacle
                    await _Brik.DirectCommand.PlayToneAsync(100, 1000, 100);
                    System.Threading.Thread.Sleep(150);
                    await _Brik.DirectCommand.PlayToneAsync(100, 1000, 100);
                    await _Brik.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, false);

                    Line OBCTACLE_LINE = new Line();
                    OBCTACLE_LINE.Stroke = System.Windows.Media.Brushes.Blue;
                    OBCTACLE_LINE.StrokeThickness = 20;
                    OBCTACLE_LINE.Fill = System.Windows.Media.Brushes.Blue;
                    OBCTACLE_LINE.Opacity = 100;

                    DrawNodeLine(COS, SIN, Main_Orient_Counter, OBCTACLE_LINE);

                    #region Check Right and Left
                    rotate_right();
                    System.Threading.Thread.Sleep(1000);
                    Obstacle = Convert.ToInt32(_Brik.Ports[InputPort.Four].SIValue.ToString());
                    Right_Obstacle = Obstacle;

                    rotate_left();
                    System.Threading.Thread.Sleep(1000);

                    rotate_left();
                    System.Threading.Thread.Sleep(1000);
                    Obstacle = Convert.ToInt32(_Brik.Ports[InputPort.Four].SIValue.ToString());
                    Left_Obstacle = Obstacle;

                    rotate_right();
                    System.Threading.Thread.Sleep(1000);
                    #endregion

                    #region Making decisions
                    if (Right_Obstacle >= Left_Obstacle && Right_Obstacle > 11)
                    {
                        Fire_Counter++;

                        move_right();
                        System.Threading.Thread.Sleep(1000);
                        Orient_Direction -= Rotate_Angle;
                        Direction = "left";

                        if (Left_Obstacle > 11)
                        {
                            Node_notes = "crossway";
                        }
                      
                        Add_Node(Direction,Node_Distance, Fire_Counter, p_array[Main_Orient_Counter].X, p_array[Main_Orient_Counter].Y, Node_notes);
                        node_text_drawing(Fire_Counter, Node_Distance);
                        Node_Distance = 0;
                    }
                    else if (Left_Obstacle >= Right_Obstacle && Left_Obstacle > 11)
                    {
                        Fire_Counter++;

                        move_left();
                        System.Threading.Thread.Sleep(1000);
                        Orient_Direction += Rotate_Angle;
                        Direction = "right";
                        if (Right_Obstacle > 11)
                        {
                            Node_notes = "crossway";
                        }

                        Add_Node(Direction, Node_Distance, Fire_Counter, p_array[Main_Orient_Counter].X, p_array[Main_Orient_Counter].Y,Node_notes);
                        node_text_drawing(Fire_Counter, Node_Distance);
                        Node_Distance = 0;

                    }
                    else
                    {
                        Fire_Counter++;

                        move_right();
                        System.Threading.Thread.Sleep(1000);
                        move_right();
                        Orient_Direction += 180;
                        Direction = "DeadEnd";

                        Add_Node(Direction, Node_Distance, Fire_Counter, p_array[Main_Orient_Counter].X, p_array[Main_Orient_Counter].Y,"");
                        node_text_drawing(Fire_Counter, Node_Distance);
                        Node_Distance = 0;
                    }
                    #endregion

                    System.Threading.Thread.Sleep(1000);
                    Obstacle = Convert.ToInt32(_Brik.Ports[InputPort.Four].SIValue.ToString());
                    Node_notes = "";

                    #endregion
                }
            }
        }

        public async void move_right()
        {
            _Brik.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, Forword, Time, false);
            _Brik.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, Backword, Time, false);
            await _Brik.BatchCommand.SendCommandAsync();
        }

        public async void move_left()
        {
            _Brik.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, Backword, Time, false);
            _Brik.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, Forword, Time, false);
            await _Brik.BatchCommand.SendCommandAsync();
        }

        public async void rotate_right()
        {
            await _Brik.DirectCommand.TurnMotorAtSpeedForTimeAsync(OutputPort.D, 20, 300, false);
        }

        public async void rotate_left()
        {
            await _Brik.DirectCommand.TurnMotorAtSpeedForTimeAsync(OutputPort.D, -20, 300, false);
        }

        public void Cos_Sin(int X)
        {
            COS = Convert.ToInt32(Math.Round(Math.Cos(X * Deg), 4));
            SIN = Convert.ToInt32(Math.Round(Math.Sin(X * Deg), 4));
        }

        private void node_text_drawing(int x, int y)
        {
            // Create a textbox
            #region Create a textbox to view the name of first node at runtime
            TextBox nodetxt = new TextBox();
            nodetxt.Name = "textBox" + x + "";
            nodetxt.Text = "node #:" + Convert.ToString(x) + "\ndistance=" + Convert.ToString(y) + "";
            M.canvas_fire.Children.Add(nodetxt);
            #endregion

            #region Create a render transformation to manipulate the textbox shape
            nodetxt.RenderTransformOrigin = new Point(0.5, 0.5);

            ScaleTransform myScaleTransform = new ScaleTransform();
            myScaleTransform.ScaleX = -1;
            RotateTransform myRotateTransform = new RotateTransform();
            myRotateTransform.Angle = 180;
            #endregion

            #region Add all tranforms to a transformation group
            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myScaleTransform);
            myTransformGroup.Children.Add(myRotateTransform);
            // Associate the transforms to the object 
            nodetxt.RenderTransform = myTransformGroup;
            #endregion

            #region Put the textbox in the exact position
            if (x == 0)
            {
                // Put the textbox in the exact position of the first node
                double txtx = p_array[Main_Orient_Counter].X;
                double txty = p_array[Main_Orient_Counter].Y;
                txtx += 100;
                txty -= 500;
                Canvas.SetLeft(nodetxt, txtx);
                Canvas.SetTop(nodetxt, txty);
            }
            else
            {
                if (COS == 0 && SIN == 1)       // Direction Y+
                {
                    double txtx = p_array[Main_Orient_Counter - 1].X;
                    double txty = p_array[Main_Orient_Counter - 1].Y;
                    txtx += 50;
                    txty -= 400;
                    Canvas.SetLeft(nodetxt, txtx);
                    Canvas.SetTop(nodetxt, txty);
                }
                else if (COS == 1 && SIN == 0)       // Direction X+
                {
                    double txtx = p_array[Main_Orient_Counter - 1].X;
                    double txty = p_array[Main_Orient_Counter - 1].Y;
                    txtx += 50;
                    txty -= 400;
                    Canvas.SetLeft(nodetxt, txtx);
                    Canvas.SetTop(nodetxt, txty);
                }
                else if (COS == -1 && SIN == 0) // Direction X-
                {
                    double txtx = p_array[Main_Orient_Counter - 1].X;
                    double txty = p_array[Main_Orient_Counter - 1].Y;
                    txtx += 50;
                    txty -= 400;
                    Canvas.SetLeft(nodetxt, txtx);
                    Canvas.SetTop(nodetxt, txty);
                }
                else if (COS == 0 && SIN == -1) // Direction Y-
                {
                    double txtx = p_array[Main_Orient_Counter - 1].X;
                    double txty = p_array[Main_Orient_Counter - 1].Y;
                    txtx += 100;
                    txty -= 500;
                    Canvas.SetLeft(nodetxt, txtx);
                    Canvas.SetTop(nodetxt, txty);
                }
            }
            #endregion
        }

        private void Add_Node (string direction,int NodeDistance, int FireCunter, double X, double Y, string node_notes)
        {
            node node = new node();
            node.Node_Direction = direction;
            node.Node_Oriant_Counter = NodeDistance;
            node.Node_Number = FireCunter;
            node.Node_X = X;
            node.Node_Y = Y;
            node.Notes = node_notes;
            Node_ArrList.Add(node);
        }

        private void Add_Point(int main_orient_counter, float rotation, int orient_direction)
        {
            Cos_Sin(orient_direction);
            if (main_orient_counter == 0)
            {
                p_array[main_orient_counter].X = 1175;
                p_array[main_orient_counter].Y = 200;
            }
            else
            {
                p_array[main_orient_counter].X = (p_array[main_orient_counter - 1].X) + (rotation * COS) / 50;
                p_array[main_orient_counter].Y = (p_array[main_orient_counter - 1].Y) + (rotation * SIN) / 50;
            }
        }

        private void DrawNodeLine(int cos, int sin, int OC, Line line)
        {
            if (cos == 1 && sin == 0)       // Direction X+
            {
                line.X1 = p_array[OC].X + 20;
                line.Y1 = p_array[OC].Y + 20;
                line.X2 = p_array[OC].X + 20;
                line.Y2 = p_array[OC].Y - 20;

                M.grid.Children.Add(line);
            }
            else if (cos == -1 && sin == 0) // Direction X-
            {
                line.X1 = p_array[OC].X - 20;
                line.Y1 = p_array[OC].Y + 20;
                line.X2 = p_array[OC].X - 20;
                line.Y2 = p_array[OC].Y - 20;

                M.grid.Children.Add(line);
            }

            else if (cos == 0 && sin == 1) // Direction Y+
            {

                line.X1 = p_array[OC].X - 20;
                line.Y1 = p_array[OC].Y + 20;
                line.X2 = p_array[OC].X + 20;
                line.Y2 = p_array[OC].Y + 20;

                M.grid.Children.Add(line);
            }
            else if (cos == 0 && sin == -1) // Direction Y-
            {
                line.X1 = p_array[OC].X - 20;
                line.Y1 = p_array[OC].Y - 20;
                line.X2 = p_array[OC].X + 20;
                line.Y2 = p_array[OC].Y - 20;

                M.grid.Children.Add(line);
            }
        }
        #endregion

        #region UI Text Boxes
        private void Distance_TextBox(object sender, TextChangedEventArgs e)
        {

        }

        private void Inertial_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        #endregion
    }
}