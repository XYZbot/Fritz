using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using NAudio.Wave;
using System.Runtime.InteropServices;

namespace Fritz
{
    public class RobotState : EventArgs
    {
        public float leftHorizontalEye = 0.5f;
        public float leftVerticalEye = 0.5f;
        public float rightHorizontalEye = 0.5f;
        public float rightVerticalEye = 0.5f;
        public float leftEyebrow = 0.5f;
        public float rightEyebrow = 0.5f;
        public float rightEyelid = 0.5f;
        public float leftEyelid = 0.5f;
        public float leftLip = 0.5f;
        public float rightLip = 0.5f;
        public float jaw = 0.5f;
        public float neckTilt = 0.5f;
        public float neckTwist = 0.5f;
        public long position = 0;
        public long triggerPosition = 0;

        public RobotState()
        {
        }

        public RobotState(bool empty)
        {
            leftHorizontalEye = -1;
            rightHorizontalEye = -1;
            leftVerticalEye = -1;
            rightVerticalEye = -1;
            leftEyebrow = -1;
            rightEyebrow = -1;
            leftEyelid = -1;
            rightEyelid = -1;
            leftLip = -1;
            rightLip = -1;
            jaw = -1;
            neckTilt = -1;
            neckTwist = -1;
        }

        public RobotState(RobotState r)
        {
            leftHorizontalEye = r.leftHorizontalEye;
            rightHorizontalEye = r.rightHorizontalEye;
            leftVerticalEye = r.leftVerticalEye;
            rightVerticalEye = r.rightVerticalEye;
            leftEyebrow = r.leftEyebrow;
            rightEyebrow = r.rightEyebrow;
            leftEyelid = r.leftEyelid;
            rightEyelid = r.rightEyelid;
            leftLip = r.leftLip;
            rightLip = r.rightLip;
            jaw = r.jaw;
            neckTilt = r.neckTilt;
            neckTwist = r.neckTwist;
            position = r.position;
        }
    }

    public class Conductor
    {
        Robot robot = new Robot();
        Simulator simulator = null;
        Recorder recorder = null;

        List<RobotState> copyStates = new List<RobotState>();
        RobotState lastState = new RobotState();
        List<RobotState> robotStates = null;
        List<RobotState> recordingStates = new List<RobotState>();
        long recordingOffset=0;

        // amount of servo rotation in 10ms
        const float servoRotationSpeed = 0.4f;
        // number of seconds for full 0.0 to 1.0 rotation
        const float servoRotationSpeedSeconds = 1.0f / (servoRotationSpeed * 100.0f);

        const float moveThreshold = 0.02f;

        private AutoResetEvent newData = new AutoResetEvent(false);

        public event EventHandler ConnectionChanged = null;

        DateTime recordingStart;

        bool paused = false;

        int actionActive = 0;
        long actionMode;
        long blinkStart;
        long neckStart;
        long rollStart;

        int playbackStateIndex = 0;
        RobotState playbackState = new RobotState();

        float f_leftHorizontalEye;
        float f_leftVerticalEye;
        float f_rightHorizontalEye;
        float f_rightVerticalEye;
        float f_leftEyebrow;
        float f_rightEyebrow;
        float f_rightEyelid;
        float f_leftEyelid;
        float f_leftLip;
        float f_rightLip;
        float f_jaw;
        float f_neckTilt;
        float f_neckTwist;

        float leftHorizontalEye;
        float leftVerticalEye;
        float rightHorizontalEye;
        float rightVerticalEye;
        float leftLip;
        float rightLip;
        float jaw;
        float neckTilt;
        float neckTwist;
        float leftEyebrow;
        float rightEyebrow;
        float leftEyelid;
        float rightEyelid;

        bool isRunning = true;

        static public String[] multiValueOptions = { "", "Eyebrows", "Eyelids", "Eyes Horizontal", "Eyes Vertical", "Lips", "Jaw", "Neck Twist", "Neck Tilt" };
        static public String[] singleValueOptions = { "", "Expression Afraid", "Expression Awkward", "Expression Angry", "Expression Disappointed", "Expression Happy", "Expression Neutral", "Expression Sad", "Expression Sinister", "Expression Sleepy", "Expression Smile", "Expression Sneaky", "Expression Sulk", "Expression Surprised", "Expression Yelling", "Expression Worried", "Eyebrows Slight Pinch", "Eyebrows Pinch", "Eyebrows Level", "Eyebrows Slight Spread", "Eyebrows Spread", "Eyebrow Left Pinch", "Eyebrow Left Spread", "Eyebrow Right Pinch", "Eyebrow Right Spread", "Eyes Center", "Eyes Slight Left", "Eyes Left", "Eyes Slight Right", "Eyes Right", "Eyes Slight Up", "Eyes Up", "Eyes Slight Down", "Eyes Down", "Eyes Roll", "Eyes Pinch", "Eyes Spread", "Eyelids Blink", "Eyelid Left Blink", "Eyelid Right Blink", "Eyelids Open", "Eyelids Half", "Eyelids Close", "Lips Smile", "Lips Level", "Lips Frown", "Lips Left Diagonal", "Lips Right Diagonal", "Jaw Slight Open", "Jaw Open", "Jaw Half", "Jaw Slight Close", "Jaw Close", "Neck Slight Left", "Neck Left", "Neck Front", "Neck Slight Right", "Neck Right", "Neck Slight Up", "Neck Up", "Neck Front", "Neck Slight Down", "Neck Down", "Neck Yes", "Neck No", "Say 'Hello'", "Say 'Goodbye'", "Say 'How are you'", "Say 'My name is Fritz'", "Say 'What is your name?'", "Say 'I am a robot'", "Say 'Are you human?'"};

        bool isRecording = false;
        bool isPaused = false;
        bool isPlaying = false;

        bool triggerCheckSonar = true;
        bool triggerCheckIR = false;
        int triggerDistance = 60;
        bool triggerEnabled = false;

        public bool triggerPlay = false;
        public bool calibrating = false;

        int speakVoiceIndex = 1;
        
        public Conductor()
        {
            RobotState initState = new RobotState();

            leftHorizontalEye = f_leftHorizontalEye = initState.leftHorizontalEye;
            leftVerticalEye = f_leftVerticalEye = initState.leftVerticalEye;
            rightHorizontalEye = f_rightHorizontalEye = initState.rightHorizontalEye;
            rightVerticalEye = f_rightVerticalEye = initState.rightVerticalEye;
            leftEyebrow = f_leftEyebrow = initState.leftEyebrow;
            rightEyebrow = f_rightEyebrow = initState.rightEyebrow;
            rightEyelid = f_rightEyelid = initState.rightEyelid;
            leftEyelid = f_leftEyelid = initState.leftEyelid;
            leftLip = f_leftLip = initState.leftLip;
            rightLip = f_rightLip = initState.rightLip;
            jaw = f_jaw = initState.jaw;
            neckTilt = f_neckTilt = initState.neckTilt;
            neckTwist = f_neckTwist = initState.neckTwist;

            Thread thread = new Thread(new ThreadStart(WorkThreadFunction));
            thread.Start();

            lastState.leftEyebrow = 2;
            lastState.leftEyelid = 2;
            lastState.leftLip = 2;
            lastState.leftHorizontalEye = 2;
            lastState.leftVerticalEye = 2;
            lastState.jaw = 2;
            lastState.neckTilt = 2;
            lastState.neckTwist = 2;
            lastState.position = 2;
            lastState.rightEyebrow = 2;
            lastState.rightEyelid = 2;
            lastState.rightLip = 2;
            lastState.rightHorizontalEye = 2;
            lastState.rightVerticalEye = 2;
            lastState.triggerPosition = 2;
        }

        public static int GetMultiIndex(String s, String def)
        {
            int i;
            if (s.Length <= 0)
            {
                s = def;
                if (s.Length <= 0) return 0;
            }

            for (i = 0; i < Conductor.multiValueOptions.Length; i++)
                if (s.Equals(Conductor.multiValueOptions[i]))
                    return i;

            return 0;
        }

        public static int GetSingleIndex(String s, String def)
        {
            int i;
            if (s.Length <= 0)
            {
                s = def;
                if (s.Length <= 0) return 0;
            }

            for (i = 0; i < Conductor.singleValueOptions.Length; i++)
                if (s.Equals(Conductor.singleValueOptions[i]))
                    return i;

            return 0;
        }

        public void SetControls(ref List<RobotState> rs, ref Recorder re, ref Simulator s)
        {
            recorder = re;
            simulator = s;
            robotStates = rs;

            recorder.PlaybackTick += new EventHandler(recorder_PlaybackTick);
            recorder.MouseDown += new MouseEventHandler(recorder_MouseDown);

            simulator.StateChangeCallback += new EventHandler(simulator_StateChangeCallback);
            recorder.PositionChangeCallback += new EventHandler(recorder_PositionChangeCallback);

            robot.LoadConfig();
        }

        public void Reset()
        {
            RobotState initState = new RobotState();

            f_leftHorizontalEye = initState.leftHorizontalEye;
            f_leftVerticalEye = initState.leftVerticalEye;
            f_rightHorizontalEye = initState.rightHorizontalEye;
            f_rightVerticalEye = initState.rightVerticalEye;
            f_leftEyebrow = initState.leftEyebrow;
            f_rightEyebrow = initState.rightEyebrow;
            f_rightEyelid = initState.rightEyelid;
            f_leftEyelid = initState.leftEyelid;
            f_leftLip = initState.leftLip;
            f_rightLip = initState.rightLip;
            f_jaw = initState.jaw;
            f_neckTilt = initState.neckTilt;
            f_neckTwist = initState.neckTwist;

            newData.Set();
        }

        public void SetState(float n_leftHorizontalEye, float n_leftVerticalEye, float n_rightHorizontalEye, float n_rightVerticalEye, float n_leftEyebrow, float n_rightEyebrow, float n_rightEyelid, float n_leftEyelid, float n_leftLip, float n_rightLip, float n_jaw, float n_neckTilt, float n_neckTwist)
        {
            if (n_leftHorizontalEye != -1) f_leftHorizontalEye = n_leftHorizontalEye;
            if (n_leftVerticalEye != -1) f_leftVerticalEye = n_leftVerticalEye;
            if (n_rightHorizontalEye != -1) f_rightHorizontalEye = n_rightHorizontalEye;
            if (n_rightVerticalEye != -1) f_rightVerticalEye = n_rightVerticalEye;
            if (n_leftEyebrow != -1) f_leftEyebrow = n_leftEyebrow;
            if (n_rightEyebrow != -1) f_rightEyebrow = n_rightEyebrow;
            if (n_rightEyelid != -1) f_rightEyelid = n_rightEyelid;
            if (n_leftEyelid != -1) f_leftEyelid = n_leftEyelid;
            if (n_leftLip != -1) f_leftLip = n_leftLip;
            if (n_rightLip != -1) f_rightLip = n_rightLip;
            if (n_jaw != -1) f_jaw = n_jaw;
            if (n_neckTilt != -1) f_neckTilt = n_neckTilt;
            if (n_neckTwist != -1) f_neckTwist = n_neckTwist;

            newData.Set();
        }

/*
                if (mouseDown)
                {
                    f_leftHorizontalEye = leftHorizontalEye;
                    f_leftVerticalEye = leftVerticalEye;
                    f_rightHorizontalEye = rightHorizontalEye;
                    f_rightVerticalEye = rightVerticalEye;
                    f_leftEyebrow = leftEyebrow;
                    f_rightEyebrow = rightEyebrow;
                    f_rightEyelid = rightEyelid;
                    f_leftEyelid = leftEyelid;
                    f_leftLip = leftLip;
                    f_rightLip = rightLip;
                    f_jaw = jaw;
                    f_neckTilt = neckTilt;
                    f_neckTwist = neckTwist;

                    robot.UpdateState((RobotState)e);
                    triggerRobotChangeCallback();
                    //triggerDirectStateChangeCallback();
                }
                else
*/

