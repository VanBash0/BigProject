using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BigProject.Managers
{
    public class GameLogger : MonoBehaviour
    {
        public enum LogLevel
        {
            None, // in Editor development
            Debug, // to activate on build: BuildProfiles > PlatformSettings > WindowsSettings > DevelopmentBuild = on
            Release, // to activate on build: BuildProfiles > PlatformSettings > WindowsSettings > DevelopmentBuild = off
        }

        private enum LogType
        {
            D,
            /// <summary>
            /// 
            /// DEBUG
            /// 
            /// Детальная отладочная информация:
            /// - Максимально детальная информация
            /// - Только для разработчиков
            /// - Отключается в релизных сборках
            /// - Помогает понять поток выполнения
            /// 
            /// Примеры использования:
            /// - Инициализация систем
            /// -- [D] GameManager: Начало инициализации
            /// -- [D] AudioSystem: Загружено 42 звуковых файла
            /// -- [D] SaveSystem: Файл сохранения не найден, создаётся новый
            /// - Вход/выход из методов
            /// -- [D] PlayerController.Update(): Начало выполнения
            /// -- [D] Inventory.AddItem(): Добавление предмета ID: 4
            /// -- [D] QuestSystem.CompleteQuest(): Квест 'Спасение деревни' завершён
            /// - Параметры вызовов
            /// -- [D] AI.CalculatePath(): From: (10,0,5), To: (25,0,15), Результат: путь найден, длина: 18.7
            /// - Состояние объектов
            /// -- [D] Player State: Quests ended: 2/5, Items: 3/10, Position: (15.3, 0, 22.1)
            /// - Производительность
            /// -- [D] Frame 1245: Update: 2.3ms, Physics: 1.7ms, Rendering: 8.1ms
            /// -- [D] Memory: Total: 512MB, Used: 387MB, Garbage: 45MB
            /// 
            /// </summary>

            I,
            /// <summary>
            /// 
            /// INFO
            /// 
            /// Нормальная работа приложения:
            /// - Важные события нормальной работы
            /// - Полезно для всех (разработчики, тестировщики, поддержка)
            /// - Даёт общую картину работы приложения
            /// 
            /// Примеры использования:
            /// - Старт/остановка приложения
            /// -- [I] Приложение запущено. Версия: 1.2.3
            /// -- [I] Игровая сессия начата. ID: ABC123
            /// -- [I] Приложение завершает работу. Время сессии: 1 час 25 минут
            /// - Загрузка ресурсов
            /// -- [I] Загрузка сцены: 'MainMenu', Прогресс: 45%
            /// -- [I] Ассеты загружены: 124/150 (82.6%)
            /// -- [I] Конфигурация загружена: difficulty=Normal, language=ru_RU
            /// - Действия игрока
            /// -- [I] Игрок перешёл в локацию: лес
            /// -- [I] Игрок начал квест: 2
            /// -- [I] Игрок выполнил квест: 2
            /// -- [I] Игрок получил предмет: инструменты
            /// - Игровые события
            /// -- [I] Состояние геймплея: Dialogue
            /// - Системные события
            /// -- [I] Автосохранение выполнено: 15:30:25, Размер: 245KB
            /// -- [I] Язык изменён: с en_US на ru_RU
            /// 
            /// </summary>

            W,
            /// <summary>
            /// 
            /// WARNING
            /// 
            /// Предупреждения:
            /// - Потенциальные проблемы
            /// - Нестандартные ситуации, но работа продолжается
            /// - Требует мониторинга
            /// - Может указывать на дизайнерские проблемы
            /// 
            /// Примеры использования:
            /// - Ресурсные проблемы
            /// -- [W] Текстура не найдена: 'Textures/Items/instruments_icon', используется fallback
            /// -- [W] Звуковой файл слишком большой: 'Music/epic_music.mp3' (15MB)
            /// - Конфигурационные проблемы
            /// -- [W] Значение конфига вне диапазона: difficulty=10 (max=5), используется 5
            /// -- [W] Отсутствует обязательный ключ: 'player_speed', используется значение по умолчанию
            /// -- [W] Устаревший формат файла сохранения: версия 1, текущая 3, попытка конвертации
            /// - Игровые аномалии
            /// -- [W] Игрок пытается использовать недоступный предмет: 'Instuments'
            /// -- [W] Предмет не может быть добавлен: инвентарь полон (5/5)
            /// -- [W] Игрок не может найти путь к цели: целевая точка недоступна
            /// - Производительность
            /// -- [W] Низкий FPS: 25 (целевой 60), сцена: 'Forest'
            /// -- [W] Высокое использование памяти: 85%, рекомендуется оптимизация
            /// -- [W] Долгий кадр: 67ms, причина: Physics.Update
            /// 
            /// </summary>

            E,
            /// <summary>
            /// 
            /// Error
            /// 
            /// Ошибки, требующие внимания:
            /// - Фактические ошибки выполнения
            /// - Функциональность сломана или работает неправильно
            /// - Требует исправления
            /// - Влияет на пользовательский опыт
            /// 
            /// Примеры использования:
            /// - Критические ресурсы не найдены
            /// -- [E] Префаб не найден: 'Prefabs/Player/Player.prefab'
            /// -- [E] Сцена не существует: 'Level_Forest'
            /// -- [E] Конфигурационный файл повреждён: 'Config/game_settings.json'
            /// - Системные ошибки
            /// -- [E] Не удалось сохранить игру: диск переполнен
            /// -- [E] Не удалось создать DirectX контекст: код ошибки 0x887A0004
            /// - Логические ошибки
            /// -- [E] Деление на ноль
            /// -- [E] Индекс вне диапазона: inventory[6] при размере 5
            /// -- [E] Null reference: enemy.target is null при вызове Use()
            /// - Игровые ошибки
            /// -- [E]  Игрок в непроходимой геометрии: position=(NaN, NaN, NaN)
            /// -- [E]  Квест не может быть завершён: цель не существует
            /// -- [E]  Отрицательное количество предметов: -1
            /// - Внешние ошибки
            /// -- [E] Обновление не удалось: недостаточно места на диске (требуется 2GB)
            /// 
            /// </summary>

            C,
            /// <summary>
            /// 
            /// CRITICAL
            /// 
            /// Критические системные сбои:
            /// - Угрожают стабильности приложения
            /// - Могут привести к крашу
            /// - Требуют немедленного внимания
            /// - Часто связаны с окружением/железом
            /// 
            /// Примеры использования:
            /// - Фатальные системные ошибки
            /// -- [C] Недостаточно памяти: запрошено 4GB, доступно 512MB
            /// -- [C] GPU не поддерживает требуемые функции: нужен DirectX 11
            /// -- [C] Диск только для чтения: невозможно создать файлы
            /// - Коррупция данных
            /// -- [C] Повреждение файла сохранения: хэш не совпадает
            /// -- [C] Нарушение целостности игровых данных: проверка не пройдена
            /// - Критические игровые состояния
            /// -- [C] Игровой мир в противоречивом состоянии: существуют 2 игрока
            /// -- [C] Экономика сломана: бесконечные квестовые предметы (overflow)
            /// -- [C] Все NPC исчезли: счетчик = 0 при expected > 10
            ///
            /// </summary>
        }

        public static GameLogger Instance;

        private const string LOGS_FOLDERNAME = "LOGS";
        private const string LOGS_FILENAME_PREFIX = "game_";
        private const string LOGS_FILENAME_TYPE = ".log";

        private const string LOG_STRING_FORMAT = "[{0}] [{1}] {2}";
        private const string LOG_SYSTEM_STRING_FORMAT = "[Sys] {0} \n {1}";
        private const string TIMESTAMP_FORMAT = "yyyy-MM-dd_HH-mm-ss";

        private const string INFO_SESSION_STARTED = "=== Session started ===";
        private const string INFO_APPLICATION_QUITTING = "=== Application quitting ===";
        private const string INFO_DELETE_OLD_LOG_FILE = "Delete old log file: {0}";
        private const string ERROR_WRITE_FAILED = "Logger write failed: {0}";
        private const string ERROR_FILE_DELETE_FAILED = "Failed to delete {0}: {1}";
        private const string ERROR_CREATE_DIRECTORY = "Cannot create log dir: {0}." +
            "\n\n Will be created in Persistent Data Path: " +
            "\n\t %userprofile%\\AppData\\LocalLow\\{1}\\{2}\\";

        // настройка частоты записи логов
        private const int BUFFER_FLUSH_COUNT = 1;
        private const float FLUSH_INTERVAL = 1f;
        private const int MAX_COUNT_LOG_FILES = 10;

        private List<string> logBuffer = new();
        private string logFilePath;
        private float lastFlushTime;

        [SerializeField] private LogLevel _currentLogLevel = LogLevel.None;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InBuildActivation();
            InitLogger();
        }

        private void Update()
        {
            if (_currentLogLevel == LogLevel.None)
            {
                return;
            }

            if (Time.time - lastFlushTime > FLUSH_INTERVAL)
            {
                WriteBufferToFile();
            }
        }

        private void OnEnable()
        {
            Application.logMessageReceived += LogCallback;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= LogCallback;
        }

        private void OnApplicationQuit()
        {
            Info(INFO_APPLICATION_QUITTING);
            WriteBufferToFile();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                WriteBufferToFile();
            }
        }

        public void Debug(string message)
        {
            if (_currentLogLevel != LogLevel.Release)
            {
                AddToLogBuffer(LogType.D, message);
            }
        }

        public void Info(string message) => AddToLogBuffer(LogType.I, message);

        public void Warning(string message) => AddToLogBuffer(LogType.W, message);

        public void Error(string message)
        {
            AddToLogBuffer(LogType.E, message);
            WriteBufferToFile();
        }

        public void Critical(string message)
        {
            AddToLogBuffer(LogType.C, message);
            WriteBufferToFile();
        }

        public void InitLogger()
        {
            if (_currentLogLevel == LogLevel.None)
            {
                return;
            }

            string timestamp = DateTime.Now.ToString(TIMESTAMP_FORMAT);
            string logsDirectory = Path.Combine(Path.GetDirectoryName(Application.dataPath), LOGS_FOLDERNAME);

            try
            {
                Directory.CreateDirectory(logsDirectory);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(string.Format(ERROR_CREATE_DIRECTORY, e.Message, Application.companyName, Application.productName));

                logsDirectory = Path.Combine(Path.GetDirectoryName(Application.persistentDataPath), LOGS_FOLDERNAME);
                Directory.CreateDirectory(logsDirectory);
            }
            finally
            {
                string logsFilename = LOGS_FILENAME_PREFIX + timestamp + LOGS_FILENAME_TYPE;
                logFilePath = Path.Combine(logsDirectory, logsFilename);

                Info(INFO_SESSION_STARTED);
                DeleteOldLogs(logsDirectory);
                WriteBufferToFile();
            }
        }

        private void AddToLogBuffer(LogType type, string message)
        {
            if (_currentLogLevel == LogLevel.None)
            {
                return;
            }

            string timestamp = DateTime.Now.ToString(TIMESTAMP_FORMAT);
            string entry = string.Format(LOG_STRING_FORMAT, timestamp, type, message);

            logBuffer.Add(entry);

            if (logBuffer.Count >= BUFFER_FLUSH_COUNT)
            {
                WriteBufferToFile();
            }
        }

        private void WriteBufferToFile()
        {
            if (_currentLogLevel == LogLevel.None)
            {
                return;
            }

            if (logBuffer.Count == 0)
            {
                return;
            }

            try
            {
                StringBuilder sb = new();

                foreach (var entry in logBuffer)
                {
                    sb.AppendLine(entry);
                }

                File.AppendAllText(logFilePath, sb.ToString());
                logBuffer.Clear();
                lastFlushTime = Time.time;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(string.Format(ERROR_WRITE_FAILED, e.Message));
            }
        }

        private void InBuildActivation()
        {
            if (!Application.isEditor)
            {
                _currentLogLevel = UnityEngine.Debug.isDebugBuild ? LogLevel.Debug : LogLevel.Release;
            }
        }

        private void LogCallback(string message, string stackTrace, UnityEngine.LogType type)
        {
            string systemMessage = string.Format(LOG_SYSTEM_STRING_FORMAT, message, stackTrace);

            switch (type)
            {
                case UnityEngine.LogType.Log:
                    Info(systemMessage);
                    break;
                case UnityEngine.LogType.Warning:
                    Warning(systemMessage);
                    break;
                case UnityEngine.LogType.Error:
                    Error(systemMessage);
                    break;
                case UnityEngine.LogType.Assert:
                    Critical(systemMessage);
                    break;
                case UnityEngine.LogType.Exception:
                    Critical(systemMessage);
                    break;
            }
        }

        private void DeleteOldLogs(string folderPath)
        {
            List<string> logFiles = new();

            string[] allFiles = Directory.GetFiles(folderPath);

            foreach (string file in allFiles)
            {
                string fileName = Path.GetFileName(file);

                if (fileName.StartsWith(LOGS_FILENAME_PREFIX) && fileName.EndsWith(LOGS_FILENAME_TYPE))
                {
                    logFiles.Add(file);
                }
            }

            logFiles.Sort();

            if (logFiles.Count > MAX_COUNT_LOG_FILES)
            {
                for (int i = 0; i < logFiles.Count - MAX_COUNT_LOG_FILES; i++)
                {
                    try
                    {
                        File.Delete(logFiles[i]);
                        Info(string.Format(INFO_DELETE_OLD_LOG_FILE, logFiles[i]));
                    }
                    catch (Exception e)
                    {
                        Error(string.Format(ERROR_FILE_DELETE_FAILED, logFiles[i], e.Message));
                    }
                }

                logFiles.RemoveRange(0, logFiles.Count - 5);
            }
        }
    }
}