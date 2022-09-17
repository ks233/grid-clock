using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grid_clock
{
    public partial class Form1 : Form
    {

        private Button[] hourButtons = new Button[24];
        private Image[] progImages = new Image[24];
        private Image[] alarmImages = new Image[24];



        private string[] notes = new string[24];
        // 避免鼠标悬停预览时触发textChange
        private bool saveNote = true;

        private int h, m, s;


        private int buttonHeight = 40;
        private int buttonWidth = 90;

        // 整体按钮位置偏移
        private int gBtnOffsetX = 20;
        private int gBtnOffsetY = 20;

        // Timer闪烁
        private bool timerFlag = false;
        // 测试用DateTime
        private DateTime dateTest;

        // Timer更新
        private int lastUpdatedHour = 23;

        // 选中的按钮
        private int selected = 0;


        

        public Form1()
        {
            InitializeComponent();
        }



        private void NewButton(int i,int x,int y,int height,int width)
        {
            this.Controls.Add(hourButtons[i]);
            hourButtons[i].Text = i.ToString();

            hourButtons[i].Font = new Font(new FontFamily("Comic Sans MS"), 16);

            hourButtons[i].Location = new Point(x + gBtnOffsetX, y +gBtnOffsetY);
            hourButtons[i].Size = new Size(height, width);
            hourButtons[i].FlatStyle = FlatStyle.Flat;
            hourButtons[i].FlatAppearance.BorderSize = 1;
            hourButtons[i].FlatAppearance.BorderColor = Color.Black;
            hourButtons[i].Click += new EventHandler((sender, e) => HourBtn_Click(sender, e, i));
            hourButtons[i].MouseEnter += new EventHandler((sender, e) => HourBtn_MouseEnter(sender, e, i));
            hourButtons[i].MouseLeave += new EventHandler((sender, e) => HourBtn_MouseLeave(sender, e, i));

        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            ColorPalette.dark = false;
            dateTest = new DateTime();
            dateTest = dateTest.AddHours(10);

            Alarm.Initialize();

            // 实例化24个按钮
            for (int i = 0; i < 24; i++)
            {
                hourButtons[i] = new Button();
                progImages[i] = new Bitmap(buttonWidth,buttonHeight);
                alarmImages[i] = new Bitmap(buttonWidth, buttonHeight);
                notes[i] = "";
            }
            // 画出24个按钮
            // 夜间
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    NewButton((i * 4 + j + 23)%24,  j * buttonWidth, i * buttonHeight , buttonWidth , buttonHeight);
                }
            }
            // 上午7~12
            int yOffset = 2 * buttonHeight + 20;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewButton(i * 3 + j + 7, j * buttonWidth , i * buttonHeight + yOffset, buttonWidth, buttonHeight);
                }
            }
            // 下午13~18
            yOffset *= 2;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewButton(i * 3 + j + 13, j * buttonWidth, i * buttonHeight + yOffset, buttonWidth, buttonHeight);
                }
            }

            yOffset  += 2*buttonHeight+ 20; 
            // 晚上19~22
            for (int j = 0; j < 4; j++)
            {
                NewButton(19 + j, j * buttonWidth, yOffset, buttonWidth, buttonHeight);
            }

            // this.Height = yOffset + buttonHeight + gBtnOffsetY * 2 +32;

            // noteTextBox.Location = new Point(gBtnOffsetX * 2 + 4 * buttonWidth, gBtnOffsetY);
            // noteTextBox.Size = new Size(this.Width - noteTextBox.Left - gBtnOffsetX * 2, this.Height - 200);
            SelectIndex(DateTime.Now.Hour);
            ApplyTheme();
            GC.Collect();
        }

        private void UpdateButtonText()
        {
            for(int i = 0;i < 24; i++)
            {
                if (selected == i)
                {
                    hourButtons[i].Text = String.Format("< {0} >", i.ToString());
                    continue;
                }

                hourButtons[i].Text = i.ToString();
                if (notes[i].Trim().Length > 0)
                {
                    hourButtons[i].Text += "*";
                }
                
            }
        }

        private void FillBtnColor(int hour)
        {
            for (int i = 0; i < 24; i++)
            {
                DrawProgImg(i, 0, ThemeColor(i));
            }
            if (hour == 23)
            {
                return;
            }
            else
            {
                DrawProgImg(23, 1, ThemeColor(23));
                for (int i = 0; i < hour; i++)
                {
                    DrawProgImg(i, 1, ThemeColor(i));
                }
            }
        }

        // 把按钮做出进度条的效果
        private void DrawProgImg(int hour,float progress, Color color)
        {
            // 生成固定大小的纯色图像
            Bitmap bg = new Bitmap(buttonWidth, buttonHeight);
            using (Graphics gfx = Graphics.FromImage(bg))
            using (SolidBrush brush = new SolidBrush(color))
            {
                gfx.FillRectangle(brush, 0, 0, buttonWidth * progress, buttonHeight);
            }
            progImages[hour] = bg;
            MixButtonImage(hour);
        }


        private void DrawAlarmImg(int hour)
        {
            Bitmap bg = new Bitmap(buttonWidth, buttonHeight);
            using (Graphics gfx = Graphics.FromImage(bg))
            using (Pen pen = new Pen(Color.Red,2))
            {
                foreach(Alarm a in Alarm.AlarmList[hour])
                {
                    int v = buttonWidth * a.minute / 60;
                    gfx.DrawLine(pen, v, 0, v, buttonHeight);
                }
            }
            alarmImages[hour] = bg;
            MixButtonImage(hour);
        }

        /*
         * 两个用来调色的Lerp函数，现在用不到了
        private float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }
        private Color Lerp(Color a, Color b, float by)
        {
            return Color.FromArgb((int)Lerp(a.R, b.R, by), (int)Lerp(a.G, b.G, by), (int)Lerp(a.B, b.B, by));
        }
        */


        private void HourBtn_Click(object sender, EventArgs e, int index)
        {
            SelectIndex(index);
        }
        private void HourBtn_MouseEnter(object sender, EventArgs e, int index)
        {
            if (notes[index].Trim().Length > 0)
            {
                saveNote = false;
                noteTextBox.Text = notes[index];
            }
        }
        private void HourBtn_MouseLeave(object sender, EventArgs e, int index)
        {
            noteTextBox.Text = notes[selected];
            saveNote = true;
        }
        // 选中按钮
        private void SelectIndex(int index)
        {
            selected = index;
            noteTextBox.Text = notes[index];
            UpdateButtonText();
        }
        private Color ThemeColor(int i)
        {
            // return Color.FromArgb(255, 144, 88);
            if(i == 23 || (i >= 0 && i <= 6)) 
            {
                return ColorPalette.night;
            }
            else if (i >= 7 && i <= 12)
            {
                return ColorPalette.morning;
            }
            else if (i >=13 && i <= 18)
            {
                return ColorPalette.afternoon;
            }
            else
            {
                return ColorPalette.evening;
            }
        }

        private void timeLabel_Click(object sender, EventArgs e)
        {
            ShowAlarmMessage(new Alarm());
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            ColorPalette.dark = !ColorPalette.dark; // 如果不是深色模式
            ApplyTheme();
        }
        private void ApplyTheme()
        {
            timeLabel.ForeColor = ColorPalette.TextColor;
            noteTextBox.ForeColor = ColorPalette.TextColor;
            noteTextBox.BackColor = ColorPalette.TextBoxColor;
            this.BackColor = ColorPalette.BackgroundColor;
            foreach (Button b in hourButtons)
            {
                b.BackColor = ColorPalette.ButtonColor;
                FillBtnColor(DateTime.Now.Hour);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now; 

            // dateTest = dateTest.AddMinutes(1);
            // DateTime dateTime = dateTest; 

            this.Text = dateTime.ToString();


            h = dateTime.Hour;
            if (m != dateTime.Minute)
            {
                m = dateTime.Minute;
                RingAlarm();
            }
            s = dateTime.Second;

            float progress = (m * 60 + s) / 3600.0f;
            
            // 如果h和缓存的lastUpdatedHour不一致，就要更新前面的按钮样式
            if (h != lastUpdatedHour)
            {
                FillBtnColor(h);
                lastUpdatedHour = h;
                UpdateButtonText();
            }
            timeLabel.Text = DateTime.Now.ToString("HH:mm:ss");
            DrawProgImg(h, progress, ThemeColor(h));
        }

        private void timerGC_Tick(object sender, EventArgs e)
        {

            GC.Collect();
        }

        private void MixButtonImage(int h)
        {
            Bitmap mixed = new Bitmap(buttonWidth,buttonHeight);
            using (Graphics g = Graphics.FromImage(mixed))
            {
                g.DrawImage(progImages[h], 0, 0);
                g.DrawImage(alarmImages[h], 0, 0);
            }
            hourButtons[h].BackgroundImage = mixed;
        }

        private void RingAlarm()
        {
            Alarm alarm = Alarm.GetAlarm(h, m);
            if (alarm != null)
            {
                ShowAlarmMessage(alarm);
            }
        }

        private void noteTextBox_TextChanged(object sender, EventArgs e)
        {
            if (saveNote)
            {
                notes[selected] = noteTextBox.Text;
                Alarm.UpdateAlarmList(notes, selected);
                DrawAlarmImg(selected);
            }
        }
        private void ShowAlarmMessage(Alarm alarm)
        {
            // 窗口前置
            if (this.WindowState==FormWindowState.Minimized)this.WindowState = FormWindowState.Normal; 
            this.Activate();

            // 播放声音
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"alarm.wav");
            player.Play();

            // 弹窗
            MessageBox.Show($"{alarm.description}", "闹钟", MessageBoxButtons.OK, MessageBoxIcon.None);
        }
    }
}
