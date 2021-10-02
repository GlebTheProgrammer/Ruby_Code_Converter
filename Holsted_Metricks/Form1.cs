using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace Holsted_Metricks
{
    public partial class Form1 : Form
    {

        private readonly string configPath = $"{Environment.CurrentDirectory}\\RubyCodeConfig.json";  // Определение пути к Config File
        private string[] codeArray;

        public Form1()
        {
            InitializeComponent();
        }

        private void ProgressBarUpload(int i)
        {
            if (i == progressBar.Maximum)
            {
                progressBar.Maximum = i + 1;
                progressBar.Value = i + 1;
                progressBar.Maximum = i;
            }
            else
            {
                progressBar.Value = i + 1;
            }
            progressBar.Value = i;
        }

        private async void UploadButton_Click(object sender, EventArgs e)
        {
            UploadButton.Enabled = false;          // Отключение возможности запускать несколько потоков при многократном нажатии на кнопку
            await Task.Run(() =>                             // Организация многопоточности для работы progressBar и взаимодействия с формой (+ асинхронность)
            {
                for (int i = 1; i <= 100; i++)
                {
                    Invoke(new Action(() =>           // Решение проблемы обращения к progressBar из другого потока
                    {
                        ProgressBarUpload(i);
                        Text = $"{i}%";
                    }));
                    Thread.Sleep(20);
                }
            });
            MessageBox.Show("Загрузка прошла успешно.");
            Text = "Holsted_Metricks_Calculation";
            ResultButton.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Width = 300;
            ResultButton.Enabled = false;

            try
            {
                var data = File.ReadAllText(configPath);

                codeArray = JsonConvert.DeserializeObject<string[]>(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ResultButton_Click(object sender, EventArgs e)
        {
            ResultButton.Enabled = false;
            int[] operators = new int[31];
            string[] operatorsName = new string[] {"+","-","*","fdiv","<",">","==","( )","<=",">=","!=","=","&&","!","||","^","/","%","<<",">>","in","begin..end / end","break","next","case..when..else","if..else","until","while","for..in","puts","gets.chomp" };

            for (int i = 0; i < codeArray.Length; i++)
            {
                textBox1.Text = textBox1.Text + codeArray[i] + Environment.NewLine;
                bool goNextStr = false;
                for (int j = 0; j < codeArray[i].Length; j++)
                {
                    switch (codeArray[i][j])
                    {
                        case '\\':
                            if (codeArray[i][j + 1] == '\\')
                                goNextStr = true;
                            break;

                        case '+':
                            operators[0]++;
                            break;

                        case '-':
                            operators[1]++;
                            break;

                        case '*':
                            operators[2]++;
                            break;

                        case 'f':
                            if (codeArray[i][j + 1] == 'd' && codeArray[i][j + 2] == 'i' && codeArray[i][j + 3] == 'v')
                                operators[3]++;
                            else
                            {
                                if (codeArray[i][j + 1] == 'o' && codeArray[i][j + 2] == 'r' && codeArray[i][j+3] == ' ')
                                    operators[28]++;
                            }
                            break;

                        case '<':
                            if (codeArray[i][j - 1] != '<' && codeArray[i][j + 1] != '=' && codeArray[i][j + 1] != '<')
                                operators[4]++;
                            else
                            {
                                if (codeArray[i][j + 1] == '=')
                                    operators[8]++;
                                else
                                {
                                    if (codeArray[i][j + 1] == '<')
                                        operators[18]++;
                                }
                            }
                            break;

                        case '>':
                            if (codeArray[i][j - 1] != '>' && codeArray[i][j + 1] != '=' && codeArray[i][j + 1] != '>')
                                operators[5]++;
                            else
                            {
                                if (codeArray[i][j + 1] == '=')
                                    operators[9]++;
                                else
                                {
                                    if (codeArray[i][j + 1] == '>')
                                        operators[19]++;
                                }
                            }
                            break;

                        case '=':
                            if (codeArray[i][j + 1] == '=')
                                operators[6]++;
                            else
                            {
                                if (codeArray[i][j - 1] != '<' && codeArray[i][j - 1] != '>' && codeArray[i][j - 1] != '=' && codeArray[i][j - 1] != '!')
                                    operators[11]++;
                                else
                                {
                                    if (codeArray[i][j - 1] == '!')
                                        operators[10]++;
                                }
                            }
                            break;

                        case '(':
                            if (codeArray[i][j - 1] != '!' && codeArray[i][j + 1] != '\"')
                                operators[7]++;
                            break;

                        case '&':
                            if (codeArray[i][j + 1] == '&')
                                operators[12]++;
                            break;

                        case '!':
                            if (codeArray[i][j + 1] != '=')
                                operators[13]++;
                            break;

                        case '|':
                            if (codeArray[i][j + 1] == '|')
                                operators[14]++;
                            break;

                        case '^':
                            operators[15]++;
                            break;

                        case '/':
                            if (codeArray[i][j + 1] != '/' && codeArray[i][j - 1] != '/')
                                operators[16]++;
                            break;

                        case '%':
                            operators[17]++;
                            break;

                        case 'i':
                            if ((j == 0 || codeArray[i][j - 1] == ' ') && codeArray[i][j + 1] == 'n' && codeArray[i][j + 2] == ' ')
                                operators[20]++;
                            else
                            {
                                if ((j == 0 || codeArray[i][j - 1] == ' ' || codeArray[i][j-1] == '\t') && codeArray[i][j + 1] == 'f' && codeArray[i][j + 2] == ' ')
                                    operators[25]++;
                            }
                            break;

                        case 'e':
                            if (j != codeArray[i].Length-1 && codeArray[i][j + 1] == 'n' && codeArray[i][j + 2] == 'd' && (j + 2 == codeArray[i].Length - 1 || (codeArray[i][j+3] == ' ' && codeArray[i][j+4] == 'u')))
                                operators[21]++;
                            break;

                        case 'b':
                            if (codeArray[i][j + 1] == 'r' && codeArray[i][j + 2] == 'e' && codeArray[i][j + 3] == 'a' && codeArray[i][j + 4] == 'k' && j + 4 == codeArray[i].Length - 1)
                                operators[22]++;
                            break;

                        case 'n':
                            if (j != codeArray[i].Length - 1 && codeArray[i][j + 1] == 'e' && codeArray[i][j + 2] == 'x' && codeArray[i][j + 3] == 't' && j + 3 == codeArray[i].Length - 1)
                                operators[23]++;
                            break;

                        case 'c':
                            if ((j == 0 || codeArray[i][j - 1] == ' ') && codeArray[i][j + 1] == 'a' && codeArray[i][j + 2] == 's' && codeArray[i][j + 3] == 'e' && codeArray[i][j + 4] == ' ')
                                operators[24]++;
                            break;

                        case 'u':
                            if ((j == 0 || codeArray[i][j - 1] == ' ') && codeArray[i][j + 1] == 'n' && codeArray[i][j + 2] == 't' && codeArray[i][j + 3] == 'i' && codeArray[i][j + 4] == 'l' && codeArray[i][j + 5] == ' ')
                                operators[26]++;
                            break;

                        case 'w':
                            if (codeArray[i][j + 1] == 'h' && codeArray[i][j + 2] == 'i' && codeArray[i][j + 3] == 'l' && codeArray[i][j + 4] == 'e' && codeArray[i][j + 5] == ' ')
                                operators[27]++;
                            break;

                        case 'p':
                            if (j+4 <= codeArray[i].Length && codeArray[i][j + 1] == 'u' && codeArray[i][j + 2] == 't' && codeArray[i][j + 3] == 's' && codeArray[i][j + 4] == ' ')
                                operators[29]++;
                            break;

                        case 'g':
                            if (j + 4 <= codeArray[i].Length && codeArray[i][j + 1] == 'e' && codeArray[i][j + 2] == 't' && codeArray[i][j + 3] == 's' && codeArray[i][j + 4] == '.')
                                operators[30]++;
                            break;

                        default:
                            break;
                    }
                    if (goNextStr)
                        break;
                }
            }

            int n1 = 1;
            int N1 = 0;
            for (int i = 0; i < operators.Length; i++)
            {
                if (operators[i] != 0)
                {
                    textBox2.Text = textBox2.Text + n1 + ")\t" + operatorsName[i] + "\t\t" + operators[i] + Environment.NewLine;
                    N1 += operators[i];
                    n1++;
                }
                if (i+1 == operators.Length)
                {
                    n1--;
                    textBox2.Text = textBox2.Text + Environment.NewLine + "n1 = " + n1 + "\t\t       N1 = " + N1;
                }
            }

            List<string> operandsList = new List<string>();

            for (int i = 0; i < codeArray.Length; i++)  // Цикл, который собирает все все операнды, кроме входящих в скобки и после знака = (Все операнды до знака равно)
            {
                for (int j = 0; j < codeArray[i].Length; j++)
                {
                    if (codeArray[i][j] == '=' && codeArray[i][j-1] == ' ' && codeArray[i][j+1] == ' ')
                    {
                        string tempStr = "";
                        int k = 0;
                        for (; k < codeArray[i].Length; k++)
                        {
                            if (codeArray[i][k] != '\t' && codeArray[i][k] != '[')
                                tempStr += codeArray[i][k];
                            if ((codeArray[i][k + 1] == ' ' && codeArray[i][k + 2] == '=') || codeArray[i][k] == '[')
                                break;
                        }
                        if (!operandsList.Contains(tempStr))
                            operandsList.Add(tempStr);

                        break;
                    } 
                }
            }
            
            for (int i = 0; i < codeArray.Length; i++)  // Цикл, который собирает операнды после знака =
            {
                for (int j = 0; j < codeArray[i].Length; j++)
                {
                    if (codeArray[i][j] == '=' && codeArray[i][j+1] == ' ' && int.TryParse(Convert.ToString(codeArray[i][j+2]),out _))
                    {
                        string tempOperand = "";
                        for (int k = j+2; k < codeArray[i].Length; k++)
                        {
                            tempOperand +=  codeArray[i][k];
                        }
                        if (tempOperand.Length == 1)
                        {
                            if (!operandsList.Contains(tempOperand))
                                operandsList.Add(tempOperand);
                            break;
                        }
                        for (int k = 0, tempIndex = 0; k < tempOperand.Length; k++)
                        {
                            if (tempOperand[k] == '&' || tempOperand[k] == '|') // Проверка на возможное попадание условного выражения, которое не должно проверяться
                                break;
                            if (tempOperand[k] == ' ')
                            {
                                if ( int.TryParse(Convert.ToString(tempOperand[k-1]),out _ ))
                                {
                                    if (!operandsList.Contains(tempOperand.Substring(tempIndex, k - tempIndex)))
                                        operandsList.Add(tempOperand.Substring(tempIndex, k - tempIndex));//////
                                    tempIndex = k + 3;
                                }
                                continue;
                            }
                            if (k + 1 == tempOperand.Length)
                            {
                                if (!operandsList.Contains(tempOperand.Substring(tempIndex, k - tempIndex+1)))
                                    operandsList.Add(tempOperand.Substring(tempIndex, k - tempIndex+1));
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            for (int i = 0; i < codeArray.Length; i++)  // Цикл, который собирает операнды в скобках
            {
                for (int j = 0; j < codeArray[i].Length; j++)
                {
                    if (codeArray[i][j] == '[')
                    {
                        j++;
                        string tempString = "";
                        while (codeArray[i][j] != ']')
                        {
                            tempString += codeArray[i][j];
                            j++;
                        }    

                        for (int k = 0, tempIndex = 0; k < tempString.Length; k++)
                        {
                            if (tempString[k] == ' ')
                            {
                                if (tempString[k-1] != '-' && tempString[k - 1] != '+' && tempString[k - 1] != '*')
                                {
                                    if (!operandsList.Contains(tempString.Substring(tempIndex, k - tempIndex)))
                                        operandsList.Add(tempString.Substring(tempIndex, k - tempIndex));
                                    tempIndex = k + 3;
                                }
                                continue;
                            }
                            if (k + 1 == tempString.Length)
                            {
                                if (!operandsList.Contains(tempString.Substring(tempIndex, k - tempIndex + 1)))
                                    operandsList.Add(tempString.Substring(tempIndex, k - tempIndex + 1));
                                break;
                            }
                        }
                    }
                }
            }

            string[] operands = operandsList.ToArray();
            int[] operandsCount = new int[operands.Length];

            string temp;
            int index;
            for (int i = 0; i < codeArray.Length; i++)
            {
                if (codeArray[i].Contains("\\\\"))  // Если строка содержит комментарий, пропускаем
                    continue;
                while (codeArray[i].Length != 0)
                {
                    index = 0;

                    if (codeArray[i].Contains("\t"))  // Если в строке содержатся долгие пробелы - удаляем их 
                    {
                        codeArray[i] = codeArray[i].Remove(0, codeArray[i].IndexOf("\t")+1); // +1, т.к. элемент расположен на нулевом индексе, а удалить надо 1 элемент
                        continue;
                    }

                    if (codeArray[i].Contains("\""))  // Удаляет текст в двойных кавычках
                    {
                        int finishIndex = 0;
                        int startIndex = codeArray[i].IndexOf("\"");
                        for (int g = codeArray[i].IndexOf("\"")+1; g < codeArray[i].Length; g++)
                        {
                            if (codeArray[i][g] == '\"')
                            {
                                finishIndex = g;
                                break;
                            }
                        }
                        codeArray[i] = codeArray[i].Remove(startIndex, finishIndex - startIndex+1);
                        continue;
                    }

                    if (codeArray[i].Contains("  "))
                    {
                        codeArray[i] = codeArray[i].Remove(codeArray[i].IndexOf("  "), 1);
                        continue;
                    }

                    if (codeArray[i].Contains("(") || codeArray[i].Contains(")") || codeArray[i].Contains("[") || codeArray[i].Contains("]") || codeArray[i].Contains("?") || codeArray[i].Contains("!("))
                    {
                        codeArray[i] = codeArray[i].Replace("!(", " ");
                        codeArray[i] = codeArray[i].Replace('(', ' ');
                        codeArray[i] = codeArray[i].Replace(')', ' ');
                        codeArray[i] = codeArray[i].Replace('[', ' ');
                        codeArray[i] = codeArray[i].Replace(']', ' ');
                        codeArray[i] = codeArray[i].Replace('?', ' ');
                        continue;
                    }

                    while (index != codeArray[i].Length - 1 && codeArray[i][index] != ' ')  // Поиск очередного слова для анализа
                        index++;

                    if (index == codeArray[i].Length-1)
                        temp = codeArray[i].Substring(0, index + 1); /////// Условия не было (выводит 15 mainStr)
                    else
                        temp = codeArray[i].Substring(0, index); /////Записываем слово в temp

                    if (index < codeArray[i].Length)  // Если это не конец строки, то добавляем пробел
                    { 
                        temp += " ";
                        index++;
                    } 

                    if (temp.Contains("  "))
                        temp = temp.Remove(temp.IndexOf("  "), 1);

                    if (temp.Contains(".") && temp != ".. ") // Если temp содержит точку, то необходимо удалить всё, начиная с этой точки и заканчивая индексом начала пробела
                    {
                        if (temp[temp.Length-1] == ' ')  // Удаление из temp
                            temp = temp.Remove(temp.IndexOf("."), temp.Length - temp.IndexOf(".")-1); // -1 потому что length возвращает на 1 элемент больше, а нам нужно сохранить пробел
                        else
                            temp = temp.Remove(temp.IndexOf("."), temp.Length - temp.IndexOf(".")+1); /////// По логике, это никогда не должно сработать, т.к. на if сверху у нас всегда добавляет пробел

                        if (codeArray[i][codeArray[i].Length - 1] == ' ')  // Удаление из главной строки
                            codeArray[i] = codeArray[i].Remove(codeArray[i].IndexOf("."), codeArray[i].Length - codeArray[i].IndexOf("."));
                        else
                            codeArray[i] = codeArray[i].Remove(codeArray[i].IndexOf("."), codeArray[i].Length - codeArray[i].IndexOf("."));
                        index = temp.Length-1;
                    }

                    for (int k = 0; k < operands.Length; k++)  // Перебор всех операндов на совпадение (цикл для нормальных слов)
                    {
                        if (temp.Contains(operands[k]+" ") && temp.Length == operands[k].Length + 1) // Если один из опеандов подошёл, удаляем его из строки и увеличиваем кол-во совпадений 
                        {
                            operandsCount[k]++;
                            codeArray[i] = codeArray[i].Remove(0, index);
                            temp = "";
                            break;
                        }
                    }
                    if (temp != "")  // Если совпадения не были найдены - прото удаляем слово из строки
                        codeArray[i] = codeArray[i].Remove(0, index);
                }
            }

            int n2 = 1;
            int N2 = 0;
            for (int i = 0; i < operands.Length || operands.Length == 0; i++)
            {
                if (operands.Length != 0)
                {
                    textBox3.Text = textBox3.Text + n2 + ")\t" + operands[i] + "\t\t" + operandsCount[i] + Environment.NewLine;
                    N2 += operandsCount[i];
                    n2++;
                }
                if (i + 1 == operands.Length || operands.Length == 0)
                {
                    n2--;
                    textBox3.Text = textBox3.Text + Environment.NewLine + "n2 = " + n2 + "\t\t       N2 = " + N2;
                    break;
                }
            }

            label8.Text = $"Словарь программы n = {n1} + {n2} = {n1 + n2}";
            label9.Text = $"Длина программы N = {N1} + {N2} = {N1 + N2}";
            label10.Text = $"Объём программы V = {N1+N2}log2({n1+n2}) = {(N1 + N2) * Math.Log((n1+n2),2)}";

            this.Width = 1400;
        }
    }
}
