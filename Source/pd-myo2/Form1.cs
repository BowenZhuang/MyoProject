using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MySql.Data;
using MySql.Data.MySqlClient;
using GraphLib;

namespace pd_myo
{
    public partial class Form1 : Form
    {
        private const string kconnStr = "server=localhost;user=root;database=myo;port=3306;password=Conestoga1;";
        private int NumGraphs = 8;
        private PrecisionTimer.Timer mTimer = null;
        private DateTime lastTimerTick = DateTime.Now;
        List<MData> mData = new List<MData>();
        List<MData> dispData = new List<MData>();
        int curPos = 0;
        public Form1()
        {
            InitializeComponent();

            display.Smoothing = System.Drawing.Drawing2D.SmoothingMode.None;
            CalcDataGraphs();
        }

        private String RenderXLabel(DataSource s, int idx)
        {
            if (s.AutoScaleX)
            {
                //if (idx % 2 == 0)
                {
                    int Value = (int)(s.Samples[idx].x);
                    return "" + Value;
                }
                return "";
            }
            else
            {
                int Value = (int)(s.Samples[idx].x / 20);
                String Label = "" + Value + "\"";
                return Label;
            }
        }

        private String RenderYLabel(DataSource s, float value)
        {
            return String.Format("{0:0.0}", value);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if(mTimer != null)
            {
                mTimer.Stop();
                mTimer.Dispose();
            }
            display.Dispose();
            mData.Clear();

            base.OnClosing(e);
        }

        private void ApplyColorSchema()
        {
            Color[] cols = { Color.DarkRed, 
                                Color.DarkSlateGray,
                                Color.DarkCyan, 
                                Color.DarkGreen, 
                                Color.DarkBlue ,
                                Color.DarkMagenta,                              
                                Color.DeepPink };

            for (int j = 0; j < NumGraphs; j++)
            {
                display.DataSources[j].GraphColor = cols[j % 7];
            }

            display.BackgroundColorTop = Color.White;
            display.BackgroundColorBot = Color.LightGray;
            display.SolidGridColor = Color.LightGray;
            display.DashedGridColor = Color.LightGray;
        }

        protected void CalcSinusFunction(DataSource src, int idx)
        {
            for (int i = 0; i < dispData.Count; i++)
            {
                src.Samples[i].x = i;

                src.Samples[i].y = dispData[i].GetData(idx);
                                    //(float)(((float)20 *
                                    //        Math.Sin(40 * (idx + 1) * (i + 1) * Math.PI / src.Length)) *
                                    //        Math.Sin(160 * (idx + 1) * (i + 1) * Math.PI / src.Length)) +
                                    //        (float)(((float)200 *
                                    //        Math.Sin(4 * (idx + 1) * (i + 1) * Math.PI / src.Length)));
            }
            src.OnRenderYAxisLabel = RenderYLabel;
        }

        protected void CalcDataGraphs()
        {

            this.SuspendLayout();

            display.DataSources.Clear();
            display.SetDisplayRangeX(0, 120);

            for (int j = 0; j < NumGraphs; j++)
            {
                display.DataSources.Add(new DataSource());
                display.DataSources[j].Name = "Graph " + (j + 1);
                display.DataSources[j].OnRenderXAxisLabel += RenderXLabel;

                //this.Text = "Tiled Graphs (horizontal prefered) autoscaled";
                display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_HOR;
                display.DataSources[j].Length = 700;
                //display.DataSources[j].AutoScaleY = true;
                display.DataSources[j].SetDisplayRangeY(-50, 50);
                display.DataSources[j].SetGridDistanceY(20);
                CalcSinusFunction(display.DataSources[j], j);
            }

            ApplyColorSchema();

            this.ResumeLayout();
            display.Refresh();

        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string line_data, strLog;
            int nRow = 0;
            
            MySqlConnection conn = new MySqlConnection(kconnStr);
            try
            {
                MySqlCommand command = conn.CreateCommand();
                // INSERT INTO `rlog` VALUES();

                string sql;


                conn.Open();

                foreach (string line in File.ReadLines("playing_guitar.txt"))
                {
                    line_data = line.Replace(" ", "");
                    string[] data = line_data.Split(',');
                    sql = String.Format("INSERT INTO `rlog1`(`rdate`,`rdata1`,`rdata2`,`rdata3`,`rdata4`,`rdata5`,`rdata6`,`rdata7`,`rdata8`) VALUES('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8});",
                        data[8], data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]);
                    command.CommandText = sql;
                    nRow = command.ExecuteNonQuery();
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                strLog = ex.ToString();
            }
        }

        private void displayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sql = "SELECT rdate, rdata1, rdata2, rdata3, rdata4, rdata5, rdata6, rdata7, rdata8 FROM myo.rlog1;";
            MySqlConnection conn = new MySqlConnection(kconnStr);
            string strdata;
            try
            {
                MySqlDataReader myReader = null;
                MySqlCommand myCommand = conn.CreateCommand(); 
                //new SqlCommand("select * from table", myConnection);

                conn.Open();

                myCommand.CommandText = sql;
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    MData oneData = new MData();
                    oneData.strTime = myReader["rdate"].ToString();
                    for (int i = 0; i < 8; i++)
                    {
                        strdata = String.Format("rdata{0}", i+1);
                        oneData.data[i] = Int32.Parse(myReader[strdata].ToString());
                    }
                    mData.Add(oneData);
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            mTimer = new PrecisionTimer.Timer();
            mTimer.Period = 160;                         // 20 fps
            mTimer.Tick += new EventHandler(OnTimerTick);
            lastTimerTick = DateTime.Now;
            dispData = mData;
            curPos = 0;
            CalcDataGraphs();
            mTimer.Start(); 
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            try
            {
                TimeSpan dt = DateTime.Now - lastTimerTick;


                MData oneData = dispData[0];
                dispData.RemoveAt(0);
                dispData.Add(oneData);

                for (int j = 0; j < NumGraphs; j++)
                {

                    CalcSinusFunction(display.DataSources[j], j);
                }

                this.Invoke(new MethodInvoker(RefreshGraph));
            }
            catch (Exception ex)
            {
                Console.Write("exception invoking refreshgraph(): " + ex.Message);
            }
        }
        private void RefreshGraph()
        {
            display.Refresh();
        }
    }
}
