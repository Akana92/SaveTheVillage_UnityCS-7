using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // UI элементы
    public Text wheatText;
    public Text peasantText;
    public Text warriorText;
    public Text waveText;
    public Text enemyText;
    public GameObject endGamePanel; // Панель для отображения конца игры (победа/поражение)
    public GameObject startGamePanel; // Панель для старта игры с правилами
    public Text startGameInfoText; // Текстовое поле на стартовой панели
    public Button startGameButton; // Кнопка для начала игры
    public Text endGameInfoText; // Текстовое поле на панели конца игры
    public Button restartButton; // Кнопка для перезапуска игры
    public Button pauseButton; // Кнопка для постановки игры на паузу

    public Button peasantButton; // Кнопка для найма крестьян
    public Button warriorButton; // Кнопка для найма воинов
    public Image peasantTimer; // Таймер для отслеживания процесса найма крестьянина
    public Image warriorTimer; // Таймер для отслеживания процесса найма воина
    public Image harvestTimer; // Таймер цикла сбора урожая
    public Image foodConsumptionTimer; // Таймер цикла потребления пищи
    public Image waveTimer; // Таймер до следующей волны врагов

    // Параметры игры
    public int startingWheat = 10; // Начальное количество пшеницы
    public int wheatPerPeasant = 3; // Сколько пшеницы добывает один крестьянин за цикл
    public int foodPerPerson = 1; // Сколько пшеницы потребляет один человек за цикл
    public int peasantCost = 5; // Стоимость найма одного крестьянина
    public int warriorCost = 8; // Стоимость найма одного воина
    public float harvestInterval = 5f; // Интервал сбора урожая
    public float foodConsumptionInterval = 5f; // Интервал потребления пищи
    public float peasantHireTime = 5f; // Время найма крестьянина
    public float warriorHireTime = 8f; // Время найма воина
    public float waveInterval = 15f; // Интервал между волнами врагов

    // Лимит волн
    public int maxWaves = 10; // Максимальное количество волн

    // Внутренние игровые параметры
    private int wheatCount; // Текущее количество пшеницы
    private int peasantCount = 0; // Текущее количество крестьян
    private int warriorCount = 0; // Текущее количество воинов
    private int enemyWaveCount = 1; // Номер текущей волны врагов (начинается с первой)
    private int currentEnemies = 0; // Количество врагов в текущей волне
    private int totalWarriorLosses = 0; // Счетчик потерь воинов
    private int totalPeasantLosses = 0; // Счетчик потерь крестьян
    private int totalEnemiesKilled = 0; // Счетчик убитых врагов

    private float nextWaveTime; // Время до следующей волны
    private bool isPeasantHiring = false; // Флаг процесса найма крестьянина
    private bool isWarriorHiring = false; // Флаг процесса найма воина
    private bool isGameOver = false; // Флаг окончания игры (победа/поражение)
    private bool isPaused = false; // Флаг паузы игры
    private bool gameStarted = false; // Флаг начала игры
    private bool hasPeasants = false; // Флаг наличия хотя бы одного крестьянина

    // Метод, вызываемый при старте игры
    void Start()
    {
        // Установка стартовой панели с текстом правил и кнопкой начала игры
        startGameInfoText.text = "Выживите 10 волн!\r\nПравила: \r\nНайм Войнов = 8 пшеницы (8 сек)\r\nНайм Крестьян = 5 пшеницы (5 сек) = генерируют 5 пшеницы за цикл сбора урожая.\r\nКаждая волна 36 сек. увеличение врагов в каждой последующей волне 33%\r\n1 враг стоит 1 воина, либо 3х крестьян";
        startGameButton.GetComponentInChildren<Text>().text = "Начать игру";
        startGamePanel.SetActive(true); // Активируем панель старта
        Time.timeScale = 0f; // Останавливаем время, пока игра не началась
        startGameButton.onClick.AddListener(StartGame); // Добавляем обработчик на кнопку старта

        // Привязываем кнопки найма, паузы и перезапуска к их методам
        peasantButton.onClick.AddListener(HirePeasant);
        warriorButton.onClick.AddListener(HireWarrior);
        pauseButton.onClick.AddListener(TogglePause);
        restartButton.onClick.AddListener(RestartGame);
    }

    // Метод, который запускает игру после нажатия на кнопку старта
    void StartGame()
    {
        gameStarted = true;
        startGamePanel.SetActive(false); // Скрываем стартовую панель
        Time.timeScale = 1f; // Возобновляем ход времени
        wheatCount = startingWheat; // Инициализируем количество пшеницы
        nextWaveTime = Time.time + waveInterval; // Устанавливаем время до следующей волны
        UpdateEnemyWaveInfo(); // Обновляем информацию о волне
        UpdateUI(); // Обновляем UI с начальной информацией
    }

    // Метод, вызываемый каждый кадр для обновления логики игры
    void Update()
    {
        // Если игра не на паузе, не окончена и началась
        if (!isPaused && !isGameOver && gameStarted)
        {
            CheckWaveTimer(); // Проверяем таймер до следующей волны
            UpdateUI(); // Обновляем UI
        }
    }

    // Метод для переключения паузы
    void TogglePause()
    {
        isPaused = !isPaused; // Инвертируем флаг паузы

        if (isPaused)
        {
            Time.timeScale = 0f;  // Останавливаем время
        }
        else
        {
            Time.timeScale = 1f;  // Возобновляем время
        }
    }

    // Корутина для запуска цикла сбора урожая
    IEnumerator HarvestCycle()
    {
        while (!isGameOver)
        {
            float timer = 0f;
            while (timer < harvestInterval)
            {
                timer += Time.deltaTime;
                harvestTimer.fillAmount = timer / harvestInterval; // Обновляем таймер на UI
                yield return null;
            }

            if (peasantCount > 0)
            {
                wheatCount += peasantCount * wheatPerPeasant; // Добавляем пшеницу за цикл
            }
        }
    }

    // Корутина для запуска цикла потребления пищи
    IEnumerator FoodConsumptionCycle()
    {
        while (!isGameOver)
        {
            float timer = 0f;
            while (timer < foodConsumptionInterval)
            {
                timer += Time.deltaTime;
                foodConsumptionTimer.fillAmount = timer / foodConsumptionInterval; // Обновляем таймер на UI
                yield return null;
            }

            int totalPeople = peasantCount + warriorCount; // Общее количество людей (крестьяне и воины)
            wheatCount -= totalPeople * foodPerPerson; // Уменьшаем пшеницу на количество потребленной еды

            if (wheatCount < 0)
            {
                wheatCount = 0; // Пшеница не может быть отрицательной

                // Если еды недостаточно, крестьяне или воины начинают умирать
                if (peasantCount > 0)
                {
                    peasantCount--; // Умирает крестьянин
                    totalPeasantLosses++; // Увеличиваем счетчик потерь крестьян
                }
                else if (warriorCount > 0)
                {
                    warriorCount--; // Умирает воин
                    totalWarriorLosses++; // Увеличиваем счетчик потерь воинов
                }
            }
        }
    }

    // Метод для найма крестьянина
    void HirePeasant()
    {
        if (!isPeasantHiring && wheatCount >= peasantCost)
        {
            StartCoroutine(HirePeasantCoroutine()); // Запускаем корутину найма
        }
    }

    // Корутина для процесса найма крестьянина
    IEnumerator HirePeasantCoroutine()
    {
        isPeasantHiring = true;
        peasantButton.interactable = false; // Отключаем кнопку на время найма
        float timer = peasantHireTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            peasantTimer.fillAmount = 1 - (timer / peasantHireTime); // Обновляем таймер найма
            yield return null;
        }

        peasantCount++; // Увеличиваем количество крестьян
        wheatCount -= peasantCost; // Уменьшаем количество пшеницы
        peasantButton.interactable = true;
        isPeasantHiring = false;
        peasantTimer.fillAmount = 0;

        // Запускаем циклы сбора и потребления пищи при наличии хотя бы одного крестьянина
        if (!hasPeasants)
        {
            hasPeasants = true;
            StartCoroutine(HarvestCycle()); // Запуск цикла сбора урожая
            StartCoroutine(FoodConsumptionCycle()); // Запуск цикла потребления пищи
        }
    }

    // Метод для найма воина
    void HireWarrior()
    {
        if (!isWarriorHiring && wheatCount >= warriorCost)
        {
            StartCoroutine(HireWarriorCoroutine()); // Запускаем корутину найма
        }
    }

    // Корутина для процесса найма воина
    IEnumerator HireWarriorCoroutine()
    {
        isWarriorHiring = true;
        warriorButton.interactable = false; // Отключаем кнопку на время найма
        float timer = warriorHireTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            warriorTimer.fillAmount = 1 - (timer / warriorHireTime); // Обновляем таймер найма
            yield return null;
        }

        warriorCount++; // Увеличиваем количество воинов
        wheatCount -= warriorCost; // Уменьшаем количество пшеницы
        warriorButton.interactable = true;
        isWarriorHiring = false;
        warriorTimer.fillAmount = 0;
    }

    // Метод проверки таймера до следующей волны
    void CheckWaveTimer()
    {
        if (Time.time >= nextWaveTime && !isGameOver)
        {
            StartEnemyWave(); // Запускаем новую волну врагов
        }
        waveTimer.fillAmount = 1 - ((nextWaveTime - Time.time) / waveInterval); // Обновляем таймер волны
    }

    // Метод для запуска новой волны врагов
    void StartEnemyWave()
    {
        // Воспроизводим музыку атаки
        FindObjectOfType<SoundManager>().PlayAttackMusic();

        nextWaveTime = Time.time + waveInterval; // Устанавливаем время до следующей волны

        if (enemyWaveCount > maxWaves)
        {
            Victory(); // Вызываем победу, если волны завершены
            return;
        }

        // Рассчитываем количество врагов для текущей волны
        currentEnemies = Mathf.RoundToInt(1 * Mathf.Pow(1.33f, enemyWaveCount - 1));
        if (currentEnemies < 1) currentEnemies = 1; // Минимум 1 враг

        // Воины сражаются первыми, каждый убивает 1 врага
        if (warriorCount > 0)
        {
            int killedWarriors = Mathf.Min(warriorCount, currentEnemies);
            warriorCount -= killedWarriors;
            totalWarriorLosses += killedWarriors;
            totalEnemiesKilled += killedWarriors;
            currentEnemies -= killedWarriors;
        }

        // Если остались враги, они убивают крестьян
        if (currentEnemies > 0 && peasantCount > 0)
        {
            int killedPeasants = Mathf.Min(peasantCount, currentEnemies * 3);
            peasantCount -= killedPeasants;
            totalPeasantLosses += killedPeasants;
            totalEnemiesKilled += Mathf.FloorToInt(killedPeasants / 3f);
        }

        // Проверка конца игры
        if (peasantCount == 0 && warriorCount == 0)
        {
            Defeat(); // Если нет людей, игрок проигрывает
        }
        else
        {
            enemyWaveCount++; // Переход к следующей волне
            UpdateEnemyWaveInfo(); // Обновляем информацию о врагах в следующей волне
            UpdateUI(); // Обновляем интерфейс
        }
    }

    // Метод для обновления информации о количестве врагов в следующей волне
    void UpdateEnemyWaveInfo()
    {
        int nextWaveEnemies = Mathf.RoundToInt(1 * Mathf.Pow(1.33f, enemyWaveCount));
        if (nextWaveEnemies < 1) nextWaveEnemies = 1;
        enemyText.text = "Врагов в следующей волне: " + nextWaveEnemies; // Обновляем текст
    }

    // Метод, вызываемый при поражении
    void Defeat()
    {
        isGameOver = true;
        Time.timeScale = 0f; // Останавливаем игру
        endGamePanel.SetActive(true); // Активируем панель конца игры
        endGameInfoText.text = $"Вы проиграли!\nВсего волн: {enemyWaveCount - 1}\n" +
                               $"Потеряно воинов: {totalWarriorLosses}\n" +
                               $"Потеряно крестьян: {totalPeasantLosses}\n" +
                               $"Убито врагов: {totalEnemiesKilled}"; // Выводим результаты игры
    }

    // Метод, вызываемый при победе
    void Victory()
    {
        isGameOver = true;
        Time.timeScale = 0f; // Останавливаем игру
        endGamePanel.SetActive(true); // Активируем панель конца игры
        endGameInfoText.text = $"Победа!\nВсего волн: {enemyWaveCount - 1}\n" +
                               $"Потеряно воинов: {totalWarriorLosses}\n" +
                               $"Потеряно крестьян: {totalPeasantLosses}\n" +
                               $"Убито врагов: {totalEnemiesKilled}"; // Выводим результаты игры
    }

    // Метод для обновления интерфейса
    void UpdateUI()
    {
        wheatText.text = "Пшеница: " + wheatCount;
        peasantText.text = "Крестьяне: " + peasantCount;
        warriorText.text = "Воины: " + warriorCount;
        waveText.text = "Волна: " + enemyWaveCount;
    }

    // Метод для перезапуска игры
    public void RestartGame()
    {
        Time.timeScale = 1f; // Возобновляем ход времени
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex); // Перезагружаем сцену
    }
}
