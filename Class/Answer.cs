using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AnswerSheetReader.Class
{
    public class Answer
    {
        public int hIndex { get; set; }
        public int vIndex { get; set; }
        public Rectangle rect { get; set; }

        public Answer(int vIndex, int hIndex)
        {
            this.vIndex = vIndex;
            this.hIndex = hIndex;
        }
    }
}