        public void SetState(RobotState ss)
        {
            if (ss.leftHorizontalEye != -1) f_leftHorizontalEye = ss.leftHorizontalEye;
            if (ss.leftVerticalEye != -1) f_leftVerticalEye = ss.leftVerticalEye;
            if (ss.rightHorizontalEye != -1) f_rightHorizontalEye = ss.rightHorizontalEye;
            if (ss.rightVerticalEye != -1) f_rightVerticalEye = ss.rightVerticalEye;
            if (ss.leftEyebrow != -1) f_leftEyebrow = ss.leftEyebrow;
            if (ss.rightEyebrow != -1) f_rightEyebrow = ss.rightEyebrow;
            if (ss.rightEyelid != -1) f_rightEyelid = ss.rightEyelid;
            if (ss.leftEyelid != -1) f_leftEyelid = ss.leftEyelid;
            if (ss.leftLip != -1) f_leftLip = ss.leftLip;
            if (ss.rightLip != -1) f_rightLip = ss.rightLip;
            if (ss.jaw != -1) f_jaw = ss.jaw;
            if (ss.neckTilt != -1) f_neckTilt = ss.neckTilt;
            if (ss.neckTwist != -1) f_neckTwist = ss.neckTwist;

            newData.Set();
        }

        /*
         * Sets the face position based on an interpolated position between two states
         */
        public void SetState(RobotState curr, RobotState next, long position)
        {
            float percent;

            // if we are not in the trigger range for the next face assume current configuration
            if (position < next.triggerPosition)
                percent = 0.0f;
            else
                if (position == next.triggerPosition)
                    percent = (float)1.0f;
                else
                    if (next.position == next.triggerPosition)
                        percent = (float)0.0f;
                    else
                        percent = (float)(position - next.triggerPosition) / (float)(next.position - next.triggerPosition);

            if (next.leftHorizontalEye != -1) f_leftHorizontalEye = curr.leftHorizontalEye + ((float)(next.leftHorizontalEye - curr.leftHorizontalEye) * percent); else f_leftHorizontalEye = curr.leftHorizontalEye;
            if (next.leftVerticalEye != -1) f_leftVerticalEye = curr.leftVerticalEye + ((float)(next.leftVerticalEye - curr.leftVerticalEye) * percent); else f_leftVerticalEye = curr.leftVerticalEye;
            if (next.rightHorizontalEye != -1) f_rightHorizontalEye = curr.rightHorizontalEye + ((float)(next.rightHorizontalEye - curr.rightHorizontalEye) * percent); else f_rightHorizontalEye = curr.rightHorizontalEye;
            if (next.rightVerticalEye != -1) f_rightVerticalEye = curr.rightVerticalEye + ((float)(next.rightVerticalEye - curr.rightVerticalEye) * percent); else f_rightVerticalEye = curr.rightVerticalEye;
            if (next.leftEyebrow != -1) f_leftEyebrow = curr.leftEyebrow + ((float)(next.leftEyebrow - curr.leftEyebrow) * percent); else f_leftEyebrow = curr.leftEyebrow;
            if (next.rightEyebrow != -1) f_rightEyebrow = curr.rightEyebrow + ((float)(next.rightEyebrow - curr.rightEyebrow) * percent); else f_rightEyebrow = curr.rightEyebrow;
            if (next.rightEyelid != -1) f_rightEyelid = curr.rightEyelid + ((float)(next.rightEyelid - curr.rightEyelid) * percent); else f_rightEyelid = curr.rightEyelid;
            if (next.leftEyelid != -1) f_leftEyelid = curr.leftEyelid + ((float)(next.leftEyelid - curr.leftEyelid) * percent); else f_leftEyelid = curr.leftEyelid;
            if (next.leftLip != -1) f_leftLip = curr.leftLip + ((float)(next.leftLip - curr.leftLip) * percent); else f_leftLip = curr.leftLip;
            if (next.rightLip != -1) f_rightLip = curr.rightLip + ((float)(next.rightLip - curr.rightLip) * percent); else f_rightLip = curr.rightLip;
            if (next.jaw != -1) f_jaw = curr.jaw + ((float)(next.jaw - curr.jaw) * percent); else f_jaw = curr.jaw;
            if (next.neckTilt != -1) f_neckTilt = curr.neckTilt + ((float)(next.neckTilt - curr.neckTilt) * percent); else f_neckTilt = curr.neckTilt;
            if (next.neckTwist != -1) f_neckTwist = curr.neckTwist + ((float)(next.neckTwist - curr.neckTwist) * percent); else f_neckTwist = curr.neckTwist;

            newData.Set();
        }

        public void SetStates(ref List<RobotState> s)
        {
            robotStates = s;
            recorder.ClearSelectedEditPoint();
            recorder.SetEditPoints(ref robotStates);
            CalculateTriggers(ref robotStates, recorder.getOneSecondRange());
        }

        private RobotState GetState()
        {
            RobotState state = new RobotState();

            state.leftHorizontalEye = leftHorizontalEye;
            state.leftVerticalEye = leftVerticalEye;
            state.rightHorizontalEye = rightHorizontalEye;
            state.rightVerticalEye = rightVerticalEye;
            state.leftEyebrow = leftEyebrow;
            state.rightEyebrow = rightEyebrow;
            state.rightEyelid = rightEyelid;
            state.leftEyelid = leftEyelid;
            state.leftLip = leftLip;
            state.rightLip = rightLip;
            state.jaw = jaw;
            state.neckTilt = neckTilt;
            state.neckTwist = neckTwist;

            return state;
        }

        private RobotState GetFinalState()
        {
            RobotState state = new RobotState();

            state.leftHorizontalEye = f_leftHorizontalEye;
            state.leftVerticalEye = f_leftVerticalEye;
            state.rightHorizontalEye = f_rightHorizontalEye;
            state.rightVerticalEye = f_rightVerticalEye;
            state.leftEyebrow = f_leftEyebrow;
            state.rightEyebrow = f_rightEyebrow;
            state.rightEyelid = f_rightEyelid;
            state.leftEyelid = f_leftEyelid;
            state.leftLip = f_leftLip;
            state.rightLip = f_rightLip;
            state.jaw = f_jaw;
            state.neckTilt = f_neckTilt;
            state.neckTwist = f_neckTwist;

            return state;
        }

