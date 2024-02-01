using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    protected int int_playerHealth;
    public int Int_playerHealth { get { return int_playerHealth; } set { int_playerHealth = value; } }

    protected PlayerController playerController;
    protected MenuManager menuReference;

    public AudioClip ac_hurtSharp;
    AudioSource as_source;

    // Start is called before the first frame update
    void Start()
    {
        int_playerHealth = Settings.int_playerHealthMax;
        playerController = GetComponent<PlayerController>();
        menuReference = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        as_source = GetComponent<AudioSource>();
    }

    // This handles all collisions with the player, determines if the Damage Overlay is called, and if int_playerHealth is affected
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude < 5) return;

        Pickupable pu_pickupable = collision.gameObject.GetComponent<Pickupable>();
        if (pu_pickupable == null) return;

        if (pu_pickupable.bl_canDamagePlayer)
        {
            GameManager.soundManager.PlayClip(ac_hurtSharp, as_source);

            int_playerHealth--;
            if (int_playerHealth <= 0)
            {
                playerController.Die();
                menuReference.ShowDeathSequence();
            }
            menuReference.IncreaseDamageOverlay();
            Debug.Log("Increased Damage Overlay");
        }
        else
        {
            menuReference.IncreaseDamageOverlayTemporarily();
        }
    }
}
