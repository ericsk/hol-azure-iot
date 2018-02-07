# Lab 1: 蒐集由設備端產生的 Event Data

在這個 Lab 中希望讓操作的人瞭解：在一個 IoT 的場景中，蒐集資料有什麼要注意的重點，以及如何使用及運用 Azure IoT Hubs 的功能來接收 event data。

## 1. 環境準備

* 熟悉的程式開發環境
* (選擇性)(Windows) [Device Explorer](https://github.com/Azure/azure-iot-sdk-csharp/tree/master/tools/DeviceExplorer)
* (選擇性)(跨平台) [IoT Hub Explorer](https://github.com/Azure/iothub-explorer)

## 2. Hands-on Tasks

1. 建立一個 IoT Hub 資源。

2. 在 IoT Hub 上註冊傳送資料的裝置。

3. 使用 IoT Hub SDK 將產生的資料傳送至 IoT Hub 上。

4. 使用 Device Explorer 或 IoT Hub Explorer 觀看訊息傳送的狀況。

5. (Bonus) 使用 IoT Hub Routes (路由) 將傳入 IoT Hub 的訊息儲存到 Azure Storage 中。

## 參考資源

* [Azure IoT Hub 文件](https://docs.microsoft.com/zh-tw/azure/iot-hub/)
* [將裝置到雲端訊息傳送至 IoT 中樞](https://docs.microsoft.com/zh-tw/azure/iot-hub/iot-hub-devguide-messages-d2c)
* 路由訊息 ([.NET](https://docs.microsoft.com/zh-tw/azure/iot-hub/iot-hub-csharp-csharp-process-d2c), [Python](https://docs.microsoft.com/zh-tw/azure/iot-hub/iot-hub-python-python-process-d2c))