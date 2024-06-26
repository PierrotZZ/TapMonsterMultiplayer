using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;

public class MonsterScript : NetworkBehaviour
{
    BossStat bossStat = new BossStat();

    int rndHp;
    Color color;
    bool isDead = false;
    [SerializeField] Animator animator;
    [SerializeField] int moneyDrop;
    [SerializeField] Slider sliderHp;
    [SerializeField] SpriteRenderer monsterSprite;

    [Header("Hurt effect")]
    [SerializeField] Material flashMaterial;
    [SerializeField] float durationEffect;
    [SerializeField] GameObject hurtEffectObj;
    SpriteRenderer spriteRenderer;
    Material originMaterial;
    Coroutine flashRoutine;
    StatUpgradeManager statUpgradeManager;

    // [Header("Properties")]
    //[SerializeField] float fadeSpeed;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        originMaterial = spriteRenderer.material;
        statUpgradeManager = FindObjectOfType<StatUpgradeManager>();
    }

    private void Start()
    {
        rndHp = Random.Range(30, 80);
        color = Random.ColorHSV();

        if (!IsServer)
        {
            OnBirth(color, rndHp);
        }

        if (!IsOwner) return;
        OnBirthServerRpc(color, rndHp);

        // if (IsOwner)
        // {
        //     OnBirthServerRpc();
        // }


    }


    void OnEnable()
    {
        rndHp = Random.Range(30, 80);
        color = Random.ColorHSV();

        if (!IsOwner) return;
        OnBirthServerRpc(color, rndHp);


        // OnBirth();

        // if (IsOwner)
        // {
        //     OnBirthServerRpc();
        // }
    }

    void Update()
    {

        if (!IsOwner) return;
        UpdateColorServerRpc();

    }

    public void TakeDamage(int damage)
    {
        TakeDamageServerRpc(damage);
        FlashServerRpc();
        OnDeathServerRpc();
    }

    void OnDeath()
    {
        if (bossStat.Health <= 0 && !isDead)
        {
            isDead = true;
            animator.Play("DeathAnimation");
            GameManager.Instance.AddMoney(moneyDrop);
        }
    }

    void OnBirth(Color serverColor, int bossHealth)
    {
        isDead = false;
        color = serverColor;
        bossStat.Health = bossHealth;
        sliderHp.maxValue = bossHealth;
        sliderHp.value = bossHealth;
    }


    public void DisableGo()
    {
        gameObject.SetActive(false);
    }

    [ServerRpc]
    void OnDeathServerRpc()
    {
        int drop = Random.Range(5, 7);
        if (bossStat.Health <= 0 && !isDead)
        {
            isDead = true;
            animator.Play("DeathAnimation");
            OnDeathClientRpc();
        }
    }

    [ClientRpc]
    void OnDeathClientRpc()
    {
        isDead = true;
        //GameManager.Instance.AddMoney(moneyDrop);
    }

    [ServerRpc]
    void OnBirthServerRpc(Color serverColor, int bossHealth)
    {
        OnBirthClientRpc(serverColor, bossHealth);
        bossStat.Health = bossHealth;
        sliderHp.maxValue = bossHealth;
        sliderHp.value = bossHealth;
        isDead = false;
    }


    [ServerRpc]
    void TakeDamageServerRpc(int damage)
    {
        bossStat.Health -= damage;
        sliderHp.value = bossStat.Health;
        TakeDamageClientRpc(bossStat.Health);
    }

    [ClientRpc]
    void TakeDamageClientRpc(int health)
    {
        sliderHp.value = health;
    }

    [ClientRpc]
    void OnBirthClientRpc(Color serverColor, int bossHealth)
    {
        isDead = false;
        color = serverColor;
        bossStat.Health = bossHealth;
        sliderHp.maxValue = bossHealth;
        sliderHp.value = bossHealth;
    }

    [ServerRpc]
    void UpdateColorServerRpc()
    {
        UpdateColorClientRpc(color);
        monsterSprite.color = color;
    }

    [ClientRpc]
    void UpdateColorClientRpc(Color serverColor)
    {
        monsterSprite.color = serverColor;
    }

    void OnDisable()
    {
        int drop = Random.Range(5, 7);
        GameManager.Instance.AddMoney02(drop);
    }

    //Hurt
    [ServerRpc]
    public void FlashServerRpc()
    {
        FlashClientRpc();
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    [ClientRpc]
    public void FlashClientRpc()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.material = flashMaterial;
        hurtEffectObj.SetActive(true);
        yield return new WaitForSeconds(durationEffect);
        hurtEffectObj.SetActive(false);
        spriteRenderer.material = originMaterial;
        flashRoutine = null;
    }

}
