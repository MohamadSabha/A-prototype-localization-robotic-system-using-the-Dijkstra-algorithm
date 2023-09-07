using System;
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

namespace bluetooth_project_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        Brick _brik;
        int forword = 60;
        int backword = -60;
        uint time = 1100;
        int obstacle; // the distance to obstacle
        int oriant_direction = 90;        //orientation in degree
        int COS;
        int SIN;
        int rotate_angle = 90;
        int firstcordinates = 0;
        int left;  // 40 degree to the left
        int right; // 40 degree to the right
        float rotation;

        //int[] oriant_array = new int[Math.Max];
        List<int> direction_list = new List<int>();

        Point[] p_array = new Point[2000];
        int oriant_counter = 0;

        double totalnavigaton = 0;

        double motorvalue = 0;


        public MainWindow()
        {
            InitializeComponent();
        }



        private async void Windows_Loaded(object sender, RoutedEventArgs e)
        {



            _brik = new Brick(new BluetoothCommunication("COM17"));

            _brik.BrickChanged += _brik_BrickChanged;
            await _brik.ConnectAsync();
            await _brik.DirectCommand.PlayToneAsync(100, 1000, 300);
            await _brik.DirectCommand.SetMotorPolarityAsync(OutputPort.B | OutputPort.C, Polarity.Forward);
            await _brik.DirectCommand.StopMotorAsync(OutputPort.All, false);

            INERTIAL_textBox.Text = "0";
            await _brik.DirectCommand.ClearAllDevicesAsync();









        }


        private async void _brik_BrickChanged(object sender, BrickChangedEventArgs e)
        {


          





            txtdistance.Text = e.Ports[InputPort.Four].SIValue.ToString();
            INERTIAL_textBox.Text = _brik.Ports[InputPort.B].SIValue.ToString();
            //rotation = _brik.Ports[InputPort.B].SIValue;

           









        }

        private async void forword_butten(object sender, RoutedEventArgs e)

        {
            await _brik.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, forword, time, true);

        }

        private async void left_button(object sender, RoutedEventArgs e)
        {
            _brik.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, backword, time,false);
            _brik.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, forword, time, false);
            await _brik.BatchCommand.SendCommandAsync();
        }

        private async void right_button(object sender, RoutedEventArgs e)
        {
            _brik.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, forword, time, false);
            _brik.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, backword, time, false);
            await _brik.BatchCommand.SendCommandAsync();


        }

        private async void backword_button(object sender, RoutedEventArgs e)
        {
            await _brik.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, backword, time, true);
        }

        private void distance_textBox(object sender, TextChangedEventArgs e)
        {

        }

        private async void button4_Click(object sender, RoutedEventArgs e)
        {
            await _brik.SystemCommand.CopyFileAsync("warning.rsf", "../prjs/new/warning.rsf");
            await _brik.DirectCommand.PlaySoundAsync(100, "../prjs/new/warning");

        }

        private void INERTIAL_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void INERTIAL_textBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void INERTIAL_textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Start_button_Click(object sender, RoutedEventArgs e)
        {
            
                obstacle_avoidence();

            




        }




        public  async void rotate_right()
        {



           await _brik.DirectCommand.TurnMotorAtSpeedForTimeAsync(OutputPort.D, 20, 300, false);
        }
        public async void rotate_left()
        {



         await   _brik.DirectCommand.TurnMotorAtSpeedForTimeAsync(OutputPort.D, -20, 300, false);
        }
        public void position_draw()
        {
            //   Line drawingline = new Line();

            //    double rotation = _brik.Ports[InputPort.B].SIValue;
            //    int  i_orit = 0;
            //    while (i_orit < 5)
            //    {

            //        //Line drawingline = new Line();

            //        //drawingline.Stroke = System.Windows.Media.Brushes.Black; //Umriss 
            //        //drawingline.Fill = System.Windows.Media.Brushes.Black; //Fuellung 


            //        //oriant_direction = oriant_array[i_orit];

            //        p_array[i_orit].X+= rotation * Math.Cos(oriant_direction) / 100;
            //        p_array[i_orit].Y += rotation * Math.Sin(oriant_direction) / 100;

            //        Debug.WriteLine(p_array[i_orit].X );
            //        Debug.WriteLine(p_array[i_orit].Y);
            //        i_orit++;
            //    }
            //    //grid.Children.Add(drawingline);


        }



        private async void stop_button_Click(object sender, RoutedEventArgs e)
        {
            await _brik.DirectCommand.StopMotorAsync(OutputPort.All, false);

        }

        private async void PATHFINDET_CHECKBOX_Checked(object sender, RoutedEventArgs e)
        {





        }
        public async void obstacle_avoidence()
        {
            int fire_counter = 0;
            Map_Window M = new Map_Window();
            M.Show();
           firstcordinates = 0;

            //vector to record the orientation, size=5 
            oriant_counter = 0;
            

            while (oriant_counter < 1500)
            {
                rotation = 45;

                obstacle = Convert.ToInt32(_brik.Ports[InputPort.Four].SIValue.ToString());
              
                if (obstacle > 11)       //check if obstacle within 30cm
                {

                    //await _brik.DirectCommand.ClearAllDevicesAsync();

                    // await _brik.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B , forword);

                    
                    await _brik.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, forword, 30, false);

                   

                    direction_list.Add(oriant_direction);//set orit[] in it's orientation
                    
                    System.Threading.Thread.Sleep(20);
                    //Debug.WriteLine(direction_list [oriant_counter]);



                   Line drawingline = new Line();
                   
                    if (firstcordinates == 0)
                    {
                        drawingline.X1 = 1175;
                        drawingline.Y1 = 200;
                        p_array[oriant_counter].X = 1175;
                        p_array[oriant_counter].Y = 200;
                        firstcordinates++;
                        oriant_counter++;
                        rotation = _brik.Ports[InputPort.B].SIValue;


                    }
                    else {
                     
                        drawingline.Stroke = System.Windows.Media.Brushes.Black; //Umriss 
                        drawingline.Fill = System.Windows.Media.Brushes.Black; //Fuellung 
                        drawingline.Opacity = 100;
                        drawingline.StrokeThickness = 20;


                        //rotation = _brik.Ports[InputPort.B].SIValue;
                        //rotation = Math.Abs(rotation);
                        //rotation /= 10;
                        SIN_COS(oriant_direction);

                       
                        p_array[oriant_counter].X = (p_array[oriant_counter - 1].X)+(rotation * COS) / 50;
                        p_array[oriant_counter].Y = (p_array[oriant_counter - 1].Y)+(rotation * SIN) / 50;
                        drawingline.X1 = p_array[oriant_counter - 1].X;
                        drawingline.Y1 = p_array[oriant_counter - 1].Y;
                        drawingline.X2 = p_array[oriant_counter].X;
                        drawingline.Y2 = p_array[oriant_counter].Y;

                        


                        M.grid.Children.Add(drawingline);


                        oriant_counter++;
                    }

                }
                else {
                    //stop motor if obstacle found
                    fire_counter++;
                    //check best path
                    //rotate_right();            
                    //    left = obstacle;
                    await _brik.DirectCommand.PlayToneAsync(100, 1000, 100);
                    System.Threading.Thread.Sleep(150);
                    await _brik.DirectCommand.PlayToneAsync(100, 1000, 100);
                    if (fire_counter == 4)
                    {
                        double x = p_array[oriant_counter - 1].X;
                        double y = p_array[oriant_counter - 1].Y;

                        //Convert.ToInt32(x);
                        //Convert.ToInt32(y);
                        x += 100;
                        y -= 400;

                        Canvas.SetLeft(M.button_fire, x);
                        Canvas.SetTop(M.button_fire, y);

                    }
                    //await _brik.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.B | OutputPort.C, forword,100, false);

                    //System.Threading.Thread.Sleep(100);

                    
                    await _brik.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, false);
                    Line OBCTACLE_LINE = new Line();
                    OBCTACLE_LINE.Stroke = System.Windows.Media.Brushes.Green; //Umriss 
                    OBCTACLE_LINE.StrokeThickness = 20;
                    OBCTACLE_LINE.Fill = System.Windows.Media.Brushes.Green; //Fuellung 
                    OBCTACLE_LINE.Opacity =100;

                    if (oriant_direction == 0 )
                    {

                        OBCTACLE_LINE.X1 = p_array[oriant_counter - 1].X + 20;
                        OBCTACLE_LINE.Y1 = p_array[oriant_counter - 1].Y + 20;
                        OBCTACLE_LINE.X2 = p_array[oriant_counter - 1].X + 20;
                        OBCTACLE_LINE.Y2 = p_array[oriant_counter - 1].Y - 20;

                        M.grid.Children.Add(OBCTACLE_LINE);

                    }
                    else if (oriant_direction == 180)
                    {

                        OBCTACLE_LINE.X1 = p_array[oriant_counter - 1].X -20;
                        OBCTACLE_LINE.Y1 = p_array[oriant_counter - 1].Y + 20;
                        OBCTACLE_LINE.X2 = p_array[oriant_counter - 1].X - 20;
                        OBCTACLE_LINE.Y2 = p_array[oriant_counter - 1].Y - 20;

                        M.grid.Children.Add(OBCTACLE_LINE);
                    }

                    else if(oriant_direction == 90 )
                    {

                        OBCTACLE_LINE.X1 = p_array[oriant_counter - 1].X - 20;
                        OBCTACLE_LINE.Y1 = p_array[oriant_counter - 1].Y + 20;
                        OBCTACLE_LINE.X2 = p_array[oriant_counter - 1].X + 20;
                        OBCTACLE_LINE.Y2 = p_array[oriant_counter - 1].Y + 20;

                        M.grid.Children.Add(OBCTACLE_LINE);


                    }
                    else if (oriant_direction == 270)
                    {

                        OBCTACLE_LINE.X1 = p_array[oriant_counter - 1].X - 20;
                        OBCTACLE_LINE.Y1 = p_array[oriant_counter - 1].Y -20;
                        OBCTACLE_LINE.X2 = p_array[oriant_counter - 1].X + 20;
                        OBCTACLE_LINE.Y2 = p_array[oriant_counter - 1].Y - 20;

                        M.grid.Children.Add(OBCTACLE_LINE);


                    }

                    rotate_right();
                    System.Threading.Thread.Sleep(1000);
                    obstacle = Convert.ToInt32(_brik.Ports[InputPort.Four].SIValue.ToString());
                    right = obstacle;
                    rotate_left();
                    System.Threading.Thread.Sleep(1000);
                    rotate_left();

                    System.Threading.Thread.Sleep(1000);
                    obstacle = Convert.ToInt32(_brik.Ports[InputPort.Four].SIValue.ToString());
                    left = obstacle;
                    if(right>left)
                    {

                        move_right();
                        System.Threading.Thread.Sleep(1000);
                        rotate_right();

                        oriant_direction -= rotate_angle;

                    }
                    if (left>right)
                    {

                        move_left();
                        System.Threading.Thread.Sleep(1000);
                        rotate_right();
                        oriant_direction += rotate_angle;


                    }
                    System.Threading.Thread.Sleep(1000);
                    //angel_rotation = _brik.Ports[InputPort.B].SIValue;
                    
                    //rotation -= angel_rotation;


                    obstacle = Convert.ToInt32(_brik.Ports[InputPort.Four].SIValue.ToString());


                }
            }

        }
        public async  void  move_right()
        {
            

            _brik.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, forword, time, false);
            _brik.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, backword, time, false);
            await _brik.BatchCommand.SendCommandAsync();

        }
        public async void move_left()
        {


            _brik.BatchCommand.TurnMotorAtPowerForTime(OutputPort.B, backword, time, false);
            _brik.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, forword, time, false);
           await _brik.BatchCommand.SendCommandAsync();

        }
        public  void SIN_COS(int X )
        {

            if (X == 0 )
            {
                SIN = 0;
                COS = 1;

            }
            else if (X == 90)
            {

                SIN = 1;
                COS = 0;
            }
            else if (X == 180)
            {

                SIN = 0;
                COS = -1;
            }
            else if(X == 270)
            {

                SIN = -1;
                COS = 0;
            }
            else if (X ==360)
            {
                SIN = 0;
                COS = 1;


            }
        



        }

        private void rowtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

      
    }
}

