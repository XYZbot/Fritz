using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NAudio.Wave;
using System.IO;
using System.Threading;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Fritz
{
    public partial class Recorder : ScrollableControl
    {
        private Brush editBrush = new SolidBrush(Color.Purple);
        private Pen editPen = new Pen(Color.Purple, 2);
        private Brush blackBrush = new SolidBrush(Color.Black);

        private Brush editSelectedBrush = new SolidBrush(Color.Green);
        private Pen editSelectedPen = new Pen(Color.Green, 2);

        private Pen linePen = new Pen(Color.Black, 1);
        private Pen doubleLinePen = new Pen(Color.Black, 2);
        private Pen iLinePen = new Pen(Color.White, 1);
        private Pen oLinePen;
        private Pen backgroundPen;
        private Pen tickPen = new Pen(Color.LightSteelBlue, 1);
        private Font timingFont = new Font("Tahoma", 10);

        int scrollHeight = System.Windows.Forms.SystemInformation.HorizontalScrollBarHeight+4;

        private WaveStream waveStream;
        private MemoryStream memoryStream;
        private IWavePlayer waveOutDevice;
        private int samplesPerPixel = 128;
        private long startPosition=0;
        private int bytesPerSample=1;
        private int totalSamples=0;
        private Point startSelection, endSelection;
        private long startSelectionPosition = -1;
        private long endSelectionPosition = -1;
        private bool mouseDrag = false;

        //private Point _startScrollPosition;
        //private Point _startMousePosition;
        private bool IsPanning;
        public bool InvertMouse;

        private int ZoomValue;
        private int ZoomIncrement;

        private const int WM_HSCROLL = 0x114;

        public event ScrollEventHandler HorzScrollValueChanged;

        private int currentSelectionStart = -1;
        private int currentSelectionEnd = -1;

        public event EventHandler PlaybackStoppedCallback;
        public event EventHandler PlaybackTick;
        public event EventHandler ClipboardCallback;

        delegate void SetPositionCallback(long prevPosition, long nextPosition);

        private Thread showPlayback;

        byte[] copyData = null;

        int nextPlaybackPosition;
        int prevPlaybackPosition=-1;

        List<RobotState> editPoints = new List<RobotState>();

        private bool isPlaying = false;

        float[] minValues = new float[0];
        float[] maxValues = new float[0];

        int editPointSelected = -1;
        long editPointPosition = 0;
        int editPointHover = -1;
        int oldLocation = -1;

        const int editPointWidth = 10;

        public event EventHandler PositionChangeCallback;

        BufferedGraphicsContext currentContext;
        BufferedGraphics myBuffer;

        Boolean showMouth=true, showEyes=true, showNeck=true;

        private int skipAudioAmount=0;
        private int skipAudioX = -1;

        public Recorder()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            this.DoubleBuffered = true;

            //waveStream = (WaveStream)new NAudio.Wave.RawSourceWaveStream(new MemoryStream(), new WaveFormat(16000, 16, 2));

            // initialize waveStream to 5 seconds
            Reset();

            oLinePen = new Pen(Color.FromArgb(255, 255 - this.BackColor.R, 255 - this.BackColor.G, 255 - this.BackColor.B), 1);
            backgroundPen = new Pen(Color.FromArgb(255, BackColor.R, BackColor.G, BackColor.B));
            //backgroundPen = new Pen(Color.White);

            InvertMouse = false;
            IsPanning = false;

            ZoomValue = 100;
            ZoomIncrement = 20;

            VScroll = false;

            HorzScrollValueChanged = new ScrollEventHandler(Recorder_HorzScrollValueChanged);

            SetAutoScrollMargin(10, 0);
            
            currentContext = BufferedGraphicsManager.Current;
        }

        public void Reset()
        {
            int defaultSize = ((16000 * 16 * 2) / 8) * 5;
            memoryStream = new MemoryStream(defaultSize);
            for (int i = 0; i < defaultSize; i++)
                memoryStream.WriteByte(0);

            waveStream = (WaveStream)new NAudio.Wave.RawSourceWaveStream(memoryStream, new WaveFormat(16000, 16, 2));
            bytesPerSample = (waveStream.WaveFormat.BitsPerSample / 8) * waveStream.WaveFormat.Channels;
            totalSamples = (int)(waveStream.Length / bytesPerSample);

            editPoints.Clear();
            editPointSelected = -1;
            editPointHover = -1;
            oldLocation = -1;
            currentSelectionStart = -1;
            currentSelectionEnd = -1;
            startSelectionPosition = -1;
            endSelectionPosition = -1;
            Invalidate();
        }

        public void LoadAudio(String filename)
        {
            try
            {
                waveStream = new NAudio.Wave.WaveFileReader(filename);
            }
            catch (Exception)
            {
                MessageBox.Show("Could not load Audio! WAV files only.");
                return;
            }
            if (waveStream != null)
            {
                bytesPerSample = (waveStream.WaveFormat.BitsPerSample / 8) * waveStream.WaveFormat.Channels;
                if (bytesPerSample == 0)
                    bytesPerSample = 1;

                totalSamples = (int)(waveStream.Length / bytesPerSample);

                // create a memory based copy so we can access internal data without moving position
                MemoryStream ms = new MemoryStream((int)waveStream.Length);
                ms.Position = 0;

                byte[] waveData = new byte[samplesPerPixel * bytesPerSample];
                while (waveStream.Position < waveStream.Length)
                {
                    int bytesRead = waveStream.Read(waveData, 0, samplesPerPixel * bytesPerSample);
                    ms.Write(waveData, 0, bytesRead);
                }

                memoryStream = ms;
                waveStream = new RawSourceWaveStream(memoryStream, waveStream.WaveFormat);

                FitToScreen();
            }
            else
                Reset();

            //Zoom(0, 100000);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000200;// WS_EX_CLIENTEDGE;
                //cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
                return cp;
            }
        }

        public void waveOut_PlaybackHasStopped(object sender, EventArgs e)
        {
            if (PlaybackStoppedCallback != null)
            {
                PlaybackStoppedCallback(this, EventArgs.Empty);
            }
        }

