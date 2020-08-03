using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace WindowsFormsApp1
{
    class NeuroWork
    {
        public string final_massage = "";
        public int current_pass = 0;
        int offset;
        public List<Line> LineList = new List<Line>();
        public Bitmap NormalizeImage;
        int[] start_signals;
        Bitmap PreShow;

        public NeuroNet neuroNet;// = NeuroNet.CreateNeuroNet(new int[] {400, 15, 1 }, new NeuroData(1000, 0.3f));

        public NeuroWork(int offset, int number_of_passes, float edukationK)
        {
            neuroNet = NeuroNet.CreateNeuroNet(new int[] { offset * offset, offset, 1 }, new NeuroData(number_of_passes, edukationK));
            this.offset = offset;
            PreShow = new Bitmap(offset, offset);
            start_signals = new int[offset * offset];
        }

        public void Work()
        {
            NormalizePhoto();
            FindAllLines();
            DebugLines();
            FindAllLetters();
            //DrawLetters();
            //Part1();
        }

        public void DebugWork()
        {
            NormalizePhoto();
            FindAllLines();
            DebugLines();
            FindAllLetters();
            DrawLetters();
        }

        #region PreparationImage

        public void NormalizePhoto()
        {
            string ImageFile = "";

            OpenFileDialog ImageDialog = new OpenFileDialog
            {
                Title = "выбор файла для преобразования",

                Filter = "Image files|*.bmp;*.jpg;*.jpeg;*.png;*.tif|All files|*.*"
            };

            if (ImageDialog.ShowDialog() == DialogResult.OK)
            {
                ImageFile = ImageDialog.FileName;
            }
            else
                return;

            NormalizeImage = new Bitmap(ImageFile);
            float color_sum = 0;
            float max_color_cum = 550;

            for (int y = 0; y < NormalizeImage.Height; y++)
            {
                for (int x = 0; x < NormalizeImage.Width; x++)
                {
                    color_sum = NormalizeImage.GetPixel(x, y).R + NormalizeImage.GetPixel(x, y).G + NormalizeImage.GetPixel(x, y).B;
                    if (!IsBlackColor(NormalizeImage.GetPixel(x, y)))
                        NormalizeImage.SetPixel(x, y, Color.White);
                    else
                        NormalizeImage.SetPixel(x, y, Color.Black);
                }
            }
            Debug.form1.Picture.Image = NormalizeImage;
        }

        bool IsBlackColor(Color color)
        {
            if (color.R > 140 && (color.G + color.B) < 345)
                return true;
            else if (color.G > 140 && (color.R + color.B) < 345)
                return true;
            else if (color.B > 140 && (color.R + color.G) < 345)
                return true;
            else if ((color.R + color.G + color.B) < 475)
                return true;
            else
                return false;
        }

        void FindAllLines()
        {
            bool clear_line;
            //bool unclear_line = true;
            int last_up_y = 0;
            int last_down_y = 0;
            int y = 0;

            while (y < NormalizeImage.Height)
            {
                for (; y < NormalizeImage.Height; y++) // нахождение верхней границы строки
                {
                    clear_line = true;
                    for (int x = 0; x < NormalizeImage.Width; x++)
                    {
                        if (NormalizeImage.GetPixel(x, y).R < 100) //черный
                        {
                            clear_line = false;
                            break;
                        }
                    }
                    if (clear_line)
                    {
                        last_up_y = y;
                        //Debug.Log("new last_up_y = " + last_up_y);
                    }
                    else
                    {
                        break;
                    }
                }                                //////////////////////////////////////////

                for (; y < NormalizeImage.Height; y++) // нахождение нижней границы строки
                {
                    clear_line = true;
                    for (int x = 0; x < NormalizeImage.Width; x++)
                    {
                        if (NormalizeImage.GetPixel(x, y).R < 100) //черный
                        {
                            clear_line = false;
                            break;
                        }
                    }
                    if (clear_line)
                    {
                        //Debug.Log("new last_down_y = " + last_down_y);
                        last_down_y = y;
                        break;
                    }
                }                                //////////////////////////////////////////
                //Debug.Log("final y = " + last_up_y + ", " + last_down_y + " (" + (last_down_y - last_up_y) + ")");
                LineList.Add(new Line(last_up_y, last_down_y));
            }
            LineList.RemoveAt(LineList.Count - 1);
            Debug.Log("total number of items = " + LineList.Count);
        }

        void DrawLines()
        {
            for (int i = 0; i < LineList.Count; i++)
            {
                for (int x = 0; x < NormalizeImage.Width; x++)
                {
                    for (int y = LineList[i].up_y; y < LineList[i].down_y; y++)
                    {
                        if (NormalizeImage.GetPixel(x, y).R > 100)
                            NormalizeImage.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0));
                        else
                            NormalizeImage.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255));
                    }
                    NormalizeImage.SetPixel(x, LineList[i].down_y, Color.FromArgb(255, 255, 0, 0));
                }
            }
        }

        void DrawLetters()
        {
            int count = 0;
            for (int i = 0; i < LineList.Count; i++)
            {
                for (int k = 0; k < LineList[i].LetterList.Count; k++)
                {
                    for (int y = LineList[i].up_y; y < LineList[i].down_y; y++)
                    {
                        for (int x = LineList[i].LetterList[k].left_x; x < LineList[i].LetterList[k].right_x; x++)
                        {
                            if (NormalizeImage.GetPixel(x, y).R > 100) //белый
                            {
                                if (count % 2 == 0)
                                {
                                    NormalizeImage.SetPixel(x, y, Color.FromArgb(255, 0, 0, 255));
                                }
                                else
                                {
                                    NormalizeImage.SetPixel(x, y, Color.FromArgb(255, 255, 0, 0));
                                }
                            }
                            else
                            {
                                NormalizeImage.SetPixel(x, y, Color.FromArgb(255, 0, 255, 0));
                            }
                        }
                        
                    }
                    count++;
                }
            }
        }

        void DebugLines()
        {
            List<Line> LocalLineList = new List<Line>();
            float medium = 0;
            float max_value;
            float min_value;

            for (int i = 0; i < LineList.Count; i++)
            {
                medium += LineList[i].down_y - LineList[i].up_y;
            }
            medium = medium / LineList.Count;
            //Debug.Log("medium = " + medium);
            max_value = medium * 1.4f;
            min_value = medium * 0.6f;
            Debug.Log("ot " + min_value + " do " + max_value);

            for (int i = 0; i < LineList.Count; i++)
            {
                int lenght = LineList[i].down_y - LineList[i].up_y;
                if (lenght < max_value && lenght > min_value)
                {
                    LocalLineList.Add(LineList[i]);
                }
                else
                {
                    int center = (LineList[i].down_y - LineList[i].up_y) / 2;
                    LocalLineList.Add(new Line(LineList[i].up_y, LineList[i].up_y + center + 1));
                    LocalLineList.Add(new Line(LineList[i].up_y + center + 2, LineList[i].down_y));
                }
            }
            LineList = LocalLineList;
        }

        void FindAllLetters()
        {
            bool clear_column = true;
            int x = 0;
            int right_x = 0;
            int left_x = 0;
            int num_of_black_pixels = 0;

            for (int i = 0; i < LineList.Count; i++)
            {
                x = 0;
                while (x < NormalizeImage.Width)
                {
                    for (; x < NormalizeImage.Width; x++)
                    {
                        clear_column = true;
                        num_of_black_pixels = 0;
                        for (int y = LineList[i].up_y; y < LineList[i].down_y; y++)
                        {
                            if (NormalizeImage.GetPixel(x, y).R < 100)
                            {
                                if (num_of_black_pixels > 0)
                                {
                                    clear_column = false;
                                }
                                else
                                {
                                    num_of_black_pixels++;
                                }
                            }
                        }
                        if (clear_column)
                        {
                            left_x = x;
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (; x < NormalizeImage.Width; x++)
                    {
                        clear_column = true;
                        num_of_black_pixels = 0;
                        for (int y = LineList[i].up_y; y < LineList[i].down_y; y++)
                        {
                            if (NormalizeImage.GetPixel(x, y).R < 100)
                            {
                                if (num_of_black_pixels >= 0)
                                {
                                    clear_column = false;
                                }
                                else
                                {
                                    num_of_black_pixels++;
                                }
                            }
                        }
                        if (clear_column)
                        {
                            right_x = x;
                            break;
                        }
                    }
                    if (x < NormalizeImage.Width)
                        LineList[i].LetterList.Add(new Letter(left_x, right_x));
                    else
                    {
                        //tab;
                    }
                }
            }
        }

        #endregion PreparationImage

        #region NeuroNetPreparation

        int current_line_TrainingPass = 0;
        int current_column_TrainingPass = 0;
        public bool NextPreTrainingPass(char letter, bool need_save = true)
        {
            if (current_line_TrainingPass >= LineList.Count)
            {
                Debug.Log("изображение полностью обработано");
                return false;
            }
            if (LineList[current_line_TrainingPass].LetterList.Count == 0)
            {
                Debug.Log("в этой строке нет букв");
                current_column_TrainingPass = 0;
                current_line_TrainingPass++;
                return false;
            }
            int min_y = LineList[current_line_TrainingPass].up_y;
            int min_x = LineList[current_line_TrainingPass].LetterList[current_column_TrainingPass].left_x;
            ClearPreShow();
            ClearStartSignals();


            int[,] start_signals_pre = new int[offset, offset * 5];
            for (int y = 0; y < start_signals_pre.GetLength(1); y++)
                for (int x = 0; x < start_signals_pre.GetLength(0); x++)
                    start_signals_pre[x, y] = 255;

            for (int y = LineList[current_line_TrainingPass].up_y; y < LineList[current_line_TrainingPass].down_y; y++)
            {
                for (int x = LineList[current_line_TrainingPass].LetterList[current_column_TrainingPass].left_x; x < LineList[current_line_TrainingPass].LetterList[current_column_TrainingPass].right_x; x++)
                {
                    if (x - min_x < offset && y - min_y < offset)
                    {
                        Color color = NormalizeImage.GetPixel(x, y);

                        start_signals_pre[(x - min_x), (y - min_y)] = color.R;
                        NormalizeImage.SetPixel(x, y, Color.White);
                        start_signals[(x - min_x) + (y - min_y) * offset] = color.R;
                        
                    }
                    else
                    {
                        if (x - min_x < start_signals_pre.GetLength(0) && y - min_y < start_signals_pre.GetLength(1))
                        {
                            Color color = NormalizeImage.GetPixel(x, y);
                            start_signals_pre[(x - min_x), (y - min_y)] = color.R;
                        }
                    }
                }
            }


            bool free_line = true;
            int start_line = 0;
            for (int y = 0; y < start_signals_pre.GetLength(1); y++)
            {
                for (int x = 0; x < start_signals_pre.GetLength(0); x++)
                {
                    if (start_signals_pre[x, y] == 0)
                        free_line = false;
                }
                if (free_line)
                    start_line++;
                else
                    break;
            }

            int current_start_signal = 0;
            for (int y = start_line; y < start_signals_pre.GetLength(1); y++)
            {
                for (int x = 0; x < start_signals_pre.GetLength(0); x++)
                {
                    if (x < offset && y - start_line < offset)
                    {
                        Color color = Color.FromArgb(255, start_signals_pre[x, y], start_signals_pre[x, y], start_signals_pre[x, y]);
                        start_signals[current_start_signal] = start_signals_pre[x, y];
                        current_start_signal++;
                        PreShow.SetPixel(x, y - start_line, color);
                    }
                }
            }

            Debug.form1.Picture.Image = NormalizeImage;
            Debug.form1.pictureBox1.Image = PreShow;

            current_column_TrainingPass++;
            if (LineList[current_line_TrainingPass].LetterList.Count <= current_column_TrainingPass)
            {
                current_column_TrainingPass = 0;
                current_line_TrainingPass++;
            }

            if (need_save)
            {
                FileWork.SaveChar(letter, @"C:\Users\Admin\Desktop\ForText\" + (current_pass - 1) + ".txt");
                FileWork.SaveImage(PreShow, @"C:\Users\Admin\Desktop\ForText\" + (current_pass) + ".jpg");
                current_pass++;
            }
            return true;
        }

        /*int NormalizeColor(Color color)
        {
            int mono_color = color.R + color.G + color.B;
            if (mono_color > 255)
                mono_color = 255;
            return mono_color;
        }*/

        void ClearPreShow()
        {
            for (int y = 0; y < offset; y++)
            {
                for (int x = 0; x < offset; x++)
                {
                    PreShow.SetPixel(x, y, Color.White);
                }
            }
        }

        void ClearStartSignals()
        {
            for (int y = 0; y < offset; y++)
            {
                for (int x = 0; x < offset; x++)
                {
                    start_signals[x + y * offset] = 0;
                }
            }
        }

        /*public void Part1()
        {
            neuroNet.WorkPart1(start_signals);
        }*/

        /*public void Part2(int key_code)
        {
            float signal = key_code / 256f;
            neuroNet.WorkPart2(new float[] { signal });
        }*/

        public void ReadText(char letter)
        {
            NextPreTrainingPass(letter);
        }
        #endregion NeuroNet

        #region NeoroNetLerning

        public void StartLearning()
        {
            Thread thread = new Thread(new ThreadStart(StartLearningThread));
            thread.Start();
        }

        void StartLearningThread()
        {
            Debug.Log("начало обучения");
            Bitmap LetterImage;
            char letter;
            int count = 100000;
            float[] start_signals;
            float[] ideal_signals;

            //neuroNet = NeuroNet.CreateNeuroNet(new int[] {offset * offset, 42, 255 }, new NeuroData(1, 0.0002f));
            int load_neuroNet = 10800;//34500;
            neuroNet = NeuroNet.LoadNeuronet(@"C:\Users\Admin\Desktop\Letter\forNeuronet\NN" + load_neuroNet + ".NN");

            Debug.SetMaxValueProgressBar(count);
            for (int j = load_neuroNet + 1; j < count; j++)
            {
                neuroNet.ZeroingErrorOfEra();
                for (int i = 0; i < 1030; i++)
                {
                    StreamReader TextReader = new StreamReader(@"C:\Users\Admin\Desktop\ForText\" + i + ".txt");
                    LetterImage = new Bitmap(@"C:\Users\Admin\Desktop\ForText\" + i + ".jpg");
                    letter = TextReader.ReadLine()[0];
                    start_signals = GetNormalisedStartSignalsFromImage(LetterImage);
                    ideal_signals = GetNormalisedIdealSignals(letter);

                    neuroNet.Pass(start_signals, ideal_signals);
                }
                

                if (j % 50 == 0)
                {
                    if (j != 0)
                    {
                        neuroNet.SaveNeuronet(@"C:\Users\Admin\Desktop\Letter\forNeuronet\NN" + j + ".NN");
                        Debug.Log("завершено на " + (j * 100f / count) + "%");
                    }
                    else
                    {
                        neuroNet.SaveNeuronet(@"C:\Users\Admin\Desktop\Letter\forNeuronet\NN" + 0 + ".NN");
                    }
                    Debug.Log("error of era = " + neuroNet.ShowErrorOfEra());
                    //neuroNet.edukationK *= 0.99f;
                }
                //Debug.Log("error of era = " + neuroNet.ShowErrorOfEra());
                Debug.SetCurrentValueProgressBar(j);
                
            }
            neuroNet.SaveNeuronet(@"C:\Users\Admin\Desktop\Letter\forNeuronet\NN" + count + ".NN");
            Debug.Log("обучение завершено");
        }

        float[] GetNormalisedStartSignalsFromImage(Bitmap bitmap)
        {
            int current_pass = 0;
            float[] signals = new float[bitmap.Height * bitmap.Width];

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    signals[current_pass] = 1 - bitmap.GetPixel(x, y).R / 255f;
                    current_pass++;
                }
            }
            return signals;
        }

        float[] GetNormalisedIdealSignals(char letter)
        {
            float[] ideal_signals = new float[256];
            int letter_code = letter;

            for (int i = 0; i < ideal_signals.Length; i++)
            {
                if (i == letter_code)
                {
                    ideal_signals[i] = 0.8f;
                }
                else
                {
                    ideal_signals[i] = 0.2f;
                }
            }
            return ideal_signals;
        }

        #endregion NeoroNetLerning

        #region NeuroNetWork

        public void StartWriteText()
        {
            string NeuroNetFile = "";
            NeuroNet neuroNet;

            OpenFileDialog NeuroNetDialog = new OpenFileDialog
            {
                Title = "Выбор нейросети",

                Filter = "Neuronet files|*.NN|All files|*.*"
            };

            if (NeuroNetDialog.ShowDialog() == DialogResult.OK)
            {
                NeuroNetFile = NeuroNetDialog.FileName;
            }
            else return;

            neuroNet = NeuroNet.LoadNeuronet(NeuroNetFile);

            NormalizePhoto();
            FindAllLines();
            DebugLines();
            FindAllLetters();
            PreparationWriteTextPass();

            int k = 0;
            while (WriteTextPass(neuroNet) && k < 200000)
            {
                k++;
            }
            Debug.Log("Расшифровка изображения завершена");
            SaveFileDialog SaveTextDialog = new SaveFileDialog
            {
                Title = "SaveTextDialog",
                Filter = "Text|*.txt|All files|*.*"
            };
            if (SaveTextDialog.ShowDialog() == DialogResult.OK)
            {
                FileWork.SaveText(final_massage, SaveTextDialog.FileName);
            }
            //FileWork.SaveText(final_massage, @"C:\Users\Admin\Desktop\Letter\ForComplete\text.txt");
            
            Debug.Log("Текст скопирован в буфер обмена");
            Clipboard.SetText(final_massage);
        }

        void PreparationWriteTextPass()
        {
            current_line_WriteTextPass = 0;
            current_column_WriteTextPass = 0;
        }

        int current_line_WriteTextPass = 0;
        int current_column_WriteTextPass = 0;
        bool WriteTextPass(NeuroNet ReadTextNeuroNet)
        {
            if (current_line_WriteTextPass >= LineList.Count)
            {
                //Debug.Log("current_line_WriteTextPass >= LineList.Count");
                return false;
            }
            if (LineList[current_line_WriteTextPass].LetterList.Count == 0)
            {
                //Debug.Log("LineList[current_line_WriteTextPass].LetterList.Count == 0");
                current_column_WriteTextPass = 0;
                current_line_WriteTextPass++;
                final_massage += "\n ";
                //FileWork.SaveText("\n", @"C:\Users\Admin\Desktop\Letter\ForComplete\text.txt");
                return true;
            }
            int min_y = LineList[current_line_WriteTextPass].up_y;
            int min_x = LineList[current_line_WriteTextPass].LetterList[current_column_WriteTextPass].left_x;

            ClearStartSignals();

            int[,] start_signals_pre = new int[offset, offset * 5];
            for (int y = 0; y < start_signals_pre.GetLength(1); y++)
                for (int x = 0; x < start_signals_pre.GetLength(0); x++)
                    start_signals_pre[x, y] = 255;

            for (int i = 0; i < start_signals.Length; i++)
            {
                start_signals[i] = 1;
            }


            for (int y = LineList[current_line_WriteTextPass].up_y; y < LineList[current_line_WriteTextPass].down_y; y++)
            {
                for (int x = LineList[current_line_WriteTextPass].LetterList[current_column_WriteTextPass].left_x; x < LineList[current_line_WriteTextPass].LetterList[current_column_WriteTextPass].right_x; x++)
                {
                    if (x - min_x < offset && y - min_y < offset)
                    {
                        Color color = NormalizeImage.GetPixel(x, y);

                        start_signals_pre[(x - min_x), (y - min_y)] = color.R;
                        //NormalizeImage.SetPixel(x, y, Color.White);
                        start_signals[(x - min_x) + (y - min_y) * offset] = color.R;
                    }
                    else
                    {
                        if (x - min_x < start_signals_pre.GetLength(0) && y - min_y < start_signals_pre.GetLength(1))
                        {
                            Color color = NormalizeImage.GetPixel(x, y);
                            start_signals_pre[(x - min_x), (y - min_y)] = color.R;
                        }
                    }
                }
            }

            bool free_line = true;
            int start_line = 0;
            for (int y = 0; y < start_signals_pre.GetLength(1); y++)
            {
                for (int x = 0; x < start_signals_pre.GetLength(0); x++)
                {
                    if (start_signals_pre[x, y] == 0)
                        free_line = false;
                }
                if (free_line)
                    start_line++;
                else
                    break;
            }

            int current_start_signal = 0;
            for (int y = start_line; y < start_signals_pre.GetLength(1); y++)
            {
                for (int x = 0; x < start_signals_pre.GetLength(0); x++)
                {
                    if (x < offset && y - start_line < offset)
                    {
                        Color color = Color.FromArgb(255, start_signals_pre[x, y], start_signals_pre[x, y], start_signals_pre[x, y]);
                        start_signals[current_start_signal] = start_signals_pre[x, y];
                        current_start_signal++;
                    }
                }
            }

            float[] start_signals_float = GetNormarisedStartSignals(start_signals);
            float[] output_signals = ReadTextNeuroNet.PassOutput(start_signals_float);

            float max_value = 0;
            int letter = 0;

            for (int i = 0; i < output_signals.Length; i++)
            {
                if (output_signals[i] > max_value)
                {
                    max_value = output_signals[i];
                    letter = i;
                }
            }
            //Debug.Log("letter = " + (char)letter + " (" + letter + ")");
            if (letter > 30)
            {
                final_massage += (char)(letter);
                string mes = "";
                mes += (char)(letter);
                //FileWork.SaveText(mes, @"C:\Users\Admin\Desktop\Letter\ForComplete\text.txt");
            }
            else
            {
                final_massage += "?";
                //FileWork.SaveText("&", @"C:\Users\Admin\Desktop\Letter\ForComplete\text.txt");
            }


            current_column_WriteTextPass++;
            if (LineList[current_line_WriteTextPass].LetterList.Count <= current_column_WriteTextPass)
            {
                current_column_WriteTextPass = 0;
                current_line_WriteTextPass++;
                final_massage += "\n ";
                //FileWork.SaveText("\n", @"C:\Users\Admin\Desktop\Letter\ForComplete\text.txt");
            }

            return true;
        }


        float[] GetNormarisedStartSignals(int[] signals)
        {
            float[] start_signals = new float[signals.Length];

            for (int i = 0; i < signals.Length; i++)
            {
                start_signals[i] = 1 - signals[i] / 255f;
            }
            return start_signals;
        }
        #endregion NeuroNetWork
    }

    class Line
    {
        public int up_y;
        public int down_y;
        public List<Letter> LetterList = new List<Letter>();

        public Line(int up_y, int down_y)
        {
            this.up_y = up_y;
            this.down_y = down_y;
        }
    }

    class Letter
    {
        public int right_x;
        public int left_x;

        public Letter(int left_x, int right_x)
        {
            this.left_x = left_x;
            this.right_x = right_x;
        }
    }
}
