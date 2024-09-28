using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // UI ��������
    public Text wheatText;
    public Text peasantText;
    public Text warriorText;
    public Text waveText;
    public Text enemyText;
    public GameObject endGamePanel; // ������ ��� ����������� ����� ���� (������/���������)
    public GameObject startGamePanel; // ������ ��� ������ ���� � ���������
    public Text startGameInfoText; // ��������� ���� �� ��������� ������
    public Button startGameButton; // ������ ��� ������ ����
    public Text endGameInfoText; // ��������� ���� �� ������ ����� ����
    public Button restartButton; // ������ ��� ����������� ����
    public Button pauseButton; // ������ ��� ���������� ���� �� �����

    public Button peasantButton; // ������ ��� ����� ��������
    public Button warriorButton; // ������ ��� ����� ������
    public Image peasantTimer; // ������ ��� ������������ �������� ����� �����������
    public Image warriorTimer; // ������ ��� ������������ �������� ����� �����
    public Image harvestTimer; // ������ ����� ����� ������
    public Image foodConsumptionTimer; // ������ ����� ����������� ����
    public Image waveTimer; // ������ �� ��������� ����� ������

    // ��������� ����
    public int startingWheat = 10; // ��������� ���������� �������
    public int wheatPerPeasant = 3; // ������� ������� �������� ���� ���������� �� ����
    public int foodPerPerson = 1; // ������� ������� ���������� ���� ������� �� ����
    public int peasantCost = 5; // ��������� ����� ������ �����������
    public int warriorCost = 8; // ��������� ����� ������ �����
    public float harvestInterval = 5f; // �������� ����� ������
    public float foodConsumptionInterval = 5f; // �������� ����������� ����
    public float peasantHireTime = 5f; // ����� ����� �����������
    public float warriorHireTime = 8f; // ����� ����� �����
    public float waveInterval = 15f; // �������� ����� ������� ������

    // ����� ����
    public int maxWaves = 10; // ������������ ���������� ����

    // ���������� ������� ���������
    private int wheatCount; // ������� ���������� �������
    private int peasantCount = 0; // ������� ���������� ��������
    private int warriorCount = 0; // ������� ���������� ������
    private int enemyWaveCount = 1; // ����� ������� ����� ������ (���������� � ������)
    private int currentEnemies = 0; // ���������� ������ � ������� �����
    private int totalWarriorLosses = 0; // ������� ������ ������
    private int totalPeasantLosses = 0; // ������� ������ ��������
    private int totalEnemiesKilled = 0; // ������� ������ ������

    private float nextWaveTime; // ����� �� ��������� �����
    private bool isPeasantHiring = false; // ���� �������� ����� �����������
    private bool isWarriorHiring = false; // ���� �������� ����� �����
    private bool isGameOver = false; // ���� ��������� ���� (������/���������)
    private bool isPaused = false; // ���� ����� ����
    private bool gameStarted = false; // ���� ������ ����
    private bool hasPeasants = false; // ���� ������� ���� �� ������ �����������

    // �����, ���������� ��� ������ ����
    void Start()
    {
        // ��������� ��������� ������ � ������� ������ � ������� ������ ����
        startGameInfoText.text = "�������� 10 ����!\r\n�������: \r\n���� ������ = 8 ������� (8 ���)\r\n���� �������� = 5 ������� (5 ���) = ���������� 5 ������� �� ���� ����� ������.\r\n������ ����� 36 ���. ���������� ������ � ������ ����������� ����� 33%\r\n1 ���� ����� 1 �����, ���� 3� ��������";
        startGameButton.GetComponentInChildren<Text>().text = "������ ����";
        startGamePanel.SetActive(true); // ���������� ������ ������
        Time.timeScale = 0f; // ������������� �����, ���� ���� �� ��������
        startGameButton.onClick.AddListener(StartGame); // ��������� ���������� �� ������ ������

        // ����������� ������ �����, ����� � ����������� � �� �������
        peasantButton.onClick.AddListener(HirePeasant);
        warriorButton.onClick.AddListener(HireWarrior);
        pauseButton.onClick.AddListener(TogglePause);
        restartButton.onClick.AddListener(RestartGame);
    }

    // �����, ������� ��������� ���� ����� ������� �� ������ ������
    void StartGame()
    {
        gameStarted = true;
        startGamePanel.SetActive(false); // �������� ��������� ������
        Time.timeScale = 1f; // ������������ ��� �������
        wheatCount = startingWheat; // �������������� ���������� �������
        nextWaveTime = Time.time + waveInterval; // ������������� ����� �� ��������� �����
        UpdateEnemyWaveInfo(); // ��������� ���������� � �����
        UpdateUI(); // ��������� UI � ��������� �����������
    }

    // �����, ���������� ������ ���� ��� ���������� ������ ����
    void Update()
    {
        // ���� ���� �� �� �����, �� �������� � ��������
        if (!isPaused && !isGameOver && gameStarted)
        {
            CheckWaveTimer(); // ��������� ������ �� ��������� �����
            UpdateUI(); // ��������� UI
        }
    }

    // ����� ��� ������������ �����
    void TogglePause()
    {
        isPaused = !isPaused; // ����������� ���� �����

        if (isPaused)
        {
            Time.timeScale = 0f;  // ������������� �����
        }
        else
        {
            Time.timeScale = 1f;  // ������������ �����
        }
    }

    // �������� ��� ������� ����� ����� ������
    IEnumerator HarvestCycle()
    {
        while (!isGameOver)
        {
            float timer = 0f;
            while (timer < harvestInterval)
            {
                timer += Time.deltaTime;
                harvestTimer.fillAmount = timer / harvestInterval; // ��������� ������ �� UI
                yield return null;
            }

            if (peasantCount > 0)
            {
                wheatCount += peasantCount * wheatPerPeasant; // ��������� ������� �� ����
            }
        }
    }

    // �������� ��� ������� ����� ����������� ����
    IEnumerator FoodConsumptionCycle()
    {
        while (!isGameOver)
        {
            float timer = 0f;
            while (timer < foodConsumptionInterval)
            {
                timer += Time.deltaTime;
                foodConsumptionTimer.fillAmount = timer / foodConsumptionInterval; // ��������� ������ �� UI
                yield return null;
            }

            int totalPeople = peasantCount + warriorCount; // ����� ���������� ����� (��������� � �����)
            wheatCount -= totalPeople * foodPerPerson; // ��������� ������� �� ���������� ������������ ���

            if (wheatCount < 0)
            {
                wheatCount = 0; // ������� �� ����� ���� �������������

                // ���� ��� ������������, ��������� ��� ����� �������� �������
                if (peasantCount > 0)
                {
                    peasantCount--; // ������� ����������
                    totalPeasantLosses++; // ����������� ������� ������ ��������
                }
                else if (warriorCount > 0)
                {
                    warriorCount--; // ������� ����
                    totalWarriorLosses++; // ����������� ������� ������ ������
                }
            }
        }
    }

    // ����� ��� ����� �����������
    void HirePeasant()
    {
        if (!isPeasantHiring && wheatCount >= peasantCost)
        {
            StartCoroutine(HirePeasantCoroutine()); // ��������� �������� �����
        }
    }

    // �������� ��� �������� ����� �����������
    IEnumerator HirePeasantCoroutine()
    {
        isPeasantHiring = true;
        peasantButton.interactable = false; // ��������� ������ �� ����� �����
        float timer = peasantHireTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            peasantTimer.fillAmount = 1 - (timer / peasantHireTime); // ��������� ������ �����
            yield return null;
        }

        peasantCount++; // ����������� ���������� ��������
        wheatCount -= peasantCost; // ��������� ���������� �������
        peasantButton.interactable = true;
        isPeasantHiring = false;
        peasantTimer.fillAmount = 0;

        // ��������� ����� ����� � ����������� ���� ��� ������� ���� �� ������ �����������
        if (!hasPeasants)
        {
            hasPeasants = true;
            StartCoroutine(HarvestCycle()); // ������ ����� ����� ������
            StartCoroutine(FoodConsumptionCycle()); // ������ ����� ����������� ����
        }
    }

    // ����� ��� ����� �����
    void HireWarrior()
    {
        if (!isWarriorHiring && wheatCount >= warriorCost)
        {
            StartCoroutine(HireWarriorCoroutine()); // ��������� �������� �����
        }
    }

    // �������� ��� �������� ����� �����
    IEnumerator HireWarriorCoroutine()
    {
        isWarriorHiring = true;
        warriorButton.interactable = false; // ��������� ������ �� ����� �����
        float timer = warriorHireTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            warriorTimer.fillAmount = 1 - (timer / warriorHireTime); // ��������� ������ �����
            yield return null;
        }

        warriorCount++; // ����������� ���������� ������
        wheatCount -= warriorCost; // ��������� ���������� �������
        warriorButton.interactable = true;
        isWarriorHiring = false;
        warriorTimer.fillAmount = 0;
    }

    // ����� �������� ������� �� ��������� �����
    void CheckWaveTimer()
    {
        if (Time.time >= nextWaveTime && !isGameOver)
        {
            StartEnemyWave(); // ��������� ����� ����� ������
        }
        waveTimer.fillAmount = 1 - ((nextWaveTime - Time.time) / waveInterval); // ��������� ������ �����
    }

    // ����� ��� ������� ����� ����� ������
    void StartEnemyWave()
    {
        // ������������� ������ �����
        FindObjectOfType<SoundManager>().PlayAttackMusic();

        nextWaveTime = Time.time + waveInterval; // ������������� ����� �� ��������� �����

        if (enemyWaveCount > maxWaves)
        {
            Victory(); // �������� ������, ���� ����� ���������
            return;
        }

        // ������������ ���������� ������ ��� ������� �����
        currentEnemies = Mathf.RoundToInt(1 * Mathf.Pow(1.33f, enemyWaveCount - 1));
        if (currentEnemies < 1) currentEnemies = 1; // ������� 1 ����

        // ����� ��������� �������, ������ ������� 1 �����
        if (warriorCount > 0)
        {
            int killedWarriors = Mathf.Min(warriorCount, currentEnemies);
            warriorCount -= killedWarriors;
            totalWarriorLosses += killedWarriors;
            totalEnemiesKilled += killedWarriors;
            currentEnemies -= killedWarriors;
        }

        // ���� �������� �����, ��� ������� ��������
        if (currentEnemies > 0 && peasantCount > 0)
        {
            int killedPeasants = Mathf.Min(peasantCount, currentEnemies * 3);
            peasantCount -= killedPeasants;
            totalPeasantLosses += killedPeasants;
            totalEnemiesKilled += Mathf.FloorToInt(killedPeasants / 3f);
        }

        // �������� ����� ����
        if (peasantCount == 0 && warriorCount == 0)
        {
            Defeat(); // ���� ��� �����, ����� �����������
        }
        else
        {
            enemyWaveCount++; // ������� � ��������� �����
            UpdateEnemyWaveInfo(); // ��������� ���������� � ������ � ��������� �����
            UpdateUI(); // ��������� ���������
        }
    }

    // ����� ��� ���������� ���������� � ���������� ������ � ��������� �����
    void UpdateEnemyWaveInfo()
    {
        int nextWaveEnemies = Mathf.RoundToInt(1 * Mathf.Pow(1.33f, enemyWaveCount));
        if (nextWaveEnemies < 1) nextWaveEnemies = 1;
        enemyText.text = "������ � ��������� �����: " + nextWaveEnemies; // ��������� �����
    }

    // �����, ���������� ��� ���������
    void Defeat()
    {
        isGameOver = true;
        Time.timeScale = 0f; // ������������� ����
        endGamePanel.SetActive(true); // ���������� ������ ����� ����
        endGameInfoText.text = $"�� ���������!\n����� ����: {enemyWaveCount - 1}\n" +
                               $"�������� ������: {totalWarriorLosses}\n" +
                               $"�������� ��������: {totalPeasantLosses}\n" +
                               $"����� ������: {totalEnemiesKilled}"; // ������� ���������� ����
    }

    // �����, ���������� ��� ������
    void Victory()
    {
        isGameOver = true;
        Time.timeScale = 0f; // ������������� ����
        endGamePanel.SetActive(true); // ���������� ������ ����� ����
        endGameInfoText.text = $"������!\n����� ����: {enemyWaveCount - 1}\n" +
                               $"�������� ������: {totalWarriorLosses}\n" +
                               $"�������� ��������: {totalPeasantLosses}\n" +
                               $"����� ������: {totalEnemiesKilled}"; // ������� ���������� ����
    }

    // ����� ��� ���������� ����������
    void UpdateUI()
    {
        wheatText.text = "�������: " + wheatCount;
        peasantText.text = "���������: " + peasantCount;
        warriorText.text = "�����: " + warriorCount;
        waveText.text = "�����: " + enemyWaveCount;
    }

    // ����� ��� ����������� ����
    public void RestartGame()
    {
        Time.timeScale = 1f; // ������������ ��� �������
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex); // ������������� �����
    }
}
