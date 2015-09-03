using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Drawing;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge;
using AForge.Math.Geometry;
using System.Drawing.Imaging;

namespace ProvaTesi
{

    public enum WhiteNote
    {
        C=0,
        D,
        E,
        F,
        G,
        A,
        B
    }
    public enum BlackNote
    {
        Cs = 0,
        Eb,
        Fs,
        Gs,
        Bb
    }
    class KeyboardPiano
    {
        public List<WhiteKey> WhiteKeys { get; set; }
        public List<BlackKey> BlackKeys { get; set; }
        public int Octaves
        {
            get
            {
                return WhiteKeysCount / 7;
            }
        }
        public int BlackKeysCount
        {
            get
            {
                return BlackKeys.Count;
            }
        }
        public int WhiteKeysCount 
        {
            get
            {
                return WhiteKeys.Count;
            }
        }

        public KeyboardPiano()
        {
            WhiteKeys = new List<WhiteKey>();
            BlackKeys = new List<BlackKey>();
        }
        public KeyboardPiano(List<WhiteKey> whiteKeys, List<BlackKey> blackKeys)
        {
            this.WhiteKeys = whiteKeys;
            this.BlackKeys = blackKeys;
        }
    }


    public class Key
    {
        public Key()
        {

        }
        public Key(int Octave)
        {
            this.Octave = Octave;
        }
        protected int Octave { get; set;}
    }

    public class BlackKey : Key
    {
        public Blob Blob { get; set; }
        public BlackNote Note { get; set; }
        public IntPoint Centroid 
        {
            get
            {
                IntPoint Point = new IntPoint();
                Point.X = Convert.ToInt32(Blob.Rectangle.X);
                Point.Y = Convert.ToInt32(Blob.CenterOfGravity.Y);
                return Point;
            }
        }
        public BlackKey()
        {
        }
        public BlackKey(Blob blob)
        {
            this.Blob = blob;
        }
        public BlackKey(Blob blob, BlackNote note)
        {
            this.Blob = blob;
            this.Note = note;
        }
    }

    public class WhiteKey : Key
    {
        public LineSegment LeftLine { get; set; }
        public LineSegment RightLine { get; set; }
        public Rectangle TopRect { get; set; }
        public Rectangle BottomRect { get; set; }
        public WhiteNote Note { get; set; }
        
        public IntPoint Centroid
        {
            get
            {
                IntPoint Point = new IntPoint();
                Point.X = (BottomRect.X);
                Point.Y = (BottomRect.Y+ (BottomRect.Y+BottomRect.Height)) / 2;
                return Point;
            }
        }



        public WhiteKey()
        {

        }
        public WhiteKey(LineSegment leftline, LineSegment rightline)
        {
            this.LeftLine = leftline;
            this.RightLine = rightline;
        }

        public WhiteKey(LineSegment leftline, LineSegment rightline, Rectangle rectTop, Rectangle rectBottom)
        {
            this.LeftLine = leftline;
            this.RightLine = rightline;
            this.TopRect = rectTop;
            this.BottomRect = rectBottom;
        }

        public WhiteKey(LineSegment leftline, LineSegment rightline, Rectangle rectTop, Rectangle rectBottom, WhiteNote note)
        {
            this.LeftLine = leftline;
            this.RightLine = rightline;
            this.TopRect = rectTop;
            this.BottomRect = rectBottom;
            this.Note = note;
        }

    }
}
