using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Fritz
{
    public partial class KeyboardConfiguration : Form
    {
        public static String[] keys = { "", "[arrow right]", "[arrow left]", "[arrow up]", "[arrow down]", "[home]", "[end]", "[page up]", 
                            "[page down]", "[delete]", "[insert]", "[backspace]", "[enter]", "[tab]", "[esc]", "[space]", "[F1]", 
                            "[F2]", "[F3]", "[F4]", "[F5]", "[F6]", "[F7]", "[F8]", "[F9]", "[F10]", "[F11]", "[F12]", "0", "1", "2", 
                            "3", "4", "5", "6", "7", "8", "9", 
                            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", 
                            "t", "u", "v", "w", "x", "y", "z"};

        Conductor conductor;
        Dictionary<string, Keys> keyMapper = new Dictionary<string, Keys>();

        public void disableKey(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        public KeyboardConfiguration()
        {
            InitializeComponent();

            Keys modifiers;

            comboBoxKey1.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction1.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey1.Items.AddRange(keys);
            comboBoxKey1.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_1, "[space]");
            comboBoxAction1.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction1.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_1, "Jaw Open");
            checkBoxRevert1.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_1;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_1;
            checkBoxCTRL1.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT1.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT1.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey2.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction2.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey2.Items.AddRange(keys);
            comboBoxKey2.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_2, "[arrow left]");
            comboBoxAction2.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction2.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_2, "Neck Slight Left");
            checkBoxRevert2.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_2;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_2;
            checkBoxCTRL2.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT2.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT2.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey3.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction3.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey3.Items.AddRange(keys);
            comboBoxKey3.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_3, "[arrow right]");
            comboBoxAction3.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction3.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_3, "Neck Slight Right");
            checkBoxRevert3.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_3;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_3;
            checkBoxCTRL3.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT3.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT3.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey4.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction4.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey4.Items.AddRange(keys);
            comboBoxKey4.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_4, "[arrow up]");
            comboBoxAction4.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction4.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_4, "Neck Slight Up");
            checkBoxRevert4.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_4;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_4;
            checkBoxCTRL4.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT4.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT4.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey5.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction5.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey5.Items.AddRange(keys);
            comboBoxKey5.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_5, "[arrow down]");
            comboBoxAction5.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction5.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_5, "Neck Slight Down");
            checkBoxRevert5.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_5;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_5;
            checkBoxCTRL5.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT5.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT5.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey6.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction6.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey6.Items.AddRange(keys);
            comboBoxKey6.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_6, "[arrow left]");
            comboBoxAction6.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction6.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_6, "Eyes Slight Left");
            checkBoxRevert6.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_6;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_6;
            checkBoxCTRL6.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT6.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT6.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey7.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction7.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey7.Items.AddRange(keys);
            comboBoxKey7.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_7, "[arrow right]");
            comboBoxAction7.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction7.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_7, "Eyes Slight Right");
            checkBoxRevert7.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_7;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_7;
            checkBoxCTRL7.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT7.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT7.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey8.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction8.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey8.Items.AddRange(keys);
            comboBoxKey8.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_8, "[arrow up]");
            comboBoxAction8.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction8.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_8, "Eyes Slight Up");
            checkBoxRevert8.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_8;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_8;
            checkBoxCTRL8.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT8.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT8.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey9.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction9.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey9.Items.AddRange(keys);
            comboBoxKey9.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_9, "[arrow down]");
            comboBoxAction9.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction9.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_9, "Eyes Slight Down");
            checkBoxRevert9.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_9;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_9;
            checkBoxCTRL9.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT9.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT9.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey10.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction10.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey10.Items.AddRange(keys);
            comboBoxKey10.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_10, "[arrow left]");
            comboBoxAction10.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction10.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_10, "[Eyebrows Slight Spread]");
            checkBoxRevert10.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_10;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_10;
            checkBoxCTRL10.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT10.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT10.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey11.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction11.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey11.Items.AddRange(keys);
            comboBoxKey11.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_11, "[arrow right]");
            comboBoxAction11.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction11.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_11, "[Eyebrows Slight Pinch]");
            checkBoxRevert11.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_11;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_11;
            checkBoxCTRL11.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT11.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT11.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey12.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction12.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey12.Items.AddRange(keys);
            comboBoxKey12.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_12, "");
            comboBoxAction12.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction12.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_12, "");
            checkBoxRevert12.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_12;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_12;
            checkBoxCTRL12.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT12.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT12.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey13.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction13.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey13.Items.AddRange(keys);
            comboBoxKey13.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_13, "");
            comboBoxAction13.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction13.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_13, "");
            checkBoxRevert13.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_13;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_13;
            checkBoxCTRL13.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT13.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT13.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey14.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction14.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey14.Items.AddRange(keys);
            comboBoxKey14.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_14, "");
            comboBoxAction14.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction14.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_14, "");
            checkBoxRevert14.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_14;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_14;
            checkBoxCTRL14.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT14.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT14.Checked = ((modifiers & Keys.Shift) != 0);

            comboBoxKey15.KeyDown += new KeyEventHandler(disableKey);
            comboBoxAction15.KeyDown += new KeyEventHandler(disableKey);
            comboBoxKey15.Items.AddRange(keys);
            comboBoxKey15.SelectedIndex = GetKeyIndex(Fritz.Properties.Settings.Default.Keyboard_Key_15, "");
            comboBoxAction15.Items.AddRange(Conductor.singleValueOptions);
            comboBoxAction15.SelectedIndex = Conductor.GetSingleIndex(Fritz.Properties.Settings.Default.Keyboard_Action_15, "");
            checkBoxRevert15.Checked = Fritz.Properties.Settings.Default.Keyboard_Revert_15;
            modifiers = (Keys)Fritz.Properties.Settings.Default.Keyboard_Modifiers_15;
            checkBoxCTRL15.Checked = ((modifiers & Keys.Control) != 0);
            checkBoxALT15.Checked = ((modifiers & Keys.Alt) != 0);
            checkBoxSHIFT15.Checked = ((modifiers & Keys.Shift) != 0);

            keyMapper.Add(keys[1], Keys.Right);
            keyMapper.Add(keys[2], Keys.Left);
            keyMapper.Add(keys[3], Keys.Up);
            keyMapper.Add(keys[4], Keys.Down);
            keyMapper.Add(keys[5], Keys.Home);
            keyMapper.Add(keys[6], Keys.End);
            keyMapper.Add(keys[7], Keys.PageUp);
            keyMapper.Add(keys[8], Keys.PageDown);
            keyMapper.Add(keys[9], Keys.Delete);
            keyMapper.Add(keys[10], Keys.Insert);
            keyMapper.Add(keys[11], Keys.Back);
            keyMapper.Add(keys[12], Keys.Enter);
            keyMapper.Add(keys[13], Keys.Tab);
            keyMapper.Add(keys[14], Keys.Escape);
            keyMapper.Add(keys[15], Keys.Space);
            keyMapper.Add(keys[16], Keys.F1);
            keyMapper.Add(keys[17], Keys.F2);
            keyMapper.Add(keys[18], Keys.F3);
            keyMapper.Add(keys[19], Keys.F4);
            keyMapper.Add(keys[20], Keys.F5);
            keyMapper.Add(keys[21], Keys.F6);
            keyMapper.Add(keys[22], Keys.F7);
            keyMapper.Add(keys[23], Keys.F8);
            keyMapper.Add(keys[24], Keys.F9);
            keyMapper.Add(keys[25], Keys.F10);
            keyMapper.Add(keys[26], Keys.F11);
            keyMapper.Add(keys[27], Keys.F12);
            keyMapper.Add(keys[28], Keys.D0);
            keyMapper.Add(keys[29], Keys.D1);
            keyMapper.Add(keys[30], Keys.D2);
            keyMapper.Add(keys[31], Keys.D3);
            keyMapper.Add(keys[32], Keys.D4);
            keyMapper.Add(keys[33], Keys.D5);
            keyMapper.Add(keys[34], Keys.D6);
            keyMapper.Add(keys[35], Keys.D7);
            keyMapper.Add(keys[36], Keys.D8);
            keyMapper.Add(keys[37], Keys.D9);
            keyMapper.Add(keys[38], Keys.A);
            keyMapper.Add(keys[39], Keys.B);
            keyMapper.Add(keys[40], Keys.C);
            keyMapper.Add(keys[41], Keys.D);
            keyMapper.Add(keys[42], Keys.E);
            keyMapper.Add(keys[43], Keys.F);
            keyMapper.Add(keys[44], Keys.G);
            keyMapper.Add(keys[45], Keys.H);
            keyMapper.Add(keys[46], Keys.I);
            keyMapper.Add(keys[47], Keys.J);
            keyMapper.Add(keys[48], Keys.K);
            keyMapper.Add(keys[49], Keys.L);
            keyMapper.Add(keys[50], Keys.M);
            keyMapper.Add(keys[51], Keys.N);
            keyMapper.Add(keys[52], Keys.O);
            keyMapper.Add(keys[53], Keys.P);
            keyMapper.Add(keys[54], Keys.Q);
            keyMapper.Add(keys[55], Keys.R);
            keyMapper.Add(keys[56], Keys.S);
            keyMapper.Add(keys[57], Keys.T);
            keyMapper.Add(keys[58], Keys.U);
            keyMapper.Add(keys[59], Keys.V);
            keyMapper.Add(keys[60], Keys.W);
            keyMapper.Add(keys[61], Keys.X);
            keyMapper.Add(keys[62], Keys.Y);
            keyMapper.Add(keys[63], Keys.Z);
        }

        public static int GetKeyIndex(String s, String def)
        {
            int i;
            if (s.Length <= 0)
            {
                s = def;
                if (s.Length <= 0) return 0;
            }

            for (i = 0; i < keys.Length; i++)
                if (s.Equals(keys[i]))
                    return i;

            return 0;
        }


        public void SetConductor(ref Conductor c)
        {
            conductor = c;
        }

        public void ProcessKey(KeyEventArgs e, bool mode)
        {
            if (conductor == null) return;

            Keys keyCode = e.KeyCode;
            Keys value;
            
            keyMapper.TryGetValue((string)comboBoxKey1.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL1.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT1.Checked)|| ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT1.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue1.BackColor = mode?Color.Green:Color.Red;

                            if ((mode) || (checkBoxRevert1.Checked))
                                conductor.Set((string)comboBoxAction1.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey2.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL2.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT2.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT2.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue2.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert2.Checked))
                                conductor.Set((string)comboBoxAction2.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey3.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL3.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT3.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT3.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue3.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert3.Checked))
                                conductor.Set((string)comboBoxAction3.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey4.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL4.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT4.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT4.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue4.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert4.Checked))
                                conductor.Set((string)comboBoxAction4.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey5.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL5.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT5.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT5.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue5.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert5.Checked))
                                conductor.Set((string)comboBoxAction5.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey6.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL6.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT6.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT6.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue6.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert6.Checked))
                                conductor.Set((string)comboBoxAction6.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey7.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL7.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT7.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT7.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue7.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert7.Checked))
                                conductor.Set((string)comboBoxAction7.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey8.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL8.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT8.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT8.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue8.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert8.Checked))
                                conductor.Set((string)comboBoxAction8.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey9.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL9.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT9.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT9.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue9.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert9.Checked))
                                conductor.Set((string)comboBoxAction9.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey10.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL10.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT10.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT10.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue10.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert10.Checked))
                                conductor.Set((string)comboBoxAction10.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey11.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL11.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT11.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT11.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue11.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert11.Checked))
                                conductor.Set((string)comboBoxAction11.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey12.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL12.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT12.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT12.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue12.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert12.Checked))
                                conductor.Set((string)comboBoxAction12.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey13.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL13.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT13.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT13.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue13.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert13.Checked))
                                conductor.Set((string)comboBoxAction13.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey14.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL14.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT14.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT14.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue14.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert14.Checked))
                                conductor.Set((string)comboBoxAction14.SelectedItem, mode);
                        }

            keyMapper.TryGetValue((string)comboBoxKey15.SelectedItem, out value);
            if (keyCode == value)
                if ((!checkBoxCTRL15.Checked) || ((Control.ModifierKeys & Keys.Control) != 0))
                    if ((!checkBoxALT15.Checked) || ((Control.ModifierKeys & Keys.Alt) != 0))
                        if ((!checkBoxSHIFT15.Checked) || ((Control.ModifierKeys & Keys.Shift) != 0))
                        {
                            e.SuppressKeyPress = true;

                            labelValue15.BackColor = mode ? Color.Green : Color.Red;

                            if ((mode) || (checkBoxRevert15.Checked))
                                conductor.Set((string)comboBoxAction15.SelectedItem, mode);
                        }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Keys modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_1 = comboBoxKey1.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_1 = comboBoxAction1.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_1 = checkBoxRevert1.Checked;
            if (checkBoxCTRL1.Checked) modifiers |= Keys.Control;
            if (checkBoxALT1.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT1.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_1 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_2 = comboBoxKey2.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_2 = comboBoxAction2.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_2 = checkBoxRevert2.Checked;
            if (checkBoxCTRL2.Checked) modifiers |= Keys.Control;
            if (checkBoxALT2.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT2.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_2 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_3 = comboBoxKey3.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_3 = comboBoxAction3.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_3 = checkBoxRevert3.Checked;
            if (checkBoxCTRL3.Checked) modifiers |= Keys.Control;
            if (checkBoxALT3.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT3.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_3 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_4 = comboBoxKey4.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_4 = comboBoxAction4.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_4 = checkBoxRevert4.Checked;
            if (checkBoxCTRL4.Checked) modifiers |= Keys.Control;
            if (checkBoxALT4.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT4.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_4 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_5 = comboBoxKey5.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_5 = comboBoxAction5.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_5 = checkBoxRevert5.Checked;
            if (checkBoxCTRL5.Checked) modifiers |= Keys.Control;
            if (checkBoxALT5.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT5.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_5 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_6 = comboBoxKey6.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_6 = comboBoxAction6.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_6 = checkBoxRevert6.Checked;
            if (checkBoxCTRL6.Checked) modifiers |= Keys.Control;
            if (checkBoxALT6.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT6.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_6 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_7 = comboBoxKey7.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_7 = comboBoxAction7.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_7 = checkBoxRevert7.Checked;
            if (checkBoxCTRL7.Checked) modifiers |= Keys.Control;
            if (checkBoxALT7.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT7.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_7 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_8 = comboBoxKey8.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_8 = comboBoxAction8.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_8 = checkBoxRevert8.Checked;
            if (checkBoxCTRL8.Checked) modifiers |= Keys.Control;
            if (checkBoxALT8.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT8.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_8 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_9 = comboBoxKey9.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_9 = comboBoxAction9.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_9 = checkBoxRevert9.Checked;
            if (checkBoxCTRL9.Checked) modifiers |= Keys.Control;
            if (checkBoxALT9.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT9.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_9 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_10 = comboBoxKey10.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_10 = comboBoxAction10.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_10 = checkBoxRevert10.Checked;
            if (checkBoxCTRL10.Checked) modifiers |= Keys.Control;
            if (checkBoxALT10.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT10.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_10 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_11 = comboBoxKey11.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_11 = comboBoxAction11.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_11 = checkBoxRevert11.Checked;
            if (checkBoxCTRL11.Checked) modifiers |= Keys.Control;
            if (checkBoxALT11.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT11.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_11 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_12 = comboBoxKey12.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_12 = comboBoxAction12.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_12 = checkBoxRevert12.Checked;
            if (checkBoxCTRL12.Checked) modifiers |= Keys.Control;
            if (checkBoxALT12.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT12.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_12 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_13 = comboBoxKey13.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_13 = comboBoxAction13.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_13 = checkBoxRevert13.Checked;
            if (checkBoxCTRL13.Checked) modifiers |= Keys.Control;
            if (checkBoxALT13.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT13.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_13 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_14 = comboBoxKey14.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_14 = comboBoxAction14.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_14 = checkBoxRevert14.Checked;
            if (checkBoxCTRL14.Checked) modifiers |= Keys.Control;
            if (checkBoxALT14.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT14.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_14 = (int)modifiers;

            modifiers = 0;
            Fritz.Properties.Settings.Default.Keyboard_Key_15 = comboBoxKey15.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Action_15 = comboBoxAction15.SelectedItem.ToString();
            Fritz.Properties.Settings.Default.Keyboard_Revert_15 = checkBoxRevert15.Checked;
            if (checkBoxCTRL15.Checked) modifiers |= Keys.Control;
            if (checkBoxALT15.Checked) modifiers |= Keys.Alt;
            if (checkBoxSHIFT15.Checked) modifiers |= Keys.Shift;
            Fritz.Properties.Settings.Default.Keyboard_Modifiers_15 = (int)modifiers;

            Fritz.Properties.Settings.Default.Save();
            this.Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void KeyboardConfiguration_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKey(e, true);
        }

        private void KeyboardConfiguration_KeyUp(object sender, KeyEventArgs e)
        {
            ProcessKey(e, false);
        }
    }
}
