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
using System.Reflection;

namespace Socket_4I
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Socket socket;
        List<Contatto> contatti;
        IPAddress IPLocale; 
        int portaLocale;
        IPAddress IPRemoto;
        int portaRemota;

        public MainWindow()
        {
            InitializeComponent();    

            contatti = new List<Contatto>();
            contatti.Add(new Contatto("Aldo", IPAddress.Parse("127.0.0.1"), 10000));
            contatti.Add(new Contatto("Baldo", IPAddress.Parse("127.0.0.1"), 11000));
            contatti.Add(new Contatto("Carlo", IPAddress.Parse("127.0.0.1"), 12000));
            contatti.Add(new Contatto("Dean", IPAddress.Parse("127.0.0.2"), 10000));
            contatti.Add(new Contatto("Ettore", IPAddress.Parse("127.0.0.3"), 10000));

            AggiornaRubrica();
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
                foreach (Contatto c in contatti)
                {
                    if (c.IP.Equals(IPMittente) && c.Porta == portaMittente)
                    {
                        //aggiungo il messaggio alla chat
                        presente = true;
                        c.AggiungiMessaggio(c.Nome + ": " + messaggio);
                        
                        Contatto mittente = new Contatto();
                        lstRubrica.Dispatcher.Invoke(() => { mittente = lstRubrica.SelectedItem as Contatto; });

                        //aggiungo il messaggio alla chat nella parte visuale
                        if (mittente.IP.Equals(IPMittente) && mittente.Porta == portaMittente)
                            lstMessaggi.Dispatcher.Invoke(() => { lstMessaggi.Items.Add(lblContatto.Content + ": " + messaggio); });
                        else//invio una notifica se non si è direttamente in quella chat
                        {
                            MessageBox.Show(c.Nome + " ti ha mandato un messaggio.");
                        }
                        break;
                    }
                }

                //se il mittente non è presente tra i contatti chiedo se l'utente lo vuole aggiungere tra i contatti
                if (!presente)
                {
                    MessageBoxResult salva = MessageBox.Show($"{IPMittente}:{portaMittente} sta cercando di inviarti un messaggio.\n" +
                        $"vuoi salvarlo tra i contatti per visualizzare la sua chat?", "Attenzione!", MessageBoxButton.YesNo);
                    if (salva == MessageBoxResult.Yes)
                    {//aggiungo il contatto
                        Contatto contatto = new Contatto($"{IPMittente}:{portaMittente}", IPMittente, portaMittente);
                        contatto.AggiungiMessaggio(contatto.Nome + ": " + messaggio);

                        AggiungiContatto(contatto);

                        lstRubrica.Dispatcher.Invoke(() => lstRubrica.Items.Add(contatto));

                        MessageBox.Show("Contatto aggiunto con successo.");
                    }
                }          
            }
        }
       
        private void btnConfermaIpPorta_Click(object sender, RoutedEventArgs e)
        {//metodo che inizializza la socket con IP e porta riportati nell'interfaccia
            try
            {
                MessageBoxResult result = MessageBox.Show("Sei sicuro di voler inserire questo IP e questa Porta?", "Attenzione!", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    //prendo l'ip                   
                    bool IPok = IPAddress.TryParse(txtIPLocale.Text, out IPLocale);
                    if (!IPok) throw new Exception("Ip non valido.");
                    //prendo la porta                   
                    bool portaOk = int.TryParse(txtPortaLocale.Text, out portaLocale);
                    if (!portaOk) throw new Exception("Porta non valida.");

                    InizializzazioneSocket();
                    lblContatto.Content = "";

                    //disattivo i TextBox e il bottone
                    txtIPLocale.IsEnabled = false;
                    txtPortaLocale.IsEnabled = false;
                    btnConfermaIpPorta.IsEnabled = false;

                    MessageBox.Show("Socket inizializzata correttamente.");

                    if (lstRubrica.SelectedIndex != -1) 
                    {
                        InizializzaIpEPortaClientRemoto();
                        
                        Contatto contatto = lstRubrica.SelectedItem as Contatto;
                        if (contatto.IP.Equals(IPLocale) && contatto.Porta == portaLocale)
                            lstRubrica.SelectedIndex = -1;
                    }                    

                    Task.Run(AspettaMessaggio);
                }           
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }    
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {//metodo per inviare un singolo messaggio al contatto selezionato
            try
            {               
                //controllo se è stato selezionato un contatto
                if (lstRubrica.SelectedIndex == -1) throw new Exception("Seleziona un contatto!");

                ControlloIpEPortaValidi();

                InviaMessaggio(lstRubrica.SelectedItem as Contatto ,txtMessaggio.Text);

                //aggiungo il messaggio alla chat locale
                lstMessaggi.Items.Add("Io: " + txtMessaggio.Text);
                txtMessaggio.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }        
        }

        private void btnBroadcast_Click(object sender, RoutedEventArgs e)
        {//metodo per inviare un messaggio a tutti i contatti
            try
            {
                //ciclo che manda il messaggio a tutti i contatti
                foreach (Contatto c in contatti)
                {
                    //non manda il messaggio solo a se stesso
                    if (!IPLocale.Equals(c.IP) || portaLocale != c.Porta)
                    {
                        InviaMessaggio(c, txtMessaggio.Text);
                    }
                }

                //aggiorna la chat selezionata
                if(lstRubrica.SelectedIndex != -1)
                {
                    lstMessaggi.Items.Add("Io: " + txtMessaggio.Text);
                    txtMessaggio.Text = "";
                }

                MessageBox.Show("Messaggio mandato in Broadcast.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }        
  
        private void lstRubrica_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//metodo che inizializza IP e Porta del client remoto selezionato e riempe la chat 
            try
            {
                bool ok = RiempiInterfaccia();

                if (IPLocale != null && ok)
                {
                    ControlloIpEPortaValidi();

                    //pulisco la chat e poi la riempo la chat con i messaggi vecchi
                    lstMessaggi.Items.Clear();
                    foreach (string s in contatti[lstRubrica.SelectedIndex].Chat)
                    {
                        lstMessaggi.Items.Add(s);
                    }

                    InizializzaIpEPortaClientRemoto();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAggiungiContatto_Click(object sender, RoutedEventArgs e)
        {//metodo che aggiunge un contatto alla rubrica e la aggiorna visualmente
            try
            {
                IPAddress IP;
                int porta;

                //prendo l'ip                   
                bool IPok = IPAddress.TryParse(txtIPContatto.Text, out IP);
                if (!IPok) throw new Exception("Ip non valido.");
                //prendo la porta                   
                bool portaOk = int.TryParse(txtPortaContatto.Text, out porta);
                if (!portaOk) throw new Exception("Porta non valida.");

                Contatto contatto = new Contatto(txtNomeContatto.Text, IP, porta);

                AggiungiContatto(contatto);

                AggiornaRubrica();

                MessageBox.Show("Contatto aggiunto con successo.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnModificaContatto_Click(object sender, RoutedEventArgs e)
        {//metodo che modifica un contatto della rubrica e la aggiorna visualmente
            try
            {
                if (lstRubrica.SelectedIndex == -1) throw new Exception("Seleziona il contatto da modificare");

                IPAddress IP;
                int porta;

                //prendo l'ip                   
                bool IPok = IPAddress.TryParse(txtIPContatto.Text, out IP);
                if (!IPok) throw new Exception("Ip non valido.");
                //prendo la porta                   
                bool portaOk = int.TryParse(txtPortaContatto.Text, out porta);
                if (!portaOk) throw new Exception("Porta non valida.");

                ModificaContatto(lstRubrica.SelectedItem as Contatto, txtNomeContatto.Text, IP, porta);

                AggiornaRubrica();

                MessageBox.Show("Contatto modificato con successo.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEliminaContatto_Click(object sender, RoutedEventArgs e)
        {//metodo che elimina un contatto dalla rubrica e la aggiorna visualmente
            try
            {
                if (lstRubrica.SelectedIndex == -1) throw new Exception("Seleziona il contatto da modificare");

                Contatto contatto = lstRubrica.SelectedItem as Contatto;

                EliminaContatto(contatto);

                AggiornaRubrica();

                MessageBox.Show("Contatto eliminato con successo.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InizializzazioneSocket()
        {//metodo che inizializza la socket
            //inizializzo l'endpoint
            IPEndPoint local_endpoint = new IPEndPoint(IPLocale, portaLocale);

            //inizializzo la socket
            socket = null;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //associo alla socket l'endpoint appena creato 
            socket.Bind(local_endpoint);
        }
        
        private void InviaMessaggio(Contatto destinatario, string msg)
        {//metodo per inviare un messaggio ad un contatto
            if (string.IsNullOrWhiteSpace(msg)) throw new Exception("Non puoi inviare un messaggio vuoto!");

            //inizializzazione endpoint destinatario
            IPEndPoint remote_endpoint = new IPEndPoint(destinatario.IP, destinatario.Porta);

            //traduzione da testo UTF8 a bytes per essere inviato
            byte[] messaggio = Encoding.UTF8.GetBytes(msg);

            //invio il messaggio al destinatario
            socket.SendTo(messaggio, remote_endpoint);

            destinatario.AggiungiMessaggio("Io: " + msg);
        }

        private void ControlloIpEPortaValidi()
        {//metodo che controlla se non si sta cercando di comunicare con se stessi      
            Contatto contatto = contatti[lstRubrica.SelectedIndex];

            if (IPLocale.Equals(contatto.IP) && portaLocale == contatto.Porta)
            {
                throw new Exception("Non puoi comunicare con te stesso!\n" +
                    "Seleziona il contatto con cui vuoi comunicare.");
            }
        }

        private void InizializzaIpEPortaClientRemoto()
        {//metodo che inizializza ip e porta del client remoto  
            Contatto contatto = contatti[lstRubrica.SelectedIndex];
            lblContatto.Content = contatto.Nome;
            IPRemoto = contatto.IP;
            portaRemota = contatto.Porta;
        }

        private bool RiempiInterfaccia()
        {//metodo che riempe l'interfaccia se selezionato un contatto, sennò la pulisce
            if (lstRubrica.SelectedIndex != -1)
            {
                //aggiorno l'interfaccia
                Contatto c = lstRubrica.SelectedItem as Contatto;
                txtNomeContatto.Text = c.Nome;
                txtIPContatto.Text = c.IP.ToString();
                txtPortaContatto.Text = c.Porta.ToString();

                return true;
            }

            txtNomeContatto.Text = "";
            txtIPContatto.Text = "";
            txtPortaContatto.Text = "";
            lblContatto.Content = "";

            return false;
        }

        private void AggiungiContatto(Contatto contatto)
        {//metodo che aggiunge un contatto alla rubrica
            foreach (Contatto c in contatti)
            {
                if (contatto.IP.Equals(c.IP) && contatto.Porta == c.Porta) throw new Exception("Esiste già un contatto con questi IP e porta");
            }

            contatti.Add(contatto);            
        }

        private void ModificaContatto(Contatto contatto, string nome, IPAddress IP, int porta)
        {//metodo che aggiorna un contatto esistente nella rubrica
            foreach (Contatto c in contatti)
            {
                if (IP.Equals(c.IP) && porta == c.Porta) throw new Exception("Esiste già un contatto con questi IP e porta");
            }

            contatto.Nome = nome;
            contatto.IP = IP;
            contatto.Porta = porta;
        }

        private void EliminaContatto(Contatto contatto)
        {//metodo che elimina un contatto esistente dalla rubrica            
            bool eliminato = contatti.Remove(contatto);
            if (!eliminato) throw new Exception("Errore durante l'eliminazione del contatto.");
        }

        private void AggiornaRubrica()
        {//metodo che aggiorna la rubrica
            lstRubrica.Items.Clear();
            foreach (Contatto c in contatti)
            {
                lstRubrica.Items.Add(c);
            }
        }
    }
}