import * as iothub from 'azure-iothub';
import * as iothubMqtt from 'azure-iot-device-mqtt';
import * as iotDevice from 'azure-iot-device';

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
 * Main entry.
 */
async function main() {
    // Get Azure IoT Hub registry instance.
    let connStr = `HostName=${IOTHUB_HOSTNAME};SharedAccessKeyName=${IOTHUB_POLICY};SharedAccessKey=${IOTHUB_KEY}`;
    let registry: iothub.Registry = iothub.Registry.fromConnectionString(connStr);

    let deviceInfo: iothub.Device = await getOrRegisterDeviceAsync(registry, DEVICE_ID);
    console.log(`\u001b[1;30m[LOG] Device ${deviceInfo.deviceId} is ready...\u001b[0m`);

    let deviceConnStr = `HostName=${IOTHUB_HOSTNAME};DeviceId=${deviceInfo.deviceId};SharedAccessKey=${deviceInfo.authentication.symmetricKey.primaryKey}`;
    let deviceClient = iothubMqtt.clientFromConnectionString(deviceConnStr);

    // listen to the message callback
    deviceClient.on('message', (msg: iotDevice.Message) => {
        console.info(`\u001b[1;32m[INFO] Message received!\u001b[0m`);
        console.log(`[DATA]\n\u001b[1;30m${msg.getData().toString()}\u001b[0m`);
        console.log(`[Properties]\u001b[1;30m`);
        for (let i = 0; i < msg.properties.count(); ++i) {
            let prop = msg.properties.getItem(i);
            console.log(`${prop.key} -> ${prop.value}`);
        }
        console.log('\u001b[0m');
    });
    console.info(`\u001b[1;37m[INFO] Device ${deviceInfo.deviceId} is listening to the IoT Hub...\u001b[0m`);
}

main();