using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Fritz
{
    public partial class Calibration : Form
    {
        Conductor conductor;

        int[] currentValue = new int[32];
        bool[] currentDirection = new bool[32];
        bool[] currentActive = new bool[32];
        int speed = 10;
        int[] indexToPin = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        int[] pinToIndex = { -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };
        int tickCount = 0;

        public Calibration()
        {
            InitializeComponent();
            
            leftEyebrowPin.SelectedIndex=0;
            leftEyelidPin.SelectedIndex=1;
            leftHorizontalEyePin.SelectedIndex=2;
            leftLipPin.SelectedIndex=3;
            leftVerticalEyePin.SelectedIndex=4;
            rightEyebrowPin.SelectedIndex=5;
            rightEyelidPin.SelectedIndex=6;
            rightHorizontalEyePin.SelectedIndex=7;
            rightLipPin.SelectedIndex=8;
            rightVerticalEyePin.SelectedIndex=9;
            rotateNeckPin.SelectedIndex=10;
            tiltNeckPin.SelectedIndex = 11;
            jawPin.SelectedIndex = 12;
            sonarTriggerPin.SelectedIndex = 13;
            sonarEchoPin.SelectedIndex = 14;
            irPin.SelectedIndex = 15;

            int i;
            for (i = 0; i < 16; i++)
                currentActive[i] = false;
        }

        // animates the values to simulate slower servo movement
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (currentActive[0])
            {
                if (currentDirection[0])
                {
                    currentValue[0] += speed;
                    if (currentValue[0] >= leftEyebrowMaximum.Value)
                    {
                        currentValue[0] = (int)leftEyebrowMaximum.Value;
                        currentDirection[0] = false;
                    }
                }
                else
                {
                    currentValue[0] -= speed;
                    if (currentValue[0] <= leftEyebrowMinimum.Value)
                    {
                        currentValue[0] = (int)leftEyebrowMinimum.Value;
                        currentDirection[0] = true;
                    }
                }
                conductor.SetDirectLeftEyebrow((int)indexToPin[leftEyebrowPin.SelectedIndex], currentValue[0], (int)leftEyebrowMinimum.Value, (int)leftEyebrowMaximum.Value, false);
            }

            if (currentActive[1])
            {
                if (currentDirection[1])
                {
                    currentValue[1] += speed;
                    if (currentValue[1] >= rightEyebrowMaximum.Value)
                    {
                        currentValue[1] = (int)rightEyebrowMaximum.Value;
                        currentDirection[1] = false;
                    }
                }
                else
                {
                    currentValue[1] -= speed;
                    if (currentValue[1] <= rightEyebrowMinimum.Value)
                    {
                        currentValue[1] = (int)rightEyebrowMinimum.Value;
                        currentDirection[1] = true;
                    }
                }
                conductor.SetDirectRightEyebrow((int)indexToPin[rightEyebrowPin.SelectedIndex], currentValue[1], (int)rightEyebrowMinimum.Value, (int)rightEyebrowMaximum.Value, false);
            }

            if (currentActive[2])
            {
                if (currentDirection[2])
                {
                    currentValue[2] += speed;
                    if (currentValue[2] >= leftEyelidMaximum.Value)
                    {
                        currentValue[2] = (int)leftEyelidMaximum.Value;
                        currentDirection[2] = false;
                    }
                }
                else
                {
                    currentValue[2] -= speed;
                    if (currentValue[2] <= leftEyelidMinimum.Value)
                    {
                        currentValue[2] = (int)leftEyelidMinimum.Value;
                        currentDirection[2] = true;
                    }
                }
                conductor.SetDirectLeftEyelid((int)indexToPin[leftEyelidPin.SelectedIndex], currentValue[2], (int)leftEyelidMinimum.Value, (int)leftEyelidMaximum.Value, false);
            }

            if (currentActive[3])
            {
                if (currentDirection[3])
                {
                    currentValue[3] += speed;
                    if (currentValue[3] >= rightEyelidMaximum.Value)
                    {
                        currentValue[3] = (int)rightEyelidMaximum.Value;
                        currentDirection[3] = false;
                    }
                }
                else
                {
                    currentValue[3] -= speed;
                    if (currentValue[3] <= rightEyelidMinimum.Value)
                    {
                        currentValue[3] = (int)rightEyelidMinimum.Value;
                        currentDirection[3] = true;
                    }
                }
                conductor.SetDirectRightEyelid((int)indexToPin[rightEyelidPin.SelectedIndex], currentValue[3], (int)rightEyelidMinimum.Value, (int)rightEyelidMaximum.Value, false);
            }

            if (currentActive[4])
            {
                if (currentDirection[4])
                {
                    currentValue[4] += speed;
                    if (currentValue[4] >= leftHorizontalEyeMaximum.Value)
                    {
                        currentValue[4] = (int)leftHorizontalEyeMaximum.Value;
                        currentDirection[4] = false;
                    }
                }
                else
                {
                    currentValue[4] -= speed;
                    if (currentValue[4] <= leftHorizontalEyeMinimum.Value)
                    {
                        currentValue[4] = (int)leftHorizontalEyeMinimum.Value;
                        currentDirection[4] = true;
                    }
                }
                conductor.SetDirectLeftHorizontalEye((int)indexToPin[leftHorizontalEyePin.SelectedIndex], currentValue[4], (int)leftHorizontalEyeMinimum.Value, (int)leftHorizontalEyeMaximum.Value, false);
            }

            if (currentActive[5])
            {
                if (currentDirection[5])
                {
                    currentValue[5] += speed;
                    if (currentValue[5] >= rightHorizontalEyeMaximum.Value)
                    {
                        currentValue[5] = (int)rightHorizontalEyeMaximum.Value;
                        currentDirection[5] = false;
                    }
                }
                else
                {
                    currentValue[5] -= speed;
                    if (currentValue[5] <= rightHorizontalEyeMinimum.Value)
                    {
                        currentValue[5] = (int)rightHorizontalEyeMinimum.Value;
                        currentDirection[5] = true;
                    }
                }
                conductor.SetDirectRightHorizontalEye((int)indexToPin[rightHorizontalEyePin.SelectedIndex], currentValue[5], (int)rightHorizontalEyeMinimum.Value, (int)rightHorizontalEyeMaximum.Value, false);
            }

            if (currentActive[6])
            {
                if (currentDirection[6])
                {
                    currentValue[6] += speed;
                    if (currentValue[6] >= leftVerticalEyeMaximum.Value)
                    {
                        currentValue[6] = (int)leftVerticalEyeMaximum.Value;
                        currentDirection[6] = false;
                    }
                }
                else
                {
                    currentValue[6] -= speed;
                    if (currentValue[6] <= leftVerticalEyeMinimum.Value)
                    {
                        currentValue[6] = (int)leftVerticalEyeMinimum.Value;
                        currentDirection[6] = true;
                    }
                }
                conductor.SetDirectLeftVerticalEye((int)indexToPin[leftVerticalEyePin.SelectedIndex], currentValue[6], (int)leftVerticalEyeMinimum.Value, (int)leftVerticalEyeMaximum.Value, false);
            }

            if (currentActive[7])
            {
                if (currentDirection[7])
                {
                    currentValue[7] += speed;
                    if (currentValue[7] >= rightVerticalEyeMaximum.Value)
                    {
                        currentValue[7] = (int)rightVerticalEyeMaximum.Value;
                        currentDirection[7] = false;
                    }
                }
                else
                {
                    currentValue[7] -= speed;
                    if (currentValue[7] <= rightVerticalEyeMinimum.Value)
                    {
                        currentValue[7] = (int)rightVerticalEyeMinimum.Value;
                        currentDirection[7] = true;
                    }
                }
                conductor.SetDirectRightVerticalEye((int)indexToPin[rightVerticalEyePin.SelectedIndex], currentValue[7], (int)rightVerticalEyeMinimum.Value, (int)rightVerticalEyeMaximum.Value, false);
            }

            if (currentActive[8])
            {
                if (currentDirection[8])
                {
                    currentValue[8] += speed;
                    if (currentValue[8] >= rotateNeckMaximum.Value)
                    {
                        currentValue[8] = (int)rotateNeckMaximum.Value;
                        currentDirection[8] = false;
                    }
                }
                else
                {
                    currentValue[8] -= speed;
                    if (currentValue[8] <= rotateNeckMinimum.Value)
                    {
                        currentValue[8] = (int)rotateNeckMinimum.Value;
                        currentDirection[8] = true;
                    }
                }
                conductor.SetDirectNeckTwist((int)indexToPin[rotateNeckPin.SelectedIndex], currentValue[8], (int)rotateNeckMinimum.Value, (int)rotateNeckMaximum.Value, false);
            }

            if (currentActive[9])
            {
                if (currentDirection[9])
                {
                    currentValue[9] += speed;
                    if (currentValue[9] >= tiltNeckMaximum.Value)
                    {
                        currentValue[9] = (int)tiltNeckMaximum.Value;
                        currentDirection[9] = false;
                    }
                }
                else
                {
                    currentValue[9] -= speed;
                    if (currentValue[9] <= tiltNeckMinimum.Value)
                    {
                        currentValue[9] = (int)tiltNeckMinimum.Value;
                        currentDirection[9] = true;
                    }
                }
                conductor.SetDirectNeckTilt((int)indexToPin[tiltNeckPin.SelectedIndex], currentValue[9], (int)tiltNeckMinimum.Value, (int)tiltNeckMaximum.Value, false);
            }

            if (currentActive[10])
            {
                if (currentDirection[10])
                {
                    currentValue[10] += speed;
                    if (currentValue[10] >= leftLipMaximum.Value)
                    {
                        currentValue[10] = (int)leftLipMaximum.Value;
                        currentDirection[10] = false;
                    }
                }
                else
                {
                    currentValue[10] -= speed;
                    if (currentValue[10] <= leftLipMinimum.Value)
                    {
                        currentValue[10] = (int)leftLipMinimum.Value;
                        currentDirection[10] = true;
                    }
                }
                conductor.SetDirectLeftLip((int)indexToPin[leftLipPin.SelectedIndex], currentValue[10], (int)leftLipMinimum.Value, (int)leftLipMaximum.Value, false);
            }

            if (currentActive[11])
            {
                if (currentDirection[11])
                {
                    currentValue[11] += speed;
                    if (currentValue[11] >= rightLipMaximum.Value)
                    {
                        currentValue[11] = (int)rightLipMaximum.Value;
                        currentDirection[11] = false;
                    }
                }
                else
                {
                    currentValue[11] -= speed;
                    if (currentValue[11] <= rightLipMinimum.Value)
                    {
                        currentValue[11] = (int)rightLipMinimum.Value;
                        currentDirection[11] = true;
                    }
                }
                conductor.SetDirectRightLip((int)indexToPin[rightLipPin.SelectedIndex], currentValue[11], (int)rightLipMinimum.Value, (int)rightLipMaximum.Value, false);
            }

            if (currentActive[12])
            {
                if (currentDirection[12])
                {
                    currentValue[12] += speed;
                    if (currentValue[12] >= jawMaximum.Value)
                    {
                        currentValue[12] = (int)jawMaximum.Value;
                        currentDirection[12] = false;
                    }
                }
                else
                {
                    currentValue[12] -= speed;
                    if (currentValue[12] <= jawMinimum.Value)
                    {
                        currentValue[12] = (int)jawMinimum.Value;
                        currentDirection[12] = true;
                    }
                }
                conductor.SetDirectJaw((int)indexToPin[jawPin.SelectedIndex], currentValue[12], (int)jawMinimum.Value, (int)jawMaximum.Value, false);
            }

            // currentActive[13] - placeholder for left ear
            // currentActive[14] - placeholder for right ear

            tickCount++;
            if ((tickCount % 50)==0)
            {
                if (enableIR.Checked)
                    irDistance.Text = Convert.ToString(conductor.GetIRValue((int)indexToPin[irPin.SelectedIndex]));

                if (enableSonar.Checked)
                    sonarDistance.Text = Convert.ToString(conductor.GetSonarValue((int)indexToPin[sonarTriggerPin.SelectedIndex], (int)indexToPin[sonarEchoPin.SelectedIndex]));
            }
        }

        public void SetConductor(ref Conductor c)
        {
            conductor = c;
        }

        public CalibrationData GetCalibration()
        {
            CalibrationData cd = new CalibrationData();

            cd.leftEyebrowTrim = (int)leftEyebrowTrim.Value;
            cd.leftEyebrowMax = (int)leftEyebrowMaximum.Value;
            cd.leftEyebrowMin = (int)leftEyebrowMinimum.Value;
            cd.leftEyebrowPin = (int)indexToPin[leftEyebrowPin.SelectedIndex];
            cd.leftEyebrowEnabled = enableLeftEyebrow.Checked;

            cd.rightEyebrowTrim = (int)rightEyebrowTrim.Value;
            cd.rightEyebrowMax = (int)rightEyebrowMaximum.Value;
            cd.rightEyebrowMin = (int)rightEyebrowMinimum.Value;
            cd.rightEyebrowPin = (int)indexToPin[rightEyebrowPin.SelectedIndex];
            cd.rightEyebrowEnabled = enableRightEyebrow.Checked;

            cd.leftLidPositionTrim = (int)leftEyelidTrim.Value;
            cd.leftLidPositionMax = (int)leftEyelidMaximum.Value;
            cd.leftLidPositionMin = (int)leftEyelidMinimum.Value;
            cd.leftLidPositionPin = (int)indexToPin[leftEyelidPin.SelectedIndex];
            cd.leftLidPositionEnabled = enableLeftEyelid.Checked;

            cd.rightLidPositionTrim = (int)rightEyelidTrim.Value;
            cd.rightLidPositionMax = (int)rightEyelidMaximum.Value;
            cd.rightLidPositionMin = (int)rightEyelidMinimum.Value;
            cd.rightLidPositionPin = (int)indexToPin[rightEyelidPin.SelectedIndex];
            cd.rightLidPositionEnabled = enableRightEyelid.Checked;

            cd.leftHorizontalEyeTrim = (int)leftHorizontalEyeTrim.Value;
            cd.leftHorizontalEyeMax = (int)leftHorizontalEyeMaximum.Value;
            cd.leftHorizontalEyeMin = (int)leftHorizontalEyeMinimum.Value;
            cd.leftHorizontalEyePin = (int)indexToPin[leftHorizontalEyePin.SelectedIndex];
            cd.leftHorizontalEyeEnabled = enableLeftHorizontalEye.Checked;

            cd.rightHorizontalEyeTrim = (int)rightHorizontalEyeTrim.Value;
            cd.rightHorizontalEyeMax = (int)rightHorizontalEyeMaximum.Value;
            cd.rightHorizontalEyeMin = (int)rightHorizontalEyeMinimum.Value;
            cd.rightHorizontalEyePin = (int)indexToPin[rightHorizontalEyePin.SelectedIndex];
            cd.rightHorizontalEyeEnabled = enableRightHorizontalEye.Checked;

            cd.leftVerticalEyeTrim = (int)leftVerticalEyeTrim.Value;
            cd.leftVerticalEyeMax = (int)leftVerticalEyeMaximum.Value;
            cd.leftVerticalEyeMin = (int)leftVerticalEyeMinimum.Value;
            cd.leftVerticalEyePin = (int)indexToPin[leftVerticalEyePin.SelectedIndex];
            cd.leftHorizontalEyeEnabled = enableLeftVerticalEye.Checked;

            cd.rightVerticalEyeTrim = (int)rightVerticalEyeTrim.Value;
            cd.rightVerticalEyeMax = (int)rightVerticalEyeMaximum.Value;
            cd.rightVerticalEyeMin = (int)rightVerticalEyeMinimum.Value;
            cd.rightVerticalEyePin = (int)indexToPin[rightVerticalEyePin.SelectedIndex];
            cd.rightVerticalEyeEnabled = enableRightVerticalEye.Checked;

            cd.neckTwistTrim = (int)rotateNeckTrim.Value;// (-(0.5f * ((float)rotateNeckMaximum.Value - (float)rotateNeckMinimum.Value)) - rotateNeckMinimum.Value + 1500 + rotateNeckTrim.Value);
            cd.neckTwistMax = (int)rotateNeckMaximum.Value;
            cd.neckTwistMin = (int)rotateNeckMinimum.Value;
            cd.neckTwistPin = (int)indexToPin[rotateNeckPin.SelectedIndex];
            cd.neckTwistEnabled = enableRotateNeck.Checked;

            cd.neckTiltTrim = (int)tiltNeckTrim.Value;
            cd.neckTiltMax = (int)tiltNeckMaximum.Value;
            cd.neckTiltMin = (int)tiltNeckMinimum.Value;
            cd.neckTiltPin = (int)indexToPin[tiltNeckPin.SelectedIndex];
            cd.neckTiltEnabled = enableTiltNeck.Checked;

            cd.leftLipTrim = (int)leftLipTrim.Value;
            cd.leftLipMax = (int)leftLipMaximum.Value;
            cd.leftLipMin = (int)leftLipMinimum.Value;
            cd.leftLipPin = (int)indexToPin[leftLipPin.SelectedIndex];
            cd.leftLipEnabled = enableLeftLip.Checked;

            cd.rightLipTrim = (int)rightLipTrim.Value;
            cd.rightLipMax = (int)rightLipMaximum.Value;
            cd.rightLipMin = (int)rightLipMinimum.Value;
            cd.rightLipPin = (int)indexToPin[rightLipPin.SelectedIndex];
            cd.rightLipEnabled = enableRightLip.Checked;

            cd.jawTrim = (int)jawTrim.Value;
            cd.jawMax = (int)jawMaximum.Value;
            cd.jawMin = (int)jawMinimum.Value;
            cd.jawPin = (int)indexToPin[jawPin.SelectedIndex];
            cd.jawEnabled = enableJaw.Checked;

            cd.sonarEnabled = enableSonar.Checked;
            cd.sonarTriggerPin = (int)indexToPin[sonarTriggerPin.SelectedIndex];
            cd.sonarEchoPin = (int)indexToPin[sonarEchoPin.SelectedIndex];

            cd.irEnabled = enableIR.Checked;
            cd.irPin = (int)indexToPin[irPin.SelectedIndex];

            return cd;
        }

        public int getIndexFromPin(int pin)
        {
            if (pin < 0) 
                return -1;
            if (pin >= pinToIndex.Length) 
                return -1;
            return pinToIndex[pin];
        }

        public void SetCalibration(CalibrationData cd)
        {
            if ((cd.leftEyebrowPin == 0) && (cd.rightEyebrowPin == 0) && (cd.leftLidPositionPin == 0) &&(cd.rightLidPositionPin==0)) return;

            // ensure that the robot does not move while we update the values ... they will trigger a change update
            conductor.SetAsPaused(true);

            leftEyebrowTrim.Value = cd.leftEyebrowTrim;
            leftEyebrowMaximum.Value = cd.leftEyebrowMax;
            leftEyebrowMinimum.Value = cd.leftEyebrowMin;
            leftEyebrowPin.SelectedIndex = getIndexFromPin(cd.leftEyebrowPin);
            enableLeftEyebrow.Checked = cd.leftEyebrowEnabled;

            rightEyebrowTrim.Value = cd.rightEyebrowTrim;
            rightEyebrowMaximum.Value = cd.rightEyebrowMax;
            rightEyebrowMinimum.Value = cd.rightEyebrowMin;
            rightEyebrowPin.SelectedIndex = getIndexFromPin(cd.rightEyebrowPin);
            enableRightEyebrow.Checked = cd.rightEyebrowEnabled;

            leftEyelidTrim.Value = cd.leftLidPositionTrim;
            leftEyelidMaximum.Value = cd.leftLidPositionMax;
            leftEyelidMinimum.Value = cd.leftLidPositionMin;
            leftEyelidPin.SelectedIndex = getIndexFromPin(cd.leftLidPositionPin);
            enableLeftEyelid.Checked = cd.leftLidPositionEnabled;

            rightEyelidTrim.Value = cd.rightLidPositionTrim;
            rightEyelidMaximum.Value = cd.rightLidPositionMax;
            rightEyelidMinimum.Value = cd.rightLidPositionMin;
            rightEyelidPin.SelectedIndex = getIndexFromPin(cd.rightLidPositionPin);
            enableRightEyelid.Checked = cd.rightLidPositionEnabled;

            leftHorizontalEyeTrim.Value = cd.leftHorizontalEyeTrim;
            leftHorizontalEyeMaximum.Value = cd.leftHorizontalEyeMax;
            leftHorizontalEyeMinimum.Value = cd.leftHorizontalEyeMin;
            leftHorizontalEyePin.SelectedIndex = getIndexFromPin(cd.leftHorizontalEyePin);
            enableLeftHorizontalEye.Checked = cd.leftHorizontalEyeEnabled;

            rightHorizontalEyeTrim.Value = cd.rightHorizontalEyeTrim;
            rightHorizontalEyeMaximum.Value = cd.rightHorizontalEyeMax;
            rightHorizontalEyeMinimum.Value = cd.rightHorizontalEyeMin;
            rightHorizontalEyePin.SelectedIndex = getIndexFromPin(cd.rightHorizontalEyePin);
            enableRightHorizontalEye.Checked = cd.rightHorizontalEyeEnabled;

            leftVerticalEyeTrim.Value = cd.leftVerticalEyeTrim;
            leftVerticalEyeMaximum.Value = cd.leftVerticalEyeMax;
            leftVerticalEyeMinimum.Value = cd.leftVerticalEyeMin;
            leftVerticalEyePin.SelectedIndex = getIndexFromPin(cd.leftVerticalEyePin);
            enableLeftVerticalEye.Checked = cd.leftVerticalEyeEnabled;

            rightVerticalEyeTrim.Value = cd.rightVerticalEyeTrim;
            rightVerticalEyeMaximum.Value = cd.rightVerticalEyeMax;
            rightVerticalEyeMinimum.Value = cd.rightVerticalEyeMin;
            rightVerticalEyePin.SelectedIndex = getIndexFromPin(cd.rightVerticalEyePin);
            enableRightVerticalEye.Checked = cd.rightVerticalEyeEnabled;

            rotateNeckTrim.Value = cd.neckTwistTrim;
            rotateNeckMaximum.Value = cd.neckTwistMax;
            rotateNeckMinimum.Value = cd.neckTwistMin;
            rotateNeckPin.SelectedIndex = getIndexFromPin(cd.neckTwistPin);
            enableRotateNeck.Checked = cd.neckTwistEnabled;

            tiltNeckTrim.Value = cd.neckTiltTrim;
            tiltNeckMaximum.Value = cd.neckTiltMax;
            tiltNeckMinimum.Value = cd.neckTiltMin;
            tiltNeckPin.SelectedIndex = getIndexFromPin(cd.neckTiltPin);
            enableTiltNeck.Checked = cd.neckTiltEnabled;

            leftLipTrim.Value = cd.leftLipTrim;
            leftLipMaximum.Value = cd.leftLipMax;
            leftLipMinimum.Value = cd.leftLipMin;
            leftLipPin.SelectedIndex = getIndexFromPin(cd.leftLipPin);
            enableLeftLip.Checked = cd.leftLipEnabled;

            rightLipTrim.Value = cd.rightLipTrim;
            rightLipMaximum.Value = cd.rightLipMax;
            rightLipMinimum.Value = cd.rightLipMin;
            rightLipPin.SelectedIndex = getIndexFromPin(cd.rightLipPin);
            enableRightLip.Checked = cd.rightLipEnabled;

            jawTrim.Value = cd.jawTrim;
            jawMaximum.Value = cd.jawMax;
            jawMinimum.Value = cd.jawMin;
            jawPin.SelectedIndex = getIndexFromPin(cd.jawPin);
            enableJaw.Checked = cd.jawEnabled;

            enableSonar.Checked = cd.sonarEnabled;
            sonarTriggerPin.SelectedIndex = getIndexFromPin(cd.sonarTriggerPin);
            sonarEchoPin.SelectedIndex = getIndexFromPin(cd.sonarEchoPin);

            enableIR.Checked = cd.irEnabled;
            irPin.SelectedIndex = getIndexFromPin(cd.irPin);

            enableLeftEyebrow_Click(null, null);
            enableRightEyebrow_Click(null, null);
            enableLeftEyelid_Click(null, null);
            enableRightEyelid_Click(null, null);
            enableLeftHorizontalEye_Click(null, null);
            enableRightHorizontalEye_Click(null, null);
            enableLeftVerticalEye_Click(null, null);
            enableRightVerticalEye_Click(null, null);
            enableRotateNeck_Click(null, null);
            enableTiltNeck_Click(null, null);
            enableLeftLip_Click(null, null);
            enableRightLip_Click(null, null);
            enableJaw_Click(null, null);
            enableSonar_Click(null, null);
            enableIR_Click(null, null);

            conductor.SetAsPaused(false);
        }

        private void testLeftEyebrow_Click(object sender, EventArgs e)
        {
            bool mode;

            if (currentActive[0])
            {
                testLeftEyebrow.Text = "Test";
                mode = false;
            }
            else
            {
                testLeftEyebrow.Text = "Stop";
                mode = true;
            }
            currentActive[0] = mode;

            leftEyebrowPin.Enabled = !mode;
            leftEyebrowTrim.Enabled = !mode;
            leftEyebrowMaximum.Enabled = !mode;
            leftEyebrowMinimum.Enabled = !mode;
        }

        private void leftEyebrowTrim_ValueChanged(object sender, EventArgs e)
        {
            currentValue[0] = (int)leftEyebrowTrim.Value;
            conductor.SetDirectLeftEyebrow((int)indexToPin[leftEyebrowPin.SelectedIndex], currentValue[0], (int)leftEyebrowMinimum.Value, (int)leftEyebrowMaximum.Value, true);
        }

        private void leftEyebrowMinimum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[0] = (int)leftEyebrowMinimum.Value;
            if (leftEyebrowMinimum.Value > leftEyebrowMaximum.Value)
                leftEyebrowMaximum.Value = leftEyebrowMinimum.Value;

            conductor.SetDirectLeftEyebrow((int)indexToPin[leftEyebrowPin.SelectedIndex], currentValue[0], (int)leftEyebrowMinimum.Value, (int)leftEyebrowMaximum.Value, false);
        }

        private void leftEyebrowMaximum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[0] = (int)leftEyebrowMaximum.Value;
            if (leftEyebrowMaximum.Value < leftEyebrowMinimum.Value)
                leftEyebrowMinimum.Value = leftEyebrowMaximum.Value;
            conductor.SetDirectLeftEyebrow((int)indexToPin[leftEyebrowPin.SelectedIndex], currentValue[0], (int)leftEyebrowMinimum.Value, (int)leftEyebrowMaximum.Value, false);
        }

        private void leftEyebrowTrim_KeyUp(object sender, KeyEventArgs e)
        {
            leftEyebrowTrim_ValueChanged(sender, e);
        }

        private void leftEyebrowMinimum_KeyUp(object sender, KeyEventArgs e)
        {
            leftEyebrowMinimum_ValueChanged(sender, e);
        }

        private void leftEyebrowMaximum_KeyUp(object sender, KeyEventArgs e)
        {
            leftEyebrowMaximum_ValueChanged(sender, e);
        }

        private void testRightEyebrow_Click(object sender, EventArgs e)
        {
            bool mode;

            if (currentActive[1])
            {
                testRightEyebrow.Text = "Test";
                mode = false;
            }
            else
            {
                testRightEyebrow.Text = "Stop";
                mode = true;
            }
            currentActive[1] = mode;

            rightEyebrowPin.Enabled = !mode;
            rightEyebrowTrim.Enabled = !mode;
            rightEyebrowMaximum.Enabled = !mode;
            rightEyebrowMinimum.Enabled = !mode;
        }

        private void rightEyebrowTrim_ValueChanged(object sender, EventArgs e)
        {
            currentValue[1] = (int)rightEyebrowTrim.Value;
            conductor.SetDirectRightEyebrow((int)indexToPin[rightEyebrowPin.SelectedIndex], currentValue[1], (int)rightEyebrowMinimum.Value, (int)rightEyebrowMaximum.Value, true);
        }

        private void rightEyebrowTrim_KeyUp(object sender, KeyEventArgs e)
        {
            rightEyebrowTrim_ValueChanged(sender, e);
        }

        private void rightEyebrowMinimum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[1] = (int)rightEyebrowMinimum.Value;
            if (rightEyebrowMinimum.Value > rightEyebrowMaximum.Value)
                rightEyebrowMaximum.Value = rightEyebrowMinimum.Value;
            conductor.SetDirectRightEyebrow((int)indexToPin[rightEyebrowPin.SelectedIndex], currentValue[1], (int)rightEyebrowMinimum.Value, (int)rightEyebrowMaximum.Value, false);
        }

        private void rightEyebrowMinimum_KeyUp(object sender, KeyEventArgs e)
        {
            rightEyebrowMinimum_ValueChanged(sender, e);
        }

        private void rightEyebrowMaximum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[1] = (int)rightEyebrowMaximum.Value;
            if (rightEyebrowMaximum.Value < rightEyebrowMinimum.Value)
                rightEyebrowMinimum.Value = rightEyebrowMaximum.Value;
            conductor.SetDirectRightEyebrow((int)indexToPin[rightEyebrowPin.SelectedIndex], currentValue[1], (int)rightEyebrowMinimum.Value, (int)rightEyebrowMaximum.Value, false);
        }

        private void rightEyebrowMaximum_KeyUp(object sender, KeyEventArgs e)
        {
            rightEyebrowMaximum_ValueChanged(sender, e);
        }

        private short []serialize()
        {
            short[] buffer = new short[85];

            buffer[0] = (short)leftEyebrowTrim.Value;
            buffer[1] = (short)(leftEyebrowMaximum.Value);
            buffer[2] = (short)(leftEyebrowMinimum.Value);
            buffer[3] = (short)indexToPin[leftEyebrowPin.SelectedIndex];
            buffer[4] = (short)(enableLeftEyebrow.Checked?1:0);

            buffer[5] = (short)rightEyebrowTrim.Value;
            buffer[6] = (short)(rightEyebrowMaximum.Value);
            buffer[7] = (short)(rightEyebrowMinimum.Value);
            buffer[8] = (short)indexToPin[rightEyebrowPin.SelectedIndex];
            buffer[9] = (short)(enableRightEyebrow.Checked?1:0);

            buffer[10] = (short)leftEyelidTrim.Value;
            buffer[11] = (short)(leftEyelidMaximum.Value);
            buffer[12] = (short)(leftEyelidMinimum.Value);
            buffer[13] = (short)indexToPin[leftEyelidPin.SelectedIndex];
            buffer[14] = (short)(enableLeftEyelid.Checked?1:0);

            buffer[15] = (short)rightEyelidTrim.Value;
            buffer[16] = (short)(rightEyelidMaximum.Value);
            buffer[17] = (short)(rightEyelidMinimum.Value);
            buffer[18] = (short)indexToPin[rightEyelidPin.SelectedIndex];
            buffer[19] = (short)(enableRightEyelid.Checked?1:0);

            buffer[20] = (short)leftHorizontalEyeTrim.Value;
            buffer[21] = (short)(leftHorizontalEyeMaximum.Value);
            buffer[22] = (short)(leftHorizontalEyeMinimum.Value);
            buffer[23] = (short)indexToPin[leftHorizontalEyePin.SelectedIndex];
            buffer[24] = (short)(enableLeftHorizontalEye.Checked?1:0);

            buffer[25] = (short)rightHorizontalEyeTrim.Value;
            buffer[26] = (short)(rightHorizontalEyeMaximum.Value);
            buffer[27] = (short)(rightHorizontalEyeMinimum.Value);
            buffer[28] = (short)indexToPin[rightHorizontalEyePin.SelectedIndex];
            buffer[29] = (short)(enableRightHorizontalEye.Checked?1:0);

            buffer[30] = (short)leftVerticalEyeTrim.Value;
            buffer[31] = (short)(leftVerticalEyeMaximum.Value);
            buffer[32] = (short)(leftVerticalEyeMinimum.Value);
            buffer[33] = (short)indexToPin[leftVerticalEyePin.SelectedIndex];
            buffer[34] = (short)(enableLeftVerticalEye.Checked?1:0);

            buffer[35] = (short)rightVerticalEyeTrim.Value;
            buffer[36] = (short)(rightVerticalEyeMaximum.Value);
            buffer[37] = (short)(rightVerticalEyeMinimum.Value);
            buffer[38] = (short)indexToPin[rightVerticalEyePin.SelectedIndex];
            buffer[39] = (short)(enableRightVerticalEye.Checked?1:0);

            buffer[40] = (short)rotateNeckTrim.Value;
            buffer[41] = (short)(rotateNeckMaximum.Value);
            buffer[42] = (short)(rotateNeckMinimum.Value);
            buffer[43] = (short)indexToPin[rotateNeckPin.SelectedIndex];
            buffer[44] = (short)(enableRotateNeck.Checked?1:0);

            buffer[45] = (short)tiltNeckTrim.Value;
            buffer[46] = (short)(tiltNeckMaximum.Value);
            buffer[47] = (short)(tiltNeckMinimum.Value);
            buffer[48] = (short)indexToPin[tiltNeckPin.SelectedIndex];
            buffer[49] = (short)(enableTiltNeck.Checked?1:0);

            buffer[50] = (short)leftLipTrim.Value;
            buffer[51] = (short)(leftLipMaximum.Value);
            buffer[52] = (short)(leftLipMinimum.Value);
            buffer[53] = (short)indexToPin[leftLipPin.SelectedIndex];
            buffer[54] = (short)(enableLeftLip.Checked ? 1 : 0);

            buffer[55] = (short)rightLipTrim.Value;
            buffer[56] = (short)(rightLipMaximum.Value);
            buffer[57] = (short)(rightLipMinimum.Value);
            buffer[58] = (short)indexToPin[rightLipPin.SelectedIndex];
            buffer[59] = (short)(enableRightLip.Checked ? 1 : 0);

            buffer[60] = (short)jawTrim.Value;
            buffer[61] = (short)(jawMaximum.Value);
            buffer[62] = (short)(jawMinimum.Value);
            buffer[63] = (short)indexToPin[jawPin.SelectedIndex];
            buffer[64] = (short)(enableJaw.Checked ? 1 : 0);

            // placeholder for left ear
            buffer[65] = (short)0;
            buffer[66] = (short)0;
            buffer[67] = (short)0;
            buffer[68] = (short)0;
            buffer[69] = (short)0;

            // placeholder for right ear
            buffer[70] = (short)0;
            buffer[71] = (short)0;
            buffer[72] = (short)0;
            buffer[73] = (short)0;
            buffer[74] = (short)0;

            // sonar
            buffer[75] = 0;
            buffer[76] = 0;
            buffer[77] = (short)indexToPin[sonarTriggerPin.SelectedIndex];
            buffer[78] = (short)indexToPin[sonarEchoPin.SelectedIndex];
            buffer[79] = (short)(enableSonar.Checked ? 1 : 0);

            // IR
            buffer[80] = 0;
            buffer[81] = 0;
            buffer[82] = 0;
            buffer[83] = (short)indexToPin[irPin.SelectedIndex];
            buffer[84] = (short)(enableIR.Checked ? 1 : 0);

            return buffer;
        }

        private void leftEyelidTrim_ValueChanged(object sender, EventArgs e)
        {
            currentValue[2] = (int)leftEyelidTrim.Value;
            conductor.SetDirectLeftEyelid((int)indexToPin[leftEyelidPin.SelectedIndex], currentValue[2], (int)leftEyelidMinimum.Value, (int)leftEyelidMaximum.Value, true);
        }

        private void leftEyelidTrim_KeyUp(object sender, KeyEventArgs e)
        {
            leftEyelidTrim_ValueChanged(sender, e);
        }

        private void leftEyelidMinimum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[2] = (int)leftEyelidMinimum.Value;
            if (leftEyelidMinimum.Value > leftEyelidMaximum.Value)
                leftEyelidMaximum.Value = leftEyelidMinimum.Value;
            conductor.SetDirectLeftEyelid((int)indexToPin[leftEyelidPin.SelectedIndex], currentValue[2], (int)leftEyelidMinimum.Value, (int)leftEyelidMaximum.Value, false);
        }

        private void leftEyelidMinimum_KeyUp(object sender, KeyEventArgs e)
        {
            leftEyelidMinimum_ValueChanged(sender, e);
        }

        private void leftEyelidMaximum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[2] = (int)leftEyelidMaximum.Value;
            if (leftEyelidMaximum.Value < leftEyelidMinimum.Value)
                leftEyelidMinimum.Value = leftEyelidMaximum.Value;
            conductor.SetDirectLeftEyelid((int)indexToPin[leftEyelidPin.SelectedIndex], currentValue[2], (int)leftEyelidMinimum.Value, (int)leftEyelidMaximum.Value, false);
        }

        private void leftEyelidMaximum_KeyUp(object sender, KeyEventArgs e)
        {
            leftEyelidMaximum_ValueChanged(sender, e);
        }

        private void testLeftEyelid_Click(object sender, EventArgs e)
        {
            bool mode;

            if (currentActive[2])
            {
                testLeftEyelid.Text = "Test";
                mode = false;
            }
            else
            {
                testLeftEyelid.Text = "Stop";
                mode = true;
            }
            currentActive[2] = mode;

            leftEyelidPin.Enabled = !mode;
            leftEyelidTrim.Enabled = !mode;
            leftEyelidMaximum.Enabled = !mode;
            leftEyelidMinimum.Enabled = !mode;
        }


        private void rightEyelidTrim_ValueChanged(object sender, EventArgs e)
        {
            currentValue[3] = (int)rightEyelidTrim.Value;
            conductor.SetDirectRightEyelid((int)indexToPin[rightEyelidPin.SelectedIndex], currentValue[3], (int)rightEyelidMinimum.Value, (int)rightEyelidMaximum.Value, true);
        }

        private void rightEyelidTrim_KeyUp(object sender, KeyEventArgs e)
        {
            rightEyelidTrim_ValueChanged(sender, e);
        }

        private void rightEyelidMinimum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[3] = (int)rightEyelidMinimum.Value;
            if (rightEyelidMinimum.Value > rightEyelidMaximum.Value)
                rightEyelidMaximum.Value = rightEyelidMinimum.Value;
            conductor.SetDirectRightEyelid((int)indexToPin[rightEyelidPin.SelectedIndex], currentValue[3], (int)rightEyelidMinimum.Value, (int)rightEyelidMaximum.Value, false);
        }

        private void rightEyelidMinimum_KeyUp(object sender, KeyEventArgs e)
        {
            rightEyelidMinimum_ValueChanged(sender, e);
        }

        private void rightEyelidMaximum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[3] = (int)rightEyelidMaximum.Value;
            if (rightEyelidMaximum.Value < rightEyelidMinimum.Value)
                rightEyelidMinimum.Value = rightEyelidMaximum.Value;
            conductor.SetDirectRightEyelid((int)indexToPin[rightEyelidPin.SelectedIndex], currentValue[3], (int)rightEyelidMinimum.Value, (int)rightEyelidMaximum.Value, false);
        }

        private void rightEyelidMaximum_KeyUp(object sender, KeyEventArgs e)
        {
            rightEyelidMaximum_ValueChanged(sender, e);
        }

        private void testRightEyelid_Click(object sender, EventArgs e)
        {
            bool mode;

            if (currentActive[3])
            {
                testRightEyelid.Text = "Test";
                mode = false;
            }
            else
            {
                testRightEyelid.Text = "Stop";
                mode = true;
            }
            currentActive[3] = mode;

            rightEyelidPin.Enabled = !mode;
            rightEyelidTrim.Enabled = !mode;
            rightEyelidMaximum.Enabled = !mode;
            rightEyelidMinimum.Enabled = !mode;
        }


        private void leftHorizontalEyeTrim_ValueChanged(object sender, EventArgs e)
        {
            currentValue[4] = (int)leftHorizontalEyeTrim.Value;
            conductor.SetDirectLeftHorizontalEye((int)indexToPin[leftHorizontalEyePin.SelectedIndex], currentValue[4], (int)leftHorizontalEyeMinimum.Value, (int)leftHorizontalEyeMaximum.Value, true);
        }

        private void leftHorizontalEyeTrim_KeyUp(object sender, KeyEventArgs e)
        {
            leftHorizontalEyeTrim_ValueChanged(sender, e);
        }

        private void leftHorizontalEyeMinimum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[4] = (int)leftHorizontalEyeMinimum.Value;
            if (leftHorizontalEyeMinimum.Value > leftHorizontalEyeMaximum.Value)
                leftHorizontalEyeMaximum.Value = leftHorizontalEyeMinimum.Value;
            conductor.SetDirectLeftHorizontalEye((int)indexToPin[leftHorizontalEyePin.SelectedIndex], currentValue[4], (int)leftHorizontalEyeMinimum.Value, (int)leftHorizontalEyeMaximum.Value, false);
        }

        private void leftHorizontalEyeMinimum_KeyUp(object sender, KeyEventArgs e)
        {
            leftHorizontalEyeMinimum_ValueChanged(sender, e);
        }

        private void leftHorizontalEyeMaximum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[4] = (int)leftHorizontalEyeMaximum.Value;
            if (leftHorizontalEyeMaximum.Value < leftHorizontalEyeMinimum.Value)
                leftHorizontalEyeMinimum.Value = leftHorizontalEyeMaximum.Value;
            conductor.SetDirectLeftHorizontalEye((int)indexToPin[leftHorizontalEyePin.SelectedIndex], currentValue[4], (int)leftHorizontalEyeMinimum.Value, (int)leftHorizontalEyeMaximum.Value, false);
        }

        private void leftHorizontalEyeMaximum_KeyUp(object sender, KeyEventArgs e)
        {
            leftHorizontalEyeMaximum_ValueChanged(sender, e);
        }

        private void testLeftHorizontalEye_Click(object sender, EventArgs e)
        {
            bool mode;

            if (currentActive[4])
            {
                testLeftHorizontalEye.Text = "Test";
                mode = false;
            }
            else
            {
                testLeftHorizontalEye.Text = "Stop";
                mode = true;
            }
            currentActive[4] = mode;

            leftHorizontalEyePin.Enabled = !mode;
            leftHorizontalEyeTrim.Enabled = !mode;
            leftHorizontalEyeMaximum.Enabled = !mode;
            leftHorizontalEyeMinimum.Enabled = !mode;
        }

        private void rightHorizontalEyeTrim_ValueChanged(object sender, EventArgs e)
        {
            currentValue[5] = (int)rightHorizontalEyeTrim.Value;
            conductor.SetDirectRightHorizontalEye((int)indexToPin[rightHorizontalEyePin.SelectedIndex], currentValue[5], (int)rightHorizontalEyeMinimum.Value, (int)rightHorizontalEyeMaximum.Value, true);
        }

        private void rightHorizontalEyeTrim_KeyUp(object sender, KeyEventArgs e)
        {
            rightHorizontalEyeTrim_ValueChanged(sender, e);
        }

        private void rightHorizontalEyeMinimum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[5] = (int)rightHorizontalEyeMinimum.Value;
            if (rightHorizontalEyeMinimum.Value > rightHorizontalEyeMaximum.Value)
                rightHorizontalEyeMaximum.Value = rightHorizontalEyeMinimum.Value;
            conductor.SetDirectRightHorizontalEye((int)indexToPin[rightHorizontalEyePin.SelectedIndex], currentValue[5], (int)rightHorizontalEyeMinimum.Value, (int)rightHorizontalEyeMaximum.Value, false);
        }

        private void rightHorizontalEyeMinimum_KeyUp(object sender, KeyEventArgs e)
        {
            rightHorizontalEyeMinimum_ValueChanged(sender, e);
        }

        private void rightHorizontalEyeMaximum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[5] = (int)rightHorizontalEyeMaximum.Value;
            if (rightHorizontalEyeMaximum.Value < rightHorizontalEyeMinimum.Value)
                rightHorizontalEyeMinimum.Value = rightHorizontalEyeMaximum.Value;
            conductor.SetDirectRightHorizontalEye((int)indexToPin[rightHorizontalEyePin.SelectedIndex], currentValue[5], (int)rightHorizontalEyeMinimum.Value, (int)rightHorizontalEyeMaximum.Value, false);
        }

        private void rightHorizontalEyeMaximum_KeyUp(object sender, KeyEventArgs e)
        {
            rightHorizontalEyeMaximum_ValueChanged(sender, e);
        }

        private void testRightHorizontalEye_Click(object sender, EventArgs e)
        {
            bool mode;

            if (currentActive[5])
            {
                testRightHorizontalEye.Text = "Test";
                mode = false;
            }
            else
            {
                testRightHorizontalEye.Text = "Stop";
                mode = true;
            }
            currentActive[5] = mode;

            rightHorizontalEyePin.Enabled = !mode;
            rightHorizontalEyeTrim.Enabled = !mode;
            rightHorizontalEyeMaximum.Enabled = !mode;
            rightHorizontalEyeMinimum.Enabled = !mode;
        }


        private void leftVerticalEyeTrim_ValueChanged(object sender, EventArgs e)
        {
            currentValue[6] = (int)leftVerticalEyeTrim.Value;
            conductor.SetDirectLeftVerticalEye((int)indexToPin[leftVerticalEyePin.SelectedIndex], currentValue[6], (int)leftVerticalEyeMinimum.Value, (int)leftVerticalEyeMaximum.Value, true);
        }

        private void leftVerticalEyeTrim_KeyUp(object sender, KeyEventArgs e)
        {
            leftVerticalEyeTrim_ValueChanged(sender, e);
        }

        private void leftVerticalEyeMinimum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[6] = (int)leftVerticalEyeMinimum.Value;
            if (leftVerticalEyeMinimum.Value > leftVerticalEyeMaximum.Value)
                leftVerticalEyeMaximum.Value = leftVerticalEyeMinimum.Value;
            conductor.SetDirectLeftVerticalEye((int)indexToPin[leftVerticalEyePin.SelectedIndex], currentValue[6], (int)leftVerticalEyeMinimum.Value, (int)leftVerticalEyeMaximum.Value, false);
        }

        private void leftVerticalEyeMinimum_KeyUp(object sender, KeyEventArgs e)
        {
            leftVerticalEyeMinimum_ValueChanged(sender, e);
        }

        private void leftVerticalEyeMaximum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[6] = (int)leftVerticalEyeMaximum.Value;
            if (leftVerticalEyeMaximum.Value < leftVerticalEyeMinimum.Value)
                leftVerticalEyeMinimum.Value = leftVerticalEyeMaximum.Value;
            conductor.SetDirectLeftVerticalEye((int)indexToPin[leftVerticalEyePin.SelectedIndex], currentValue[6], (int)leftVerticalEyeMinimum.Value, (int)leftVerticalEyeMaximum.Value, false);
        }

        private void leftVerticalEyeMaximum_KeyUp(object sender, KeyEventArgs e)
        {
            leftVerticalEyeMaximum_ValueChanged(sender, e);
        }

        private void testLeftVerticalEye_Click(object sender, EventArgs e)
        {
            bool mode;

            if (currentActive[6])
            {
                testLeftVerticalEye.Text = "Test";
                mode = false;
            }
            else
            {
                testLeftVerticalEye.Text = "Stop";
                mode = true;
            }
            currentActive[6] = mode;

            leftVerticalEyePin.Enabled = !mode;
            leftVerticalEyeTrim.Enabled = !mode;
            leftVerticalEyeMaximum.Enabled = !mode;
            leftVerticalEyeMinimum.Enabled = !mode;
        }


        private void rightVerticalEyeTrim_ValueChanged(object sender, EventArgs e)
        {
            currentValue[7] = (int)rightVerticalEyeTrim.Value;
            conductor.SetDirectRightVerticalEye((int)indexToPin[rightVerticalEyePin.SelectedIndex], currentValue[7], (int)rightVerticalEyeMinimum.Value, (int)rightVerticalEyeMaximum.Value, true);
        }

        private void rightVerticalEyeTrim_KeyUp(object sender, KeyEventArgs e)
        {
            rightVerticalEyeTrim_ValueChanged(sender, e);
        }

        private void rightVerticalEyeMinimum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[7] = (int)rightVerticalEyeMinimum.Value;
            if (rightVerticalEyeMinimum.Value > rightVerticalEyeMaximum.Value)
                rightVerticalEyeMaximum.Value = rightVerticalEyeMinimum.Value;
            conductor.SetDirectRightVerticalEye((int)indexToPin[rightVerticalEyePin.SelectedIndex], currentValue[7], (int)rightVerticalEyeMinimum.Value, (int)rightVerticalEyeMaximum.Value, false);
        }

        private void rightVerticalEyeMinimum_KeyUp(object sender, KeyEventArgs e)
        {
            rightVerticalEyeMinimum_ValueChanged(sender, e);
        }

        private void rightVerticalEyeMaximum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[7] = (int)rightVerticalEyeMaximum.Value;
            if (rightVerticalEyeMaximum.Value < rightVerticalEyeMinimum.Value)
                rightVerticalEyeMinimum.Value = rightVerticalEyeMaximum.Value;
            conductor.SetDirectRightVerticalEye((int)indexToPin[rightVerticalEyePin.SelectedIndex], currentValue[7], (int)rightVerticalEyeMinimum.Value, (int)rightVerticalEyeMaximum.Value, false);
        }

        private void rightVerticalEyeMaximum_KeyUp(object sender, KeyEventArgs e)
        {
            rightVerticalEyeMaximum_ValueChanged(sender, e);
        }

        private void testRightVerticalEye_Click(object sender, EventArgs e)
        {
            bool mode;

            if (currentActive[7])
            {
                testRightVerticalEye.Text = "Test";
                mode = false;
            }
            else
            {
                testRightVerticalEye.Text = "Stop";
                mode = true;
            }
            currentActive[7] = mode;

            rightVerticalEyePin.Enabled = !mode;
            rightVerticalEyeTrim.Enabled = !mode;
            rightVerticalEyeMaximum.Enabled = !mode;
            rightVerticalEyeMinimum.Enabled = !mode;
        }


        private void rotateNeckTrim_ValueChanged(object sender, EventArgs e)
        {
            currentValue[8] = (int)rotateNeckTrim.Value;
            conductor.SetDirectNeckTwist((int)indexToPin[rotateNeckPin.SelectedIndex], currentValue[8], (int)rotateNeckMinimum.Value, (int)rotateNeckMaximum.Value, true);
        }

        private void rotateNeckTrim_KeyUp(object sender, EventArgs e)
        {
            rotateNeckTrim_ValueChanged(sender, e);
        }

        private void rotateNeckMinimum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[8] = (int)rotateNeckMinimum.Value;
            if (rotateNeckMinimum.Value > rotateNeckMaximum.Value)
                rotateNeckMaximum.Value = rotateNeckMinimum.Value;
            conductor.SetDirectNeckTwist((int)indexToPin[rotateNeckPin.SelectedIndex], currentValue[8], (int)rotateNeckMinimum.Value, (int)rotateNeckMaximum.Value, false);
        }

        private void rotateNeckMinimum_KeyUp(object sender, EventArgs e)
        {
            rotateNeckMinimum_ValueChanged(sender, e);
        }

        private void rotateNeckMaximum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[8] = (int)rotateNeckMaximum.Value;
            if (rotateNeckMaximum.Value < rotateNeckMinimum.Value)
                rotateNeckMinimum.Value = rotateNeckMaximum.Value;
            conductor.SetDirectNeckTwist((int)indexToPin[rotateNeckPin.SelectedIndex], currentValue[8], (int)rotateNeckMinimum.Value, (int)rotateNeckMaximum.Value, false);
        }

        private void rotateNeckMaximum_KeyUp(object sender, EventArgs e)
        {
            rotateNeckMaximum_ValueChanged(sender, e);
        }

        private void testRotateNeck_Click(object sender, EventArgs e)
        {
            bool mode;

            if (currentActive[8])
            {
                testRotateNeck.Text = "Test";
                mode = false;
            }
            else
            {
                testRotateNeck.Text = "Stop";
                mode = true;
            }
            currentActive[8] = mode;

            rotateNeckPin.Enabled = !mode;
            rotateNeckTrim.Enabled = !mode;
            rotateNeckMaximum.Enabled = !mode;
            rotateNeckMinimum.Enabled = !mode;
        }

        private void tiltNeckTrim_ValueChanged(object sender, EventArgs e)
        {
            currentValue[9] = (int)tiltNeckTrim.Value;
            conductor.SetDirectNeckTilt((int)indexToPin[tiltNeckPin.SelectedIndex], currentValue[9], (int)tiltNeckMinimum.Value, (int)tiltNeckMaximum.Value, true);
        }

        private void tiltNeckTrim_KeyUp(object sender, EventArgs e)
        {
            tiltNeckTrim_ValueChanged(sender, e);
        }

        private void tiltNeckMinimum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[9] = (int)tiltNeckMinimum.Value;
            if (tiltNeckMinimum.Value > tiltNeckMaximum.Value)
                tiltNeckMaximum.Value = tiltNeckMinimum.Value;
            conductor.SetDirectNeckTilt((int)indexToPin[tiltNeckPin.SelectedIndex], currentValue[9], (int)tiltNeckMinimum.Value, (int)tiltNeckMaximum.Value, false);
        }

        private void tiltNeckMinimum_KeyUp(object sender, EventArgs e)
        {
            tiltNeckMinimum_ValueChanged(sender, e);
        }

        private void tiltNeckMaximum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[9] = (int)tiltNeckMaximum.Value;
            if (tiltNeckMaximum.Value < tiltNeckMinimum.Value)
                tiltNeckMinimum.Value = tiltNeckMaximum.Value;
            conductor.SetDirectNeckTilt((int)indexToPin[tiltNeckPin.SelectedIndex], currentValue[9], (int)tiltNeckMinimum.Value, (int)tiltNeckMaximum.Value, false);
        }

        private void tiltNeckMaximum_KeyUp(object sender, EventArgs e)
        {
            tiltNeckMaximum_ValueChanged(sender, e);
        }

        private void testTiltNeck_Click(object sender, EventArgs e)
        {
            bool mode;

            if (currentActive[9])
            {
                testTiltNeck.Text = "Test";
                mode = false;
            }
            else
            {
                testTiltNeck.Text = "Stop";
                mode = true;
            }
            currentActive[9] = mode;

            tiltNeckPin.Enabled = !mode;
            tiltNeckTrim.Enabled = !mode;
            tiltNeckMaximum.Enabled = !mode;
            tiltNeckMinimum.Enabled = !mode;
        }

        private void leftLipTrim_ValueChanged(object sender, EventArgs e)
        {
            currentValue[10] = (int)leftLipTrim.Value;
            conductor.SetDirectLeftLip((int)indexToPin[leftLipPin.SelectedIndex], currentValue[10], (int)leftLipMinimum.Value, (int)leftLipMaximum.Value, true);
        }

        private void leftLipTrim_KeyUp(object sender, EventArgs e)
        {
            leftLipTrim_ValueChanged(sender, e);
        }

        private void leftLipMinimum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[10] = (int)leftLipMinimum.Value;
            if (leftLipMinimum.Value > leftLipMaximum.Value)
                leftLipMaximum.Value = leftLipMinimum.Value;
            conductor.SetDirectLeftLip((int)indexToPin[leftLipPin.SelectedIndex], currentValue[10], (int)leftLipMinimum.Value, (int)leftLipMaximum.Value, false);
        }

        private void leftLipMinimum_KeyUp(object sender, EventArgs e)
        {
            leftLipMinimum_ValueChanged(sender, e);
        }

        private void leftLipMaximum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[10] = (int)leftLipMaximum.Value;
            if (leftLipMaximum.Value < leftLipMinimum.Value)
                leftLipMinimum.Value = leftLipMaximum.Value;
            conductor.SetDirectLeftLip((int)indexToPin[leftLipPin.SelectedIndex], currentValue[10], (int)leftLipMinimum.Value, (int)leftLipMaximum.Value, false);
        }

        private void leftLipMaximum_KeyUp(object sender, EventArgs e)
        {
            leftLipMaximum_ValueChanged(sender, e);
        }

        private void testLeftLip_Click(object sender, EventArgs e)
        {
            bool mode;

            if (currentActive[10])
            {
                testLeftLip.Text = "Test";
                mode = false;
            }
            else
            {
                testLeftLip.Text = "Stop";
                mode = true;
            }
            currentActive[10] = mode;

            leftLipPin.Enabled = !mode;
            leftLipTrim.Enabled = !mode;
            leftLipMaximum.Enabled = !mode;
            leftLipMinimum.Enabled = !mode;
        }

        private void rightLipTrim_ValueChanged(object sender, EventArgs e)
        {
            currentValue[11] = (int)rightLipTrim.Value;
            conductor.SetDirectRightLip((int)indexToPin[rightLipPin.SelectedIndex], currentValue[11], (int)rightLipMinimum.Value, (int)rightLipMaximum.Value, true);
        }

        private void rightLipTrim_KeyUp(object sender, EventArgs e)
        {
            rightLipTrim_ValueChanged(sender, e);
        }

        private void rightLipMinimum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[11] = (int)rightLipMinimum.Value;
            if (rightLipMinimum.Value > rightLipMaximum.Value)
                rightLipMaximum.Value = rightLipMinimum.Value;
            conductor.SetDirectRightLip((int)indexToPin[rightLipPin.SelectedIndex], currentValue[11], (int)rightLipMinimum.Value, (int)rightLipMaximum.Value, false);
        }

        private void rightLipMinimum_KeyUp(object sender, EventArgs e)
        {
            rightLipMinimum_ValueChanged(sender, e);
        }

        private void rightLipMaximum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[11] = (int)rightLipMaximum.Value;
            if (rightLipMaximum.Value < rightLipMinimum.Value)
                rightLipMinimum.Value = rightLipMaximum.Value;
            conductor.SetDirectRightLip((int)indexToPin[rightLipPin.SelectedIndex], currentValue[11], (int)rightLipMinimum.Value, (int)rightLipMaximum.Value, false);
        }

        private void rightLipMaximum_KeyUp(object sender, EventArgs e)
        {
            rightLipMaximum_ValueChanged(sender, e);
        }

        private void testRightLip_Click(object sender, EventArgs e)
        {
            bool mode;

            if (currentActive[11])
            {
                testRightLip.Text = "Test";
                mode = false;
            }
            else
            {
                testRightLip.Text = "Stop";
                mode = true;
            }
            currentActive[11] = mode;

            rightLipPin.Enabled = !mode;
            rightLipTrim.Enabled = !mode;
            rightLipMaximum.Enabled = !mode;
            rightLipMinimum.Enabled = !mode;
        }

        private void jawTrim_ValueChanged(object sender, EventArgs e)
        {
            currentValue[12] = (int)jawTrim.Value;
            conductor.SetDirectJaw((int)indexToPin[jawPin.SelectedIndex], currentValue[12], (int)jawMinimum.Value, (int)jawMaximum.Value, true);
        }

        private void jawTrim_KeyUp(object sender, EventArgs e)
        {
            jawTrim_ValueChanged(sender, e);
        }

        private void jawMinimum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[12] = (int)jawMinimum.Value;
            if (jawMinimum.Value > jawMaximum.Value)
                jawMaximum.Value = jawMinimum.Value;
            conductor.SetDirectJaw((int)indexToPin[jawPin.SelectedIndex], currentValue[12], (int)jawMinimum.Value, (int)jawMaximum.Value, false);
        }

        private void jawMinimum_KeyUp(object sender, EventArgs e)
        {
            jawMinimum_ValueChanged(sender, e);
        }

        private void jawMaximum_ValueChanged(object sender, EventArgs e)
        {
            currentValue[12] = (int)jawMaximum.Value;
            if (jawMaximum.Value < jawMinimum.Value)
                jawMinimum.Value = jawMaximum.Value;
            conductor.SetDirectJaw((int)indexToPin[jawPin.SelectedIndex], currentValue[12], (int)jawMinimum.Value, (int)jawMaximum.Value, false);
        }

        private void jawMaximum_KeyUp(object sender, EventArgs e)
        {
            jawMaximum_ValueChanged(sender, e);
        }

        private void testJaw_Click(object sender, EventArgs e)
        {
            bool mode;

            if (currentActive[12])
            {
                testJaw.Text = "Test";
                mode = false;
            }
            else
            {
                testJaw.Text = "Stop";
                mode = true;
            }
            currentActive[12] = mode;

            jawPin.Enabled = !mode;
            jawTrim.Enabled = !mode;
            jawMaximum.Enabled = !mode;
            jawMinimum.Enabled = !mode;
        }

        private void enableLeftEyebrow_Click(object sender, EventArgs e)
        {
            bool res = enableLeftEyebrow.Checked;
            leftEyebrowMaximum.Enabled = res;
            leftEyebrowMinimum.Enabled = res;
            leftEyebrowPin.Enabled = res;
            leftEyebrowTrim.Enabled = res;
            testLeftEyebrow.Enabled = res;
            if (!res) conductor.ReleaseServo((int)indexToPin[leftEyebrowPin.SelectedIndex]);
        }

        private void enableRightEyebrow_Click(object sender, EventArgs e)
        {
            bool res = enableRightEyebrow.Checked;
            rightEyebrowMaximum.Enabled = res;
            rightEyebrowMinimum.Enabled = res;
            rightEyebrowPin.Enabled = res;
            rightEyebrowTrim.Enabled = res;
            testRightEyebrow.Enabled = res;
            if (!res) conductor.ReleaseServo((int)indexToPin[leftEyebrowPin.SelectedIndex]);
        }

        private void enableLeftEyelid_Click(object sender, EventArgs e)
        {
            bool res = enableLeftEyelid.Checked;
            leftEyelidMaximum.Enabled = res;
            leftEyelidMinimum.Enabled = res;
            leftEyelidPin.Enabled = res;
            leftEyelidTrim.Enabled = res;
            testLeftEyelid.Enabled = res;
            if (!res) conductor.ReleaseServo((int)indexToPin[leftEyelidPin.SelectedIndex]);
        }

        private void enableRightEyelid_Click(object sender, EventArgs e)
        {
            bool res = enableRightEyelid.Checked;
            rightEyelidMaximum.Enabled = res;
            rightEyelidMinimum.Enabled = res;
            rightEyelidPin.Enabled = res;
            rightEyelidTrim.Enabled = res;
            testRightEyelid.Enabled = res;
            if (!res) conductor.ReleaseServo((int)indexToPin[rightEyelidPin.SelectedIndex]);
        }

        private void enableLeftHorizontalEye_Click(object sender, EventArgs e)
        {
            bool res = enableLeftHorizontalEye.Checked;
            leftHorizontalEyeMaximum.Enabled = res;
            leftHorizontalEyeMinimum.Enabled = res;
            leftHorizontalEyePin.Enabled = res;
            leftHorizontalEyeTrim.Enabled = res;
            testLeftHorizontalEye.Enabled = res;
            if (!res) conductor.ReleaseServo((int)indexToPin[leftHorizontalEyePin.SelectedIndex]);
        }

        private void enableRightHorizontalEye_Click(object sender, EventArgs e)
        {
            bool res = enableRightHorizontalEye.Checked;
            rightHorizontalEyeMaximum.Enabled = res;
            rightHorizontalEyeMinimum.Enabled = res;
            rightHorizontalEyePin.Enabled = res;
            rightHorizontalEyeTrim.Enabled = res;
            testRightHorizontalEye.Enabled = res;
            if (!res) conductor.ReleaseServo((int)indexToPin[rightHorizontalEyePin.SelectedIndex]);
        }

        private void enableLeftVerticalEye_Click(object sender, EventArgs e)
        {
            bool res = enableLeftVerticalEye.Checked;
            leftVerticalEyeMaximum.Enabled = res;
            leftVerticalEyeMinimum.Enabled = res;
            leftVerticalEyePin.Enabled = res;
            leftVerticalEyeTrim.Enabled = res;
            testLeftVerticalEye.Enabled = res;
            if (!res) conductor.ReleaseServo((int)indexToPin[leftVerticalEyePin.SelectedIndex]);
        }

        private void enableRightVerticalEye_Click(object sender, EventArgs e)
        {
            bool res = enableRightVerticalEye.Checked;
            rightVerticalEyeMaximum.Enabled = res;
            rightVerticalEyeMinimum.Enabled = res;
            rightVerticalEyePin.Enabled = res;
            rightVerticalEyeTrim.Enabled = res;
            testRightVerticalEye.Enabled = res;
            if (!res) conductor.ReleaseServo((int)indexToPin[rightVerticalEyePin.SelectedIndex]);
        }

        private void enableRotateNeck_Click(object sender, EventArgs e)
        {
            bool res = enableRotateNeck.Checked;
            rotateNeckMaximum.Enabled = res;
            rotateNeckMinimum.Enabled = res;
            rotateNeckPin.Enabled = res;
            rotateNeckTrim.Enabled = res;
            testRotateNeck.Enabled = res;
            if (!res) conductor.ReleaseServo((int)indexToPin[rotateNeckPin.SelectedIndex]);
        }

        private void enableTiltNeck_Click(object sender, EventArgs e)
        {
            bool res = enableTiltNeck.Checked;
            tiltNeckMaximum.Enabled = res;
            tiltNeckMinimum.Enabled = res;
            tiltNeckPin.Enabled = res;
            tiltNeckTrim.Enabled = res;
            testTiltNeck.Enabled = res;
            if (!res) conductor.ReleaseServo((int)indexToPin[tiltNeckPin.SelectedIndex]);
        }

        private void enableLeftLip_Click(object sender, EventArgs e)
        {
            bool res = enableLeftLip.Checked;
            leftLipMaximum.Enabled = res;
            leftLipMinimum.Enabled = res;
            leftLipPin.Enabled = res;
            leftLipTrim.Enabled = res;
            testLeftLip.Enabled = res;
            if (!res) conductor.ReleaseServo((int)indexToPin[leftLipPin.SelectedIndex]);
        }

        private void enableRightLip_Click(object sender, EventArgs e)
        {
            bool res = enableRightLip.Checked;
            rightLipMaximum.Enabled = res;
            rightLipMinimum.Enabled = res;
            rightLipPin.Enabled = res;
            rightLipTrim.Enabled = res;
            testRightLip.Enabled = res;
            if (!res) conductor.ReleaseServo((int)indexToPin[rightLipPin.SelectedIndex]);
        }

        private void enableJaw_Click(object sender, EventArgs e)
        {
            bool res = enableJaw.Checked;
            jawMaximum.Enabled = res;
            jawMinimum.Enabled = res;
            jawPin.Enabled = res;
            jawTrim.Enabled = res;
            testJaw.Enabled = res;
            if (!res) conductor.ReleaseServo((int)indexToPin[jawPin.SelectedIndex]);
        }

        private void enableSonar_Click(object sender, EventArgs e)
        {
            bool res = enableSonar.Checked;
            sonarTriggerPin.Enabled = res;
            sonarEchoPin.Enabled = res;
        }

        private void enableIR_Click(object sender, EventArgs e)
        {
            irPin.Enabled = enableIR.Checked;
        }

        public void OK_Click(object sender, EventArgs e)
        {
            conductor.Save(serialize());
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    public class CalibrationData
    {
        public int leftHorizontalEyeTrim;
        public int leftHorizontalEyeMax;
        public int leftHorizontalEyeMin;
        public int leftHorizontalEyePin;
        public bool leftHorizontalEyeEnabled;

        public int leftVerticalEyeTrim;
        public int leftVerticalEyeMax;
        public int leftVerticalEyeMin;
        public int leftVerticalEyePin;
        public bool leftVerticalEyeEnabled;

        public int rightHorizontalEyeTrim;
        public int rightHorizontalEyeMax;
        public int rightHorizontalEyeMin;
        public int rightHorizontalEyePin;
        public bool rightHorizontalEyeEnabled;

        public int rightVerticalEyeTrim;
        public int rightVerticalEyeMax;
        public int rightVerticalEyeMin;
        public int rightVerticalEyePin;
        public bool rightVerticalEyeEnabled;

        public int leftEyebrowTrim;
        public int leftEyebrowMax;
        public int leftEyebrowMin;
        public int leftEyebrowPin;
        public bool leftEyebrowEnabled;

        public int rightEyebrowTrim;
        public int rightEyebrowMax;
        public int rightEyebrowMin;
        public int rightEyebrowPin;
        public bool rightEyebrowEnabled;

        public int rightLidPositionTrim;
        public int rightLidPositionMax;
        public int rightLidPositionMin;
        public int rightLidPositionPin;
        public bool rightLidPositionEnabled;

        public int leftLidPositionTrim;
        public int leftLidPositionMax;
        public int leftLidPositionMin;
        public int leftLidPositionPin;
        public bool leftLidPositionEnabled;

        public int neckTiltTrim;
        public int neckTiltMax;
        public int neckTiltMin;
        public int neckTiltPin;
        public bool neckTiltEnabled;

        public int neckTwistTrim;
        public int neckTwistMax;
        public int neckTwistMin;
        public int neckTwistPin;
        public bool neckTwistEnabled;

        public int leftLipTrim;
        public int leftLipMax;
        public int leftLipMin;
        public int leftLipPin;
        public bool leftLipEnabled;

        public int rightLipTrim;
        public int rightLipMax;
        public int rightLipMin;
        public int rightLipPin;
        public bool rightLipEnabled;

        public int jawTrim;
        public int jawMax;
        public int jawMin;
        public int jawPin;
        public bool jawEnabled;

        public int sonarTriggerPin;
        public int sonarEchoPin;
        public bool sonarEnabled;

        public int irPin;
        public bool irEnabled;
    }
}
