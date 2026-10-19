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
    public float sprintSpeed;               //달릴 시 추가 속도
    public float gravity;                   //중력 가속도
    public CharacterController caracterController;  //캐릭터 컨트롤러 컴포넌트
    //public JoyStick joystick;
    //public Buttons shootButton;
    //public Buttons aimButton;

    private Vector2 dir;                        //방향
    private Vector3 moveInput;                  //속도

    [Header("Animation")]
    public Animator soldierAni;                     //애니메이터
    public Transform spine;                         //상체 트랜스폼

    [Header("Aim")]
    private bool isRunning;

    private void LateUpdate()
    {
        
    }

    //플레이어 이동
    public bool PlayerMove()
    {
        soldierAni.SetFloat("X", dir.x);
        soldierAni.SetFloat("Y", dir.y);

        if (dir.y > 0)
        {
            float s = dir.y * speed;
            soldierAni.SetFloat("Speed", s);
            soldierAni.SetBool("Aiming", !isRunning);
            if (isRunning)
                sprintSpeed = speed * .5f;
        }
        else
        {
            sprintSpeed = 0f;
            soldierAni.SetFloat("Speed", 0f);
            soldierAni.SetBool("Aiming", true);
        }

        moveInput = transform.forward * dir.y + transform.right * dir.x + transform.up * moveInput.y;

        if (caracterController.isGrounded == false)
            moveInput.y -= gravity * Time.deltaTime;
        else
            moveInput.y = 0;

        caracterController.Move(moveInput * (speed + sprintSpeed) * Time.deltaTime);

        return (dir != Vector2.zero);
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

    //WASD or 방향키 눌렀을 때 (이동)
    public void OnMove(InputAction.CallbackContext context)
    {
        dir = context.ReadValue<Vector2>();
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
