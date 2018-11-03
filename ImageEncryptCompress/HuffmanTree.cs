using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace ImageQuantization
{
    class Node
    {
        public int Symbol;
        public int freq;
        public Node rightChild;
        public Node leftChild;
        public bool RC, LC;
        public Node()
        {
            RC = LC = false;
        }
        public Node(int val, int valfreq)
        {
            Symbol = val;
            freq = valfreq;
            rightChild = new Node();
            leftChild = new Node();
            RC = LC = false;
        }
        public Node(Node node1, Node node2)
        {
            if (node1.freq >= node2.freq)
            {
                leftChild = node2;
                rightChild = node1;
            }
            else
            {
                leftChild = node1;
                rightChild = node2;
            }
            RC = LC = true;
            freq = node2.freq + node1.freq;
        }
    }
    class HuffmanTree
    {
        const string filename = "logs.dat";

        long total_memory_bytes;

        Node RootRed;
        Node RootGreen;
        Node RootBlue;

        // to store freqs for each color 
        int[] RedFreqs;
        int[] BlueFreqs;
        int[] GreenFreqs;

        // to store string of bits
        string[] huffmancodesred;
        string[] huffmancodesblue;
        string[] huffmancodesgreen;


        // PQs to Construct the tree 
        PriorityQueue<int, Node> RedNodes;
        PriorityQueue<int, Node> BlueNodes;
        PriorityQueue<int, Node> GreenNodes;

        private void push_to_pq(ref PriorityQueue<int, Node> pq, int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 0) continue;
                Node n = new Node(i, array[i]);
                pq.Enqueue(array[i], n);
            }
        }

        private void build_tree(ref PriorityQueue<int, Node> pq, ref Node root)
        {
            while (pq.Count > 1)
            {
                Node Parentnode = new Node(pq.Dequeue().Value, pq.Dequeue().Value);
                pq.Enqueue(Parentnode.freq, Parentnode);
                root = Parentnode;
            }
        }

        private void Set_huffmancode(ref string[] code, Node Parent, string huffman)
        {
            if (!Parent.LC && !Parent.RC)
            {
                code[Parent.Symbol] = huffman;
                return;
            }
            if (Parent.LC)
                Set_huffmancode(ref code, Parent.leftChild, huffman + "1");
            if (Parent.RC)
                Set_huffmancode(ref code, Parent.rightChild, huffman + "0");
        }

        private void ConstructHuffmanTree()
        {
            push_to_pq(ref RedNodes, RedFreqs);
            push_to_pq(ref GreenNodes, GreenFreqs);
            push_to_pq(ref BlueNodes, BlueFreqs);
            build_tree(ref RedNodes, ref RootRed);
            build_tree(ref GreenNodes, ref RootGreen);
            build_tree(ref BlueNodes, ref RootBlue);
            Set_huffmancode(ref huffmancodesred, RootRed, "");
            Set_huffmancode(ref huffmancodesgreen, RootGreen, "");
            Set_huffmancode(ref huffmancodesblue, RootBlue, "");
        }



        public HuffmanTree(RGBPixel[,] Image,long seed,short tap,short size)
        {
            //initializations
            total_memory_bytes = 0;
            RedFreqs = new int[256];
            GreenFreqs = new int[256];
            BlueFreqs = new int[256];
            huffmancodesred = new string[256];
            huffmancodesgreen = new string[256];
            huffmancodesblue = new string[256];
            RedNodes = new PriorityQueue<int, Node>();
            GreenNodes = new PriorityQueue<int, Node>();
            BlueNodes = new PriorityQueue<int, Node>();

            total_memory_bytes = 3 * Image.GetLength(0) * Image.GetLength(1);
            for (int i = 0; i < Image.GetLength(0); i++)
            {
                for (int j = 0; j < Image.GetLength(1); j++)
                {
                    RedFreqs[Image[i, j].red]++;
                    GreenFreqs[Image[i, j].green]++;
                    BlueFreqs[Image[i, j].blue]++;
                }
            }
            ConstructHuffmanTree();
            Print_log(Image,seed,tap,size);
        }

        public HuffmanTree()
        {
            //initializations
            total_memory_bytes = 0;
            RedFreqs = new int[256];
            GreenFreqs = new int[256];
            BlueFreqs = new int[256];
            huffmancodesred = new string[256];
            huffmancodesgreen = new string[256];
            huffmancodesblue = new string[256];
            RedNodes = new PriorityQueue<int, Node>();
            GreenNodes = new PriorityQueue<int, Node>();
            BlueNodes = new PriorityQueue<int, Node>();
        }



        public void Write_Trees()
        {
            double total = 0;
            StreamWriter SW = new StreamWriter("Logs.txt");
            SW.WriteLine("Red Huffman Tree");
            SW.WriteLine();
            SW.WriteLine("(Color, Frequency, Huffman Representation, Total Bits)");
            SW.WriteLine();
            double Redbits = 0, Bluebits = 0, Greenbits = 0;
            for (int i = 0; i < huffmancodesred.Length; i++)
            {
                if (huffmancodesred[i] != null)
                {
                    SW.WriteLine("( " + i + ", " + RedFreqs[i] + ", " + huffmancodesred[i] + ", " +
                        huffmancodesred[i].Length + " * " + RedFreqs[i].ToString() + " )");
                    Redbits += huffmancodesred[i].Length * RedFreqs[i];
                }
            }
            SW.WriteLine();
            SW.WriteLine("With total of " + Redbits / 8 + " bytes");
            SW.WriteLine();
            SW.WriteLine("Green Huffman Tree");
            SW.WriteLine();
            SW.WriteLine("(Color, Frequency, Huffman Representation, Total Bits)");
            SW.WriteLine();
            for (int i = 0; i < huffmancodesgreen.Length; i++)
            {
                if (huffmancodesgreen[i] != null)
                {
                    SW.WriteLine("( " + i + ", " + GreenFreqs[i] + ", " + huffmancodesgreen[i] + ", " +
                        huffmancodesgreen[i].Length + " * " + GreenFreqs[i].ToString() + " )");
                    Greenbits += huffmancodesgreen[i].Length * GreenFreqs[i];
                }
            }
            SW.WriteLine();
            SW.WriteLine("With total of " + Greenbits / 8 + " bytes");
            SW.WriteLine();
            SW.WriteLine("Blue Huffman Tree");
            SW.WriteLine();
            SW.WriteLine("(Color, Frequency, Huffman Representation, Total Bits)");
            SW.WriteLine();
            for (int i = 0; i < huffmancodesblue.Length; i++)
            {
                if (huffmancodesblue[i] != null)
                {
                    SW.WriteLine("( " + i + ", " + BlueFreqs[i] + ", " + huffmancodesblue[i] + ", " +
                        huffmancodesblue[i].Length + " * " + BlueFreqs[i].ToString() + " )");
                    Bluebits += huffmancodesblue[i].Length * BlueFreqs[i];
                }
            }
            SW.WriteLine();
            SW.WriteLine("With total of " + Bluebits / 8 + " bytes");
            SW.WriteLine();
            total += Redbits / 8;
            total += Greenbits / 8;
            total += Bluebits / 8;
            SW.WriteLine("Total memory after compression is " + Redbits / 8 + " + "
               + Bluebits / 8 + " + " + Greenbits / 8 + " = " + total + " bytes");
            SW.WriteLine("Compression ratio is " + total / total_memory_bytes * 100 + "%");
            SW.WriteLine("------------------------- End of logs ----------------------------");
            SW.Close();
            MessageBox.Show("Your image have been compressed successfuly");
            MessageBox.Show("Check the log file for more details");
        }
        ulong get_code(ref string s)
        {
            ulong ans = 0;
            int it = 63;
            for(int i = 0; i < s.Length; i++,it--)
            {
                if (s[i] == '1')
                {
                    ans |= (ulong)((ulong) 1 << it);
                }
            }
            return ans;
        }
        private void parse(ref BinaryReader reader)
        {
            int n = (int)reader.ReadUInt32();
            for (int i = 0; i < n; i++)
            {
                byte idx = reader.ReadByte();
                int freq = reader.ReadInt32();
                RedFreqs[idx] = freq;
            }
            n = (int)reader.ReadUInt32();
            for (int i = 0; i < n; i++)
            {
                byte idx = reader.ReadByte();
                int freq = reader.ReadInt32();
                GreenFreqs[idx] = freq;
            }
            n = (int)reader.ReadUInt32();
            for (int i = 0; i < n; i++)
            {
                byte idx = reader.ReadByte();
                int freq = reader.ReadInt32();
                BlueFreqs[idx] = freq;
            }
        }
        public RGBPixel[,] Decompress(string filename,ref long seed,ref short tap_pos,ref short size)
        {
            RGBPixel[,] arr;
            BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open));
            parse(ref reader);
            ConstructHuffmanTree();
            arr = decompress(ref reader);
            seed = reader.ReadInt64();
            tap_pos = reader.ReadInt16();
            size = reader.ReadInt16();
            reader.Close();
            return arr;
        }
        private RGBPixel[,] decompress(ref BinaryReader reader)
        {
            RGBPixel[,] ret;
            int L = reader.ReadInt32();
            int W = reader.ReadInt32();
            ret = new RGBPixel[L, W];
            Node curr = RootRed;
            int n = reader.ReadInt32();
            int l = 0, w = 0;
            while (n > 0)
            {
                n -= 8;
                byte tmp = reader.ReadByte();
                for (int i = 7; i >= 0; i--)
                {
                    if (((1 << i) & tmp) > 0)
                    {
                        curr = curr.leftChild;
                    }
                    else
                    {
                        curr = curr.rightChild;
                    }
                    if (!curr.RC && !curr.LC)
                    {
                        if(l == L)
                        {
                            continue;
                        }
                        ret[l, w].red = Convert.ToByte(curr.Symbol);
                        
                        w++;
                        if (w == W)
                        {
                            w = 0;
                            l++;
                        }
                        curr = RootRed;
                    }
                }
            }
            curr = RootGreen;
            n = reader.ReadInt32();
            l = 0;
            w = 0;
            while (n > 0)
            {
                n -= 8;
                byte tmp = reader.ReadByte();
                for (int i = 7; i >= 0; i--)
                {
                    if (((1 << i) & tmp) > 0)
                    {
                        curr = curr.leftChild;
                    }
                    else
                    {
                        curr = curr.rightChild;
                    }
                    if (!curr.LC && !curr.RC)
                    {
                        if (l == L)
                        {
                            continue;
                        }
                        ret[l, w].green = Convert.ToByte(curr.Symbol);
                        
                        w++;
                        if (w == W)
                        {
                            w = 0;
                            l++;
                        }
                        curr = RootGreen;
                    }
                }
            }
            curr = RootBlue;
            n = reader.ReadInt32();
            l = 0;
            w = 0;
            while (n > 0)
            {
                n -= 8;
                byte tmp = reader.ReadByte();
                for (int i = 7; i >= 0; i--)
                {
                    if (((1 << i) & tmp) > 0)
                    {
                        curr = curr.leftChild;
                    }
                    else
                    {
                        curr = curr.rightChild;
                    }
                    if (!curr.LC && !curr.RC)
                    {
                        if (l == L)
                        {
                            continue;
                        }
                        ret[l, w].blue = Convert.ToByte(curr.Symbol);
                        w++;
                        if (w == W)
                        {
                            w = 0;
                            l++;
                        }
                        curr = RootBlue;
                    }
                }
            }

            return ret;
        }
        private void Print_log(RGBPixel[,] Image,long seed,short tap,short size)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create));
            {
                int[] n = new int[3];
                n[0] = n[1] = n[2] = 0;

                int cnt = 0;
                for (int i = 0; i < huffmancodesred.Length; i++)
                {
                    if (huffmancodesred[i] != null) cnt++;
                }

                writer.Write(cnt);

                for (int i = 0; i < huffmancodesred.Length; i++)
                {
                    if (huffmancodesred[i] != null)
                    {
                        writer.Write(Convert.ToByte(i));
                        writer.Write(RedFreqs[i]);
                        n[0] += RedFreqs[i] * huffmancodesred[i].Length;
                    }
                }


                cnt = 0;
                for (int i = 0; i < huffmancodesgreen.Length; i++)
                {
                    if (huffmancodesgreen[i] != null) cnt++;
                }

                writer.Write(cnt);

                for (int i = 0; i < huffmancodesgreen.Length; i++)
                {
                    if (huffmancodesgreen[i] != null)
                    {
                        writer.Write(Convert.ToByte(i));
                        writer.Write(GreenFreqs[i]);
                        n[1] += GreenFreqs[i] * huffmancodesgreen[i].Length;
                    }
                }

                cnt = 0;
                for (int i = 0; i < huffmancodesblue.Length; i++)
                {
                    if (huffmancodesblue[i] != null) cnt++;
                }

                writer.Write(cnt);

                for (int i = 0; i < huffmancodesblue.Length; i++)
                {
                    if (huffmancodesblue[i] != null)
                    {
                        writer.Write(Convert.ToByte(i));
                        writer.Write(BlueFreqs[i]);
                        n[2] += BlueFreqs[i] * huffmancodesblue[i].Length;
                    }
                }

                writer.Write((int) Image.GetLength(0));
                writer.Write((int) Image.GetLength(1));

                n[0] = ((n[0] + 7) / 8) * 8;
                writer.Write(n[0]);
                Write_dat(Image, 0, ref writer, n[0]);

                n[1] = ((n[1] + 7) / 8) * 8;
                writer.Write(n[1]);
                Write_dat(Image, 1, ref writer, n[1]);

                n[2] = ((n[2] + 7) / 8) * 8;
                writer.Write(n[2]);
                Write_dat(Image, 2, ref writer, n[2]);

                writer.Write(seed);
                writer.Write(tap);
                writer.Write(size);
                writer.Close();

            }
        }
        private void Write_dat(RGBPixel[,] Image, int idx, ref BinaryWriter writer, int N)
        {
            int L = Image.GetLength(0), W = Image.GetLength(1);
            if (idx == 0)
            {
                char[] arr = new char[N];
                int len = 0;
                for (int i = 0; i < L; i++)
                {
                    for (int j = 0; j < W; j++)
                    {
                        for (int k = 0; k < huffmancodesred[Image[i, j].red].Length; k++)
                        {
                            arr[len++] = huffmancodesred[Image[i, j].red][k];
                        }
                    }
                }
                while (len < N) arr[len++] = '0';
                for(int i = 0; i < N; i+=8)
                {
                    byte tmp = 0;
                    for(int j = 0;j < 8; j++)
                    {
                        tmp <<= 1;
                        if (arr[i + j] == '1') tmp++;
                    }
                    writer.Write(tmp);
                }
            }
            else if (idx == 1)
            {
                char[] arr = new char[N];
                int len = 0;
                for (int i = 0; i < L; i++)
                {
                    for (int j = 0; j < W; j++)
                    {
                        for (int k = 0; k < huffmancodesgreen[Image[i, j].green].Length; k++)
                        {
                            arr[len++] = huffmancodesgreen[Image[i, j].green][k];
                        }
                    }
                }
                while (len < N) arr[len++] = '0';
                for (int i = 0; i < N; i += 8)
                {
                    byte tmp = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        tmp <<= 1;
                        if (arr[i + j] == '1') tmp++;
                    }
                    writer.Write(tmp);
                }
            }
            else
            {
                char[] arr = new char[N];
                int len = 0;
                for (int i = 0; i < L; i++)
                {
                    for (int j = 0; j < W; j++)
                    {
                        for (int k = 0; k < huffmancodesblue[Image[i, j].blue].Length; k++)
                        {
                            arr[len++] = huffmancodesblue[Image[i, j].blue][k];
                        }
                    }
                }
                while (len < N) arr[len++] = '0';
                int t = 0;
                for (int i = 0; i < N; i += 8)
                {
                    byte tmp = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        tmp <<= 1;
                        if (arr[i + j] == '1') tmp++;
                    }
                    writer.Write(tmp);
                    t += 8;
                }
            }

        }

    }
}
