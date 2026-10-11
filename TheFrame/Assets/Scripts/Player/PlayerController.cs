using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


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

    [Header("Camera")]
    public float lookDistance;              //플레이어가 바라볼 카메라 거리
    public CinemachineCamera cinemachineCamera;

    [Header("Fire")]
    private bool isFire;                        //총 발사 논리값

    [Header("Animation")]
    public Animator soldierAni;                     //애니메이터
    public Transform spine;                         //상체 트랜스폼

    [Header("PlayerStats")]
    public Player stats;

    [Header("Weapon")]
    public WeaponController weaponController;       //무기 스크립트

    [Header("Aim")]
    private bool isAiming;
    private bool isRunning;
    private float minFov = 30f;
    private float maxFov = 60f;
    private float aimSpeed = 10f;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        PlayerMove();
        OnFire();
        RecoilPivot();
        AimCheck();
    }

    private void LateUpdate()
    {
        PlayerLook();
    }

    //플레이어 이동
    private void PlayerMove()
    {
        //dir = new Vector2(joystick.InputDirection.x, joystick.InputDirection.y);

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

        if (dir != Vector2.zero)
        {
            //여긴 플레이어 파이어 시스템에서 가져옴
            weaponController.SetMove(true);
        }
        else
        {
            weaponController.SetMove(false);
        }

        moveInput = transform.forward * dir.y + transform.right * dir.x + transform.up * moveInput.y;

        if (caracterController.isGrounded == false)
            moveInput.y -= gravity * Time.deltaTime;
        else
            moveInput.y = 0;

        caracterController.Move(moveInput * (speed + sprintSpeed) * Time.deltaTime);
    }

    //플레이어가 바라보는 방향으로 회전
    private void PlayerLook()
    {
        Vector3 dir = Camera.main.transform.position + (Camera.main.transform.forward * lookDistance);
        transform.LookAt(new Vector3(dir.x, transform.position.y, dir.z));

        Vector3 newDir = dir - spine.position;
        spine.rotation = Quaternion.LookRotation(newDir) * Quaternion.Euler(0f, -60f, -110f);

        //spine.up = new Vector3(dir.z, dir.y, dir.x);
        //spine.LookAt(dir);
    }


    //발사 기능
    private void OnFire()
    {
        //if(weaponController.Weapon != null)
        //{
        //    fireTime += Time.deltaTime;

        //    if (weaponController.ReloadCheck())
        //    {
        //        if (isFire)
        //        {
        //            if (fireTime >= FireInterval())
        //            {
        //                soldierAni.SetTrigger("Shoot");
        //                weaponController.Shoot(lookDistance);
        //                FireRecoil();

        //                fireTime = 0f;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        weaponController.Reloaded();
        //    }
        //}
    }

    //조준 기능
    private void AimCheck()
    {
        //isAiming = aimButton.isButtonDown;

        if(isAiming)
        {
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, minFov, Time.deltaTime * aimSpeed);
            //조준 상태인지 확인
            weaponController.SetAdd(isAiming);
        }
        else
        {
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, maxFov, Time.deltaTime * aimSpeed);
            weaponController.SetAdd(isAiming);
        }
    }

    //오른쪽 클릭 했을 때 (조준)
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }
    }

    //왼쪽 클릭 했을 때 (공격)
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            isFire = true;
        }
        else
        {
            isFire = false;
        }
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
