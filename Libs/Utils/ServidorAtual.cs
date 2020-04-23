using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Libs.Utils
{
    public class ServidorAtual
    {
        public string NomeServidor { get; set; }
        public ServidorAtual()
        {
            try
            {
                using (var conn = new SqlConnection(Servidor.Context))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT @@SERVERNAME AS InstanceName";
                        NomeServidor = (string)cmd.ExecuteScalar();
                    }

                    var partes = NomeServidor.Split('\\');

                    if (partes.Length <= 1) return;
                    if (string.Equals(partes[0], partes[1], StringComparison.CurrentCultureIgnoreCase))
                        NomeServidor = partes[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public static class Servidor
    {
        public static string Ds_Usuario => "";
        public static string Ds_Senha => "";
        public static string Ds_BancoDados => "";
        public static string PRODUCAO => "data source=.;initial catalog="+ Ds_BancoDados + ";persist security info=False;Enlist=False;packet size=4096;user id='" + Ds_Usuario + "';password='" + Ds_Senha + "'";
        public static string Context => "context connection=true";
        public static string Localhost => "data source=.;initial catalog=" + Ds_BancoDados + ";persist security info=False;Enlist=False;packet size=4096;user id='" + Ds_Usuario + "';password='" + Ds_Senha + "'";
        public static string getLocalhost()
        {
            var servidorAtual = new ServidorAtual().NomeServidor;
            return "data source=" + servidorAtual + ";initial catalog=" + Ds_BancoDados + ";persist security info=False;Enlist=False;packet size=4096;user id='" + Ds_Usuario + "';password='" + Ds_Senha + "'";
        }
        public static List<string> Servidores
        {
            get
            {
                var servidores = new List<string>
                {
                    PRODUCAO,
                    Localhost
                };

                return servidores;
            }
        }
    }
}
