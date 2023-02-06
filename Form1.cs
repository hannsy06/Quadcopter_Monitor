using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO.Ports;
using System.Globalization;
using System.Diagnostics;

namespace Drone_monitor_2
{
    public partial class Form1 : Form
    {
        string kp, ki, kd, error_altitude, heading, flight_mode, angle_roll, angle_pitch, pressure_setpoint, ground_pressure, actual_pressure, err_lat, err_long, gps_roll, gps_pitch, lattitude, longtitude, number_sats;
        string battery,time_wp, altitude, altitude_setpoint, heading_lock, battery_voltage, throttle_altitude, temper, location_address;

      

        int counter, time_landing;

      

        string gps_lat_setpoint, emergency_return,check_waypoint,fix_type, complete_wp, gps_lon_setpoint, lat_home_return, lon_home_return, l_lat_waypoint, l_lon_waypoint, waypoint_list, waypoint_mode, lat_wp1, lat_wp2, lat_wp3, lat_wp4, lon_wp1, lon_wp2, lon_wp3, lon_wp4;

     

        int[] receive_buffer;
        byte receive_buffer_counter, start, receive_byte_previous, receive_start_detect, check_byte, temp_byte;

        int roll_angle, pitch_angle;
        string check_start, new_data, previous_data = "check";
        string[] data_buffer = new string[30];
        byte webbrouwser_active = 0, webbrouwser_1_ready, webbrouwser_2_ready;

        String InputData = String.Empty;
        delegate void SetTextCallback(string text);





        public Form1()
        {
            InitializeComponent();


        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                btn_connect.Text = "CLOSE";
                try
                {
                    serialPort1.PortName = comboBox1.Text;//cổng serialPort1 = ComboBox mà bạn đang chọn
                    serialPort1.Open();// Mở cổng serialPort1
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);



                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                serialPort1.Close();
                btn_connect.Text = "OPEN";
               // button1.BackColor = Color.GreenYellow;
                btn_start_stop_takeoff.BackColor = Color.Gold;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = SerialPort.GetPortNames();//Quét các cổng Serial đang hoạt động lên ComboBox1
        }

        private void indicator_on()
        {
            Graphics g = panel1.CreateGraphics();
            Pen p = new Pen(Color.Green);
            SolidBrush sb = new SolidBrush(Color.Green);
            g.DrawEllipse(p, 1, 1, 15, 15);
            g.FillEllipse(sb, 1, 1, 15, 15);
        }

        private void indicator_off()
        {
            Graphics g = panel1.CreateGraphics();
            Pen p = new Pen(Color.Red);
            SolidBrush sb = new SolidBrush(Color.Red);
            g.DrawEllipse(p, 1, 1, 15, 15);
            g.FillEllipse(sb, 1, 1, 15, 15);
        }


        private void btn_start_stop_takeoff_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
                if (serialPort1.IsOpen)
            {
               
                
                btn_start_stop_takeoff.BackColor=Color.DodgerBlue;
               
                serialPort1.WriteLine("4");
                           
            }
            else
            {
                btn_start_stop_takeoff.BackColor = Color.Gold;
            }
        }
 
