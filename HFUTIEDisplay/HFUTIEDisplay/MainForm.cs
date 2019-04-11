using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace HFUTIEDisplay
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

        }
        int x, y;
        DateTime start;
        public bool isMove = true;
        public VideoConfig videoConfig  ;
        public string videoConfigPath = Application.StartupPath + @"\Config\VideoPathConfig.xml";
        VideoForm videoForm = new VideoForm();
        #region 事件
        private void MainForm_Load(object sender, EventArgs e)
        {
            x = MousePosition.X;
            y = MousePosition.Y;
            videoConfig =getVideoFileName(videoConfigPath);
            cmbPath.Items.Add(videoConfig.FileName);
            numericUpDown1.Value = videoConfig.Second;
            cmbPath.SelectedIndex = 0;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            IsMouseMoved(videoConfig.Second);
        }



        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示    
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点
                this.Activate();
                //任务栏区显示图标
                this.ShowInTaskbar = true;
                //托盘区图标隐藏
                notifyIcon1.Visible = false;
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            //判断是否选择的是最小化按钮
            if (WindowState == FormWindowState.Minimized)
            {
                //隐藏任务栏区图标
                this.ShowInTaskbar = false;
                //图标显示在托盘区
                notifyIcon1.Visible = true;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否确认退出程序？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // 关闭所有的线程
                this.Dispose();
                this.Close();
            }
            else
            {
                e.Cancel = true;
            }
        }

      

        private void setVideoPathbtn_Click(object sender, EventArgs e)
        {
            try
            {
                SelectVideo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            setVideoFileName(videoConfigPath,videoConfig);
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
            this.WindowState = FormWindowState.Minimized;
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ChangeConfig();
        }

      
        private void cmbPath_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeConfig();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 当配置出现变化时
        /// </summary>
        private void ChangeConfig()
        {
            videoConfig.Second = Convert.ToInt32(numericUpDown1.Value);
            videoConfig.FileName = cmbPath.Text.Trim();
        }

        /// <summary>
        /// 判断鼠标在某一段时间内，是否有移动
        /// </summary>
        void IsMouseMoved(int second)
        {
            if (!videoForm.isPlaying)
            {
                int x1 = MousePosition.X;
                int y1 = MousePosition.Y;

                if ((x == x1) && (y == y1) && isMove)
                {
                    start = DateTime.Now;
                    isMove = false;
                }
                if (x != x1 || y != y1)
                {
                    x = x1;
                    y = y1;
                    start = DateTime.Now;
                    isMove = true;
                }

                TimeSpan ts = DateTime.Now.Subtract(start);

                //鼠标或键盘误动作1分钟后开始播放视频   
                if (ts.Seconds >= second)
                {
                    videoForm = new VideoForm(videoConfig.FileName, this);
                    videoForm.ShowDialog();
                }
            }
        }

        /// <summary>
        /// 选择视频文件
        /// </summary>
        private void SelectVideo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                videoConfig.FileName = openFileDialog.FileName;
                videoConfig.Second = Convert.ToInt32(numericUpDown1.Value);
                setVideoFileName(videoConfigPath, videoConfig);
                cmbPath.Items.Clear();
                cmbPath.Items.Add(videoConfig.FileName);
                cmbPath.SelectedIndex = 0;
            }
        }


        /// <summary>
        /// 获取播放视频的路径
        /// </summary>
        /// <param name="xmlpath">配置文件路径</param>
        /// <returns></returns>
        public VideoConfig getVideoFileName(string xmlpath)
        {
            try
            {
                VideoConfig videoConfig = new VideoConfig();
                XmlDocument xml_doc = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;//忽略文档里面的注释
                XmlReader reader = XmlReader.Create(xmlpath, settings);
                xml_doc.Load(reader);
                XmlNode root_node = xml_doc.SelectSingleNode("data");
                XmlNodeList list_nodes = root_node.ChildNodes;
                foreach (XmlNode _nodes in list_nodes)
                {
                    switch (_nodes.Name)
                    {
                        case "FileName":
                            videoConfig.FileName = _nodes.InnerText;
                            break;

                        case "Second":
                            videoConfig.Second =Convert.ToInt32(_nodes.InnerText) ;
                            break;
                    }
                }
                reader.Close();
                return videoConfig;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

       


        /// <summary>
        /// 保存上次的视频文件
        /// </summary>
        /// <param name="xmlpath"></param>
        /// <param name="videoConfig"></param>
        public void setVideoFileName(string xmlpath, VideoConfig videoConfig )
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("   ");
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.CloseOutput = false;
            settings.OmitXmlDeclaration = false;
            XmlWriter writer = XmlWriter.Create(xmlpath, settings);
            writer.WriteStartDocument();
            writer.WriteComment("当前打印机名字——(安徽)合肥工业大学");
            writer.WriteStartElement("data");
            writer.WriteElementString("FileName", videoConfig.FileName);
            writer.WriteElementString("Second", videoConfig.Second.ToString());
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }
        #endregion

    }
}