/*
        public void ClearPlaybackPosition()
        {
            ControlPaint.DrawReversibleLine(PointToScreen(new Point((int)prevPlaybackPosition, 0)), PointToScreen(new Point((int)prevPlaybackPosition, Height - scrollHeight)), Color.Red);
        }
*/
        public void updatePlaybackPosition(long prevPosition, long nextPosition)
        {
            if ((prevPosition >= 0) && (prevPosition < Width) && (prevPosition != nextPosition))
            {
                // use actual values to overwrite previous highlight in case we had a repaint inbetween
                //ControlPaint.GetGraphics().DrawLine(linePen, prevPosition, minValues[prevPosition], prevPosition, maxValues[prevPosition]);

                ControlPaint.DrawReversibleLine(PointToScreen(new Point((int)prevPosition, 0)), PointToScreen(new Point((int)prevPosition, Height - scrollHeight)), Color.Red);
            }

            if ((nextPosition >= 0) /*&& (prevPosition >= 0)*/ && (nextPosition < Width) && (prevPosition != nextPosition))
                ControlPaint.DrawReversibleLine(PointToScreen(new Point((int)nextPosition, 0)), PointToScreen(new Point((int)nextPosition, Height - scrollHeight)), Color.Red);
        }

        private void ShowCurrentPlayback()
        {
            prevPlaybackPosition=-1;

            while (isPlaying)
            {
                nextPlaybackPosition = (int)(waveStream.Position - startPosition) / (samplesPerPixel * bytesPerSample);
                this.Invoke(new SetPositionCallback(updatePlaybackPosition), new object[] { prevPlaybackPosition, nextPlaybackPosition });
                prevPlaybackPosition = nextPlaybackPosition;
                Thread.Sleep(20);
                if ((endSelectionPosition > 0) && (endSelectionPosition - startSelectionPosition)>(getOneSecondRange()/2))
                {
                    if (waveStream.Position > endSelectionPosition)
                    {
                        Stop();
                    }
                }
                if (PlaybackTick != null)
                {
                    PlaybackTick(this, EventArgs.Empty);
                }
            }
        }

        public void Play()
        {
            waveOutDevice = new WaveOutEvent();

            isPlaying = true;
            // start playback placement thread
            showPlayback = new Thread(new ThreadStart(ShowCurrentPlayback));
            showPlayback.IsBackground = true;
            showPlayback.Start();

            // setup wave playback options
            if (startSelectionPosition>0)
                waveStream.Position = startSelectionPosition;
            else
                waveStream.Position = 0;

            waveOutDevice.PlaybackStopped += new EventHandler(waveOut_PlaybackHasStopped);
            try
            {
                waveOutDevice.Init(waveStream);
                waveOutDevice.Play();
            }
            catch (Exception)
            {
                isPlaying = false;
                MessageBox.Show("Unable to play Audio!");
            }
       }

        public void Pause()
        {
            if (waveOutDevice.PlaybackState == PlaybackState.Paused)
                waveOutDevice.Play();
            else
                waveOutDevice.Pause();
        }

        public void Stop()
        {
            isPlaying = false;

            waveOutDevice.Stop();
            waveOutDevice.Dispose();

            Invalidate();
            //ClearPlaybackPosition();
        }

        private static ScrollEventType[] _events =
            new ScrollEventType[] {
									  ScrollEventType.SmallDecrement,
									  ScrollEventType.SmallIncrement,
									  ScrollEventType.LargeDecrement,
									  ScrollEventType.LargeIncrement,
									  ScrollEventType.ThumbPosition,
									  ScrollEventType.ThumbTrack,
									  ScrollEventType.First,
									  ScrollEventType.Last,
									  ScrollEventType.EndScroll
								  };

        private ScrollEventType GetEventType(uint wParam)
        {
            if (wParam < _events.Length)
                return _events[wParam];
            else
                return ScrollEventType.EndScroll;
        }

        protected override void WndProc(ref Message m)
        {
            // Let the control process the message
            base.WndProc(ref m);

            // Was this a horizontal scroll message?
            if (m.Msg == WM_HSCROLL)
            {
                if (HorzScrollValueChanged != null)
                {
                    uint wParam = (uint)m.WParam.ToInt32();
                    HorzScrollValueChanged(this,
                        new ScrollEventArgs(
                            GetEventType(wParam & 0xffff), (int)(wParam >> 16)));
                }
            }
        }

        public void FitToScreen()
        {
            startPosition = 0;
            samplesPerPixel = Math.Max(1, totalSamples / this.Width);
            AutoScrollMinSize = new Size(this.Width + this.Padding.Horizontal, this.Height + this.Padding.Vertical - 30);
            Invalidate();
        }

        public void ZoomOut()
        {
            int hold = samplesPerPixel;
            samplesPerPixel = (int)((float)samplesPerPixel * 1.3333333f);
            if (samplesPerPixel == hold) samplesPerPixel++;

            int minSize = totalSamples / samplesPerPixel;
            int maxSamplesPerPixel = Math.Max(1, totalSamples / this.Width);
            if (minSize < Width)
            {
                samplesPerPixel = maxSamplesPerPixel;
                minSize = Width;
            }
            AutoScrollMinSize = new Size(minSize + this.Padding.Horizontal, this.Height + this.Padding.Vertical - 30);
            Invalidate();
        }

        public void ZoomIn()
        {
            samplesPerPixel = (int)((float)samplesPerPixel * 0.75f);
            if (samplesPerPixel <= 0) samplesPerPixel = 1;
            AutoScrollMinSize = new Size(Math.Max(1, totalSamples / samplesPerPixel) + this.Padding.Horizontal, this.Height + this.Padding.Vertical - 30);
            Invalidate();
        }

        public void InsertAudioSilence(long startSelectionPosition, long endSelectionPosition)
        {
            if ((startSelectionPosition < 0) || (endSelectionPosition < 0)) return;

            MemoryStream ms = new MemoryStream((int)waveStream.Length);

            byte[] waveData = new byte[samplesPerPixel * bytesPerSample];

            long holdPosition = waveStream.Position;

            waveStream.Position = 0;
            ms.Position = 0;

            bool insertedData = false;
            int slen = samplesPerPixel * bytesPerSample;
            long ilen = endSelectionPosition - startSelectionPosition;

            while (waveStream.Position < waveStream.Length)
            {
                if (!insertedData)
                {
                    if (waveStream.Position >= startSelectionPosition)
                    {
                        int i;
                        for (i=0;i<slen;i++)
                            waveData[i]=0;

                        for (i=0;i<ilen;i+=slen)
                            ms.Write(waveData, 0, slen);

                        insertedData = true;
                    }
                }
                int bytesRead = waveStream.Read(waveData, 0, slen);
                ms.Write(waveData, 0, bytesRead);
            }

            if (!insertedData)
            {
                if (waveStream.Position >= startSelectionPosition)
                {
                    int i;
                    for (i = 0; i < slen; i++)
                        waveData[i] = 0;

                    for (i = 0; i < ilen; i += slen)
                        ms.Write(waveData, 0, slen);

                    insertedData = true;
                }
            }

            waveStream = new RawSourceWaveStream(ms, waveStream.WaveFormat);
            waveStream.Position = holdPosition;
            memoryStream = ms;

            endSelectionPosition = startSelectionPosition;
            currentSelectionEnd = currentSelectionStart;

            totalSamples = (int)(waveStream.Length / bytesPerSample);

            AutoScrollMinSize = new Size(Math.Max(1, totalSamples / samplesPerPixel) + this.Padding.Horizontal, this.Height + this.Padding.Vertical - 30);

            Invalidate();
        }

        public void InsertAudio(MemoryStream insertData)
        {
            WaveFormat waveFormat;
            MemoryStream ms;
            long holdPosition;

/*
            if (waveStream == null)
            {
                waveFormat = new WaveFormat(16000, 16, 2);
                bytesPerSample = (waveFormat.BitsPerSample / 8) * waveFormat.Channels;

                ms = new MemoryStream();
                if (insertData != null)
                    ms.Write(insertData.ToArray(), 0, (int)insertData.Length);
                
                holdPosition = 0;
            }
            else
 */ 
            {
                ms = new MemoryStream((int)waveStream.Length + (int)insertData.Length);

                byte[] waveData = new byte[samplesPerPixel * bytesPerSample];

                holdPosition = waveStream.Position;

                waveStream.Position = 0;
                ms.Position = 0;

                bool insertedData = false;
                int slen = samplesPerPixel * bytesPerSample;
                while (waveStream.Position < waveStream.Length)
                {
                    if (!insertedData)
                    {
                        if (waveStream.Position >= startSelectionPosition)
                        {
                            ms.Write(insertData.ToArray(), 0, (int)insertData.Length);
                            insertedData = true;
                        }
                    }
                    int bytesRead = waveStream.Read(waveData, 0, slen);
                    ms.Write(waveData, 0, bytesRead);
                }

                waveFormat = waveStream.WaveFormat;
            }

            waveStream = new RawSourceWaveStream(ms, waveFormat);
            waveStream.Position = holdPosition;
            memoryStream = ms;

            endSelectionPosition = startSelectionPosition;
            currentSelectionEnd = currentSelectionStart;

            totalSamples = (int)(waveStream.Length / bytesPerSample);

            AutoScrollMinSize = new Size(Math.Max(1, totalSamples / samplesPerPixel) + this.Padding.Horizontal, this.Height + this.Padding.Vertical - 30);

            Invalidate();
        }

        public void InsertFrames(List<RobotState> states, long timespan)
        {
            // find insertion point
            int i, j;
            if (startSelectionPosition >= 0)
            {
                for (i = 0; (i < editPoints.Count) && (editPoints[i].position < startSelectionPosition); i++) ;

                for (j = 0; j < states.Count; j++, i++)
                {
                    states[j].position += startSelectionPosition;
                    editPoints.Insert(i, states[j]);
                }

                for (; i < editPoints.Count; i++)
                {
                    editPoints[i].position += timespan;
                }
            }
            else
            {
                long offset;

                if (editPoints.Count > 0)
                    offset = editPoints[editPoints.Count - 1].position;
                else
                    offset = 0;

                for (j = 0; j < states.Count; j++)
                {
                    states[j].position += offset;
                    editPoints.Add(states[j]);
                }
            }
        }

        public void Copy()
        {
            if ((startSelectionPosition < 0) || (endSelectionPosition < 0)) return;

            long copyLen = endSelectionPosition - startSelectionPosition;
            copyData = new byte[copyLen];

            waveStream.Position = startSelectionPosition;

            waveStream.Read(copyData, 0, (int)copyLen);
        }

        public void Paste()
        {
            if ((startSelectionPosition < 0) || (endSelectionPosition < 0)) return;

            MemoryStream ms = new MemoryStream((int)waveStream.Length);

            byte[] waveData = new byte[samplesPerPixel * bytesPerSample];

            long holdPosition = waveStream.Position;

            waveStream.Position = 0;
            ms.Position = 0;

            bool insertedData = false;

            while (waveStream.Position < waveStream.Length)
            {
                int bytesRead = waveStream.Read(waveData, 0, samplesPerPixel * bytesPerSample);
                if ((waveStream.Position < startSelectionPosition) || (waveStream.Position > endSelectionPosition))
                    ms.Write(waveData, 0, bytesRead);
                else
                {
                    if (!insertedData)
                    {
                        ms.Write(copyData, 0, copyData.Length);
                        insertedData = true;
                    }
                }
            }

            waveStream = new RawSourceWaveStream(ms, waveStream.WaveFormat);
            waveStream.Position = holdPosition;
            memoryStream = ms;

            endSelectionPosition = startSelectionPosition;
            currentSelectionEnd = currentSelectionStart;

            totalSamples = (int)(waveStream.Length / bytesPerSample);

            AutoScrollMinSize = new Size(Math.Max(1, totalSamples / samplesPerPixel) + this.Padding.Horizontal, this.Height + this.Padding.Vertical - 30);

            Invalidate();
        }

        public void Delete()
        {
            if ((startSelectionPosition < 0) || (endSelectionPosition < 0)) return;

            waveStream.Position = startSelectionPosition;

            // cut data
            MemoryStream ms = new MemoryStream((int)waveStream.Length);

            byte[] waveData = new byte[samplesPerPixel * bytesPerSample];

            long holdPosition = waveStream.Position;

            waveStream.Position = 0;
            ms.Position = 0;

            while (waveStream.Position < waveStream.Length)
            {
                int bytesRead = waveStream.Read(waveData, 0, samplesPerPixel * bytesPerSample);
                if ((waveStream.Position < startSelectionPosition) || ((waveStream.Position + 1) > endSelectionPosition))
                    ms.Write(waveData, 0, bytesRead);
            }

            waveStream = new RawSourceWaveStream(ms, waveStream.WaveFormat);
            waveStream.Position = holdPosition;
            memoryStream = ms;

            endSelectionPosition = startSelectionPosition;
            currentSelectionEnd = currentSelectionStart;

            totalSamples = (int)(waveStream.Length / bytesPerSample);

            AutoScrollMinSize = new Size(Math.Max(1, totalSamples / samplesPerPixel) + this.Padding.Horizontal, this.Height + this.Padding.Vertical - 30);

            editPointHover = -1;

            Invalidate();
        }


        public void Cut()
        {
            if ((startSelectionPosition < 0) || (endSelectionPosition < 0)) return;

            // copy data
            long copyLen = endSelectionPosition - startSelectionPosition;
            copyData = new byte[copyLen];

            waveStream.Position = startSelectionPosition;

            waveStream.Read(copyData, (int)0, (int)copyLen);

            Delete();
        }

        public void Zoom(int leftSample, int rightSample)
        {
            startPosition = leftSample * bytesPerSample;
            if (startPosition < 0) startPosition = 0;
            samplesPerPixel = Math.Max(1, (rightSample - leftSample) / this.Width);
            AutoScrollMinSize = new Size((totalSamples / samplesPerPixel) + this.Padding.Horizontal, this.Height + this.Padding.Vertical - 30);
            AutoScrollPosition = new Point((int)leftSample / samplesPerPixel, this.Height - 30);
            Invalidate();
        }

        private void UpdateSelection(int a, int b)
        {
            if (a > b)
            {
                int t = b;
                b = a;
                a = t;
            }
            if (a > Width) a = Width;
            if (b > Width) b = Width;
            if (a <= 0) a = 0;
            if (b <= 0) b = 0;

            if ((currentSelectionStart == -1) && (currentSelectionEnd == -1))
            {
                ControlPaint.FillReversibleRectangle(RectangleToScreen(new Rectangle(a, 0, b - a + 1, Height - scrollHeight)), Color.Black);
                currentSelectionStart = a;
                currentSelectionEnd = b+1;
            }
            else
            {
                if (a < currentSelectionStart)
                {
                    if (currentSelectionStart >= 0)
                        ControlPaint.FillReversibleRectangle(RectangleToScreen(new Rectangle(a, 0, currentSelectionStart - a, Height - scrollHeight)), Color.Black);

                    currentSelectionStart = a;
                }

                if (b > currentSelectionEnd)
                {
                    if (currentSelectionEnd >= 0)
                        ControlPaint.FillReversibleRectangle(RectangleToScreen(new Rectangle(currentSelectionEnd, 0, b - currentSelectionEnd+1, Height - scrollHeight)), Color.Black);

                    currentSelectionEnd = b+1;
                }
                //ControlPaint.FillReversibleRectangle(RectangleToScreen(new Rectangle(a, 0, b - a, Height)), Color.Black);
            }
        }

