using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*To do List
1. spawn 8x8 grid of random symbols -- DONZO :) --
2. Detect bunches of symbols
3. Delete old symbols + keep track some how
4. Add new symbols + fall down..
*/

public class SlotManagerV2 : MonoBehaviour
{
    // --------------------------- CLASSES ---------------------------------
    public class slotClassObj{ //Class which is what every tile is made of! and then stored within a 2D list.
        public GameObject slotGameObject; //Gameobject of tile
        public string slotImageTag; //Reference to what type of tile this is.

        public bool tileChecked;
        public bool tileCounted;
        public slotClassObj(GameObject slotObj){ //contructor
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

    private int tileCount = 0;
    private bool droppingTiles = false;
    [SerializeField] private int slotMoveSpeed = 6; //movementspeed of the slot

    // --------------------------- FUNCTIONS ---------------------------------
    void Awake(){
        
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
                slotScreen[i][c].slotGameObject.transform.position = new Vector3(i - 2,c-4,0); //i & c spread them out equally and then use +/- to position into place
                //set names of tiles 
                slotScreen[i][c].slotGameObject.name = (i.ToString()+c.ToString());
                //randomize what type of tile it is. 
                randomizeGameobjects(i,c);
            }
        }
    }
    void FixedUpdate(){
       
       if(droppingTiles == true){
           DropDownTiles();
       }
    }
   
