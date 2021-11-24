using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotManagerV2 : MonoBehaviour
{
    // --------------------------- CLASSES ---------------------------------
    public class slotClassObj
    { //Class which is what every tile is made of! and then stored within a 2D list.
        public GameObject slotGameObject; //Gameobject of tile
        public string slotImageTag; //Reference to what type of tile this is.

        public bool tileChecked;
        public bool tileCounted;
        public slotClassObj(GameObject slotObj)
        { //contructor
            slotGameObject = slotObj; //ref gameobject by passing into constructor
            slotImageTag = "club"; //set default string of what tile type it is. default is club 
            tileChecked = false;
            tileCounted = false;
        }
    }
    // --------------------------- VARIABLES ---------------------------------
    public GameObject slotObjPrefab; //prefab of tile (passed by inspector)

    [Header("the images below must be in the same order as the randomizer")]
    public Sprite[] slotImages;//tile images (passed by inspector)
    List<List<slotClassObj>> slotScreen = new List<List<slotClassObj>>(); //creation of 2D list.
    private int clusterSizeCount = 0; //Holds the value of how big a cluster is.
    [SerializeField] private bool fastPlay = true; // fast play meaning tiles drop all at once!

    //DROPPING VARIABLES
    private bool droppingTiles = false; // Are you currently dropping tiles on the screen
    [SerializeField] private int slotMoveSpeed = 6; //Movement speed of the tiles when LERP
    private int droppingCol = 0; //what is the current column that is dropping
    [SerializeField] private float tilePosSnap = 0.05f; //How close to drop objects before it snaps to position.

    //GAME LOOP VARIABLES
    private bool startGame = false; //Has the game started
    private bool gameRunning = false;//Is the game already running
    private float deleteCTR = 0; //Gameloop counter for how long to wait before deleting tiles after highlight 
    private float addCTR = 0;//Gameloop counter for how long to wait before adding and dropping new tiles after deleted.
    private bool tileHighlighted = false; //In the gameloop has it already highlighted the tiles?
    [SerializeField] private float timeB4Delete = 0.3f; //How long should the game wait b/w  highlighting and deleting objects

    //COIN VARIABLES
    [SerializeField] private int playerCoins = 100; //How many coins the player has in total
    [SerializeField] private int betSize = 10; //How much the player is spending per bet
    [SerializeField] private Text playersCoinText; //reference to the text object to display players coins.
    private int tilePremium = 1; //How much more money does the tile give you per cluster
    private int coinTotal = 0; //How many coins did the player earn that round
    private bool displayCoinTotal = false; //Round is over.. Display Coins.
    [SerializeField] private Text betSizeTextbox; //displays the betsize to player
    private int betSizeScale = 4;//bet size scale (so its not just +1 every time you hit the button)


    // --------------------------- FUNCTIONS ---------------------------------
    void Awake()
    {
        DisplayPCoins();
        changeBetSize();
        //create 8x8 grid
        for (int i = 0; i < 8; i++)
        {
            slotScreen.Add(new List<slotClassObj>()); //add new col
            for (int c = 0; c < 8; c++)
            {
                //populate each list (it is just a list of a list of game objects)
                slotClassObj objTest = new slotClassObj(Instantiate(slotObjPrefab)); //create new slotClassObject, and pass new prefab for its game object
                slotScreen[i].Add(objTest); // create new row/add item to current column.

                //set spawn position of tiles.
                slotScreen[i][c].slotGameObject.transform.position = new Vector3(i - 2, c - 4, 0); //i & c spread them out equally and then use +/- to position into place
                //set names of tiles 
                slotScreen[i][c].slotGameObject.name = (i.ToString() + c.ToString());
                //randomize what type of tile it is. 
                randomizeGameobjects(i, c);
                //slotScreen[i][c].slotGameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;
            }
        }
    }
    void FixedUpdate()
    {

        //GAME LOOP
        //Start game is set to true when you click the start button.
        if (startGame && playerCoins >= betSize)
        { //check that you have enough coins to play game 
            playerCoins -= betSize;
            DisplayPCoins();
            gameRunning = true;
        }
        else if (playerCoins < betSize && !gameRunning)
        { //if not enough coins then dont start.
            startGame = false;
        }
        //loop game loop if you had enough coins to start game.
        if (gameRunning)
        {
            GameLoop();
            displayCoinTotal = true;
        }
        //Display coin total when you run out of clusters to pop.
        else if (!gameRunning && displayCoinTotal)
        {
            if (betSizeScale == 0)
            {
                Debug.Log("You are playing free play");
            }
            else
            {
                Debug.Log("Round Over, you won " + coinTotal + " coins");
            }
            coinTotal = 0;
            displayCoinTotal = false;
        }

        //dropping tiles.
        if (droppingTiles == true)
        {
            DropDownTiles();
        }
    }

    //GAME LOOP
    void GameLoop()
    {
        if (startGame)
        { //did you press the start button 
            startGame = false;
            RandomizeButton(); //randomize + reset position of tiles
            DropButton(); //Drop tiles from top of screen down.
            deleteCTR = 0;
            addCTR = 0;
            tileHighlighted = false;
        }
        else if (deleteCTR >= timeB4Delete)
        { //has enough time passed after highlighting to delete tiles.
            deleteCTR = 0; //reset counter
            DeleteButton(); //delete tiles
            DropButton(); //drop tiles that are floating (needs timer between)
            if(fastPlay){
                AddButton();
                DropButton();
                tileHighlighted = false;
            }
        }
        else if (addCTR >= timeB4Delete + 0.2f && !fastPlay)
        {//has enough time passed after highlighting to add + drop tiles.
            addCTR = 0;
            AddButton();
            DropButton();
            tileHighlighted = false; //make it so it can highlight tiles again
        }
        else if (!droppingTiles)
        { //if you arent dropping tiles
            if (!tileHighlighted)
            { //highlight if you havent
                HighlightButton();
                tileHighlighted = true;
                deleteCTR = 0;
                addCTR = 0;
            }
            //add to counters!
            deleteCTR += Time.deltaTime;
            addCTR += Time.deltaTime;
        }

    }

    void randomizeGameobjects(int i, int c)
    { //Randomize tiles (pass current tile pos that you want to randomize!)
        //Get a random integer to pick what type of tile it will be  
        int randomInt = Random.Range(0, 4); //int makes whole && range is from 0-3.999 *but since whole # deletes decimal.

        //set image of tile to correct sprite.
        slotScreen[i][c].slotGameObject.GetComponent<SpriteRenderer>().sprite = slotImages[randomInt];

        //set string in list so tile knows what type it is.
        switch (randomInt)
        {
            case 0:
                slotScreen[i][c].slotImageTag = "club";
                break;
            case 1:
                slotScreen[i][c].slotImageTag = "diamond";
                break;
            case 2:
                slotScreen[i][c].slotImageTag = "heart";
                break;
            case 3:
                slotScreen[i][c].slotImageTag = "spade";
                break;

        }

        //reset color of object / shouldnt need this i dont think..
        //slotScreen[i][c].slotGameObject.GetComponent<SpriteRenderer>().color = Color.white;


        //reset name of object just for testing purposes.
        slotScreen[i][c].slotGameObject.name = (i.ToString() + c.ToString());
        slotScreen[i][c].tileChecked = false;
        slotScreen[i][c].tileCounted = false;
        slotScreen[i][c].slotGameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }


    //Find what tiles are in clusters of 5+ and then highlight them 
    public void HighlightTiles()
    {
        bool clusterExists = false;
        //Rotate through 8x8 grid.
        for (int i = 0; i < 8; i++)
        {
            for (int c = 0; c < 8; c++)
            {
                //holds count of how many tiles within cluster, resets to 0 when new item.
                clusterSizeCount = 0;

                //Make sure tile hasnt already been in another cluster!
                if (!slotScreen[i][c].tileChecked)
                {
                    checkTile(i, c, slotScreen[i][c].slotImageTag);
                }
                //Check if big enough cluster!
                //if there was enough tiles in that cluster... re-cycle through those tiles and collect(highlight) them.
                if (clusterSizeCount >= 5)
                {
                    clusterExists = true;
                    Debug.Log(clusterExists);
                    //Highlight the tiles that are in a large enough cluster + tag them special.
                    changeTiles(i, c, slotScreen[i][c].slotImageTag);
                    AddCoins(clusterSizeCount, slotScreen[i][c].slotImageTag);
                }
            }
        }
        Debug.Log(clusterExists);
        //if no clusters exist end game
        if (!clusterExists)
        {
            gameRunning = false;

        }
    }
    void checkTile(int i, int c, string lookingFor)
    {
        //I am done checking the tile im on.
        slotScreen[i][c].tileChecked = true;
        //Found another one of these tiles.
        clusterSizeCount++;
        //check that you arent going out of bounds
        if (i > 0)
        {
            //Check if there is the same string to the left.
            if (lookingFor == slotScreen[i - 1][c].slotImageTag && !slotScreen[i - 1][c].tileChecked)
            {
                checkTile(i - 1, c, lookingFor);
            }
        }
        //check that you arent going out of bounds
        if (c > 0)
        {
            //Check if there is the same string above.
            if (lookingFor == slotScreen[i][c - 1].slotImageTag && !slotScreen[i][c - 1].tileChecked)
            {
                checkTile(i, c - 1, lookingFor);
            }
        }
        //check that you arent going out of bounds
        if (i < slotScreen.Count - 1)
        {
            //Check if there is the same string to the right.
            if (lookingFor == slotScreen[i + 1][c].slotImageTag && !slotScreen[i + 1][c].tileChecked)
            {
                checkTile(i + 1, c, lookingFor);
            }
        }
        //check that you arent going out of bounds
        if (c < slotScreen.Count - 1)
        {
            //Check if there is the same string below.
            if (lookingFor == slotScreen[i][c + 1].slotImageTag && !slotScreen[i][c + 1].tileChecked)
            {
                checkTile(i, c + 1, lookingFor);
            }
        }

    }
    void changeTiles(int i, int c, string lookingFor)
    {
        //I am done counting the tile im on.
        slotScreen[i][c].tileCounted = true;
        slotScreen[i][c].slotGameObject.GetComponent<SpriteRenderer>().color = Color.green;
        //check that you arent going out of bounds
        if (i > 0)
        {
            //Check if there is the same string to the left.
            if (lookingFor == slotScreen[i - 1][c].slotImageTag && !slotScreen[i - 1][c].tileCounted)
            {
                changeTiles(i - 1, c, lookingFor);
            }
        }
        //check that you arent going out of bounds
        if (c > 0)
        {
            //Check if there is the same string above.
            if (lookingFor == slotScreen[i][c - 1].slotImageTag && !slotScreen[i][c - 1].tileCounted)
            {
                changeTiles(i, c - 1, lookingFor);
            }
        }
        //check that you arent going out of bounds
        if (i < slotScreen.Count - 1)
        {
            //Check if there is the same string to the right.
            if (lookingFor == slotScreen[i + 1][c].slotImageTag && !slotScreen[i + 1][c].tileCounted)
            {
                changeTiles(i + 1, c, lookingFor);
            }
        }
        //check that you arent going out of bounds
        if (c < slotScreen.Count - 1)
        {
            //Check if there is the same string below.
            if (lookingFor == slotScreen[i][c + 1].slotImageTag && !slotScreen[i][c + 1].tileCounted)
            {
                changeTiles(i, c + 1, lookingFor);
            }
        }

    }
    // ------------------------- BUTTONS ----------------------------
    public void DeleteButton()
    {
        DeleteTiles();
    }
    public void AddButton()
    {
        AddTiles();
    }
    public void DropButton()
    {
        droppingTiles = true;
    }
    public void RandomizeButton()
    {
        RandomizeAll();
    }
    public void HighlightButton()
    {


        HighlightTiles();
        DisplayPCoins();

    }
    public void StartButton()
    {
        if (!gameRunning)
        {
            startGame = true;
        }
    }
    public void IncreaseBetButton()
    { // increase bet size
        if (betSizeScale < 8)
        {
            betSizeScale++;
        }
        changeBetSize();
    }
    public void DecreaseBetButton()
    { //decrease bet size
        if (betSizeScale > 0)
        {
            betSizeScale--;
        }
        changeBetSize();
    }
    void changeBetSize()
    {
        //change bet size
        switch (betSizeScale)
        {
            case 0:
                betSize = 0;
                break;
            case 1:
                betSize = 1;
                break;
            case 2:
                betSize = 2;
                break;
            case 3:
                betSize = 5;
                break;
            case 4:
                betSize = 10;
                break;
            case 5:
                betSize = 20;
                break;
            case 6:
                betSize = 30;
                break;
            case 7:
                betSize = 40;
                break;
            case 8:
                betSize = 50;
                break;
        }
        //display bet size or free play
        if (betSizeScale > 0)
        {
            betSizeTextbox.text = betSize.ToString();
        }
        else
        {
            betSizeTextbox.text = "free play";
        }
    }

    void DeleteTiles()
    {
        int destroying = 0;
        while (destroying < 9)
        {

            for (int i = 0; i < 8; i++)
            {
                for (int c = 0; c < slotScreen[i].Count; c++)
                {
                    if (slotScreen[i][c].tileChecked)
                    {
                        slotScreen[i][c].tileChecked = false;
                    }
                    if (slotScreen[i][c].tileCounted)
                    {
                        Destroy(slotScreen[i][c].slotGameObject);
                        slotScreen[i].RemoveAt(c);
                        break;
                    }
                }
            }
            destroying++;
        }
    }
    void AddTiles()
    {
        int i = 0;
        while (i < 8)
        {
            while (slotScreen[i].Count < 8)
            {

                slotClassObj objTest;
                objTest = new slotClassObj(Instantiate(slotObjPrefab));
                slotScreen[i].Add(objTest);
                randomizeGameobjects(i, slotScreen[i].Count - 1);
                slotScreen[i][slotScreen[i].Count - 1].slotGameObject.transform.position = new Vector3(i - 2, slotScreen[i].Count - 1 + 5, 0);
            }
            i++;
        }

    }
    void DropDownTiles()
    {
        if (fastPlay)
        { //If you want all grids to drop at the same time!
            bool didMove = false;

            //Rotate through 8x8 grid.
            for (int i = 0; i < 8; i++)
            {
                for (int c = 0; c < slotScreen[i].Count; c++)
                {
                    if (slotScreen[i][c].slotGameObject.transform.position.y > (c - (4.0f - tilePosSnap)))
                    { //check if game object is above where you want it to snap from.
                      //slotScreen[i][c].slotGameObject.transform.position =  slotScreen[i][c].slotGameObject.transform.position  + new Vector3(0,-slotMoveSpeed,0) * Time.deltaTime;
                        slotScreen[i][c].slotGameObject.transform.position = Vector3.Lerp(slotScreen[i][c].slotGameObject.transform.position, new Vector3(slotScreen[i][c].slotGameObject.transform.position.x, c - 4.0f, slotScreen[i][c].slotGameObject.transform.position.z), Time.deltaTime * slotMoveSpeed);
                        didMove = true;
                    }
                    else if (slotScreen[i][c].slotGameObject.transform.position.y != (c - 4.0f))
                    { //check if tile is on the exact position you want
                      //Snap gameobject to correct position
                        slotScreen[i][c].slotGameObject.transform.position = new Vector3(slotScreen[i][c].slotGameObject.transform.position.x, c - 4.0f, slotScreen[i][c].slotGameObject.transform.position.z);
                    }
                }
            }


            if (didMove == false)
            {
                droppingTiles = false;
            }
        }
        if (!fastPlay)
        {
            bool didMove = true;
            bool colComplete = true;
            for (int i = 0; i < slotScreen[droppingCol].Count; i++)
            {
                if (slotScreen[droppingCol][i].slotGameObject.transform.position.y > (i - (3.0f)))
                { //check if game object is above where you want it to snap from.

                    slotScreen[droppingCol][i].slotGameObject.transform.position = Vector3.Lerp(slotScreen[droppingCol][i].slotGameObject.transform.position, new Vector3(slotScreen[droppingCol][i].slotGameObject.transform.position.x, i - 4.0f, slotScreen[droppingCol][i].slotGameObject.transform.position.z), Time.deltaTime * slotMoveSpeed);
                    colComplete = false;
                }
                else if (slotScreen[droppingCol][i].slotGameObject.transform.position.y > (i - (4.0f - tilePosSnap)))
                { //check if tile is on the exact position you want
                    slotScreen[droppingCol][i].slotGameObject.transform.position = Vector3.Lerp(slotScreen[droppingCol][i].slotGameObject.transform.position, new Vector3(slotScreen[droppingCol][i].slotGameObject.transform.position.x, i - 4.0f, slotScreen[droppingCol][i].slotGameObject.transform.position.z), Time.deltaTime * slotMoveSpeed * 2);
                    colComplete = false;
                }
                else if (slotScreen[droppingCol][i].slotGameObject.transform.position.y != (i - 4.0f))
                { //check if tile is on the exact position you want
                    //Snap gameobject to correct position
                    slotScreen[droppingCol][i].slotGameObject.transform.position = new Vector3(slotScreen[droppingCol][i].slotGameObject.transform.position.x, i - 4.0f, slotScreen[droppingCol][i].slotGameObject.transform.position.z);

                }
            }
            if (colComplete)
            {
                droppingCol++;
                if (droppingCol >= slotScreen.Count)
                {
                    didMove = false;
                }
            }
            if (didMove == false)
            {
                droppingTiles = false;
                droppingCol = 0;
            }
        }
    }

    void RandomizeAll()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int c = 0; c < slotScreen[i].Count; c++)
            {
                slotScreen[i][c].slotGameObject.transform.position = new Vector3(i - 2, c + 5, 0);
                randomizeGameobjects(i, c);
            }
        }
    }




    void DisplayPCoins()
    {
        //Debug.Log("players coins: " + playerCoins);
        playersCoinText.text = "players coins: " + playerCoins.ToString();
    }

    void AddCoins(int clusterSize, string objType)
    {
        //Set tile premium cost (how much each cluster is worth).
        if (objType == "club" || objType == "spade" || objType == "heart" || objType == "diamond")
        {
            tilePremium = 1;
        }

        int profit = 0;
        //Evaluate cluster size and add to players coins.
        switch (clusterSize)
        {

            case 5:
            case 6:
                profit = betSize / 10 * tilePremium;
                break;
            case 7:
            case 8:
                profit = betSize / 5 * tilePremium;
                break;
            case 9:
            case 10:
                profit = betSize / 2 * tilePremium;
                break;
            case 11:
            case 12:
                profit = betSize * 1 * tilePremium;
                break;
            case 13:
            case 14:
                profit = (betSize * 1 + betSize / 2) * tilePremium;
                break;
            case 15:
            case 16:
                profit = betSize * 2 * tilePremium;
                break;
            default:
                if (clusterSize > 16)
                {
                    profit = betSize * 5 * tilePremium;
                }
                else
                {
                    Debug.Log("Cluster size is not big enough");
                }
                break;

        }
        Debug.Log("profit is: " + profit);
        playerCoins += profit;
        coinTotal += profit;
    }











}