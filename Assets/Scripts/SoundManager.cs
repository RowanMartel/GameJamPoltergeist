using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    List<AudioClip> l_ac_curSFX = new();

    private IEnumerator coroutine;

    public int int_attemptsToPickNewSound;

    public bool bl_inGameplay;

    private void Start()
    {
        GameManager.menuManager.GameStart += EnterGame;
        GameManager.menuManager.DeathScreenEntered += LeaveGame;
        GameManager.menuManager.StartQuitingGame += LeaveGame;
        GameManager.menuManager.CreditsEntered += LeaveGame;
    }

    void EnterGame(object source, EventArgs e)
    {
        bl_inGameplay = true;
    }

    void LeaveGame(object source, EventArgs e)
    {
        bl_inGameplay = false;
    }

    // plays the given clip in the given source if it isn't already playing and if it's near the player
    public void PlayClip(AudioClip ac_clip, AudioSource as_source, bool bl_distanceMatters)
    {
        if (bl_inGameplay == false) return;
        if (as_source.isPlaying) return;

        if (l_ac_curSFX.Contains(ac_clip)) return;

        Vector3 v3_sourcePos = as_source.transform.position;
        if (bl_distanceMatters && Vector3.Distance(GameManager.playerController.transform.position, v3_sourcePos) >= Settings.int_SFXCullingDist)
            return;
        
        l_ac_curSFX.Add(ac_clip);

        as_source.clip = ac_clip;
        as_source.volume = Settings.flt_volume;
        as_source.Play();

        coroutine = RemoveClip(ac_clip.length, ac_clip);
        StartCoroutine(coroutine);
    }

    //Picks a random clip from a list of clips, then plays that clip with the other PlayClip method
    public void PlayClip(AudioClip[] a_ac_clips, AudioSource as_source, bool bl_distanceMatters)
    {
        if (as_source.isPlaying) return;

        AudioClip ac_clip;
        int attempts = 0;
        do
        {
            ac_clip = a_ac_clips[UnityEngine.Random.Range(0, a_ac_clips.Length)];
            attempts++;
        }while(l_ac_curSFX.Contains(ac_clip) && attempts >= int_attemptsToPickNewSound);

        PlayClip(ac_clip, as_source, bl_distanceMatters);
    }

    // removes the clip from the currently playing list after the duration of the clip has passed
    IEnumerator RemoveClip(float flt_waitTime, AudioClip cl)
    {
        yield return new WaitForSeconds(flt_waitTime);
        l_ac_curSFX.Remove(cl);
    }
}