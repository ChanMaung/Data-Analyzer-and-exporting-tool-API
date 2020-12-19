using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace A6
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string Data(string values, string op) // takes a string and depending on operation chosen,will write,remove, or delete a file. Will return the content in file as string.
        {
            int v = -112;
            try
            {
                v = Convert.ToInt32(values);
            }
            catch
            {
                return "Please enter a single number";
            }
            string fileName = "file.txt";
            string fLocation = Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data"); 
            fLocation = Path.Combine(fLocation, fileName); // From current to App_Data
            //File.CreateText(fLocation);
            if (op == "Add") // write to file
            {
                using (StreamWriter sw = File.AppendText(fLocation))
                {
                    sw.WriteLine(values);
                }

            }
            else if(op == "Remove") // remove a value from file 
            {
                int index;
                string searchFor = values;
                string[] lines = File.ReadAllLines(fLocation);
                for (int i = 0 ; i < lines.Length ; i++)
                {
                    if (lines[i].Contains(searchFor) )
                    {
                        index = i;
                    }
                }
               // File.WriteAllLines(fLocation,lines);
                var list1 = new List<string>(lines);
                list1.Remove(values);
                lines = list1.ToArray();
                File.WriteAllLines(fLocation, lines);

            }
            else if(op == "Reset") // delete all content in file
            {
                File.Delete(fLocation);
               
            }

            if (!File.Exists(fLocation))
            {
                return "List has been reset.";
            }
            using (StreamReader sr = File.OpenText(fLocation)) // Read text file and convert to string to return
            {
                string[] lines = File.ReadAllLines(fLocation);
                string p = String.Join(",", lines);
                string re = "Lists: " + p;
               // string s = sr.ReadLine();
                return re;
            }


        }


        public string Math(string op) // Perform different match function depending on operation specific file
        {
            string fileName = "file.txt";
            string fLocation = Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data");
            fLocation = Path.Combine(fLocation, fileName); // From current to App_Data

            if (!File.Exists(fLocation))
            {
                return "No list data to analyze, please add data to list using Data Service.";
            }

            string[] lines = File.ReadAllLines(fLocation);  // Convert text file to string array
            int[] ilists = new int[lines.Length];
            int numValue;

            for (int i = 0; i < lines.Length; i++)      // Convert string array to int array
            {
                numValue = Convert.ToInt32(lines[i]);
                ilists[i] = numValue;
            }
        
            if (op == "min") //find min
            {
                Array.Sort(ilists);
                string p = ilists[0].ToString();
                return p;
            }
            else if(op == "max")//find max
            {
                Array.Sort(ilists);
                int lastE = ilists.Length - 1;
                string p = ilists[lastE].ToString();
                return p;
            }
            else if(op == "mean")//find mean
            {
                int sum = 0;
                for (int i = 0; i < ilists.Length; i++)
                {
                    sum = sum + ilists.ElementAt(i);
                }
                sum = sum / ilists.Length;
                string p = sum.ToString();
                return p;
            }
            else if(op == "median") // find median
            {
                Array.Sort(ilists);
                int mLocation = 0;
                int median = 0;
                if (ilists.Length % 2 != 0)
                {
                    mLocation = ilists.Length / 2;
                    median = ilists.ElementAt(mLocation);
                }
                else
                {
                    mLocation = ilists.Length / 2;
                    int median1 = ilists.ElementAt(mLocation);
                    int median2 = ilists.ElementAt(mLocation - 1);
                    median = ((median1 + median2) / 2);
                }
                string p = median.ToString();
                return p;
            }
            else if(op == "mode") //find mode, if no mode, return first int in array
            {
                Array.Sort(ilists);
                int count = 0;
                int most = 0;
                float currentMode;
                float mode = ilists.ElementAt(0);
                for (int i = 0; i < ilists.Length; i++)
                {
                    currentMode = ilists.ElementAt(i);
                    count = 0;
                    for (int j = 0; j < ilists.Length; j++)
                    {
                        if (ilists.ElementAt(j) == currentMode)
                        {
                            count++;
                        }
                        if (count > most)
                        {
                            most++;
                            mode = ilists.ElementAt(j);
                        }
                    }
                }
                string p = mode.ToString();
                return p;
            }
            return "error";
        }

        public string ExportResult(string fileName1) //save a file with math function done to the server
        {
            string fileName = fileName1;
            string fLocation = Path.Combine(HttpRuntime.AppDomainAppPath, @"UData");
            fLocation = Path.Combine(fLocation, fileName); // From current to App_Data

          
            // Perform math function and write to file
            
            string min = Math("min");
            using (StreamWriter sw = File.AppendText(fLocation))
            {
                sw.WriteLine("min: " + min);
            }

            string max = Math("max");
            using (StreamWriter sw = File.AppendText(fLocation))
            {
                sw.WriteLine("max: " + max);
            }

            string mean = Math("mean");
            using (StreamWriter sw = File.AppendText(fLocation))
            {
                sw.WriteLine("mean: " + mean);
            }

            string mode = Math("mode");
            using (StreamWriter sw = File.AppendText(fLocation))
            {
                sw.WriteLine("mode: " + mode);
            }

            string median = Math("median");
            using (StreamWriter sw = File.AppendText(fLocation))
            {
                sw.WriteLine("median: " + median);
            }
            string urlLocation = "http://aspnet-2jtwjvzkhcayq.azurewebsites.net/UData/" + fileName; // return the URL adress
            return "File exported at: " + urlLocation;

        }

    }
}
