using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Socket_4I
{
    internal class Utente
    {
        private string _nome;
        private IPAddress _IP;
        private int _porta;
        private List<string> _chat;

        public Utente(string nome, IPAddress IP, int porta)
        {
            _nome = nome;
            _IP = IP;
            _porta = porta;
            _chat = new List<string>();
        }

        public Utente()
        {

        }

        public string Nome
        {
            set 
            {
                if (!string.IsNullOrWhiteSpace(value)) _nome = value;
                else throw new Exception("Inserisci un nome valido!");
            }
            get { return _nome; }
        }

        public IPAddress IP 
        { 
            set { _IP = value; }
            get { return _IP; } 
        }

        public int Porta
        {
            set { _porta = value; }
            get { return _porta; }
        }

        public List<string> Chat
        {
            get { return _chat; } 
        }

        public void AggiungiMessaggio(string messaggio) 
        {
            _chat.Add(messaggio);
        }

        public override string ToString()
        {
            return $"{Nome} {IP}:{Porta}";
        }
    }
}
