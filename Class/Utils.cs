﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnswerSheetReader.Class
{
    public static class Utils
    {
        public static string toReadableString(long millis)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(millis);
            string answer = string.Format("{0:D2} h {1:D2} m {2:D2} s {3:D3} ms",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds,
                                    t.Milliseconds);
            return answer;
        }
    }
}
