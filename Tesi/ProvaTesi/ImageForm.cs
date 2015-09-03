using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProvaTesi
{
    public partial class ImageForm : Form
    {
        public ImageForm()
        {
            InitializeComponent();
        }
        public ImageForm(String title)
        {
            InitializeComponent();
            this.Text = title;
        }
        public ImageForm(String title, Point StartPosition)
        {
            InitializeComponent();
            this.Text = title;
            this.Location = StartPosition;
        }
        public ImageForm(Bitmap image, String title)
        {

            InitializeComponent();
            this.Text = title;
            pictureBox1.ClientSize = new Size(image.Width, image.Height);
            this.ClientSize = pictureBox1.ClientSize;
            pictureBox1.Image = image;

        }
        public ImageForm(Bitmap image, String title, Point StartPosition)
        {
            
            InitializeComponent();
            this.Text = title;
            pictureBox1.ClientSize = new Size(image.Width, image.Height);
            this.ClientSize = pictureBox1.ClientSize;
            pictureBox1.Image = image;
            this.Location = StartPosition;
           
        }
        public ImageForm(Bitmap image, String title, int Height, int Width)
        {

            InitializeComponent();
            this.Text = title;
            pictureBox1.Image = image;
            pictureBox1.ClientSize = new Size(Width, Height);
            this.ClientSize = pictureBox1.ClientSize;

        }

        public Bitmap getImage()
        {
            return (Bitmap)pictureBox1.Image;
        }

        public void ChangeFrame(Bitmap frame)
        {
            Bitmap bitmap = (Bitmap)frame;
            this.Invoke((MethodInvoker)delegate
            {
                pictureBox1.Image = bitmap;
                pictureBox1.Refresh();
                this.ClientSize = pictureBox1.Image.Size;
            });
        }

        private void ImageForm_Load(object sender, EventArgs e)
        {
            
          
        }
    }
}
