using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //将要播放的音乐组 0 1 为背景音乐 之后为音效
    public AudioClip[] audioGroup;
    // 播放音乐
    private AudioSource MusicPlayer;
    // 播放音效
    public AudioSource SoundPlayer;
     

    // 切换背景音乐播放
    public void PlayAudio(int index) {
        MusicPlayer = this.GetComponent<AudioSource> ();
        MusicPlayer.clip = audioGroup[index];
        MusicPlayer.Play ();
    }
    public void PlaySound(int index) {
        SoundPlayer = SoundPlayer.GetComponent<AudioSource> ();
        SoundPlayer.clip = audioGroup[index];
        SoundPlayer.PlayOneShot (audioGroup[index]);
    }
}