        // animates the values to simulate slower servo movement
        public void WorkThreadFunction()
        {
            bool lastChanged;
            bool changed = false;
            bool wasConnected = false;

            while (isRunning)
            {
                lastChanged = changed;
                changed = false;

                float diff;

                diff = f_leftHorizontalEye - leftHorizontalEye;
                if (Math.Abs(diff) < servoRotationSpeed)
                {
                    if (f_leftHorizontalEye != leftHorizontalEye)
                    {
                        leftHorizontalEye = f_leftHorizontalEye;
                        changed = true;
                    }
                }
                else
                {
                    if (diff < 0)
                    {
                        leftHorizontalEye -= servoRotationSpeed;
                        if (leftHorizontalEye < f_leftHorizontalEye) leftHorizontalEye = f_leftHorizontalEye;
                    }
                    else
                    {
                        leftHorizontalEye += servoRotationSpeed;
                        if (leftHorizontalEye > f_leftHorizontalEye) leftHorizontalEye = f_leftHorizontalEye;
                    }
                    changed = true;
                }

                diff = f_leftVerticalEye - leftVerticalEye;
                if (Math.Abs(diff) < servoRotationSpeed)
                {
                    if (f_leftVerticalEye != leftVerticalEye)
                    {
                        leftVerticalEye = f_leftVerticalEye;
                        changed = true;
                    }
                }
                else
                {
                    if (diff < 0)
                    {
                        leftVerticalEye -= servoRotationSpeed;
                        if (leftVerticalEye < f_leftVerticalEye) leftVerticalEye = f_leftVerticalEye;
                    }
                    else
                    {
                        leftVerticalEye += servoRotationSpeed;
                        if (leftVerticalEye > f_leftVerticalEye) leftVerticalEye = f_leftVerticalEye;
                    }
                    changed = true;
                }

                diff = f_rightHorizontalEye - rightHorizontalEye;
                if (Math.Abs(diff) < servoRotationSpeed)
                {
                    if (f_rightHorizontalEye != rightHorizontalEye)
                    {
                        rightHorizontalEye = f_rightHorizontalEye;
                        changed = true;
                    }
                }
                else
                {
                    if (diff < 0)
                    {
                        rightHorizontalEye -= servoRotationSpeed;
                        if (rightHorizontalEye < f_rightHorizontalEye) rightHorizontalEye = f_rightHorizontalEye;
                    }
                    else
                    {
                        rightHorizontalEye += servoRotationSpeed;
                        if (rightHorizontalEye > f_rightHorizontalEye) rightHorizontalEye = f_rightHorizontalEye;
                    }
                    changed = true;
                }

                diff = f_rightVerticalEye - rightVerticalEye;
                if (Math.Abs(diff) < servoRotationSpeed)
                {
                    if (f_rightVerticalEye != rightVerticalEye)
                    {
                        rightVerticalEye = f_rightVerticalEye;
                        changed = true;
                    }
                }
                else
                {
                    if (diff < 0)
                    {
                        rightVerticalEye -= servoRotationSpeed;
                        if (rightVerticalEye < f_rightVerticalEye) rightVerticalEye = f_rightVerticalEye;
                    }
                    else
                    {
                        rightVerticalEye += servoRotationSpeed;
                        if (rightVerticalEye > f_rightVerticalEye) rightVerticalEye = f_rightVerticalEye;
                    }
                    changed = true;
                }

                diff = f_leftEyebrow - leftEyebrow;
                if (Math.Abs(diff) < servoRotationSpeed)
                {
                    if (f_leftEyebrow != leftEyebrow)
                    {
                        leftEyebrow = f_leftEyebrow;
                        changed = true;
                    }
                }
                else
                {
                    if (diff < 0)
                    {
                        leftEyebrow -= servoRotationSpeed;
                        if (leftEyebrow < f_leftEyebrow) leftEyebrow = f_leftEyebrow;
                    }
                    else
                    {
                        leftEyebrow += servoRotationSpeed;
                        if (leftEyebrow > f_leftEyebrow) leftEyebrow = f_leftEyebrow;
                    }
                    changed = true;
                }

                diff = f_rightEyebrow - rightEyebrow;
                if (Math.Abs(diff) < servoRotationSpeed)
                {
                    if (f_rightEyebrow != rightEyebrow)
                    {
                        rightEyebrow = f_rightEyebrow;
                        changed = true;
                    }
                }
                else
                {
                    if (diff < 0)
                    {
                        rightEyebrow -= servoRotationSpeed;
                        if (rightEyebrow < f_rightEyebrow) rightEyebrow = f_rightEyebrow;
                    }
                    else
                    {
                        rightEyebrow += servoRotationSpeed;
                        if (rightEyebrow > f_rightEyebrow) rightEyebrow = f_rightEyebrow;
                    }
                    changed = true;
                }

                diff = f_leftEyelid - leftEyelid;
                if (Math.Abs(diff) < servoRotationSpeed)
                {
                    if (f_leftEyelid != leftEyelid)
                    {
                        leftEyelid = f_leftEyelid;
                        changed = true;
                    }
                }
                else
                {
                    if (diff < 0)
                    {
                        leftEyelid -= servoRotationSpeed;
                        if (leftEyelid < f_leftEyelid) rightEyebrow = f_rightEyebrow;
                    }
                    else
                    {
                        leftEyelid += servoRotationSpeed;
                        if (leftEyelid > f_leftEyelid) rightEyebrow = f_rightEyebrow;
                    }
                    changed = true;
                }

                diff = f_rightEyelid - rightEyelid;
                if (Math.Abs(diff) < servoRotationSpeed)
                {
                    if (f_rightEyelid != rightEyelid)
                    {
                        rightEyelid = f_rightEyelid;
                        changed = true;
                    }
                }
                else
                {
                    if (diff < 0)
                    {
                        rightEyelid -= servoRotationSpeed;
                        if (rightEyelid < f_rightEyelid) rightEyelid = f_rightEyelid;

                    }
                    else
                    {
                        rightEyelid += servoRotationSpeed;
                        if (rightEyelid > f_rightEyelid) rightEyelid = f_rightEyelid;
                    }
                    changed = true;
                }

                diff = f_leftLip - leftLip;
                if (Math.Abs(diff) < servoRotationSpeed)
                {
                    if (f_leftLip != leftLip)
                    {
                        leftLip = f_leftLip;
                        changed = true;
                    }
                }
                else
                {
                    if (diff < 0)
                    {
                        leftLip -= servoRotationSpeed;
                        if (leftLip < f_leftLip) leftLip = f_leftLip;
                    }
                    else
                    {
                        leftLip += servoRotationSpeed;
                        if (leftLip > f_leftLip) leftLip = f_leftLip;
                    }
                    changed = true;
                }

                diff = f_rightLip - rightLip;
                if (Math.Abs(diff) < servoRotationSpeed)
                {
                    if (f_rightLip != rightLip)
                    {
                        rightLip = f_rightLip;
                        changed = true;
                    }
                }
                else
                {
                    if (diff < 0)
                    {
                        rightLip -= servoRotationSpeed;
                        if (rightLip < f_rightLip) rightLip = f_rightLip;
                    }
                    else
                    {
                        rightLip += servoRotationSpeed;
                        if (rightLip > f_rightLip) rightLip = f_rightLip;
                    }
                    changed = true;
                }

                diff = f_jaw - jaw;
                if (Math.Abs(diff) < servoRotationSpeed)
                {
                    if (f_jaw != jaw)
                    {
                        jaw = f_jaw;
                        changed = true;
                    }
                }
                else
                {
                    if (diff < 0)
                    {
                        jaw -= servoRotationSpeed;
                        if (jaw < f_jaw) jaw = f_jaw;
                    }
                    else
                    {
                        jaw += servoRotationSpeed;
                        if (jaw > f_jaw) jaw = f_jaw;
                    }
                    changed = true;
                }

                diff = f_neckTilt - neckTilt;
                if (Math.Abs(diff) < servoRotationSpeed)
                {
                    if (f_neckTilt != neckTilt)
                    {
                        neckTilt = f_neckTilt;
                        changed = true;
                    }
                }
                else
                {
                    if (diff < 0)
                    {
                        neckTilt -= servoRotationSpeed;
                        if (neckTilt < f_neckTilt) neckTilt = f_neckTilt;
                    }
                    else
                    {
                        neckTilt += servoRotationSpeed;
                        if (neckTilt > f_neckTilt) neckTilt = f_neckTilt;
                    }
                    changed = true;
                }

                diff = f_neckTwist - neckTwist;
                if (Math.Abs(diff) < servoRotationSpeed)
                {
                    if (f_neckTwist != neckTwist)
                    {
                        neckTwist = f_neckTwist;
                        changed = true;
                    }
                }
                else
                {
                    if (diff < 0)
                    {
                        neckTwist -= servoRotationSpeed;
                        if (neckTwist < f_neckTwist) neckTwist = f_neckTwist;
                    }
                    else
                    {
                        neckTwist += servoRotationSpeed;
                        if (neckTwist > f_neckTwist) neckTwist = f_neckTwist;
                    }
                    changed = true;
                }

                if (actionActive > 0)
                {
                    // blink
                    if ((actionMode & 1)!=0)
                    {
                        long cur = Environment.TickCount - blinkStart;
                        if (cur>=150)
                        {
                            actionMode^=1;
                            actionActive--;
                            leftEyelid = f_leftEyelid = 0.0f;
                            rightEyelid = f_rightEyelid = 0.0f;
                            changed = true;
                            if (isRecording)
                            {
                                RobotState rs = new RobotState(true);
                                rs.rightEyelid = rs.leftEyelid = 0;
                                rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                                recordingStates.Add(rs);
                            }
                        }
                    }
                    // left blink
                    if ((actionMode & 2) != 0)
                    {
                        long cur = Environment.TickCount - blinkStart;
                        if (cur >= 150)
                        {
                            actionMode ^= 2;
                            actionActive--;
                            leftEyelid = f_leftEyelid = 0.0f;
                            changed = true;
                            if (isRecording)
                            {
                                RobotState rs = new RobotState(true);
                                rs.leftEyelid = 0;
                                rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                                recordingStates.Add(rs);
                            }
                        }
                    }
                    // right blink
                    if ((actionMode & 4) != 0)
                    {
                        long cur = Environment.TickCount - blinkStart;
                        if (cur >= 150)
                        {
                            actionMode ^= 4;
                            actionActive--;
                            rightEyelid = f_rightEyelid = 0.0f;
                            changed = true;
                            if (isRecording)
                            {
                                RobotState rs = new RobotState(true);
                                rs.rightEyelid = 0;
                                rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                                recordingStates.Add(rs);
                            }
                        }
                    }
                    // neck no
                    if ((actionMode & 8) != 0)
                    {
                        long cur = Environment.TickCount - neckStart;
                        if (cur >= 600)
                        {
                            if (neckTwist != 0.5f)
                            {
                                neckTwist = f_neckTwist = 0.5f;
                                changed = true;
                                if (isRecording)
                                {
                                    RobotState rs = new RobotState(true);
                                    rs.neckTwist = 0.5f;
                                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                                    recordingStates.Add(rs);
                                }
                            }

                            actionMode ^= 8;
                            actionActive--;
                        }
                        else
                        if (cur >= 400)
                        {
                            if (neckTwist != 0.60f)
                            {
                                neckTwist = f_neckTwist = 0.60f;
                                changed = true;
                                if (isRecording)
                                {
                                    RobotState rs = new RobotState(true);
                                    rs.neckTwist = 0.60f;
                                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                                    recordingStates.Add(rs);
                                }
                            }
                        }
                        else
                        if (cur >= 200)
                        {
                            if (neckTwist != 0.40f)
                            {
                                neckTwist = f_neckTwist = 0.40f;
                                changed = true;
                                if (isRecording)
                                {
                                    RobotState rs = new RobotState(true);
                                    rs.neckTwist = 0.40f;
                                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                                    recordingStates.Add(rs);
                                }
                            }
                        }
                    }
                    // neck yes
                    if ((actionMode & 16) != 0)
                    {
                        long cur = Environment.TickCount - neckStart;
                        if (cur >= 600)
                        {
                            if (neckTilt!=0.5f)
                            {
                                neckTilt = f_neckTilt = 0.5f;
                                changed = true;
                                if (isRecording)
                                {
                                    RobotState rs = new RobotState(true);
                                    rs.neckTilt = 0.5f;
                                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                                    recordingStates.Add(rs);
                                }
                            }

                            actionMode ^= 16;
                            actionActive--;
                        }
                        else
                        if (cur >= 400)
                        {
                            if (neckTilt!=0.0f)
                            {
                                neckTilt = f_neckTilt = 0.0f;
                                changed = true;
                                if (isRecording)
                                {
                                    RobotState rs = new RobotState(true);
                                    rs.neckTilt = rs.leftEyelid = 0.0f;
                                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                                    recordingStates.Add(rs);
                                }
                            }
                        }
                        else
                        if (cur >= 200)
                        {
                            if (neckTilt != 0.5f)
                            {
                                neckTilt = f_neckTilt = 0.5f;
                                changed = true;
                                if (isRecording)
                                {
                                    RobotState rs = new RobotState(true);
                                    rs.neckTilt = 0.5f;
                                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                                    recordingStates.Add(rs);
                                }
                            }
                        }
                    }
                    // eye roll
                    if ((actionMode & 32) != 0)
                    {
                        long cur = Environment.TickCount - rollStart;
                        if (cur >= 1500)
                        {
                            f_leftHorizontalEye = f_rightHorizontalEye = 0.5f;
                            f_leftVerticalEye = f_rightVerticalEye = 0.5f;
                            changed = true;
                            actionMode ^= 32;
                            actionActive--;
                            if (isRecording)
                            {
                                RobotState rs = new RobotState(true);
                                rs.leftHorizontalEye = rs.rightHorizontalEye = 0.5f;
                                rs.leftVerticalEye = rs.rightVerticalEye = 0.5f;
                                rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                                recordingStates.Add(rs);
                            }
                        }
                        else
                        // determine position based on rotation within 1000 ms
                        {
                            f_rightHorizontalEye = rightHorizontalEye = f_leftHorizontalEye = leftHorizontalEye = ((float)Math.Sin((cur * Math.PI * 2) / 1500.0f) / 2.0f) + 0.5f;
                            f_rightVerticalEye = rightVerticalEye = f_leftVerticalEye = leftVerticalEye = 1.0f - (((float)Math.Cos((cur * Math.PI * 2) / 1500.0f) / 2.0f) + 0.5f);
                            changed = true;
                            if (isRecording)
                            {
                                RobotState rs = new RobotState(true);
                                rs.leftHorizontalEye = rs.rightHorizontalEye = leftHorizontalEye;
                                rs.leftVerticalEye = rs.rightVerticalEye = leftVerticalEye;
                                rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                                recordingStates.Add(rs);
                            }
                        }
                    }
                }

                // check for trigger
                if ((!isPlaying)&&(!triggerPlay)&&(!calibrating))
                {
                    if (triggerEnabled)
                    {
                        if (triggerCheckIR)
                        {
                            float dist = robot.GetIRValue();
                            if ((dist < triggerDistance)&&(dist>0))
                            {
                                triggerPlay = true;
                                robot.ResetDistances();
                            }
                        }
                        if (triggerCheckSonar)
                        {
                            float dist = robot.GetSonarValue();
                            if ((dist > triggerDistance)&&(dist>0))
                            {
                                triggerPlay = true;
                                robot.ResetDistances();
                            }
                        }
                    }
                }

                if (changed)
                {
                    RobotState currentState = GetState();
                    robot.SetState(currentState, lastState);
                    simulator.SetState(currentState);
                    lastState = currentState;
                    Thread.Sleep(10);
                }
                else
                    if (actionActive > 0)
                        Thread.Sleep(25);
                    else
                        newData.WaitOne(20);


                if (wasConnected)
                {
                    if (!robot.IsConnected())
                    {
                        wasConnected = false;
                        ConnectionChanged(this, new EventArgs<string>("disconnected"));
                    }
                }
                else
                {
                    if (robot.IsConnected())
                    {
                        wasConnected = true;
                        ConnectionChanged(this, new EventArgs<string>("connected"));
                    }
                }
/*
                else
                {
                    if (!isPlaying)
                        // trigger state change when 
                        if (lastChanged)
                            triggerStateChangeCallback();
                }
 */ 
            }
        }

