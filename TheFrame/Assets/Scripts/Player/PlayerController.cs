using UnityEngine;
using UnityEngine.InputSystem;


//플레이어 오브젝트 관리, 입력 감지
public class PlayerController : MonoBehaviour
{
    //public JoyStick joystick;
    //public Buttons shootButton;
    //public Buttons aimButton;

    //플레이어 클래스
    public Player player;

    //이동 입력
    public void OnMove(InputAction.CallbackContext context)
    {
        player.SetMoveDir(context.ReadValue<Vector2>());
    }

    //달리기 입력
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            player.SetRunning(true);
        }
        else if (context.canceled)
        {
            player.SetRunning(false);
        }
    }

    //공격 입력
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            player.SetAttacking(true);
        }
        else
        {
            player.SetAttacking(false);
        }
    }

    //조준 입력
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            player.SetAiming(true);
        }
        else
        {
            player.SetAiming(false);
        }
    }

    //재장전 입력
    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            player.Reload();
        }
    }
}
