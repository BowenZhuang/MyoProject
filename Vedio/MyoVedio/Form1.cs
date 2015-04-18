using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DirectX.Capture;
using DShowNET;
using System.IO;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace MyoVedio
{
    public partial class Form1 : Form
    {
        Capture capture = null;
        Filters filters = null;
        private const string kconnStr = "server=localhost;user=root;database=myo;port=3306;password=1234;";
        int counter = 1;
        DateTime startTime;
        DateTime endTime;
        NamedPipeServer PServer1;
        int deviceNumber = 0;
        public Form1()
        {
           
            InitializeComponent();

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            Point ptLocation = new Point(0, 0);
            Size szSize = new Size(resolution.Width / 2, resolution.Height / 2);
            this.Location = ptLocation;
            this.Size = szSize;
            this.StartPosition = FormStartPosition.Manual;
            this.panel1.Size = this.Size;
            PServer1 = new NamedPipeServer(@"\\.\pipe\myNamedPipe3", 0, this);
            PServer1.Start();
            
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            filters = new Filters();

            if (filters.VideoInputDevices != null)
            {
                try
                {
                    preview(deviceNumber);
                    startCapturing();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Maybe any other software is already using your WebCam.\n\n Error Message: \n\n" + ex.Message);
                }
            }
            else
            {
                
                MessageBox.Show("No video device connected to your PC!");
            }

             

        }

        public void InsertNewData(string filePath, DateTime startTime, DateTime endTime)
        {
             

            MySqlConnection conn = new MySqlConnection(kconnStr);
            try
            {

              
                MySqlCommand command = conn.CreateCommand();
             
                int nRow = 0;
                conn.Open();
                command.CommandText = "INSERT INTO `vedioFile`(`filePath`,`startTime`,`endTime`) VALUES(@1, @2, @3);";
                command.Parameters.AddWithValue("@1", filePath);
                command.Parameters.AddWithValue("@2", startTime);
                command.Parameters.AddWithValue("@3", endTime);
                 
                nRow = command.ExecuteNonQuery();
                Debug.WriteLine("Insert Return: {0}", nRow);

                conn.Close();
            }
            catch (Exception ex)
            {
               string strLog = ex.ToString();
                Debug.WriteLine(strLog);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            PServer1.StopServer();

            base.OnClosing(e);
        }

        void preview(int deviceNo)
        {
            try
            {
                capture = new Capture(filters.VideoInputDevices[deviceNo], filters.AudioInputDevices[0]);

                capture.PreviewWindow = panel1;
                 
            }
            catch { }
        }

       public void startCapturing()
        {
            
            //if (timer.Enabled) timer.Stop();
             startTime = DateTime.Now;

                try
                {
                    if (!this.capture.Cued) this.capture.Filename = counter + ".wmv";

                    this.capture.Cue();
                    this.capture.Start();

                    //timer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Message: \n\n" + ex.Message);
                } 
        }

        public void StopCapturing()
        {
            if (this.capture != null)
            {
                this.capture.Stop();
                endTime = DateTime.Now;
            }
            String path = Directory.GetCurrentDirectory();
            string oldpath = Directory.GetCurrentDirectory() + "\\" + this.capture.Filename;
            path = path + "\\" + (startTime.ToShortDateString()) + "-" + (startTime.Hour + "-" + startTime.Minute + "-" + startTime.Second) + "_" + (endTime.ToShortDateString())+"-"+(endTime.Hour + "-" + endTime.Minute + "-" + endTime.Second) + ".wmv";
            System.IO.File.Move(oldpath, path);
            InsertNewData(path,startTime, endTime);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopCapturing();
        }
    }
}
