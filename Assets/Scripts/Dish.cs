using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dish : Pickupable
{
    [HideInInspector] public bool inCupboard;
    [HideInInspector] public bool inTrash;

    [Tooltip("Check if the dish starts broken")]
    public bool broken;

    [Tooltip("Add broken mesh here")]
    public Mesh brokenMesh;

    [Tooltip("Check if object can be cleaned by placing it in the sink")]
    public bool dirtyDish;

    [Tooltip("Add material here if dirtyDish is true")]
    public Material dirtyMat;

    [Tooltip("Put the breaking SFX here")]
    public AudioClip breakingSFX;
    AudioSource audioSource;

    private void Start()
    {
        pickupable = true;
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        baseMat = meshRenderer.material;

        if (dirtyDish)
            meshRenderer.material = dirtyMat;

        if (broken) Break();
    }

    // calls break if the dish collides with anything too hard
    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 10)
            Break();
    }

    // changes the model and adjusts all tasks relating to this dish to either remove or add it as a requirement
    void Break()
    {
        audioSource.PlayOneShot(breakingSFX);
        GetComponent<MeshFilter>().mesh = brokenMesh;

        List<CleaningWater> waters = FindObjectsByType<CleaningWater>(FindObjectsSortMode.None).ToList();
        List<CupboardTrigger> cupboards = FindObjectsByType<CupboardTrigger>(FindObjectsSortMode.None).ToList();
        List<TrashCanTrigger> trashes = FindObjectsByType<TrashCanTrigger>(FindObjectsSortMode.None).ToList();

        foreach(CleaningWater w in waters)
        {
            w.dishes.Remove(this);
            w.CheckIfComplete();
        }
        foreach(CupboardTrigger c in cupboards)
        {
            c.dishes.Remove(this);
            c.CheckIfComplete();
        }
        foreach(TrashCanTrigger t in trashes)
        {
            t.dishes.Add(this);
            t.CheckIfComplete();
        }

        GameManager.ghost.RemovePoint(transform);
    }

    // marks dish as clean and changes back to the clean material
    public void Clean()
    {
        if (!dirtyDish) return;
        dirtyDish = false;
        meshRenderer.material = baseMat;
    }
}