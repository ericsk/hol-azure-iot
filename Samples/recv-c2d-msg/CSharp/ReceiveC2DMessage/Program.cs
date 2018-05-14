using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ReceiveC2DMessage
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

        static async Task StartAsync()
        {
            var connStr = $"HostName={IOTHUB_HOSTNAME};SharedAccessKeyName={IOTHUB_POLICY};SharedAccessKey={IOTHUB_KEY}";
            var registry = RegistryManager.CreateFromConnectionString(connStr);

            Device device = await GetOrRegisterDeviceAsync(registry, DEVICE_ID);
            Console.WriteLine($"[INFO] Device {device.Id} is ready.");

            var deviceConn = $"HostName={IOTHUB_HOSTNAME};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConn);

            Console.WriteLine($"[INFO] Device {device.Id} is listening to the Azure IoT Hub...");
            while (true)
            {
                var receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.WriteLine("[INFO] Message received!");
                Console.WriteLine($"[DATA]\n{ Encoding.UTF8.GetString(receivedMessage.GetBytes()) }");
                if (receivedMessage.Properties.Count > 0)
                {
                    foreach (var key in receivedMessage.Properties.Keys)
                    {
                        Console.WriteLine($"{key} -> {receivedMessage.Properties[key]}");
                    }
                }

                await deviceClient.CompleteAsync(receivedMessage);
            }

        }

        static void Main(string[] args)
        {
            StartAsync().Wait();
        }
    }
}
