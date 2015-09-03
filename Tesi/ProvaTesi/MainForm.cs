using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Kinect;
using Emgu;
using Emgu.CV.BgSegm;
using Emgu.CV.Structure;
using AForge;
using AForge.Math;
using AForge.Video;
using AForge.Controls;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Vision.Motion;
using AForge.Vision;
using AForge.Imaging.Filters;
using AForge.MachineLearning;
using AForge.Genetic;
using AForge.Math.Geometry;
using System.Drawing.Imaging;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Imaging.Converters;
using Emgu.CV.BgSegm;
using Emgu.CV;
using Accord.Statistics.Analysis;


namespace ProvaTesi
{
    public partial class MainForm : Form
    {
        int currentImageIndex = 0;
        Bitmap[] imagesToDisplay = new Bitmap[197];
        VideoSourcePlayer videoPlayer = new VideoSourcePlayer();
        
        FileVideoSource fileSource = new FileVideoSource();
        int numberOfClick = 0;
        bool CreateQuadrilater = false;
        List<WhiteKey> whiteKeys = new List<WhiteKey>();
        List<BlackKey> blackKeys = new List<BlackKey>();
        KeyboardPiano Keyboard = new KeyboardPiano();
        IntPoint c1, c2, c3, c4;
        IntPoint[] corners = new IntPoint[4];
        IntPoint[] cornersClient = new IntPoint[4];
        Bitmap oldFrameColor;
        Bitmap backgroundKeyboard;
      //  Bitmap frameColor = new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//back.jpg");
       Bitmap frameColor = new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//Dataset//Dataset10//BackgroundMod.bmp");
        //Bitmap frameColor = new Bitmap("C://Users//Carmine//Desktop//Materiale tesi//Progetto//Implementazioni//PianoKeyDetector-master//images//CallMeMaybe.jpg");
        //C:\Users\Carmine\Desktop\Materiale tesi\Progetto\Implementazioni\PianoKeyDetector-master\images
        double MaxR, MinR, MaxG, MinG;
        ImageForm HandLeftForm = new ImageForm(new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//hand.png"), "Left Hand",new System.Drawing.Point(50,30));
        ImageForm HandRightForm = new ImageForm(new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//hand.png"), "Right Hand", new System.Drawing.Point(50, 280));
       // ImageForm BackHandLeft = new ImageForm(new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//hand.png"), "Back Left Hand", new System.Drawing.Point(380, 30));
        //ImageForm BackHandRight = new ImageForm(new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//hand.png"), "Back Right Hand", new System.Drawing.Point(380, 280));
       // ImageForm HandExtractLeft = new ImageForm(new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//hand.png"), "Back Left Extract", new System.Drawing.Point(700, 30));
        //ImageForm HandExtractRight = new ImageForm(new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//hand.png"), "Back Right Extract", new System.Drawing.Point(700, 280));
        //ImageForm hand = new ImageForm(new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//hand.png"), "Back Right Extract", new System.Drawing.Point(1080, 30));
        ImageForm HandExtractLeft = new ImageForm(new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//hand.png"), "Back Left Extract", new System.Drawing.Point(380, 30));
        ImageForm HandExtractRight = new ImageForm(new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//hand.png"), "Back Right Extract", new System.Drawing.Point(380, 280));
        public MainForm()
        {
            InitializeComponent();
            HandExtractLeft.Show();
            HandExtractRight.Show();
            //hand.Show();
            HandLeftForm.Show();
            HandRightForm.Show();
           // BackHandLeft.Show();
            //BackHandRight.Show();
           // frameColor = CropImage(frameColor, new Rectangle(0, 400, frameColor.Width, frameColor.Height - 400));
            List<Bitmap> listBitmap = new List<Bitmap>();
            for (int i = 2; i <= 198; i++)
            {
               // Bitmap frame = new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//Dataset//Dataset10//" + i.ToString() + "_Color.bmp");
                string path = "C://Users//Carmine//Desktop//MaterialeTesi//Dataset//Dataset10//" + i.ToString() + "_ColorMod.bmp";
              //  MessageBox.Show(path);
                Bitmap frame = new Bitmap(path);
              //  frame = CropImage(frame, new Rectangle(0, 400, frame.Width, frame.Height - 400));
                listBitmap.Add(frame);
            }
            //MessageBox.Show(listBitmap.Count.ToString());
            imagesToDisplay = listBitmap.ToArray();
                //videoPlayer.VideoSource = new FileVideoSource("C://Users//Carmine//Desktop//MaterialeTesi//Video//prova2.avi");
                videoPlayer.VideoSource = new FileVideoSource("C://Users//Carmine//Desktop//prova10.avi");
            videoPlayer.NewFrame += new VideoSourcePlayer.NewFrameHandler(videoPlayer_NewFrame);
            pictureBox1.ClientSize = new Size(frameColor.Width, frameColor.Height);
            this.ClientSize = pictureBox1.ClientSize;
            pictureBox1.Image = frameColor;
            Bitmap PIL = new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//PIL//PIL.jpeg");
            List<double> R = new List<double>();
            List<double> G = new List<double>();
            for(int i = 0; i < PIL.Width; i++)
            {
                for(int j = 0; j < PIL.Height; j++)
                {
                    byte r = PIL.GetPixel(i, j).R;
                    byte g = PIL.GetPixel(i, j).G;
                    byte b = PIL.GetPixel(i, j).B;
                    double normR = (double)r / (double)(r + g + b);
                    double normG = (double)g / (double)(r + g + b);
                    R.Add(normR);
                    G.Add(normG);
                }
            }
            MaxR = R.Max(); MaxG = G.Max();
            MinR = R.Min(); MinG = G.Min();


          /*  List<Bitmap> listBack = new List<Bitmap>();
            for(int i = 2; i <= 111; i++)
            {
                listBack.Add(new Bitmap("C://Users//Carmine//Desktop//MaterialeTesi//Dataset//Dataset10//" + i + "_Color.bmp"));
            }
            MessageBox.Show("Fatto");

            int rTot = 0, gTot = 0, bTot = 0;
            Bitmap BackgroundKeyboard = new Bitmap(frameColor.Width, frameColor.Height);
                for(int i = 0; i < frameColor.Width; i++)
                {
                    for(int j = 0; j < frameColor.Height; j++)
                    {
                        rTot = gTot = bTot = 0;
                        for (int k = 0; k < listBack.Count; k++)
                        {
                            rTot += listBack[k].GetPixel(i, j).R;
                            gTot += listBack[k].GetPixel(i, j).G;
                            bTot += listBack[k].GetPixel(i, j).B;
                        }
                        BackgroundKeyboard.SetPixel(i, j, new RGB(Convert.ToByte(rTot / listBack.Count), Convert.ToByte(gTot / listBack.Count), Convert.ToByte(bTot / listBack.Count)).Color);
                    }
                }

               // new ImageForm(BackgroundKeyboard, "background").Show();
                frameColor = BackgroundKeyboard;
                frameColor.Save("C://Users//Carmine//Desktop//MaterialeTesi//Dataset//Dataset10//Background.bmp");
                MessageBox.Show("Fatto2");*/
            


        }

        public double Mean(List<double> valueList)
        {
            double M = 0.0;
            double S = 0.0;
            int k = 1;
            foreach(double value in valueList)
            {
                S += value;
                k++;
            }
            M = S / k;
            return M;
            
        }
        public double StdDev(List<double> valueList)
        {
            double M = 0.0;
            double S = 0.0;
            int k = 1;
            foreach (double value in valueList)
            {
                double tmpM = M;
                M += (value - tmpM) / k;
                S += (value - tmpM) * (value - M);
                k++;
            }
            return Math.Sqrt(S / (k - 2));
        }
        Bitmap DetectSkinColor(Bitmap Hand)
        {
            Bitmap result = Hand;
        /*    ImageStatistics stat = new ImageStatistics(Hand);
            List<double> valuesR = new List<double>();
            List<double> valuesG = new List<double>();
            List<double> valuesI = new List<double>(); 
            for (int i = 0; i < result.Width; i++)
            {
                for (int j = 0; j < result.Height; j++)
                {
                    byte r = result.GetPixel(i, j).R;
                    byte g = result.GetPixel(i, j).G;
                    byte b = result.GetPixel(i, j).B;
                    int I = r + g + b;
                    if (I != 0)
                    {
                        valuesR.Add(r / I);
                        valuesG.Add(g / I);
                        valuesI.Add(Convert.ToDouble(I));

                    }
                }
            }
            double sigmaR = StdDev(valuesR);
            double sigmaG = StdDev(valuesG);
            double sigmaI = StdDev(valuesI);
            double meanI = Mean(valuesI);
            double meanR = Mean(valuesR);
            double meanG = Mean(valuesG);*/
            
            for (int i = 0; i < result.Width; i++)
            {
                for (int j = 0; j < result.Height; j++)
                {
                    byte r = result.GetPixel(i, j).R;
                    byte g = result.GetPixel(i, j).G;
                    byte b = result.GetPixel(i, j).B;
                   /* int I = r + g + b; double aI = 100, aR = 100, aG = 100;
                    if (I != 0)
                    {
                       // MessageBox.Show("okok");
                        double R = r / I; double G = g / I;
                        if (Math.Abs(R - meanR) < sigmaR * aR)
                        {

                            if (Math.Abs(G - meanG) < sigmaG * aG)
                            {
                               
                                if (Math.Abs(I - meanI) < sigmaI * aI)
                                {
                                  //  MessageBox.Show(Math.Abs(I - meanI) + " " + sigmaI + " " + aI + " " + sigmaI * aI);
                                    result.SetPixel(i, j, Color.White);
                                }
                                else
                                    result.SetPixel(i, j, Color.Black);
                            }
                            else
                                result.SetPixel(i, j, Color.Black);
                        }
                        else
                            result.SetPixel(i, j, Color.Black);
                    }
                    
                 /*   double normR = (double)r / (double)(r + g + b);
                    double normG = (double)g / (double)(r + g + b);
                    HSL hsl = new HSL();
                    hsl = HSL.FromRGB(new RGB(r, g, b));
                    if((r >= 90 && r <= 255) && (normG >= 0.277f && normG <=0.335) && ((hsl.Hue >= 0 && hsl.Hue <= 34) || (hsl.Hue >= 347 && hsl.Hue <= 359 )) && (hsl.Saturation >=0.2 && hsl.Saturation <= 0.757))
                    {
                        result.SetPixel(i, j, Color.White);
                    }
                    else
                        result.SetPixel(i, j, Color.Black);
                   /* if(IsSkin(result.GetPixel(i,j)))
                    {
                        result.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        result.SetPixel(i, j, Color.Black);
                    }*/
                 /*  if(r > 220 && g > 210 && b > 170 && Math.Abs(r-g) <= 15 && b < r && b < g)
                   {
                       result.SetPixel(i, j, Color.White);
                   }
                   else
                   {
                       result.SetPixel(i, j, Color.Black);
                   }*/
                /*    double I1, I2, I3;
                    I1 = 1 / 3 * (r + g + b); I2 = 1 / 2 * (r - b); I3 = 1 / 4 * (2 * g - r - b); ;
                    double I = I1;
                    double S = Math.Sqrt(Math.Pow(I2, 2) + Math.Pow(I3, 2));
                    double H = Math.Atan(I3 / I2);
                    if(I > 40)
                    {
                        MessageBox.Show(I.ToString());
                        if(S > 13 && S < 110)
                        {
                            if(H > 0 &&  H < 28)
                            {
                                result.SetPixel(i, j, Color.White);
                            }
                            else
                                result.SetPixel(i, j, Color.Black);
                        }
                        else
                        {
                            if (S > 13 && S < 75)
                            {
                                if (H > 309 && H < 360)
                                {
                                    result.SetPixel(i, j, Color.White);
                                }
                                else
                                    result.SetPixel(i, j, Color.Black);
                            }
                            else
                                result.SetPixel(i, j, Color.Black);
                        }
                    }*/
                     
                    /*
                    if(hsl.Luminance > 40 && (hsl.Saturation > 0.2 && hsl.Saturation < 0.6) && (hsl.Hue > 0 && hsl.Hue < 25))
                    {
                        Hand.SetPixel(i, j, Color.White);
                    }
                    else
                        Hand.SetPixel(i, j, Color.Black);*/
                   List<int> rgb = new List<int> {r, g, b};
                    if(  (r > 95 && g > 40 && b > 20) &&  ((rgb.Max() - rgb.Min()) > 15) && (Math.Abs(r - g) > 15) && (r>g && r > b)  )
                    {
                        Hand.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        Hand.SetPixel(i, j, Color.Black);
                    }
                  /* double normR = (double)r / (double)(r + g + b);
                    double normG = (double)g / (double)(r + g + b);

                    if ((normR >= MinR && normR <= MaxR) && (normG >= MinG && normG <= MaxG))
                    {
                        Hand.SetPixel(i, j, Color.White);
                    }
                    else
                        Hand.SetPixel(i, j, Color.Black);

                 //  if (r < 100 && g < 100 && b < 100)
                   //     Hand.SetPixel(i, j, Color.Black);
                 /*   if(r >= 160 && g >= 90 && b >= 93)
                        Hand.SetPixel(i, j, Color.White);
                    else
                        Hand.SetPixel(i, j, Color.Black);*/
                  
                }
            }
            return result;
        }

    
        int cont = 1;
        Bitmap keyboard= null;
        private void videoPlayer_NewFrame(object sender, ref Bitmap Image)
        {
            Bitmap bitmap = (Bitmap)Image;
            this.Invoke((MethodInvoker)delegate
            {
                lock (pictureBox1)
                {
                    pictureBox1.Image = bitmap;
                    frameColor = bitmap;
                    pictureBox1.Refresh();
                    // this.ClientSize = new Size(pictureBox1.Image.Size.Width-100, pictureBox1.Image.Size.Height-200);
                }
            });
            //  bitmap = DetectSkinColor(bitmap);
            Bitmap dest = quad.Apply(bitmap);
            //dest.Save("C://Users//Carmine//Desktop//MaterialeTesi//Dataset//Dataset9//Quad_" + cont.ToString() + ".bmp");

            keyboard = dest;
            dest = Grayscale.CommonAlgorithms.BT709.Apply(dest);
            ThresholdedDifference th = new ThresholdedDifference();
            th.OverlayImage = backgroundKeyboard;
            dest = th.Apply(dest);
            new Opening().ApplyInPlace(dest);
            //  new Median(9).ApplyInPlace(dest);
            //  new Erosion().ApplyInPlace(dest);
            // new OtsuThreshold().ApplyInPlace(dest);
            //Bitmap difference = dest;

            /*
            dest = CreateNonIndexedImage(dest);
            for (int i = 0; i < dest.Width; i++)
            {
                for (int j = 0; j < dest.Height; j++)
                {
                    Color noto = dest.GetPixel(i, j);
                    Color c = Color.FromArgb(255 - noto.R, 255 - noto.G, 255 - noto.B);
                    dest.SetPixel(i, j, c);
                }

            }*/

            ConnectedComponentsLabeling com = new ConnectedComponentsLabeling();

            com.Apply(dest);


            // com.MinHeight = 100;
            Blob[] blobs = com.BlobCounter.GetObjectsInformation();

            /*  var rectanglesToClear = from blob in blobs select blob.Rectangle;
             // dest = CreateNonIndexedImage(dest);
              BitmapData datidest = dest.LockBits(new Rectangle(0, 0, dest.Width, dest.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        dest.PixelFormat);
              foreach (var rect in rectanglesToClear)
              {
                  if (rect.Width < 50)
                      Drawing.FillRectangle(datidest, rect, Color.Black);
              }
              dest.UnlockBits(datidest);
            */

            int maxArea1 = blobs[0].Area;
            int maxArea2 = blobs[1].Area;
            Blob maxBlob1 = blobs[0];
            Blob maxBlob2 = blobs[1];
            for (int i = 2; i < blobs.Length; i++)
            {
                if (blobs[i].Area > maxArea1)
                {
                    maxArea2 = maxArea1;
                    maxArea1 = blobs[i].Area;
                    maxBlob2 = maxBlob1;
                    maxBlob1 = blobs[i];
                }
                else if (maxArea2 < blobs[i].Area)
                {
                    maxArea2 = blobs[i].Area;
                    maxBlob2 = blobs[i];
                }
            }

            // CropImage(keyboard, maxBlob1.Rectangle).Save("C://Users//Carmine//Desktop//Color2//C" + cont.ToString() + ".bmp"); cont++;
            Bitmap hand1 = CropImage(keyboard, maxBlob1.Rectangle);
            Bitmap hand2 = CropImage(keyboard, maxBlob2.Rectangle);
           // Bitmap backHand1 = CropImage(backgroundKeyboard, maxBlob1.Rectangle);
            //Bitmap backHand2 = CropImage(backgroundKeyboard, maxBlob2.Rectangle);


            //Bitmap Hand1Extract = BackgroundSubstraction(hand1, backHand1);
            // Bitmap Hand2Extract = BackgroundSubstraction(hand2, backHand2);

            Bitmap Hand1Extract = DetectSkinColor((Bitmap)hand1.Clone());
            Bitmap Hand2Extract = DetectSkinColor((Bitmap)hand2.Clone());
            /* Hand1Extract = Grayscale.CommonAlgorithms.BT709.Apply(Hand1Extract);
             Hand2Extract = Grayscale.CommonAlgorithms.BT709.Apply(Hand2Extract);
             new Erosion().ApplyInPlace(Hand1Extract);
            new Erosion().ApplyInPlace(Hand2Extract);*/

            /*  hand1 = Grayscale.CommonAlgorithms.BT709.Apply(hand1);
              hand2 = Grayscale.CommonAlgorithms.BT709.Apply(hand2);
              backHand1 = Grayscale.CommonAlgorithms.BT709.Apply(backHand1);
              backHand2 = Grayscale.CommonAlgorithms.BT709.Apply(backHand2);
              MoveTowards th1 = new MoveTowards(backHand1, 50);
              Bitmap Hand1Normal = th1.Apply(hand1);
              MoveTowards th2 = new MoveTowards(backHand2, 50);
              Bitmap Hand2Normal = th2.Apply(hand2);
              Bitmap Hand1Positive, Hand1Negative, Hand2Positive, Hand2Negative;
           /   Hand1Positive = new Bitmap(hand1.Width, hand1.Height);
              Hand1Negative = new Bitmap(hand1.Width, hand1.Height);
              Hand2Positive = new Bitmap(hand2.Width, hand2.Height);
              Hand2Negative = new Bitmap(hand2.Width, hand2.Height);
             * */
            /*  for(int i = 0; i < hand1.Width; i++)
              {
                  for(int j = 0; j < hand1.Height; j++)
                  {
                      Hand1Positive.SetPixel(i, j, Color.Black);
                      Hand1Negative.SetPixel(i, j, Color.Black);
                  }
              }
              for (int i = 0; i < hand2.Width; i++)
              {
                  for (int j = 0; j < hand2.Height; j++)
                  {
                      Hand2Positive.SetPixel(i, j, Color.Black);
                      Hand2Negative.SetPixel(i, j, Color.Black);
                  }
              }*/
            /*  Hand1Normal = new GrayscaleToRGB().Apply(Hand1Normal);
              Hand2Normal = new GrayscaleToRGB().Apply(Hand2Normal);
              backHand1 = new GrayscaleToRGB().Apply(backHand1);
              backHand2 = new GrayscaleToRGB().Apply(backHand2);
              hand1 = new GrayscaleToRGB().Apply(hand1);
              hand2 = new GrayscaleToRGB().Apply(hand2);
              Bitmap Hand1Extract = BackgroundSubstractionRGB(Hand1Normal, backHand1, ref Hand1Positive, ref Hand1Negative);
              Bitmap Hand2Extract = BackgroundSubstractionRGB(Hand2Normal, backHand2, ref Hand2Positive, ref Hand2Negative);
            /*  Hand1Normal = new GrayscaleToRGB().Apply(Hand1Normal);
              Hand2Normal = new GrayscaleToRGB().Apply(Hand2Normal);
              backHand1 = new GrayscaleToRGB().Apply(backHand1);
              backHand2 = new GrayscaleToRGB().Apply(backHand2);
              Bitmap Hand1Extract = BackgroundSubstraction(Hand1Normal, backHand1);
              Bitmap Hand2Extract = BackgroundSubstraction(Hand2Normal, backHand2);*/
            if (maxBlob1.Rectangle.X > maxBlob2.Rectangle.X)
            {
                HandExtractLeft.ChangeFrame(Hand2Extract);
                HandLeftForm.ChangeFrame(hand2);
                // BackHandLeft.ChangeFrame(backHand2);

                HandRightForm.ChangeFrame(hand1);
                //BackHandRight.ChangeFrame(backHand1);
                HandExtractRight.ChangeFrame(Hand1Extract);
            }
            else
            {
                HandLeftForm.ChangeFrame(hand1);
                //BackHandLeft.ChangeFrame(backHand1);
                HandExtractLeft.ChangeFrame(Hand1Extract);

                HandRightForm.ChangeFrame(hand2);
                //  BackHandRight.ChangeFrame(backHand2);
                HandExtractRight.ChangeFrame(Hand2Extract);

            }

            Bitmap destClone = (Bitmap)dest.Clone();
            destClone = new GrayscaleToRGB().Apply(destClone);
            BitmapData dati = destClone.LockBits(new Rectangle(0, 0, destClone.Width, destClone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, destClone.PixelFormat);
            Drawing.Rectangle(dati, maxBlob1.Rectangle, Color.Red);
            Drawing.Rectangle(dati, maxBlob2.Rectangle, Color.Blue);
            Drawing.Rectangle(dati, new Rectangle(Convert.ToInt32(maxBlob1.CenterOfGravity.X - 2), Convert.ToInt32(maxBlob1.CenterOfGravity.Y - 2), 4, 4), Color.Red);
            Drawing.Rectangle(dati, new Rectangle(Convert.ToInt32(maxBlob2.CenterOfGravity.X - 2), Convert.ToInt32(maxBlob2.CenterOfGravity.Y - 2), 4, 4), Color.Red);

            destClone.UnlockBits(dati);

            VideoForm.ChangeFrame(destClone);


            // Thread.Sleep(30);



        }


        public Bitmap BackgroundSubstraction(Bitmap source, Bitmap background)
        {
            Bitmap result = new Bitmap(source.Width, source.Height);
            for(int i = 0; i < source.Width; i++)
            {
                for(int j = 0; j < source.Height; j++)
                {
                    
                    Color s = source.GetPixel(i, j);
                    Color b = background.GetPixel(i, j);
                    byte resultR = (byte)(s.R - b.R);
                    byte resultG = (byte)(s.G - b.G);
                    byte resultB = (byte)(s.B - b.B);
                  //  if (resultR < 0) resultR += 255;
                   // if (resultG < 0) resultG += 255;
                   // if (resultB < 0) resultB += 255;
                  /*  HSL colorS = HSL.FromRGB(new RGB(s.R, s.G, s.B));
                    HSL colorB = HSL.FromRGB(new RGB(b.R, b.G, b.B));
                    HSL colorR = new HSL(Math.Abs(colorS.Hue - colorB.Hue), Math.Abs(colorS.Saturation - colorB.Saturation), Math.Abs(colorS.Luminance - colorB.Luminance));
                  */
                    Color r = new RGB((byte)Math.Abs(resultR), (byte)Math.Abs(resultG), (byte)Math.Abs(resultB)).Color;
                    result.SetPixel(i, j, r);
                }
            }
            return result;
        }
        public Bitmap BackgroundSubstractionRGB(Bitmap source, Bitmap background, ref Bitmap resultP, ref Bitmap resultN)
        {
            Bitmap result = new Bitmap(source.Width, source.Height);
            
            for (int i = 0; i < source.Width; i++)
            {
                for (int j = 0; j < source.Height; j++)
                {

                    Color s = source.GetPixel(i, j);
                    Color b = background.GetPixel(i, j);
                    byte resultR = (byte)(s.R - b.R);
                    byte resultG = (byte)(s.G - b.G);
                    byte resultB = (byte)(s.B - b.B);
                    
                  //  MessageBox.Show(resultR + " " + resultG + " " + resultB);
                    if(resultR > 0 && resultB > 0 && resultG > 0)
                    {
                        resultN.SetPixel(i, j, Color.Black);
                        resultP.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        resultP.SetPixel(i, j, Color.Black);
                        resultN.SetPixel(i, j, Color.White);
                    }
                    if (resultR < 0) resultR += 255;
                    if (resultG < 0) resultG += 255;
                    if (resultB < 0) resultB += 255;
                    Color r = new RGB(resultR, resultG, resultB).Color;
                    result.SetPixel(i, j, r);
                }
            }
            return result;
        }
        public Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }
        struct DistancesHand
        {
            public Int64 distance;
            public IntPoint point;
        };

        private void Prova_Load(object sender, EventArgs e)
        {
           // Accord.MachineLearning.DecisionTrees.DecisionTree
        }

        #region DetectKeys
    
        ImageForm VideoForm;
        QuadrilateralTransformation quad;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (CreateQuadrilater)
            {
                if (numberOfClick < 4)
                {
                    int X = (e.X * pictureBox1.Image.Width) / pictureBox1.ClientSize.Width;
                    int Y = (e.Y * pictureBox1.Image.Height) / pictureBox1.ClientSize.Height;
                    corners[numberOfClick] = new IntPoint(X, Y);
                    cornersClient[numberOfClick] = new IntPoint(e.X, e.Y);
                    numberOfClick++;
                    if (numberOfClick == 1)
                    {
                        oldFrameColor = frameColor;
                        c1 = new IntPoint(X, Y);
                    }
                    else if (numberOfClick == 2)
                    {
                        c2 = new IntPoint(X, Y);
                         BitmapData dati = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                            frameColor.PixelFormat);
                        Drawing.Line(dati, c1, c2, Color.Gold);
                        frameColor.UnlockBits(dati);
                        pictureBox1.Image = frameColor;
                    }
                    else if (numberOfClick == 3)
                    {
                        c3 = new IntPoint(X, Y);
                        BitmapData dati = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                           frameColor.PixelFormat);
                        Drawing.Line(dati, c2, c3, Color.Gold);
                        frameColor.UnlockBits(dati);
                        pictureBox1.Image = frameColor;
                    }
                    else if (numberOfClick == 4)
                    {
                        c4 = new IntPoint(X, Y);
                        BitmapData dati = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                            frameColor.PixelFormat);
                        Drawing.Line(dati, c3, c4, Color.Gold);
                        Drawing.Line(dati, c4, c1, Color.Gold);
                        frameColor.UnlockBits(dati);
                        numberOfClick = 0;
                        pictureBox1.Image = frameColor;
                        numberOfClick = 0;
                        quad = new QuadrilateralTransformation(new List<IntPoint> { corners[0], corners[1], corners[2], corners[3] });
                        Bitmap newFrame = quad.Apply(frameColor);
                        VideoForm = new ImageForm(newFrame, "Quadrilater Transform");
                        VideoForm.Show();
                        
                        pictureBox1.Image = oldFrameColor;
                        frameColor = newFrame;
                        backgroundKeyboard = newFrame;
                        backgroundKeyboard = Grayscale.CommonAlgorithms.BT709.Apply(backgroundKeyboard);
                        CreateQuadrilater = false;
                        pictureBox1.Cursor = Cursors.Default;

                    }
                }
            }
        }

