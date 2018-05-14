using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SendD2CMessage
{
    class Program
    {
        const string DEVICE_ID = "";
        const string IOTHUB_HOSTNAME = "";
        const string IOTHUB_POLICY = "iothubowner";
        const string IOTHUB_KEY = "";

        static async Task<Device> GetOrRegisterDeviceAsync(RegistryManager registry, string deviceId)
        {
            Console.WriteLine($"[INFO] Getting device {deviceId} from registry...");
            Device device = null;
            device = await registry.GetDeviceAsync(deviceId);
            if (device == null)
            {
                Console.WriteLine($"[INFO] Creating device {deviceId} on registry...");
                device = await registry.AddDeviceAsync(new Device(deviceId));
            }

            return device;
        }

        static async Task PeriodicallySendMessageAsync(Device device)
        {
            var deviceConn = $"HostName={IOTHUB_HOSTNAME};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConn);
            Random rand = new Random();

            while (true)
            {
                var data = new {
                    deviceId = device.Id,
                    temperature = 20 + rand.NextDouble() * 15,
                    humidity = 60 + rand.NextDouble() * 20
                };
                var rawMessage = JsonConvert.SerializeObject(data);
                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(rawMessage));

                Console.Write($"[INFO] Sending message {rawMessage}");
                await deviceClient.SendEventAsync(message);

                await Task.Delay(1000);
            }
        }

        static async void StartAsync()
        {
            var connStr = $"HostName={IOTHUB_HOSTNAME};SharedAccessKeyName={IOTHUB_POLICY};SharedAccessKey={IOTHUB_KEY}";
            var registry = RegistryManager.CreateFromConnectionString(connStr);

            Device device = await GetOrRegisterDeviceAsync(registry, DEVICE_ID);
            Console.WriteLine($"[INFO] Device {device.Id} is ready.");
            await PeriodicallySendMessageAsync(device);

        }

        static void Main(string[] args)
        {
            StartAsync();
        }
    }
}
