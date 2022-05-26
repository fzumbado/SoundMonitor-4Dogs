using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundMonitor4Dogs
{
    
    public partial class Form1 : Form
    {
        MicrophoneLevel micLevel;
        bool isRunning = false;
        bool isPlaying = false;

        #region Constructors 

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            micLevel = new MicrophoneLevel();
            micLevel.MicLevelReceived += MicLevel_MicLevelReceived;

#if (DEBUG)
            txtFilepath.Text = @"C:\Temp\Sound-Windows Logon.wav";
#endif
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            micLevel.MicLevelReceived -= MicLevel_MicLevelReceived;
            micLevel = null;
        }
        #endregion

        #region Methods 

        /// <summary>
        /// Displays current sound level on volume bar
        /// </summary>
        /// <param name="Peak">Current value</param>
        /// <param name="Max">Maximum value</param>
        private void PrintSoundValue(int Peak, double Max)
        {
            int level = (int)(Peak / Max * 100);
            progressBar1.Value = level;

            // Checks sound level and Executes sound
            if (level > trackBar1.Value)
            {
                this.txtAlertLevel.BackColor = System.Drawing.Color.Red;
                this.Refresh();

                // Verifies if is already playing sound
                if (isRunning && !isPlaying)
                {
                    isPlaying = true;
                    PlaySound(true);
                    isPlaying = false;
                }
            }
            else
                this.txtAlertLevel.BackColor = System.Drawing.Color.LightYellow;
        }

        /// <summary>
        /// Plays sound on the filename
        /// </summary>
        private void PlaySound(bool sameThread = false)
        {
            if (txtFilepath.Text.Trim().Equals(String.Empty))
                MessageBox.Show("Must declare a filename first");
            else
            {
                PlaySound playSound = new PlaySound(txtFilepath.Text);
                playSound.Play(sameThread);
            }
        }

        #endregion

        #region Events

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\temp\\";
                openFileDialog.Filter = "audio files (*.wav)|*.wav|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    txtFilepath.Text = openFileDialog.FileName;
                }
            }
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            btnAction.Text = btnAction.Text == "Start" ? "Stop (Running)" : "Start";
            isRunning = btnAction.Text != "Start";
        }

        private void btnPlaySample_Click(object sender, EventArgs e)
        {
           PlaySound(true);
        }

        private void MicLevel_MicLevelReceived(object sender, SoundLevelArgs e)
        {
            try
            {
                if (!isPlaying)
                {
                    // Calls the PrintLabel in Main Thread (to avoid cross-threading error)
                    if (InvokeRequired)
                    {
                        Invoke((MethodInvoker)delegate { MicLevel_MicLevelReceived(sender, e); });
                        return;
                    }
                    // this code will run on main (UI) thread 
                    PrintSoundValue(e.LevelL, e.Maximum);
                }
            }
            catch(Exception)
            { }
        }

        #endregion
    }
}
