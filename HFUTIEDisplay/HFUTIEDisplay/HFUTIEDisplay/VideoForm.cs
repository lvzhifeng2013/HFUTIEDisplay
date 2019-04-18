using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HFUTIEDisplay
{
    public partial class VideoForm : Form
    {
        public VideoForm(string videoPath,MainForm mainForm)
        {
            InitializeComponent();
            this.videoPath = videoPath;
            this.mainForm = mainForm;
        }
        public VideoForm()
        {

            InitializeComponent();
        }
        MainForm mainForm;
        /// <summary>
        /// 播放文件的名字
        /// </summary>
        string videoPath;
        /// <summary>
        /// 判断是否正在播放
        /// </summary>
        public bool isPlaying;
        private void VideoForm_Load(object sender, EventArgs e)
        {
            try
            {
              
                this.WindowState = FormWindowState.Maximized;
                this.wmp.URL = videoPath;
                this.wmp.Ctlcontrols.play();
                this.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void wmp_StatusChange(object sender, EventArgs e)
        {
            try
            {
                if (wmp.playState == WMPLib.WMPPlayState.wmppsStopped)
                {

                    if (isPlaying)
                    {
                        //停顿100毫秒再重新播放  
                        Thread.Sleep(1000);
                        //重新播放  
                        wmp.URL = videoPath;
                        wmp.Ctlcontrols.play();
                    }
                }
                else if (wmp.playState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    isPlaying = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void wmp_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            StopPlay();
        }

        public void StopPlay()
        {
            try
            {
                if (wmp.playState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    this.WindowState = FormWindowState.Minimized;
                    isPlaying = false;
                    mainForm.mouseIsMove = true;
                    wmp.Ctlcontrols.stop();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void wmp_KeyDownEvent(object sender, AxWMPLib._WMPOCXEvents_KeyDownEvent e)
        {
            StopPlay();
        }

        private void wmp_KeyPressEvent(object sender, AxWMPLib._WMPOCXEvents_KeyPressEvent e)
        {
            StopPlay();
        }

        private void wmp_ErrorEvent(object sender, EventArgs e)
        {
            MessageBox.Show("发生错误");
        }
    }
}
