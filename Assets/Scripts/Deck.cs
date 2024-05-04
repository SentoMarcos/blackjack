using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;
    public Text playerPoints;
    public Text dealerPoints;
    [SerializeField] Dropdown betDropdown;

    public int[] values = new int[52];
    int cardIndex = 0;    
       
    private void Awake()
    {    
        InitCardValues();        

    }

    private void Start()
    {
        ShuffleCards();
        StartGame(); 
        updatePoints();
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        for (int i = 0; i < values.Length; i++)
        {
             values[i] = (i % 13) + 1;
             if (values[i] > 10)
             values[i] = 10;
        }
    }

    private void ShuffleCards()
    {
        // Create a new array to store the shuffled card values
        int[] shuffledValues = new int[values.Length];
        Sprite[] shuffledFaces = new Sprite[values.Length];

        // Create a list to keep track of the card indices
    

        // Shuffle the card indices using Fisher-Yates algorithm
        System.Random random = new System.Random();
        for (int i = 0; i < values.Length; i++)
        {
            int randomIndex = random.Next(values.Length);
            shuffledValues[i] = values[randomIndex];
            shuffledFaces[i] = faces[randomIndex];

            
        }
        // Update the values array with the shuffled values
        values = shuffledValues;
        faces = shuffledFaces;
    }

    void StartGame()
    {
        // Reparte dos cartas al jugador y al dealer
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            

            // Comprueba si el jugador o el dealer tienen Blackjack después de cada carta repartida
            if (i == 1) // Después de la segunda carta
            {
                // Comprueba si el jugador tiene Blackjack
                if (player.GetComponent<CardHand>().points == 21)
                {
                    finalMessage.text = "¡Blackjack! ¡Ganaste!";
                    finalMessage.color = Color.green;
                    hitButton.interactable = false;
                    stickButton.interactable = false;
                    MoneyManager.Instance.Win();
                    return; // Salimos del método, el juego no se reinicia
                }

                // Comprueba si el dealer tiene Blackjack
                if (dealer.GetComponent<CardHand>().points == 21)
                {
                    finalMessage.text = "¡El dealer tiene Blackjack! Has perdido.";
                    finalMessage.color = Color.red;
                    hitButton.interactable = false;
                    stickButton.interactable = false;
                    MoneyManager.Instance.Lose();
                    return; // Salimos del método, el juego no se reinicia
                }
            }
        }

        CalculateProbabilities();
        updatePoints();
    }


    private void CalculateProbabilities()
    {
        /*TODO:
 * Calcular las probabilidades de:
 * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
 * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
 * - Probabilidad de que el jugador obtenga más de 21 si pide una carta        
 */

        // Calcular la probabilidad de que el jugador se pase de 21
        int playerPoints = player.GetComponent<CardHand>().points;
        int cardsRemaining = 52 - cardIndex;
        int cardsOver21 = 0;
        for (int i = cardIndex; i < values.Length; i++)
        {
            if (playerPoints + values[i] > 21)
            {
                cardsOver21++;
            }
        }
        float probabilityOver21 = (float)cardsOver21 / cardsRemaining;

        // Calcular la probabilidad de que el dealer se pase de 21
        int dealerPoints = dealer.GetComponent<CardHand>().points;
        cardsOver21 = 0;
        for (int i = cardIndex; i < values.Length; i++)
        {
            if (dealerPoints + values[i] > 21)
            {
                cardsOver21++;
            }
        }
        float probabilityDealerOver21 = (float)cardsOver21 / cardsRemaining;

        //Probabilitat de que la siguiente carta sea enrtre 17 i 21
        float probability17to21 = 1 - probabilityOver21;

        // Calcular la probabilidad de que el dealer tenga más puntuación que el jugador
        int pointsCardUp = dealer.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value;
        int cardsDealerWins = 0;
        for (int i = cardIndex; i < values.Length; i++)
        {
            if (dealerPoints + values[i] > playerPoints && dealerPoints + values[i] <= 21)
            {
                cardsDealerWins++;
            }
        }
        float probabilityDealerWins = (float)cardsDealerWins / cardsRemaining;

        // Mostrar los resultados en pantalla
        probMessage.text = "Dealer > Player: " + probabilityDealerWins.ToString("P2") + "\n" +
              "17<= X <= 21: " + probabilityOver21.ToString("P2") + "\n" +
              "X <= 21: " + probability17to21.ToString("P2");
    }


    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        
    }       

    public void Hit()
    {
        //Repartimos carta al jugador
        PushPlayer();
        CalculateProbabilities();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (player.GetComponent<CardHand>().points == 21)
        {
            dealer.GetComponent<CardHand>().ToggleFirst(true);
            finalMessage.text = "¡Has ganado!";
            finalMessage.color = Color.green;
            hitButton.interactable = false;
            stickButton.interactable = false;
            MoneyManager.Instance.Win();
        }
        else if(player.GetComponent<CardHand>().points > 21)
         {
            dealer.GetComponent<CardHand>().ToggleFirst(true);
            finalMessage.text = "¡Has perdido!";
            finalMessage.color = Color.red;
            hitButton.interactable = false;
            stickButton.interactable = false;
             MoneyManager.Instance.Lose();
        }      
         // Deshabilitar los botones de hit y stand
         updatePoints();
        

    }

    public void Stand()
    {



        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
        // Obtener las manos actuales del jugador y del dealer
        dealer.GetComponent<CardHand>().ToggleFirst(true);
        CardHand playerHand = player.GetComponent<CardHand>();
        CardHand dealerHand = dealer.GetComponent<CardHand>();

        // El dealer toma cartas hasta que su puntuación sea 17 o más
        while (dealerHand.points < 17)
        {
            // Repartir una carta al dealer
            PushDealer();
        }

        // Determinar el resultado del juego
        int playerScore = playerHand.points;
        int dealerScore = dealerHand.points;

        if (dealerScore > 21 || (dealerScore < playerScore && playerScore <= 21))
        {
            finalMessage.text = "¡Has ganado!";
            MoneyManager.Instance.Win();
            finalMessage.color = Color.green;
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
        else if (dealerScore == playerScore)
        {
            finalMessage.text = "¡Empate!";
            MoneyManager.Instance.Draw();
            hitButton.interactable = false;
            stickButton.interactable = false;
        
        }
        else
        {
            finalMessage.text = "¡El dealer ha ganado!";
            MoneyManager.Instance.Lose();
            finalMessage.color = Color.red;
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
        CalculateProbabilities();
        updatePoints();
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
        CalculateProbabilities();
        updatePoints();
        betDropdown.value = 0;

        betDropdown.interactable = true;
       
    }

    public void getBet()
    {
        MoneyManager.Instance.Bet(int.Parse(betDropdown.options[betDropdown.value].text));
        betDropdown.interactable = false;
    }

    int getPointsPlayer() {
        int points = 0;
        for (int i = 0; i < player.GetComponent<CardHand>().cards.Count; i++)
        {
            points += player.GetComponent<CardHand>().cards[i].GetComponent<CardModel>().value;
        }
        return points;
    }

    int getPointsDealer()
    {
        int points = 0;
        for (int i = 0; i < dealer.GetComponent<CardHand>().cards.Count; i++)
        {
            points += dealer.GetComponent<CardHand>().cards[i].GetComponent<CardModel>().value;
        }
        return points;
    }

    void updatePoints() {
        playerPoints.text = "Player: " + getPointsPlayer();
        dealerPoints.text = "Dealer: " + getPointsDealer();
    }
    
}