    void randomizeGameobjects(int i, int c){ //Randomize tiles (pass current tile pos that you want to randomize!)
        //Get a random integer to pick what type of tile it will be  
        int randomInt = Random.Range(0,4); //int makes whole && range is from 0-3.999 *but since whole # deletes decimal.
        
        //set image of tile to correct sprite.
        slotScreen[i][c].slotGameObject.GetComponent<SpriteRenderer>().sprite = slotImages[randomInt];
       
        //set string in list so tile knows what type it is.
        switch(randomInt){
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
        //slotScreen[i][c].slotGameObject.name = (i.ToString()+c.ToString());
       
    }
    

    //Find what tiles are in clusters of 5+ and then highlight them 
    public void HighlightTiles(){
        
            //Rotate through 8x8 grid.
            for (int i = 0; i < 8; i++)
            {
                for (int c = 0; c < 8; c++)
                {
                    //holds count of how many tiles within cluster, resets to 0 when new item.
                    tileCount = 0; 
                    
                    //Make sure tile hasnt already been in another cluster!
                    if(!slotScreen[i][c].tileChecked){
                        checkTile(i,c, slotScreen[i][c].slotImageTag); 
                    }
                    //Check if big enough cluster!
                    //if there was enough tiles in that cluster... re-cycle through those tiles and collect(highlight) them.
                    if(tileCount >=  5){
                        //Highlight the tiles that are in a large enough cluster + tag them special.
                        changeTiles(i,c, slotScreen[i][c].slotImageTag);
                    }
                }
            }
    }
    void checkTile(int i, int c, string lookingFor){
        //I am done checking the tile im on.
        slotScreen[i][c].tileChecked = true;
        //Found another one of these tiles.
        tileCount ++;
        //check that you arent going out of bounds
        if(i > 0){
            //Check if there is the same string to the left.
            if(lookingFor == slotScreen[i - 1][c].slotImageTag && !slotScreen[i-1][c].tileChecked){
                checkTile(i-1,c, lookingFor);
            }
        }
        //check that you arent going out of bounds
        if(c > 0){
            //Check if there is the same string above.
            if(lookingFor == slotScreen[i][c - 1].slotImageTag&& !slotScreen[i][c-1].tileChecked){
                checkTile(i,c-1, lookingFor);
            }
        }
        //check that you arent going out of bounds
        if(i < slotScreen.Count -1){
            //Check if there is the same string to the right.
            if(lookingFor == slotScreen[i+ 1][c].slotImageTag&& !slotScreen[i+1][c].tileChecked){
                checkTile(i + 1,c, lookingFor);
            }
        }
        //check that you arent going out of bounds
        if(c < slotScreen.Count -1){
            //Check if there is the same string below.
            if(lookingFor == slotScreen[i][c + 1].slotImageTag&& !slotScreen[i][c+1].tileChecked){
                checkTile(i,c + 1, lookingFor);
            }
        }
        
    }
    void changeTiles(int i, int c, string lookingFor){
        //I am done counting the tile im on.
        slotScreen[i][c].tileCounted = true;
        slotScreen[i][c].slotGameObject.GetComponent<SpriteRenderer>().color = Color.green;
        //check that you arent going out of bounds
        if(i > 0){
            //Check if there is the same string to the left.
            if(lookingFor == slotScreen[i - 1][c].slotImageTag && !slotScreen[i-1][c].tileCounted){
                changeTiles(i-1,c, lookingFor);
            }
        }
        //check that you arent going out of bounds
        if(c > 0){
            //Check if there is the same string above.
            if(lookingFor == slotScreen[i][c - 1].slotImageTag&& !slotScreen[i][c-1].tileCounted){
                changeTiles(i,c-1, lookingFor);
            }
        }
        //check that you arent going out of bounds
        if(i < slotScreen.Count -1){
            //Check if there is the same string to the right.
            if(lookingFor == slotScreen[i+ 1][c].slotImageTag&& !slotScreen[i+1][c].tileCounted){
                changeTiles(i + 1,c, lookingFor);
            }
        }
        //check that you arent going out of bounds
        if(c < slotScreen.Count -1){
            //Check if there is the same string below.
            if(lookingFor == slotScreen[i][c + 1].slotImageTag&& !slotScreen[i][c+1].tileCounted){
                changeTiles(i,c + 1, lookingFor);
            }
        }
        
    }

    public void DeleteButton(){
        DeleteTiles();
    }
    public void AddButton(){
        AddTiles();
    }
    public void DropButton(){
        droppingTiles = true;
    }
    public void HighlightButton(){
        HighlightTiles();
    }
    
    void DeleteTiles(){
        int destroying = 0;
        while(destroying < 9){

        for (int i = 0; i < 8; i++)
        {
            for (int c = 0; c <slotScreen[i].Count ; c++)
            {
                    if(slotScreen[i][c].tileChecked){
                        slotScreen[i][c].tileChecked = false;
                    }
                    if(slotScreen[i][c].tileCounted){
                        Destroy(slotScreen[i][c].slotGameObject);
                        slotScreen[i].RemoveAt(c);
                        Debug.Log(slotScreen[i].Count);
                        //slotClassObj objTest;
                        //objTest = new slotClassObj(Instantiate(slotObjPrefab));
                        //slotScreen[i].Add(objTest);
                        //randomizeGameobjects(i,c);
                        //slotScreen[i][slotScreen[i].Count-1].slotGameObject.transform.position = new Vector3(i - 2,slotScreen[i].Count-1 + 5,0);
                        break;
                        
                        
                    }
            }
        }
        destroying ++;
        }
    }
    void AddTiles(){
        int i = 0;
        while(i < 8){
            while(slotScreen[i].Count < 8){

                slotClassObj objTest;
                objTest = new slotClassObj(Instantiate(slotObjPrefab));
                slotScreen[i].Add(objTest);
                randomizeGameobjects(i,slotScreen[i].Count-1);
                slotScreen[i][slotScreen[i].Count-1].slotGameObject.transform.position = new Vector3(i - 2,slotScreen[i].Count-1 + 5,0);
            }
             i++;
        }
        
    }
//new Vector3(i - 2,c-4,0);
    void DropDownTiles(){
        bool didMove = false;
            //Rotate through 8x8 grid.
            for (int i = 0; i < 8; i++)
            {
                for (int c = 0; c < 8; c++)
                {
                    if(slotScreen[i][c].slotGameObject.transform.position.y > (c-4.0f)){
                        slotScreen[i][c].slotGameObject.transform.position =  slotScreen[i][c].slotGameObject.transform.position  + new Vector3(0,-slotMoveSpeed,0) * Time.deltaTime;
                        didMove = true;
                    }
                }
            } 
            if(didMove == false){
                droppingTiles = false;
            }
    }
    

}
