using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    const int BUFFERLEN = 180;
    public static int dpad = 5;
    public static int hori = 0;
    public static int vert = 0;
    bool faceLeft = false;
    InputAction directionAction;

    // Charge Timers
    int chargeTimerBack = 0;
    int chargeTimerDown = 0;
    int chargeTimerForward = 0;
    int chargeTimerUp = 0;
    const int MINCHARGETIME = 60;
    const int CHARGEUSETIME = 12;
    const int CHARGETOOSLOW = -12;
    bool sfstrictcharge = false;
    int useTimerBack = 0;
    int useTimerDown = 0;
    int useTimerForward = 0;
    int useTimerUp = 0;
    
    // example inputs being created for some common motions
    inputMotion shoryu = new inputMotion("Shoryuken!").Add(6,8,true).Add(2,8,false).Add(6,12,false); //fluent code, baybee!
    inputMotion hadouken = new inputMotion("Hadouken!").Add(2,10,true).Add(3,10,true).Add(6,12,true);
    inputMotion hcb = new inputMotion("Half Circle Back!").Add(6,12,true).Add(2,18,false).Add(4,12,true);

    inputMotion SPD = new inputMotion("360 Spinning Pile Driver!").Add(6,12,false).Add(2,12,false).Add(4,12,false).Add(8,8,false);
    inputMotion RSPD = new inputMotion("Reverse 360 Spinning Pile Driver!").Add(4,12,false).Add(2,12,false).Add(6,12,false).Add(8,8,false);

    inputMotion tigerKnee = new inputMotion("Tiger Knee!").Add(2,8,true).Add(6,12,false).Add(9,8,true);

    inputMotion qcfx2 = new inputMotion("Super!").Add(2,12,true).Add(6,12,true).Add(2,12,true).Add(6,12,true);
    inputMotion hcbf = new inputMotion("Half Circle Back Forward!").Add(6,12,true).Add(2,24,false).Add(4,12,false).Add(6,12,true);

    inputMotion[] motionQueue;
    inputMotion[] failedMotionQueue;

    public static List<int> dirBuffer = new List<int>();
    private void Awake() {
        motionQueue = new inputMotion[]{tightspin720, tightrspin720, spin720, rspin720, strictSPD, strictRSPD, SPD, RSPD, hcbf, hcfb, qcfx2, qcbx2, revElectric, revShoryu, electric, shoryu, hcf, hadouken, hcb, tatsu, downdown};
        failedMotionQueue = new inputMotion[]{slowShoryu, slowRevShoryu, slowhcf, slowhcb, slowHadouken, slowTatsu, fastShoryu, fastRevShoryu, fasthcb, fasthcf, fastHadouken, fastTatsu, badHadou, badTatsu};
        //sets application to vsync at 60fps
        QualitySettings.vSyncCount = 1;
        //ignored because vsync is on, but if vsync were turned off, would fall back to this
        Application.targetFrameRate = 60;
        
        playerInput.actions["Direction"].performed += OnDir;
        directionAction = InputSystem.actions.FindAction("Direction");
    }
    void Start()
    {
        directionAction = InputSystem.actions.FindAction("Direction");
        dirBuffer = new List<int>();
    }

    public void OnDir(InputAction.CallbackContext context)
    {
        int hori = Sign(directionAction.ReadValue<Vector2>()[0]);
        int vert = Sign(directionAction.ReadValue<Vector2>()[1]);
        int dpad = hori + 2 + ((vert + 1) * 3);
        if(dpad == 5) return;
    }
    public static int Sign(float value)
    {
        if (value < 0) return -1;
        else if (value > 0) return 1;
        else if (value == 0) return 0;
        return 0;
    }
    
    // Update is called once per frame
    void Update()
    {
        hori = Sign(directionAction.ReadValue<Vector2>()[0]);
        vert = Sign(directionAction.ReadValue<Vector2>()[1]);
        dpad = hori + 2 + ((vert + 1) * 3);
        ball.transform.position = splinePath.Spline[dpad].Position + splinePath.EvaluatePosition(0,0.0f);
    }
    void FixedUpdate()
    {
        while (dirBuffer.Count > BUFFERLEN)
        { //clears inputs past the buffer length
            dirBuffer.RemoveAt(dirBuffer.Count - 1);
        }
        dirBuffer.Insert(0, dpad);

        //Charge timer code
        switch(dpad){
            case 4:
            if(faceLeft){
                chargeTimerForward ++;
                chargeTimerBack = 0;
            } else{
                chargeTimerBack ++;
                chargeTimerForward = 0;
            }
            chargeTimerDown = 0;
            chargeTimerUp = 0;
            break;
            case 2:
            chargeTimerDown ++;
            chargeTimerBack = 0;
            chargeTimerForward = 0;
            chargeTimerUp = 0;
            break;
            case 6:
            if(faceLeft){
                chargeTimerBack ++;
                chargeTimerForward = 0;
            } else{
                chargeTimerForward ++;
                chargeTimerBack = 0;
            }
            chargeTimerDown = 0;
            chargeTimerUp = 0;
            break;
            case 8:
            chargeTimerUp ++;
            chargeTimerBack = 0;
            chargeTimerDown = 0;
            chargeTimerForward = 0;
            break;
            case 1:
            if(faceLeft){
                chargeTimerForward ++;
                chargeTimerBack = 0;
            } else{
                chargeTimerBack ++;
                chargeTimerForward = 0;
            }
            chargeTimerDown ++;
            chargeTimerUp = 0;
            break;
            case 3:
            if(faceLeft){
                chargeTimerBack ++;
                chargeTimerForward = 0;
            } else{
                chargeTimerForward ++;
                chargeTimerBack = 0;
            }
            chargeTimerDown ++;
            chargeTimerUp = 0;
            break;
            case 9:
            if(faceLeft){
                chargeTimerBack ++;
                chargeTimerForward = 0;
            } else{
                chargeTimerForward ++;
                chargeTimerBack = 0;
            }
            chargeTimerUp ++;
            chargeTimerDown = 0;
            break;
            case 7:
            if(faceLeft){
                chargeTimerForward ++;
                chargeTimerBack = 0;
            } else{
                chargeTimerBack ++;
                chargeTimerForward = 0;
            }
            chargeTimerUp ++;
            chargeTimerDown = 0;
            break;
            case 5:
            case 0:
            chargeTimerBack = 0;
            chargeTimerDown = 0;
            chargeTimerForward = 0;
            chargeTimerUp = 0;
            break;
        }
        if(chargeTimerBack >= MINCHARGETIME){
            useTimerBack = 12;
        } else if (useTimerBack > CHARGETOOSLOW) useTimerBack --;
        if(chargeTimerDown >= MINCHARGETIME){
            useTimerDown = 12;
        } else if (useTimerDown > CHARGETOOSLOW) useTimerDown --;
        if(chargeTimerForward >= MINCHARGETIME){
            useTimerForward = 12;
        } else if (chargeTimerForward > CHARGETOOSLOW) useTimerForward --;
        if(chargeTimerUp >= MINCHARGETIME){
            useTimerUp = 12;
        } else if (useTimerUp > CHARGETOOSLOW) useTimerUp --;
    }

    bool checkCharge(string button){
        //If we want to switch this to street fighter strictness, we can replace Sign(directionAction.ReadValue<Vector2>()[0]) with dpad == 6
        if(sfstrictcharge){
            if(dpad == 6) hori = 1;
            else if(dpad == 4) hori = -1;
            else if (dpad == 8) vert = 1;
            else if (dpad == 4) vert = -1;
            else {
                hori = 0;
                vert = 0;
            }
        }else{
            hori = Sign(directionAction.ReadValue<Vector2>()[0]);
            vert = Sign(directionAction.ReadValue<Vector2>()[1]);
        }
        if(faceLeft){
            if(hori < 0 && useTimerBack > 0){
                successMessage.text = button + " Sonniku Bewm!";
                Debug.Log(button + " Sonniku Bewm!");
                return true;
            } else if (hori < 0 && useTimerBack > CHARGETOOSLOW){
                successMessage.text = button + " expired charge back-forward!";
                Debug.Log(button + " expired charge back-forward!");
                return false;
            }
            if(hori > 0 && useTimerForward > 0){
                successMessage.text = button + " Mico Ruseo!";
                Debug.Log(button + " Mico Ruseo!");
                return true;
            } else if (hori > 0 && useTimerForward > CHARGETOOSLOW){
                successMessage.text = button + " expired charge forward-back!";
                Debug.Log(button + " expired charge forward-back!");
                return false;
            }
        }
        if(hori > 0 && useTimerBack > 0){
            successMessage.text = button + " Sonniku Bewm!";
            Debug.Log(button + " Sonniku Bewm!");
            return true;
        } else if (hori > 0 && useTimerBack > CHARGETOOSLOW){
            successMessage.text = button + " expired charge back-forward!";
            Debug.Log(button + " expired charge back-forward!");
            return false;
        }
        if(hori < 0 && useTimerForward > 0){
            successMessage.text = button + " Mico Ruseo!";
            Debug.Log(button + " Mico Ruseo!");
            return true;
        } else if (hori < 0 && useTimerForward > CHARGETOOSLOW){
            successMessage.text = button + " expired charge forward-back!";
            Debug.Log(button + " expired charge forward-back!");
            return false;
        }
        if(vert > 0 && useTimerDown > 0){
            successMessage.text = button + " Somasalt!";
            Debug.Log(button + " Somasalt!");
            return true;
        } else if (vert > 0 && useTimerDown > CHARGETOOSLOW){
            successMessage.text = button + " expired charge down-up!";
            Debug.Log(button + " expired charge down-up!");
            return false;
        }
        if(vert < 0 && useTimerUp > 0){
            successMessage.text = button + " Beedrill!";
            Debug.Log(button + " Beedrill!");
            return true;
        } else if (vert < 0 && useTimerUp > CHARGETOOSLOW){
            successMessage.text = button + " expired charge up-down!";
            Debug.Log(button + " expired charge up-down!");
            return false;
        }
        return false;
    }
}
