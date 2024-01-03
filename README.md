# Quiz Telegram Bot

## Overview
This bot is designed to streamline the process of new user registration by collecting only essential information: Name and ChatId. 
It facilitates sending a list of questions from a Google Sheet on a scheduled basis and saves the responses back to another Google Sheet.

## Contributing
Contributions to enhance the functionality or efficiency of this code are welcome. 
Please submit a pull request for review.

## License
MIT

## Support
For support, please open an issue in the GitHub repository or contact the repository owner directly.

## Disclaimer
This code is provided as-is with no warranty. Users should ensure they have the proper permissions to interact with Google Sheets API and handle user data responsibly.

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
    Файл Worker.cs(35 строка). 
    ```csharp
    var botClient = new TelegramBotClient("YOUR_TELEGRAM_BOT_TOKEN");
    ```
    Файл GoogleSheetIntergration.cs(23 строка). 
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