            private void btn_set_debug2_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("2");
        }

        private void btn_set_debug3_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("3");
        }



        private void btn_set_debug0_Click_1(object sender, EventArgs e)
        {
            serialPort1.WriteLine("0");
            btn_start_stop_takeoff.BackColor = Color.Gold;
        }
        private void Set_wp_Click_1(object sender, EventArgs e)
        {
            serialPort1.Write(txt_lat1.Text + "/");
            serialPort1.Write(txt_lat2.Text + "/");
            serialPort1.Write(txt_lat3.Text + "/");
            serialPort1.Write(txt_lat4.Text + "/");
            serialPort1.Write(txt_long1.Text + "/");
            serialPort1.Write(txt_long2.Text + "/");
            serialPort1.Write(txt_long3.Text + "/");
            serialPort1.WriteLine(txt_long4.Text);
        }

        private void set_altitude_Click(object sender, EventArgs e)
        {
            serialPort1.Write(txt_SetAltitdue.Text + "/"); //send kP to Arduino
            serialPort1.Write(txt_settime_wp.Text + "/");
            serialPort1.Write(txt_lat3.Text + "/");
            serialPort1.Write(txt_lat4.Text + "/");
            serialPort1.Write(txt_long1.Text + "/");
            serialPort1.Write(txt_long2.Text + "/");
            serialPort1.Write(txt_long3.Text + "/");
            serialPort1.WriteLine(txt_long4.Text);
        }






        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            InputData = serialPort1.ReadLine();


            string[] cut;
            cut = InputData.Split('|');
            new_data = cut[0];

            if (string.Compare("check", new_data) == 0)
            {
                try
                {

                    altitude = cut[1];
                    battery = cut[2];
                    check_waypoint = cut[3];
                    fix_type = cut[4];
                    longtitude = cut[5];

                    lattitude = cut[6];

                    flight_mode = cut[7];
                    number_sats = cut[8];
                    heading_lock = cut[9];
                    waypoint_list = cut[10];
                    complete_wp = cut[11];
                    waypoint_mode = cut[12];
                    emergency_return = cut[13];
                    heading = cut[14];
                    if (longtitude !="0")
                    {
                        longtitude = longtitude.Insert(3, ".");
                        lattitude = lattitude.Insert(2, ".");
                    } 
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                }
            }
            else if (string.Compare("debug1", new_data) == 0)
            {
                try
                {
                    kp = cut[1];
                    ki = cut[2];
                    kd = cut[3];
                    battery = cut[4];
                    angle_roll = cut[5];
                    angle_pitch = cut[6];
                    flight_mode = cut[7];
                    heading_lock = cut[8];
                    heading = cut[9];
                    throttle_altitude = cut[10];


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message); ;
                }
            }
            else if (string.Compare("debug2", new_data) == 0)
            {
                try
                {
                    altitude_setpoint = cut[1];
                    time_wp = cut[2];

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message); ;

                }
            }
            else if(string.Compare("debug3", new_data) == 0)
            {
                try
                {
                    lat_wp1 = cut[1];
                    lat_wp2 = cut[2];
                    lat_wp3 = cut[3];
                    lat_wp4 = cut[4];
                    lon_wp1 = cut[5];
                    lon_wp2 = cut[6];
                    lon_wp3 = cut[7];
                    lon_wp4 = cut[8];



                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message); ;

                }
            }


            SetText(InputData);
        }


        private void SetText(string inputData)
        {

            if (this.textBox1.InvokeRequired)
            {

                SetTextCallback d = new SetTextCallback(SetText); // khởi tạo 1 delegate mới gọi đến SetText

                this.Invoke(d, new object[] { inputData });

            }

            else
            {
                this.textBox1.Text = inputData;
            }
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                // btn_connect.Enabled = true;
                lb_connect.Text = ("DISCONNECTED");
                lb_connect.ForeColor = Color.Red;
                // btn_disconnect.Enabled = false;
            }
            else
            {
                //  btn_connect.Enabled = true;
                lb_connect.Text = ("CONNECTED");
                lb_connect.ForeColor = Color.Green;
                //btn_disconnect.Enabled = true;


            }
         
            txt_lat_wp1.Text = lat_wp1;
            txt_lat_wp2.Text = lat_wp2;
            txt_lat_wp3.Text = lat_wp3;
            txt_lat_wp4.Text = lat_wp4;
            txt_lon_wp1.Text = lon_wp1;
            txt_lon_wp2.Text = lon_wp2;
            txt_lon_wp3.Text = lon_wp3;
            txt_lon_wp4.Text = lon_wp4;

            txt_time_wp.Text = time_wp;
            txt_altitude_setpoint.Text = altitude_setpoint;
            label_wp_list.Text = waypoint_list;
            //  txt_lat_wp.Text = gps_lat_setpoint;
            // txt_lon_wp.Text = gps_lon_setpoint;

            label_lat.Text = lattitude;
            label_long.Text = longtitude;
            label_altitude.Text = altitude + "m";
            label_battery.Text = battery + "V";
            label_compass.Text = heading;
            label_satelite.Text = number_sats;
            label_FLmode.Text = flight_mode;
            
            if(check_waypoint=="0")
            {
                counter++;
                panel1.Visible = true;
                indicator_off();
                if (counter == 2)
                {
                    counter = 0;
                    panel1.Visible = false;
                }
            }
            else
            {
                panel1.Visible = true;
                indicator_on();
            }
            
          //  textBox1.Visible = false;
            if (fix_type == "1")
            {
                labeL_fix_type.Text = "No Fix";
            }
            else if (fix_type == "2")
            {
                labeL_fix_type.Text = "2D Fix";
            }
            else if (fix_type == "3")
            {
                labeL_fix_type.Text = "3D Fix";
            } 
            
           

            if (string.Compare("check", new_data) == 0)
            {
                
            
                label_debug2.Text = "OFF";
                label_debug2.ForeColor = Color.Red;
                label_debug3.Text = "OFF";
                label_debug3.ForeColor = Color.Red;

            }
            if (string.Compare("debug1", new_data) == 0)
            {
               

                label_debug2.Text = "OFF";
                label_debug2.ForeColor = Color.Red;
                label_debug3.Text = "OFF";
                label_debug3.ForeColor = Color.Red;

            }
            if (string.Compare("debug2", new_data) == 0)
            {
              
                   txt_altitude_setpoint.Text = altitude_setpoint;
       
             
                label_debug2.Text = "ON";
                label_debug2.ForeColor = Color.Green;
                label_debug3.Text = "OFF";
                label_debug3.ForeColor = Color.Red;
             

            }
            if (string.Compare("debug3", new_data) == 0)
            {
         
                label_debug3.Text = "ON";
                label_debug3.ForeColor = Color.Green;
                label_debug2.Text = "OFF";
                label_debug2.ForeColor = Color.Red;
              

            }
            if (waypoint_list == "0")
            {
                lat_home_return = l_lat_waypoint;
                lon_home_return = l_lon_waypoint;
            }
            if (waypoint_list == "1")
            {
                lat_wp1 = l_lat_waypoint;
                lon_wp1 = l_lon_waypoint;
            }
            if (waypoint_list == "2")
            {
                lat_wp2 = l_lat_waypoint;
                lon_wp2 = l_lon_waypoint;
            }
            if (waypoint_list == "3")
            {
                lat_wp3 = l_lat_waypoint;
                lon_wp3 = l_lon_waypoint;
            }
            if (waypoint_list == "4")
            {
                lat_wp4 = l_lat_waypoint;
                lon_wp4 = l_lon_waypoint;
            }

            // btn_disconnect.Enabled = true;



        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (heading_lock == "1")
            {
                label_heading.Text = ("ON");
                label_heading.ForeColor = Color.Green;

            }
            else
            {
                label_heading.Text = ("OFF");
                label_heading.ForeColor = Color.Red;
            }



            if (waypoint_mode == "1")
            {
                //  label_wpmode.Text = ("ON");
                // label_wpmode.ForeColor = Color.Green;

               //indicator_on();

                if (emergency_return == "0")
                {

                    if (complete_wp == "0")
                    {
                        time_landing = 0;
                        label_return.Text = ("OFF");
                        label_return.ForeColor = Color.Red;
                        if (waypoint_list == "1")
                        {

                            label_flight_mission.Text = ("Flying to location 1");
                            label_flight_mission.ForeColor = Color.Green;
                        }
                        else if (waypoint_list == "2")
                        {

                            label_flight_mission.Text = ("Flying to location 2");
                            label_flight_mission.ForeColor = Color.Green;
                        }
                        else if (waypoint_list == "3")

                        {

                            label_flight_mission.Text = ("Flying to location 3");
                            label_flight_mission.ForeColor = Color.Green;
                        }

                        else if (waypoint_list == "4")
                        {

                            label_flight_mission.Text = ("Flying to location 4");
                            label_flight_mission.ForeColor = Color.Green;
                        }


                    }
                    else //if (complete_wp == "1")
                    {
                        label_home_wp.Text = ("ON");
                        label_home_wp.ForeColor = Color.Green;
                        if (time_landing < 16)
                        {                         
                            label_flight_mission.Text = ("COMPLETED");
                            label_flight_mission.ForeColor = Color.Green;
                            time_landing++;
                        }                
                        else 
                        {
                            time_landing = 16 ;
                            label_flight_mission.Text = ("LANDING...");
                            label_flight_mission.ForeColor = Color.Green;
                            btn_start_stop_takeoff.BackColor = Color.Gold;
                        }    

                    }



                }

                else
                {
                   // label_home_wp.Text = ("ON");
                   // label_home_wp.ForeColor = Color.Green;
                    label_return.Text = ("ON");
                    label_return.ForeColor = Color.Green;

                    label_flight_mission.Text = ("Emergency Return");
                    label_flight_mission.ForeColor = Color.Red;
                }
            }

            else
            {
                label_home_wp.Text = ("OFF");
                label_home_wp.ForeColor = Color.Red;
                label_flight_mission.Text = ("     Manual");
                label_flight_mission.ForeColor = Color.Green;
              //  btn_start_stop_takeoff.BackColor = Color.Blue;
            } 
            

        }
    }
}
    




      

      
    

