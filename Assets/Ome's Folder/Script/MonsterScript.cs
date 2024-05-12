
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class MonsterScript : MonoBehaviour
{
    BossStat bossStat = new BossStat();

    int rndHp;
    Color color;
    bool isDead = false;
    [SerializeField] Animator animator;
    [SerializeField] int moneyDrop;
    [SerializeField] Slider sliderHp;
    [SerializeField] SpriteRenderer monsterSprite;


    // [Header("Properties")]
    //[SerializeField] float fadeSpeed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        OnBirth();
    }


    void OnEnable()
    {
        OnBirth();
        int rndHp = Random.Range(30, 80);
        bossStat.Health = rndHp;
        sliderHp.maxValue = rndHp;
        sliderHp.value = rndHp;
        Debug.Log(bossStat.Health);
    }

    void Update()
    {
        monsterSprite.color = color;
    }

    public void TakeDamage()
    {
        bossStat.Health -= 5;
        sliderHp.value = bossStat.Health;
        OnDeath();
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

    void OnBirth()
    {
        color = Random.ColorHSV();
        isDead = false;
    }

    public void DisableGo()
    {

        gameObject.SetActive(false);
    }


}
