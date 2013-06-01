using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AGaugeApp;
using SharpDX.DirectInput;

namespace Fritz
{
    public partial class JoystickConfiguration : Form
    {
        // Initialize DirectInput
        DirectInput directInput = new DirectInput();

        Conductor conductor;
        Guid joystickGuid = Guid.Empty;
        bool reAcquireJoystick = true;
        Joystick joystick = null;
        int skipCount = 0;
        int skipCheck = 0;

        public bool calibrating = false;

        bool []buttons = {false, false, false, false, false, false, false, false, false, false, false, false};

        Dictionary<string, Guid> devices = new Dictionary<string, Guid>();

        public JoystickConfiguration()
        {
            InitializeComponent();

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
            {
                devices.Add(deviceInstance.InstanceName, deviceInstance.InstanceGuid);
                if (joystickGuid.Equals(Guid.Empty)) joystickGuid = deviceInstance.InstanceGuid;
            }

            comboBoxXAxis.Items.AddRange(Conductor.multiValueOptions);
            comboBoxXAxis.SelectedIndex = Conductor.GetMultiIndex(Fritz.Properties.Settings.Default.Joystick_X_Axis, "Eyes Horizontal");
            checkBoxInvertXAxis.Checked = Fritz.Properties.Settings.Default.Joystick_X_Axis_Invert;

            comboBoxYAxis.Items.AddRange(Conductor.multiValueOptions);
            comboBoxYAxis.SelectedIndex = Conductor.GetMultiIndex(Fritz.Properties.Settings.Default.Joystick_Y_Axis, "Neck Tilt");
            checkBoxInvertYAxis.Checked = Fritz.Properties.Settings.Default.Joystick_Y_Axis_Invert;

            comboBoxTwist.Items.AddRange(Conductor.multiValueOptions);
            comboBoxTwist.SelectedIndex = Conductor.GetMultiIndex(Fritz.Properties.Settings.Default.Joystick_Twist, "Neck Twist");
            checkBoxInvertTwist.Checked = Fritz.Properties.Settings.Default.Joystick_Twist_Invert;

            comboBoxRudder.Items.AddRange(Conductor.multiValueOptions);
            comboBoxRudder.SelectedIndex = Conductor.GetMultiIndex(Fritz.Properties.Settings.Default.Joystick_Rudder, "Eyes Vertical");
            checkBoxInvertRudder.Checked = Fritz.Properties.Settings.Default.Joystick_Rudder_Invert;

            comboBoxThrottle.Items.AddRange(Conductor.multiValueOptions);
            comboBoxThrottle.SelectedIndex = Conductor.GetMultiIndex(Fritz.Properties.Settings.Default.Joystick_Throttle, "Eyebrows");
            checkBoxInvertThrottle.Checked = Fritz.Properties.Settings.Default.Joystick_Throttle_Invert;

            comboBoxRotation.Items.AddRange(Conductor.multiValueOptions);
            comboBoxRotation.SelectedIndex = Conductor.GetMultiIndex(Fritz.Properties.Settings.Default.Joystick_Rotation, "Lips");
            checkBoxInvertRotation.Checked = Fritz.Properties.Settings.Default.Joystick_Rotation_Invert;

            comboBox0.Items.AddRange(Conductor.singleValueOptions);
            comboBox0.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Joystick_button_0, "Jaw Open");
            checkBoxRevert0.Checked = Fritz.Properties.Settings.Default.Joystick_button_0_Revert;

            comboBox1.Items.AddRange(Conductor.singleValueOptions);
            comboBox1.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Joystick_button_1, "Eyelids Blink");
            checkBoxRevert1.Checked = Fritz.Properties.Settings.Default.Joystick_button_1_Revert;

