import * as iothub from 'azure-iothub';
import * as iothubMqtt from 'azure-iot-device-mqtt';
import { Message } from 'azure-iot-device';

const DEVICE_ID = "ericsk-iot-sb2";
const IOTHUB_HOSTNAME = "ericskHub.azure-devices.net"; // e.g., *.azure-device.net
const IOTHUB_POLICY = "iothubowner"; // e.g., "iothubowner"
const IOTHUB_KEY = "VE2+Ez+hDLp5iM7FA5FnwAdHbExXa4etbMIZHgi5ROc=";

/**
 * Get device info from Azure IoT Hub. If device doesn't exist, register a new one.
 * 
 * @param {iothub.Registry} registry The Azure IoT Hub registry instance.
 * @param {string} deviceId The Device ID.
 * @return {Promise<iothub.Device>} The device instance.
 */
async function getOrRegisterDeviceAsync(registry: iothub.Registry, deviceId: string): Promise<iothub.Device> {
    console.info(`\u001b[1;37m[INFO] Getting device ${deviceId} from registry...\u001b[0m`);
    return new Promise<iothub.Device>((resolve, reject) => {
        registry.get(DEVICE_ID, (error, deviceInfo: iothub.Device, respBody) => {
            if (error) {    // doesn't exist.
                // register a new one.
                console.info(`\u001b[1;37m[INFO] Creating device ${deviceId} on registry...\u001b[0m`);
                registry.create({ 'deviceId': deviceId }, (err, dInfo, body) => {
                    if (err) {
                        reject(err);
                    } else {
                        resolve(dInfo);
                    }
                });
            } else {
                resolve(deviceInfo)
            }
        });
    });
}

/**
 * Periodically sending the messages to the Azure IoT Hub.
 * 
 * @param {iothub.Device} deviceInfo The Device instance.
 */
async function periodicallySendMessagesAsync(deviceInfo: iothub.Device): Promise<void> {
    return new Promise<void>((resolve, reject) => {
        let deviceConnStr = `HostName=${IOTHUB_HOSTNAME};DeviceId=${deviceInfo.deviceId};SharedAccessKey=${deviceInfo.authentication.symmetricKey.primaryKey}`;
        let deviceClient = iothubMqtt.clientFromConnectionString(deviceConnStr);

        setInterval(() => {
            let rawMessage = {
                deviceId: deviceInfo.deviceId,
                temperature: Math.floor(Math.random() * 50 + 195) / 10.0,
                humidity: Math.floor(Math.random() * 40) + 60
            };
            let message : Message = new Message(JSON.stringify(rawMessage));
            console.info(`\u001b[1;37m[INFO] Sending message ${message.getData()}...\u001b[0m`);
            deviceClient.sendEvent(message, (err, res) => {
                if (err) console.error(`\u001b[1;31m[ERROR] ${err.toString()}\u001b[0m`);
                if (res) console.log(`\u001b[1;30m[STATUS] ${res.constructor.name}\u001b[0m`);
            });
        }, 1000);
    });
}

/**
 * Main entry.
 */
async function main() {
    // Get Azure IoT Hub registry instance.
    let connStr = `HostName=${IOTHUB_HOSTNAME};SharedAccessKeyName=${IOTHUB_POLICY};SharedAccessKey=${IOTHUB_KEY}`;
    let registry: iothub.Registry = iothub.Registry.fromConnectionString(connStr);

    let deviceInfo: iothub.Device = await getOrRegisterDeviceAsync(registry, DEVICE_ID);
    console.log(`\u001b[1;30m[LOG] Device ${deviceInfo.deviceId} is ready...\u001b[0m`);
    await periodicallySendMessagesAsync(deviceInfo);
}

main();