/*
        private void DrawSelection(Graphics g, int a, int b)
        {
            if (a > b)
            {
                int t = b;
                b = a;
                a = t;
            }
            if (a > Width) a = Width;
            if (b > Width) b = Width;
            if (a <= 0) a = 0;
            if (b <= 0) b = 0;

            ControlPaint.FillReversibleRectangle(RectangleToScreen(new Rectangle(a, 0, b - a + 1, Height - scrollHeight)), Color.Black);
            //g.FillRectangle(blackBrush, new Rectangle(a, 0, b - a + 1, Height - scrollHeight));
        }
*/

        public void SetEditPoints(ref List<RobotState> ss)
        {
            int i;
            editPoints = ss;
            if (ss.Count > 0)
            {
                int oneSecond = (int)getOneSecondRange();

                if (editPointSelected < 0)
                {
                    for (i = 0; i < ss.Count; i++)
                    {
                        if ((editPoints[i].position < (startSelectionPosition + editPointWidth)) && (editPoints[i].position > (startSelectionPosition - editPointWidth)))
                        {
                            editPointSelected = i;
                            editPointPosition = editPoints[i].position;
                        }
                    }
                }

                // ensure that the audio is long enough to time these state changes
                if (waveStream.Length < (editPoints[editPoints.Count - 1].position+oneSecond))
                {
                    // otherwise expand the length
                    int size = (int)editPoints[editPoints.Count - 1].position + oneSecond;
                    MemoryStream newMemoryStream = new MemoryStream(size);
                    newMemoryStream.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
                    for (long f = memoryStream.Length; f < size; f++)
                        newMemoryStream.WriteByte(0);

                    waveStream = (WaveStream)new NAudio.Wave.RawSourceWaveStream(newMemoryStream, waveStream.WaveFormat);
                    memoryStream = newMemoryStream;

                    totalSamples = (int)(waveStream.Length / bytesPerSample);
                    AutoScrollMinSize = new Size(Math.Max(1, totalSamples / samplesPerPixel) + this.Padding.Horizontal, this.Height + this.Padding.Vertical - 30);
                }
            }

            this.Invalidate();
        }

        private bool IsStateVisible(RobotState state)
        {
            bool R = false;
            bool G = false;
            bool B = false;

            // determine edit point color based on feature changes
            if ((state.leftHorizontalEye != -1)||(state.leftVerticalEye != -1)) R = true;
            if ((state.rightHorizontalEye != -1)||(state.rightVerticalEye != -1)) R = true;
            if ((state.leftEyebrow != -1)||(state.rightEyebrow != -1)) R = true;
            if ((state.rightEyelid != -1)||(state.leftEyelid != -1)) R = true;

            if (state.leftLip != -1) G = true;
            if (state.rightLip != -1) G = true;
            if (state.jaw != -1) G = true;

            if (state.neckTilt != -1) B = true;
            if (state.neckTwist != -1) B = true;

            if ((!showEyes) && (R) && (!G) && (!B)) return false;
            if ((!showMouth) && (G) && (!R) && (!B)) return false;
            if ((!showNeck) && (B) && (!R) && (!G)) return false;

            if ((!showMouth) && (!showEyes) && (R) && (G) && (!B)) return false;
            if ((!showNeck) && (!showEyes) && (R) && (B) && (!G)) return false;
            if ((!showMouth) && (!showNeck) && (B) && (G) && (!R)) return false;

            return true;
        }

        private void DrawEditPoint(RobotState state, Graphics g, int height)
        {
            int rweight = 255 / 4;
            int gweight = 255 / 3;
            int bweight = 255 / 2;
            int R = 255;
            int G = 255;
            int B = 255;

            // determine edit point color based on feature changes
            if ((state.leftHorizontalEye != -1)||(state.leftVerticalEye != -1)) R-=rweight;
            if ((state.rightHorizontalEye != -1)||(state.rightVerticalEye != -1)) R-=rweight;
            if ((state.leftEyebrow != -1)||(state.rightEyebrow != -1)) R-=rweight;
            if ((state.rightEyelid != -1)||(state.leftEyelid != -1)) R-=rweight;

            if (state.leftLip != -1) G-=gweight;
            if (state.rightLip != -1) G-=gweight;
            if (state.jaw != -1) G-=gweight;

            if (state.neckTilt != -1) B-=bweight;
            if (state.neckTwist != -1) B-=bweight;

            if ((!showEyes) && (R != 255) && (G == 255) && (B == 255)) return;
            if ((!showMouth) && (G != 255) && (R == 255) && (B == 255)) return;
            if ((!showNeck) && (B != 255) && (R == 255) && (G == 255)) return;

            if ((!showMouth) && (!showEyes) && (R != 255) && (G != 255) && (B == 255)) return;
            if ((!showNeck) && (!showEyes) && (R != 255) && (B != 255) && (G == 255)) return;
            if ((!showMouth) && (!showNeck) && (B != 255) && (G != 255) && (R == 255)) return;

            if ((!showEyes) && (!showNeck) && (!showMouth)) return;

            Brush editBrush = new SolidBrush(Color.FromArgb(R, G, B));
            Pen editPen = new Pen(Color.FromArgb(R, G, B), 2);

            int pos = (int)(state.position - startPosition) / (bytesPerSample * samplesPerPixel);
            Point[] pts = new Point[3];
            pts[0].X = pos - 5;
            pts[0].Y = 0;
            pts[1].X = pos + 5;
            pts[1].Y = 0;
            pts[2].X = pos;
            pts[2].Y = 5;
            g.FillPolygon(editBrush, pts);
            pts[0].X = pos - 6;
            pts[0].Y = (int)height - 1;
            pts[1].X = pos + 6;
            pts[1].Y = (int)height - 1;
            pts[2].X = pos;
            pts[2].Y = (int)height - 7;
            g.FillPolygon(editBrush, pts);
            g.DrawLine(editPen, pos, 0, pos, height - 1);
        }

        private void DrawSelectedEditPoint(Graphics g, int i, Brush editBrush, Pen editPen, int height)
        {
            int pos = (int)(editPoints[i].position - startPosition) / (bytesPerSample * samplesPerPixel);
            Point[] pts = new Point[3];
            pts[0].X = pos - 5;
            pts[0].Y = 0;
            pts[1].X = pos + 5;
            pts[1].Y = 0;
            pts[2].X = pos;
            pts[2].Y = 5;
            g.FillPolygon(editBrush, pts);
            pts[0].X = pos - 6;
            pts[0].Y = (int)height - 1;
            pts[1].X = pos + 6;
            pts[1].Y = (int)height - 1;
            pts[2].X = pos;
            pts[2].Y = (int)height - 7;
            g.FillPolygon(editBrush, pts);
            g.DrawLine(editPen, pos, 0, pos, height - 1);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            int i;

            try
            {
                float height = (float)(this.Height - scrollHeight);

                long position = startPosition;
                long beginPosition = position;
                long endPosition = position + (Width * bytesPerSample * samplesPerPixel);

                int startSelectionX = (int)(startSelectionPosition - startPosition) / (samplesPerPixel * bytesPerSample);
                int endSelectionX = (int)(endSelectionPosition - startPosition) / (samplesPerPixel * bytesPerSample);

                myBuffer = currentContext.Allocate(e.Graphics, this.ClientRectangle);

                Brush brush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(250, 250, 255), Color.FromArgb(180, 180, 255), 90);
                myBuffer.Graphics.FillRectangle(brush, this.ClientRectangle);

                // draw timing lines
                int startPixels;
                // calculate start time
                long startTimeMillis = (position * 1000) / (bytesPerSample * waveStream.WaveFormat.SampleRate);
                // calculate end time
                long endTimeMillis = ((position * 1000) + ((long)Width * (long)samplesPerPixel * (long)bytesPerSample * 1000)) / ((long)bytesPerSample * (long)waveStream.WaveFormat.SampleRate);
                // determine skip frequency for ticks
                long diff = endTimeMillis - startTimeMillis;
                // we want about 20 ticks
                int factor = (int)(Math.Pow(10.0f, (double)((int)Math.Log10(diff / 10))));
                long skipMS = ((int)((diff / 10.0) / factor)) * factor * 2;

                int skipPixels = (int)(((skipMS * waveStream.WaveFormat.AverageBytesPerSecond) / 1000.0f) / (samplesPerPixel * bytesPerSample));

                long offsetMS = (startTimeMillis % skipMS);
                if (offsetMS > 0)
                {
                    startTimeMillis += (skipMS - offsetMS);
                    int offsetPixels = (int)(((skipMS - offsetMS) * waveStream.WaveFormat.AverageBytesPerSecond) / 1000.0f) / (samplesPerPixel * bytesPerSample);
                    startPixels = offsetPixels;
                }
                else
                    startPixels = 0;

                long t;
                int x;
                for (x = startPixels, t = startTimeMillis; x < this.Width; x += skipPixels, t += skipMS)
                {
                    if (skipPixels < 30)
                    {
                        if (skipMS > 3600000)
                            myBuffer.Graphics.DrawString(String.Format("{0:0}h", t / 3600000.0f), timingFont, blackBrush, new PointF(x, 0));
                        else
                            if (skipMS > 60000)
                                myBuffer.Graphics.DrawString(String.Format("{0:0}m", t / 60000.0f), timingFont, blackBrush, new PointF(x, 0));
                            else
                                if (skipMS > 100)
                                    myBuffer.Graphics.DrawString(String.Format("{0:0}s", t / 1000.0f), timingFont, blackBrush, new PointF(x, 0));
                                else
                                    myBuffer.Graphics.DrawString(String.Format("{0:0}", t), timingFont, blackBrush, new PointF(x, 0));
                    }
                    else
                    {
                        if (skipMS > 3600000)
                            myBuffer.Graphics.DrawString(String.Format("{0:n}h", t / 3600000.0f), timingFont, blackBrush, new PointF(x, 0));
                        else
                            if (skipMS > 60000)
                                myBuffer.Graphics.DrawString(String.Format("{0:n}m", t / 60000.0f), timingFont, blackBrush, new PointF(x, 0));
                            else
                                if (skipMS > 100)
                                    myBuffer.Graphics.DrawString(String.Format("{0:n}s", t / 1000.0f), timingFont, blackBrush, new PointF(x, 0));
                                else
                                    myBuffer.Graphics.DrawString(String.Format("{0:n}", t), timingFont, blackBrush, new PointF(x, 0));
                    }

                    myBuffer.Graphics.DrawLine(tickPen, x, 0, x, height - 1);
                }

                byte[] waveData = memoryStream.GetBuffer();
                int bytesToProcess = samplesPerPixel * bytesPerSample;

                if (minValues.Length < (e.ClipRectangle.Width + e.ClipRectangle.X))
                {
                    minValues = new float[e.ClipRectangle.Width + e.ClipRectangle.X];
                    maxValues = new float[e.ClipRectangle.Width + e.ClipRectangle.X];
                }

                bool skipTaken = false;

                for (x = 0; (x < e.ClipRectangle.Right) && (position < waveData.Length); x++)
                {
                    if ((x >= skipAudioX)&&(!skipTaken))
                    {
                        x += skipAudioAmount;
                        skipTaken = true;
                    }
                    short low = BitConverter.ToInt16(waveData, (int)position);
                    short high = low;
                    long n;
                    for (n = position + bytesPerSample; (n < (position + bytesToProcess)) && (n < waveData.Length); n += bytesPerSample)
                    {
                        short sample = BitConverter.ToInt16(waveData, (int)n);
                        if (sample < low) low = sample;
                        if (sample > high) high = sample;
                    }

                    position = n;
                    float lowPercent = ((((float)low) - short.MinValue) / ushort.MaxValue);
                    float highPercent = ((((float)high) - short.MinValue) / ushort.MaxValue);

                    minValues[x] = height * lowPercent;
                    maxValues[x] = height * highPercent;

                    int l = (int)minValues[x];
                    int h = (int)maxValues[x] + 1;
                    myBuffer.Graphics.DrawLine(linePen, x, l, x, h);
                }

                for (i = 0; i < editPoints.Count; i++)
                {
                    if ((editPoints[i].position > beginPosition) && (editPoints[i].position < endPosition))
                    {
                        if (i == editPointSelected)
                            DrawSelectedEditPoint(myBuffer.Graphics, i, editSelectedBrush, editSelectedPen, (int)height);
                        else
                            DrawEditPoint(editPoints[i], myBuffer.Graphics, (int)height);

                    }
                }

                if (!mouseDrag)
                {
                    int hover = editPointHover;
                    if (hover >= 0)
                    {
                        x = (int)((long)editPoints[hover].position - startPosition) / (samplesPerPixel * bytesPerSample);
                        if ((x>0)&&(x<e.ClipRectangle.Right))
                            ControlPaint.DrawReversibleFrame(RectangleToScreen(new Rectangle((int)x - 8, 0, 16, Height - scrollHeight)), Color.Black, FrameStyle.Dashed);
                    }
                }

                myBuffer.Render(e.Graphics);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //base.OnPaint(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Left:
                    this.AdjustScroll(-(e.Modifiers == Keys.None ? this.HorizontalScroll.SmallChange : this.HorizontalScroll.LargeChange), 0);
                    break;
                case Keys.Right:
                    this.AdjustScroll(e.Modifiers == Keys.None ? this.HorizontalScroll.SmallChange : this.HorizontalScroll.LargeChange, 0);
                    break;
                case Keys.Up:
                    this.AdjustScroll(0, -(e.Modifiers == Keys.None ? this.VerticalScroll.SmallChange : this.VerticalScroll.LargeChange));
                    break;
                case Keys.Down:
                    this.AdjustScroll(0, e.Modifiers == Keys.None ? this.VerticalScroll.SmallChange : this.VerticalScroll.LargeChange);
                    break;
            }
        }

        public void ClearSelection()
        {
            if ((startSelectionPosition >= 0) && (endSelectionPosition >= 0))
            {
                int startX = (int)(startSelectionPosition - startPosition) / (samplesPerPixel * bytesPerSample);
                int endX = (int)(endSelectionPosition - startPosition) / (samplesPerPixel * bytesPerSample);

                if (startX < 0) startX = 0;
                if (endX < 0) endX = 0;
                if (startX > Width) startX = Width;
                if (endX > Width) endX = Width;
                Invalidate(new Rectangle(startX, 0, Math.Max(1, endX - startX), Height - scrollHeight));
                //ControlPaint.FillReversibleRectangle(RectangleToScreen(new Rectangle(startX, 0, Math.Max(1, endX - startX), Height - scrollHeight)), Color.Black);
            }

            startSelectionPosition = endSelectionPosition = -1;
            currentSelectionStart = currentSelectionEnd = -1;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (isPlaying) return;

            if (!mouseDrag)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    ClearSelection();

                    mouseDrag = true;

                    startSelection = endSelection = e.Location;

                    // determine if we selected an edit point
                    if (editPointHover >= 0)
                    {
                        int x = (int)((long)editPoints[editPointHover].position - startPosition) / (samplesPerPixel * bytesPerSample);
                        ControlPaint.DrawReversibleFrame(RectangleToScreen(new Rectangle((int)x - 8, 0, 16, Height - scrollHeight)), Color.Black, FrameStyle.Dashed);

                        if ((startSelection.X < (x + editPointWidth)) && (startSelection.X > (x - editPointWidth)))
                        {
                            if (editPointSelected != editPointHover)
                            {
                                if (editPointSelected >= 0)
                                    DrawEditPoint(editPoints[editPointSelected], CreateGraphics(), this.Height - scrollHeight);

                                DrawSelectedEditPoint(CreateGraphics(), editPointHover, editSelectedBrush, editSelectedPen, this.Height - scrollHeight);
                                editPointSelected = editPointHover;
                                editPointPosition = editPoints[editPointHover].position;
                                oldLocation = x;
                            }

                            currentSelectionStart = (int)editPoints[editPointSelected].position / (samplesPerPixel * bytesPerSample);
                        }
                        else
                        {
                            if (editPointSelected >= 0)
                            {
                                DrawEditPoint(editPoints[editPointSelected], CreateGraphics(), this.Height - scrollHeight);
                                editPointSelected = -1;
                            }
                            UpdateSelection(e.X, e.X);
                        }
                    }
                    else
                    {
                        if (editPointSelected >= 0)
                        {
                            DrawEditPoint(editPoints[editPointSelected], CreateGraphics(), this.Height - scrollHeight);
                            editPointSelected = -1;
                        }
                        UpdateSelection(e.X, e.X);
                    }

                    startSelectionPosition = endSelectionPosition = startPosition + (long)(currentSelectionStart * samplesPerPixel * bytesPerSample);
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isPlaying) return;

            base.OnMouseMove(e);

            if (mouseDrag)
            {
                if ((editPointSelected >= 0) && (editPointHover >= 0))
                {
                    long restore = editPoints[editPointSelected].position - editPointPosition;
                    editPoints[editPointSelected].position = startPosition + (long)(e.Location.X * samplesPerPixel * bytesPerSample);
                    long offset = editPoints[editPointSelected].position - editPointPosition;

                    //if CTRL key is pressed move all subsequent edit points
                    if ((Control.ModifierKeys & Keys.Control) != 0)
                    {
                        int i;
                        for (i = editPointSelected + 1; i < editPoints.Count; i++)
                            editPoints[i].position += offset - restore;

                        this.Invalidate();
                    }
                    else
                        this.Invalidate(new Rectangle(oldLocation - editPointWidth, 0, e.Location.X + editPointWidth, Height - scrollHeight));

                    currentSelectionStart = currentSelectionEnd = oldLocation = e.Location.X;
                }
                else
                {
                    // if CTRL key pressed ... shift everything over
                    if ((Control.ModifierKeys & Keys.Control) != 0)
                    {
                        if (e.Location.X > startSelection.X)
                        {
                            long restore = startPosition + (long)((endSelection.X - startSelection.X) * samplesPerPixel * bytesPerSample);
                            long offset = startPosition + (long)((e.Location.X - startSelection.X) * samplesPerPixel * bytesPerSample);

                            int i;
                            for (i = 0; i < editPoints.Count; i++)
                            {
                                if (editPoints[i].position > startSelectionPosition)
                                    editPoints[i].position += offset - restore;
                            }

                            skipAudioAmount = Math.Abs(e.Location.X - startSelection.X);
                            skipAudioX = startSelection.X;

                            currentSelectionEnd = e.Location.X;

                            this.Invalidate();
                        }
                    }
                    else
                        UpdateSelection(startSelection.X, e.Location.X);
                }

                endSelection = e.Location;
                startSelectionPosition = startPosition + (long)(currentSelectionStart * samplesPerPixel * bytesPerSample);
                endSelectionPosition = startPosition + (long)(currentSelectionEnd * samplesPerPixel * bytesPerSample);
            }
            else
            {
                long position = startPosition + (long)(e.Location.X * samplesPerPixel * bytesPerSample);
                long beginPosition = position - (4 * samplesPerPixel * bytesPerSample);
                long endPosition = position + (4 * samplesPerPixel * bytesPerSample);
                int newEditPointHover = -1;
                for (int i = 0; i < editPoints.Count; i++)
                {
                    if ((editPoints[i].position > beginPosition) && (editPoints[i].position < endPosition) && IsStateVisible(editPoints[i]))
                    {
                        newEditPointHover = i;
                        break;
                    }
                }

                if (newEditPointHover != editPointHover)
                {
                    if (editPointHover >= 0)
                    {
                        int x = (int)((long)editPoints[editPointHover].position - startPosition) / (samplesPerPixel * bytesPerSample);
                        ControlPaint.DrawReversibleFrame(RectangleToScreen(new Rectangle((int)x - 8, 0, 16, Height - scrollHeight)), Color.Black, FrameStyle.Dashed);
                    }
                    editPointHover = newEditPointHover;
                    if (editPointHover >= 0)
                    {
                        int x = (int)((long)editPoints[editPointHover].position - startPosition) / (samplesPerPixel * bytesPerSample);
                        ControlPaint.DrawReversibleFrame(RectangleToScreen(new Rectangle((int)x - 8, 0, 16, Height - scrollHeight)), Color.Black, FrameStyle.Dashed);
                    }
                }
            }

