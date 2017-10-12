using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GestCommande
{
    [Serializable]
    class Client
    {
        private string _numClient;
        private string _nomClient;
        private List<Commande> _lesCommandes;

        public string NumClient
        {
            get
            {
                return _numClient;
            }

            set
            {
                _numClient = value;
            }
        }

        public string NomClient
        {
            get
            {
                return _nomClient;
            }

            set
            {
                _nomClient = value;
            }
        }

        public Client (string numClient, string nomClient)
        {
            NumClient = numClient;
            NomClient = nomClient;
            _lesCommandes = new List<Commande>();
        }       
        
    }
}