        public void SetExpression(string name)
        {
            if (name.Equals("Afraid"))
                SetState(0.5f/*leftHorizontalEye*/, 0.7f/*leftVerticalEye*/, 0.5f/*rightHorizontalEye*/, 0.7f/*rightVerticalEye*/, 1.0f-0.1836375f/*leftEyebrow*/, 0.7860668f/*rightEyebrow*/, 0.0625f/*rightEyelid*/, 0.0625f/*leftEyelid*/, 0.1142002f/*leftLip*/, 0.8580002f/*rightLip*/, 0.225f/*jaw*/, 0.5f/*neckTilt*/, -1.0f/*neckTwist*/);
            else
                if (name.Equals("Awkward"))
                    SetState(0.5119048f/*leftHorizontalEye*/, 0.4880952f/*leftVerticalEye*/, 0.5119048f/*rightHorizontalEye*/, 0.4880952f/*rightVerticalEye*/, 1f/*leftEyebrow*/, 1f/*rightEyebrow*/, 0.452381f/*rightEyelid*/, 0.452381f/*leftEyelid*/, 1f/*leftLip*/, 0f/*rightLip*/, 0.675f/*jaw*/, 0.5f/*neckTilt*/, -1.0f/*neckTwist*/);
                else
                    if (name.Equals("Angry"))
                        SetState(0.5f/*leftHorizontalEye*/, 0.5f/*leftVerticalEye*/, 0.5f/*rightHorizontalEye*/, 0.5f/*rightVerticalEye*/, 1.0f - 0.9075357f/*leftEyebrow*/, 0.09246436f/*rightEyebrow*/, 0.4047619f/*rightEyelid*/, 0.4047619f/*leftEyelid*/, 0.2235353f/*leftLip*/, 0.7764647f/*rightLip*/, 0f/*jaw*/, 0.33f/*neckTilt*/, -1.0f/*neckTwist*/);
                    else
                        if (name.Equals("Disappointed"))
                            SetState(0.4761905f/*leftHorizontalEye*/, 0.7857143f/*leftVerticalEye*/, 0.4761905f/*rightHorizontalEye*/, 0.7857143f/*rightVerticalEye*/, 1.0f - 0.5687325f/*leftEyebrow*/, 0.4312675f/*rightEyebrow*/, 0.07142857f/*rightEyelid*/, 0.07142857f/*leftEyelid*/, 0.312833f/*leftLip*/, 0.687167f/*rightLip*/, 0f/*jaw*/, 0.17f/*neckTilt*/, -1.0f/*neckTwist*/);
                        else
                            if (name.Equals("Happy"))
                                SetState(0.625f/*leftHorizontalEye*/, 0.5625f/*leftVerticalEye*/, 0.5f/*rightHorizontalEye*/, 0.5625f/*rightVerticalEye*/, 1.0f - 0.2499876f/*leftEyebrow*/, 0.6599966f/*rightEyebrow*/, 0.4047619f/*rightEyelid*/, 0.4047619f/*leftEyelid*/, 0.75f/*leftLip*/, 0.25f/*rightLip*/, 1f/*jaw*/, 0.5f/*neckTilt*/, -1.0f/*neckTwist*/);
                            else
                                if (name.Equals("Neutral"))
                                    SetState(0.5f/*leftHorizontalEye*/, 0.5f/*leftVerticalEye*/, 0.5f/*rightHorizontalEye*/, 0.5f/*rightVerticalEye*/, 0.5f/*leftEyebrow*/, 0.5f/*rightEyebrow*/, 0.5f/*rightEyelid*/, 0.5f/*leftEyelid*/, 0.5f/*leftLip*/, 0.5f/*rightLip*/, 0.0f/*jaw*/, 0.5f/*neckTilt*/, 0.5f/*neckTwist*/);
                                else
                                    if (name.Equals("Sad"))
                                        SetState(0.5595238f/*leftHorizontalEye*/, 0.6428571f/*leftVerticalEye*/, 0.5595238f/*rightHorizontalEye*/, 0.6428571f/*rightVerticalEye*/, 1.0f - 0.09873044f/*leftEyebrow*/, 0.9012696f/*rightEyebrow*/, 0.5714286f/*rightEyelid*/, 0.5714286f/*leftEyelid*/, 0f/*leftLip*/, 0.9494985f/*rightLip*/, 0.25f/*jaw*/, 0.5f/*neckTilt*/, -1.0f/*neckTwist*/);
                                    else
                                        if (name.Equals("Sinister"))
                                            SetState(0.5f/*leftHorizontalEye*/, 0.4404762f/*leftVerticalEye*/, 0.5f/*rightHorizontalEye*/, 0.4404762f/*rightVerticalEye*/, 1.0f - 0.9320638f/*leftEyebrow*/, 0.06793616f/*rightEyebrow*/, 0.0625f/*rightEyelid*/, 0.0625f/*leftEyelid*/, 0.7014102f/*leftLip*/, 0.2985898f/*rightLip*/, 0.675f/*jaw*/, 0.5f/*neckTilt*/, -1.0f/*neckTwist*/);
                                        else
                                            if (name.Equals("Sleepy"))
                                                SetState(0.5f/*leftHorizontalEye*/, 0.5595238f/*leftVerticalEye*/, 0.5119048f/*rightHorizontalEye*/, 0.5952381f/*rightVerticalEye*/, 1.0f - 0.4f/*leftEyebrow*/, 0.6f/*rightEyebrow*/, 0.8f/*rightEyelid*/, 0.8f/*leftEyelid*/, 0.25f/*leftLip*/, 0.75f/*rightLip*/, 0.2f/*jaw*/, 0.3f/*neckTilt*/, -1.0f/*neckTwist*/);
                                            else
                                                if (name.Equals("Smile"))
                                                    SetState(0.625f/*leftHorizontalEye*/, 0.5625f/*leftVerticalEye*/, 0.5f/*rightHorizontalEye*/, 0.5625f/*rightVerticalEye*/, 1.0f - 0.419993f/*leftEyebrow*/, 0.580007f/*rightEyebrow*/, 0.07142857f/*rightEyelid*/, 0.07142857f/*leftEyelid*/, 1f/*leftLip*/, 0f/*rightLip*/, 0f/*jaw*/, 0.5f/*neckTilt*/, -1.0f/*neckTwist*/);
                                                else
                                                    if (name.Equals("Sneaky"))
                                                        SetState(0.6190476f/*leftHorizontalEye*/, 0.297619f/*leftVerticalEye*/, 0.6190476f/*rightHorizontalEye*/, 0.297619f/*rightVerticalEye*/, 1.0f - 0.4217965f/*leftEyebrow*/, 0.5137788f/*rightEyebrow*/, 0.2619048f/*rightEyelid*/, 0.2619048f/*leftEyelid*/, 0f/*leftLip*/, 0f/*rightLip*/, 0.925f/*jaw*/, 0.5f/*neckTilt*/, -1.0f/*neckTwist*/);
                                                    else
                                                        if (name.Equals("Sulk"))
                                                            SetState(0.3690476f/*leftHorizontalEye*/, 0.6071429f/*leftVerticalEye*/, 0.3690476f/*rightHorizontalEye*/, 0.6071429f/*rightVerticalEye*/, 1.0f - 0.6799673f/*leftEyebrow*/, 0.3200327f/*rightEyebrow*/, 0.7619048f/*rightEyelid*/, 0.7619048f/*leftEyelid*/, 0.1740483f/*leftLip*/, 0.3668751f/*rightLip*/, 0f/*jaw*/, 0.5f/*neckTilt*/, -1.0f/*neckTwist*/);
                                                        else
                                                            if (name.Equals("Surprised"))
                                                                SetState(0.5f/*leftHorizontalEye*/, 0.4761905f/*leftVerticalEye*/, 0.5f/*rightHorizontalEye*/, 0.4761905f/*rightVerticalEye*/, 1f/*leftEyebrow*/, 1f/*rightEyebrow*/, 0.1428571f/*rightEyelid*/, 0.1428571f/*leftEyelid*/, 0.25f/*leftLip*/, 0.7147767f/*rightLip*/, 1f/*jaw*/, 0.5f/*neckTilt*/, -1.0f/*neckTwist*/);
                                                            else
                                                                if (name.Equals("Yelling"))
                                                                    SetState(0.5f/*leftHorizontalEye*/, 0.5f/*leftVerticalEye*/, 0.5f/*rightHorizontalEye*/, 0.5f/*rightVerticalEye*/, 1.0f - 0.9075357f/*leftEyebrow*/, 0.09246436f/*rightEyebrow*/, 0.4047619f/*rightEyelid*/, 0.4047619f/*leftEyelid*/, 0.2235353f/*leftLip*/, 0.7764647f/*rightLip*/, 1f/*jaw*/, 0.33f/*neckTilt*/, -1.0f/*neckTwist*/);
                                                                else
                                                                    if (name.Equals("Worried"))
                                                                        SetState(0.5119048f/*leftHorizontalEye*/, 0.3690476f/*leftVerticalEye*/, 0.5119048f/*rightHorizontalEye*/, 0.3690476f/*rightVerticalEye*/, 1.0f - 0.1930289f/*leftEyebrow*/, 0.8069711f/*rightEyebrow*/, 0.07142857f/*rightEyelid*/, 0.07142857f/*leftEyelid*/, 0.1828069f/*leftLip*/, 0.8171931f/*rightLip*/, 0.5f/*jaw*/, 0.67f/*neckTilt*/, -1.0f/*neckTwist*/);

            EditState(GetFinalState());
        }

        public void recorder_PositionChangeCallback(object sender, EventArgs e)
        {
            CalculateTriggers(ref robotStates, recorder.getOneSecondRange());
        }

        public void simulator_StateChangeCallback(object sender, EventArgs e)
        {
            RobotState ss = (RobotState)e;

            robot.SetState(ss, lastState);

            EditState(ss);
        }