        private void drawROIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateQuadrilater = true;
            pictureBox1.Cursor = Cursors.Cross;
        }

        private void chiudiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        public Bitmap CreateNonIndexedImage(System.Drawing.Image src)
        {
            Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }

  

        private void detectKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap frameGray = Grayscale.CommonAlgorithms.BT709.Apply(frameColor);
            OtsuThreshold filterThresh = new OtsuThreshold();
            Bitmap frameBW = filterThresh.Apply(frameGray);
            new ImageForm(frameBW, "Threshold").Show();

            new Closing().ApplyInPlace(frameBW);
            new Erosion().ApplyInPlace(frameBW);
            new Opening().ApplyInPlace(frameBW);
            frameBW = CreateNonIndexedImage(frameBW);
            for (int i = 0; i < frameBW.Width; i++)
            {
                for (int j = 0; j < frameBW.Height; j++)
                {
                    Color noto = frameBW.GetPixel(i, j);
                    Color c = Color.FromArgb(255 - noto.R, 255 - noto.G, 255 - noto.B);
                    frameBW.SetPixel(i, j, c);
                }
            }
            new ImageForm(frameBW, "Threshold after morph").Show();
            ConnectedComponentsLabeling cp = new ConnectedComponentsLabeling();
          //  cp.MinHeight = 50;
            //cp.MinWidth = 8;
            Bitmap res = cp.Apply(frameBW);
            new ImageForm(res, "Blobs Result").Show();
           // Rectangle[] rects = cp.BlobCounter.GetObjectsRectangles();
            Blob[] blobsInitial = cp.BlobCounter.GetObjectsInformation();
            List<Blob> blobsList = new List<Blob>();
            foreach(Blob b in blobsInitial)
            {
                if (b.Rectangle.Height > 20) blobsList.Add(b);
            }
            Blob[] blobs = blobsList.ToArray();
           // rects = rects.OrderBy(r => r.Right).ToArray();
            blobs = blobs.OrderBy(b => b.Rectangle.Right).ToArray();
            List<int> slopes = new List<int>();
            List<Rectangle> rectsFull = new List<Rectangle>();
            List<Blob> blobsFull = new List<Blob>();

