
using MySql.Data.MySqlClient;

namespace EasyActiveRecord
{
    static class DataBaseAccess
    {
        private static MySqlConnection _connexion = new MySqlConnection("Database=Contacts;Data Source=localhost;User Id=root;Password=root");

        public static MySqlConnection Connexion
        {
            get
            {
                return _connexion;
            }

            set
            {
                _connexion = value;
            }
        }

        public static MySqlParameter CodeParam(string paramName, object value)
        {
            MySqlCommand commandSql = Connexion.CreateCommand();
            MySqlParameter parametre = commandSql.CreateParameter();
            parametre.ParameterName = paramName;
            parametre.Value = value;
            return parametre;
        }
    }
}

//Avec Abstraction -> Diminue le COUPLAGE 

//public static IDbConnection Connexion { get; set; }

//public static IDbDataParameter CodeParam(string paramName, object value)
//{
//    IDbCommand commandSql = Connexion.CreateCommand();
//    IDbDataParameter parametre = commandSql.CreateParameter();
//    parametre.ParameterName = paramName;
//    parametre.Value = value;
//    return parametre;
//}