using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        public static int getRectangleArea(Rectangle r)
        {
            return r.Width * r.Height;
        }

        public static string GetApplicantID(List<Answer> answerList)
        {
            string result = "";
            answerList = answerList.OrderBy(ans => ans.hIndex).ToList();

            // applicant id  index : 29=>36
            for (int i = 29; i < 37; i++)
            {
                int count = answerList.Where(answer => answer.hIndex.Equals(i)).Count();
                if (count == 1)
                {
                    result += ( answerList.Find(answer => answer.hIndex.Equals(i)).vIndex - 1).ToString();
                }
                else
                {
                    result += "X";
                }
            }
            return result;
        }
    }
}
