using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Text;
using Libs.Utils;

public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void prEnviaMensagemTelegram(SqlString Ds_Canal, SqlString Ds_Mensagem)
    {
        const string token = "1082624301:AAEpTCi_0SQyVVHudmuLmNKxhaVwU2DbISo";
        try
        {
            var mensagem = Ds_Mensagem.Value;
            var canais = Ds_Canal.Value.Split(';');

            foreach (var canal in canais)
            {
                var dsScript = $"chat_id={canal.Trim()}&text={mensagem}&parse_mode=Markdown";
                var url = $"https://api.telegram.org/bot{token}/sendMessage";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = (snder, cert, chain, error) => true;

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.UserAgent = "curl/7.45.0";
                request.ContentType = "application/x-www-form-urlencoded";

                var buffer = Encoding.GetEncoding("UTF-8").GetBytes(dsScript);
                using (var reqstr = request.GetRequestStream())
                {
                    reqstr.Write(buffer, 0, buffer.Length);

                    using (var response = request.GetResponse())
                    {
                        using (var dataStream = response.GetResponseStream())
                        {
                            if (dataStream == null) return;

                            using (var reader = new StreamReader(dataStream))
                            {
                                var responseFromServer = reader.ReadToEnd();
                                Retorno.Mensagem(responseFromServer);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Retorno.Erro("Erro : " + e.Message);
        }
    }
};