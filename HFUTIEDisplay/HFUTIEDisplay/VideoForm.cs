using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HFUTIEDisplay
{
    public partial class VideoForm : Form
    {
        public VideoForm(string  videoPath,MainForm mainForm)
        {
            InitializeComponent();
            this.videoPath = videoPath;
            this.mainForm = mainForm;
        }

        public VideoForm()
        {
            InitializeComponent();          
        }
        string videoPath;
        MainForm mainForm;
        public bool isPlaying ;
        private void VideoForm_Load(object sender, EventArgs e)
        {
            try
            {
                isPlaying = true;
                this.WindowState = FormWindowState.Maximized;
                this.wmp.URL = videoPath;
                this.wmp.Ctlcontrols.play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void wmp_StatusChange(object sender, EventArgs e)
        {
            if ((int)wmp.playState == 1)
            {
                if (isPlaying)
                {
                    //停顿100毫秒再重新播放  
                    Thread.Sleep(100);
                    //重新播放  
                    wmp.Ctlcontrols.play();
                }

            }
        }

        private void wmp_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            StopPlay();
        }

        private void StopPlay()
        {
            isPlaying = false;
            this.mainForm.isMove = true;
            wmp.Ctlcontrols.stop();
            this.Close();
        }

        private void wmp_KeyDownEvent(object sender, AxWMPLib._WMPOCXEvents_KeyDownEvent e)
        {
            StopPlay();
        }

        private void VideoForm_KeyDown(object sender, KeyEventArgs e)
        {
            StopPlay();
        }
    }
}
