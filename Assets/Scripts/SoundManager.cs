using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource musicSource; // �������� ��� ������� � ������ ������
    public AudioSource sfxSource; // �������� ��� ������ ������ ������

    public AudioClip backgroundMusic; // ������� ������
    public AudioClip attackMusic; // ������ ��� ����� �����
    public AudioClip buttonClickSound; // ���� ������ ������

    public Button[] buttons; // ������ ���� ������ ��� ���������� ����� ������
    public Button musicToggleButton; // ������ ��� ���������� �������
    public Text musicToggleButtonText; // ����� �� ������ ���������� �������

    private bool isPlayingAttackMusic = false; // ����, �����������, ��� ������ ������ �����
    private bool isMusicMuted = false; // ����, �����������, ��� ������ ���������

    void Start()
    {
        // �������� ������� ������ ��� ������ ����
        PlayBackgroundMusic();

        // ����������� ����� ������ �� ���� �������
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayButtonClickSound); // ��������� ���� ����� ��� ������ ������
        }

        // ����������� ������ ���������/���������� ������ � ������� ToggleMusic
        musicToggleButton.onClick.AddListener(ToggleMusic);
    }

    // ����� ��� ������������ ������� ������
    public void PlayBackgroundMusic()
    {
        // ���������, ��� ������ ����� �� ������ � ������ �� ���������
        if (!isPlayingAttackMusic && !isMusicMuted)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true; // ������� ������ ���������
            musicSource.Play(); // ������������� ������� ������
        }
    }

    // ����� ��� ������������ ������ �����
    public void PlayAttackMusic()
    {
        if (!isMusicMuted) // ���� ������ �� ���������
        {
            isPlayingAttackMusic = true; // ������������� ����, ��� ������ ������ �����
            musicSource.clip = attackMusic;
            musicSource.loop = false; // ������ ����� �� �������������
            musicSource.Play(); // ������������� ������ �����

            // ��������� �������� �� ��������� ������ �����
            StartCoroutine(CheckIfAttackMusicFinished());
        }
    }

    // ����� ��� ��������, ����������� �� ������ �����
    private IEnumerator CheckIfAttackMusicFinished()
    {
        // ������� ���������� ����� ������ �����
        while (musicSource.isPlaying)
        {
            yield return null; // ���, ���� ������ ��������
        }

        // ����� ���������� ������ ����� �������� ������� ������
        isPlayingAttackMusic = false;
        PlayBackgroundMusic(); // ���������� ������� ������
    }

    // ����� ��� ��������������� ������ ������
    public void PlayButtonClickSound()
    {
        sfxSource.PlayOneShot(buttonClickSound); // ����������� ���� �����
    }

    // ����� ��� ���������/���������� ������
    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted; // ����������� ���� ���������� ������

        if (isMusicMuted)
        {
            musicSource.Pause(); // ������������� ��������������� ������
            musicToggleButtonText.text = "�������� ������"; // ������ ����� �� ������
        }
        else
        {
            musicSource.UnPause(); // ������������ ��������������� ������
            musicToggleButtonText.text = "��������� ������"; // ������ ����� �� ������
        }
    }
}