            // int index = 0;
            foreach (Blob b in blobs)
            {
                blackKeys.Add(new BlackKey(b));
                IntPoint corner1, corner2, corner3, corner4;
                BitmapData dati = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                       frameColor.PixelFormat);
                corner1 = new IntPoint(b.Rectangle.X, b.Rectangle.Y);
                corner2 = new IntPoint(b.Rectangle.Right, b.Rectangle.Top);
                corner3 = new IntPoint(b.Rectangle.Right, b.Rectangle.Bottom);
                corner4 = new IntPoint(b.Rectangle.Left, b.Rectangle.Bottom);
                Drawing.Rectangle(dati, new Rectangle(new System.Drawing.Point(corner1.X - 2, corner1.Y - 2), new Size(4, 4)), Color.Blue);
                Drawing.Rectangle(dati, new Rectangle(new System.Drawing.Point(corner2.X - 2, corner2.Y - 2), new Size(4, 4)), Color.Blue);
                Drawing.Rectangle(dati, new Rectangle(new System.Drawing.Point(corner3.X - 2, corner3.Y - 2), new Size(4, 4)), Color.Blue);
                Drawing.Rectangle(dati, new Rectangle(new System.Drawing.Point(corner4.X - 2, corner4.Y - 2), new Size(4, 4)), Color.Blue);
                Drawing.FillRectangle(dati, b.Rectangle, Color.Blue);
                frameColor.UnlockBits(dati);

