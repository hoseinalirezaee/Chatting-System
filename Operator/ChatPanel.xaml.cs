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
    
    public delegate void AddTextMessage(TextMessage textMessage, string ownerName);
    public delegate void AddFileMessage(FileMessage fileMessage, string ownerName);
    public delegate void SetUserName(string name);

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

        private string userName = "";
        public void SetUserName(string name)
        {
            this.userName = name;
        }

        public void Init()
        {
            waitText.Text = "";
            messagePanel.IsEnabled = true;
            TextBlock textBlock = new TextBlock();
            textBlock.Text = this.userName + " joied the chat.";
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.Children.Add(textBlock);
            chatContent.Add(stackPanel);
        }

        private ObservableCollection<FrameworkElement> chatContent = new ObservableCollection<FrameworkElement>();
        private OperatorImp.Operator @operator = null;
        private EndPoint serverEndPoint = new EndPoint { IP = "127.0.0.1", Port = 1111 };

        public ChatPanel()
        {
            InitializeComponent();
            chatCon.ItemsSource = chatContent;
        }

        public void AddFileMessage(FileMessage fileMessage, string ownerName)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.Add(new Run(ownerName + ": ") { Foreground = new SolidColorBrush(Colors.Red)});
            stackPanel.Children.Add(textBlock);

            Button button = new Button();
            button.Content = fileMessage.FileName + " (" + ByteTo(fileMessage.Size) + ")";
            button.Tag = fileMessage.ID;
            button.Click += RecevieFileBtnClicked;

            textBlock.VerticalAlignment = VerticalAlignment.Center;
            button.VerticalAlignment = VerticalAlignment.Center;
            stackPanel.Children.Add(button);
            chatContent.Add(stackPanel);
        }
        public void AddTextMessage(TextMessage textMessage, string ownerName)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.VerticalAlignment = VerticalAlignment.Center;
            TextBlock tbName = new TextBlock();
            tbName.Inlines.Add(new Run(ownerName + ": ") { Foreground = new SolidColorBrush(Colors.Red) });
            stackPanel.Children.Add(tbName);

            TextBlock textBlock = new TextBlock();
            textBlock.Text = textMessage.Text;

            textBlock.VerticalAlignment = VerticalAlignment.Center;
            tbName.VerticalAlignment = VerticalAlignment.Center;

            stackPanel.Children.Add(textBlock);
            chatContent.Add(stackPanel);
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
                AddTextMessage(textMessage, @operator.Name);
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
                AddFileMessage(fileMessage, @operator.Name);
            }
        }

        public void Connect(int id, string name)
        {
            @operator = new OperatorImp.Operator(id);
            @operator.Name = name;
            @operator.MessageReceived += User_MessageReceived;
            @operator.ConnectToUser(serverEndPoint);
        }

        private void User_MessageReceived(Message message)
        {
            if (message is Introduction)
            {
                SetUserName setUserName = SetUserName;
                Dispatcher.Invoke(setUserName, (message as Introduction).Name);
                Dispatcher.Invoke(Init);
            }
            else if (message is TextMessage)
            {
                AddTextMessage addTextMessageDele = AddTextMessage;
                Dispatcher.Invoke(addTextMessageDele, message as TextMessage, userName);
            }
            else if (message is FileMessage)
            {
                AddFileMessage addFileMessageDele = AddFileMessage;
                Dispatcher.Invoke(addFileMessageDele, message as FileMessage, userName);
            }
        }
    }
}
