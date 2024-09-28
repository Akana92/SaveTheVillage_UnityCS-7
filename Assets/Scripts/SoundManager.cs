using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource musicSource; // Источник для фоновой и боевой музыки
    public AudioSource sfxSource; // Источник для звуков кликов кнопок

    public AudioClip backgroundMusic; // Фоновая музыка
    public AudioClip attackMusic; // Музыка для атаки волны
    public AudioClip buttonClickSound; // Звук кликов кнопок

    public Button[] buttons; // Массив всех кнопок для добавления звука кликов

    private bool isPlayingAttackMusic = false; // Флаг, указывающий, что играет музыка атаки

    void Start()
    {
        // Включаем фоновую музыку при старте игры
        PlayBackgroundMusic();

        // Привязываем звуки кликов ко всем кнопкам
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayButtonClickSound); // Добавляем звук клика для каждой кнопки
        }
    }

    // Метод для проигрывания фоновой музыки
    public void PlayBackgroundMusic()
    {
        // Проверяем, что музыка атаки не играет
        if (!isPlayingAttackMusic)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true; // Фоновая музыка зациклена
            musicSource.Play(); // Воспроизводим фоновую музыку
        }
    }

    // Метод для проигрывания музыки атаки
    public void PlayAttackMusic()
    {
        isPlayingAttackMusic = true; // Устанавливаем флаг, что играет музыка атаки
        musicSource.clip = attackMusic;
        musicSource.loop = false; // Музыка атаки не зацикливается
        musicSource.Play(); // Воспроизводим музыку атаки

        // Запускаем проверку на окончание музыки атаки
        StartCoroutine(CheckIfAttackMusicFinished());
    }

    // Метод для проверки, закончилась ли музыка атаки
    private IEnumerator CheckIfAttackMusicFinished()
    {
        // Ожидаем завершения трека музыки атаки
        while (musicSource.isPlaying)
        {
            yield return null; // Ждём, пока музыка играется
        }

        // После завершения музыки атаки включаем фоновую музыку
        isPlayingAttackMusic = false;
        PlayBackgroundMusic(); // Возвращаем фоновую музыку
    }

    // Метод для воспроизведения звуков кнопок
    public void PlayButtonClickSound()
    {
        sfxSource.PlayOneShot(buttonClickSound); // Проигрываем звук клика
    }
}
