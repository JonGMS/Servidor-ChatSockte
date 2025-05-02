using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class Program
{
    static List<Remetente> listaDeRemetentes = new List<Remetente>();

    static void Main()
    {
        TcpListener servidor = new TcpListener(IPAddress.Any, 20000);
        servidor.Start();
        Console.WriteLine("Servidor ouvindo na porta 20000...");

        while (true)
        {
            try
            {
                TcpClient cliente = servidor.AcceptTcpClient();
                Console.WriteLine("Cliente conectado.");

                ProcessarCliente(cliente);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro no servidor: " + ex.Message);
            }
        }
    }

    private static void ProcessarCliente(TcpClient cliente)
    {
        try
        {
            NetworkStream stream = cliente.GetStream();

            byte[] buffer = new byte[4096];
            int bytesLidos = stream.Read(buffer, 0, buffer.Length);
            string jsonRecebido = Encoding.UTF8.GetString(buffer, 0, bytesLidos);

            Console.WriteLine("JSON recebido do cliente:");
            Console.WriteLine(jsonRecebido);

            Remetente remetente = JsonSerializer.Deserialize<Remetente>(jsonRecebido);

            if (remetente != null)
            {
                remetente.Id = listaDeRemetentes.Count; // usa índice da lista como ID
                listaDeRemetentes.Add(remetente);

                Console.WriteLine($"Novo remetente adicionado: {remetente.Nome} (ID: {remetente.Id})");

                // Confirmação de inserção via JSON
                string jsonConfirmacao = JsonSerializer.Serialize(remetente);
                byte[] dadosConfirmacao = Encoding.UTF8.GetBytes(jsonConfirmacao);
                stream.Write(dadosConfirmacao, 0, dadosConfirmacao.Length);

                // Envia lista com ID e Nome de todos os remetentes
                foreach (var r in listaDeRemetentes)
                {
                    if (stream.CanWrite)
                    {

                        Console.WriteLine("Enviando JSON inicial...");



                        byte[] bytesId = Encoding.UTF8.GetBytes(r.Id.ToString());
                        byte[] bytesNome = Encoding.UTF8.GetBytes(r.Nome);

                        byte[] tamanhoId = BitConverter.GetBytes(bytesId.Length);
                        byte[] tamanhoNome = BitConverter.GetBytes(bytesNome.Length);
                        Console.WriteLine("Enviando tamanho e conteúdo de ID...");
                        Console.WriteLine("Enviando tamanho e conteúdo de Nome...");
                        stream.Write(tamanhoId, 0, 4);
                        stream.Write(bytesId, 0, bytesId.Length);

                        stream.Write(tamanhoNome, 0, 4);
                        stream.Write(bytesNome, 0, bytesNome.Length);

                        Console.WriteLine($"Enviado: ID = {r.Id}, Nome = {r.Nome}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Erro ao desserializar o JSON.");
            }

            cliente.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao processar cliente: " + ex.Message);
        }
    }

    public class Remetente
    {
        public Remetente() { } // construtor vazio obrigatório para desserialização

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Ip { get; set; }
        public DateTime DataHora { get; set; }
        public bool Status { get; set; }
    }
}