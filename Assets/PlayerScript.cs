using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
class PlayerScript : MonoBehaviour
{
    private enum State
    {
        Start,
        PlayerTurn,
        EnemyTurn,
        Stop,
        Win,
        Lost
    }
    private State memberState;
    public Text ShowPlayerHP;
    public Text ShowPlayerMP;
    public Text ShowPlayerAC;
    public Text ShowEnemyHP;
    public Text ShowEnemyMP;
    public Text ShowEnemyAC;
    public Text Dialogue;
    public int PlayerCurrentHP = 100;
    public int PlayerCurrentMP = 50;
    public int PlayerCurrentAC = 10;
    public int EnemyCurrentHP = 100;
    public int EnemyCurrentMP = 50;
    public int EnemyCurrentAC = 10;
    private int memberDamage = 0;
    private int DamageTaken = 0;
    [SerializeField] private GameObject Player = null;
    [SerializeField] private Sprite PlayerIdle = null;
    [SerializeField] private Sprite PlayerAttack = null;
    [SerializeField] private Sprite PlayerDefend = null;
    [SerializeField] private Sprite PlayerSpell = null;
    [SerializeField] private Sprite PlayerDead = null;
    [SerializeField] private GameObject Enemy = null;
    [SerializeField] private Sprite EnemyIdle = null;
    [SerializeField] private Sprite EnemyAttack = null;
    [SerializeField] private Sprite EnemyDefend = null;
    [SerializeField] private Sprite EnemySpell = null;
    [SerializeField] private Sprite EnemyDead = null;
    [SerializeField] private GameObject BG = null;
    private SpriteRenderer PlayerRenderer = null;
    private SpriteRenderer EnemyRenderer = null;
    private bool HasPotion = true;
    void Start()
    {
        PlayerRenderer = Player.GetComponent<SpriteRenderer>();
        EnemyRenderer = Enemy.GetComponent<SpriteRenderer>();
        memberState = State.Start;
        Dialogue.text = "Game Start";
        StartCoroutine(SetupBattle());
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    IEnumerator SetupBattle()
    {
        ShowPlayerHP.text = PlayerCurrentHP.ToString();
        ShowEnemyHP.text = EnemyCurrentHP.ToString();
        ShowPlayerMP.text = PlayerCurrentMP.ToString();
        ShowEnemyMP.text = EnemyCurrentMP.ToString();
        ShowPlayerAC.text = PlayerCurrentAC.ToString();
        ShowEnemyAC.text = EnemyCurrentAC.ToString();
        yield return new WaitForSeconds(2f);
        memberState = State.PlayerTurn;
        PlayerTurn();
    }
    void PlayerTurn()
    {
        PlayerCurrentAC = 10;
        ShowPlayerAC.text = PlayerCurrentAC.ToString();
        BG.transform.position = new Vector3(-5, 0, 5);
        Dialogue.text = "Player's turn.";
    }
    IEnumerator PlayerMove()
    {
        EnemyCurrentHP -= DamageTaken;
        ShowEnemyHP.text = EnemyCurrentHP.ToString();
        ShowPlayerMP.text = PlayerCurrentMP.ToString();
        ShowPlayerAC.text = PlayerCurrentAC.ToString();
        memberState = State.Stop;
        yield return new WaitForSeconds(2f);
        PlayerRenderer.sprite = PlayerIdle;
        if (EnemyCurrentHP <= 0)
        {
            memberState = State.Win;
            EndBattle();
        }
        else
        {
            memberState = State.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }
    void EndBattle()
    {
        if (memberState == State.Win)
        {
            ShowEnemyHP.text = "0";
            EnemyRenderer.sprite = EnemyDead;
            Dialogue.text = "You Win!";
        }
        if (memberState == State.Lost)
        {
            ShowPlayerHP.text = "0";
            PlayerRenderer.sprite = PlayerDead;
            Dialogue.text = "You Lose.";
        }
        Time.timeScale = 0;
    }
    //player move
    public void OnAttackBotton()
    {
        if (memberState != State.PlayerTurn)
        {
            return;
        }
        else
        {
            GetDamage();
            DamageTaken = memberDamage - EnemyCurrentAC;
            if (DamageTaken <= 0)
            {
                DamageTaken = 1;
            }
            Dialogue.text = "Player deal " + DamageTaken + " damage with attack.";
            StartCoroutine(PlayerMove());
            PlayerRenderer.sprite = PlayerAttack;
        }
    }
    public void OnDefendBotton()
    {
        if (memberState != State.PlayerTurn)
        {
            return;
        }
        else
        {
            PlayerCurrentAC = PlayerCurrentAC * 2;
            PlayerCurrentMP = PlayerCurrentMP + 5;
            if (PlayerCurrentMP > 50)
            {
                PlayerCurrentMP = 50;
            }
            DamageTaken = 0;
            Dialogue.text = "Player get double AC for 1 turn and recover 5MP with defend.";
            StartCoroutine(PlayerMove());
            PlayerRenderer.sprite = PlayerDefend;
        }
    }
    public void OnSpellBotton()
    {
        if (memberState != State.PlayerTurn)
        {
            return;
        }
        else
        {
            if (PlayerCurrentMP < 15)
            {
                Dialogue.text = "Not enough mana.";
                return;
            }
            else
            {
                PlayerCurrentMP = PlayerCurrentMP - 15;
                GetDamage();
                DamageTaken = 2 * memberDamage - EnemyCurrentAC;
                if (DamageTaken <= 0)
                {
                    DamageTaken = 1;
                }
                Dialogue.text = "Player cost 15MP and deal " + DamageTaken + " damage with spell.";
                StartCoroutine(PlayerMove());
                PlayerRenderer.sprite = PlayerSpell;
            }
        }
    }
    public void OnPotionBotton()
    {
        if (memberState != State.PlayerTurn)
        {
            return;
        }
        else
        {
            if (HasPotion == true)
            {
                PlayerCurrentHP += 50;
                if (PlayerCurrentHP >= 100)
                {
                    PlayerCurrentHP = 100;
                }
                PlayerCurrentMP += 25;
                if (PlayerCurrentMP > 50)
                {
                    PlayerCurrentMP = 50;
                }
                HasPotion = false;
            }
            else
            {
                int CheatMin = 1;
                int CheatMax = 2;
                int Cheat = Random.Range(CheatMin, CheatMax + 1);
                if (Cheat == 1)
                {
                    Dialogue.text = "No more potion!";
                }
                if (Cheat == 2)
                {
                    Dialogue.text = "Out of potion!";
                }
                return;
            }
            ShowPlayerHP.text = PlayerCurrentHP.ToString();
            ShowPlayerMP.text = PlayerCurrentMP.ToString();
            Dialogue.text = "Player recover 50HP and 25MP.";
        }
    }
    void GetDamage()
    {
        int Max = 20;
        int Min = 10;
        memberDamage = Random.Range(Min, Max + 1);
    }
    IEnumerator EnemyTurn()
    {
        Dialogue.text = "Enemy's turn.";
        BG.transform.position = new Vector3(5, 0, 5);
        EnemyCurrentAC = 10;
        ShowEnemyAC.text = EnemyCurrentAC.ToString();
        yield return new WaitForSeconds(1f);
        int MoveMin = 1;
        int MoveMax = 3;
        int Move = Random.Range(MoveMin, MoveMax + 1);
        if (Move == 3) //spell
        {
            if (EnemyCurrentMP < 15)
            {
                Move = Random.Range(MoveMin, MoveMax);
            }
            else
            {
                EnemyCurrentMP = EnemyCurrentMP - 15;
                GetDamage();
                DamageTaken = 2 * memberDamage - EnemyCurrentAC;
                if (DamageTaken <= 0)
                {
                    DamageTaken = 1;
                }
                StartCoroutine(EnemyMove());
                EnemyRenderer.sprite = EnemySpell;
                Dialogue.text = "Enemy cost 15MP and deal " + DamageTaken + " damage with spell.";
            }
        }
        if (Move == 1) //attack
        {
            GetDamage();
            DamageTaken = memberDamage - PlayerCurrentAC;
            if (DamageTaken <= 0)
            {
                DamageTaken = 1;
            }
            StartCoroutine(EnemyMove());
            EnemyRenderer.sprite = EnemyAttack;
            Dialogue.text = "Enemy deal " + DamageTaken + " damage with attack.";
        }
        if (Move == 2) //defend
        {
            EnemyCurrentAC = EnemyCurrentAC * 2;
            EnemyCurrentMP = EnemyCurrentMP + 5;
            if (EnemyCurrentMP > 50)
            {
                EnemyCurrentMP = 50;
            }
            DamageTaken = 0;
            StartCoroutine(EnemyMove());
            EnemyRenderer.sprite = EnemyDefend;
            Dialogue.text = "Enemy get double AC for 1 turn and recover 5MP with defend.";
        }
    }
    IEnumerator EnemyMove()
    {
        PlayerCurrentHP -= DamageTaken;
        ShowPlayerHP.text = PlayerCurrentHP.ToString();
        ShowEnemyMP.text = EnemyCurrentMP.ToString();
        ShowEnemyAC.text = EnemyCurrentAC.ToString();
        memberState = State.Stop;
        yield return new WaitForSeconds(2f);
        EnemyRenderer.sprite = EnemyIdle;
        if (PlayerCurrentHP <= 0)
        {
            memberState = State.Lost;
            EndBattle();
        }
        else
        {
            memberState = State.PlayerTurn;
            PlayerTurn();
        }
    }
}
