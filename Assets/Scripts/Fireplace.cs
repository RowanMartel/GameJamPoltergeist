using UnityEngine;

public class Fireplace : Interactable
{
    [Tooltip("Put the fireplace particle effect / light object here")]
    public GameObject fireFX;
    [Tooltip("Put the fireplace SFX here")]
    public AudioClip fireplaceSFX;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = fireplaceSFX;
    }

    // light the fireplace if the player is holding the right item
    public override void Interact()
    {
        if (GameManager.playerController.heldObject == null || !GameManager.playerController.heldObject.GetComponent<Pickupable>().lighter) return;

        Light();
    }

    // complete the light fireplace task and start the vfx and sfx
    public void Light()
    {
        fireFX.SetActive(true);
        audioSource.Play();

        GameManager.taskManager.CompleteTask(TaskManager.Task.LightFireplace);
    }
    // stop the vfx and sfx, and re-grant the light fireplace task
    public void UnLight()
    {
        fireFX.SetActive(false);
        audioSource.Stop();

        if (GameManager.taskManager.taskList.Contains(TaskManager.Task.FindKey) ||
            GameManager.taskManager.taskList.Contains(TaskManager.Task.EscapeHouse))
            return;

        GameManager.taskManager.taskList.Add(TaskManager.Task.LightFireplace);
    }
}