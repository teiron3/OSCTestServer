/*
    Copyright (c) 2023 teiron
    Released under the MIT license
    https://github.com/teiron3/OSCTestServer/blob/main/LICENSE
*/

using System;
using System.Windows;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Net;
using System.Net.Sockets;

class Program
{
    [STAThread]
    static void Main()
    {
        Application.Run(new Form1());
    }    
}
class Form1:Form{
    private Button ButtonStart;
    private Button ButtonStop;
    private Label LabelSendport;
    private TextBox TextboxSendport;
    private Label LabelDestport;
    private TextBox TextboxDestport;
    
    private Label LabelOutsideIP;
    private TextBox TextboxOutsideIP;
    private Label LabelThisAppSendPort;
    private TextBox TextboxThisAppSendPort;
    
    private Label LabelRemoveloopback;
    private CheckBox CheckboxRemoveloopback;

    private Label LabelRemoveoutside;
    private CheckBox CheckboxRemoveoutside;
    
    private UdpClient UdpSendSocket;
    private UdpClient UdpDestSocket;
    private IPEndPoint VirtualCastSendIP;
    private IPEndPoint VirtualCastDestIP;
    private IPEndPoint OutsideSendIP;
    private IPEndPoint ThisAppSendIP;

    public Form1(){
        this.Size = new Size(460, 300);
        this.Location = new Point(100, 100);
        this.Text = "OSC転送アプリ";
        
        ///
        int lableX = 10;
        int textboxx = 220;
        
        int locationy = 50;
        int addy = 25;
        
        UdpSendSocket = new UdpClient();
        Size labelSize = new Size(200, 20);
        Size labeloutsideSize = new Size(70, 20);
        Size textboxSize = new Size(200, 20);
        
        LabelThisAppSendPort = new Label();
        LabelThisAppSendPort.Parent = this;
        LabelThisAppSendPort.Size = labelSize;
        LabelThisAppSendPort.Location = new Point(lableX, locationy);
        LabelThisAppSendPort.Text = "このアプリからの送信ポート";

        TextboxThisAppSendPort = new TextBox();
        TextboxThisAppSendPort.Parent = this;
        TextboxThisAppSendPort.Size = textboxSize;
        TextboxThisAppSendPort.Location = new Point(textboxx, locationy);
        TextboxThisAppSendPort.Text = "18101";
        locationy += addy;

        LabelSendport = new Label();
        LabelSendport.Parent = this;
        LabelSendport.Size = labelSize;
        LabelSendport.Text = "バーチャルキャストで設定した送信ポート";
        LabelSendport.Location = new Point(lableX, locationy);
        
        TextboxSendport = new TextBox();
        TextboxSendport.Parent = this;
        TextboxSendport.Size = textboxSize;
        TextboxSendport.Location = new Point(textboxx, locationy);
        TextboxSendport.Text = "18100";
        locationy += addy;
        
        LabelDestport = new Label();
        LabelDestport.Parent = this;
        LabelDestport.Size = labelSize;
        LabelDestport.Text = "バーチャルキャストで設定した受信ポート";
        LabelDestport.Location = new Point(lableX, locationy);
        
        TextboxDestport = new TextBox();
        TextboxDestport.Parent = this;
        TextboxDestport.Size = textboxSize;
        TextboxDestport.Location = new Point(textboxx, locationy);
        TextboxDestport.Text = "19100";
        locationy += addy;
        locationy += addy;
        
        LabelOutsideIP = new Label();
        LabelOutsideIP.Parent = this;
        LabelOutsideIP.Size = labelSize;
        LabelOutsideIP.Location = new Point(lableX, locationy);
        LabelOutsideIP.Text = "送信先外部IPアドレス";
        
        TextboxOutsideIP = new TextBox();
        TextboxOutsideIP.Parent = this;
        TextboxOutsideIP.Size = textboxSize;
        TextboxOutsideIP.Location = new Point(textboxx, locationy);
        TextboxOutsideIP.Text = "127.0.0.1";
        locationy += addy;
        locationy += addy;

        LabelRemoveloopback = new Label();
        LabelRemoveloopback.Parent = this;
        LabelRemoveloopback.Size = labelSize;
        LabelRemoveloopback.Location = new Point(lableX, locationy);
        LabelRemoveloopback.Text = "アドレスから/loopbackを除く";
        
        CheckboxRemoveloopback = new CheckBox();
        CheckboxRemoveloopback.Parent = this;
        CheckboxRemoveloopback.Location = new Point(textboxx, locationy);
        CheckboxRemoveloopback.Checked = true;
        locationy += addy;

        LabelRemoveoutside = new Label();
        LabelRemoveoutside.Parent = this;
        LabelRemoveoutside.Size = labelSize;
        LabelRemoveoutside.Location = new Point(lableX, locationy);
        LabelRemoveoutside.Text = "アドレスから/outsideを除く";
        
        CheckboxRemoveoutside = new CheckBox();
        CheckboxRemoveoutside.Parent = this;
        CheckboxRemoveoutside.Location = new Point(textboxx, locationy);
        CheckboxRemoveoutside.Checked = true;

        ButtonStart = new Button();
        ButtonStart.Parent = this;
        ButtonStart.Location = new Point(10, 10);
        ButtonStart.Size = new Size(100, 30);
        ButtonStart.Text = "start";
        ButtonStart.Click += (obj, e) =>{
            Button btn = (Button) obj;
            try{
                VirtualCastSendIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Int32.Parse(TextboxSendport.Text));
                VirtualCastDestIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Int32.Parse(TextboxDestport.Text));
                OutsideSendIP = new IPEndPoint(IPAddress.Parse(TextboxOutsideIP.Text), Int32.Parse(TextboxThisAppSendPort.Text));
                ThisAppSendIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Int32.Parse(TextboxThisAppSendPort.Text));
                UdpDestSocket = new UdpClient(VirtualCastSendIP);
            }catch(Exception ex){
                MessageBox.Show("どれかのアドレスまたはポートの設定が間違っています。\n(UDPの受信ポートが塞がってる可能性もあります。)");
                return;
            }
            UdpDestSocket.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            btn.Enabled = false;
        };

        ButtonStop = new Button();
        ButtonStop.Parent = this;
        ButtonStop.Location = new Point(130, 10);
        ButtonStop.Size = new Size(100, 30);
        ButtonStop.Text = "stop";
        ButtonStop.Click += (obj, e) =>{
               UdpSendSocket.Send(new byte[] {0x71} , 1, VirtualCastSendIP);
            if(!ButtonStart.Enabled){
               ButtonStart.Enabled = true; 
            }
        };
    }
    private void ReceiveCallback(IAsyncResult ar){
        IPEndPoint refIP = null;
        byte[] receiveBytes = UdpDestSocket.EndReceive(ar, ref refIP);

        //受信の終了
        //最初のデータが q の場合に受信を終了します
        if(receiveBytes[0] == 0x71){
            Console.WriteLine("stop process");
            UdpDestSocket.Close();
            return;
        }

        //OSCのアドレスの最初に /loopback/ (/loopback だけだとダメ)が入っている場合の処理
        //バーチャルキャストの受信ポートにデータをそのまま戻します
        byte[] loopbackarray = System.Text.Encoding.ASCII.GetBytes("/loopback/");
        for(int count = 0; count < loopbackarray.Length; count++){
           if(receiveBytes[count] != loopbackarray[count])break; 
           if(count == loopbackarray.Length - 1){
               if(CheckboxRemoveloopback.Checked){
                   byte[] sendBytes = removeaddress(receiveBytes, "/loopback");
                   UdpSendSocket.Send(sendBytes, sendBytes.Length, VirtualCastDestIP);
               }else{
                   UdpSendSocket.Send(receiveBytes, receiveBytes.Length, VirtualCastDestIP);
               }
               UdpDestSocket.BeginReceive(new AsyncCallback(ReceiveCallback), null);
               return;
           }
        }
        
        //OSCのアドレスの最初に /outside/ (/outside だけだとダメ)が入っている場合の処理
        //外部のIPアドレスにデータをそのまま送信します
        byte[] outsidearray = System.Text.Encoding.ASCII.GetBytes("/outside/");
        for(int count = 0; count < outsidearray.Length; count++){
           if(receiveBytes[count] != outsidearray[count])break; 
           if(count == outsidearray.Length - 1){
               if(CheckboxRemoveoutside.Checked){
                   byte[] sendBytes = removeaddress(receiveBytes, "/outside");
                   UdpSendSocket.Send(sendBytes, sendBytes.Length, OutsideSendIP);
               }else{
                   UdpSendSocket.Send(receiveBytes, receiveBytes.Length, OutsideSendIP);
               }
               UdpDestSocket.BeginReceive(new AsyncCallback(ReceiveCallback), null);
               return;
           }
        }

        //OSCのアドレスが /sendkeys の場合の処理
        //string1個のみ場合だけSendkeysでアクティブウィンドウにキーを送信します
        byte[] sendkeysarray = System.Text.Encoding.ASCII.GetBytes("/sendkeys");
        for(int count = 0; count < sendkeysarray.Length; count++){
           if(receiveBytes[count] != sendkeysarray[count])break; 
           if(count == sendkeysarray.Length - 1 && receiveBytes[count  + 1] == 0){
               int subcount = count + 1;
               while((char)receiveBytes[++subcount] != ','){}
               if((char)receiveBytes[subcount + 1] != 's' && receiveBytes[subcount + 2] != 0)break;
               subcount += 2;
               while(receiveBytes[++subcount] == 0){if(subcount + 1 == receiveBytes.Length)break;}
               if(subcount + 1 == receiveBytes.Length)break;
               int stringstartpoint = subcount;
               while(receiveBytes[++subcount] != 0){if(subcount + 1 == receiveBytes.Length)break;}
               int stringlength = subcount - stringstartpoint;
               SendKeys.SendWait(System.Text.Encoding.ASCII.GetString((new ArraySegment<byte>(receiveBytes, stringstartpoint, stringlength)).ToArray()));
               UdpDestSocket.BeginReceive(new AsyncCallback(ReceiveCallback), null);
               return;
           }
        }
        
        //上記以外のものはそのまま送信
        UdpSendSocket.Send(receiveBytes, receiveBytes.Length, ThisAppSendIP);
        UdpDestSocket.BeginReceive(new AsyncCallback(ReceiveCallback), null);
    }
    
    ///OSCのアドレスから不要な部分を取り除く処理
    private byte[] removeaddress(byte[] bytes, string removestring){
        int bytesstartpoint = removestring.Length;
        int bytesaddressendpoint = bytesstartpoint;
        int addresslenght;
        int padzerolength;
        int datastartpoint;
        byte[] returnbytes;
        while(bytes[++bytesaddressendpoint] != 0);
        --bytesaddressendpoint;
        addresslenght = bytesaddressendpoint - bytesstartpoint + 1;
        padzerolength = addresslenght % 4;
        padzerolength = (padzerolength == 0)  ? 4 :4 - padzerolength;
        datastartpoint = bytesaddressendpoint + 1;
        while(bytes[++datastartpoint] != (byte)',');
        
        returnbytes = new byte[addresslenght + padzerolength + (bytes.Length - datastartpoint)];
        int writepoint = 0;
        while(writepoint + bytesstartpoint <= bytesaddressendpoint){
            returnbytes[writepoint] = bytes[writepoint + bytesstartpoint];
            writepoint++;
        }
        for(int writecount = 0; writecount < padzerolength; writecount++){
           returnbytes[writepoint] = 0;
           writepoint++; 
        }
        for(int writecount = 0; writecount + datastartpoint < bytes.Length; writecount++){
            returnbytes[writepoint] = bytes[writecount + datastartpoint];
            writepoint++;
        }

        return returnbytes;
    }
}