        public void EditState(RobotState ss)
        {
            //lastState = ss;

            // if we are currently editing an existing frame update 
            int editPoint = recorder.GetSelectedEditPoint();
            if (editPoint >= 0)
            {
                recorder.SetRelativeState(ss, editPoint);

                // update existing configuration
                ss.position = robotStates[editPoint].position;
                robotStates[editPoint] = ss;
                recorder.SetEditPoints(ref robotStates);
                CalculateTriggers(ref robotStates, recorder.getOneSecondRange());
                return;
            }
            else
            {
                ss.position = recorder.getSelectionStart();
                if (ss.position < 0) return;

                long range = recorder.getOneSecondRange() / 10;

                int i;
                for (i = 0; i < robotStates.Count; i++)
                {
                    RobotState t = robotStates[i];

                    if (ss.position < t.position)
                    {
                        //otherwise insert it into the sequence
                        if (i == 0)
                            robotStates.Insert(0, ss);
                        else
                        {
                            recorder.SetRelativeState(ss, i);
                            robotStates.Insert(i, ss);
                        }
                        recorder.SetEditPoints(ref robotStates);
                        CalculateTriggers(ref robotStates, recorder.getOneSecondRange());
                        return;
                    }
                }

                // otherwise sequence might be empty so just add it on the end
                if (i <= robotStates.Count)
                {
                    recorder.SetRelativeState(ss, robotStates.Count);

                    robotStates.Add(ss);
                    recorder.SetEditPoints(ref robotStates);
                    CalculateTriggers(ref robotStates, recorder.getOneSecondRange());
                }
            }
        }

        public void recorder_PlaybackTick(object sender, EventArgs e)
        {
            long currentPosition = recorder.getCurrentPosition();

            // find this position within the sequence
            int currentStateIndex = playbackStateIndex;
            while ((currentStateIndex < robotStates.Count) && (robotStates[currentStateIndex].triggerPosition < currentPosition))
            {
                recorder.UpdateState(playbackState, robotStates[currentStateIndex]);
                currentStateIndex++;
            }
            
            if (currentStateIndex != playbackStateIndex)
            {
                SetState(playbackState);
                playbackStateIndex = currentStateIndex;
            }
        }

        public void Restart()
        {
            isPlaying = true;

            robot.SetState(new RobotState(), lastState);

            playbackState = new RobotState();

            playbackStateIndex = 0;
            // calculate the trigger positions based on movement time from one expression to the next
            simulator.isPlaying = true;
            //simulator.CalculateTriggers(robotStates, recorder.getOneSecondRange());
            Reset();
            //recorder.Invalidate();
            recorder.Play();
        }

        public void Play()
        {
            isPlaying = true;

            robot.SetState(new RobotState(), lastState);

            playbackState = new RobotState();

            playbackStateIndex = 0;
            // calculate the trigger positions based on movement time from one expression to the next
            simulator.isPlaying = true;
            //simulator.CalculateTriggers(robotStates, recorder.getOneSecondRange());
            Reset();
            //recorder.Invalidate();
            recorder.Play();
        }

        public void Stop()
        {
            simulator.isPlaying = false;
            recorder.Stop();
            isPlaying = false;
        }

