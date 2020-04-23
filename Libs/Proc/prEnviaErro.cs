using Libs.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void prEnviaErro(SqlString Ds_String)
    {
        try
        {
            if (Ds_String.Value == "ERRO")
            {
                // Vou forçar um erro ao tentar inserir dados em uma tabela que não existe, causando uma Exception
                using (var Conexao = new SqlConnection(Servidor.Localhost))
                {

                    var Comando = new SqlCommand("INSERT INTO dbo.Erro (Nm_Objeto, Ds_Erro) VALUES (@Nm_Objeto, @Ds_Erro)", Conexao);

                    Comando.Parameters.Add(new SqlParameter("@Nm_Objeto", SqlDbType.VarChar, 100)).Value = "Vai dar erro";
                    Comando.Parameters.Add(new SqlParameter("@Ds_Erro", SqlDbType.VarChar, 8000)).Value = "Descrição do Erro";
                    Conexao.Open();

                    Comando.ExecuteNonQuery();
                }
            }
            Retorno.Mensagem("Alerta enviado para o banco (PRINT)");
        }
        catch (Exception e)
        {
            Retorno.Erro("Mensagem de erro (RAISEERROR) gravada. Descrição do erro: " + e.Message);
        }
    }
}