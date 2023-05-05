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
        List<Utente> utenti;
        IPAddress IPLocale; 
        int portaLocale;
        IPAddress IPRemoto;
        int portaRemota;

        public MainWindow()
        {
            InitializeComponent();    

            utenti = new List<Utente>();
            utenti.Add(new Utente("Aldo", IPAddress.Parse("127.0.0.1"), 10000));
            utenti.Add(new Utente("Baldo", IPAddress.Parse("127.0.0.1"), 11000));
            utenti.Add(new Utente("Carlo", IPAddress.Parse("127.0.0.1"), 12000));
            utenti.Add(new Utente("Dean", IPAddress.Parse("127.0.0.2"), 10000));
            utenti.Add(new Utente("Ettore", IPAddress.Parse("127.0.0.3"), 10000));

            foreach (Utente s in utenti)
            {
                lstRubrica.Items.Add(s);
            }
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
                
                //prendo ip e porta del mittente
                IPAddress IPMittente = ((IPEndPoint)remoteEndPoint).Address;
                int portaMittente = ((IPEndPoint)remoteEndPoint).Port;

                //decodifico il messaggio
                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes);

                //guardo se il mittente è già tra i contatti
                bool presente = false;
                foreach (Utente u in utenti)
                {
                    if (u.IP.Equals(IPMittente) && u.Porta == portaMittente)
                    {
                        presente = true;
                        u.AggiungiMessaggio(u.Nome + ": " + messaggio);
                        Utente mittente = new Utente();
                        lstRubrica.Dispatcher.Invoke(() => { mittente = lstRubrica.SelectedItem as Utente; });
                        if (mittente.IP.Equals(IPMittente) && mittente.Porta == portaMittente)
                            lstMessaggi.Dispatcher.Invoke(() => { lstMessaggi.Items.Add(lblContatto.Content + ": " + messaggio); });
                        else
                        {
                            MessageBox.Show(u.Nome + " ti ha mandato un messaggio.");
                        }
                        break;
                    }
                }

                /*if (!presente)
                {
                    MessageBoxResult result = MessageBox.Show($"{IPMittente}:{portaMittente} sta cercando di inviarti un messaggio.\n" +
                        $"vuoi salvarlo tra i contatti?", "Attenzione!", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        //implementa tu sai cosa
                    }
                }*/               
            }
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {//metodo per inviare un messaggio
            try
            {               
                if (lstRubrica.SelectedIndex == -1) throw new Exception("Seleziona un contatto!");
                CheckIpEPortaValidi();

                InviaMessaggio(lstRubrica.SelectedItem as Utente ,txtMessaggio.Text);

                //aggiungo il messaggio alla chat locale
                lstMessaggi.Items.Add("Io: " + txtMessaggio.Text);
                txtMessaggio.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }        
        }

        private void InviaMessaggio(Utente destinatario, string msg)
        {
            if (string.IsNullOrEmpty(msg)) throw new Exception("Non puoi inviare un messaggio vuoto!");           

            //inizializzazione endpoint destinatario
            IPEndPoint remote_endpoint = new IPEndPoint(destinatario.IP, destinatario.Porta);

            //traduzione da testo UTF8 a bytes per essere inviato
            byte[] messaggio = Encoding.UTF8.GetBytes(msg);

            //invio il messaggio al destinatario
            socket.SendTo(messaggio, remote_endpoint);

            destinatario.AggiungiMessaggio("Io: " + msg);
        }

        private void btnConfermaIpPorta_Click(object sender, RoutedEventArgs e)
        {//metodo che inizializza la socket con IP e porta riportati nell'interfaccia
            try
            {
                MessageBoxResult result = MessageBox.Show("Sei sicuro di voler inserire questo IP e questa Porta?", "Attenzione!", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
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

                    //disattivo i TextBox e il bottone
                    txtIP.IsEnabled = false;
                    txtPorta.IsEnabled = false;
                    btnConfermaIpPorta.IsEnabled = false;

                    MessageBox.Show("Socket inizializzata correttamente.");

                    if (lstRubrica.SelectedIndex != -1) InizializzaIpEPortaClientRemoto();

                    Task.Run(AspettaMessaggio);
                }           
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }    
        }

        private void lstRubrica_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//metodo che inizializza IP e Porta del client remoto e riempe la chat 
            try
            {
                if (IPLocale != null && lstRubrica.SelectedIndex != -1)
                {
                    CheckIpEPortaValidi();

                    //pulisco la chat e poi la riempo con i messaggi vecchi
                    lstMessaggi.Items.Clear();
                    foreach(string s in utenti[lstRubrica.SelectedIndex].Chat)
                    {
                        lstMessaggi.Items.Add(s);
                    }

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
            Utente utente = utenti[lstRubrica.SelectedIndex];

            if (IPLocale.Equals(utente.IP) && portaLocale == utente.Porta)
            {
                lstRubrica.SelectedIndex = -1;
                throw new Exception("Non puoi comunicare con te stesso!\n" +
                    "Seleziona l'utente con cui vuoi comunicare.");
            }
        }

        private void InizializzaIpEPortaClientRemoto()
        {//metodo che inizializza ip e porta del client remoto  
            Utente utente = utenti[lstRubrica.SelectedIndex];
            lblContatto.Content = utente.Nome;
            IPRemoto = utente.IP;
            portaRemota = utente.Porta;
        }

        private void btnBroadcast_Click(object sender, RoutedEventArgs e)
        {          
            try
            {
                foreach (Utente u in utenti)
                {
                    if (!IPLocale.Equals(u.IP) || portaLocale != u.Porta)
                    {
                        InviaMessaggio(u, txtMessaggio.Text);
                    }
                }

                MessageBox.Show("Messaggio mandato in Broadcast.");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

