using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAni : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] CinemachineVirtualCamera vCamera;

    public void OnPlayEndAAA()
    {
        Debug.Log("aa");
        vCamera.enabled = false;
        animator.enabled = true;
    }
}
