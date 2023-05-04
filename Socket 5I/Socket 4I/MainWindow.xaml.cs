using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Socket_4I
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Socket socket;
        List<string[]> utenti;
        IPAddress IPLocale; 
        int portaLocale;
        IPAddress IPRemoto;
        int portaRemota;
        Task task;

        public MainWindow()
        {
            InitializeComponent();

            utenti = new List<string[]>();
            utenti.Add(new string[] { "Aldo", "127.0.0.1", "10000" });
            utenti.Add(new string[] { "Baldo", "127.0.0.1", "11000" });
            utenti.Add(new string[] { "Carlo", "127.0.0.1", "12000" });
            utenti.Add(new string[] { "Dean", "127.0.0.2", "10000" });
            utenti.Add(new string[] { "Ettore", "127.0.0.3", "10000" });
            
            foreach(string[] s in utenti)
            {
                lstRubrica.Items.Add($"{s[0]} {s[1]} {s[2]}");
            }

            Task.Run(AspettaMessaggio);

            /*IPAddress local_address = IPAddress.Any;
            IPEndPoint local_endpoint = new IPEndPoint(local_address, 10000);

            socket = null;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //associo alla socket l'endpoint appena creato 
            socket.Bind(local_endpoint);

            //socket.Blocking = false;
            //socket.EnableBroadcast = true;*/

        }

        private Task AspettaMessaggio()
        {//metodo che aspetta la ricezione di un messaggio
            int nBytes = 1024;
            byte[] buffer = new byte[nBytes];
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                //ricezione dei caratteri in attesa
                nBytes = socket.ReceiveFrom(buffer, ref remoteEndPoint);

                string from = ((IPEndPoint)remoteEndPoint).Address.ToString();

                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes);

                lstMessaggi.Dispatcher.Invoke(() => { lstMessaggi.Items.Add(lblContatto.Content + ": " + messaggio); });
            }
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {//metodo per inviare un messaggio
            try
            {
                if (string.IsNullOrEmpty(txtMessaggio.Text)) throw new Exception("Non puoi inviare un messaggio vuoto!");
                if (lstRubrica.SelectedIndex == -1) throw new Exception("Seleziona un contatto!");
                CheckIpEPortaValidi();

                //inizializzazione endpoint destinatario
                IPEndPoint remote_endpoint = new IPEndPoint(IPRemoto, portaRemota);
                
                //traduzione da testo UTF8 a bytes per essere inviato
                byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);

                //invio il messaggio al destinatario
                socket.SendTo(messaggio, remote_endpoint);

                //aggiungo il messaggio alla chat locale
                lstMessaggi.Items.Add("Io: " + txtMessaggio.Text);
                txtMessaggio.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }        
        }

        private void btnConfermaIpPorta_Click(object sender, RoutedEventArgs e)
        {//metodo che inizializza la socket con IP e porta riportati nell'interfaccia
            try
            {
                if (txtIP.IsEnabled)
                {
                    //prendo l'ip                   
                    bool IPok = IPAddress.TryParse(txtIP.Text, out IPLocale);
                    if (!IPok) throw new Exception("Ip non valido.");
                    //prendo la porta                   
                    bool portaOk = int.TryParse(txtPorta.Text, out portaLocale);
                    if (!portaOk) throw new Exception("Porta non valida.");                   

                    //inizializzo l'endpoint
                    IPEndPoint local_endpoint = new IPEndPoint(IPLocale, portaLocale);
                                       
                    //inizializzo la socket
                    socket = null;
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    //associo alla socket l'endpoint appena creato 
                    socket.Bind(local_endpoint);

                    //disattivo i TextBox
                    txtIP.IsEnabled = false;
                    txtPorta.IsEnabled = false;

                    MessageBox.Show("Socket inizializzata correttamente.");

                    if (lstRubrica.SelectedIndex != -1) InizializzaIpEPortaClientRemoto();
                }
                else
                {
                    txtIP.IsEnabled = true;
                    txtPorta.IsEnabled = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }    
        }

        private void lstRubrica_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//metodo che inizializza IP e Porta del client remoto
            try
            {
                if (IPLocale != null && lstRubrica.SelectedIndex != -1)
                {
                    CheckIpEPortaValidi();

                    InizializzaIpEPortaClientRemoto();                 
                }                        
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void CheckIpEPortaValidi()
        {//metodo che controlla se non si sta cercando di comunicare con se stessi      
            string[] utente = utenti[lstRubrica.SelectedIndex];

            if (IPLocale.ToString() == utente[1] && portaLocale.ToString() == utente[2])
            {
                lstRubrica.SelectedIndex = -1;
                throw new Exception("Non puoi comunicare con te stesso!\n" +
                    "Seleziona l'utente con cui vuoi comunicare.");
            }
        }

        private void InizializzaIpEPortaClientRemoto()
        {//metodo che inizializza ip e porta del client remoto  
            string[] utente = utenti[lstRubrica.SelectedIndex];
            lblContatto.Content = utente[0];
            IPRemoto = IPAddress.Parse(utente[1]);
            portaRemota = int.Parse(utente[2]);
        }
    }
}

