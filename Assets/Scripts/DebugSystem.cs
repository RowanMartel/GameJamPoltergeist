using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugSystem : MonoBehaviour
{

    public bool bl_inDebug;
    public bool bl_inFPS;
    public TextMeshProUGUI tmp_leftText;
    public TextMeshProUGUI tmp_rightText;
    public TextMeshProUGUI tmp_fpsText;

    GhostBehavior gb_ghost;
    PlayerController pc_player;
    TaskManager tm_taskManager;
    TVStaticTrigger[] a_tv_televisionTrigger;

    GameObject[] a_go_cams;

    private void Start()
    {
        a_go_cams = GameObject.FindGameObjectsWithTag("MinimapCam");
        SceneManager.sceneLoaded += SetCams;
    }

    void SetCams(Scene scene, LoadSceneMode mode)
    {
        a_go_cams = GameObject.FindGameObjectsWithTag("MinimapCam");

        foreach (GameObject go_cam in a_go_cams)
        {
            go_cam.gameObject.SetActive(false);
        }
    }


    // Update is called once per frame
    void LateUpdate()
    {
        //Toggle Debug Mode
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (bl_inDebug)
            {
                ExitDebug();
            }
            else
            {
                EnterDebug();
            }
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (bl_inFPS)
            {
                bl_inFPS = false;
                GameManager.menuManager.ExitFPS();
            }
            else
            {
                bl_inFPS = true;
                GameManager.menuManager.EnterFPS();
            }
        }

        if (bl_inFPS)
        {
            tmp_fpsText.text = (1 / Time.unscaledDeltaTime).ToString("0000");
        }

        if (bl_inDebug)
        {
            if(gb_ghost == null)
            {
                //Get Gameplay Data When First Entering Game Scene
                gb_ghost = GameManager.ghost;
                pc_player = GameManager.playerController;
                tm_taskManager = GameManager.taskManager;
                a_tv_televisionTrigger = FindObjectsByType<TVStaticTrigger>(FindObjectsSortMode.None);

                ActivateCams(true);
            }
            if(gb_ghost != null)
            {
                //Set Ghost Aggression Levels
                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                {
                    gb_ghost.SetAggressionLevel(1);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                {
                    gb_ghost.SetAggressionLevel(2);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                {
                    gb_ghost.SetAggressionLevel(3);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                {
                    gb_ghost.SetAggressionLevel(4);
                }

                //Freeze Ghost
                if (Input.GetKeyDown(KeyCode.G))
                {
                    gb_ghost.bl_frozen = !gb_ghost.bl_frozen;
                }

                //Make Debug Visible
                if (gb_ghost.GetComponent<MeshRenderer>().enabled == false)
                {
                    //Ghost
                    gb_ghost.GetComponent<MeshRenderer>().enabled = true;
                    gb_ghost.go_floatTrigger.GetComponent<MeshRenderer>().enabled = true;
                    gb_ghost.go_heldItemParent.GetComponent<MeshRenderer>().enabled = true;
                    //TV
                    foreach (TVStaticTrigger tv in a_tv_televisionTrigger)
                    {
                        tv.GetComponent<MeshRenderer>().enabled = true;
                    }
                }

                //Skip to End Game
                if (Input.GetKeyDown(KeyCode.R))
                {
                    gb_ghost.EnterEndGame();

                    while (tm_taskManager.li_taskList.Contains(TaskManager.Task.FindKey) == false && tm_taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse) == false)
                    {
                        tm_taskManager.AddTask(TaskManager.Task.EscapeHouse);
                    }
                }

                //Update Text
                tmp_leftText.text = "Debug Mode Enabled:\n-Press R to enter\n end game\n-Press G to freeze\n the ghost\n-Press 1-4 to set\n the ghost's aggro level\n-Player can jump: " + pc_player.bl_isGrounded + "\n-Player Current Region: " + pc_player.go_curRegion.name + "\n\nGhost Patrol Point:\n" + gb_ghost.tr_currentPatrolPoint.gameObject + "\n\nGhost Held Item: " + gb_ghost.go_curHeldItem;
                tmp_rightText.text = "Ghost Aggro Level: " + gb_ghost.int_curAggressionLevel + "\nGhost Current Task:\n" + GetTaskString(gb_ghost.l_tsk_currentTasks[gb_ghost.int_curIndex]) + "\nGhost Task List:\n" + TaskListToString(gb_ghost.l_tsk_currentTasks) + "Ghost Current Region: " + gb_ghost.go_curRegion + "Is Hiding?: " + gb_ghost.bl_hiding;
            }
            else
            {
                //Update Text
                tmp_leftText.text = "Debug Mode Enabled:\n-Press R to enter\n end game\n-Press G to freeze\n the ghost\n-Press 1-4 to set\n the ghost's aggro level\n-Player can jump: Null\n-Player Current Region: Null\n\nGhost Patrol Point:\nNull\n\nGhost Held Item: Null";
                tmp_rightText.text = "Ghost Aggro Level: Null\n\nGhost Current Task:\nNull\nGhost Task List:\nNull";
            }
        }

    }

    //Turn A Tasks Into A String
    string GetTaskString(TaskManager.Task tsk_task)
    {
        switch (tsk_task)
        {
            case TaskManager.Task.CleanDishes:
                return "Clean Dishes";
            case TaskManager.Task.PutAwayDishes:
                return "Put Away Dishes";
            case TaskManager.Task.MopFloor:
                return "Mop Floor";
            case TaskManager.Task.CleanMirror:
                return "Clean Mirror";
            case TaskManager.Task.CleanCobwebs:
                return "Clean Cobwebs";
            case TaskManager.Task.LightFireplace:
                return "Light Fireplace";
            case TaskManager.Task.FindKey:
                return "Find Key";
            case TaskManager.Task.EscapeHouse:
                return "Escape House";
            case TaskManager.Task.ThrowOutBrokenDishes:
                return "Throw Out Broken Dishes";
            case TaskManager.Task.GhostDirtyMirror:
                return "Dirty Mirror";
            case TaskManager.Task.GhostDirtyFloor:
                return "Dirty Floor";
            case TaskManager.Task.GhostDouseFireplace:
                return "Douse Fireplace";
            case TaskManager.Task.PutAwayBooks:
                return "Put Away Books";
            case TaskManager.Task.ResetBreakerBox:
                return "Reset Breaker Box";
            case TaskManager.Task.PutAwayToys:
                return "Put Away Toys";
            default:
                return "Error, Improper Task";
        }
    }

    //Turn A List Of Tasks Into A String
    string TaskListToString(List<TaskManager.Task> l_tsk_taskList)
    {
        string str_taskList = "";

        foreach (TaskManager.Task tsk_task in l_tsk_taskList)
        {
            str_taskList += GetTaskString(tsk_task) + "\n";
        }

        return str_taskList;
    }

    //Set minimap cameras to active or inactive
    void ActivateCams(bool bl_camSetting)
    {
        foreach (GameObject go_cam in a_go_cams)
        {
            go_cam.gameObject.SetActive(bl_camSetting);
        }
    }

    //Enter Debug Mode
    void EnterDebug()
    {
        ActivateCams(true);
        bl_inDebug = true;
        GameManager.menuManager.EnterDebug();
    }

    //Exit Debug Mode
    void ExitDebug()
    {
        ActivateCams(false);
        //Turn Off MeshRenderers If Appropriate
        if (gb_ghost != null)
        {
            gb_ghost.GetComponent<MeshRenderer>().enabled = false;
            gb_ghost.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            gb_ghost.go_heldItemParent.GetComponent<MeshRenderer>().enabled = false;

            foreach (TVStaticTrigger tv in a_tv_televisionTrigger)
            {
                tv.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        bl_inDebug = false;
        GameManager.menuManager.ExitDebug();
    }


}
