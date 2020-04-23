using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.SqlServer.Server;

namespace Libs.Utils
{
    public static class Retorno
    {
        public static void Erro(string erro)
        {
            /*

            IF (OBJECT_ID('CLR.dbo.Log_Erro') IS NOT NULL) DROP TABLE CLR.dbo.Log_Erro
            CREATE TABLE CLR.dbo.Log_Erro (
	            Id_Erro INT IDENTITY(1,1),
	            Dt_Erro DATETIME DEFAULT GETDATE(),
	            Nm_Objeto VARCHAR(100),
	            Ds_Erro VARCHAR(MAX),
	            CONSTRAINT [PK_Log_Erro] PRIMARY KEY CLUSTERED (Id_Erro)
            )

            */
            using (var conexao = new SqlConnection(Servidor.getLocalhost()))
            {
                var comando = new SqlCommand("INSERT INTO dbo.Log_Erro (Nm_Objeto, Ds_Erro) VALUES (@Nm_Objeto, @Ds_Erro)", conexao);

                var stackTrace = new StackTrace();
                var objeto = stackTrace.GetFrame(1).GetMethod().Name;

                comando.Parameters.Add(new SqlParameter("@Nm_Objeto", SqlDbType.VarChar, 100)).Value = objeto;
                comando.Parameters.Add(new SqlParameter("@Ds_Erro", SqlDbType.VarChar, 8000)).Value = erro;
                conexao.Open();

                comando.ExecuteNonQuery();
            }
            throw new ApplicationException(erro);
        }
        public static void Mensagem(string mensagem)
        {
            using (var conexao = new SqlConnection(Servidor.Context))
            {
                var Comando = new SqlCommand("IF ( (512 & @@OPTIONS) = 512 ) select 1 else select 0", conexao);
                conexao.Open();

                if ((int)Comando.ExecuteScalar() != 0) return;

                var retorno = SqlContext.Pipe;
                retorno?.Send(mensagem.Length > 4000 ? mensagem.Substring(0, 4000) : mensagem);
            }
        }
        public static void RetornaReader(SqlDataReader dataReader)
        {
            var retorno = SqlContext.Pipe;
            retorno?.Send(dataReader);
        }
    }

    public class Ret : Exception
    {
        public Ret(string str) : base(str)
        {
        }
    }
}
