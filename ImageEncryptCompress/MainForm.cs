using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace ImageQuantization
{
    
    
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private RGBPixel[,] ImageMatrix;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
        }
        

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
        
        

        private void set_password(string s, ref long seed, ref short size)
        {
            for (int i = 0; i < s.Length; i++)
            {
                size += 8;
                seed <<= 8;
                seed += (long)s[i];
            }
        }

        private void encrypt_show_Click(object sender, EventArgs e)
        {
            long initial_seed = 0;
            short size_seed = 0;
            bool good = true;
            if (Binary.Checked)
            {
                if (txtGaussSigma.Text.Length > 64)
                {
                    MessageBox.Show("Binary password length should not exceed 64 letters");
                    good = false;
                }
                else
                {
                    initial_seed = Convert.ToInt64(txtGaussSigma.Text, 2);
                    size_seed = (short)txtGaussSigma.Text.Length;
                }
            }
            else
            {
                if (txtGaussSigma.Text.Length > 8)
                {
                    good = false;
                    MessageBox.Show("Character password length should not exceed 8 letters");
                }
                else
                {
                    initial_seed = size_seed = 0;
                    set_password(txtGaussSigma.Text.ToString(),ref initial_seed, ref size_seed);
                }
            }
            if (!good) return;
            short tap_pos = (short)nudMaskSize.Value;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            RGBPixel[,] encrypted = ImageOperations.Encrypt_No_cycles(ImageMatrix, size_seed, tap_pos, initial_seed);
            sw.Stop();
            MessageBox.Show(sw.ElapsedMilliseconds + " ms to encrypt");
            ImageOperations.DisplayImage(encrypted, pictureBox2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RGBPixel[,] tmp;
            HuffmanTree obj = new HuffmanTree();
            long seed = 0;
            short tap = 0;
            short size = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            tmp = obj.Decompress("logs.dat",ref seed,ref tap,ref size);
            sw.Stop();
            MessageBox.Show(sw.ElapsedMilliseconds + " ms to decompress");
            sw.Reset();
            sw.Start();
            tmp = ImageOperations.Encrypt_No_cycles(tmp, size, tap, seed);
            sw.Stop();
            MessageBox.Show(sw.ElapsedMilliseconds + " ms to decrypt");
            ImageOperations.DisplayImage(tmp, pictureBox2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            long initial_seed = 0;
            short size_seed = 0;
            bool good = true;
            if (Binary.Checked)
            {
                if (txtGaussSigma.Text.Length > 64)
                {
                    MessageBox.Show("Binary password length should not exceed 64 letters");
                    good = false;
                }
                else
                {
                    initial_seed = Convert.ToInt64(txtGaussSigma.Text, 2);
                    size_seed = (short)txtGaussSigma.Text.Length;
                }
            }
            else
            {
                if (txtGaussSigma.Text.Length > 8)
                {
                    good = false;
                    MessageBox.Show("Character password length should not exceed 8 letters");
                }
                else
                {
                    initial_seed = size_seed = 0;
                    set_password(txtGaussSigma.Text.ToString(), ref initial_seed, ref size_seed);
                }
            }
            if (!good) return;
            short tap_pos = (short)nudMaskSize.Value;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long tmp = initial_seed; 
            RGBPixel[,] arr = ImageOperations.Encrypt_No_cycles(ImageMatrix, size_seed, tap_pos, initial_seed);
            sw.Stop();
            MessageBox.Show(sw.ElapsedMilliseconds + " ms to encrypt");
            sw.Reset();
            sw.Start();
            HuffmanTree tree = new HuffmanTree(arr,initial_seed,tap_pos,size_seed);
            sw.Stop();
            MessageBox.Show(sw.ElapsedMilliseconds + " ms to compress");
            ImageOperations.DisplayImage(arr, pictureBox2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            HuffmanTree obj = new HuffmanTree(ImageMatrix, 0, 0, 0);
            obj.Write_Trees();
            sw.Stop();
            MessageBox.Show(sw.ElapsedMilliseconds + " ms to compress");
        }
    }
}