            comboBox2.Items.AddRange(Conductor.singleValueOptions);
            comboBox2.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Joystick_button_2, "Eyes Left");
            checkBoxRevert2.Checked = Fritz.Properties.Settings.Default.Joystick_button_2_Revert;

            comboBox3.Items.AddRange(Conductor.singleValueOptions);
            comboBox3.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Joystick_button_3, "Eyes Right");
            checkBoxRevert3.Checked = Fritz.Properties.Settings.Default.Joystick_button_3_Revert;

            comboBox4.Items.AddRange(Conductor.singleValueOptions);
            comboBox4.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Joystick_button_4, "Eyes Center");
            checkBoxRevert4.Checked = Fritz.Properties.Settings.Default.Joystick_button_4_Revert;

            comboBox5.Items.AddRange(Conductor.singleValueOptions);
            comboBox5.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Joystick_button_5, "Lips Smile");
            checkBoxRevert5.Checked = Fritz.Properties.Settings.Default.Joystick_button_5_Revert;

            comboBox6.Items.AddRange(Conductor.singleValueOptions);
            comboBox6.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Joystick_button_6, "Lips Frown");
            checkBoxRevert6.Checked = Fritz.Properties.Settings.Default.Joystick_button_6_Revert;

            comboBox7.Items.AddRange(Conductor.singleValueOptions);
            comboBox7.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Joystick_button_7, "");
            checkBoxRevert7.Checked = Fritz.Properties.Settings.Default.Joystick_button_7_Revert;

            comboBox8.Items.AddRange(Conductor.singleValueOptions);
            comboBox8.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Joystick_button_8, "");
            checkBoxRevert8.Checked = Fritz.Properties.Settings.Default.Joystick_button_8_Revert;

            comboBox9.Items.AddRange(Conductor.singleValueOptions);
            comboBox9.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Joystick_button_9, "");
            checkBoxRevert9.Checked = Fritz.Properties.Settings.Default.Joystick_button_9_Revert;

            comboBox10.Items.AddRange(Conductor.singleValueOptions);
            comboBox10.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Joystick_button_10, "");
            checkBoxRevert10.Checked = Fritz.Properties.Settings.Default.Joystick_button_10_Revert;

            comboBox11.Items.AddRange(Conductor.singleValueOptions);
            comboBox11.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Joystick_button_11, "");
            checkBoxRevert11.Checked = Fritz.Properties.Settings.Default.Joystick_button_11_Revert;

            comboBox12.Items.AddRange(Conductor.singleValueOptions);
            comboBox12.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Joystick_button_12, "");
            checkBoxRevert12.Checked = Fritz.Properties.Settings.Default.Joystick_button_12_Revert;
        }

        public void SetConductor(ref Conductor c)
        {
            conductor = c;
        }

        private void JoystickConfiguration_Load(object sender, EventArgs e)
        {
            if (comboBoxJoysticks.Items.Count>0)
                comboBoxJoysticks.SelectedIndex = 0;
        }

        public void RefreshDeviceList()
        {
            // If Gamepad not found, look for a Joystick
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
            {
                comboBoxJoysticks.Items.Add(deviceInstance.InstanceName);
                if (!devices.ContainsKey(deviceInstance.InstanceName))
                    devices.Add(deviceInstance.InstanceName, deviceInstance.InstanceGuid);
            }
        }

        private void comboBoxJoysticks_SelectedIndexChanged(object sender, EventArgs e)
        {
            devices.TryGetValue((string)comboBoxJoysticks.SelectedItem, out joystickGuid);
            reAcquireJoystick = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((conductor==null)||(calibrating)) return;

            if (skipCheck > 0)
            {
                --skipCheck;
                return;
            }

            if (reAcquireJoystick||(!directInput.IsDeviceAttached(joystickGuid)))
            {
                if (joystick != null)
                    joystick.Unacquire();

                IList<DeviceInstance> deviceInstance = directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices);
                if (deviceInstance.Count > 0)
                    joystickGuid = deviceInstance[0].InstanceGuid;
                else
                {
                    // don't check for another second
                    skipCheck = 100;
                    return;
                }

                reAcquireJoystick = false;

                // Instantiate the joystick
                joystick = new Joystick(directInput, joystickGuid);

                // Set BufferSize in order to use buffered data.
                joystick.Properties.BufferSize = 128;

                // Acquire the joystick
                joystick.Acquire();

                // allow values to stabilize before using them!
                skipCount = 50;
            }

            float tmp;
            JoystickUpdate[] datas = null;
            try
            {
                joystick.Poll();
                datas = joystick.GetBufferedData();
            }
            catch (Exception)
            {
                reAcquireJoystick = true;
                joystick.Unacquire();
                joystick = null;
            }

            if (skipCount-- > 0) return;

            if (datas == null) return;

            foreach (JoystickUpdate state in datas)
            {
                if (state.Offset == JoystickOffset.X)
                {
                    guageXAxis.Value = tmp = state.Value / 65535.0f;
                    conductor.Set((string)comboBoxXAxis.SelectedItem, checkBoxInvertXAxis.Checked? (1.0f - tmp) : tmp);
                }
                if (state.Offset == JoystickOffset.Y)
                {
                    guageYAxis.Value = tmp = state.Value / 65535.0f;
                    conductor.Set((string)comboBoxYAxis.SelectedItem, checkBoxInvertYAxis.Checked ? (1.0f - tmp) : tmp);
                }
                if (state.Offset == JoystickOffset.Sliders0)
                {
                    guageRudder.Value = tmp = state.Value / 65535.0f;
                    conductor.Set((string)comboBoxRudder.SelectedItem, checkBoxInvertRudder.Checked ? (1.0f - tmp) : tmp);
                }
                if (state.Offset == JoystickOffset.Z)
                {
                    guageThrottle.Value = tmp = state.Value / 65535.0f;
                    conductor.Set((string)comboBoxThrottle.SelectedItem, checkBoxInvertThrottle.Checked ? (1.0f - tmp) : tmp);
                }
                if (state.Offset == JoystickOffset.RotationZ)
                {
                    guageTwist.Value = tmp = state.Value / 65535.0f;
                    conductor.Set((string)comboBoxTwist.SelectedItem, checkBoxInvertTwist.Checked ? (1.0f - tmp) : tmp);
                }
                if (state.Offset == JoystickOffset.RotationX)
                {
                    guageRotation.Value = tmp = state.Value / 65535.0f;
                    conductor.Set((string)comboBoxRotation.SelectedItem, checkBoxInvertRotation.Checked ? (1.0f - tmp) : tmp);
                }
                if (state.Offset == JoystickOffset.Buttons0)
                {
                    if (state.Value != 0)
                    {
                        if (!buttons[0])
                        {
                            conductor.Set((string)comboBox0.SelectedItem, true);
                            buttons[0] = true;
                        }

                        value0.Text = "1";
                        value0.BackColor = Color.Green;
                    }
                    else
                    {
                        if (buttons[0])
                        {
                            if (checkBoxRevert0.Checked)
                                conductor.Set((string)comboBox0.SelectedItem, false);
                            buttons[0] = false;
                        }
                        value0.Text = "0";
                        value0.BackColor = Color.Red;
                    }
                }
                if (state.Offset == JoystickOffset.Buttons1)
                {
                    if (state.Value != 0)
                    {
                        if (!buttons[1])
                        {
                            conductor.Set((string)comboBox1.SelectedItem, true);
                            buttons[1] = true;
                        }

                        value1.Text = "1";
                        value1.BackColor = Color.Green;
                    }
                    else
                    {
                        if (buttons[1])
                        {
                            if (checkBoxRevert1.Checked)
                                conductor.Set((string)comboBox1.SelectedItem, false);
                            buttons[1] = false;
                        }
                        value1.Text = "0";
                        value1.BackColor = Color.Red;
                    }
                }
                if (state.Offset == JoystickOffset.Buttons2)
                {
                    if (state.Value != 0)
                    {
                        if (!buttons[2])
                        {
                            conductor.Set((string)comboBox2.SelectedItem, true);
                            buttons[2] = true;
                        }

                        value2.Text = "1";
                        value2.BackColor = Color.Green;
                    }
                    else
                    {
                        if (buttons[2])
                        {
                            if (checkBoxRevert2.Checked)
                                conductor.Set((string)comboBox2.SelectedItem, false);
                            buttons[2] = false;
                        }
                        value2.Text = "0";
                        value2.BackColor = Color.Red;
                    }
                }
                if (state.Offset == JoystickOffset.Buttons3)
                {
                    if (state.Value != 0)
                    {
                        if (!buttons[3])
                        {
                            conductor.Set((string)comboBox3.SelectedItem, true);
                            buttons[3] = true;
                        }

                        value3.Text = "1";
                        value3.BackColor = Color.Green;
                    }
                    else
                    {
                        if (buttons[3])
                        {
                            if (checkBoxRevert3.Checked)
                                conductor.Set((string)comboBox3.SelectedItem, false);
                            buttons[3] = false;
                        }
                        value3.Text = "0";
                        value3.BackColor = Color.Red;
                    }
                }
                if (state.Offset == JoystickOffset.Buttons4)
                {
                    if (state.Value != 0)
                    {
                        if (!buttons[4])
                        {
                            conductor.Set((string)comboBox4.SelectedItem, true);
                            buttons[4] = true;
                        }

                        value4.Text = "1";
                        value4.BackColor = Color.Green;
                    }
                    else
                    {
                        if (buttons[4])
                        {
                            if (checkBoxRevert4.Checked)
                                conductor.Set((string)comboBox4.SelectedItem, false);
                            buttons[4] = false;
                        }
                        value4.Text = "0";
                        value4.BackColor = Color.Red;
                    }
                }
                if (state.Offset == JoystickOffset.Buttons5)
                {
                    if (state.Value != 0)
                    {
                        if (!buttons[5])
                        {
                            conductor.Set((string)comboBox5.SelectedItem, true);
                            buttons[5] = true;
                        }

                        value5.Text = "1";
                        value5.BackColor = Color.Green;
                    }
                    else
                    {
                        if (buttons[5])
                        {
                            if (checkBoxRevert5.Checked)
                                conductor.Set((string)comboBox5.SelectedItem, false);
                            buttons[5] = false;
                        }
                        value5.Text = "0";
                        value5.BackColor = Color.Red;
                    }
                }
                if (state.Offset == JoystickOffset.Buttons6)
                {
                    if (state.Value != 0)
                    {
                        if (!buttons[6])
                        {
                            conductor.Set((string)comboBox6.SelectedItem, true);
                            buttons[6] = true;
                        }

                        value6.Text = "1";
                        value6.BackColor = Color.Green;
                    }
                    else
                    {
                        if (buttons[6])
                        {
                            if (checkBoxRevert6.Checked)
                                conductor.Set((string)comboBox6.SelectedItem, false);
                            buttons[6] = false;
                        }
                        value6.Text = "0";
                        value6.BackColor = Color.Red;
                    }
                }
                if (state.Offset == JoystickOffset.Buttons7)
                {
                    if (state.Value != 0)
                    {
                        if (!buttons[7])
                        {
                            conductor.Set((string)comboBox7.SelectedItem, true);
                            buttons[7] = true;
                        }

                        value7.Text = "1";
                        value7.BackColor = Color.Green;
                    }
                    else
                    {
                        if (buttons[7])
                        {
                            if (checkBoxRevert7.Checked)
                                conductor.Set((string)comboBox7.SelectedItem, false);
                            buttons[7] = false;
                        }
                        value7.Text = "0";
                        value7.BackColor = Color.Red;
                    }
                }
                if (state.Offset == JoystickOffset.Buttons8)
                {
                    if (state.Value != 0)
                    {
                        if (!buttons[8])
                        {
                            conductor.Set((string)comboBox8.SelectedItem, true);
                            buttons[8] = true;
                        }

                        value8.Text = "1";
                        value8.BackColor = Color.Green;
                    }
                    else
                    {
                        if (buttons[8])
                        {
                            if (checkBoxRevert8.Checked)
                                conductor.Set((string)comboBox8.SelectedItem, false);
                            buttons[8] = false;
                        }
                        value8.Text = "0";
                        value8.BackColor = Color.Red;
                    }
                }
                if (state.Offset == JoystickOffset.Buttons9)
                {
                    if (state.Value != 0)
                    {
                        if (!buttons[9])
                        {
                            conductor.Set((string)comboBox9.SelectedItem, true);
                            buttons[9] = true;
                        }

                        value9.Text = "1";
                        value9.BackColor = Color.Green;
                    }
                    else
                    {
                        if (buttons[9])
                        {
                            if (checkBoxRevert9.Checked)
                                conductor.Set((string)comboBox9.SelectedItem, false);
                            buttons[9] = false;
                        }
                        value9.Text = "0";
                        value9.BackColor = Color.Red;
                    }
                }
                if (state.Offset == JoystickOffset.Buttons10)
                {
                    if (state.Value != 0)
                    {
                        if (!buttons[10])
                        {
                            conductor.Set((string)comboBox10.SelectedItem, true);
                            buttons[10] = true;
                        }

                        value10.Text = "1";
                        value10.BackColor = Color.Green;
                    }
                    else
                    {
                        if (buttons[10])
                        {
                            if (checkBoxRevert10.Checked)
                                conductor.Set((string)comboBox10.SelectedItem, false);
                            buttons[10] = false;
                        }
                        value10.Text = "0";
                        value10.BackColor = Color.Red;
                    }
                }
                if (state.Offset == JoystickOffset.Buttons11)
                {
                    if (state.Value != 0)
                    {
                        if (!buttons[11])
                        {
                            conductor.Set((string)comboBox11.SelectedItem, true);
                            buttons[11] = true;
                        }

                        value11.Text = "1";
                        value11.BackColor = Color.Green;
                    }
                    else
                    {
                        if (buttons[11])
                        {
                            if (checkBoxRevert11.Checked)
                                conductor.Set((string)comboBox11.SelectedItem, false);
                            buttons[11] = false;
                        }
                        value11.Text = "0";
                        value11.BackColor = Color.Red;
                    }
                }
                if (state.Offset == JoystickOffset.Buttons12)
                {
                    if (state.Value != 0)
                    {
                        if (!buttons[12])
                        {
                            conductor.Set((string)comboBox12.SelectedItem, true);
                            buttons[12] = true;
                        }

                        value12.Text = "1";
                        value12.BackColor = Color.Green;
                    }
                    else
                    {
                        if (buttons[12])
                        {
                            if (checkBoxRevert12.Checked)
                                conductor.Set((string)comboBox12.SelectedItem, false);
                            buttons[12] = false;
                        }
                        value12.Text = "0";
                        value12.BackColor = Color.Red;
                    }
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Fritz.Properties.Settings.Default.Joystick_X_Axis = comboBoxXAxis.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_X_Axis_Invert = checkBoxInvertXAxis.Checked;

            Fritz.Properties.Settings.Default.Joystick_Y_Axis = comboBoxYAxis.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_Y_Axis_Invert = checkBoxInvertYAxis.Checked;

            Fritz.Properties.Settings.Default.Joystick_Twist = comboBoxTwist.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_Twist_Invert = checkBoxInvertTwist.Checked;

            Fritz.Properties.Settings.Default.Joystick_Rudder = comboBoxRudder.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_Rudder_Invert = checkBoxInvertRudder.Checked;

            Fritz.Properties.Settings.Default.Joystick_Throttle = comboBoxThrottle.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_Throttle_Invert = checkBoxInvertThrottle.Checked;

            Fritz.Properties.Settings.Default.Joystick_Rotation = comboBoxRotation.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_Rotation_Invert = checkBoxInvertRotation.Checked;

            Fritz.Properties.Settings.Default.Joystick_button_0 = comboBox0.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_button_0_Revert = checkBoxRevert0.Checked;

            Fritz.Properties.Settings.Default.Joystick_button_1 = comboBox1.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_button_1_Revert = checkBoxRevert1.Checked;

            Fritz.Properties.Settings.Default.Joystick_button_2 = comboBox2.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_button_2_Revert = checkBoxRevert2.Checked;

            Fritz.Properties.Settings.Default.Joystick_button_3 = comboBox3.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_button_3_Revert = checkBoxRevert3.Checked;

            Fritz.Properties.Settings.Default.Joystick_button_4 = comboBox4.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_button_4_Revert = checkBoxRevert4.Checked;

            Fritz.Properties.Settings.Default.Joystick_button_5 = comboBox5.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_button_5_Revert = checkBoxRevert5.Checked;

            Fritz.Properties.Settings.Default.Joystick_button_6 = comboBox6.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_button_6_Revert = checkBoxRevert6.Checked;

            Fritz.Properties.Settings.Default.Joystick_button_7 = comboBox7.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_button_7_Revert = checkBoxRevert7.Checked;

            Fritz.Properties.Settings.Default.Joystick_button_8 = comboBox8.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_button_8_Revert = checkBoxRevert8.Checked;

            Fritz.Properties.Settings.Default.Joystick_button_9 = comboBox9.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_button_9_Revert = checkBoxRevert9.Checked;

            Fritz.Properties.Settings.Default.Joystick_button_10 = comboBox10.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_button_10_Revert = checkBoxRevert10.Checked;

            Fritz.Properties.Settings.Default.Joystick_button_11 = comboBox11.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_button_11_Revert = checkBoxRevert11.Checked;

            Fritz.Properties.Settings.Default.Joystick_button_12 = comboBox12.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Joystick_button_12_Revert = checkBoxRevert12.Checked;

            Fritz.Properties.Settings.Default.Save();

            this.Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
