using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hamming
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Структура "узел H-дерева"
        /// </summary>
        public struct huffmannTreeNode
        {
            /// <summary>
            /// Текст
            /// </summary>
            public String text;
            /// <summary>
            /// Двоичный код
            /// </summary>
            public String code;
            /// <summary>
            /// Частота встречаемости
            /// </summary>
            public int frequency;

            public huffmannTreeNode(String t, String c, int f)
            {
                text = t;
                code = c;
                frequency = f;
            }
        };

        /// <summary>
        /// Частота встречаемости отдельных символов алфавита
        /// </summary>
        Dictionary<char, int> freqs = new Dictionary<char, int>();
        /// <summary>
        /// Исходное дерево
        /// </summary>
        List<huffmannTreeNode> source = new List<huffmannTreeNode>();
        /// <summary>
        /// Вспомогательное дерево
        /// </summary>
        List<huffmannTreeNode> newRes = new List<huffmannTreeNode>();
        /// <summary>
        /// Еще какое-то дерево
        /// </summary>
        List<huffmannTreeNode> tree = new List<huffmannTreeNode>();
        
        /// <summary>
        /// Сортировка узлов дерева по убыванию
        /// </summary>
        void sortTree()
        {
            for (int index = 0; index < tree.Count - 1; index++)
            {
                for (int index2 = index; index2 < tree.Count; index2++)
                {
                    if (tree[index].frequency < tree[index2].frequency)
                    {
                        huffmannTreeNode buf = tree[index];
                        tree[index] = tree[index2];
                        tree[index2] = buf;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String text = richTextBox1.Text;

            //Считаем частоту
            for (int index = 0; index < richTextBox1.Text.Length; index++)
            {
                if (freqs.Keys.Contains(text[index]))
                {
                    freqs[text[index]]++;
                }
                else
                {
                    freqs.Add(text[index], 1);
                }
            }

            //Начальное заполнение деревьев
            foreach (KeyValuePair<char, int> Pair in freqs)
            {
                source.Add(new huffmannTreeNode(Pair.Key.ToString(), "", Pair.Value));
                tree.Add(new huffmannTreeNode(Pair.Key.ToString(), "", Pair.Value));
                newRes.Add(new huffmannTreeNode(Pair.Key.ToString(), "", Pair.Value));
            }

            //Переводим в байты
            String textAsBytes = "";
            BitArray btr = new BitArray(Encoding.ASCII.GetBytes(text));

            for (int index = 0; index < btr.Count; index++)
            {
                textAsBytes += (btr[index] ? "1" : "0");
            }

            label1.Text = btr.Count.ToString() + "(" + freqs.Count.ToString() + ")";
            richTextBox2.Text = textAsBytes;




            //Строим дерево
            while (tree.Count > 1)
            {
                sortTree();

                for (int index = 0; index < source.Count; index++)
                {
                    if (tree[tree.Count - 2].text.Contains(source[index].text))
                    {
                        newRes[index] = new huffmannTreeNode(newRes[index].text, "0" + newRes[index].code, newRes[index].frequency);
                    }
                    else if (tree[tree.Count - 1].text.Contains(source[index].text))
                    {
                        newRes[index] = new huffmannTreeNode(newRes[index].text, "1" + newRes[index].code, newRes[index].frequency);
                    }
                }

                tree[tree.Count - 2] = new huffmannTreeNode(tree[tree.Count - 2].text + tree[tree.Count - 1].text, "",
                    tree[tree.Count - 2].frequency + tree[tree.Count - 1].frequency);
                tree.RemoveAt(tree.Count - 1);
            }

            //Выводим алфавит на экран
            treeView1.Nodes.Clear();
            for (int index = 0; index < source.Count; index++)
            {
                treeView1.Nodes.Add(newRes[index].text + " (" + newRes[index].code + ")");
            }

            //Битовая последовательность с новыми кодами
            for (int index = 0; index < text.Length; index++)
            {
                foreach (huffmannTreeNode htn in newRes)
                {
                    if (htn.text == text[index].ToString())
                    {
                        richTextBox4.Text += htn.code;
                    }
                }
            }

            label2.Text = richTextBox4.Text.Length.ToString();
            
            //Кодируем обратно в буквы
            String encodedText = richTextBox4.Text;
            for (int index = 0; index < encodedText.Length - 7; index += 8)
            {
                int code = 0;
                for (int ind = 0; ind < 8; ind++)
                {
                    if (encodedText[index + ind] == '1')
                    {
                        code += Convert.ToInt32(Math.Pow(2, 7 - ind));
                    }
                }
                richTextBox3.Text += (char)code;
            }

            //Дописываем алфавит в новую строку (чтобы раскодировать было можно)
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
