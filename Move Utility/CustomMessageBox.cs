using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Move_Utility
{
    static class CustomMessageBox
    {
        /// <summary>
        /// Displays a message box that has a message, caption, and Yes/No buttons with custom System.String values for the buttons' text;
        /// and that returns a result.
        /// </summary>
        /// <param name="messageBox">A System.String that specifies the text to display</param>
        /// <param name="yesButtonText">A System.String that specifies the text to display within the Yes button.</param>
        /// <param name="noButtonText">A System.String that specifies the text to display within the No button.</param>
        /// <returns></returns>
        public static MessageBoxResult ShowYesNo(string messageBox,string yesButtonText,string noButtonText)
        {
            CustomeMessageBox msg = new CustomeMessageBox(messageBox);
            //{
            msg.YesButtonText = yesButtonText;
            msg.NoButtonText = noButtonText;
            //};
            msg.ShowDialog();
            return msg.Result;
        }
    }
}
