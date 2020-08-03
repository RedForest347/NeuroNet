using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//http://www.cyberforum.ru/csharp-beginners/thread2266709.html

namespace WindowsFormsApp1
{
    static class FileWork
    {
        public static NeuroNet LoadNeuroNet(string filePath)
        {
            NeuroNet neuroNet;
            using (StreamReader reader = new StreamReader(filePath))
            {
                NeuroData neuroData = NeuroData.LoadNeuroData(reader);
                int number_of_layers = Convert.ToInt32(reader.ReadLine());
                int[] layers = new int[number_of_layers];

                for (int i = 0; i < number_of_layers; i++)
                    layers[i] = Convert.ToInt32(reader.ReadLine());

                neuroNet = NeuroNet.CreateNeuroNet(layers, neuroData);

                for (int i = 0; i < neuroNet.neuroLayer.Length; i++)
                {
                    for (int j = 0; j < neuroNet.neuroLayer[i].Neurons.Length; j++)
                        for (int k = 0; k < neuroNet.neuroLayer[i].Neurons[j].weights.Length; k++)
                            neuroNet.neuroLayer[i].Neurons[j].weights[k] = Convert.ToSingle(reader.ReadLine());

                    if (i < neuroNet.neuroLayer.Length - 1)
                        for (int k = 0; k < neuroNet.neuroLayer[i + 1].Neurons.Length; k++)
                            neuroNet.neuroLayer[i].AdditionalNeuron.weights[k] = Convert.ToSingle(reader.ReadLine());
                }
            }
            return neuroNet;
        }

        public static void SaveNeuroNet(NeuroNet neuroNet, string filePath)
        {
            //Random random = new Random();

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                NeuroData.SaveNeuroData(neuroNet.neuroData, writer);

                writer.WriteLine(neuroNet.neuroLayer.Length);

                for (int i = 0; i < neuroNet.neuroLayer.Length; i++)
                    writer.WriteLine(neuroNet.neuroLayer[i].Neurons.Length);

                for (int i = 0; i < neuroNet.neuroLayer.Length; i++)
                {
                    for (int j = 0; j < neuroNet.neuroLayer[i].Neurons.Length; j++)
                        for (int k = 0; k < neuroNet.neuroLayer[i].Neurons[j].weights.Length; k++)
                            writer.WriteLine(neuroNet.neuroLayer[i].Neurons[j].weights[k]);

                    if (i < neuroNet.neuroLayer.Length - 1)
                        for (int k = 0; k < neuroNet.neuroLayer[i + 1].Neurons.Length; k++)
                            writer.WriteLine(neuroNet.neuroLayer[i].AdditionalNeuron.weights[k]);
                } 
            }
        }

        public static void SaveChar(char letter, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(letter);
            }
        }

        public static void SaveImage(Bitmap Image, string filePath)
        {
            Image.Save(filePath);
        }

        public static void SaveText(string text, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(text);
            }
        }
    }
}
