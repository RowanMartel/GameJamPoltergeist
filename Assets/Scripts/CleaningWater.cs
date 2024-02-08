using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class CleaningWater : MonoBehaviour
{
    [Tooltip("Put the splash particle effect here (see splashEffect prefab)")]
    public ParticleSystem splashEffect;
    [Tooltip("Put all the dirty dish game objects here")]
    public List<Dish> li_dishes;

    [Tooltip("Put the clean SFX here")]
    public AudioClip ac_splash;
    AudioSource as_source;

    List<CleaningWater> cleaningWaters;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
        cleaningWaters = FindObjectsByType<CleaningWater>(FindObjectsSortMode.None).ToList();
    }

    // if other is a dirty dish, clean it, then check if complete in all instances of this script
    private void OnTriggerEnter(Collider other)
    {
        GameManager.soundManager.PlayClip(ac_splash, as_source);
        if(splashEffect != null) splashEffect.Play();

        Dish dish = other.GetComponent<Dish>();
        if (!dish || !dish.bl_dirtyDish) return;

        dish.Clean();

        foreach (CleaningWater water in cleaningWaters)
            if (water != this) water.CheckIfComplete();
        CheckIfComplete();
    }

    // checks if all the dishes in the dishes list are clean, and ends the task if so
    public void CheckIfComplete()
    {
        foreach (Dish dish in li_dishes)
            if (dish.bl_dirtyDish) return;

        GameManager.taskManager.CompleteTask(TaskManager.Task.CleanDishes);
    }
}