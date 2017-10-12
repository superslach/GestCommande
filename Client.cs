using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;


namespace EasyActiveRecord
{
    class Contact
    {

        #region Modele Objet


        //champ privé
        private short id;

        //propriété qui permet l'accès en lecture au champ id
        public short Id
        {
            get { return id; }
        }


        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }

        public Service Service { get; set; }

        public override string ToString()
        {
            return String.Format("Nom : {0} Prenom : {1} Telephone : {2} Email : {3} ", Nom, Prenom, Telephone, Email);
        }

        #endregion

        public Contact()
        {
            id = -1;//Identifie un contact non référencé dans la base de données
        }

        #region Champs à portée classe contenant l'ensemble des requêtes d'accès aux données
        private static string _selectSql =
            "SELECT id , prenom, nom ,  telephone , email FROM contact";

        private static string _selectByIdSql =
            "SELECT id , prenom, nom ,  telephone , email FROM contact WHERE id = ?id ";

        private static string _updateSql =
            "UPDATE contact SET prenom = ?prenom, nom=?nom , telephone=?telephone, email=?email  WHERE id=?id ";

        private static string _insertSql =
            "INSERT INTO contact (prenom,nom,email,telephone) VALUES (?prenom,?nom,?email,?telephone)";

        private static string _deleteByIdSql =
            "DELETE FROM contact WHERE id = ?id";

        private static string _getLastInsertId =
            "SELECT id FROM contact WHERE prenom = ?prenom AND nom=?nom AND telephone=?telephone AND email=?email  ";
        #endregion

        #region Méthodes d'accès aux données

        /// <summary>
        /// Valorise un objet contact depuis le système de gestion de bases de données
        /// </summary>
        /// <param name="idContact">La valeur de la clé primaire</param>
        public static Contact Fetch(int idContact)
        {
            Contact c = null;
            DataBaseAccess.Connexion.Open();
            MySqlCommand commandSql = DataBaseAccess.Connexion.CreateCommand();//Initialisation d'un objet permettant d'interroger la bd
            commandSql.CommandText = _selectByIdSql;//Définit la requete à utiliser
            commandSql.Parameters.Add(DataBaseAccess.CodeParam("?id", idContact));//Transmet un paramètre à utiliser lors de l'envoie de la requête
            commandSql.Prepare();//Prépare la requête (modification du paramètre de la requête)
            MySqlDataReader jeuEnregistrements = commandSql.ExecuteReader();//Exécution de la requête
            bool existEnregistrement = jeuEnregistrements.Read();//Lecture du premier enregistrement
            if (existEnregistrement)//Si l'enregistrement existe
            {//alors
                c = new Contact();//Initialisation de la variable Contact
                c.id = Convert.ToInt16(jeuEnregistrements["id"].ToString());//Lecture d'un champ de l'enregistrement
                c.Nom = jeuEnregistrements["Nom"].ToString();
                c.Prenom = jeuEnregistrements["Prenom"].ToString();
                c.Telephone = jeuEnregistrements["Telephone"].ToString();
                c.Email = jeuEnregistrements["Email"].ToString();
                string numService = jeuEnregistrements["Service"].ToString();
                c.Service = Service.Fetch(numService);
            }
            DataBaseAccess.Connexion.Close();//fermeture de la connexion
            return c;
        }




        /// <summary>
        /// Sauvegarde ou met à jour un contact dans la base de données
        /// </summary>
        public void Save()
        {
            if (id == -1)
            {
                Insert();
            }
            else
            {
                Update();
            }
        }

        /// <summary>
        /// Supprime le contact représenté par l'instance courante dans le SGBD
        /// </summary>
        public void Delete()
        {
            DataBaseAccess.Connexion.Open();
            MySqlCommand commandSql = DataBaseAccess.Connexion.CreateCommand();
            commandSql.CommandText = _deleteByIdSql;
            commandSql.Parameters.Add(DataBaseAccess.CodeParam("?id", id));
            commandSql.Prepare();
            commandSql.ExecuteNonQuery();
            id = -1;
        }

        private void Update()
        {
            DataBaseAccess.Connexion.Open();
            MySqlCommand commandSql = DataBaseAccess.Connexion.CreateCommand();
            commandSql.CommandText = _updateSql;
            commandSql.Parameters.Add(DataBaseAccess.CodeParam("?id", Id));
            commandSql.Parameters.Add(DataBaseAccess.CodeParam("?prenom", Prenom));
            commandSql.Parameters.Add(DataBaseAccess.CodeParam("?nom", Nom));
            commandSql.Parameters.Add(DataBaseAccess.CodeParam("?email", Email));
            commandSql.Parameters.Add(DataBaseAccess.CodeParam("?telephone", Telephone));
            commandSql.Prepare();
            commandSql.ExecuteNonQuery();
            DataBaseAccess.Connexion.Close();
        }

        private void Insert()
        {
            DataBaseAccess.Connexion.Open();
            MySqlCommand commandSql = DataBaseAccess.Connexion.CreateCommand();
            commandSql.CommandText = _insertSql;

            commandSql.Parameters.Add(DataBaseAccess.CodeParam("?prenom", Prenom));
            commandSql.Parameters.Add(DataBaseAccess.CodeParam("?nom", Nom));
            commandSql.Parameters.Add(DataBaseAccess.CodeParam("?telephone", Telephone));
            commandSql.Parameters.Add(DataBaseAccess.CodeParam("?email", Email));
            commandSql.Prepare();
            commandSql.ExecuteNonQuery();

            MySqlCommand commandGetLastId = DataBaseAccess.Connexion.CreateCommand();
            commandGetLastId.CommandText = _getLastInsertId;
            commandGetLastId.Parameters.Add(DataBaseAccess.CodeParam("?prenom", Prenom));
            commandGetLastId.Parameters.Add(DataBaseAccess.CodeParam("?nom", Nom));
            commandGetLastId.Parameters.Add(DataBaseAccess.CodeParam("?telephone", Telephone));
            commandGetLastId.Parameters.Add(DataBaseAccess.CodeParam("?email", Email));
            commandGetLastId.Prepare();
            MySqlDataReader jeuEnregistrements = commandGetLastId.ExecuteReader();
            bool existEnregistrement = jeuEnregistrements.Read();
            if (existEnregistrement)
            {
                id = Convert.ToInt16(jeuEnregistrements["id"].ToString());
            }
            else
            {
                commandSql.Transaction.Rollback();
                throw new Exception("Duplicate entry");
            }
            DataBaseAccess.Connexion.Close();
        }

        /// <summary>
        /// Retourne une collection contenant les contacts
        /// </summary>
        /// <returns>Une collection de contacts</returns>
        public static List<Contact> FetchAll()
        {
            List<Contact> resultat = new List<Contact>();
            DataBaseAccess.Connexion.Open();
            MySqlCommand commandSql = DataBaseAccess.Connexion.CreateCommand();
            commandSql.CommandText = _selectSql;
            MySqlDataReader jeuEnregistrements = commandSql.ExecuteReader();
            while (jeuEnregistrements.Read())
            {
                Contact contact = new Contact();
                string idContact = jeuEnregistrements["id"].ToString();
                contact.id = Convert.ToInt16(idContact);
                contact.Nom = jeuEnregistrements["Nom"].ToString();
                contact.Prenom = jeuEnregistrements["Prenom"].ToString();
                contact.Telephone = jeuEnregistrements["Telephone"].ToString();
                contact.Email = jeuEnregistrements["Email"].ToString();
                resultat.Add(contact);
            }
            DataBaseAccess.Connexion.Close();
            return resultat;
        }

        #endregion
    }
}