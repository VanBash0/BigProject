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
            /// <summary>
            /// DEBUG
            /// 
            /// <para>Детальная отладочная информация:</para>
            /// - Максимально детальная информация
            /// - Только для разработчиков
            /// - Отключается в релизных сборках
            /// - Помогает понять поток выполнения
            /// </summary>
            /// 
            /// <remarks>
            /// Примеры использования:
            /// <list type="bullet">
            /// <item> Инициализация систем</item>
            /// <para>- [D] GameManager: Начало инициализации</para>
            /// <para>- [D] AudioSystem: Загружено 42 звуковых файла</para>
            /// <para>- [D] SaveSystem: Файл сохранения не найден, создаётся новый</para>
            /// <item> Вход/выход из методов</item>
            /// <para>- [D] PlayerController.Update(): Начало выполнения</para>
            /// <para>- [D] Inventory.AddItem(): Добавление предмета ID: 4</para>
            /// <para>- [D] QuestSystem.CompleteQuest(): Квест 'Спасение деревни' завершён</para>
            /// <item> Параметры вызовов</item>
            /// <para>- [D] AI.CalculatePath(): From: (10,0,5), To: (25,0,15), Результат: путь найден, длина: 18.7</para>
            /// <item> Состояние объектов</item>
            /// <para>- [D] Player State: Quests ended: 2/5, Items: 3/10, Position: (15.3, 0, 22.1)</para>
            /// <item> Производительность</item>
            /// <para>- [D] Frame 1245: Update: 2.3ms, Physics: 1.7ms, Rendering: 8.1ms</para>
            /// <para>- [D] Memory: Total: 512MB, Used: 387MB, Garbage: 45MB</para>
            /// </list>
            /// </remarks>
            D,

            /// <summary>
            /// INFO
            /// 
            /// <para>Нормальная работа приложения:</para>
            /// - Важные события нормальной работы
            /// - Полезно для всех (разработчики, тестировщики, поддержка)
            /// - Даёт общую картину работы приложения
            /// </summary>
            /// 
            /// <remarks>
            /// Примеры использования:
            /// <list type="bullet">
            /// <item> Старт/остановка приложения</item>
            /// <para>- [I] Приложение запущено. Версия: 1.2.3</para>
            /// <para>- [I] Игровая сессия начата. ID: ABC123</para>
            /// <para>- [I] Приложение завершает работу. Время сессии: 1 час 25 минут</para>
            /// <item> Загрузка ресурсов</item>
            /// <para>- [I] Загрузка сцены: 'MainMenu', Прогресс: 45%</para>
            /// <para>- [I] Ассеты загружены: 124/150 (82.6%)</para>
            /// <para>- [I] Конфигурация загружена: difficulty=Normal, language=ru_RU</para>
            /// <item> Действия игрока</item>
            /// <para>- [I] Игрок перешёл в локацию: лес</para>
            /// <para>- [I] Игрок начал квест: 2</para>
            /// <para>- [I] Игрок выполнил квест: 2</para>
            /// <para>- [I] Игрок получил предмет: инструменты</para>
            /// <item> Игровые события</item>
            /// <para>- [I] Состояние геймплея: Dialogue</para>
            /// <item> Системные события</item>
            /// <para>- [I] Автосохранение выполнено: 15:30:25, Размер: 245KB</para>
            /// <para>- [I] Язык изменён: с en_US на ru_RU</para>
            /// </list>
            /// </remarks>
            I,

            /// <summary>
            /// WARNING
            /// 
            /// <para>Предупреждения:</para>
            /// - Потенциальные проблемы
            /// - Нестандартные ситуации, но работа продолжается
            /// - Требует мониторинга
            /// - Может указывать на дизайнерские проблемы
            /// </summary>
            /// 
            /// <remarks>
            /// Примеры использования:
            /// <list type="bullet">
            /// <item> Ресурсные проблемы</item>
            /// <para>- [W] Текстура не найдена: 'Textures/Items/instruments_icon', используется fallback</para>
            /// <para>- [W] Звуковой файл слишком большой: 'Music/epic_music.mp3' (15MB)</para>
            /// <item> Конфигурационные проблемы</item>
            /// <para>- [W] Значение конфига вне диапазона: difficulty=10 (max=5), используется 5</para>
            /// <para>- [W] Отсутствует обязательный ключ: 'player_speed', используется значение по умолчанию</para>
            /// <para>- [W] Устаревший формат файла сохранения: версия 1, текущая 3, попытка конвертации</para>
            /// <item> Игровые аномалии</item>
            /// <para>- [W] Игрок пытается использовать недоступный предмет: 'Instuments'</para>
            /// <para>- [W] Предмет не может быть добавлен: инвентарь полон (5/5)</para>
            /// <para>- [W] Игрок не может найти путь к цели: целевая точка недоступна</para>
            /// <item> Производительность</item>
            /// <para>- [W] Низкий FPS: 25 (целевой 60), сцена: 'Forest'</para>
            /// <para>- [W] Высокое использование памяти: 85%, рекомендуется оптимизация</para>
            /// <para>- [W] Долгий кадр: 67ms, причина: Physics.Update</para>
            /// </list>
            /// </remarks>
            W,

            /// <summary>
            /// Error
            /// 
            /// <para>Ошибки, требующие внимания:</para>
            /// - Фактические ошибки выполнения
            /// - Функциональность сломана или работает неправильно
            /// - Требует исправления
            /// - Влияет на пользовательский опыт
            /// </summary>
            /// 
            /// <remarks>
            /// Примеры использования:
            /// <list type="bullet">
            /// <item> Критические ресурсы не найдены</item>
            /// <para>- [E] Префаб не найден: 'Prefabs/Player/Player.prefab'</para>
            /// <para>- [E] Сцена не существует: 'Level_Forest'</para>
            /// <para>- [E] Конфигурационный файл повреждён: 'Config/game_settings.json'</para>
            /// <item> Системные ошибки</item>
            /// <para>- [E] Не удалось сохранить игру: диск переполнен</para>
            /// <para>- [E] Не удалось создать DirectX контекст: код ошибки 0x887A0004</para>
            /// <item> Логические ошибки</item>
            /// <para>- [E] Деление на ноль</para>
            /// <para>- [E] Индекс вне диапазона: inventory[6] при размере 5</para>
            /// <para>- [E] Null reference: enemy.target is null при вызове Use()</para>
            /// <item> Игровые ошибки</item>
            /// <para>- [E]  Игрок в непроходимой геометрии: position=(NaN, NaN, NaN)</para>
            /// <para>- [E]  Квест не может быть завершён: цель не существует</para>
            /// <para>- [E]  Отрицательное количество предметов: -1</para>
            /// <item> Внешние ошибки</item>
            /// <para>- [E] Обновление не удалось: недостаточно места на диске (требуется 2GB)</para>
            /// </list>
            /// </remarks>
            E,

            /// <summary>
            /// CRITICAL
            /// 
            /// <para>Критические системные сбои:</para>
            /// - Угрожают стабильности приложения
            /// - Могут привести к крашу
            /// - Требуют немедленного внимания
            /// - Часто связаны с окружением/железом
            /// </summary>
            /// 
            /// <remarks>
            /// Примеры использования:
            /// <list type="bullet">
            /// <item> Фатальные системные ошибки</item>
            /// <para>- [C] Недостаточно памяти: запрошено 4GB, доступно 512MB</para>
            /// <para>- [C] GPU не поддерживает требуемые функции: нужен DirectX 11</para>
            /// <para>- [C] Диск только для чтения: невозможно создать файлы</para>
            /// <item> Коррупция данных</item>
            /// <para>- [C] Повреждение файла сохранения: хэш не совпадает</para>
            /// <para>- [C] Нарушение целостности игровых данных: проверка не пройдена</para>
            /// <item> Критические игровые состояния</item>
            /// <para>- [C] Игровой мир в противоречивом состоянии: существуют 2 игрока</para>
            /// <para>- [C] Экономика сломана: бесконечные квестовые предметы (overflow)</para>
            /// <para>- [C] Все NPC исчезли: счетчик = 0 при expected > 10</para>
            /// </list>
            ///</remarks>
            C,
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
        private const string WARNING_UNHANDLED_SYSTEM_MESSAGE_TYPE = "Unhandled sysytem message type!\nMessage:\n{0}";
        private const string ERROR_WRITE_FAILED = "Logger write failed: {0}";
        private const string ERROR_FILE_DELETE_FAILED = "Failed to delete {0}: {1}";
        private const string ERROR_CREATE_DIRECTORY = "Cannot create log dir: {0}." +
            "\n\n Will be created in Persistent Data Path: " +
            "\n\t %userprofile%\\AppData\\LocalLow\\{1}\\{2}\\";

        // настройка частоты записи логов
        private const int BUFFER_LOGS_COUNT = 1;
        private const float WRITE_INTERVAL = 1f;
        // максимальное количество файлов логов
        private const int MAX_LOG_FILES_COUNT = 10;

        private readonly List<string> _logBuffer = new();
        private string _logFilePath;
        private float _lastWriteTime;

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

            if (Time.time - _lastWriteTime > WRITE_INTERVAL)
            {
                WriteBufferToFile();
            }
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
                AddLogToBuffer(LogType.D, message);
            }
        }

        public void Info(string message) => AddLogToBuffer(LogType.I, message);

        public void Warning(string message) => AddLogToBuffer(LogType.W, message);

        public void Error(string message)
        {
            AddLogToBuffer(LogType.E, message);
            WriteBufferToFile();
        }

        public void Critical(string message)
        {
            AddLogToBuffer(LogType.C, message);
            WriteBufferToFile();
        }

        private void AddLogToBuffer(LogType type, string message)
        {
            if (_currentLogLevel == LogLevel.None)
            {
                return;
            }

            string timestamp = DateTime.Now.ToString(TIMESTAMP_FORMAT);
            string logString = string.Format(LOG_STRING_FORMAT, timestamp, type, message);

            _logBuffer.Add(logString);

            if (_logBuffer.Count >= BUFFER_LOGS_COUNT)
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

            if (_logBuffer.Count == 0)
            {
                return;
            }

            try
            {
                StringBuilder sb = new();

                foreach (string logString in _logBuffer)
                {
                    sb.AppendLine(logString);
                }

                File.AppendAllText(_logFilePath, sb.ToString());
                _logBuffer.Clear();
                _lastWriteTime = Time.time;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(string.Format(ERROR_WRITE_FAILED, e.Message));
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
                default:
                    Warning(string.Format(WARNING_UNHANDLED_SYSTEM_MESSAGE_TYPE, systemMessage));
                    break;
            }
        }

        private void InBuildActivation()
        {
            if (!Application.isEditor)
            {
                _currentLogLevel = UnityEngine.Debug.isDebugBuild ? LogLevel.Debug : LogLevel.Release;
            }
        }

        private void InitLogger()
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
                _logFilePath = Path.Combine(logsDirectory, logsFilename);

                Info(INFO_SESSION_STARTED);
                DeleteOldLogs(logsDirectory);
                WriteBufferToFile();
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

            if (logFiles.Count > MAX_LOG_FILES_COUNT)
            {
                for (int i = 0; i < logFiles.Count - MAX_LOG_FILES_COUNT; i++)
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

                logFiles.RemoveRange(0, logFiles.Count - MAX_LOG_FILES_COUNT);
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
    }
}