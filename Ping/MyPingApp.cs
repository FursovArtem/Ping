using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Diagnostics;

namespace Ping
{
    public partial class MyPingApp : Form
    {
        static int count = 0;
        List<IPAddress> currentIPs = new List<IPAddress>();
        List<RichTextBox> rtbCurrent = new List<RichTextBox>();
        List<RichTextBox> rtbOutput = new List<RichTextBox>();
        List<Button> btnDisconnect = new List<Button>();
        public MyPingApp()
        {
            InitializeComponent();
            tabControl.Visible = false;
            buttonPing.BackgroundImage = Properties.Resources.ping;
            timer.Start();
        }

        private static string PingHost(IPAddress ip)
        {
            string returnMessage = string.Empty;
            PingOptions pingOptions = new PingOptions(128, true);
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            byte[] buffer = new byte[32];
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        PingReply pingReply = ping.Send(ip, 1000, buffer, pingOptions);
                        if (pingReply != null)
                        {
                            switch (pingReply.Status)
                            {
                                case IPStatus.Success:
                                    returnMessage = string.Format("Reply from {0}: bytes={1} time={2}ms TTL={3}", pingReply.Address, pingReply.Buffer.Length, pingReply.RoundtripTime, pingReply.Options.Ttl);
                                    break;
                                case IPStatus.TimedOut:
                                    returnMessage = "Connection has timed out...";
                                    break;
                                default:
                                    returnMessage = string.Format("Ping failed: {0}", pingReply.Status.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            returnMessage = "Connection failed...";
                        }
                    }
                    catch (PingException ex)
                    {
                        returnMessage = string.Format("Connection Error: {0}", ex.Message);
                    }
                    catch (SocketException ex)
                    {
                        returnMessage = string.Format("Connection Error: {0}", ex.Message);
                    }
                }
            }
            else
                returnMessage = "No Internet connection found...";
            return returnMessage;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < count; i++)
            {
                rtbOutput[i].AppendText(PingHost(currentIPs[i]) + "\n");
            }
        }
        private void buttonPing_Click(object sender, EventArgs e)
        {
            if (IPAddress.TryParse(textBoxAddress.Text, out IPAddress ip))
            {
                tabControl.Visible = true;
                tabControl.TabPages.Add($"{count}");
                currentIPs.Add(ip);
                rtbCurrent.Add(new RichTextBox());
                rtbOutput.Add(new RichTextBox());
                btnDisconnect.Add(new Button());
                ApplyStyle(rtbCurrent[count], new Size(503, 34), new Point(0, 0), new Font("Comic Sans MS", 16, FontStyle.Regular));
                ApplyStyle(rtbOutput[count], new Size(537, 160), new Point(0, 34), new Font("Comic Sans MS", 13, FontStyle.Regular));
                btnDisconnect[count].Size = new Size(34, 34);
                btnDisconnect[count].Location = new Point(503, 0);
                btnDisconnect[count].BackgroundImage = Properties.Resources.disconnect;
                btnDisconnect[count].FlatStyle = FlatStyle.Flat;
                btnDisconnect[count].FlatAppearance.BorderSize = 0;
                btnDisconnect[count].MouseEnter += (_sender, _eventArgs) =>
                {
                    Button senderButton = (Button)_sender;
                    senderButton.BackgroundImage = Properties.Resources.disconnectHover;
                };
                btnDisconnect[count].MouseLeave += (_sender, _eventArgs) =>
                {
                    Button senderButton = (Button)_sender;
                    senderButton.BackgroundImage = Properties.Resources.disconnect;
                };
                btnDisconnect[count].Click += (_sender, _eventArgs) =>
                {
                    Button senderButton = (Button)_sender;
                    if (count > 0)
                    {
                        currentIPs.RemoveAt(tabControl.SelectedIndex);
                        rtbCurrent.RemoveAt(tabControl.SelectedIndex);
                        rtbOutput.RemoveAt(tabControl.SelectedIndex);
                        btnDisconnect.RemoveAt(tabControl.SelectedIndex);
                        tabControl.TabPages.RemoveAt(tabControl.SelectedIndex);
                        count--;
                        if (count > 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                tabControl.TabPages[i].Text = i.ToString();
                            }
                        }
                        else tabControl.Visible = false;
                    }
                };
                rtbCurrent[count].Text = "Ping host: " + currentIPs[count].ToString();
                tabControl.TabPages[count].Controls.Add(rtbCurrent[count]);
                tabControl.TabPages[count].Controls.Add(rtbOutput[count]);
                tabControl.TabPages[count].Controls.Add(btnDisconnect[count]);
                tabControl.SelectedIndex = count;
                count++;
            }
            else
            {
                MessageBox.Show("Wrong ip address", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyStyle(RichTextBox rtb, Size size, Point location, Font font)
        {
            rtb.Size = size;
            rtb.Location = location;
            rtb.HideSelection = false;
            rtb.BackColor = SystemColors.WindowText;
            rtb.ForeColor = SystemColors.Window;
            rtb.Font = font;
            rtb.BorderStyle = BorderStyle.None;
        }

        private void buttonPing_MouseEnter(object sender, EventArgs e)
        {
            buttonPing.BackgroundImage = Properties.Resources.pingHover;
            buttonPing.FlatAppearance.MouseOverBackColor = buttonPing.BackColor;
        }

        private void buttonPing_MouseLeave(object sender, EventArgs e)
        {
            buttonPing.BackgroundImage = Properties.Resources.ping;
        }
    }
}
