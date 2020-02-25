using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TeamClient
{
    public class TimerTextItem
    {
        int _leftMargin;

        public string TimerText
        {
            get; private set;
        }

        public Thickness Margin
        {
            get
            {
                return new Thickness(_leftMargin, 0, 0, 0);
            }
        }

        public TimerTextItem(string timerText, int leftMargin)
        {
            TimerText = timerText;
            _leftMargin = leftMargin;
        }
    }
}
