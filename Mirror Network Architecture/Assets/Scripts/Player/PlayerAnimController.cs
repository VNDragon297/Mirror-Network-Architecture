using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAnimController : PlayerComponent
{
    [SerializeField] private Animator animator;

    private PlayerController controller;

    public void Start()
    {
        base.OnStartAuthority();

        if (animator == null)
            animator.GetComponentInChildren<Animator>();
        controller = GetComponent<PlayerController>();

        controller.onBooleanChanged += SetAnimatorState;
        controller.onFloatChanged += SetAnimatorState;
    }

    private void SetAnimatorState(bool val, string name) => animator.SetBool(name, val);

    private void SetAnimatorState(float val, string name) => animator.SetFloat(name, val);

    private void OnDestroy()
    {
        controller.onBooleanChanged -= SetAnimatorState;
        controller.onFloatChanged -= SetAnimatorState;
    }
}
