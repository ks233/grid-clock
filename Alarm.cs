using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace grid_clock
{
    internal class Alarm
    {
        public int hour, minute;
        public string description;

        // TODO: 读取notes中的闹钟
        public static List<Alarm>[] AlarmList = new List<Alarm>[24];

        public static void Initialize()
        {
            for(int i = 0;i < AlarmList.Length; i++)
            {
                AlarmList[i] = new List<Alarm>();
            }
        }

        public static void UpdateAlarmList(string[] notes, int h)
        {
            if (h > 23 || h < 0) return;
            AlarmList[h].Clear();
            string[] stringSeparators = new string[] { "\r\n" };
            string[] lines = notes[h].Split(stringSeparators, StringSplitOptions.None); // 逐行切分
            foreach (string line in lines)
            {
                Alarm alarm = new Alarm();
                Regex filter = new Regex(@"^[:|：]([0-5][0-9]) (..*)$"); // 匹配 :<时间> <内容>的格式
                if (filter.IsMatch(line))
                {
                    Match match = filter.Match(line);
                    alarm.hour = h;
                    alarm.minute = Convert.ToInt32(match.Groups[1].Value);
                    alarm.description = match.Groups[2].Value;
                    AlarmList[h].Add(alarm);
                }
            }
        }


    }
}
