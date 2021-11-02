using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    //CLASS that deals with slot objects. (appearance and variables.)
    public class slotClassObj{
        public GameObject slotGameObject; //gameobj (what displays picture)
        public string slotImageTag; //what type of item it is (auto sets to clubs)
        public bool moving;
        public bool tileChecked;
        public bool tileCounted;
        public slotClassObj(GameObject slotObj){ //contructor
            slotGameObject = slotObj; //set gameobject to prefab when you create the slotface.
            slotImageTag = "club"; //set default image of slot images to clubs.
            moving = false; //default not moving
            tileChecked = false; //default tile hasnt been checked
            tileCounted = false; //default tile hasnt been counted if its above 5
        }
    }
    public GameObject slotObjPrefab; //reference slot prefab (linked in the editor )
    public Sprite[] slotImages;//reference to slot images (linked in editor)
    List<List<slotClassObj>> slotScreen = new List<List<slotClassObj>>(); //basically an 2D array but at function.. this is how you define a 2d generic list.

    int colCount = 0; //What column are you moving.. 
    bool readyToCheck = false; //are all tiles in place and ready to check
    [SerializeField] private int slotMoveSpeed = 6; //movementspeed of the slot

    bool playGame = true;
    int tileCount = 0;
    void Awake(){
        
        //set the count of the column back to 0 
        colCount = 0;
        //Make an 8x8 list grid of the tiles. 
        for (int i = 0; i < 8; i++)
        {
            //Add a new list to the list
            slotScreen.Add(new List<slotClassObj>());
            for (int c = 0; c < 8; c++)
            {
                //populate each list (it is just a list of a list of game objects)
                slotClassObj objTest;
                objTest = new slotClassObj(Instantiate(slotObjPrefab));
                slotScreen[i].Add(objTest);
                //set spawn position of tiles.
                slotScreen[i][c].slotGameObject.transform.position = new Vector3(i - 2,c + 5,0);
                //set names of tiles 
                slotScreen[i][c].slotGameObject.name = (i.ToString()+c.ToString());
                //randomize what type of tile it is. 
                randomizeGameobjects(i,c);
            }
        }
    }
    void FixedUpdate(){
       
       //Check if you are supposed to be moving tiles;
       if(!readyToCheck && playGame){
           moveTiles();
        }
        //Check if you have tiles are all in place
        if(readyToCheck && playGame){
            highlightTiles();
            
           //destoryTiles();
        }
    }
    //Code that moves tiles at the start of the game!
    public void moveTiles(){
        
            
           //count of how many tiles are in each row.
           int _lastRowTile = slotScreen.Count - 1;

            
           //Cycle through all 8 items of that row.
           for (int c = 0; c < slotScreen.Count; c++)
            {
                //Check if all tiles are in their correct positions --> once ready set "readyToCheck" to true.
                
                //Check if all the top row items are below 4 in the y 
                if(slotScreen[colCount][_lastRowTile].slotGameObject.transform.position.y <= 4 ){
                            //add one to colCount to increase what col youre moving
                            colCount++;
                            //check that the colCount isnt out of bounds.
                            if(colCount > _lastRowTile){
                                readyToCheck = true;
                            }
                            return;
                } 
                //Check if each individual tile should be moving
                if(slotScreen[colCount][c].slotGameObject.transform.position.y > (-3 + c) &&  !slotScreen[colCount][c].moving){
                    slotScreen[colCount][c].moving = true;
                }
                //check if the moving tile is above where it should be. 
                else if(slotScreen[colCount][c].slotGameObject.transform.position.y > (-3 + c) &&  slotScreen[colCount][c].moving){
                    //Move the tiles within the one column
                    slotScreen[colCount][c ].slotGameObject.transform.position =  slotScreen[colCount][c].slotGameObject.transform.position  + new Vector3(0,-slotMoveSpeed,0) * Time.deltaTime;
                }
                //end movement of tile. 
                else if(slotScreen[colCount][c].slotGameObject.transform.position.y <= (-3 + c)){
                    slotScreen[colCount][c].moving = false;
                }

                
                    
                     
            }
    }
    //Highlight tiles when you have a bigger then 5 cluster! && checks if there are clusters big enough
    public void highlightTiles(){
        
            for (int i = 0; i < 8; i++)
            {
                for (int c = 0; c < 8; c++)
                {
                    //set to 0 so when you find a identical tile in the cluster you can keep track of how many.
                    tileCount = 0;
                    //if you havent checked the tile yet do this..
                    if(!slotScreen[i][c].tileChecked){
                        checkTile(i,c, slotScreen[i][c].slotImageTag);
                    }
                    //Check if big enough cluster!
                    //if there was enough tiles in that cluster... recycle through those tiles and collect them.
                    if(tileCount >=  5){
                        //Highlight the tiles that are in a large enough cluster + tag them special.
                        changeTiles(i,c, slotScreen[i][c].slotImageTag);
                    }
                }
            }
    }
    //Destroy any clusters that are large enough + add new tiles to replace..
    public void destoryTiles(){
         //DESTROY ITEMS AND MOVE THE ITEMS DOWN AND REPOPULATE GAME.. DOESNT QUITE WORK ATM
            for (int i = 0; i < 8; i++)
            {
                for (int c = 0; c < 8; c++)
                {
                    if(!slotScreen[i][c].tileCounted){
                        slotScreen[i][c].tileChecked = false;
                    }
                    if(slotScreen[i][c].tileCounted){
                        Destroy(slotScreen[i][c].slotGameObject);
                        //slotScreen[i].RemoveAt(c);
                        Debug.Log(slotScreen[i].Count);
                        //slotClassObj objTest;
                        //objTest = new slotClassObj(Instantiate(slotObjPrefab));
                        //slotScreen[i].Add(objTest);
                        //randomizeGameobjects(i,c);
                        //slotScreen[i][slotScreen[i].Count-1].slotGameObject.transform.position = new Vector3(i - 2,slotScreen[i].Count-1 + 5,0);
                        
                        
                    }
                }
            }
            playGame = false;
            //readyToCheck = false;
            //colCount = 0;
    }
    

    //When you press the start button on the screen
    public void StartButton(){
        
       /* for (int i = 0; i < 8; i++)
        {
            for (int c = 0; c < 8; c++)
            {
                slotScreen[i][c].slotGameObject.transform.position = new Vector3(i - 2,c + 5,0);

               randomizeGameobjects(i,c);
            }
        }
        
        readyToCheck = false;
        colCount = 0;
        */
        destoryTiles();
        
        
        
        //objTest.slotGameObject.transform.position = new Vector3(0,0 + i++,0);
    }
    
    //Randomize tiles (need to make more random and harder to get tough tiles eventually)
    void randomizeGameobjects(int i, int c){
        //Get a random integer to pick what type of tile it will be 
        int randomInt = Random.Range(0,4);
        
        //set image of tile to correct sprite.
        slotScreen[i][c].slotGameObject.GetComponent<SpriteRenderer>().sprite = slotImages[randomInt];
       
        //set string so tile knows what type it is.
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
        //reset name of object just for testing purposes.
        slotScreen[i][c].slotGameObject.name = (i.ToString()+c.ToString());
         //reset color of object
        slotScreen[i][c].slotGameObject.GetComponent<SpriteRenderer>().color = Color.white;
        //reset tags on item so it can be checked again..
        slotScreen[i][c].tileChecked = false;
        slotScreen[i][c].tileCounted = false;
        
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

}


/*

        Destroy(slotScreen[1][3].slotGameObject);
        slotScreen[1].RemoveAt(3);
        slotClassObj objTest;
        objTest = new slotClassObj(Instantiate(slotObjPrefab));
        slotScreen[1].Add(objTest);
*/