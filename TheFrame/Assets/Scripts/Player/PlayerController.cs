using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


//플레이어 오브젝트 관리, 입력 감지
public class PlayerController : MonoBehaviour
{
    [Header("PlayerMove")]
    public float speed;                     //이동 속도
    public float currentSpeed;               //현재 속도
    public float gravity;                   //중력 가속도
    public CharacterController caracterController;  //캐릭터 컨트롤러 컴포넌트
    //public JoyStick joystick;
    //public Buttons shootButton;
    //public Buttons aimButton;

    private Vector2 dir;                        //방향
    private Vector3 moveInput;                  //속도

    public Transform spine;                         //상체 트랜스폼

    //움직이는지 체크
    bool isMoving = false;
    //달리는 중인지 체크
    bool isRunning = false;
    //발사 버튼을 눌렀는지 체크
    bool isAttackInput = false;
    //조준 버튼을 눌렀는지 체크
    bool isAimInput = false;
    //재장전 버튼을 눌렀는지 체크
    bool isReloadInput = false;

    public bool IsMoving => isMoving;
    public bool IsRunning => isRunning;
    public bool IsAttackInput => isAttackInput;
    public bool IsAimInput => isAimInput;
    public bool IsReloadInput => isReloadInput;

    //플레이어 이동
    public Vector2 PlayerMove()
    {
        if (dir.y > 0)
        {
            if (isRunning)
                currentSpeed = speed * 1.5f;
        }
        else
        {
            currentSpeed = 0f;
        }

        moveInput = transform.forward * dir.y + transform.right * dir.x + transform.up * moveInput.y;

        if (caracterController.isGrounded == false)
            moveInput.y -= gravity * Time.deltaTime;
        else
            moveInput.y = 0;

        caracterController.Move(moveInput * (currentSpeed) * Time.deltaTime);

        return dir;
    }

    //플레이어가 바라보는 방향으로 회전
    public void PlayerLook(float lookDistance)
    {
        Vector3 dir = Camera.main.transform.position + (Camera.main.transform.forward * lookDistance);
        transform.LookAt(new Vector3(dir.x, transform.position.y, dir.z));

        Vector3 newDir = dir - spine.position;
        spine.rotation = Quaternion.LookRotation(newDir) * Quaternion.Euler(0f, -60f, -110f);

        //spine.up = new Vector3(dir.z, dir.y, dir.x);
        //spine.LookAt(dir);
    }

    public bool ReloadInput()
    {
        bool tryReload = false;

        if(isReloadInput)
        {
            tryReload = true;
            isReloadInput = false;
        }

        return tryReload;
    }

    //오른쪽 클릭 했을 때 (조준)
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAimInput = true;
        }
        else
        {
            isAimInput = false;
        }
    }

    //왼쪽 클릭 했을 때 (공격)
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAttackInput = true;
        }
        else
        {
            isAttackInput = false;
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isReloadInput = true;
        }
    }

    //WASD or 방향키 눌렀을 때 (이동)
    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            dir = context.ReadValue<Vector2>();
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    //왼쪽 쉬프트 눌렀을 때(달리기)
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRunning = true;
        }
        else if (context.canceled)
        {
            isRunning = false;
        }
    }
}
