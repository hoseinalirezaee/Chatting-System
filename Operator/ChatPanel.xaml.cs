using MessageTransfer.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using OperatorImp;
using MessageTransfer;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace Operator
{
    
    public delegate void AddTextMessage(TextMessage textMessage);
    public delegate void AddFileMessage(FileMessage fileMessage);

    public partial class ChatPanel : UserControl
    {
        private string ByteTo(long @byte)
        {
            if (@byte < 1024)
            {
                return @byte + "B";
            }
            else if (@byte < 1024 * 1024)
            {
                return (@byte / 1024.0).ToString("0.00") + " KB";
            }

            return (@byte / (1024 * 1024.0)).ToString("0.00") + " MB";
        }


        private ObservableCollection<FrameworkElement> chatContent = new ObservableCollection<FrameworkElement>();
        private OperatorImp.Operator @operator = null;
        private EndPoint serverEndPoint = new EndPoint { IP = "127.0.0.1", Port = 1111 };

        public ChatPanel()
        {
            InitializeComponent();
            chatCon.ItemsSource = chatContent;
        }

        public void AddFileMessage(FileMessage fileMessage)
        {
            Button button = new Button();
            button.Content = fileMessage.FileName + " (" + ByteTo(fileMessage.Size) + ")";
            button.Tag = fileMessage.ID;
            button.Click += RecevieFileBtnClicked;
            chatContent.Add(button);
        }

        private void RecevieFileBtnClicked(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var id = (int)btn.Tag;
            FileInfo file;
            if ((file = @operator.GetFile(id)) == null)
            {
                @operator.RecevieFile(id, @"C:\Users\Hosein\Desktop");
            }

            if (file != null)
            {
                string path = file.FullName;
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = path
                };
                Process process = new Process();
                process.StartInfo = psi;
                process.Start();
            }
        }

        public void AddTextMessage(TextMessage textMessage)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = textMessage.Text;
            chatContent.Add(textBlock);
        }

        private void SendButtonClicked(object sender, RoutedEventArgs e)
        {
            if (textBox.Text != String.Empty)
            {
                TextMessage textMessage = new TextMessage
                {
                    Text = textBox.Text,
                    DateTime = DateTime.Now,
                    Direction = TextDirection.LeftToRight
                };
                textBox.Text = String.Empty;
                @operator.SendMessage(textMessage);
                AddTextMessage(textMessage);
            }
        }

        private void SendFileButtonClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                FileInfo file = new FileInfo(fileDialog.FileName);
                @operator.SendFile(file);
                var id = @operator.GetFileID(file);
                FileMessage fileMessage = new FileMessage
                {
                    FileName = file.Name,
                    ID = id,
                    Size = file.Length
                };
                AddFileMessage(fileMessage);
            }
        }

        public void Connect(int id)
        {
            @operator = new OperatorImp.Operator(id);
            @operator.MessageReceived += User_MessageReceived;
            @operator.ConnectToUser(serverEndPoint);
        }

        private void User_MessageReceived(Message message)
        {
            if (message is TextMessage)
            {
                AddTextMessage addTextMessageDele = AddTextMessage;
                Dispatcher.Invoke(addTextMessageDele, message as TextMessage);
            }
            else if (message is FileMessage)
            {
                AddFileMessage addFileMessageDele = AddFileMessage;
                Dispatcher.Invoke(addFileMessageDele, message as FileMessage);
            }
        }
    }
}
