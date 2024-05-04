using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    private int _money;
    public int Money
    {
        get { return _money; }
        set { _money = value; }
    }

    int bet = 0;
    int win = 0;
    int lose = 0;
    int draw = 0;
    public Text moneyText;
    public Text winText;
    public Text loseText;
    public Text drawText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Money = 1000;
        UpdateMoneyText(); // Update the money text when the game starts
    }

    void UpdateMoneyText()
    {
        moneyText.text = $"Money: {Money}";
    }

    public void Bet(int amount)
    {
        bet = amount;
        Money -= amount;
        UpdateMoneyText(); // Update the money text after betting
    }

    public void Win()
    {
        win++;
        Money += bet * 2;
        UpdateMoneyText(); // Update the money text after winning
        winText.text = $"Wins: {win}";
    }

    public void Lose()
    {
        lose++;
        UpdateMoneyText(); // Update the money text after losing
        loseText.text = $"Loses: {lose}";
    }

    public void Draw()
    {
        draw++;
        Money += bet;
        UpdateMoneyText(); // Update the money text after a draw
        drawText.text = $"Draws: {draw}";
    }
}
