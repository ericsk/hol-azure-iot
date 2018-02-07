# Lab 3: 在 edge 端處理訊息

在這份 Lab 中，希望學員瞭解如何在 edge 端處理資料，並且瞭解如何開發 edge 模組。

## 1. 準備環境

* 需建立 IoT Hub
* 模擬 edge 的環境 (開發機或 VM) 上要有 docker container 的環境

## 2. Hands-on Tasks

* 在開發機或 VM 上安裝好 Azure IoT Edge 環境，並且連接上 IoT Hub

* 部署 3 個 modules 到 edge 環境 [Module 1](https://hub.docker.com/r/openhack/iot-sim-1/), [Module 2](https://hub.docker.com/r/openhack/iot-sim-2/), [Module 3](https://hub.docker.com/r/openhack/iot-sim-3/)。觀察這三個 module 傳上 IoT Hub 的訊息。

* (bonus) 開發一個自己的模組並且 deploy 到 edge 環境上。

* (bonus) 攔截其它模組的訊息做處理

## 參考資料

* [Azure IoT Edge](https://docs.microsoft.com/zh-tw/azure/iot-edge/)
* [Docker 命令集](https://docs.docker.com/engine/reference/commandline/cli/)