using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pickupable : Interactable
{
    [Tooltip("Check if object can be used to mop the floor")]
    public bool bl_mop;

    [Tooltip("Check if object can be used to sweep cobwebs")]
    public bool bl_duster;

    [Tooltip("check if object can be used to clean the mirror")]
    public bool bl_soapBar;

    [Tooltip("Check if object can be used to light the fireplace")]
    public bool bl_lighter;

    [Tooltip("Check if object can be used to unlock the front door")]
    public bool bl_frontDoorKey;

    [Tooltip("Check if object can be used to turn off the TV")]
    public bool bl_remote;

    [Tooltip("Check if object is a doorknob")]
    public bool bl_doorKnob;

    [Tooltip("Check if the ghost wants to hide this")]
    public bool bl_hideable;

    [Tooltip("Check if the ghost wants to throw this")]
    public bool bl_toThrow;

    [Tooltip("Check if this object can damage the player")]
    public bool bl_canDamagePlayer;

    [Tooltip("check if the damage SFX from this object should be blunt or sharp")]
    public bool bl_sharp;

    [Tooltip("Rotation applied when held")]
    public Vector3 v3_heldRotationMod;

    [Tooltip("Position applied when held")]
    public Vector3 v3_heldPositionMod;

    protected Rigidbody rb;
    public Rigidbody RB { get { return rb; } }
    protected Collider[] a_col;
    public Collider[] a_Col { get { return a_col; } }
    [HideInInspector] public bool bl_held;
    protected MeshRenderer ren_meshRenderer;
    // holds the default material for objects that change materials
    // i.e. clean material for dirty dish
    protected Material mat_base;

    List<Collider> l_col_overlapping = new List<Collider>();

    Vector3 v3_startPos;
    Vector3 v3_startRot;

    public int int_ignoreLiveBoxFrames = 0;
    public int int_startingLayer;

    protected virtual void Start()
    {
        bl_pickupable = true;
        ren_meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        a_col = GetComponents<Collider>();
        bl_held = false;
        mat_base = ren_meshRenderer.material;
        v3_startPos = transform.position;
        v3_startRot = transform.eulerAngles;
        int_startingLayer = gameObject.layer;
    }

    //Trigger Enter/Exit scripts are used to make sure objects don't stuck in the environment when picked up
    private void OnTriggerEnter(Collider other)
    {
        if (a_col.Length > 0)
        {
            if (!a_Col[0].isTrigger || a_Col.Contains(other)) return;
        }
        if(l_col_overlapping.Count > 0)
        {
            if (l_col_overlapping.Contains(other)) return;
        }
        if(other.isTrigger) return;
        l_col_overlapping.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        l_col_overlapping.Remove(other);
        if(l_col_overlapping.Count <= 0)
        {
            foreach(Collider co in a_col)
            {
                co.isTrigger = false;
            }
        }
    }

    public void Drop()
    {
        foreach (Collider co in a_col)
        {
            co.isTrigger = false;
        }
    }

    // turns collider off when picked up, until item is in hand. This should prevent things from getting stuck in hand.
    public override void Interact()
    {
        if(transform.GetComponent<Candle>())
        {
            if (GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().bl_lighter) transform.GetComponent<Candle>().Light();
        }

        l_col_overlapping.Clear();
        foreach (Collider co in a_col)
        {
            co.isTrigger = true;
        }
    }

    private void LateUpdate ()
    {
        if (int_ignoreLiveBoxFrames > 0) int_ignoreLiveBoxFrames--;
    }

    public void Respawn()
    {
        if (int_ignoreLiveBoxFrames > 0) return;
        rb.Sleep();
        transform.position = v3_startPos;
        transform.eulerAngles = v3_startRot;
    }

}