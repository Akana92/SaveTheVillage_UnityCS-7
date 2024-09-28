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

    private bool isPlayingAttackMusic = false; // ����, �����������, ��� ������ ������ �����

    void Start()
    {
        // �������� ������� ������ ��� ������ ����
        PlayBackgroundMusic();

        // ����������� ����� ������ �� ���� �������
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayButtonClickSound); // ��������� ���� ����� ��� ������ ������
        }
    }

    // ����� ��� ������������ ������� ������
    public void PlayBackgroundMusic()
    {
        // ���������, ��� ������ ����� �� ������
        if (!isPlayingAttackMusic)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true; // ������� ������ ���������
            musicSource.Play(); // ������������� ������� ������
        }
    }

    // ����� ��� ������������ ������ �����
    public void PlayAttackMusic()
    {
        isPlayingAttackMusic = true; // ������������� ����, ��� ������ ������ �����
        musicSource.clip = attackMusic;
        musicSource.loop = false; // ������ ����� �� �������������
        musicSource.Play(); // ������������� ������ �����

        // ��������� �������� �� ��������� ������ �����
        StartCoroutine(CheckIfAttackMusicFinished());
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
}
