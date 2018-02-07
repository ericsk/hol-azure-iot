# Lab 2: 即時串流處理訊息

這份 Lab 希望讓學員瞭解，如何即時處理送至 IoT Hub 中的訊息，也是一般 data pipeline 中的 hot path。

## 1. 環境準備

* (選擇性) Visual Studio 中的 Stream Analytics 專案。

## 2. Hands-on Tasks

1. 將 IoT Hub 的資料讀入，儲存到外部儲存服務 (SQL 資料庫或 Azure 儲存體)。

2. 使用參考資料模式，與資料來源 (IoT Hub) 的資料互相參照。

3. 資料蒐集五分鐘再傳送到資料輸出。

4. (Bonus) 把資料即時傳入 Power BI

## 參考資源

* [Azure Stream Analytics (串流分析)](https://azure.microsoft.com/zh-tw/services/stream-analytics/)
* [輸出資料](https://docs.microsoft.com/zh-tw/azure/stream-analytics/stream-analytics-define-outputs)
* [使用參考資料](https://docs.microsoft.com/zh-tw/azure/stream-analytics/stream-analytics-use-reference-data)
* [Window functions](https://docs.microsoft.com/zh-tw/azure/stream-analytics/stream-analytics-window-functions)