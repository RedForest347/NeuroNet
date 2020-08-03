using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace WindowsFormsApp1
{
    class NeuroNet
    {
        #region NotModified

        public NeuroLayer[] neuroLayer;
        public NeuroData neuroData;
        public float edukationK;
        int additional_signal = 1;
        public float error_of_era = 0;

        float[] current_input_signals;

        public static NeuroNet CreateNeuroNet(int[] layers, NeuroData neuroData)
        {
            NeuroNet neuroNet = new NeuroNet();

            neuroNet.neuroData = neuroData;
            neuroNet.neuroLayer = new NeuroLayer[layers.Length];
            neuroNet.SetData();

            for (int i = 0; i < layers.Length; i++)
            {
                if (i < layers.Length - 1)
                    neuroNet.neuroLayer[i] = new NeuroLayer(layers[i], layers[i + 1]);
                else
                    neuroNet.neuroLayer[i] = new NeuroLayer(layers[i], 0);
            }
            neuroNet.FullZeroingNeurons();
            return neuroNet;
        }

        void SetData()
        {
            edukationK = neuroData.edukationK;
        }

        public void ZeroingErrorOfEra()
        {
            error_of_era = 0;
        }

        public void FullZeroingNeurons()
        {
            for (int i = 0; i < neuroLayer.Length; i++)
            {
                for (int j = 0; j < neuroLayer[i].length; j++)
                {
                    neuroLayer[i].Neurons[j].current_signal = 0;
                    neuroLayer[i].Neurons[j].current_error = 0;
                }
                
                if (i < neuroLayer.Length - 1)
                    neuroLayer[i].AdditionalNeuron.current_signal = additional_signal;
            }
        }

        public void PassZeroing()
        {
            for (int i = 0; i < neuroLayer.Length; i++)
            {
                for (int j = 0; j < neuroLayer[i].Neurons.Length; j++)
                {
                    neuroLayer[i].Neurons[j].current_error = 0;
                    neuroLayer[i].Neurons[j].current_signal = 0;
                }
                if (neuroLayer[i].AdditionalNeuron != null)
                    neuroLayer[i].AdditionalNeuron.current_error = 0;
            }
        }

        public void ForwardPass()
        {
            float signal;
            Neuron neuron;
            for (int current_layer = 0; current_layer < neuroLayer.Length - 1; current_layer++) //проход по уровням
            {
                for (int current_neuron = 0; current_neuron < neuroLayer[current_layer].Neurons.Length; current_neuron++)//проход по нейронам в уровне
                {
                    signal = neuroLayer[current_layer].Neurons[current_neuron].current_signal;
                    neuron = neuroLayer[current_layer].Neurons[current_neuron];
                    for (int next_neuron = 0; next_neuron < neuroLayer[current_layer + 1].length; next_neuron++)//проход по нейронам в следующем уровне
                    {
                        neuroLayer[current_layer + 1].Neurons[next_neuron].current_signal += signal * neuron.weights[next_neuron];
                    }
                }

                for (int next_neuron = 0; next_neuron < neuroLayer[current_layer + 1].length; next_neuron++)//проход по нейронам в следующем уровне (прибавка нейрона смещения)
                {
                    neuroLayer[current_layer + 1].Neurons[next_neuron].current_signal += neuroLayer[current_layer].AdditionalNeuron.weights[next_neuron];
                }

                for (int current_neuron = 0; current_neuron < neuroLayer[current_layer + 1].length; current_neuron++)
                {
                    neuron = neuroLayer[current_layer + 1].Neurons[current_neuron];
                    neuron.current_signal = ActivationFunction(neuron.current_signal);
                }
            }
        }

        public void FindErrors(float[] ideal_signals)
        {
            Neuron neuron;

            for (int i = 0; i < neuroLayer[neuroLayer.Length - 1].length; i++)
                neuroLayer[neuroLayer.Length - 1].Neurons[i].current_error = ideal_signals[i] - neuroLayer[neuroLayer.Length - 1].Neurons[i].current_signal;

            for (int i = neuroLayer.Length - 1 - 1; i >= 0; i--)
            {
                for (int j = 0; j < neuroLayer[i].Neurons.Length; j++)
                {
                    neuron = neuroLayer[i].Neurons[j];
                    for (int k = 0; k < neuroLayer[i + 1].length; k++)
                    {
                        neuron.current_error += neuroLayer[i + 1].Neurons[k].current_error * neuron.weights[k];
                    }
                }
                for (int j = 0; j < neuroLayer[i + 1].Neurons.Length; j++)
                {
                    neuroLayer[i].AdditionalNeuron.current_error += neuroLayer[i + 1].Neurons[j].current_error * neuroLayer[i].AdditionalNeuron.weights[j];
                }
            }
        }

        public void FindNewWeights()
        {
            float new_weight = 0;
            Neuron neuron, nextNeuron, AdditionalNeuron;

            for (int current_layer = 0; current_layer < neuroLayer.Length - 1; current_layer++)
            {
                for (int this_layer_neuron = 0; this_layer_neuron < neuroLayer[current_layer].Neurons.Length; this_layer_neuron++) // обычные нейроны
                {
                    neuron = neuroLayer[current_layer].Neurons[this_layer_neuron];
                    for (int next_layer_neuron = 0; next_layer_neuron < neuroLayer[current_layer + 1].Neurons.Length; next_layer_neuron++)
                    {
                        nextNeuron = neuroLayer[current_layer + 1].Neurons[next_layer_neuron];
                        new_weight = neuron.weights[next_layer_neuron] + edukationK * nextNeuron.current_error * CalculateDerivative(nextNeuron.current_signal) * neuron.current_signal;
                        neuron.weights[next_layer_neuron] = new_weight;
                    }
                }

                AdditionalNeuron = neuroLayer[current_layer].AdditionalNeuron;

                for (int next_layer_neuron = 0; next_layer_neuron < neuroLayer[current_layer + 1].Neurons.Length; next_layer_neuron++) //нейроны смещения
                {
                    nextNeuron = neuroLayer[current_layer + 1].Neurons[next_layer_neuron];
                    new_weight = AdditionalNeuron.weights[next_layer_neuron] + edukationK * nextNeuron.current_error * CalculateDerivative(nextNeuron.current_signal);
                    AdditionalNeuron.weights[next_layer_neuron] = new_weight;
                }
            }
        }

        public void SetFirstSignals(float[] signals)
        {
            current_input_signals = signals;

            for (int i = 0; i < neuroLayer[0].Neurons.Length; i++)
            {
                neuroLayer[0].Neurons[i].current_signal = signals[i];
            }
        }

        public float ShowError()
        {
            float error = 0;

                for (int k = 0; k < neuroLayer[neuroLayer.Length - 1].Neurons.Length; k++)
                {
                    error += neuroLayer[neuroLayer.Length - 1].Neurons[k].current_error * neuroLayer[neuroLayer.Length - 1].Neurons[k].current_error;
                }
            error_of_era += error;
            return error;
        }

        public float ShowErrorOfEra()
        {
            return error_of_era;
        }

        static float ActivationFunction(float x)
        {
            return (float)(1 / (1 + Math.Pow(Math.E, -x)));
        }

        static float CalculateDerivative(float x)
        {
            return x * (1 - x);
        }

        public void Info()
        {
            string mes = "начало информации о нейросети";
            mes += "\n Нейросеть состоит из " + neuroLayer.Length + " слоев.";
            for (int i = 0; i < neuroLayer.Length; i++)
            {
                mes += ("\n " + i + " слой состоит из " + neuroLayer[i].length + " нейронов с весами:");
                for (int j = 0; j < neuroLayer[i].length; j++)
                {
                    mes += "\n\t    " + j + " нейрон (с сигналом " + neuroLayer[i].Neurons[j].current_signal + ") :";
                    for (int k = 0; k < neuroLayer[i].Neurons[j].weights.Length; k++)
                    {
                        mes += "\n \t \t    " + k + "-ая связь имеет вес: " + neuroLayer[i].Neurons[j].weights[k];
                    }
                }
            }
            mes += "\n\n" + "конец информации о нейросети";
            Debug.Log(mes);
        }

        public float[] GetOutputSignals()
        {
            float[] output_signals = new float[neuroLayer[neuroLayer.Length - 1].length];

            for (int i = 0; i < output_signals.Length; i++)
            {
                output_signals[i] = neuroLayer[neuroLayer.Length - 1].Neurons[i].current_signal;
            }
            return output_signals;
        }

        public void SaveNeuronet(string filepath)
        {
            FileWork.SaveNeuroNet(this, filepath);
        }

        public static NeuroNet LoadNeuronet(string filepath)
        {
            return FileWork.LoadNeuroNet(filepath);
        }

        public static NeuroNet LoadNeuronet(string filepath, object NeuroDataSupportClass)
        {
            NeuroNet neuroNet = FileWork.LoadNeuroNet(filepath);
            neuroNet.neuroData.SupportClass = NeuroDataSupportClass;
            return neuroNet;
        }

        #endregion NotModified

        public void Work(float[] signals)
        {
            Info();
            FullZeroingNeurons();
            SetFirstSignals(signals);
            ForwardPass();
            Info();
        }

        public void WorkPart1(float[] signals)
        {
            PassZeroing();
            SetFirstSignals(signals);
            ForwardPass();
            //Debug.Log(GetOutputSignals()[0] * 256 + " предположительно здесь " + ((char)(GetOutputSignals()[0] * 256 + 0.5f)));
        }

        public void WorkPart2(float[] ideal_signals)
        {
            FindErrors(ideal_signals);
            FindNewWeights();
        }

        public void Pass(float[] signals, float[] ideal_signals)
        {
            PassZeroing();
            SetFirstSignals(signals);
            ForwardPass();
            FindErrors(ideal_signals);
            FindNewWeights();
            ShowError();
        }

        public float[] PassOutput(float[] signals)
        {
            PassZeroing();
            SetFirstSignals(signals);
            ForwardPass();
            return GetOutputSignals();
        }

        public void FinalPass(float[] signals)
        {
            PassZeroing();
            SetFirstSignals(signals);
            ForwardPass();
            //Info();
        }
    }

    class NeuroLayer
    {
        public int length;
        public Neuron[] Neurons;
        public Neuron AdditionalNeuron;

        public NeuroLayer(int size, int size_of_next_layer)
        {
            Neurons = new Neuron[size];
            length = size;
            for (int i = 0; i < size; i++)
            {
                Neurons[i] = new Neuron(size_of_next_layer);
            }
            if (size_of_next_layer != 0)
                AdditionalNeuron = new Neuron(size_of_next_layer);
        }
    }

    class Neuron
    {
        static Random R = new Random();
        public float current_signal;
        public float current_error;
        public float[] weights;

        public Neuron(int num_of_wights)
        {
            current_error = 0;
            current_signal = 0;
            weights = new float[num_of_wights];
            for (int i = 0; i < num_of_wights; i++)
            {
                weights[i] = R.Next(-50, 50) / 100f;
            }
        }
    }

    class NeuroData
    {
        //public int offset;
        public int number_of_passes;
        public float edukationK;
        public object SupportClass;
        public NeuroNet neuroNet;

        public NeuroData(int number_of_passes, float edukationK)
        {
            this.edukationK = edukationK;
            this.number_of_passes = number_of_passes;
            SupportClass = new object();
        }

        public NeuroData(int number_of_passes, float edukationK, object SupportClass)
        {
            this.edukationK = edukationK;
            this.number_of_passes = number_of_passes;
            this.SupportClass = SupportClass;
        }

        public static void SaveNeuroData(NeuroData neuroData, StreamWriter writer)
        {
            writer.WriteLine(neuroData.number_of_passes);
            writer.WriteLine(neuroData.edukationK);
        }

        public static NeuroData LoadNeuroData(StreamReader reader)
        {
            int number_of_passes = Convert.ToInt32(reader.ReadLine());
            float edukationK = Convert.ToSingle(reader.ReadLine());
            return new NeuroData(number_of_passes, edukationK);
        }
    }
}
