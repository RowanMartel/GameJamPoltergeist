using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : Interactable
{
    public bool bl_isOn = true;
    protected bool bl_ready = true;

    public GameObject go_lever;

    LightSwitch[] a_ls_switches;

    // Start is called before the first frame update
    void Start()
    {
        a_ls_switches = FindObjectsByType<LightSwitch>(FindObjectsSortMode.None);
        SetSwitchesOn();
    }

    public override void Interact()
    {
        if(bl_isOn && bl_ready)
        {
            bl_ready = false;
            LeanTween.rotateLocal(go_lever, new Vector3(90, 0, 0), 0.8f).setEase(LeanTweenType.easeInQuint).setOnComplete(SetSwitchesOff);
            bl_isOn = false;
            GameManager.taskManager.AddTask(TaskManager.Task.ResetBreakerBox);
        }
        else if(!bl_isOn && bl_ready)
        {
            bl_ready = false;
            LeanTween.rotateLocal(go_lever, new Vector3(0, 0, 0), 0.8f).setEase(LeanTweenType.easeInQuint).setOnComplete(SetSwitchesOn);
            bl_isOn = true;
            GameManager.taskManager.CompleteTask(TaskManager.Task.ResetBreakerBox);
        }
    }

    void SetSwitchesOn()
    {
        foreach (LightSwitch ls_switch in a_ls_switches)
        {
            ls_switch.SetFuseActive(true);
        }
        FuseBoxReady();
    }

    void SetSwitchesOff()
    {
        foreach (LightSwitch ls_switch in a_ls_switches)
        {
            ls_switch.SetFuseActive(false);
        }
        FuseBoxReady();
    }

    protected void FuseBoxReady()
    {
        bl_ready = true;
        if (bl_isOn) Debug.Log("Fuse Box ON"); // this is where we probably want to interact with the Task Manager, and turn on all active lights
    }
}
