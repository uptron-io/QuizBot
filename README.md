# Quiz Telegram Bot

## Overview
This bot is designed to streamline the process of new user registration by collecting only essential information: Name and ChatId. 
It facilitates sending a list of questions from a Google Sheet on a scheduled basis and saves the responses back to another Google Sheet.

# Instructions for the "Quiz Bot" project
This project is a bot integrated with Google Sheets. It uses the Google Sheets API to read and write data to a Google Sheets table.

## How to create a bot

1. **Create a bot project in Telegram:**
    - Open Telegram and search for **BotFather**.
    - Use the command `/newbot`, follow the instructions to create a new bot and get a bot token.
  
2. **Register a Service Account:**
    - Go to [Google Cloud Console](https://console.cloud.google.com/).
    - Create a new project or select an existing one.
    - Go to the “IAM and Administration” tab -> “Service accounts”.
    - Create a new service account and download the JSON file with credentials (`client-secrets.json`). This file contains the credentials that your application will use to authenticate itself when working with the Google Sheets API.
  
3. **Install the project:**
    - Clone the project from the repository.
    - Create a `.env` file in the project root and add the necessary environment variables to it, for example:
     File Worker.cs
     ```csharp
     var botClient = new TelegramBotClient("YOUR_TELEGRAM_BOT_TOKEN");
     ```
     File GoogleSheetIntergration.cs.
     ```csharp
     static readonly string SpreadsheetId = "YOUR_SHREADSHEET_ID;
     ```
    - Replace the token in the code with your Telegram token: `TELEGRAM_BOT_TOKEN`.
  
4. **Replace `SpreadsheetId`:**
    - Replace `SpreadsheetId` in the `GoogleSheetIntegration` class with the ID of your Google Sheets spreadsheet. This will allow the bot to read and write data to this table.
  
## How to use the project
1. Make sure you have the .NET Core SDK installed.
2. Open the project in your IDE (such as Visual Studio or Visual Studio Code).
3. Run the project.
4. The bot will be ready to use in your Telegram account.

## Note
Please note that using the API and storing security tokens requires caution. Do not store them in the public domain or transfer them to third parties.

## Обзор
Бот предназначен для упрощения процесса отправки чек листов, собирая только необходимую информацию: имя и ChatId.
Отправляет список вопросов из Google Sheet по расписанию и сохраняет ответы обратно в Google Sheet.

# Инструкция по проекту "Quiz Bot"
Этот проект представляет собой бота, интегрированного с Google Sheets. Он использует Google Sheets API для чтения и записи данных в таблицу Google Sheets.

## Как создать бота

1. **Создайте проект бота в Telegram:**
   - Откройте Telegram и найдите **BotFather**.
   - Используйте команду `/newbot`, следуйте инструкциям для создания нового бота и получите токен бота.

2. **Зарегистрируйте Service Account:**
   - Перейдите в [Google Cloud Console](https://console.cloud.google.com/).
   - Создайте новый проект или выберите существующий.
   - Перейдите во вкладку "IAM и администрирование" -> "Service accounts".
   - Создайте новый сервисный аккаунт и скачайте JSON-файл с учетными данными (`client-secrets.json`). Этот файл содержит учетные данные, которые ваше приложение будет использовать для аутентификации при работе с Google Sheets API.

3. **Установите проект:**
   - Клонируйте проект с репозитория.
   - Создайте файл `.env` в корне проекта и добавьте в него необходимые переменные окружения, например:
    Файл Worker.cs 
    ```csharp
    var botClient = new TelegramBotClient("YOUR_TELEGRAM_BOT_TOKEN");
    ```
    Файл GoogleSheetIntergration.cs
    ```csharp
    static readonly string SpreadsheetId = "YOUR_SHREADSHEET_ID;
    ```
   - Замените токен в коде на ваш токен Telegram: `TELEGRAM_BOT_TOKEN`.

4. **Замените `SpreadsheetId`:**
   - Замените `SpreadsheetId` в классе `GoogleSheetIntegration` на ID вашей таблицы Google Sheets. Это позволит боту читать и записывать данные в эту таблицу.

## Как использовать проект
1. Убедитесь, что у вас установлен .NET Core SDK.
2. Откройте проект в вашей IDE (например, Visual Studio или Visual Studio Code).
3. Запустите проект.
4. Бот будет готов к использованию в вашем Telegram аккаунте.

## Примечание
Пожалуйста, обратите внимание, что использование API и хранение токенов безопасности требует осторожности. Не храните их в открытом доступе и не передавайте третьим лицам.

## Contributing
Contributions to enhance the functionality or efficiency of this code are welcome. 
Please submit a pull request for review.

## License
MIT

## Support
For support, please open an issue in the GitHub repository or contact the repository owner directly.

## Disclaimer
This code is provided as-is with no warranty. Users should ensure they have the proper permissions to interact with Google Sheets API and handle user data responsibly.