                //pictureBox1.Image = frameColor;
            }

          //  ShowImageForm(frameColor, "Rect");
            new ImageForm(frameColor, "rectangles").Show();
          //  ShowImageForm(frameColor, "Rectangles");
            double NumbersOfKeys = Math.Round(Convert.ToDouble(cp.ObjectCount / 5)) + 1;


            int indexMiddleThree = 0, indexRightThree = 0, indexLeftThree = 0, indexRightTwo = 0, indexLeftTwo = 0;

            Blob bMiddleThree, bRightThree, bLeftThree, bRightTwo, bLeftTwo;
            bMiddleThree = bRightThree = bLeftThree = bRightTwo = bLeftTwo = null;
            double slopeMiddleThree = 0, slopeRightThree = 0, slopeLeftThree = 0, slopeRightTwo = 0, slopeLeftTwo = 0;
            List<LineSegment> linesWhiteKeys = new List<LineSegment>();

            //Detection Lines
            for (int i = 0; i < NumbersOfKeys; i++)
            {
                //indici per cambiare ottava
                indexMiddleThree = 1 + i * 5;
                indexRightThree = i * 5;
                indexLeftThree = 2 + i * 5;
                indexRightTwo = 4 + i * 5;
                indexLeftTwo = 3 + i * 5;

                BitmapData dati = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, frameColor.PixelFormat);

                if (indexMiddleThree < blobs.Length)
                {
                    float x1MiddleThree = 0, x2MiddleThree = 0, y1MiddleThree = 0, y2MiddleThree = 0;
                    bMiddleThree = blobs[indexMiddleThree];
                    x1MiddleThree = (bMiddleThree.Rectangle.Right + bMiddleThree.Rectangle.X) / 2; y1MiddleThree = bMiddleThree.Rectangle.Bottom; y2MiddleThree = frameColor.Height;
                    // Drawing.Line(dati, new IntPoint(Convert.ToInt32(x1MiddleThree), bMiddleThree.Rectangle.Top), new IntPoint(Convert.ToInt32(bMiddleThree.CenterOfGravity.X), Convert.ToInt32(bMiddleThree.CenterOfGravity.Y)), Color.Red);
                    if (bMiddleThree.CenterOfGravity.X != x1MiddleThree)
                    {
                        slopeMiddleThree = Math.Abs(bMiddleThree.CenterOfGravity.Y - bMiddleThree.Rectangle.Top) / (bMiddleThree.CenterOfGravity.X - x1MiddleThree);
                        x2MiddleThree = Convert.ToSingle(Math.Floor((y2MiddleThree - y1MiddleThree) / slopeMiddleThree) + (x1MiddleThree));
                    }
                    else
                    {
                        x2MiddleThree = x1MiddleThree;
                    }
                    Drawing.Line(dati, new IntPoint(Convert.ToInt32(x1MiddleThree), Convert.ToInt32(y1MiddleThree)), new IntPoint(Convert.ToInt32(x2MiddleThree), Convert.ToInt32(y2MiddleThree)), Color.Red);
                    linesWhiteKeys.Add(new LineSegment(new AForge.Point(x1MiddleThree, y1MiddleThree), new AForge.Point(x2MiddleThree, y2MiddleThree)));

                }

                if (indexRightThree < blobs.Length)
                {
                    float x1RightThree = 0, x2RightThree = 0, y1RightThree = 0, y2RightThree = 0;
                    bRightThree = blobs[indexRightThree];
                    x1RightThree = (bRightThree.Rectangle.Right); y1RightThree = bRightThree.Rectangle.Bottom; y2RightThree = frameColor.Height;
                    // Drawing.Line(dati, new IntPoint(Convert.ToInt32((bRightThree.Rectangle.X + bRightThree.Rectangle.Right) / 2), bRightThree.Rectangle.Top), new IntPoint(Convert.ToInt32(bRightThree.CenterOfGravity.X), Convert.ToInt32(bRightThree.CenterOfGravity.Y)), Color.Red);
                    if ((bRightThree.CenterOfGravity.X + ((bRightThree.Rectangle.Width / 2) /*+ 3*/)) != x1RightThree)
                    {
                        slopeRightThree = Math.Abs(bRightThree.CenterOfGravity.Y - bRightThree.Rectangle.Top) / ((bRightThree.CenterOfGravity.X + ((bRightThree.Rectangle.Width / 2) /*+ 3*/)) - x1RightThree - 3);
                        x2RightThree = Convert.ToSingle(Math.Floor((y2RightThree - y1RightThree) / slopeRightThree) + (x1RightThree));
                    }
                    else
                    {
                        x2RightThree = x1RightThree;
                    }
                    Drawing.Line(dati, new IntPoint(Convert.ToInt32(x1RightThree - 3), Convert.ToInt32(y1RightThree)), new IntPoint(Convert.ToInt32(x2RightThree), Convert.ToInt32(y2RightThree)), Color.Red);
                    linesWhiteKeys.Add(new LineSegment(new AForge.Point(x1RightThree - 3, y1RightThree), new AForge.Point(x2RightThree, y2RightThree)));
                }

                if (indexLeftThree < blobs.Length)
                {
                    float x1LeftThree = 0, x2LeftThree = 0, y1LeftThree = 0, y2LeftThree = 0;
                    bLeftThree = blobs[indexLeftThree];
                    x1LeftThree = bLeftThree.Rectangle.X; y1LeftThree = bLeftThree.Rectangle.Bottom; y2LeftThree = frameColor.Height;
                    // Drawing.Line(dati, new IntPoint(Convert.ToInt32((bLeftThree.Rectangle.X + bLeftThree.Rectangle.Right) / 2), bLeftThree.Rectangle.Top), new IntPoint(Convert.ToInt32(bLeftThree.CenterOfGravity.X), Convert.ToInt32(bLeftThree.CenterOfGravity.Y)), Color.Red);
                    if ((bLeftThree.CenterOfGravity.X - (bLeftThree.Rectangle.Width / 2)) != x1LeftThree)
                    {
                        slopeLeftThree = Math.Abs(bLeftThree.CenterOfGravity.Y - bLeftThree.Rectangle.Top) / ((bLeftThree.CenterOfGravity.X - (bLeftThree.Rectangle.Width / 2)) - x1LeftThree + 3);
                        x2LeftThree = Convert.ToSingle(Math.Floor((y2LeftThree - y1LeftThree) / slopeLeftThree) + (x1LeftThree));
                    }
                    else
                    {
                        x2LeftThree = x1LeftThree;
                    }
                    Drawing.Line(dati, new IntPoint(Convert.ToInt32(x1LeftThree + 3), Convert.ToInt32(y1LeftThree)), new IntPoint(Convert.ToInt32(x2LeftThree), Convert.ToInt32(y2LeftThree)), Color.Red);
                    linesWhiteKeys.Add(new LineSegment(new AForge.Point(x1LeftThree + 3, y1LeftThree), new AForge.Point(x2LeftThree, y2LeftThree)));
                    if (indexLeftThree + 1 < blobs.Length)
                    {
                        float X1, Y1, X2, Y2;
                        Blob b1 = blobs[indexLeftThree], b2 = blobs[indexLeftThree + 1];
                        Y1 = 0; X1 = (b1.CenterOfGravity.X + b2.CenterOfGravity.X) / 2;
                        X2 = X1; Y2 = frameColor.Height;
                        Drawing.Line(dati, new IntPoint(Convert.ToInt32(X1), Convert.ToInt32(Y1)), new IntPoint(Convert.ToInt32(X2), Convert.ToInt32(Y2)), Color.Red);
                        linesWhiteKeys.Add(new LineSegment(new AForge.Point(X1, Y1), new AForge.Point(X2, Y2)));
                    }
                }

                if (indexLeftTwo < blobs.Length)
                {
                    float x1LeftTwo = 0, x2LeftTwo = 0, y1LeftTwo = 0, y2LeftTwo = 0;
                    bLeftTwo = blobs[indexLeftTwo];
                    x1LeftTwo = bLeftTwo.Rectangle.Right; y1LeftTwo = bLeftTwo.Rectangle.Bottom; y2LeftTwo = frameColor.Height;
                    // Drawing.Line(dati, new IntPoint(Convert.ToInt32((bLeftTwo.Rectangle.X + bLeftTwo.Rectangle.Right) / 2), bLeftTwo.Rectangle.Top), new IntPoint(Convert.ToInt32(bLeftTwo.CenterOfGravity.X), Convert.ToInt32(bLeftTwo.CenterOfGravity.Y)), Color.Red);
                    if ((bLeftTwo.CenterOfGravity.X + bLeftTwo.Rectangle.Width / 2) != x1LeftTwo)
                    {
                        slopeLeftTwo = Math.Abs(bLeftTwo.CenterOfGravity.Y - bLeftTwo.Rectangle.Top) / ((bLeftTwo.CenterOfGravity.X + bLeftTwo.Rectangle.Width / 2) - x1LeftTwo - 3);
                        x2LeftTwo = Convert.ToSingle(Math.Floor((y2LeftTwo - y1LeftTwo) / slopeLeftTwo) + (x1LeftTwo));
                    }
                    else
                    {
                        x2LeftTwo = x1LeftTwo;
                    }
                    Drawing.Line(dati, new IntPoint(Convert.ToInt32(x1LeftTwo - 3), Convert.ToInt32(y1LeftTwo)), new IntPoint(Convert.ToInt32(x2LeftTwo), Convert.ToInt32(y2LeftTwo)), Color.Red);
                    linesWhiteKeys.Add(new LineSegment(new AForge.Point(x1LeftTwo - 3, y1LeftTwo), new AForge.Point(x2LeftTwo, y2LeftTwo)));
                }

                if (indexRightTwo < blobs.Length)
                {
                    float x1RightTwo = 0, x2RightTwo = 0, y1RightTwo = 0, y2RightTwo = 0;
                    bRightTwo = blobs[indexRightTwo];
                    x1RightTwo = bRightTwo.Rectangle.X; y1RightTwo = bRightTwo.Rectangle.Bottom; y2RightTwo = frameColor.Height;
                    // Drawing.Line(dati, new IntPoint(Convert.ToInt32((bRightTwo.Rectangle.X + bRightTwo.Rectangle.Right) / 2), bRightTwo.Rectangle.Top), new IntPoint(Convert.ToInt32(bRightTwo.CenterOfGravity.X), Convert.ToInt32(bRightTwo.CenterOfGravity.Y)), Color.Red);
                    if ((bRightTwo.CenterOfGravity.X - bRightTwo.Rectangle.Width / 2) != x1RightTwo)
                    {
                        slopeRightTwo = Math.Abs(bRightTwo.CenterOfGravity.Y - bRightTwo.Rectangle.Top) / ((bRightTwo.CenterOfGravity.X - bRightTwo.Rectangle.Width / 2) - x1RightTwo + 3);
                        x2RightTwo = Convert.ToSingle(Math.Floor((y2RightTwo - y1RightTwo) / slopeRightTwo) + (x1RightTwo));
                    }
                    else
                    {
                        x2RightTwo = x1RightTwo;
                    }
                    Drawing.Line(dati, new IntPoint(Convert.ToInt32(x1RightTwo + 3), Convert.ToInt32(y1RightTwo)), new IntPoint(Convert.ToInt32(x2RightTwo), Convert.ToInt32(y2RightTwo)), Color.Red);
                    linesWhiteKeys.Add(new LineSegment(new AForge.Point(x1RightTwo + 3, y1RightTwo), new AForge.Point(x2RightTwo, y2RightTwo)));
                    if (indexRightTwo + 1 < blobs.Length)
                    {
                        float X1, Y1, X2, Y2;
                        Blob b1 = blobs[indexRightTwo], b2 = blobs[indexRightTwo + 1];
                        Y1 = 0; X1 = (b1.CenterOfGravity.X + b2.CenterOfGravity.X) / 2;
                        X2 = X1; Y2 = frameColor.Height;
                        Drawing.Line(dati, new IntPoint(Convert.ToInt32(X1), Convert.ToInt32(Y1)), new IntPoint(Convert.ToInt32(X2), Convert.ToInt32(Y2)), Color.Red);
                        linesWhiteKeys.Add(new LineSegment(new AForge.Point(X1, Y1), new AForge.Point(X2, Y2)));
                    }
                }

                frameColor.UnlockBits(dati);

                //  pictureBox1.Image = frameColor;

            }
             new ImageForm(frameColor, "WhiteKeys").Show();
            linesWhiteKeys.Add(new LineSegment(new AForge.Point(0, 0), new AForge.Point(0, frameColor.Height)));
            linesWhiteKeys.Add(new LineSegment(new AForge.Point(frameColor.Width, 0), new AForge.Point(frameColor.Width, frameColor.Height)));
            linesWhiteKeys = linesWhiteKeys.OrderBy(x => x.Start.X).ToList();
            LineSegment[] linesArrayWhiteKeys = linesWhiteKeys.ToArray();

            for (int i = 0; i < linesArrayWhiteKeys.Length - 1; i++)
            {
                WhiteKey key = new WhiteKey();
                key.LeftLine = linesArrayWhiteKeys[i];
                key.RightLine = linesArrayWhiteKeys[i + 1];
                whiteKeys.Add(key);
            }

            int Resto = whiteKeys.Count % 7;
            for (int i = 0; i < whiteKeys.Count - Resto; i += 7)
            {
                BitmapData dat = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, frameColor.PixelFormat);
                whiteKeys[i].BottomRect = new Rectangle(Convert.ToInt32(whiteKeys[i].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i].RightLine.Start.Y), Convert.ToInt32(whiteKeys[i].RightLine.End.X - whiteKeys[i].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i].RightLine.End.Y - whiteKeys[i].RightLine.Start.Y));
                whiteKeys[i + 1].BottomRect = new Rectangle(Convert.ToInt32(whiteKeys[i + 1].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i + 1].RightLine.Start.Y), Convert.ToInt32(whiteKeys[i + 1].RightLine.End.X - whiteKeys[i + 1].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i + 1].RightLine.End.Y - whiteKeys[i + 1].RightLine.Start.Y));
                whiteKeys[i + 2].BottomRect = new Rectangle(Convert.ToInt32(whiteKeys[i + 2].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i + 2].RightLine.Start.Y), Convert.ToInt32(whiteKeys[i + 2].RightLine.End.X - whiteKeys[i + 2].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i + 2].RightLine.End.Y - whiteKeys[i + 2].RightLine.Start.Y));
                whiteKeys[i + 3].BottomRect = new Rectangle(Convert.ToInt32(whiteKeys[i + 3].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i + 3].LeftLine.Start.Y), Convert.ToInt32(whiteKeys[i + 3].RightLine.End.X - whiteKeys[i + 3].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i + 3].RightLine.End.Y - whiteKeys[i + 3].LeftLine.Start.Y));
                whiteKeys[i + 4].BottomRect = new Rectangle(Convert.ToInt32(whiteKeys[i + 4].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i + 4].RightLine.Start.Y), Convert.ToInt32(whiteKeys[i + 4].RightLine.End.X - whiteKeys[i + 4].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i + 4].RightLine.End.Y - whiteKeys[i + 4].RightLine.Start.Y));
                whiteKeys[i + 5].BottomRect = new Rectangle(Convert.ToInt32(whiteKeys[i + 5].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i + 5].RightLine.Start.Y), Convert.ToInt32(whiteKeys[i + 5].RightLine.End.X - whiteKeys[i + 5].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i + 5].RightLine.End.Y - whiteKeys[i + 5].RightLine.Start.Y));
                whiteKeys[i + 6].BottomRect = new Rectangle(Convert.ToInt32(whiteKeys[i + 6].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i + 6].LeftLine.Start.Y), Convert.ToInt32(whiteKeys[i + 6].RightLine.End.X - whiteKeys[i + 6].LeftLine.Start.X), Convert.ToInt32(whiteKeys[i + 6].RightLine.End.Y - whiteKeys[i + 6].LeftLine.Start.Y));
                whiteKeys[i].TopRect = new Rectangle(whiteKeys[i].BottomRect.X, 0, (((whiteKeys[i].BottomRect.X + whiteKeys[i].BottomRect.Right) / 2) - whiteKeys[i].BottomRect.X), whiteKeys[i].BottomRect.Y - 0);
                whiteKeys[i + 1].TopRect = new Rectangle(whiteKeys[i + 1].BottomRect.X + 4, 0, (((whiteKeys[i + 1].BottomRect.Right) - 4) - ((whiteKeys[i + 1].BottomRect.X) + 4)), whiteKeys[i + 1].BottomRect.Y - 0);
                whiteKeys[i + 2].TopRect = new Rectangle(whiteKeys[i + 2].BottomRect.X + 4, 0, (((whiteKeys[i + 2].BottomRect.Right) - 4) - ((whiteKeys[i + 2].BottomRect.X) + 4)), whiteKeys[i + 2].BottomRect.Y - 0);
                whiteKeys[i + 3].TopRect = new Rectangle(((whiteKeys[i + 3].BottomRect.X + whiteKeys[i + 3].BottomRect.Right) / 2), 0, (whiteKeys[i + 3].BottomRect.Right - ((whiteKeys[i + 3].BottomRect.X + whiteKeys[i + 3].BottomRect.Right) / 2)), whiteKeys[i + 3].BottomRect.Y - 0);
                whiteKeys[i + 4].TopRect = new Rectangle(whiteKeys[i + 4].BottomRect.X, 0, (((whiteKeys[i + 4].BottomRect.X + whiteKeys[i + 4].BottomRect.Right) / 2) - whiteKeys[i + 4].BottomRect.X) + 3, whiteKeys[i + 4].BottomRect.Y - 0);
                whiteKeys[i + 5].TopRect = new Rectangle(whiteKeys[i + 5].BottomRect.X + 4, 0, (((whiteKeys[i + 5].BottomRect.Right) - 4) - ((whiteKeys[i + 5].BottomRect.X) + 4)), whiteKeys[i + 5].BottomRect.Y - 0);
                whiteKeys[i + 6].TopRect = new Rectangle(((whiteKeys[i + 6].BottomRect.X + whiteKeys[i + 6].BottomRect.Right) / 2), 0, (whiteKeys[i + 6].BottomRect.Right - ((whiteKeys[i + 6].BottomRect.X + whiteKeys[i + 6].BottomRect.Right) / 2)), whiteKeys[i + 6].BottomRect.Y - 0);
                Drawing.FillRectangle(dat, whiteKeys[i].TopRect, Color.Black);
                Drawing.FillRectangle(dat, whiteKeys[i].BottomRect, Color.Black);
                Drawing.FillRectangle(dat, whiteKeys[i + 1].BottomRect, Color.Red);
                Drawing.FillRectangle(dat, whiteKeys[i + 1].TopRect, Color.Red);
                Drawing.FillRectangle(dat, whiteKeys[i + 2].BottomRect, Color.Green);
                Drawing.FillRectangle(dat, whiteKeys[i + 2].TopRect, Color.Green);
                Drawing.FillRectangle(dat, whiteKeys[i + 3].BottomRect, Color.Orange);
                Drawing.FillRectangle(dat, whiteKeys[i + 3].TopRect, Color.Orange);
                Drawing.FillRectangle(dat, whiteKeys[i + 4].BottomRect, Color.Black);
                Drawing.FillRectangle(dat, whiteKeys[i + 4].TopRect, Color.Black);
                Drawing.FillRectangle(dat, whiteKeys[i + 5].BottomRect, Color.Red);
                Drawing.FillRectangle(dat, whiteKeys[i + 5].TopRect, Color.Red);
                Drawing.FillRectangle(dat, whiteKeys[i + 6].BottomRect, Color.Orange);
                Drawing.FillRectangle(dat, whiteKeys[i + 6].TopRect, Color.Orange);
                frameColor.UnlockBits(dat);

                //pictureBox1.Image = frameColor;
            }
            //Resto Mancante perchè sono 46 tasti bianchi non divisibili per 7
          /*  if (Resto > 0)
            {
                whiteKeys[whiteKeys.Count - Resto].BottomRect = new Rectangle(Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto].LeftLine.Start.X), Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto].RightLine.Start.Y), Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto].RightLine.End.X - whiteKeys[whiteKeys.Count - Resto].LeftLine.Start.X), Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto].RightLine.End.Y - whiteKeys[whiteKeys.Count - Resto].RightLine.Start.Y));
                whiteKeys[whiteKeys.Count - Resto + 1].BottomRect = new Rectangle(Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto + 1].LeftLine.Start.X), Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto + 1].RightLine.Start.Y), Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto + 1].RightLine.End.X - whiteKeys[whiteKeys.Count - Resto + 1].LeftLine.Start.X), Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto + 1].RightLine.End.Y - whiteKeys[whiteKeys.Count - Resto + 1].RightLine.Start.Y));
                whiteKeys[whiteKeys.Count - Resto + 2].BottomRect = new Rectangle(Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto + 2].LeftLine.Start.X), Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto + 2].RightLine.Start.Y), Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto + 2].RightLine.End.X - whiteKeys[whiteKeys.Count - Resto + 2].LeftLine.Start.X), Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto + 2].RightLine.End.Y - whiteKeys[whiteKeys.Count - Resto + 2].RightLine.Start.Y));
                whiteKeys[whiteKeys.Count - Resto + 3].BottomRect = new Rectangle(Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto + 3].LeftLine.Start.X), Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto + 3].LeftLine.Start.Y), Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto + 3].RightLine.End.X - whiteKeys[whiteKeys.Count - Resto + 3].LeftLine.Start.X), Convert.ToInt32(whiteKeys[whiteKeys.Count - Resto + 3].RightLine.End.Y - whiteKeys[whiteKeys.Count - Resto + 3].LeftLine.Start.Y));
                whiteKeys[whiteKeys.Count - Resto].TopRect = new Rectangle(whiteKeys[whiteKeys.Count - Resto].BottomRect.X, 0, (((whiteKeys[whiteKeys.Count - Resto].BottomRect.X + whiteKeys[whiteKeys.Count - Resto].BottomRect.Right) / 2) - whiteKeys[whiteKeys.Count - Resto].BottomRect.X), whiteKeys[whiteKeys.Count - Resto].BottomRect.Y - 0);
                whiteKeys[whiteKeys.Count - Resto + 1].TopRect = new Rectangle(whiteKeys[whiteKeys.Count - Resto + 1].BottomRect.X + 4, 0, (((whiteKeys[whiteKeys.Count - Resto + 1].BottomRect.Right) - 4) - ((whiteKeys[whiteKeys.Count - Resto + 1].BottomRect.X) + 4)), whiteKeys[whiteKeys.Count - Resto + 1].BottomRect.Y - 0);
                whiteKeys[whiteKeys.Count - Resto + 2].TopRect = new Rectangle(whiteKeys[whiteKeys.Count - Resto + 2].BottomRect.X + 4, 0, (((whiteKeys[whiteKeys.Count - Resto + 2].BottomRect.Right) - 4) - ((whiteKeys[whiteKeys.Count - Resto + 2].BottomRect.X) + 4)), whiteKeys[whiteKeys.Count - Resto + 2].BottomRect.Y - 0);
                whiteKeys[whiteKeys.Count - Resto + 3].TopRect = new Rectangle(((whiteKeys[whiteKeys.Count - Resto + 3].BottomRect.X + whiteKeys[whiteKeys.Count - Resto + 3].BottomRect.Right) / 2), 0, (whiteKeys[whiteKeys.Count - Resto + 3].BottomRect.Right - ((whiteKeys[whiteKeys.Count - Resto + 3].BottomRect.X + whiteKeys[whiteKeys.Count - Resto + 3].BottomRect.Right) / 2)), whiteKeys[whiteKeys.Count - Resto + 3].BottomRect.Y - 0);
                BitmapData dat = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, frameColor.PixelFormat);
                Drawing.FillRectangle(dat, whiteKeys[whiteKeys.Count - Resto].BottomRect, Color.Black);
                Drawing.FillRectangle(dat, whiteKeys[whiteKeys.Count - Resto].TopRect, Color.Black);
                Drawing.FillRectangle(dat, whiteKeys[whiteKeys.Count - Resto + 1].BottomRect, Color.Red);
                Drawing.FillRectangle(dat, whiteKeys[whiteKeys.Count - Resto + 1].TopRect, Color.Red);
                Drawing.FillRectangle(dat, whiteKeys[whiteKeys.Count - Resto + 2].BottomRect, Color.Green);
                Drawing.FillRectangle(dat, whiteKeys[whiteKeys.Count - Resto + 2].TopRect, Color.Green);
                Drawing.FillRectangle(dat, whiteKeys[whiteKeys.Count - Resto + 3].BottomRect, Color.Orange);
                Drawing.FillRectangle(dat, whiteKeys[whiteKeys.Count - Resto + 3].TopRect, Color.Orange);
                frameColor.UnlockBits(dat);
                //  pictureBox1.Image = frameColor;
            }*/
            /*
            //new ImageForm(frameColor, "Detect Keys").Show();

            Keyboard.WhiteKeys = whiteKeys;
            Keyboard.BlackKeys = blackKeys;
            int centerOctave = Keyboard.Octaves / 2;

            Keyboard.WhiteKeys[centerOctave * 7].Note = WhiteNote.F;
            Keyboard.BlackKeys[centerOctave * 5].Note = BlackNote.Fs;
            int contNote = 1;
            for (int i = (centerOctave * 7) + 1; i < Keyboard.WhiteKeysCount; i++)
            {
                int BaseWhiteNote = (int)WhiteNote.F;
                int posWhiteNote = ((BaseWhiteNote + contNote) % 7);
                contNote++;
                WhiteNote newWhiteNotePos = (WhiteNote)posWhiteNote;
                Keyboard.WhiteKeys[i].Note = newWhiteNotePos;

            }
            contNote = 1;
            for (int i = (centerOctave * 7) - 1; i >= 0; i--)
            {
                int BaseWhiteNote = (int)WhiteNote.F;
                int negWhiteNote = ((BaseWhiteNote - contNote) % 7);
                if (negWhiteNote < 0) negWhiteNote += 7;
                contNote++;
                WhiteNote newWhiteNoteNeg = (WhiteNote)negWhiteNote;
                Keyboard.WhiteKeys[i].Note = newWhiteNoteNeg;
            }
            contNote = 1;
            for (int i = (centerOctave * 5) + 1; i < Keyboard.BlackKeysCount; i++)
            {
                int BaseBlackNote = (int)BlackNote.Fs;
                int posBlackNote = ((BaseBlackNote + contNote) % 5);
                contNote++;
                BlackNote newBlackNotePos = (BlackNote)posBlackNote;
                Keyboard.BlackKeys[i].Note = newBlackNotePos;
            }
            contNote = 1;
            for (int i = (centerOctave * 5) - 1; i >= 0; i--)
            {
                int BaseBlackNote = (int)BlackNote.Fs;
                int negBlackNote = ((BaseBlackNote - contNote) % 5);
                if (negBlackNote < 0) negBlackNote += 5;
                contNote++;
                BlackNote newBlackNoteNeg = (BlackNote)negBlackNote;
                Keyboard.BlackKeys[i].Note = newBlackNoteNeg;
            }

            for (int i = 0; i < Keyboard.WhiteKeysCount; i++)
            {
                Graphics.FromImage(frameColor).DrawString(Keyboard.WhiteKeys[i].Note.ToString(), new Font("Arial", 7), new SolidBrush(Color.White), new PointF(Keyboard.WhiteKeys[i].Centroid.X, Keyboard.WhiteKeys[i].Centroid.Y));
            }
            for (int i = 0; i < Keyboard.BlackKeysCount; i++)
            {
                Graphics.FromImage(frameColor).DrawString(Keyboard.BlackKeys[i].Note.ToString(), new Font("Arial", 7), new SolidBrush(Color.White), new PointF(Keyboard.BlackKeys[i].Centroid.X, Keyboard.BlackKeys[i].Centroid.Y));
            }
            new ImageForm(frameColor, "Detect Notes").Show();*/
            
        }

        private void apriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Video Files(*.avi; *.mpeg; *.mov) | *.avi; *.mpeg; *.mpg; *.mov";

                if (dlg.ShowDialog() == DialogResult.OK)
                {

                    // Create a new Bitmap object from the picture file on disk,
                    // and assign that to the PictureBox.Image property
                    fileSource.Source = dlg.FileName;
                    videoPlayer.VideoSource = fileSource;
                    
                   /* pictureBox1.Image = new Bitmap(fileSource.);
                    frameColor = new Bitmap(pictureBox1.Image);
                    this.ClientSize = pictureBox1.Image.Size;*/
                }
            }
        }
        #endregion


        private void detectPressedKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //videoPlayer.Start();
            currentImageIndex = 0;
            playbackTimer.Interval = 1000 / 12;
            playbackTimer.Tick += new EventHandler(playbackTimer_Tick);
            playbackTimer.Start();
        }

        private void playbackTimer_Tick(object sender, EventArgs e)
        {
            if (currentImageIndex + 1 >= imagesToDisplay.Length)
            {
                playbackTimer.Stop();

                return;
            }
            Bitmap frame = imagesToDisplay[currentImageIndex++];
            pictureBox1.Image = frame;
            Bitmap dest = quad.Apply(frame);
            keyboard = dest;
            dest = Grayscale.CommonAlgorithms.BT709.Apply(dest);
            ThresholdedDifference th = new ThresholdedDifference();
            th.OverlayImage = backgroundKeyboard;
            dest = th.Apply(dest);
            new Opening().ApplyInPlace(dest);
            ConnectedComponentsLabeling com = new ConnectedComponentsLabeling();
            com.Apply(dest);
            Blob[] blobs = com.BlobCounter.GetObjectsInformation();
            int maxArea1 = blobs[0].Area;
            int maxArea2 = blobs[1].Area;
            Blob maxBlob1 = blobs[0];
            Blob maxBlob2 = blobs[1];
            for (int i = 2; i < blobs.Length; i++)
            {
                if (blobs[i].Area > maxArea1)
                {
                    maxArea2 = maxArea1;
                    maxArea1 = blobs[i].Area;
                    maxBlob2 = maxBlob1;
                    maxBlob1 = blobs[i];
                }
                else if (maxArea2 < blobs[i].Area)
                {
                    maxArea2 = blobs[i].Area;
                    maxBlob2 = blobs[i];
                }
            }

            // CropImage(keyboard, maxBlob1.Rectangle).Save("C://Users//Carmine//Desktop//Color2//C" + cont.ToString() + ".bmp"); cont++;
            Bitmap hand1 = CropImage(keyboard, maxBlob1.Rectangle);
            Bitmap hand2 = CropImage(keyboard, maxBlob2.Rectangle);
            if (hand1.Width > 20 || hand2.Width > 20)
            {
                // Bitmap backHand1 = CropImage(backgroundKeyboard, maxBlob1.Rectangle);
                // Bitmap backHand2 = CropImage(backgroundKeyboard, maxBlob2.Rectangle);
                Bitmap Hand1Extract = DetectSkinColor((Bitmap)hand1.Clone());
                Bitmap Hand2Extract = DetectSkinColor((Bitmap)hand2.Clone());
                Hand1Extract = Grayscale.CommonAlgorithms.BT709.Apply(Hand1Extract);
                Hand2Extract = Grayscale.CommonAlgorithms.BT709.Apply(Hand2Extract);
                // new Threshold().ApplyInPlace(Hand1Extract);
                // new Threshold().ApplyInPlace(Hand2Extract);
                new Opening().ApplyInPlace(Hand1Extract);
                new Opening().ApplyInPlace(Hand2Extract);

                new GaussianBlur().ApplyInPlace(Hand1Extract);
                new GaussianBlur().ApplyInPlace(Hand2Extract);
                /* new CannyEdgeDetector().ApplyInPlace(Hand1Extract);
                   new CannyEdgeDetector().ApplyInPlace(Hand2Extract);*/
                // Hand1Extract = new ExtractBiggestBlob().Apply(Hand1Extract);
                //Hand2Extract = new ExtractBiggestBlob().Apply(Hand2Extract);
                //new Threshold().ApplyInPlace(Hand1Extract);
                //new Threshold().ApplyInPlace(Hand2Extract);
                //  MessageBox.Show(blobsCountingHand1.ObjectsCount.ToString());
                new CannyEdgeDetector().ApplyInPlace(Hand1Extract);
                new CannyEdgeDetector().ApplyInPlace(Hand2Extract);
                Hand1Extract = new GrayscaleToRGB().Apply(Hand1Extract);
                Hand2Extract = new GrayscaleToRGB().Apply(Hand2Extract);
                if (HandExtractLeft.ClientRectangle.Height > 30 && HandExtractRight.ClientRectangle.Height > 30)
                {
                    List<IntPoint> pointsEdgeHand1 = new List<IntPoint>();
                    List<IntPoint> pointsEdgeHand2 = new List<IntPoint>();
                    for (int i = 0; i < Hand1Extract.Width; i++)
                    {
                        for (int j = 0; j < Hand1Extract.Height; j++)
                        {

                            // if (Hand1Extract.GetPixel(i, j) == Color.Black) MessageBox.Show("Okok");
                            if (Hand1Extract.GetPixel(i, j).ToArgb() != Color.FromArgb(-16777216).ToArgb())
                            {
                                BitmapData datiHand1 = hand1.LockBits(new Rectangle(0, 0, hand1.Width, hand1.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                    hand1.PixelFormat);
                                pointsEdgeHand1.Add(new IntPoint(i, j));
                                Drawing.Rectangle(datiHand1, new Rectangle(pointsEdgeHand1[pointsEdgeHand1.Count - 1].X - 2, pointsEdgeHand1[pointsEdgeHand1.Count - 1].Y - 2, 4, 4), Color.Red);
                                hand1.UnlockBits(datiHand1);
                            }
                        }
                    }
                    for (int i = 0; i < Hand2Extract.Width; i++)
                    {
                        for (int j = 0; j < Hand2Extract.Height; j++)
                        {
                            if (Hand2Extract.GetPixel(i, j).ToArgb() != Color.FromArgb(-16777216).ToArgb())
                            {
                                BitmapData datiHand2 = hand2.LockBits(new Rectangle(0, 0, hand2.Width, hand2.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                    hand2.PixelFormat);
                                pointsEdgeHand2.Add(new IntPoint(i, j));
                                Drawing.Rectangle(datiHand2, new Rectangle(pointsEdgeHand2[pointsEdgeHand2.Count - 1].X - 2, pointsEdgeHand2[pointsEdgeHand2.Count - 1].Y - 2, 4, 4), Color.Red);
                                hand2.UnlockBits(datiHand2);
                            }
                        }
                    }
                    int N1 = pointsEdgeHand1.Count;
                    int N2 = pointsEdgeHand2.Count;
                    if (N1 > 0 && N2 > 0)
                    {
                        int sXHand1 = 0, sYHand1 = 0;
                        

                        for (int i = 0; i < N1; i++)
                        {
                            sXHand1 += pointsEdgeHand1[i].X;
                            sYHand1 += pointsEdgeHand1[i].Y;
                        }
                        int centrXHand1 = (sXHand1 / N1) - 20, centrYHand1 = (sYHand1 / N1) + 50;

                        int sXHand2 = 0, sYHand2 = 0;
                        
                        for (int i = 0; i < N2; i++)
                        {
                            sXHand2 += pointsEdgeHand2[i].X;
                            sYHand2 += pointsEdgeHand2[i].Y;
                        }
                        int centrXHand2 = (sXHand2 / N2) - 20, centrYHand2 = (sYHand2 / N2) + 50;
                        IntPoint CentrHand1 = new IntPoint(centrXHand1, centrYHand1 + 30);
                        IntPoint CentrHand2 = new IntPoint(centrXHand2, centrYHand2 + 30);
                        //  int maxHand1 = EuclideanDistance(CentrHand1, pointsEdgeHand1[0]);
                        // String Mess = "";
                         List<DistancesHand> DistancesHand1 = new List<DistancesHand>();
                         List<DistancesHand> DistancesHand2 = new List<DistancesHand>();
                         for (int i = 0; i < N1; i++)
                         {
                             Int64 val = Convert.ToInt64(Accord.Math.Distance.SquareEuclidean(Convert.ToDouble(centrXHand1), Convert.ToDouble(pointsEdgeHand1[i].X), Convert.ToDouble(centrYHand1), Convert.ToDouble(pointsEdgeHand1[i].Y)));//EuclideanDistance(CentrHand2, pointsEdgeHand2[i]);//EuclideanDistance(CentrHand1, pointsEdgeHand1[i]);
                             DistancesHand dis = new DistancesHand();
                             dis.distance = val;
                             dis.point = pointsEdgeHand1[i];
                             if (i > 0)
                             {
                                 if (Math.Abs(val - DistancesHand1[DistancesHand1.Count - 1].distance) > 350)
                                     DistancesHand1.Add(dis);
                             }
                             else
                                 DistancesHand1.Add(dis);
                           //  File.AppendAllText("C:/Users//Carmine//Desktop//prova.txt", "hand1: " + dis.distance + " " + dis.point.X + " " + dis.point.Y + "  -  ");
                             // File.AppendAllText("C:/Users//Carmine//Desktop//prova.txt", );

                             // Mess += dis.distance + "\n";
                             // if (val > maxHand1) maxHand1 = val;
                         }


                         // int maxHand2 = EuclideanDistance(CentrHand2, pointsEdgeHand2[0]);
                         for (int i = 0; i < N2; i++)
                         {
                             Int64 val = Convert.ToInt64(Accord.Math.Distance.SquareEuclidean(Convert.ToDouble(centrXHand2), Convert.ToDouble(pointsEdgeHand2[i].X), Convert.ToDouble(centrYHand2), Convert.ToDouble(pointsEdgeHand2[i].Y)));//EuclideanDistance(CentrHand2, pointsEdgeHand2[i]);
                             DistancesHand dis = new DistancesHand();
                             dis.distance = val;
                             dis.point = pointsEdgeHand2[i];
                             if (i > 0)
                             {
                                 if (Math.Abs(val - DistancesHand2[DistancesHand2.Count - 1].distance) > 350)
                                     DistancesHand2.Add(dis);
                             }
                             else
                                 DistancesHand2.Add(dis);
                           //  File.AppendAllText("C:/Users//Carmine//Desktop//prova.txt", "hand2: " + dis.distance + " " + dis.point.X + " " + dis.point.Y + "  -  ");
                             // File.AppendAllText("C:/Users//Carmine//Desktop//prova.txt", "\n");
                             // Mess += dis.distance + "\n";
                             // if (val > maxHand2) maxHand2 = val;
                         }
                       //  MessageBox.Show(DistancesHand1.Count + " " + DistancesHand2.Count);
                         if (DistancesHand1.Count > 4 && DistancesHand2.Count > 4)
                         {
                             DistancesHand1.OrderByDescending(dis => dis.distance);
                             DistancesHand2.OrderByDescending(dis => dis.distance);
                             BitmapData datiHand1 = hand1.LockBits(new Rectangle(0, 0, hand1.Width, hand1.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                            hand1.PixelFormat);
                             BitmapData datiHand2 = hand2.LockBits(new Rectangle(0, 0, hand2.Width, hand2.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                 hand2.PixelFormat);
                             Drawing.Rectangle(datiHand1, new Rectangle(Convert.ToInt32(centrXHand1 - 2), Convert.ToInt32(centrYHand1 - 2), 4, 4), Color.Blue);
                             Drawing.Rectangle(datiHand2, new Rectangle(Convert.ToInt32(centrXHand2 - 2), Convert.ToInt32(centrXHand2 - 2), 4, 4), Color.Blue);

                             Drawing.FillRectangle(datiHand1, new Rectangle(Convert.ToInt32(DistancesHand1[0].point.X - 2), Convert.ToInt32(DistancesHand1[0].point.Y - 2), 4, 4), Color.Blue);
                             Drawing.FillRectangle(datiHand1, new Rectangle(Convert.ToInt32(DistancesHand1[1].point.X - 2), Convert.ToInt32(DistancesHand1[1].point.Y - 2), 4, 4), Color.Blue);
                             Drawing.FillRectangle(datiHand1, new Rectangle(Convert.ToInt32(DistancesHand1[2].point.X - 2), Convert.ToInt32(DistancesHand1[2].point.Y - 2), 4, 4), Color.Blue);
                             Drawing.FillRectangle(datiHand1, new Rectangle(Convert.ToInt32(DistancesHand1[3].point.X - 2), Convert.ToInt32(DistancesHand1[3].point.Y - 2), 4, 4), Color.Blue);
                             Drawing.FillRectangle(datiHand1, new Rectangle(Convert.ToInt32(DistancesHand1[4].point.X - 2), Convert.ToInt32(DistancesHand1[4].point.Y - 2), 4, 4), Color.Blue);

                             Drawing.FillRectangle(datiHand2, new Rectangle(Convert.ToInt32(DistancesHand2[0].point.X - 2), Convert.ToInt32(DistancesHand2[0].point.Y - 2), 4, 4), Color.Blue);
                             Drawing.FillRectangle(datiHand2, new Rectangle(Convert.ToInt32(DistancesHand2[1].point.X - 2), Convert.ToInt32(DistancesHand2[1].point.Y - 2), 4, 4), Color.Blue);
                             Drawing.FillRectangle(datiHand2, new Rectangle(Convert.ToInt32(DistancesHand2[2].point.X - 2), Convert.ToInt32(DistancesHand2[2].point.Y - 2), 4, 4), Color.Blue);
                             Drawing.FillRectangle(datiHand2, new Rectangle(Convert.ToInt32(DistancesHand2[3].point.X - 2), Convert.ToInt32(DistancesHand2[3].point.Y - 2), 4, 4), Color.Blue);
                             Drawing.FillRectangle(datiHand2, new Rectangle(Convert.ToInt32(DistancesHand2[4].point.X - 2), Convert.ToInt32(DistancesHand2[4].point.Y - 2), 4, 4), Color.Blue);
                             hand2.UnlockBits(datiHand2);
                             hand1.UnlockBits(datiHand1);
                         }
                    }

                }
                if (maxBlob1.Rectangle.X > maxBlob2.Rectangle.X)
                {
                    HandExtractLeft.ChangeFrame(Hand2Extract);
                    HandLeftForm.ChangeFrame(hand2);
                    //BackHandLeft.ChangeFrame(backHand2);

                    HandRightForm.ChangeFrame(hand1);
                    // BackHandRight.ChangeFrame(backHand1);
                    HandExtractRight.ChangeFrame(Hand1Extract);
                }
                else
                {
                    HandLeftForm.ChangeFrame(hand1);
                    // BackHandLeft.ChangeFrame(backHand1);
                    HandExtractLeft.ChangeFrame(Hand1Extract);

                    HandRightForm.ChangeFrame(hand2);
                    //BackHandRight.ChangeFrame(backHand2);
                    HandExtractRight.ChangeFrame(Hand2Extract);

                }
            }
            
            dest = new GrayscaleToRGB().Apply(dest);
            BitmapData dati = dest.LockBits(new Rectangle(0, 0, dest.Width, dest.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, dest.PixelFormat);
           
            Drawing.Rectangle(dati, maxBlob1.Rectangle, Color.Red);
            Drawing.Rectangle(dati, maxBlob2.Rectangle, Color.Blue);
           
            dest.UnlockBits(dati);

            VideoForm.ChangeFrame(dest);
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
           
        }

       /* private Int64 EuclideanDistance(IntPoint a, IntPoint b)
        {
            return Convert.ToInt64(Math.Sqrt((b.X + a.X) * (b.X + a.X) - (b.Y + a.Y) * (b.Y + a.Y)));
        }*/

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

    }

   

   
}



/*

                       //Right Line
                       for (int i = 0; i < NumbersOfKeys; i++)
                       {
                           float x1 = 0, x2 = 0, y1 = 0, y2 = 0;

                           index = i * 5;
                           Blob b = blobs[index];
                           Rectangle r = b.Rectangle;
                           // Graphics.FromImage(frameColor).DrawString(index.ToString(), new Font("Arial", 13), new SolidBrush(Color.Blue), new PointF(b.CenterOfGravity.X, b.CenterOfGravity.Y));
                           // MessageBox.Show("Left " + index.ToString());
                           BitmapData dati = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, frameColor.PixelFormat);
                           //Rectangle r = rects[index];

                           x1 = (r.Right); y1 = r.Bottom; y2 = pictureBox1.Image.Height;
                           Drawing.Line(dati, new IntPoint(Convert.ToInt32((r.X + r.Right) / 2), r.Top), new IntPoint(Convert.ToInt32(b.CenterOfGravity.X), Convert.ToInt32(b.CenterOfGravity.Y)), Color.Red);
                           if ((b.CenterOfGravity.X + ((b.Rectangle.Width / 2) + 3)) != x1)
                           {
                               //  MessageBox.Show("calcolo pendenza");
                               double slope = Math.Abs(b.CenterOfGravity.Y - r.Top) / ((b.CenterOfGravity.X + ((b.Rectangle.Width / 2) + 3)) - x1 - 3);
                               x2 = Convert.ToSingle(Math.Floor((y2 - y1) / slope) + (x1));
                               //  x2 = x1;
                           }
                           else
                           {
                               // MessageBox.Show("X2 = X1");
                               x2 = x1;
                           }
                           Drawing.Line(dati, new IntPoint(Convert.ToInt32(x1 - 3), Convert.ToInt32(y1)), new IntPoint(Convert.ToInt32(x2), Convert.ToInt32(y2)), Color.Red);
                           frameColor.UnlockBits(dati);
                           pictureBox1.Image = frameColor;

                       }

                       //Middle Line
                       for (int i = 0; i < NumbersOfKeys; i++)
                       {
                           float x1=0, x2=0, y1=0, y2=0;
                
                           index = 1 + i * 5;
                           Blob b = blobs[index];
                           Rectangle r = b.Rectangle;
                           //Graphics.FromImage(frameColor).DrawString(index.ToString(), new Font("Arial", 13), new SolidBrush(Color.Blue), new PointF(b.CenterOfGravity.X, b.CenterOfGravity.Y));
                        //   MessageBox.Show("Middle " + index.ToString());
                           BitmapData dati = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, frameColor.PixelFormat);
                          // Rectangle r = rects[index];
                
                           x1 = (r.Right + r.X) / 2; y1 = r.Bottom; y2 = pictureBox1.Image.Height;
                           Drawing.Line(dati, new IntPoint(Convert.ToInt32(x1), r.Top), new IntPoint(Convert.ToInt32(b.CenterOfGravity.X), Convert.ToInt32(b.CenterOfGravity.Y)), Color.Red);
                           if (b.CenterOfGravity.X != x1)
                           {
                             //  MessageBox.Show("calcolo pendenza");
                               double slope = Math.Abs(b.CenterOfGravity.Y - r.Top) / (b.CenterOfGravity.X - x1);
                               x2 = Convert.ToSingle(Math.Floor((y2 - y1) / slope) + (x1));
                              // x2 = x1;
                           }
                           else
                           {
                              // MessageBox.Show("X2 = X1");
                               x2 = x1;
                           }
                           Drawing.Line(dati, new IntPoint(Convert.ToInt32(x1), Convert.ToInt32(y1)), new IntPoint(Convert.ToInt32(x2), Convert.ToInt32(y2)), Color.Red);

                           if (index + 1 < blobs.Length)
                           {

                           }
                           frameColor.UnlockBits(dati);
                           pictureBox1.Image = frameColor;
                
                       }
            
                       //Left Line
                       for (int i = 0; i < NumbersOfKeys; i++)
                       {
                           float x1 = 0, x2 = 0, y1 = 0, y2 = 0;
                           float X1 = 0, X2 = 0, Y1 = 0, Y2 = 0;
                           index = 2 + i * 5;
                           Blob b = blobs[index];
                           Rectangle r = b.Rectangle;
                          // Graphics.FromImage(frameColor).DrawString(index.ToString(), new Font("Arial", 13), new SolidBrush(Color.Blue), new PointF(b.CenterOfGravity.X, b.CenterOfGravity.Y));
                           //MessageBox.Show("Right " + index.ToString());
                           BitmapData dati = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, frameColor.PixelFormat);
                
                
                         //Calcolo retta piccola a sinistra del tasto nero
                
                           x1 = r.X; y1 = r.Bottom; y2 = pictureBox1.Image.Height;
                           Drawing.Line(dati, new IntPoint(Convert.ToInt32((r.X+r.Right) /2), r.Top), new IntPoint(Convert.ToInt32(b.CenterOfGravity.X), Convert.ToInt32(b.CenterOfGravity.Y)), Color.Red);
                           if ((b.CenterOfGravity.X - b.Rectangle.Width / 2) != x1)
                           {
                               //  MessageBox.Show("calcolo pendenza");
                               double slope = Math.Abs(b.CenterOfGravity.Y - r.Top) / ((b.CenterOfGravity.X - b.Rectangle.Width / 2) - x1+3);
                               x2 = Convert.ToSingle(Math.Floor((y2 - y1) / slope) + (x1));
                               // x2 = x1;
                           }
                           else
                           {
                               // MessageBox.Show("X2 = X1");
                               x2 = x1;
                           }
                           Drawing.Line(dati, new IntPoint(Convert.ToInt32(x1+3), Convert.ToInt32(y1)), new IntPoint(Convert.ToInt32(x2), Convert.ToInt32(y2)), Color.Red);
               
                     //Calcolo retta grande a destra del tasto nero Sib
                           if (index + 1 < blobs.Length)
                           {
                               Blob b1 = blobs[index], b2 = blobs[index + 1];
                               Y1 = 0; X1 = (b1.CenterOfGravity.X + b2.CenterOfGravity.X) / 2;
                               X2 = X1; Y2 = pictureBox1.Image.Height;
                               Drawing.Line(dati, new IntPoint(Convert.ToInt32(X1), Convert.ToInt32(Y1)), new IntPoint(Convert.ToInt32(X2), Convert.ToInt32(Y2)), Color.Red);
                           }
                           frameColor.UnlockBits(dati);
                           pictureBox1.Image = frameColor;

                       }
                    //   MessageBox.Show(blobsFull[0].Rectangle.Height + " " + blobsFull[0].Rectangle.Width);
                       for (int i = 0; i < NumbersOfKeys; i++)
                       {
                           float x1 = 0, x2 = 0, y1 = 0, y2 = 0;
                           float X1 = 0, X2 = 0, Y1 = 0, Y2 = 0;
                           index = 4 + i * 5;
                           if (index >= blobs.Length) break;
                           Blob b = blobs[index];
                           Rectangle r = b.Rectangle;
               
                           BitmapData dati = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, frameColor.PixelFormat);  
                
                           x1 = r.X; y1 = r.Bottom; y2 = pictureBox1.Image.Height;
                           Drawing.Line(dati, new IntPoint(Convert.ToInt32((r.X + r.Right)/2), r.Top), new IntPoint(Convert.ToInt32(b.CenterOfGravity.X), Convert.ToInt32(b.CenterOfGravity.Y)), Color.Red);
                           if ((b.CenterOfGravity.X - b.Rectangle.Width / 2) != x1)
                           {
                               //  MessageBox.Show("calcolo pendenza");
                               double slope = Math.Abs(b.CenterOfGravity.Y - r.Top) / ((b.CenterOfGravity.X - b.Rectangle.Width / 2) - x1+3);
                               x2 = Convert.ToSingle(Math.Floor((y2 - y1) / slope) + (x1));
                               // x2 = x1;
                           }
                           else
                           {
                               // MessageBox.Show("X2 = X1");
                               x2 = x1;
                           }
                           Drawing.Line(dati, new IntPoint(Convert.ToInt32(x1+3), Convert.ToInt32(y1)), new IntPoint(Convert.ToInt32(x2), Convert.ToInt32(y2)), Color.Red);

                           if (index + 1 < blobs.Length)
                           {
                               Blob b1 = blobs[index], b2 = blobs[index + 1];
                               Y1 = 0; X1 = (b1.CenterOfGravity.X + b2.CenterOfGravity.X) / 2;
                               X2 = X1; Y2 = pictureBox1.Image.Height;
                               Drawing.Line(dati, new IntPoint(Convert.ToInt32(X1), Convert.ToInt32(Y1)), new IntPoint(Convert.ToInt32(X2), Convert.ToInt32(Y2)), Color.Red);
                           }
                           frameColor.UnlockBits(dati);

                       }

                       for (int i = 0; i < NumbersOfKeys; i++)
                       {
                           float x1 = 0, x2 = 0, y1 = 0, y2 = 0;

                           index = 3 + i * 5;
                           if (index >= blobs.Length) break;
                           Blob b = blobs[index];
                           Rectangle r = b.Rectangle;
                          // Graphics.FromImage(frameColor).DrawString(index.ToString(), new Font("Arial", 13), new SolidBrush(Color.Blue), new PointF(b.CenterOfGravity.X, b.CenterOfGravity.Y));
                         //  MessageBox.Show("Right due " + index.ToString());
                           BitmapData dati = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, frameColor.PixelFormat);
                         //  Rectangle r = rects[index];
                
                           x1 = r.Right; y1 = r.Bottom; y2 = pictureBox1.Image.Height;
                           Drawing.Line(dati, new IntPoint(Convert.ToInt32((r.X +  r.Right)/2), r.Top), new IntPoint(Convert.ToInt32(b.CenterOfGravity.X), Convert.ToInt32(b.CenterOfGravity.Y)), Color.Red);
                           if ((b.CenterOfGravity.X + b.Rectangle.Width / 2) != x1)
                           {
                               //  MessageBox.Show("calcolo pendenza");
                               double slope = Math.Abs(b.CenterOfGravity.Y - r.Top) / ((b.CenterOfGravity.X + b.Rectangle.Width / 2) - x1-3 );
                               x2 = Convert.ToSingle(Math.Floor((y2 - y1) / slope) + (x1));
                              //  x2 = x1;
                           }
                           else
                           {
                               // MessageBox.Show("X2 = X1");
                               x2 = x1;
                           }
                           Drawing.Line(dati, new IntPoint(Convert.ToInt32(x1-3), Convert.ToInt32(y1)), new IntPoint(Convert.ToInt32(x2), Convert.ToInt32(y2)), Color.Red);
                           frameColor.UnlockBits(dati);

                       }*/


/*  Bitmap frameGray = AForge.Imaging.Filters.Grayscale.CommonAlgorithms.BT709.Apply(frameColor);
              CannyEdgeDetector cannyFilter = new CannyEdgeDetector(255, 0);
              cannyFilter.ApplyInPlace(frameGray);
              HoughLineTransformation hough = new HoughLineTransformation();
              hough.ProcessImage(frameGray);
            
             HoughLine[] lines = hough.GetLinesByRelativeIntensity(0.51);
             List<Line> Lines = new List<Line>();
            // List<IntersectionLines> LinesInt = new List<IntersectionLines>() ;
             //IntersectionLines LinesInt;
            
             foreach (HoughLine line in lines)
              {
                  // get line's radius and theta values4
                  int r = line.Radius;
                  double t = line.Theta;
                  if ((t != 90) && (t != 92) && t != 18 && t != 160) continue;
                 // if (t != 0 && t != 90) continue;
                  // check if line is in lower part of the image
                  if (r < 0)
                  {
                      t += 180;
                      r = -r;
                  }

                  // convert degrees to radians
                  t = (t / 180) * Math.PI;

                  // get image centers (all coordinate are measured relative
                  // to center)
                  int w2 = frameGray.Width / 2;
                  int h2 = frameGray.Height / 2;

                  double x0 = 0, x1 = 0, y0 = 0, y1 = 0;

                  if (line.Theta != 0)
                  {
                      // none-vertical line
                      x0 = -w2; // most left point
                      x1 = w2;  // most right point

                      // calculate corresponding y values
                      y0 = (-Math.Cos(t) * x0 + r) / Math.Sin(t);
                      y1 = (-Math.Cos(t) * x1 + r) / Math.Sin(t);
                  }
                  else
                  {
                      // vertical line
                      x0 = line.Radius;
                      x1 = line.Radius;

                      y0 = h2;
                      y1 = -h2;
                  }
               
                  BitmapData bmpDataColor =
                      frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                      frameColor.PixelFormat);

                  BitmapData bmpData =
                       frameGray.LockBits(new Rectangle(0, 0, frameGray.Width, frameGray.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                       frameGray.PixelFormat);
                  // draw line on the image
                  int X1 = (int)x0 + w2; int Y1 = h2 - (int)y0;
                  int X2 = (int)x1 + w2; int Y2 = h2 - (int)y1;
                  Drawing.Line(bmpDataColor, new IntPoint(X1, Y1), new IntPoint(X2, Y2), Color.Red);
                  Drawing.Line(bmpData, new IntPoint(X1, Y1), new IntPoint(X2, Y2), Color.Red);
                  frameColor.UnlockBits(bmpDataColor);
                  frameGray.UnlockBits(bmpData);
                  Lines.Add(new Line(new IntPoint(X1, Y1), new IntPoint(X2, Y2), line.Theta));
                  //LinesInt.Line = Lines[Lines.Count - 1];
              }
             Line[] linesArray = Lines.ToArray();
             List<IntPoint> corners = new List<IntPoint>();
             int count = 0;
             Dictionary<Line, int> intersetions = new Dictionary<Line, int>();
             int cont = 0;
             for (int i = 0; i < linesArray.Length - 1; i++)
             {
                 cont = 0;
                 for (int j = i + 1; j < linesArray.Length; j++)
                 {
                     IntPoint intersect = ComputeIntersect(linesArray[i], linesArray[j]);

                     if (intersect.X >= 0 && intersect.Y >= 0)
                     {
                         intersetions[linesArray[i]] = cont++;
                         BitmapData data = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                      frameColor.PixelFormat);
                         corners.Add(intersect);
                       
                              Drawing.Rectangle(data, new Rectangle(new System.Drawing.Point(intersect.X-5, intersect.Y-5),new Size(10,10)),Color.Blue);
                         frameColor.UnlockBits(data);
                     }
                 }
             }
             corners = corners.OrderBy(p => p.Y).ToList();
             for (int i = 0; i < corners.Count; i++ )
             {
                 var g = Graphics.FromImage(frameColor);
                 g.DrawString((count++).ToString(), new Font("Arial", 7), new SolidBrush(Color.Gold), corners[i].X, corners[i].Y);
             }
               
             String a="";
              foreach(Line line in intersetions.Keys)
              {
                  a += intersetions[line].ToString() + "\n";
              }
            // MessageBox.Show(a);
            BitmapData dati = frameColor.LockBits(new Rectangle(0, 0, frameColor.Width, frameColor.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                      frameColor.PixelFormat);

             // Drawing.Line(dati, corners[375], corners[366], Color.Gold);
              //Drawing.Line(dati, corners[366], corners[386], Color.Gold);
              //Drawing.Line(dati, corners[386], corners[402], Color.Gold);
              frameColor.UnlockBits(dati);
        
              Bitmap frameRect = frameColor;*/


/* IntPoint ComputeIntersect(Line line1, Line line2)
        {
            int x1 = line1.StartPoint.X;
            int y1 = line1.StartPoint.Y;
            int x2 = line1.EndPoint.X;
            int y2 = line1.EndPoint.Y;
            int x3 = line2.StartPoint.X;
            int y3 = line2.StartPoint.Y;
            int x4 = line2.EndPoint.X;
            int y4 = line2.EndPoint.Y;

            float d = ((float)(x1 - x2) * (y3 - y4)) - ((y1 - y2) * (x3 - x4));
            if (d != 0)
            {
                IntPoint pt;
                pt.X = Convert.ToInt32(((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / d);
                pt.Y = Convert.ToInt32(((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / d);
                return pt;
            }
            else
                return new IntPoint(-1, -1);
        }*/