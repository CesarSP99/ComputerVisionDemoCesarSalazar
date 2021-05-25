using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleAppCVAnalisis
{
    class Program
    {
        static string subscriptionKey = "a28631f410ba4db0a5b90acaf35dc787";
        static string endpoint = "https://cvcesar.cognitiveservices.azure.com/";

        static void Main(string[] args)
        {
            string ANALYZE_URL_IMAGE = Console.ReadLine();
            ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);
            AnalyzeImageUrl(client, ANALYZE_URL_IMAGE).Wait();
        }

        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        public static async Task AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALISIS DE IMAGEN POR URL");
            Console.WriteLine();

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };

            Console.WriteLine($"Analizando la imagen {Path.GetFileName(imageUrl)}...");
            Console.WriteLine();

            ImageAnalysis results = await client.AnalyzeImageAsync(imageUrl, visualFeatures: features);

            //Descripción de la imagen

            Console.WriteLine("Resumen:");
            foreach (var caption in results.Description.Captions)
            {
                Console.WriteLine($"{caption.Text} con confianza de {caption.Confidence}");
            }
            Console.WriteLine();

            //Obteniendo categorías relacionadas con la imagen

            Console.WriteLine("Categorías:");
            foreach (var category in results.Categories)
            {
                Console.WriteLine($"{category.Name} con confianza de {category.Score}");
            }
            Console.WriteLine();

            //Obteniendo etiquetas relacionadas con la imagen

            Console.WriteLine("Etiquetas:");
            foreach (var tag in results.Tags)
            {
                Console.WriteLine($"{tag.Name} {tag.Confidence}");
            }
            Console.WriteLine();

            //Detección de objetos en la imagen

            Console.WriteLine("Objetos:");
            foreach (var obj in results.Objects)
            {
                Console.WriteLine($"{obj.ObjectProperty} con confianza de {obj.Confidence} at location {obj.Rectangle.X}, " +
                  $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
            }
            Console.WriteLine();

            //Detección de marcas

            Console.WriteLine("Marcas:");
            foreach (var brand in results.Brands)
            {
                Console.WriteLine($"Logo de {brand.Name} con confianza de {brand.Confidence} en {brand.Rectangle.X}, " +
                  $"{brand.Rectangle.X + brand.Rectangle.W}, {brand.Rectangle.Y}, {brand.Rectangle.Y + brand.Rectangle.H}");
            }
            Console.WriteLine();

            //Detección de caras

            Console.WriteLine("Caras:");
            foreach (var face in results.Faces)
            {
                Console.WriteLine($"Un humano de genero {face.Gender} y de {face.Age} años en {face.FaceRectangle.Left}, " +
                  $"{face.FaceRectangle.Left}, {face.FaceRectangle.Top + face.FaceRectangle.Width}, " +
                  $"{face.FaceRectangle.Top + face.FaceRectangle.Height}");
            }
            Console.WriteLine();

            //Detección de contenido para adultos, explícito o sangriento

            Console.WriteLine("Contenido adulto:");
            Console.WriteLine($"Tiene contenido adulto?: {results.Adult.IsAdultContent} con confianza de {results.Adult.AdultScore}");
            Console.WriteLine($"Tiene contenido explícito?: {results.Adult.IsRacyContent} con confianza de {results.Adult.RacyScore}");
            Console.WriteLine($"Tiene contenido sangriento?: {results.Adult.IsGoryContent} con confianza de {results.Adult.GoreScore}");
            Console.WriteLine();

            //Obteniendo la combinación de colores de la imagen

            Console.WriteLine("Esquema de Colores:");
            Console.WriteLine("Es B/W?: " + results.Color.IsBWImg);
            Console.WriteLine("Color: " + results.Color.AccentColor);
            Console.WriteLine("Color dominante de fondo: " + results.Color.DominantColorBackground);
            Console.WriteLine("Color primario dominante: " + results.Color.DominantColorForeground);
            Console.WriteLine("Colores dominantes: " + string.Join(",", results.Color.DominantColors));
            Console.WriteLine();

            //Detección de celebridades y lugares famosos

            Console.WriteLine("Celebridades:");
            foreach (var category in results.Categories)
            {
                if (category.Detail?.Celebrities != null)
                {
                    foreach (var celeb in category.Detail.Celebrities)
                    {
                        Console.WriteLine($"{celeb.Name} con confianza de {celeb.Confidence} at location {celeb.FaceRectangle.Left}, " +
                          $"{celeb.FaceRectangle.Top}, {celeb.FaceRectangle.Height}, {celeb.FaceRectangle.Width}");
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("Monumentos:");
            foreach (var category in results.Categories)
            {
                if (category.Detail?.Landmarks != null)
                {
                    foreach (var landmark in category.Detail.Landmarks)
                    {
                        Console.WriteLine($"{landmark.Name} con confianza de {landmark.Confidence}");
                    }
                }
            }
            Console.WriteLine();

            //Obteniendo tipo de imagen

            Console.WriteLine("Tipo de imagen:");
            Console.WriteLine("Tipo de ClipArt: " + results.ImageType.ClipArtType);
            Console.WriteLine("Tipo de dibujo lineal: " + results.ImageType.LineDrawingType);
            Console.WriteLine();

        }
    }
}
