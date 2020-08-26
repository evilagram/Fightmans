using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//direction units hold a direction, the window of time that direction needs to be pressed across,
//and whether it's an exact match or fuzzy match
struct dirUnit{
    public dirUnit(int newdir, int newwind, bool newstrict){
        direction = newdir;
        window = newwind;
        strict = newstrict;
    }
    public int direction;
    public int window;
    public bool strict;
}

//an input object stores a whole command motion in the form of a list of directionUnits
//it contains several methods for determining if an input is valid, by checking the directional input buffer stored in the game controller
//it keeps this list backwards, reversing it when adding new entries, because we read the input buffer for a valid command motion
//when the button is pressed, not all the time, so we're checking from the most recent inputs backwards to the least recent,
//meaning we need to check the last direction in the motion first, so it's simpler to just store the whole thing backwards and
//reverse it when users interact with it so it's sensible to humans.
class inputMotion{
    public inputMotion(string nameme, List<dirUnit> newInputs){
        name = nameme;
        inputList = newInputs;
        inputList.Reverse();
    }
    public inputMotion(string nameme){
        name = nameme;
        inputList = new List<dirUnit>();
    }
    public string name;
    List<dirUnit> inputList;

    public inputMotion Add(int direction,int window, bool strict){
        inputList.Reverse();//the list needs to be reversed, because you're always checking the window of the last input first
        inputList.Add(new dirUnit(direction,window,strict));
        inputList.Reverse();//last input first, because it happened most recently, first input last because that was least recently
        return this; //lets you add immediately after instantiating the object, and chain more adds
    }

    public bool checkValidInput(){
        return checkValidInput(0, 0); //this means I can publicly call checkValidInput() on the object without needing to specify 0 every time
    }

    //this is a recursive function, so that I can nest multiple for loops inside of each other arbitrarily.
    private bool checkValidInput(int curInput, int bufferpos){ //takes in the current buffer (unnecessary if the buffer is stored publicly), current position in the input list as an int
        for(int i = bufferpos; i < bufferpos + inputList[curInput].window; i++){
            if(GameController.dirBuffer[i] == 5) continue; //if it's a blank input this frame, skips to the next frame
            if(checkDir(GameController.dirBuffer[i], inputList[curInput].direction, inputList[curInput].strict)){
                Debug.Log("Check success, iter: "+ curInput + " curinput: " + GameController.dirBuffer[i] + " i: " + i + " target: " + inputList[curInput].direction);
                if (curInput+1 >= inputList.Count) { //if there's no input at this point in the list, we're done, return true
                    Debug.Log(name);
                    return true;
                } else return checkValidInput(curInput+1, i+1);
            }
        }
        return false;
    }

    //checks if the currently checked direction matches the one in the input object, ideally takes in a direction, and an input object
    //corrects the held direction based on the facing direction, then compares it to the input object's direction, or multiple similar directions if set to loose
    //consequence of this is that cross-cut shoryukens don't work versus crossups, you need to input the shoryuken backwards
    //to get the correct effect. This could be "fixed" by having the direction correction over in the directional buffer instead.
    bool checkDir(int curDir, int targetDir, bool strict){
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
