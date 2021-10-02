using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBrain : MonoBehaviour
{
    public class slotClassObj{
        public GameObject slotGameObject; //gameobj (what displays picture)
        public string slotImageTag; //what type of item it is (auto sets to clubs)
        public slotClassObj(GameObject slotObj){ //contructor
            slotGameObject = slotObj; //set gameobject to prefab when you create the slotface.
            slotImageTag = "club"; //set default image of slot images to clubs.
        }
    }
    public GameObject slotObjPrefab;
    public GameObject topBar;
    public slotClassObj objTest;
    bool hitBottom = false; 

    public Sprite[] slotImages;

    List<slotClassObj> slotScreen = new List<slotClassObj>();
    //List<List<slotClassObj>> slotScreen = new List<List<slotClassObj>>(); //basically an 2D array but at function.. this is how you define a 2d generic list.
    int t = 0;

    void Awake(){
        
        
        // SPAWN ALL CUBES INTO GAME 8x8
        int x = 0;
        int y = 0;
        for (int i = 0; i < 64; i++)
        {
            objTest = new slotClassObj(Instantiate(slotObjPrefab));
            slotScreen.Add(objTest);
            y++;
            if(i % 8 == 0){
                x = i/8;
                y = 0;
            }
            slotScreen[i].slotGameObject.transform.position = new Vector3(x,y,0);

            Sprite tempSprite;
            int randomInt = Random.Range(0,4);
            tempSprite = slotImages[randomInt];

            slotScreen[i].slotGameObject.GetComponent<SpriteRenderer>().sprite = tempSprite;
        }
        /*
         objTest = new slotClassObj(Instantiate(slotObjPrefab));
        screen.Add(objTest);
        objTest = new slotClassObj(Instantiate(slotObjPrefab));
        screen.Add(objTest);
        screen[0].slotGameObject.transform.position = new Vector3(1,0,0);
        screen[1].slotGameObject.transform.position = new Vector3(-1,0,0);
        
        for (int i = 0; i < 6; i++)
        {
            slotScreen.Add(new List<slotClassObj>());
            for (int c = 0; c < 6; c++)
            {
                objTest = new slotClassObj(Instantiate(slotObjPrefab));
                slotScreen[i].Add(objTest);
                slotScreen[i][c].slotGameObject.transform.position = new Vector3(i - 2,c + 5,0);
                slotScreen[i][c].slotGameObject.name = (i.ToString()+c.ToString());
            }
        }*/
    }
    bool atBottom = false;
    void FixedUpdate(){
        if(!atBottom){
            bool belowTarget = true;
            for (int i = 0; i < 64; i++)
            {
                if(slotScreen[i].slotGameObject.transform.position.y >=topBar.transform.position.y + 1){
                    belowTarget = false;
                }
            }
            if(belowTarget){
                
                atBottom = true;
            }
        }
        if(atBottom){
            Debug.Log("all stopped");
        }
       /*if(!hitBottom){
           for (int c = 0; c < 36; c++)
            {
                if(slotScreen[q][5].slotGameObject.transform.position.y <= 4 ){
                            q++;
                            return;
                } 
                if(c - q * 5 < 6 && c - q * 5 >= 0 ){
                    Debug.Log(q + " " + (c - (q * 5)));
                    
                    slotScreen[q][c - (q * 5)].slotGameObject.transform.position =  slotScreen[q][c - (q * 5)].slotGameObject.transform.position  + new Vector3(0,-6,0) * Time.deltaTime;
                     
                }
                
                        
            }
        }else if (slotScreen[slotScreen.Count][0].slotGameObject.transform.position.y <= -2) {
            hitBottom = true;
        }*/
    }

    //When you press the start button on the screen
    public void StartButton(){
        atBottom = false;
        for (int i = 0; i < 64; i++)
        {
            slotScreen[i].slotGameObject.transform.position = slotScreen[i].slotGameObject.transform.position + new Vector3(0,5,0);
        }
        /*for (int i = 0; i < 6; i++)
        {
            for (int c = 0; c < 6; c++)
            {
                slotScreen[i][c].slotGameObject.transform.position = new Vector3(i - 2,c + 5,0);

            }
        }
        
        hitBottom = false;
        q = 0;
        */

        
        //objTest.slotGameObject.transform.position = new Vector3(0,0 + i++,0);
    }


}