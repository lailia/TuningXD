using TMPro;
using UnityEngine;


public class Player : MonoBehaviour
{
    //방어구
    public ArmorInstance armor;

    //애니메이터
    public Animator playerAni;

    //기본 스탯
    //피해 감소
    public float reduce;
    //체력
    public float health;
    //최대 탄약
    public float maxAmmo;
    //기본 속도
    public float speed;
    //현재 속도
    public float currentSpeed;
    //중력 가속도
    public float gravity;                   

    //플레이어가 바라볼 카메라 거리
    public float lookDistance;

    public PlayerWeaponHandler playerWeaponHandler;
    //캐릭터 컨트롤러 컴포넌트
    public CharacterController caracterController;

    //상체 트랜스폼
    public Transform spine;

    //움직이는지 체크
    private bool isMoving = false;
    //달리는 중인지 체크
    private bool isRunning = false;

    //발사 버튼을 눌렀는지 체크
    private bool isAttacking = false;
    //재장전 버튼을 눌렀는지 체크
    //private bool isReloading = false;

    //조준 버튼을 눌렀는지 체크
    private bool isAiming = false;

    //방향키 방향
    private Vector2 dir;
    //움직일 속도
    private Vector3 moveInput;

    private void Update()
    {
        PlayerMove(dir, isRunning);

        if(isAttacking)
        {
            WeaponFire();
        }

        playerWeaponHandler.UpdateCrossHair(isMoving, isAiming);

        if(playerWeaponHandler.CheckReloadIsFinish())
        {
            playerAni.SetBool("Reloading", false);
        }
    }

    private void LateUpdate()
    {
        PlayerLook(lookDistance);
    }

    //플레이어 이동
    public void PlayerMove(Vector2 dir, bool isRunning)
    {
        if(isRunning)
        {
            if (dir.y > 0f)
            {
                currentSpeed = speed * 1.5f;
            }
            else
            {
                currentSpeed = speed;
            }
        }
        else
        {
            currentSpeed = speed;
        }

        moveInput = transform.forward * dir.y + transform.right * dir.x + transform.up * moveInput.y;

        if (caracterController.isGrounded == false)
            moveInput.y -= gravity * Time.deltaTime;
        else
            moveInput.y = 0;

        if (moveInput == Vector3.zero)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }

        MoveAnimation();

        caracterController.Move(moveInput * (currentSpeed) * Time.deltaTime);
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

    //장착 무기 변경
    public void SetWeapon(WeaponInstance weapon)
    {
        if (weapon!=null)
        {
            playerWeaponHandler.SetWeapon(weapon);
        }
        else
        {
            playerWeaponHandler.SetWeapon(weapon);
        }
    }

    //장착 방어구 변경
    public void SetArmor(ArmorInstance armor)
    {
        if(armor!=null)
        {
            this.armor = armor;
        }
        else
        {
            this.armor = null;
        }
    }

    //이동 입력이 들어왔는지
    public void SetMoveDir(Vector2 dir)
    {
        this.dir = dir;
    }

    public void SetRunning(bool isRunning)
    {
        this.isRunning = isRunning;
    }

    //공격 입력이 들어왔는지
    public void SetAttacking(bool isAttacking)
    {
        this.isAttacking = isAttacking;
    }

    //조준 입력이 들어왔는지
    public void SetAiming(bool isAiming)
    {
        this.isAiming = isAiming;
    }

    //재장전 입력이 들어왔는지
    public void Reload()
    {
        if (playerWeaponHandler.TryReload())
        {
            playerAni.SetBool("Reloading", true);
        }
    }

    void WeaponFire()
    {
        if (playerWeaponHandler.IsFire())
        {
            playerAni.SetTrigger("Shoot");
        }
    }

    void MoveAnimation()
    {
        playerAni.SetFloat("X", dir.x);
        playerAni.SetFloat("Y", dir.y);
        playerAni.SetFloat("Speed", currentSpeed);
        if (currentSpeed > speed)
        {
            playerAni.SetBool("Aiming", false);
        }
        else
        {
            playerAni.SetBool("Aiming", true);
        }
    }
}
