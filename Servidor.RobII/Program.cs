using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class Program
{
    static void Main()
    {
        TcpListener servidor = new TcpListener(IPAddress.Any, 20000);
        servidor.Start();
        Console.WriteLine("Servidor ouvindo na porta 20000...");

        TcpClient cliente = servidor.AcceptTcpClient();
        Console.WriteLine("Conectou - Linha 16");

        NetworkStream stream = cliente.GetStream();
        Console.WriteLine("Conexão com Cliente - linha 17");
        //Para aq
        // BUFFER E LEITURA EM LOOP PARA GARANTIR QUE O JSON CHEGUE COMPLETO

        byte[] buffer = new byte[4096]; // Quantidade de bytes permitida dentro de um array

        int bytesLidos = 0;
        StringBuilder sb = new StringBuilder();

        
        
            bytesLidos = stream.Read(buffer, 0, buffer.Length);
            sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesLidos));
        // garante que tudo foi lido

        string jsonRecebido = sb.ToString();
        Console.WriteLine("JSON recebido:");
        Console.WriteLine(jsonRecebido);

        // DESSERIALIZAÇÃO
        var objetoRecebido = JsonSerializer.Deserialize<Mensagem>(jsonRecebido);

        Console.WriteLine("Usuário: " + objetoRecebido.Usuario.Nome);
        Console.WriteLine("Mensagem: " + objetoRecebido.TxtMensagem);

        cliente.Close();
        servidor.Stop();
    }

    public class Mensagem
    {
        public int Id { get; set; }
        public Usuario Usuario { get; set; }
        public string TxtMensagem { get; set; }
    }

    public class Usuario
    {
        public int Id { get; set; }
        public string Ip { get; set; }
        public string Nome { get; set; }
    }
}