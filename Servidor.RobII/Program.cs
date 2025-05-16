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

            /*//if (remetente != null)
            //{
            //    remetente.Id = listaDeRemetentes.Count; // usa índice da lista como ID
            //    listaDeRemetentes.Add(remetente);

            //    Console.WriteLine($"Novo remetente adicionado: {remetente.Nome} (ID: {remetente.Id})");

            //    // Envia a lista completa de remetentes como JSON
            //    string jsonLista = JsonSerializer.Serialize(listaDeRemetentes);
            //    byte[] dadosLista = Encoding.UTF8.GetBytes(jsonLista);

            //    if (stream.CanWrite)
            //    {
            //        stream.Write(dadosLista, 0, dadosLista.Length);
            //        Console.WriteLine("Lista de remetentes enviada ao cliente:");
            //        Console.WriteLine(jsonLista);
            //    }
            //    else
            //    {
            //        Console.WriteLine("Erro: O fluxo não está disponível para escrita.");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Erro ao desserializar o JSON.");
            /}*/

            while (true) // Thread de Mensagens 
            {

            }

            // Fecha o fluxo e a conexão
            stream.Close();
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