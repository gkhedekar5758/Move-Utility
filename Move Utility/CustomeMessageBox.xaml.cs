using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Move_Utility
{
    /// <summary>
    /// Interaction logic for CustomeMessageBox.xaml
    /// </summary>
    public partial class CustomeMessageBox : Window
    {
        

        public string Message
        {
            get { return TextBlock_Message.Text; }
            set { TextBlock_Message.Text = value; }
        }


        
        public string YesButtonText
        {
            get { return Label_Yes.Content.ToString(); }
            set { Label_Yes.Content = value; }
        }
        public string NoButtonText
        {
            get { return Label_No.Content.ToString(); }
            set { Label_No.Content = value; }
        }

        public MessageBoxResult Result { get; set; }

        public CustomeMessageBox()
        {
            InitializeComponent();
        }
        public CustomeMessageBox(string _message)
        {
            InitializeComponent();
            Message = _message;
            //Image_MessageBox.Visibility = System.Windows.Visibility.Collapsed;
            
        }

        private void Button_No_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }

        private void Button_Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }
    }
}
