using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using System.Collections.Generic;     // for KeyValuePair<>
using Microsoft.Azure.Devices.Shared; // for TwinCollection
using Newtonsoft.Json;                // for JsonConvert

#pragma warning disable CS4014


namespace HVACModule
{

    class Program
    {
        static int counter = 0;
        static int dataSentInterval { get; set; } = 10;


        class MessageBody
        {
            public double temperature { get; set; }
            public int energy { get; set;}
            public int humidity { get; set;}
            public double light { get; set; }
            public string timeCreated {get; set;}
        }

        static void Main(string[] args)
        {
            // The Edge runtime gives us the connection string we need -- it is injected as an environment variable
            string connectionString = Environment.GetEnvironmentVariable("EdgeHubConnectionString");

            // Cert verification is not yet fully functional when using Windows OS for the container
            bool bypassCertVerification = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (!bypassCertVerification) InstallCert();
            Init(connectionString, bypassCertVerification).Wait();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            WhenCancelled(cts.Token).Wait();
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Add certificate in local cert store for use by client for secure connection to IoT Edge runtime
        /// </summary>
        static void InstallCert()
        {
            string certPath = Environment.GetEnvironmentVariable("EdgeModuleCACertificateFile");
            if (string.IsNullOrWhiteSpace(certPath))
            {
                // We cannot proceed further without a proper cert file
                Console.WriteLine($"Missing path to certificate collection file: {certPath}");
                throw new InvalidOperationException("Missing path to certificate file.");
            }
            else if (!File.Exists(certPath))
            {
                // We cannot proceed further without a proper cert file
                Console.WriteLine($"Missing path to certificate collection file: {certPath}");
                throw new InvalidOperationException("Missing certificate file.");
            }
            X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(new X509Certificate2(X509Certificate2.CreateFromCertFile(certPath)));
            Console.WriteLine("Added Cert: " + certPath);
            store.Close();
        }


        /// <summary>
        /// Initializes the DeviceClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        static async Task Init(string connectionString, bool bypassCertVerification = false)
        {
            Console.WriteLine("Connection String {0}", connectionString);

            MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
            // During dev you might want to bypass the cert verification. It is highly recommended to verify certs systematically in production
            if (bypassCertVerification)
            {
                mqttSetting.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }
            ITransportSettings[] settings = { mqttSetting };

            // Open a connection to the Edge runtime
            DeviceClient ioTHubModuleClient = DeviceClient.CreateFromConnectionString(connectionString, settings);
            await ioTHubModuleClient.OpenAsync();
            Console.WriteLine("IoT Hub module client initialized.");

            // Read DataSentInterval from Module Twin Desired Properties
            var moduleTwin = await ioTHubModuleClient.GetTwinAsync();
            var moduleTwinCollection = moduleTwin.Properties.Desired;
            try {
                dataSentInterval = moduleTwinCollection["DataSentInterval"];
            } catch(ArgumentOutOfRangeException e) {
                Console.WriteLine("Property DataSentInterval not exist");
            }

            // Attach callback for Twin desired properties updates
            await ioTHubModuleClient.SetDesiredPropertyUpdateCallbackAsync(onDesiredPropertiesUpdate, null);

            // as this runs in a loop we don't await
            SendDataAsync(ioTHubModuleClient);
        }

        static async Task SendDataAsync(DeviceClient deviceClient) 
        {
            var rand = new Random((int)DateTime.UtcNow.Ticks);

            while (true) 
            {
                try
                {
                    counter++;

                    var rawMsg = new MessageBody()
                    {
                        humidity = rand.Next(40, 102),
                        temperature = rand.Next(1960, 3747) / 100,
                        energy = rand.Next(2000, 6338),
                        light = rand.Next(10, 2306),
                        timeCreated = $"{DateTime.UtcNow.ToShortDateString()} {DateTime.UtcNow.ToLongTimeString()}"
                    };
                    var messageString = JsonConvert.SerializeObject(rawMsg);
                    var message = new Message(Encoding.UTF8.GetBytes(messageString));
                    await deviceClient.SendEventAsync("hvacOutput", message);

                    Console.WriteLine($"\t{DateTime.UtcNow.ToShortDateString()} {DateTime.UtcNow.ToLongTimeString()}> Sending message: {counter}, Body: {messageString}");

                    // delay for next data gen.
                    await Task.Delay(TimeSpan.FromSeconds(dataSentInterval));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Unexpected Exception {ex.Message}" );
                    Console.WriteLine($"\t{ex.ToString()}");
                }
            }    
        }

        static Task onDesiredPropertiesUpdate(TwinCollection desiredProperties, object userContext)
        {
            try
            {
                Console.WriteLine("Desired property change:");
                Console.WriteLine(JsonConvert.SerializeObject(desiredProperties));

                if (desiredProperties["DataSentInterval"] != null)
                    dataSentInterval = desiredProperties["DataSentInterval"];

            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error when receiving desired property: {0}", exception);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error when receiving desired property: {0}", ex.Message);
            }
            return Task.CompletedTask;
        }
    }
}
