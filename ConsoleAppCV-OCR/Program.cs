using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Linq;

namespace ConsoleAppCV_OCR
{
    class Program
    {
        static string subscriptionKey = "a28631f410ba4db0a5b90acaf35dc787";
        static string endpoint = "https://cvcesar.cognitiveservices.azure.com/";

        static void Main(string[] args)
        {
            Console.Write("Ingrese la URL a procesar: ");
            string READ_TEXT_URL_IMAGE = Console.ReadLine();
            ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);
            ReadFileUrl(client, READ_TEXT_URL_IMAGE).Wait();
        }

        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }
        public static async Task ReadFileUrl(ComputerVisionClient client, string urlFile)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Extrayendo texto de la imagen");
            Console.WriteLine();

            //Leyendo el texto

            var textHeaders = await client.ReadAsync(urlFile);

            //Obteniendo el ID de operación
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);

            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extrayendo el texto
            ReadOperationResult results;
            Console.WriteLine($"Extrayendo texto de {Path.GetFileName(urlFile)}...");
            Console.WriteLine();
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            //Escribiendo en Consola texto encontrado

            Console.WriteLine();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    Console.WriteLine(line.Text);
                }
            }
            Console.WriteLine();
        }
    }
}