        private void recorder_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (sender == recorder)
                {
                    if (robotStates.Count > 0)
                    {
                        // determine current simulator state based on position click
                        long currentPosition = recorder.getSelectionEnd();

                        // find this position within the sequence
                        RobotState currentState = new RobotState();
                        int currentStateIndex = 0;
                        while ((currentStateIndex < robotStates.Count) && (robotStates[currentStateIndex].position < currentPosition))
                        {
                            recorder.UpdateState(currentState, robotStates[currentStateIndex]);
                            currentStateIndex++;
                        }
                        if (currentStateIndex < robotStates.Count)
                        {
                            if (recorder.GetSelectedEditPoint() == currentStateIndex)
                                SetState(currentState, robotStates[currentStateIndex], robotStates[currentStateIndex].triggerPosition);
                            else
                                SetState(currentState, robotStates[currentStateIndex], currentPosition);
                        }
                        else
                            SetState(currentState, currentState, currentPosition);
                    }
                }
            }
        }

        public void Close()
        {
            robot.Stop();
            isRunning = false;
        }

        public RobotState CreateStateFromViseme(int id)
        {
            RobotState ss = new RobotState();

            // finish up as to what id translates to in a viseme ... will need final servos
            ss.leftHorizontalEye = -1.0f;
            ss.leftVerticalEye = -1.0f;
            ss.rightHorizontalEye = -1.0f;
            ss.rightVerticalEye = -1.0f;
            ss.leftEyebrow = -1.0f;
            ss.rightEyebrow = -1.0f;
            ss.rightEyelid = -1.0f;
            ss.leftEyelid = -1.0f;
            ss.neckTilt = -1.0f;
            ss.neckTwist = -1.0f;
            ss.position = 0;
            ss.triggerPosition = 0;

            switch (id)
            {
                case 0:
                    ss.leftLip = 0.5f;
                    ss.rightLip = 0.5f;
                    ss.jaw = 0.0f;
                    break;
                case 1:
                    ss.leftLip = 0.0f;
                    ss.rightLip = 1.0f;
                    ss.jaw = 0.95f;
                    break;
                case 2:
                case 3:
                    ss.leftLip = 0.0f;
                    ss.rightLip = 1.0f;
                    ss.jaw = 1.0f;
                    break;
                case 4:
                    ss.leftLip = 0.5f;
                    ss.rightLip = 0.5f;
                    ss.jaw = 1.0f;
                    break;
                case 5:
                case 13:
                    ss.leftLip = 0.0f;
                    ss.rightLip = 1.0f;
                    ss.jaw = 0.75f;
                    break;
                case 6:
                    ss.leftLip = 0.55f;
                    ss.rightLip = 0.45f;
                    ss.jaw = 0.48f;
                    break;
                case 7:
                    ss.leftLip = 0.0f;
                    ss.rightLip = 1.0f;
                    ss.jaw = 0.75f;
                    break;
                case 8:
                    ss.leftLip = 0.0f;
                    ss.rightLip = 1.0f;
                    ss.jaw = 1.0f;
                    break;
                case 9:
                    ss.leftLip = 0.20f;
                    ss.rightLip = 0.75f;
                    ss.jaw = 1.0f;
                    break;
                case 10:
                    ss.leftLip = 0.0f;
                    ss.rightLip = 1.0f;
                    ss.jaw = 0.8f;
                    break;
                case 11:
                    ss.leftLip = 0.24f;
                    ss.rightLip = 0.66f;
                    ss.jaw = 0.8f;
                    break;
                case 12:
                    ss.leftLip = 0.0f;
                    ss.rightLip = 1.0f;
                    ss.jaw = 1.0f;
                    break;
                case 14:
                    ss.leftLip = 0.1f;
                    ss.rightLip = 0.9f;
                    ss.jaw = 0.7f;
                    break;
                case 15:
                    ss.leftLip = 0.1f;
                    ss.rightLip = 0.9f;
                    ss.jaw = 0.4f;
                    break;
                case 16:
                    ss.leftLip = 0.1f;
                    ss.rightLip = 0.9f;
                    ss.jaw = 0.67f;
                    break;
                case 17:
                    ss.leftLip = 0.3f;
                    ss.rightLip = 0.7f;
                    ss.jaw = 0.475f;
                    break;
                case 18:
                    ss.leftLip = 0.16f;
                    ss.rightLip = 0.84f;
                    ss.jaw = 0.23f;
                    break;
                case 19:
                case 20:
                    ss.leftLip = 0.312833f;
                    ss.rightLip = 0.6572264f;
                    ss.jaw = 0.475f;
                    break;
                case 21:
                    ss.leftLip = 0.5f;
                    ss.rightLip = 0.5f;
                    ss.jaw = 0.0f;
                    break;
            }

            return ss;
        }

        /*
            Eyebrows
            Eyelids
            Eyes Horizontal
            Eyes Vertical
            Lips
            Jaw
            Neck Twist
            Neck Tilt
        */ 

        public void Set(string name, float val)
        {
            if (name == null) return;

            if (name.Equals("Eyebrows"))
                SetEyebrows(val);
            else
                if (name.Equals("Eyelids"))
                    SetEyelids(val);
                else
                    if (name.Equals("Eyes Horizontal"))
                        SetEyesHorizontal(val);
                    else
                        if (name.Equals("Eyes Vertical"))
                            SetEyesVertical(val);
                        else
                            if (name.Equals("Kips"))
                                SetLips(val);
                            else
                                if (name.Equals("Jaw"))
                                    SetJaw(val);
                                else
                                    if (name.Equals("Neck Twist"))
                                        SetNeckTwist(val);
                                    else
                                        if (name.Equals("Neck Tilt"))
                                            SetNeckTilt(val);
        }

        /*
            Expression Afraid
            Expression Awkward
            Expression Angry
            Expression Disappointed
            Expression Happy
            Expression Neutral
            Expression Sad
            Expression Sinister
            Expression Sleepy
            Expression Smile
            Expression Sneaky
            Expression Sulk
            Expression Surprised
            Expression Yelling
            Expression Worried
          
            Eyebrows Pinch
            Eyebrows Level
            Eyebrows Spread
            Eyebrow Left Pinch
            Eyebrow Left Spread
            Eyebrow Right Pinch
            Eyebrow Right Spread
         
            Eyes Center
            Eyes Left
            Eyes Right
            Eyes Up
            Eyes Down
            Eyes Roll
            Eyes Pinch
            Eyes Spread
          
            Eyelids Blink
            Eyelid Left Blink
            Eyelid Right Blink
            Eyelids Open
            Eyelids Half
            Eyelids Closed
            
            Lips Smile
            Lips Level
            Lips Frown
            Lips Left Diagonal
            Lips Right Diagonal

            Jaw Open
            Jaw Half
            Jaw Close

            Neck Slight Left 
            Neck Left
            Neck Front
            Neck Slight Right 
            Neck Right
            Neck Slight Up 
            Neck Up
            Neck Front
            Neck Slight Down 
            Neck Down
            Neck Yes
            Neck No
         */
        public void Set(string name, bool val)
        {
            if (name == null) return;

            if (name.StartsWith("Expression "))
            {
                if (val)
                    SetExpression(name.Substring(11));
                else
                    SetExpression("Neutral");
            }
            else
            if (name.Equals("Eyebrows Slight Pinch"))
            {
                if (val)
                    MoveEyebrows(-0.02f);
                else
                    MoveEyebrows(0.02f);
            }
            else
            if (name.Equals("Eyebrows Pinch"))
            {
                if (val)
                    SetEyebrows(0.0f);
                else
                    SetEyebrows(0.5f);
            }
            else
            if (name.Equals("Eyebrows Level"))
            {
                SetEyebrows(0.5f);
            }
            else
            if (name.Equals("Eyebrows Slight Spread"))
            {
                if (val)
                    MoveEyebrows(0.02f);
                else
                    MoveEyebrows(-0.02f);
            }
            else
            if (name.Equals("Eyebrows Spread"))
            {
                if (val)
                    SetEyebrows(1.0f);
                else
                    SetEyebrows(0.5f);
            }
            else
            if (name.Equals("Eyebrow Left Pinch"))
            {
                if (val)
                    SetLeftEyebrow(0.0f);
                else
                    SetLeftEyebrow(0.5f);
            }
            else
            if (name.Equals("Eyebrow Right Pinch"))
            {
                if (val)
                    SetRightEyebrow(0.0f);
                else
                    SetRightEyebrow(0.5f);
            }
            else
            if (name.Equals("Eyebrow Left Spread"))
            {
                if (val)
                    SetLeftEyebrow(1.0f);
                else
                    SetLeftEyebrow(0.5f);
            }
            else
            if (name.Equals("Eyebrow Right Spread"))
            {
                if (val)
                    SetRightEyebrow(1.0f);
                else
                    SetRightEyebrow(0.5f);
            }
            else
            if (name.Equals("Eyes Center"))
            {
                SetEyesHorizontal(0.5f);
                SetEyesVertical(0.5f);
            }
            else
            if (name.Equals("Eyes Slight Left"))
            {
                if (val)
                    MoveEyesHorizontal(-0.02f);
                else
                    MoveEyesHorizontal(0.02f);
            }
            else
            if (name.Equals("Eyes Left"))
            {
                if (val)
                    SetEyesHorizontal(0.0f);
                else
                    SetEyesHorizontal(0.5f);
            }
            else
            if (name.Equals("Eyes Slight Right"))
            {
                if (val)
                    MoveEyesHorizontal(0.02f);
                else
                    MoveEyesHorizontal(-0.02f);
            }
            else
            if (name.Equals("Eyes Right"))
            {
                if (val)
                    SetEyesHorizontal(1.0f);
                else
                    SetEyesHorizontal(0.5f);
            }
            else
            if (name.Equals("Eyes Slight Up"))
            {
                if (val)
                    MoveEyesVertical(-0.02f);
                else
                    MoveEyesVertical(0.02f);
            }
            else
            if (name.Equals("Eyes Up"))
            {
                if (val)
                    SetEyesVertical(0.0f);
                else
                    SetEyesVertical(0.5f);
            }
            else
            if (name.Equals("Eyes Slight Down"))
            {
                if (val)
                    MoveEyesVertical(0.02f);
                else
                    MoveEyesVertical(-0.02f);
            }
            else
            if (name.Equals("Eyes Down"))
            {
                if (val)
                    SetEyesVertical(1.0f);
                else
                    SetEyesVertical(0.5f);
            }
            else
            if (name.Equals("Eyes Roll"))
            {
                if (val)
                {
                    if ((actionMode & 32) == 0)
                    {
                        actionActive++;
                        actionMode |= 32;
                        rollStart = Environment.TickCount;

                        // close eyelids ... wait for action to open them
                        SetEyesVertical(0.0f);
                        newData.Set();
                    }
                }
            }
            else
            if (name.Equals("Eyes Pinch"))
            {
                if (val)
                {
                    SetLeftEyeHorizontal(1.0f);
                    SetRightEyeHorizontal(0.0f);
                }
                else
                {
                    SetLeftEyeHorizontal(0.5f);
                    SetRightEyeHorizontal(0.5f);
                }
            }
            else
            if (name.Equals("Eyes Spread"))
            {
                if (val)
                {
                    SetLeftEyeHorizontal(0.0f);
                    SetRightEyeHorizontal(1.0f);
                }
                else
                {
                    SetLeftEyeHorizontal(0.5f);
                    SetRightEyeHorizontal(0.5f);
                }
            }
            else
            if (name.Equals("Eyelid Left Blink"))
            {
                if (val)
                {
                    if ((actionMode & 2) == 0)
                    {
                        actionActive++;
                        actionMode |= 2;
                        blinkStart = Environment.TickCount;

                        // close eyelids ... wait for action to open them
                        SetLeftEyelid(1.0f);
                        newData.Set();
                    }
                }
            }
            else
            if (name.Equals("Eyelid Right Blink"))
            {
                if (val)
                {
                    if ((actionMode & 4) == 0)
                    {
                        actionActive++;
                        actionMode |= 4;
                        blinkStart = Environment.TickCount;

                        // close eyelids ... wait for action to open them
                        SetRightEyelid(1.0f);
                        newData.Set();
                    }
                }
            }
            else
            if (name.Equals("Eyelids Blink"))
            {
                if (val)
                {
                    if ((actionMode & 1) == 0)
                    {
                        actionActive++;
                        actionMode |= 1;
                        blinkStart = Environment.TickCount;

                        // close eyelids ... wait for action to open them
                        SetEyelids(1.0f);
                        newData.Set();
                    }
                }
            }
            else
            if (name.Equals("Eyelids Open"))
            {
                if (val)
                    SetEyelids(0.0f);
                else
                    SetEyelids(1.0f);
            }
            else
            if (name.Equals("Eyelids Half"))
            {
                SetEyelids(0.5f);
            }
            else
            if (name.Equals("Eyelids Close"))
            {
                if (val)
                    SetEyelids(1.0f);
                else
                    SetEyelids(0.0f);
            }
            else
            if (name.Equals("Lips Smile"))
            {
                if (val)
                    SetLips(0.10f);
                else
                    SetLips(0.5f);
            }
            else
            if (name.Equals("Lips Level"))
            {
                SetLips(0.5f);
            }
            else
            if (name.Equals("Lips Frown"))
            {
                if (val)
                    SetLips(0.90f);
                else
                    SetLips(0.5f);
            }
            else
            if (name.Equals("Lips Left Diagonal"))
            {
                if (val)
                {
                    SetLeftLip(0.90f);
                    SetRightLip(0.10f);
                }
                else
                    SetLips(0.5f);
            }
            else
            if (name.Equals("Lips Right Diagonal"))
            {
                if (val)
                {
                    SetLeftLip(0.10f);
                    SetRightLip(0.90f);
                }
                else
                    SetLips(0.5f);
            }
            else
            if (name.Equals("Jaw Slight Open"))
            {
                if (val)
                    MoveJaw(0.02f);
                else
                    MoveJaw(-0.02f);
            }
            else
            if (name.Equals("Jaw Open"))
            {
                if (val)
                    SetJaw(1.0f);
                else
                    SetJaw(0.0f);
            }
            else
            if (name.Equals("Jaw Half"))
            {
                if (val)
                    SetJaw(0.5f);
                else
                    SetJaw(0.0f);
            }
            else
            if (name.Equals("Jaw Slight Close"))
            {
                if (val)
                    MoveJaw(-0.02f);
                else
                    MoveJaw(0.02f);
            }
            else
            if (name.Equals("Jaw Close"))
            {
                if (val)
                    SetJaw(0.0f);
                else
                    SetJaw(1.0f);
            }
            else
            if (name.Equals("Neck Slight Left"))
            {
                if (val)
                    MoveNeckTwist(-0.02f);
                else
                    MoveNeckTwist(0.02f);
            }
            else
            if (name.Equals("Neck Left"))
            {
                if (val)
                    SetNeckTwist(0.0f);
                else
                    SetNeckTwist(0.5f);
            }
            else
            if (name.Equals("Neck Front"))
            {
                 SetNeckTwist(0.5f);
            }
            else
            if (name.Equals("Neck Slight Right"))
            {
                if (val)
                    MoveNeckTwist(0.02f);
                else
                    MoveNeckTwist(-0.02f);
            }
            else
            if (name.Equals("Neck Right"))
            {
                if (val)
                    SetNeckTwist(1.0f);
                else
                    SetNeckTwist(0.5f);
            }
            else
            if (name.Equals("Neck Center"))
            {
                SetNeckTilt(0.5f);
            }
            else
            if (name.Equals("Neck Slight Up"))
            {
                if (val)
                    MoveNeckTilt(0.02f);
                else
                    MoveNeckTilt(-0.02f);
            }
            else
            if (name.Equals("Neck Up"))
            {
                if (val)
                    SetNeckTilt(1.0f);
                else
                    SetNeckTilt(0.5f);
            }
            else
            if (name.Equals("Neck Slight Down"))
            {
                if (val)
                    MoveNeckTilt(-0.02f);
                else
                    MoveNeckTilt(0.02f);
            }
            else
            if (name.Equals("Neck Down"))
            {
                if (val)
                    SetNeckTilt(0.0f);
                else
                    SetNeckTilt(0.5f);
            }
            else
            if (name.Equals("Neck No"))
            {
                if (val)
                {
                    if ((actionMode & 8) == 0)
                    {
                        actionActive++;
                        actionMode |= 8;
                        neckStart = Environment.TickCount;

                        // close eyelids ... wait for action to open them
                        SetNeckTwist(0.60f);
                        newData.Set();
                    }
                }
            }
            else
            if (name.Equals("Neck Yes"))
            {
                if (val)
                {
                    if ((actionMode & 16) == 0)
                    {
                        actionActive++;
                        actionMode |= 16;
                        neckStart = Environment.TickCount;

                        // close eyelids ... wait for action to open them
                        SetNeckTilt(0.0f);
                        newData.Set();
                    }
                }
            }
            else
            if (name.StartsWith("Say "))
            {
                if (val)
                    new Speak(this, name.Substring(0, name.Length-1).Substring(5), speakVoiceIndex);
            }
        }

        public float GetIRValue(int pin)
        {
            return robot.GetIRValue(pin);
        }

        public float GetSonarValue(int triggerPin, int echoPin)
        {
            return robot.GetSonarValue(triggerPin, echoPin);
        }

        public void SetDirectJaw(int pin, int val, int min, int max, bool isCenter)
        {
            if (paused) return;
            if (isCenter)
                robot.SetServoCenter((int)pin, max, min, val);
            else
                robot.SetServo((int)pin, val);
            float ratio = 1.0f - ((float)(val - min) / (max - min));
            simulator.SetJaw(ratio);
            lastState.jaw = ratio;
        }

        public void SetJaw(float val)
        {
            if (Math.Abs(val - lastState.jaw) > moveThreshold)
            {
                f_jaw = jaw = lastState.jaw = val;
                robot.SetJaw(val);
                simulator.SetJaw(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.jaw = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetDirectNeckTwist(int pin, int val, int min, int max, bool isCenter)
        {
            if (paused) return;
            if (isCenter)
                robot.SetServoCenter((int)pin, max, min, val);
            else
                robot.SetServo((int)pin, val);
            float ratio = ((float)(val - min) / (max - min));
            simulator.SetNeckTwist(ratio);
            lastState.neckTwist = ratio;
        }

        public void SetNeckTwist(float val)
        {
            if (Math.Abs(val - lastState.neckTwist) > moveThreshold)
            {
                f_neckTwist = neckTwist = lastState.neckTwist = val;
                robot.SetNeckTwist(val);
                simulator.SetNeckTwist(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.neckTwist = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetDirectNeckTilt(int pin, int val, int min, int max, bool isCenter)
        {
            if (paused) return;
            if (isCenter)
                robot.SetServoCenter((int)pin, max, min, val);
            else
                robot.SetServo((int)pin, val);
            float ratio = 1.0f - ((float)(val - min) / (max - min));
            simulator.SetNeckTilt(ratio);
            lastState.neckTilt = ratio;
        }

        public void SetNeckTilt(float val)
        {
            if (Math.Abs(val - lastState.neckTilt) > moveThreshold)
            {
                f_neckTilt = neckTilt = lastState.neckTilt = val;
                robot.SetNeckTilt(val);
                simulator.SetNeckTilt(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.neckTilt = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetDirectLeftEyelid(int pin, int val, int min, int max, bool isCenter)
        {
            if (paused) return;
            if (isCenter)
                robot.SetServoCenter((int)pin, max, min, val);
            else
                robot.SetServo((int)pin, val);
            float ratio = 1.0f - ((float)(val - min) / (max - min));
            simulator.SetLeftEyelid(ratio);
            lastState.leftEyelid = ratio;
        }

        public void SetDirectRightEyelid(int pin, int val, int min, int max, bool isCenter)
        {
            if (paused) return;
            if (isCenter)
                robot.SetServoCenter((int)pin, max, min, val);
            else
                robot.SetServo((int)pin, val);
            float ratio = 1.0f - ((float)(val - min) / (max - min));
            simulator.SetRightEyelid(ratio);
            lastState.rightEyelid = ratio;
        }

        public void SetLeftEyelid(float val)
        {
            if (Math.Abs(val - lastState.leftEyelid) > moveThreshold)
            {
                f_leftEyelid = leftEyelid = lastState.leftEyelid = val;
                robot.SetLeftEyelid(val);
                simulator.SetLeftEyelid(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.leftEyelid = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetRightEyelid(float val)
        {
            if (Math.Abs(val - lastState.rightEyelid) > moveThreshold)
            {
                f_rightEyelid = rightEyelid = lastState.rightEyelid = val;
                robot.SetRightEyelid(val);
                simulator.SetRightEyelid(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.rightEyelid = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetEyelids(float val)
        {
            if ((Math.Abs(val - lastState.rightEyelid) > moveThreshold) || (Math.Abs(val - lastState.leftEyelid) > moveThreshold))
            {
                f_leftEyelid = leftEyelid = lastState.leftEyelid = f_rightEyelid = rightEyelid = lastState.rightEyelid = val;
                robot.SetEyelids(val);
                simulator.SetEyelids(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.rightEyelid = rs.leftEyelid = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetDirectLeftEyebrow(int pin, int val, int min, int max, bool isCenter)
        {
            if (paused) return;
            if (isCenter)
                robot.SetServoCenter((int)pin, max, min, val);
            else
                robot.SetServo((int)pin, val);
            float ratio = 1.0f - ((float)(val - min) / (max - min));
            simulator.SetLeftEyebrow(ratio);
            lastState.leftEyebrow = ratio;
        }

        public void SetDirectRightEyebrow(int pin, int val, int min, int max, bool isCenter)
        {
            if (paused) return;
            if (isCenter)
                robot.SetServoCenter((int)pin, max, min, val);
            else
                robot.SetServo((int)pin, val);
            float ratio = 1.0f - ((float)(val - min) / (max - min));
            simulator.SetRightEyebrow(ratio);
            lastState.rightEyebrow = ratio;
        }

        public void SetLeftEyebrow(float val)
        {
            if (Math.Abs(val - lastState.leftEyebrow) > moveThreshold)
            {
                f_leftEyebrow = leftEyebrow = lastState.leftEyebrow = val;
                robot.SetLeftEyebrow(val);
                simulator.SetLeftEyebrow(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.leftEyebrow = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetRightEyebrow(float val)
        {
            if (Math.Abs(val - lastState.rightEyebrow) > moveThreshold)
            {
                f_rightEyebrow = rightEyebrow = lastState.rightEyebrow = val;
                robot.SetRightEyebrow(val);
                simulator.SetRightEyebrow(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.rightEyebrow = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }
        
        public void SetEyebrows(float val)
        {
            if ((Math.Abs(val - lastState.rightEyebrow) > moveThreshold) || (Math.Abs(val - lastState.leftEyebrow) > moveThreshold))
            {
                f_rightEyebrow = rightEyebrow = f_leftEyebrow = leftEyebrow = lastState.rightEyebrow = lastState.leftEyebrow = val;
                robot.SetEyebrows(val);
                simulator.SetEyebrows(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.leftEyebrow = rs.rightEyebrow = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetDirectLeftHorizontalEye(int pin, int val, int min, int max, bool isCenter)
        {
            if (paused) return;
            if (isCenter)
                robot.SetServoCenter((int)pin, max, min, val);
            else
                robot.SetServo((int)pin, val);
            float ratio = ((float)(val - min) / (max - min));
            simulator.SetLeftEye(ratio, -1);
            lastState.leftHorizontalEye = ratio;
        }

        public void SetDirectRightHorizontalEye(int pin, int val, int min, int max, bool isCenter)
        {
            if (paused) return;
            if (isCenter)
                robot.SetServoCenter((int)pin, max, min, val);
            else
                robot.SetServo((int)pin, val);
            float ratio = ((float)(val - min) / (max - min));
            simulator.SetRightEye(ratio, -1);
            lastState.rightHorizontalEye = ratio;
        }

        public void SetLeftEyeHorizontal(float val)
        {
            if (Math.Abs(val - lastState.leftHorizontalEye) > moveThreshold)
            {
                f_leftHorizontalEye = leftHorizontalEye = lastState.leftHorizontalEye = val;
                robot.SetLeftEyeHorizontal(val);
                simulator.SetLeftEyeHorizontal(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.leftHorizontalEye = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetRightEyeHorizontal(float val)
        {
            if (Math.Abs(val - lastState.rightHorizontalEye) > moveThreshold)
            {
                f_rightHorizontalEye = rightHorizontalEye = lastState.rightHorizontalEye = val;
                robot.SetRightEyeHorizontal(val);
                simulator.SetRightEyeHorizontal(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.rightHorizontalEye = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetEyesHorizontal(float val)
        {
            if ((Math.Abs(val - lastState.leftHorizontalEye) > moveThreshold) || (Math.Abs(val - lastState.rightHorizontalEye) > moveThreshold))
            {
                f_leftHorizontalEye = leftHorizontalEye = f_rightHorizontalEye = rightHorizontalEye = lastState.leftHorizontalEye = lastState.rightHorizontalEye = val;
                robot.SetEyesHorizontal(val);
                simulator.SetEyesHorizontal(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.leftHorizontalEye = rs.rightHorizontalEye = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetDirectLeftVerticalEye(int pin, int val, int min, int max, bool isCenter)
        {
            if (paused) return;
            if (isCenter)
                robot.SetServoCenter((int)pin, max, min, val);
            else
                robot.SetServo((int)pin, val);
            float ratio = 1.0f - ((float)(val - min) / (max - min));
            simulator.SetLeftEye(-1, ratio);
            lastState.leftVerticalEye = ratio;
        }

        public void SetDirectRightVerticalEye(int pin, int val, int min, int max, bool isCenter)
        {
            if (paused) return;
            if (isCenter)
                robot.SetServoCenter((int)pin, max, min, val);
            else
                robot.SetServo((int)pin, val);
            float ratio = ((float)(val - min) / (max - min));
            simulator.SetRightEye(-1, ratio);
            lastState.rightVerticalEye = ratio;
        }

        public void SetEyesVertical(float val)
        {
            if ((Math.Abs(val - lastState.leftVerticalEye) > moveThreshold) || (Math.Abs(val - lastState.rightVerticalEye) > moveThreshold))
            {
                f_leftVerticalEye = leftVerticalEye = f_rightVerticalEye = rightVerticalEye = lastState.leftVerticalEye = lastState.rightVerticalEye = val;
                robot.SetEyesVertical(val);
                simulator.SetEyesVertical(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.leftVerticalEye = rs.rightVerticalEye = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetDirectLeftLip(int pin, int val, int min, int max, bool isCenter)
        {
            if (paused) return;
            if (isCenter)
                robot.SetServoCenter((int)pin, max, min, val);
            else
                robot.SetServo((int)pin, val);
            float ratio = ((float)(val - min) / (max - min));
            simulator.SetLeftLip(ratio);
            lastState.leftLip = ratio;
        }

        public void SetDirectRightLip(int pin, int val, int min, int max, bool isCenter)
        {
            if (paused) return;
            if (isCenter)
                robot.SetServoCenter((int)pin, max, min, val);
            else
                robot.SetServo((int)pin, val);
            float ratio = 1.0f - ((float)(val - min) / (max - min));
            simulator.SetRightLip(ratio);
            lastState.rightLip = ratio;
        }

        public void SetLips(float val)
        {
            if ((Math.Abs(val - lastState.leftLip) > moveThreshold) || (Math.Abs(val - lastState.rightLip) > moveThreshold))
            {
                f_leftLip = leftLip = f_rightLip = lastState.leftLip = lastState.rightLip = val;
                robot.SetLips(val);
                simulator.SetLips(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.rightLip = rs.leftLip = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetLeftLip(float val)
        {
            if (Math.Abs(val - lastState.leftLip) > moveThreshold)
            {
                f_leftLip = leftLip = lastState.leftLip = val;
                robot.SetLeftLip(val);
                simulator.SetLeftLip(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.leftLip = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        public void SetRightLip(float val)
        {
            if (Math.Abs(val - lastState.rightLip) > moveThreshold)
            {
                f_rightLip = rightLip = lastState.rightLip = val;
                robot.SetRightLip(val);
                simulator.SetRightLip(val);
                if (isRecording)
                {
                    RobotState rs = new RobotState(true);
                    rs.rightLip = val;
                    rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                    recordingStates.Add(rs);
                }
            }
        }

        float MinMax(float d)
        {
            if (d < 0.0f) return 0.0f;
            if (d > 1.0f) return 1.0f;
            return d;
        }

        public void MoveEyelids(float val)
        {
            lastState.rightEyelid = MinMax(lastState.rightEyelid + val);
            f_rightEyelid = f_leftEyelid = rightEyelid = leftEyelid = lastState.leftEyelid = lastState.rightEyelid;
            robot.SetEyelids(lastState.leftEyelid);
            simulator.SetEyelids(lastState.leftEyelid);
            if (isRecording)
            {
                RobotState rs = new RobotState(true);
                rs.rightEyelid = rs.leftEyelid = lastState.leftEyelid;
                rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                recordingStates.Add(rs);
            }
        }

        public void MoveEyesHorizontal(float val)
        {
            f_leftHorizontalEye = leftHorizontalEye = f_rightHorizontalEye = rightHorizontalEye = lastState.leftHorizontalEye = lastState.rightHorizontalEye = MinMax(lastState.rightHorizontalEye + val);
            robot.SetEyesHorizontal(lastState.leftHorizontalEye);
            simulator.SetEyesHorizontal(lastState.leftHorizontalEye);
            if (isRecording)
            {
                RobotState rs = new RobotState(true);
                rs.leftHorizontalEye = rs.rightHorizontalEye = lastState.leftHorizontalEye;
                rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                robotStates.Add(rs);
            }
        }

        public void MoveEyesVertical(float val)
        {
            leftVerticalEye = f_leftVerticalEye = rightVerticalEye = f_rightVerticalEye = lastState.leftVerticalEye = lastState.rightVerticalEye = MinMax(lastState.rightVerticalEye + val);
            robot.SetEyesVertical(lastState.leftVerticalEye);
            simulator.SetEyesVertical(lastState.leftVerticalEye);
            if (isRecording)
            {
                RobotState rs = new RobotState(true);
                rs.rightVerticalEye = rs.leftVerticalEye = lastState.leftVerticalEye;
                rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                robotStates.Add(rs);
            }
        }

        public void MoveLips(float val)
        {
            f_leftLip = leftLip = f_rightLip = rightLip = lastState.leftLip = lastState.rightLip = MinMax(lastState.rightLip + val);
            robot.SetLips(lastState.leftLip);
            simulator.SetLips(lastState.leftLip);
            if (isRecording)
            {
                RobotState rs = new RobotState(true);
                rs.leftLip = rs.rightLip = lastState.leftLip;
                rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                robotStates.Add(rs);
            }
        }

        public void MoveJaw(float val)
        {
            f_jaw = jaw = lastState.jaw = MinMax(lastState.jaw + val);
            robot.SetJaw(lastState.jaw);
            simulator.SetJaw(lastState.jaw);
            if (isRecording)
            {
                RobotState rs = new RobotState(true);
                rs.jaw = lastState.jaw;
                rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                robotStates.Add(rs);
            }
        }

        public void MoveNeckTwist(float val)
        {
            f_neckTwist = neckTwist = lastState.neckTwist = MinMax(lastState.neckTwist + val);
            robot.SetNeckTwist(lastState.neckTwist);
            simulator.SetNeckTwist(lastState.neckTwist);
            if (isRecording)
            {
                RobotState rs = new RobotState(true);
                rs.neckTwist = lastState.neckTwist;
                rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                robotStates.Add(rs);
            }
        }

        public void MoveNeckTilt(float val)
        {
            f_neckTilt = neckTilt = lastState.neckTilt = MinMax(lastState.neckTilt + val);
            robot.SetNeckTilt(lastState.neckTilt);
            simulator.SetNeckTilt(lastState.neckTilt);
            if (isRecording)
            {
                RobotState rs = new RobotState(true);
                rs.neckTilt = lastState.neckTilt;
                rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                robotStates.Add(rs);
            }
        }

        public void MoveEyebrows(float val)
        {
            f_leftEyebrow = leftEyebrow = f_rightEyebrow = rightEyebrow = lastState.rightEyebrow = lastState.leftEyebrow = MinMax(lastState.rightEyebrow + val);
            robot.SetEyebrows(lastState.rightEyebrow);
            simulator.SetEyebrows(lastState.rightEyebrow);
            if (isRecording)
            {
                RobotState rs = new RobotState(true);
                rs.leftEyebrow = rs.rightEyebrow = lastState.rightEyebrow;
                rs.position = recorder.getPositionFromMillis((DateTime.Now - recordingStart).Ticks / 10000) + recordingOffset;
                robotStates.Add(rs);
            }
        }

        // Determine transition time from one state to the next so that we can trigger state changes
        // with enough time to meet the target position
        public void CalculateTriggers(ref List<RobotState> states, long averageBytesPerSecond)
        {
            int i;

            if (states.Count <= 0) return;

            RobotState curr = new RobotState();

            for (i = 0; i < states.Count; i++)
            {
                float diff;
                float max = 0;
                RobotState next = states[i];

                if (next.leftHorizontalEye != -1) { if ((diff = Math.Abs(curr.leftHorizontalEye - next.leftHorizontalEye)) > max) max = diff; }
                if (next.leftVerticalEye != -1) { if ((diff = Math.Abs(curr.leftVerticalEye - next.leftVerticalEye)) > max) max = diff; }
                if (next.rightHorizontalEye != -1) { if ((diff = Math.Abs(curr.rightHorizontalEye - next.rightHorizontalEye)) > max) max = diff; }
                if (next.rightVerticalEye != -1) { if ((diff = Math.Abs(curr.rightVerticalEye - next.rightVerticalEye)) > max) max = diff; }
                if (next.leftEyebrow != -1) { if ((diff = Math.Abs(curr.leftEyebrow - next.leftEyebrow)) > max) max = diff; }
                if (next.rightEyebrow != -1) { if ((diff = Math.Abs(curr.rightEyebrow - next.rightEyebrow)) > max) max = diff; }
                if (next.rightEyelid != -1) { if ((diff = Math.Abs(curr.rightEyelid - next.rightEyelid)) > max) max = diff; }
                if (next.leftEyelid != -1) { if ((diff = Math.Abs(curr.leftEyelid - next.leftEyelid)) > max) max = diff; }
                if (next.leftLip != -1) { if ((diff = Math.Abs(curr.leftLip - next.leftLip)) > max) max = diff; }
                if (next.rightLip != -1) { if ((diff = Math.Abs(curr.rightLip - next.rightLip)) > max) max = diff; }
                if (next.jaw != -1) { if ((diff = Math.Abs(curr.jaw - next.jaw)) > max) max = diff; }
                if (next.neckTilt != -1) { if ((diff = Math.Abs(curr.neckTilt - next.neckTilt)) > max) max = diff; }
                if (next.neckTwist != -1) { if ((diff = Math.Abs(curr.neckTwist - next.neckTwist)) > max) max = diff; }

                next.triggerPosition = next.position - (long)(max * (float)servoRotationSpeedSeconds * (float)averageBytesPerSecond);
                recorder.UpdateState(curr, next);
            }
        }

        public CalibrationData GetCalibration()
        {
            return robot.GetCalibration();
        }

        public void SetAsPaused(bool p)
        {
            paused = p;
        }

        public void ReleaseServo(int pin)
        {
            robot.ReleaseServo(pin);
        }

        public void Save(short[] data)
        {
            robot.Save(data);
        }

        public void ExecuteCopy()
        {
            // copy all states within the selection
            long startSelectionPosition = recorder.getSelectionStart();
            long endSelectionPosition = recorder.getSelectionEnd();

            copyStates.Clear();

            int i;
            for (i = 0; (i < robotStates.Count) && (endSelectionPosition > robotStates[i].position); i++)
            {
                if ((robotStates[i].position >= startSelectionPosition) && (robotStates[i].position <= endSelectionPosition))
                {
                    RobotState tmp = new RobotState(robotStates[i]);
                    copyStates.Add(tmp);
                    tmp.position -= startSelectionPosition;
                }
            }

            recorder.Copy();
            recorder.SetEditPoints(ref robotStates);
            CalculateTriggers(ref robotStates, recorder.getOneSecondRange());
        }

        public void ExecuteDelete()
        {
            int editPoint = recorder.GetSelectedEditPoint();
            // if we have a movement frame selected just delete that
            if (editPoint>=0)
            {
                robotStates.RemoveAt(editPoint);
                recorder.ClearSelectedEditPoint();
            }
            else
            // otherwise delete frames and audio    
            {
                // remove all states within the selection
                long startSelectionPosition = recorder.getSelectionStart();
                long endSelectionPosition = recorder.getSelectionEnd();

                int i;
                long adjust = endSelectionPosition - startSelectionPosition;
                for (i = 0; (i < robotStates.Count); i++)
                {
                    if (robotStates[i].position >= startSelectionPosition)
                    {
                        // if a state is within the selection cut it too
                        if (robotStates[i].position <= endSelectionPosition)
                        {
                            robotStates.RemoveAt(i);
                            i--;
                        }
                        else
                        // otherwise shift all states above the cut down
                        {
                            robotStates[i].position -= adjust;
                        }
                    }
                }

                recorder.Delete();
                recorder.SetEditPoints(ref robotStates);
                CalculateTriggers(ref robotStates, recorder.getOneSecondRange());
            }
        }

        public void ExecuteCut()
        {
            // remove all states within the selection
            long startSelectionPosition = recorder.getSelectionStart();
            long endSelectionPosition = recorder.getSelectionEnd();

            copyStates.Clear();

            int i;
            long adjust = endSelectionPosition - startSelectionPosition;
            for (i = 0; (i < robotStates.Count); i++)
            {
                if (robotStates[i].position >= startSelectionPosition) 
                {
                    // if a state is within the selection cut it too
                    if (robotStates[i].position <= endSelectionPosition)
                    {
                        robotStates[i].position -= startSelectionPosition;
                        copyStates.Add(robotStates[i]);
                        robotStates.RemoveAt(i);
                        i--;
                    }
                    else
                    // otherwise shift all states above the cut down
                    {
                        robotStates[i].position -= adjust;
                    }
                }
            }

            recorder.Cut();
            recorder.SetEditPoints(ref robotStates);
            CalculateTriggers(ref robotStates, recorder.getOneSecondRange());
        }

        public void ExecuteRemoveMovement()
        {
            int editPoint = recorder.GetSelectedEditPoint();

            if (editPoint >= 0)
            {
                robotStates.RemoveAt(editPoint);
                recorder.ClearSelectedEditPoint();
            }
            else
            {
                // remove all states within the selection
                long startSelectionPosition = recorder.getSelectionStart();
                long endSelectionPosition = recorder.getSelectionEnd();

                int i;
                for (i = 0; (i < robotStates.Count) && (endSelectionPosition > robotStates[i].position); i++)
                {
                    if ((robotStates[i].position >= startSelectionPosition) && (robotStates[i].position <= endSelectionPosition))
                    {
                        robotStates.RemoveAt(i);
                        i--;
                    }
                }
                recorder.ClearSelectedEditPoint();
            }

            recorder.SetEditPoints(ref robotStates);
            CalculateTriggers(ref robotStates, recorder.getOneSecondRange());
        }


        public void ExecutePaste()
        {
            // insert all copied states
            long startSelectionPosition = recorder.getSelectionStart();
            long endSelectionPosition = recorder.getSelectionEnd();
            int offset = 0;
            if (copyStates.Count>0)
                 offset = (int)copyStates[copyStates.Count-1].position;

            int i, j;
            // find the insertion point within the state list
            for (i = 0; (i < robotStates.Count) && (startSelectionPosition >= robotStates[i].position); i++) ;

            // i will be +1 of actual place
            if (i > 0)
            {
                if (robotStates[i - 1].position >= startSelectionPosition) i--;
            }

            // insert in all copied states
            for (j = 0; (j < copyStates.Count); j++)
            {
                robotStates.Insert(i, new RobotState(copyStates[j]));
                // adjust its position in the current stream (position is relative to copy)
                robotStates[i].position += startSelectionPosition;
                i++;
            }

            // bump rest of states to include new insertion
            while (i < robotStates.Count)
            {
                robotStates[i].position += offset;
                i++;
            }

            recorder.Paste();
            recorder.SetEditPoints(ref robotStates);
            CalculateTriggers(ref robotStates, recorder.getOneSecondRange());
        }

        public void InsertAmpMouthMovements()
        {
            recorder.InsertAmpMouthMovements();
            CalculateTriggers(ref robotStates, recorder.getOneSecondRange());
        }

        public void SetVisibleStates(bool showMouth, bool showEyes, bool showNeck)
        {
            recorder.SetVisibleStates(showMouth, showEyes, showNeck);
        }

        public void StartRecording(long offset)
        {
            if (!isPaused)
            {
                recordingStart = DateTime.Now;
                recordingOffset = offset;
                recordingStates.Clear();
            }
            isPaused = false;
            isRecording = true;
        }

        public void StopRecording()
        {
            isRecording = false;
        }

        public void PauseRecording()
        {
            isRecording = false;
            isPaused = true;
        }

        public void Insert(MemoryStream audio, List<RobotState> states)
        {
            recorder.InsertFrames(states, audio.Length);
            recorder.InsertAudio(audio);
            CalculateTriggers(ref robotStates, recorder.getOneSecondRange());
        }

        public List<RobotState> getRecordedFrames()
        {
            return recordingStates;
        }

        public WaveFormat GetWaveFormat()
        {
            return recorder.GetWaveFormat();
        }

        public void SetDistanceTrigger(bool e, bool s, bool i, int d)
        {
            triggerEnabled = e;
            triggerCheckSonar = s;
            triggerCheckIR = i;
            triggerDistance = d;
        }
    }
}
