using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClarifaiApiClient;
using ClarifaiApiClient.Models;
namespace TestClarifaiApi
{
    class Program
    {
        static void Main(string[] args)
        {
            ClarifaiClient c = new ClarifaiClient("JU5zeSv_YJQG5THfAHwUvuD_oDFI13PqFKTXjKoS", "GQynbkt8D5mQcXUlZhcPqw7SAyQbTjj3WC1SYpkM");
            var task_r_ntf = c.GenerateToken();
            task_r_ntf.ConfigureAwait(true)
                .GetAwaiter()
                .OnCompleted(() =>
                {
                    try
                    {
                        if (task_r_ntf.IsCompleted)
                        {
                            if (task_r_ntf.Status == TaskStatus.RanToCompletion)
                            {
                                Task.Run(async () =>
                                {
                                    
                                    var p = await c.PredictByFolderPath(@"C:\D\Ken\LogoDetector\Photos\notworking\New folder");
                                    Console.WriteLine(p.Status.Code);
                                    Console.WriteLine(p.Status.Description);
                                });

                            }
                            else
                            {
                                Console.WriteLine("completed with errors " + task_r_ntf.Status);
                                if (task_r_ntf.Exception != null)
                                    if (task_r_ntf.Exception.InnerException != null)
                                        Console.WriteLine("Excepion : " + task_r_ntf.Exception.Message + " - Details : " + task_r_ntf.Exception.InnerException.Message);
                                    else
                                        Console.WriteLine("Excepion : " + task_r_ntf.Exception.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine("serving notifications failed");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Excepion : " + e.Message);
                    }
                });
            Console.ReadLine();
        }
        static void Main2(string[] args)
        {
            ClarifaiClient c = new ClarifaiClient("JU5zeSv_YJQG5THfAHwUvuD_oDFI13PqFKTXjKoS", "GQynbkt8D5mQcXUlZhcPqw7SAyQbTjj3WC1SYpkM");
            var task_r_ntf = c.GenerateToken();
            task_r_ntf.ConfigureAwait(true)
                .GetAwaiter()
                .OnCompleted(() =>
                {
                    try
                    {
                        if (task_r_ntf.IsCompleted)
                        {
                            if (task_r_ntf.Status == TaskStatus.RanToCompletion)
                            {
                                Task.Run(async () =>
                                {
                                    var p = await c.PredictByImgURL(new List<string> { "https://samples.clarifai.com/metro-north.jpg" });
                                    Console.WriteLine(p.Status.Code);
                                    Console.WriteLine(p.Status.Description);
                                });

                            }
                            else
                            {
                                Console.WriteLine("completed with errors " + task_r_ntf.Status);
                                if (task_r_ntf.Exception != null)
                                    if (task_r_ntf.Exception.InnerException != null)
                                        Console.WriteLine("Excepion : " + task_r_ntf.Exception.Message + " - Details : " + task_r_ntf.Exception.InnerException.Message);
                                    else
                                        Console.WriteLine("Excepion : " + task_r_ntf.Exception.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine("serving notifications failed");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Excepion : " + e.Message);
                    }
                });
            Console.ReadLine();
        }

        static void Main1(string[] args)
        {
            ClarifaiClient c = new ClarifaiClient("JU5zeSv_YJQG5THfAHwUvuD_oDFI13PqFKTXjKoS", "GQynbkt8D5mQcXUlZhcPqw7SAyQbTjj3WC1SYpkM");
            var task_r_ntf = c.GetToken();
            task_r_ntf.ConfigureAwait(true)
                .GetAwaiter()
                .OnCompleted(() =>
                {
                    try
                    {
                        if (task_r_ntf.IsCompleted)
                        {
                            if (task_r_ntf.Status == TaskStatus.RanToCompletion)
                            {
                                var res = task_r_ntf.Result;
                                if (res is TokenError)
                                    Console.WriteLine(((TokenError)res).Status_Msg);
                                else
                                    Console.WriteLine(((Token)res).Access_Token);

                            }
                            else
                            {
                                Console.WriteLine("completed with errors " + task_r_ntf.Status);
                                if (task_r_ntf.Exception != null)
                                    if (task_r_ntf.Exception.InnerException != null)
                                        Console.WriteLine("Excepion : " + task_r_ntf.Exception.Message + " - Details : " + task_r_ntf.Exception.InnerException.Message);
                                    else
                                        Console.WriteLine("Excepion : " + task_r_ntf.Exception.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine("serving notifications failed");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Excepion : " + e.Message);
                    }
                });
            Console.ReadLine();
        }
    }
}
