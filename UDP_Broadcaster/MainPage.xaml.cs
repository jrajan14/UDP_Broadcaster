using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UDP_Broadcaster
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int PortNo = 50000;     //Default Port Number
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Btn_SetPort_Click(object sender, RoutedEventArgs e)
        {
            if (Txt_PortNo.Text == "")
            {
                var dialog = new MessageDialog("Enter a Numeric Port number");
                await dialog.ShowAsync();
            }
            else
            {
                PortNo = Int32.Parse(Txt_PortNo.Text);
            }
        }

        private async void Btn_AddIP_Click(object sender, RoutedEventArgs e)
        {
            int validate = Validate_IP_Address(Txt_AddIP.Text);

            if(validate == 0)
            {
                Display_Logs("Unable to validate given IP address");
            }
            else
            {
                if (Txt_AddIP.Text == "")
                {
                    var dialog = new MessageDialog("Enter a Numeric Port number");
                    await dialog.ShowAsync();
                }
                else
                {
                    List_IPAddresses.Items.Add(Txt_AddIP.Text);
                    /*//FOR SINGLE IP VALIDATION
                     
                    if (List_IPAddresses.Items.Contains(Txt_AddIP.Text))
                    {
                        Display_Logs("IP address already in list");
                    }
                    else
                    {
                        List_IPAddresses.Items.Add(Txt_AddIP.Text);

                    }*/
                }
            }
        }

        private void Btn_DeleteIP_Click(object sender, RoutedEventArgs e)
        {
            while (List_IPAddresses.SelectedItems.Count > 0) {
                Display_Logs(List_IPAddresses.SelectedItem + " Deleted");
                //List_IPAddresses.Items.Remove(List_IPAddresses.SelectedItem);
                List_IPAddresses.Items.RemoveAt(List_IPAddresses.Items.IndexOf(List_IPAddresses.SelectedItem));
                
            }

        }
                
        private void Btn_AddRange_Click(object sender, RoutedEventArgs e)
        {
            string startIP = Txt_StartRangeIP.Text;
            string endIP = Txt_EndRangeIP.Text;

            string[] sub_startIP = startIP.Split('.');
            string[] sub_endIP = endIP.Split(".");

            int S_IP_0 = Int32.Parse(sub_startIP[0]);
            int S_IP_1 = Int32.Parse(sub_startIP[1]);
            int S_IP_2 = Int32.Parse(sub_startIP[2]);
            int S_IP_3 = Int32.Parse(sub_startIP[3]);
            
            int E_IP_0 = Int32.Parse(sub_endIP[0]);
            int E_IP_1 = Int32.Parse(sub_endIP[1]);
            int E_IP_2 = Int32.Parse(sub_endIP[2]);
            int E_IP_3 = Int32.Parse(sub_endIP[3]);

            string IPtoAdd;

            for (int i=S_IP_0; i<=E_IP_0; i++) 
            {
                for(int j=S_IP_1; j<=E_IP_1; j++) 
                { 
                    for(int k=S_IP_2; k<=E_IP_2; k++)
                    {
                        for(int l=S_IP_3; l<=E_IP_3; l++) 
                        {
                            IPtoAdd = i.ToString() + "." + j.ToString() + "." + k.ToString() + "." +l.ToString();
                            List_IPAddresses.Items.Add(IPtoAdd);
                        }
                    }
                }
            }
        }

        //VALIDATE IP ADDRESS 
        int Validate_IP_Address(string IP_address)
        {
            string[] sub_IP = IP_address.Split('.');
            if(sub_IP.Length != 4)
            {
                Display_Logs("INVALID IP ADDRESS : IPv4 address much contain only 4 octets");
                return 0;
            }

            foreach (var str in sub_IP)
            {
                //TODO:Check for Alphabets
            }
            
            int octet_0 = Int32.Parse(sub_IP[0]);
            int octet_1 = Int32.Parse(sub_IP[1]);
            int octet_2 = Int32.Parse(sub_IP[2]);
            int octet_3 = Int32.Parse(sub_IP[3]);

            if(octet_0 == 0)
            {
                Display_Logs("Please use higher range. ie: First octet greater than 0");
                return 0;
            }
            
            if (octet_0 < 0 || octet_1 < 0 || octet_2 < 0 || octet_3 < 0)
            {
                Display_Logs("Octets cannot be lower than 0");
                return 0;
            }


            if (octet_0 > 255 || octet_1 > 255 || octet_2 > 255 || octet_3 > 255) 
            {
                Display_Logs("Octets cannot be higher than 255");
                return 0;
            }

            return 1;
        }
                
        private void Btn_Send_Click(object sender, RoutedEventArgs e)
        {
            if(Txt_SendMessage.Text == "")
            {
                Display_Logs("ERROR: Message Empty \nWrite a message to send");
            }
            else
            {
                foreach (string IPtoBroadcast in List_IPAddresses.Items)
                {
                    string command = Txt_SendMessage.Text;
                    byte[] PacketCommand = System.Text.ASCIIEncoding.ASCII.GetBytes(command);
                    int portToSend = PortNo;

                    IPEndPoint epCommand = new IPEndPoint(IPAddress.Parse(IPtoBroadcast), portToSend);
                    Socket BroadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    BroadcastSocket.SendTo(PacketCommand, epCommand);
                }

            }
        }

        private void Btn_Receive_Click(object sender, RoutedEventArgs e)
        {

        }

        //CUSTOM LOG FUNCTION
        void Display_Logs(string log_message)
        {
            Txt_Logs.Text += "\n" + log_message;
        }
    }
}
