using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApplication
{
    class Program
    {
        static byte[] DecodeReadBytes(FileStream fs, int numberOfBytes)
        {
            byte[] ret = new byte[numberOfBytes]; 
            for(int i=0;i<numberOfBytes && fs.CanRead; i++)
            {
                ret[i] = (byte)(fs.ReadByte() & 0x7F);
            }
            return ret; 
        }

        static string DecodeReadBytesAsString(FileStream fs, int numberOfBytes)
        {
            byte[] bytes = ReadTillSpace(fs);
            return ASCIIEncoding.ASCII.GetString(bytes); 
        }

        static bool ReadSpace(FileStream fs)
        {
            return fs.ReadByte() == 0x0D &&
                fs.ReadByte() == 0x0A &&
                fs.ReadByte() == 0x0D &&
                fs.ReadByte() == 0x0A; 
        }

        static byte[] ReadTillSpace(FileStream fs)
        {
            List<byte> ret = new List<byte>();

            for(;;)
            {
                byte v = (byte)fs.ReadByte(); 
                v = (byte)(v & 0x7F);

                if(v == 0x0D)
                {
                    fs.ReadByte();
                    fs.ReadByte();
                    fs.ReadByte();

                    break; 
                }
                else
                {
                    ret.Add(v);
                }
            }

            return ret.ToArray(); 
        }

        static void Main(string[] args)
        {
            const string SanityString = "Scenarist_SCC V1.0"; 

            using (FileStream fs = File.OpenRead("test.scc"))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string firstLine = sr.ReadLine(); 

                    if(firstLine == SanityString)
                    {
                        for(;;)
                        {
                            string emptyLine = sr.ReadLine();
                            string contentLine = sr.ReadLine();

                            if(string.IsNullOrEmpty(contentLine))
                            {
                                break; 
                            }

                            int spaceIndex = contentLine.IndexOf(' '); 
                            if(spaceIndex >=0)
                            {
                                string timeStamp = contentLine.Substring(0, spaceIndex);

                                string remainder = contentLine.Substring(contentLine.IndexOf(' ') + 1);
                                string[] bits = remainder.Split(' ');

                                StringBuilder sb = new StringBuilder();
                                foreach (string bit in bits)
                                {
                                    if (!bit.StartsWith("9"))
                                    {
                                        byte firstByte = (byte)(byte.Parse(bit.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) & 0x7F);
                                        byte secondByte = (byte)(byte.Parse(bit.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) & 0x7F);

                                        if(firstByte >= 0x20)
                                        {
                                            sb.Append((char)firstByte);
                                            sb.Append((char)secondByte);
                                        }
                                    }
                                }

                                File.AppendAllText("C:/users/brush/desktop/decoded.txt", timeStamp + Environment.NewLine);
                                File.AppendAllText("C:/users/brush/desktop/decoded.txt", sb.ToString() + Environment.NewLine);
                                File.AppendAllText("C:/users/brush/desktop/decoded.txt", "========================" + Environment.NewLine);
                            }
                            else
                            {
                                File.AppendAllText("C:/users/brush/desktop/decoded.txt", contentLine + Environment.NewLine);
                                File.AppendAllText("C:/users/brush/desktop/decoded.txt", "========================" + Environment.NewLine);
                            }
                        }
                    }
                }
            }
        }
    }
}