/*
            if (e.Button == MouseButtons.Left)
            {
                if (!this.IsPanning)
                {
                    _startMousePosition = e.Location;
                    this.IsPanning = true;
                }

                if (this.IsPanning)
                {
                    int x;
                    int y;
                    Point position;

                    if (!this.InvertMouse)
                    {
                        x = -_startScrollPosition.X + (_startMousePosition.X - e.Location.X);
                        y = -_startScrollPosition.Y + (_startMousePosition.Y - e.Location.Y);
                    }
                    else
                    {
                        x = -(_startScrollPosition.X + (_startMousePosition.X - e.Location.X));
                        y = -(_startScrollPosition.Y + (_startMousePosition.Y - e.Location.Y));
                    }

                    position = new Point(x, y);

                    this.UpdateScrollPosition(position);
                }
            }
 */ 
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isPlaying) return;

            if (IsPanning) IsPanning = false;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mouseDrag = false;
                if ((editPointSelected >= 0) && (editPointHover >= 0))
                {
                    SetAbsoluteState(editPoints[editPointSelected], editPointSelected);

                    // move editPointSelected to appropriate sorted position
                    while ((editPointSelected>0)&&(editPoints[editPointSelected].position<editPoints[editPointSelected-1].position))
                    {
                        RobotState tmp = editPoints[editPointSelected-1];
                        editPoints[editPointSelected-1] = editPoints[editPointSelected];
                        editPoints[editPointSelected] = tmp;
                        editPointSelected--;
                    }

                    while (((editPointSelected+1)<editPoints.Count)&&(editPoints[editPointSelected].position>editPoints[editPointSelected+1].position))
                    {
                        RobotState tmp = editPoints[editPointSelected+1];
                        editPoints[editPointSelected+1] = editPoints[editPointSelected];
                        editPoints[editPointSelected] = tmp;
                        editPointSelected++;
                    }

                    SetRelativeState(editPoints[editPointSelected], editPointSelected);

                    // check if we have exceeded beyone the audio size ... if so we need to insert silence
                    // since audio is what drives the playback
                    long lastPosition = editPoints[editPoints.Count - 1].position;
                    if (lastPosition>waveStream.Length)
                        InsertAudioSilence(waveStream.Length, lastPosition + getOneSecondRange());

                    if (PositionChangeCallback != null)
                    {
                        PositionChangeCallback(this, null);
                    }
                    startSelectionPosition = endSelectionPosition = -1;
                }
                else
                {
                    if (endSelection.X == -1) return;

                    startSelectionPosition = endSelectionPosition = startPosition + (long)(currentSelectionStart * samplesPerPixel * bytesPerSample);
                    endSelectionPosition = startPosition + (long)(currentSelectionEnd * samplesPerPixel * bytesPerSample);

                    if (startSelectionPosition > endSelectionPosition)
                    {
                        long t = endSelectionPosition;
                        endSelectionPosition = startSelectionPosition;
                        startSelectionPosition = t;
                    }

                    if (skipAudioAmount > 0)
                    {
                        InsertAudioSilence(startSelectionPosition, endSelectionPosition);
                        skipAudioAmount = 0;
                        skipAudioX = -1;
                    }

                    if (startSelectionPosition != endSelectionPosition)
                    {
                        if (ClipboardCallback != null)
                        {
                            ClipboardCallback(this, EventArgs.Empty);
                        }
                    }
                }

                if (editPointHover >= 0)
                {
                    int x = (int)((long)editPoints[editPointHover].position - startPosition) / (samplesPerPixel * bytesPerSample);
                    ControlPaint.DrawReversibleFrame(RectangleToScreen(new Rectangle((int)x - 8, 0, 16, Height - scrollHeight)), Color.Black, FrameStyle.Dashed);
                }
            }
            else
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Middle) 
                    FitToScreen();
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int increment;

            if (Control.ModifierKeys == Keys.None)
                increment = this.ZoomIncrement;
            else
                increment = this.ZoomIncrement * 5;

            if (e.Delta < 0)
                increment = -increment;

            this.ZoomValue += increment;
        }

        protected virtual void AdjustScroll(int x, int y)
        {
            Point scrollPosition;

            scrollPosition = new Point(this.HorizontalScroll.Value + x, this.VerticalScroll.Value + y);

            this.UpdateScrollPosition(scrollPosition);
        }

        protected virtual void UpdateScrollPosition(Point position)
        {
            this.AutoScrollPosition = position;
            this.Invalidate();
            this.OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, 0));
        }

        private void Recorder_HorzScrollValueChanged(object sender, ScrollEventArgs se)
        {
            startPosition = -AutoScrollPosition.X * samplesPerPixel * bytesPerSample;
            if (startPosition < 0) startPosition = 0;
            this.Invalidate();
        }

        public long getSelectionStart()
        {
            return startSelectionPosition;
        }

        public long getSelectionEnd()
        {
            return endSelectionPosition;
        }

        public long getOneSecondRange()
        {
            if (waveStream == null)
                return (16000 * 16 * 2) / 8;
            else
                return waveStream.WaveFormat.AverageBytesPerSecond;
        }

        public long getCurrentPosition()
        {
            return waveStream.Position;
        }

        public byte[] GetAudioData()
        {
            return memoryStream.GetBuffer();
        }

        public byte[] GetAudioFormat()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            waveStream.WaveFormat.Serialize(bw);
            return ms.ToArray();
        }

        public void SetAudioFormat(MemoryStream fmt)
        {
            fmt.Position = 0;
            BinaryReader br = new BinaryReader(fmt);
            WaveFormat wf = new WaveFormat(br);
            waveStream = new RawSourceWaveStream(memoryStream, wf);

            bytesPerSample = (waveStream.WaveFormat.BitsPerSample / 8) * waveStream.WaveFormat.Channels;
            totalSamples = (int)(waveStream.Length / bytesPerSample);
        }

        public void SetAudioData(MemoryStream ms)
        {
            ms.Position = 0;
            memoryStream = ms;
            waveStream = new RawSourceWaveStream(memoryStream, waveStream.WaveFormat);

            bytesPerSample = (waveStream.WaveFormat.BitsPerSample / 8) * waveStream.WaveFormat.Channels;
            totalSamples = (int)(waveStream.Length / bytesPerSample);

            FitToScreen();
        }

        public WaveFormat GetWaveFormat()
        {
            if (waveStream == null)
                return new WaveFormat(16000, 16, 2);
            else
                return waveStream.WaveFormat;
        }

        // insert movement frames based on amplitude of audio
        public void InsertAmpMouthMovements()
        {
            byte[] waveData = memoryStream.GetBuffer();
            short[] processedData = new short[(memoryStream.Length / (bytesPerSample * 500))+1];

            int windowSize = 1000 * bytesPerSample;
            int globalMax=0;
            int globalMin=0;
            long n, p, q;
            for (n = p = 0; p < waveData.Length; p += bytesPerSample * 500, n++)
            {
                int max = 0;
                int min = 0;

                for (q = p; (q < waveData.Length)&&(q<p+(bytesPerSample * 500)); q += bytesPerSample)
                {
                    int val = BitConverter.ToInt16(waveData, (int)q);
                    if (val > max) max = val;
                    if (val < min) min = val;
                }

                if (min < globalMin) globalMin = min;
                if (max > globalMax) globalMax = max;

                min = -min;

                if (min>max)
                    processedData[n] = (short)(min);
                else
                    processedData[n] = (short)(max);
            }

            globalMin = -globalMin;
            if (globalMin > globalMax) globalMax = globalMin;
            float factor = 32768.0f / globalMax;

            int i;
            int last = (int)((float)processedData[0]*factor);
            for (i = 1; i < n; i++)
            {
                int val = (int)((float)processedData[i-1]*factor);
                if (Math.Abs(last - val) > 4096)
                {
                    if (val < 0) val = -val;

                    RobotState rs = new RobotState();

                    rs.leftHorizontalEye = -1.0f;
                    rs.leftVerticalEye = -1.0f;
                    rs.rightHorizontalEye = -1.0f;
                    rs.rightVerticalEye = -1.0f;
                    rs.leftEyebrow = -1.0f;
                    rs.rightEyebrow = -1.0f;
                    rs.rightEyelid = -1.0f;
                    rs.leftEyelid = -1.0f;
                    rs.neckTilt = -1.0f;
                    rs.neckTwist = -1.0f;
                    rs.triggerPosition = 0;

                    rs.position = i*bytesPerSample * 500;
                    rs.jaw = (val / 32768.0f);
                    if (rs.jaw > 1.0f) rs.jaw = 1.0f;
                    editPoints.Add(rs);
                    last = val;
                }
            }

/*
            int i;
            memoryStream.Position = 0;
            for (i = 0; i < n; i ++)
            {
                int val = (int)((float)processedData[i]*factor);
                memoryStream.WriteByte((byte)(val & 255));
                memoryStream.WriteByte((byte)(val >> 8));
            }
*/
            Invalidate();
        }

        public int GetSelectedEditPoint()
        {
            return editPointSelected;
        }

        public void ClearSelectedEditPoint()
        {
            editPointSelected=-1;
        }

        public void SetRelativeState(RobotState curr, int stop)
        {
            RobotState last = new RobotState();
            // determine values up to this edit point
            int i;
            if (stop>editPoints.Count) stop = editPoints.Count;
            for (i = 0; i < stop; i++)
                UpdateState(last, editPoints[i]);

            if (last.leftHorizontalEye == curr.leftHorizontalEye) curr.leftHorizontalEye = -1;
            if (last.leftVerticalEye == curr.leftVerticalEye) curr.leftVerticalEye = -1;
            if (last.rightHorizontalEye == curr.rightHorizontalEye) curr.rightHorizontalEye = -1;
            if (last.rightVerticalEye == curr.rightVerticalEye) curr.rightVerticalEye = -1;
            if (last.leftEyebrow == curr.leftEyebrow) curr.leftEyebrow = -1;
            if (last.rightEyebrow == curr.rightEyebrow) curr.rightEyebrow = -1;
            if (last.rightEyelid == curr.rightEyelid) curr.rightEyelid = -1;
            if (last.leftEyelid == curr.leftEyelid) curr.leftEyelid = -1;
            if (last.leftLip == curr.leftLip) curr.leftLip = -1;
            if (last.rightLip == curr.rightLip) curr.rightLip = -1;
            if (last.jaw == curr.jaw) curr.jaw = -1;
            if (last.neckTilt == curr.neckTilt) curr.neckTilt = -1;
            if (last.neckTwist == curr.neckTwist) curr.neckTwist = -1;
        }

        public void SetAbsoluteState(RobotState curr, int stop)
        {
            RobotState last = new RobotState();
            // determine values up to this edit point
            int i;
            for (i = 0; i < stop; i++)
                UpdateState(last, editPoints[i]);

            if (curr.leftHorizontalEye == -1) curr.leftHorizontalEye = last.leftHorizontalEye;
            if (curr.leftVerticalEye == -1) curr.leftVerticalEye = last.leftVerticalEye;
            if (curr.rightHorizontalEye == -1) curr.rightHorizontalEye = last.rightHorizontalEye;
            if (curr.rightVerticalEye == -1) curr.rightVerticalEye = last.rightVerticalEye;
            if (curr.leftEyebrow == -1) curr.leftEyebrow = last.leftEyebrow;
            if (curr.rightEyebrow == -1) curr.rightEyebrow = last.rightEyebrow;
            if (curr.rightEyelid == -1) curr.rightEyelid = last.rightEyelid;
            if (curr.leftEyelid == -1) curr.leftEyelid = last.leftEyelid;
            if (curr.leftLip == -1) curr.leftLip = last.leftLip;
            if (curr.rightLip == -1) curr.rightLip = last.rightLip;
            if (curr.jaw == -1) curr.jaw = last.jaw;
            if (curr.neckTilt == -1) curr.neckTilt = last.neckTilt;
            if (curr.neckTwist == -1) curr.neckTwist = last.neckTwist;
        }

        public void UpdateState(RobotState curr, RobotState next)
        {
            if (next.leftHorizontalEye != -1) curr.leftHorizontalEye = next.leftHorizontalEye;
            if (next.leftVerticalEye != -1) curr.leftVerticalEye = next.leftVerticalEye;
            if (next.rightHorizontalEye != -1) curr.rightHorizontalEye = next.rightHorizontalEye;
            if (next.rightVerticalEye != -1) curr.rightVerticalEye = next.rightVerticalEye;
            if (next.leftEyebrow != -1) curr.leftEyebrow = next.leftEyebrow;
            if (next.rightEyebrow != -1) curr.rightEyebrow = next.rightEyebrow;
            if (next.rightEyelid != -1) curr.rightEyelid = next.rightEyelid;
            if (next.leftEyelid != -1) curr.leftEyelid = next.leftEyelid;
            if (next.leftLip != -1) curr.leftLip = next.leftLip;
            if (next.rightLip != -1) curr.rightLip = next.rightLip;
            if (next.jaw != -1) curr.jaw = next.jaw;
            if (next.neckTilt != -1) curr.neckTilt = next.neckTilt;
            if (next.neckTwist != -1) curr.neckTwist = next.neckTwist;
            curr.position = next.position;
            curr.triggerPosition = next.triggerPosition;
        }

        public void SetVisibleStates(bool sm, bool se, bool sn)
        {
            showMouth =sm;
            showEyes = se;
            showNeck = sn;

            editPointSelected = -1;

            Invalidate();
        }

        public long getPositionFromMillis(long millis)
        {
            return (millis * ((long)bytesPerSample * (long)waveStream.WaveFormat.SampleRate)) / 1000;
        }

        public long getMillisFromPosition(long pos)
        {
            return (pos * 1000) / (bytesPerSample * waveStream.WaveFormat.SampleRate);
        }
    }
}
