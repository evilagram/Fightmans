using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    public static int dpad = 5;
    public static int hori = 0;
    public static int vert = 0;
    
    // example inputs being created for some common motions
    inputMotion shoryu = new inputMotion("Shoryuken!").Add(6,8,true).Add(2,8,false).Add(6,12,false); //fluent code, baybee!
    inputMotion hadouken = new inputMotion("Hadouken!").Add(2,10,true).Add(3,10,true).Add(6,12,true);
    inputMotion hcb = new inputMotion("Half Circle Back!").Add(6,12,true).Add(2,18,false).Add(4,12,true);

    inputMotion SPD = new inputMotion("360 Spinning Pile Driver!").Add(6,12,false).Add(2,12,false).Add(4,12,false).Add(8,8,false);
    inputMotion RSPD = new inputMotion("Reverse 360 Spinning Pile Driver!").Add(4,12,false).Add(2,12,false).Add(6,12,false).Add(8,8,false);

    inputMotion tigerKnee = new inputMotion("Tiger Knee!").Add(2,8,true).Add(6,12,false).Add(9,8,true);

    inputMotion qcfx2 = new inputMotion("Super!").Add(2,12,true).Add(6,12,true).Add(2,12,true).Add(6,12,true);
    inputMotion hcbf = new inputMotion("Half Circle Back Forward!").Add(6,12,true).Add(2,24,false).Add(4,12,false).Add(6,12,true);



    public static List<int> dirBuffer = new List<int>();
    private void Awake() {
        //sets application to vsync at 60fps
        QualitySettings.vSyncCount = 1;

        //ignored because vsync is on, but if vsync were turned off, would fall back to this
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        dirBuffer = new List<int>();
    }
    void Update()
    {
        //all of this makes a single directional value out of a bunch of possible inputs, eventually this would be rerouted to rebindable keys
        if(!Input.GetKey("left") && !Input.GetKey("right") && !Input.GetKey("up") && !Input.GetKey("down")){
            dpad = 5;
            hori = 0;
            vert = 0;
        }
        else{
            if(Input.GetKey("left") && Input.GetKey("right") || !Input.GetKey("left") && !Input.GetKey("right")){
                hori = 0;
            }
            else if(Input.GetKey("left")){
                hori = -1;
            }
            else if(Input.GetKey("right")){
                hori = 1;
            }

            if(Input.GetKey("up") && Input.GetKey("down") || !Input.GetKey("up") && !Input.GetKey("down")){
                vert = 0;
            }
            else if(Input.GetKey("down")){
                vert = -1;
            }
            else if(Input.GetKey("up")){
                vert = 1;
            }
            dpad = hori + 2 + ((vert + 1) * 3); //hashes the directions together to make a single number without a ton of if/elses
        }
        while(dirBuffer.Count > 60){ //clears inputs past the buffer length, which I've set to 60, should technically be a variable
            dirBuffer.RemoveAt(dirBuffer.Count - 1);
        }
        if(Input.GetKeyDown("left") || Input.GetKeyDown("right") || Input.GetKeyDown("up") || Input.GetKeyDown("down") ||
        Input.GetKeyUp("left") || Input.GetKeyUp("right") || Input.GetKeyUp("up") || Input.GetKeyUp("down")){ //checks if there is any change to directional keys this frame
            //dirBuffer.Add(dpad);
            dirBuffer.Insert(0,dpad);
        }else{
            //dirBuffer.Add(5); //5 is ignored
            dirBuffer.Insert(0,5);
        }

        //this is the correct way inputs are supposed to be checked
        if(Input.GetKeyDown("space")){
            if(shoryu.checkValidInput()){
                Debug.Log("success returning");
            }
        }


        //older test code
        //lots of hardcoded values here
        if(Input.GetKeyDown("l")){
            for(int i = dirBuffer.Count - 1; i > dirBuffer.Count - 12; i--){
                Debug.Log(dirBuffer[i]);
                if(dirBuffer[i] == 5) continue;
                if(checkDir(dirBuffer[i],6,false)){
                    Debug.Log("first check success, " + dirBuffer[i]);
                    for(int j = i  - 1; j > i - 12; j--){
                        if(dirBuffer[j] == 5) continue;
                        if(checkDir(dirBuffer[j],2,false)){
                            Debug.Log("Second check success, " + dirBuffer[j]);
                            for(int k = j  - 1; k > j - 12; k--){
                                if(dirBuffer[k] == 5) continue;
                                if(checkDir(dirBuffer[k],6,false)){
                                    Debug.Log("Shoryuken! " + dirBuffer[k]);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }

        if(Input.GetKeyDown("j")){
            if(checkDir(3,6,false)){
                Debug.Log("first check success");
                if(checkDir(1,2,false)){
                    Debug.Log("Second check success");
                    if(checkDir(6,6,true)){
                        Debug.Log("Shoryuken!");
                    }
                }
            }
        }
    }
    //this is test code, no longer being used
    public bool checkDir(int curDir, int targetDir, bool strict){
        //step 1, correct the currently held direction based on the facing direction, so 6 is forward, and 4 is back.
        if (false /*facing left*/){ //TODO: add a way to detect facing direction
            if (curDir == 7 || curDir == 4 || curDir == 1) curDir += 2;
            else if (curDir == 9 || curDir == 6 || curDir == 3) curDir -= 2;
        }
        
        if (strict == true){
            if (curDir == targetDir) return true;
            else return false;
        }
        else { //do some fuzzy comparison shit
            //else ifs let it terminate if it finds a value before the others
            //the diagonals are at the bottom because it's less likely to have an input with fuzzy diagonals, so you do less comparisons
            //the ands let it early terminate if the target direction isn't right.
            if (targetDir == 6 && (curDir == 6 || curDir == 9 || curDir == 3)) return true;
            else if (targetDir == 4 && (curDir == 4 || curDir == 1 || curDir == 7)) return true;
            else if (targetDir == 2 && (curDir == 2 || curDir == 1 || curDir == 3)) return true;
            else if (targetDir == 8 && (curDir == 8 || curDir == 7 || curDir == 9)) return true;
            else if (targetDir == 3 && (curDir == 3 || curDir == 6 || curDir == 2)) return true;
            else if (targetDir == 1 && (curDir == 1 || curDir == 4 || curDir == 2)) return true;
            else if (targetDir == 9 && (curDir == 9 || curDir == 8 || curDir == 6)) return true;
            else if (targetDir == 7 && (curDir == 7 || curDir == 8 || curDir == 4)) return true;
        }
        return false;
    }
}
