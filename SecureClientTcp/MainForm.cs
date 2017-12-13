using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SecureClientTcp
{
    public partial class MainForm : Form
    {
        Client client;

        ManualResetEvent _event;

        bool canSendMessage;

        public MainForm()
        {
            InitializeComponent();
            _event = new ManualResetEvent(false);
            canSendMessage = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void regButton_Click(object sender, EventArgs e)
        {
            client = new Client(textBox1.Text, Convert.ToInt32(textBox2.Text));
           
            client.ClientRegistredHandlerListForUI += RegistrationAnswer;
            client.ClientAuthorizedHandlersListForUI += AutAnswer;
            client.ClientDisconnectHandlerListForUI += Stop;
            client.ClientMessageHandlerListForUI += NewMessage;
            client.Registration(loginTextBox.Text, passwordTextBox.Text);
            _event.WaitOne();
            if (canSendMessage)
            {
                ClientReady();
            }
            else
            {
                ClientChanged();
            }
            
        }

        public void RegistrationAnswer(bool result)
        {
            if (result)
            {
                MessageBox.Show("Регитрация", "успех", MessageBoxButtons.OK);
                canSendMessage = true;
                _event.Set();
            }
            else
            {
                MessageBox.Show("Регитрация", "уже существует пользователь", MessageBoxButtons.OK);
                canSendMessage = false;
                _event.Set();
                ClientChanged();
            }

        }

        public void AutAnswer(bool result)
        {
            if (result)
            {
                MessageBox.Show("успех","Аутентификация", MessageBoxButtons.OK);
                canSendMessage = true;
                _event.Set();
            }
            else
            {
                MessageBox.Show("отказано","Аутентификация", MessageBoxButtons.OK);
                canSendMessage = false;
                _event.Set();
            }
        }

        public void Stop()
        {
            MessageBox.Show("Прервано!","Соединение" , MessageBoxButtons.OK);
        }

        public void NewMessage(string sender,string message)
        {
            chatTextBox.Invoke(new Action(() => { chatTextBox.AppendText("user " + sender + " : " + message+Environment.NewLine);}));
             
        }

        private void ClientReady()
        {
            chatTextBox.Visible = true;
            label7.Visible = true;
            sendButton.Visible = true;
            messageTextBox.Visible = true;
        }

        private void ClientChanged()
        {
            chatTextBox.Visible = false;
            label7.Visible = false;
            sendButton.Visible = false;
            messageTextBox.Visible = false;
        }

        private void autoButton_Click(object sender, EventArgs e)
        {
            client = new Client(textBox1.Text, Convert.ToInt32(textBox2.Text));

            client.ClientRegistredHandlerListForUI += RegistrationAnswer;
            client.ClientAuthorizedHandlersListForUI += AutAnswer;
            client.ClientDisconnectHandlerListForUI += Stop;
            client.ClientMessageHandlerListForUI += NewMessage;
            client.Autentification(loginTextBox.Text, passwordTextBox.Text);
            _event.WaitOne();
            if (canSendMessage)
            {
                ClientReady();
            }
            else
            {
                ClientChanged();
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (canSendMessage)
            {
                client.SendMessage(loginTextBox.Text, messageTextBox.Text);
            }
        }
    }
}
