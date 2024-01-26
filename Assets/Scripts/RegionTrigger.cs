using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RegionTrigger : MonoBehaviour
{

    NavMeshObstacle nav_obstacle;
    GhostBehavior gb_ghost;
    PlayerController pc_player;

    private void Start()
    {
        nav_obstacle = GetComponent<NavMeshObstacle>();
        gb_ghost = GameManager.ghost;
        pc_player = GameManager.playerController;
    }

    //Assigns current region to characters who enter the region
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pc_player.go_curRegion = gameObject;

            if (gb_ghost.bl_hiding && gb_ghost.go_curRegion != pc_player.go_curRegion)
            {
                nav_obstacle.enabled = true;
            }

        }else if (other.CompareTag("Ghost"))
        {
            GameManager.ghost.go_curRegion = gameObject;
        }
    }

    //Turns off the nav mesh obstacle if needed
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && nav_obstacle.enabled)
        {
            nav_obstacle.enabled = false;
        }
    }

}
