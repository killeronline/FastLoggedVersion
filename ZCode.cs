﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ZCode : MonoBehaviour
{
    #region PreZC

    // Visual and Audio Manager 
    public GameObject playerMainCamera;
    public GameObject soundManagerObject;
    public GameObject BoundaryLineNonStatic;
    public GameObject FastForwardIndicatorNonStatic;
    public static GameObject BoundaryLine;
    public static GameObject FastForwardIndicator;
    public static GameObject[] limitSpawnRings;
    public static GameObject[] limitTeamRings;    
    public static SSound MusicPlayer;
    public static bool isAudioAllowed;
    public static string informationOn12 ;
    public static int adjustableFPS;
    public static int timeForLastMemory;
    public static int gameDecision;
    public static int futureFalcon;
    public static bool isSync;

    // Blood Engine
    public GameObject SpillEngine;
    public static Vblood SpillScript;
    public static bool BloodToggle;

    // Later changed by server
    public static int ZNOP;
    public static int ZPID;
    public static int ZTID;
    public static int ZCOINS;
    public static int ZAIGEMS;
    public static int ZGTIME;
    public static int ZMINUTES;
    public static int ZMAXGROUP;
    public static int ZTUNITS;
    public static int[] ZUnitCosts;
    public static int[] ZDECK;
    public static int[] ZHealths;
    public static int[] ZMaxHealths;    
    public static string[] ZPLayerNames;
    public static bool ZDepButtonToggle;
    public static int ZAlive;
    public static int ZAIUnits;
    public static int ZFPS;
    public static int ZSelectedGID;
    public static bool isUnitsPanelVisible;
    public static bool isGameOver;
    public static bool isAIdefeated;
    public static bool isTimeUp;
    public static bool triggerAttack;
    public static bool WideLook;
    public static bool isJudgeMentDone;
    public static float ZROOT2;    
    public static int[,] ZCurrentUnitCount;
    public static int[,] ZTotalUnitCount;
    public static int[,] ZMaxUnitCount;
    public static int[] TowerH;
    public static int[] TowerX;
    public static int[] TowerY;
    public static bool isDrummedByAI;
    public static List<string> chatMsgList;        

    public static int GPG_GTIME;
    public static int GPG_STIME;
    public static int GPG_UL;
    public static int GPG_MsgsRec;
    public static int GPG_MsgsRExe;
    public static int GPG_MsgsSExe;
    public static int GPG_MsgsSlf;
    public static int[] GPG_PlayerLastSeen;    


    #endregion preZC

    public void BattleSetter()
    {
        isGameOver = false;
        isAIdefeated = false;
        isTimeUp = false;
        triggerAttack = false;
        isJudgeMentDone = false;
        WideLook = false;       
        GPG_GTIME = 0;
        GPG_STIME = 0;
        GPG_UL = 0;
        GPG_MsgsRec = 0;
        GPG_MsgsRExe = 0;
        GPG_MsgsSExe = 0;
        GPG_MsgsSlf = 0;
        GPG_PlayerLastSeen = new int[5];        
        ZFPS = 60;        
        ZHealths = new int[5];
        ZMaxHealths = new int[5];
        //ZPLayerNames = new string[5];
        isUnitsPanelVisible = true;
        for (int j = 0; j < 5; j++)
        {
            ZHealths[j] = -9999;
            ZMaxHealths[j] = -9999;
            //ZPLayerNames[j] = "Player " + j;
        }
        gameDecision = 0;
        futureFalcon = 1;// This will decide gap between player move and implementation
        isSync = false;
        adjustableFPS = 40;
        timeForLastMemory = 12;
        informationOn12 = "";        
        if (XStatics.NOP <= 1)
        {
            ZPID = 1;
            ZPLayerNames = new string[5];
            ZPLayerNames[1] = "Romans";
            ZPLayerNames[2] = "Carthaginians";
            futureFalcon = 0;
        }
        else
        {
            #if UNITY_EDITOR //ManualPid
            ZPID = 2;
            ZPLayerNames = new string[5];
            ZPLayerNames[1] = "Blue Player";
            ZPLayerNames[2] = "Red Player";
            ZPLayerNames[3] = "Green Player";
            ZPLayerNames[4] = "Yellow Player";
            XStatics.isMultiPlayer = false;            
            #endif
        }
        ZSelectedGID = -1;
        ZNOP = XStatics.NOP;
        // For Now i assume myself as Player3 in a 2v2 without MPlayer Room UI
        //ZPID = 4;
        adjustPlayerView();
        // Here very important is get the Player stats like country , Tgames , Twins , Tlosses , Tscore
        // Xstatics is all the global storage        
        ZTID = ZPID%2==0?2:1;
        ZCOINS = 1;
        ZAIGEMS = 1;
        ZGTIME = 0;
        ZMINUTES = 5;// Upon user request + gamer feel, we will increase time in version2 
        ZMAXGROUP = 9;
        ZTUNITS = 3;
        ZDepButtonToggle = false;
        ZAlive = 0;
        ZUnitCosts = new int[ZTUNITS + 1];
        ZDECK = new int[ZTUNITS + 1];
        for (int i = 0; i < ZTUNITS + 1; i++)
        {
            ZUnitCosts[i] = 0;
            ZDECK[i] = 0;
        }
        // Specific Unit Costs - Should be subtracted from reinforcements
        ZUnitCosts[1] = 2;
        ZUnitCosts[2] = 3;
        ZUnitCosts[3] = 5;
        // ZUC Values       
        int L3 = ZNOP <= 1 ? 3 : ZNOP + 1;
        ZCurrentUnitCount = new int[L3, ZTUNITS + 1];//4+1 players x 3+1 diff units WorstCase
        ZTotalUnitCount = new int[L3, ZTUNITS + 1];
        ZMaxUnitCount = new int[L3, ZTUNITS + 1];
        TowerH = new int[L3];
        TowerX = new int[L3];
        TowerY = new int[L3];
        isDrummedByAI = false;
        chatMsgList = new List<string>();
        if (XStatics.NOP >= 2 && XStatics.isMultiPlayer)
        {
            L2D.CreateLogFile();// detailed Log ... remove it for real deployment
        }
    }

    #region postZC

    public void SendMessage2GUI(string msg2)
    {
        guiCBScriptHolder.GetComponent<CbScript>().msgShow(msg2);
    }

    public GameObject guiCBScriptHolder;
    public Color[] castleColorsBase;
    public GameObject CameraForZoomObject;
    public Transform Ring;
    public GameObject RingAnimo;
    public GameObject TorusMesh;
    public Transform DestRing;
    public GameObject DestAnimo;
    public GameObject TorusMesh2;
    public Color attackColor;
    public Color selectColor;
    public Color moveColor;
    private float distBtwTroops = 3.001f;// This must be same as in spacing units in JLife

    public static string pad(int _text, int _slen)
    {
        string result = _text + "";
        int plen = _slen - result.Length;
        for (int i = 0; i < plen; i++)
        {
            result = "0" + result;// Left padding with 0
        }
        return result;
    }

    public static int rpad(string cBuf2, int start, int includedEnd)
    {
        int result = 0;
        bool leftPaddingEnded = false;
        string ctmp = String.Empty;
        for (int i = start; i <= includedEnd; i++)
        {
            if (cBuf2[i] != '0')
            {
                leftPaddingEnded = true;
            }
            if (leftPaddingEnded)
            {
                ctmp += cBuf2[i] + String.Empty;
            }
        }
        if (ctmp.Length > 0)
        {
            return Convert.ToInt32(ctmp);
        }
        return result;
    }

    public void EditorPause()
    {
        Debug.Break();
    }

    public void adjustPlayerView()
    {
        if ( XStatics.NOP <= 1 )
        {
            playerMainCamera.transform.position = new Vector3(30f, 2f, 30f);            
        }
        else if ( XStatics.NOP == 2 )// 2P
        {
            if ( ZCode.ZPID == 1 )
            {
                playerMainCamera.transform.position = new Vector3(30f, 2f, 30f);                
            }
            else//2
            {
                playerMainCamera.transform.position = new Vector3(80f, 2f, 30f);
                playerMainCamera.transform.Rotate(new Vector3(0f, 180f, 0f), Space.World);
            }            
        }
        else
        {
            if (ZCode.ZPID == 1)
            {
                playerMainCamera.transform.position = new Vector3(30f, 2f, 30f);
            }
            else if(ZCode.ZPID == 3)
            {
                playerMainCamera.transform.position = new Vector3(30f, 2f, 80f);
                playerMainCamera.transform.Rotate(new Vector3(0f, 90f, 0f), Space.World);
            }
            else if(ZCode.ZPID == 2)
            {
                playerMainCamera.transform.position = new Vector3(80f, 2f, 30f);
                playerMainCamera.transform.Rotate(new Vector3(0f, -90f, 0f), Space.World);
            }
            else//4
            {
                playerMainCamera.transform.position = new Vector3(80f, 2f, 80f);
                playerMainCamera.transform.Rotate(new Vector3(0f, 180f, 0f), Space.World);
            }
        }
    }

    public static float ForVBloodDist;

    public static void BGMusicAdjustment(bool bgMusicOption)
    {
        try
        {
            MusicPlayer.BGMusicBool(bgMusicOption);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error Triggering the BG Music");
        }

    }

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 40;//TargetFPS
        isAudioAllowed = true;
        MusicPlayer = soundManagerObject.gameObject.GetComponent<SSound>();
        SpillScript = SpillEngine.GetComponent<Vblood>();
        ForVBloodDist = distBtwTroops;
        BattleSetter();     
        Invoke("adjustCamera", 1f);
    }

    private int zoomCount = 0;
    private BE.MobileRTSCam mRCam;

    private void startCameraCinematically()
    {
        if (zoomCount < 40)// #Banga...requirement to see both towers
        {
            float tZoom = 95f;
            float cZoom = mRCam.zoomCurrent;
            float mZoom = cZoom + ((tZoom - cZoom) / 8);
            mRCam.zoomCurrent = mZoom;
            mRCam.SetCameraZoom(mZoom);
            //mRCam.SetCameraPosition(new Vector3(25, 10, 0));

            zoomCount++;
            Invoke("startCameraCinematically", 0.1f);
        }
    }

    private void adjustCamera()
    {
        try
        {
            mRCam = CameraForZoomObject.gameObject.GetComponent<BE.MobileRTSCam>();
            mRCam.SetCameraZoom(mRCam.zoomCurrent);
            startCameraCinematically();
        }
        catch (Exception e)
        {
            Invoke("adjustCamera", 1f);
        }
    }

    void chooseSelectionColor(int _pid)
    {
        //-- i am removing the _pid and setting to 0 as default move color --earlier:castleColorsBase[_pid];
        attackColor = castleColorsBase[0];
        selectColor = castleColorsBase[0];
        moveColor = castleColorsBase[0];
    }

    void onClick(Vector3 clickPoint)
    {        
        float s2x = (clickPoint.x / (int)distBtwTroops);
        int sx = (int)s2x;
        if (s2x - (int)s2x > 0.51f) { sx++; }

        float s2y = (clickPoint.z / (int)distBtwTroops);
        int sy = (int)s2y;
        if (s2y - (int)s2y > 0.51f) { sy++; }

        //sy = ((int)clickPoint.z / (int)distBtwTroops) ;        
        if (b != null && !isGameOver)
        {
            string log7;
            int n32x;
            int n32y;
            b.command(sx, sy, out log7, out n32x, out n32y);
            ZCode.ZSelectedGID = b.selectedGid;// Change color of selection for 1 second
            Ring.position = new Vector3(10f, -99f, 10f);
            DestRing.position = new Vector3(10f, -99f, 10f);
            float ringposX = sx * distBtwTroops;
            float ringposZ = sy * distBtwTroops;//yz relation
            if (log7.Equals("select"))
            {
                if (n32x > 0 || n32y > 0)//Human Ergo
                {
                    n32y += 1; // SpawnRings                   
                    ringposX = n32x * distBtwTroops;
                    ringposZ = n32y * distBtwTroops;
                }
                foreach (KeyValuePair<int, JLife> escript in b.allScripts)
                {
                    escript.Value.ManageSelectionRings();
                }
                //Ring.position = new Vector3( ringposX , 0.2f , ringposZ );
                //TorusMesh.GetComponent<SkinnedMeshRenderer>().material.color = selectColor;
                //RingAnimo.SendMessage("triggerRingAction",SendMessageOptions.DontRequireReceiver);
            }
            else if (log7.Equals("hamla"))
            {
                if (n32x > 0 || n32y > 0)
                {
                    ringposX = n32x * distBtwTroops;
                    ringposZ = n32y * distBtwTroops;
                }
                DestRing.position = new Vector3(ringposX, 0.2f, ringposZ);
                TorusMesh2.GetComponent<SkinnedMeshRenderer>().material.color = attackColor;
                DestAnimo.SendMessage("triggerRingAction", SendMessageOptions.DontRequireReceiver);
            }
            else if (log7.Equals("move"))
            {
                DestRing.position = new Vector3(ringposX, 0.2f, ringposZ);
                TorusMesh2.GetComponent<SkinnedMeshRenderer>().material.color = moveColor;
                DestAnimo.SendMessage("triggerRingAction", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                //print(log7);
            }
            RingAnimo.SendMessage("postRingAction", SendMessageOptions.DontRequireReceiver);
            DestAnimo.SendMessage("postRingAction", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void hideDestRing()
    {
        DestRing.position = new Vector3(10f, -99f, 10f);
    }

    public Transform arrowModelBase;
    public Transform[] Models;
    public static Begin b;

    void Start()
    {        
        b = new Begin(this, distBtwTroops, castleColorsBase);        
        ZFPS = b.maxFps;
        adjustableFPS = b.maxFps;
        ZROOT2 = (float)System.Math.Sqrt(2);        
        foreach (KeyValuePair<int, Fighter> entry in b.allUnits)
        {
            entry.Value.gid = (entry.Value.pid * 100) + entry.Value.wid;
            // Assigning GIDs
        }
        // EXP2
        if ( true && XStatics.NOP >= 2 && (!XStatics.isMultiPlayer) )// used for testing the FastForwardingLogic
        {
            //02028032128082$
            //Invoke("ExperimentSimulation", 15f );
        }
        
        Invoke("GameSpine", 1f / b.maxFps);
        
        BoundaryLine = BoundaryLineNonStatic;
        FastForwardIndicator = FastForwardIndicatorNonStatic;
        FastForwardIndicator.SetActive(false);
    }

    void ExperimentSimulation()// Remove its references after use
    {
        string tmpMoves = "01018032108082$";
        KID.ProcessMoves(tmpMoves);
        ZCode.informationOn12 += "Added Simulation P2 Spawn Move" + "\n" ;
    }

    public static void EpsilonFunction()
    {        
        if ( XStatics.isMultiPlayer )//Sending Epoch Time
        {
            int secondsSinceEpoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            //b.EpochSync[GPG_GTIME, ZPID] = secondsSinceEpoch;
            KID.SendBroadcastChatMessage(12, pad(GPG_GTIME, 3) + pad(ZPID, 1) + secondsSinceEpoch);
            bool isFastForwarded = false;
            long sE = 0;
            long nE = 0;
            for (int i = 1; i < 5; i++)
            {
                if (i != ZPID)
                {
                    long tE = (long)b.EpochSync[GPG_GTIME, i];
                    if (tE > 0)
                    {
                        sE += tE;
                        nE += 1;
                    }
                }
            }
            if (nE == 0)//No one is ahead of you
            {
                adjustableFPS = b.maxFps - 10 ;// barely 25 FPS
                //ZCode.informationOn12 = "";
            }
            else
            {
                adjustableFPS = 60 ; // barely 80 FPS
                isFastForwarded = true;
                /*
                long dE = 0;
                if (nE == 1)
                {
                    dE = secondsSinceEpoch - sE;
                }
                else
                {// I am unable to trust if this works same on x86 and x64 Architecture
                    dE = secondsSinceEpoch - (sE / nE);
                }

                if (dE > 0)
                {
                    adjustableFPS = 2 * b.maxFps ;
                    isFastForwarded = true;
                    ZCode.informationOn12 = "FF" + dE;
                }
                else
                {
                    adjustableFPS = b.maxFps;
                    ZCode.informationOn12 = "???" + dE;
                }
                */
            }
            if ( isFastForwarded )
            {
                FastForwardIndicator.SetActive(true);
            }
            else
            {
                FastForwardIndicator.SetActive(false);
            }
        }        
    }

    void GameSpine()
    {        
        b.UpdateCycle();
        if (b.gtime < (ZCode.ZMINUTES * 60))
        {
            if (isJudgeMentDone)
            {
                idleAllUnits();
            }
            else
            {
                Invoke("GameSpine", 1f / adjustableFPS );
            }
        }
        else
        {
            idleAllUnits();

            if (XStatics.NOP == 4)
            {
                int standT1 = 0;
                if (ZCode.TowerH[1] > 0)
                {
                    standT1++;
                }
                if (ZCode.TowerH[3] > 0)
                {
                    standT1++;
                }

                int standT2 = 0;
                if (ZCode.TowerH[2] > 0)
                {
                    standT2++;
                }
                if (ZCode.TowerH[4] > 0)
                {
                    standT2++;
                }

                if ( standT1 > standT2 )
                {
                    if (ZPID % 2 == 1) { ZCode.gameDecision = 1; }
                    else { ZCode.gameDecision = -1; }
                    ZCode.isGameOver = true;
                }
                else if ( standT2 > standT1 )
                {
                    if (ZPID % 2 == 0) { ZCode.gameDecision = 1; }
                    else { ZCode.gameDecision = -1; }
                    ZCode.isGameOver = true;
                }
                else
                {
                    // Balanced
                }                
            }
            if ( ! ZCode.isGameOver )
            {
                isTimeUp = true ;
            }                                    
        }        
        try
        {
            if ( mRCam.zoomCurrent < ((mRCam.zoomMin + mRCam.zoomMax)/2) )
            {
                WideLook = false;
            }
            else
            {
                WideLook = true;
            }
        }
        catch {;}
    }

    public static void idleAllUnits()
    {
        foreach (KeyValuePair<int, Fighter> entry in b.allUnits)
        {
            entry.Value.mode = 0;
            if (entry.Value.wid > 0)
            {
                b.allScripts[entry.Value.id].mode = 0;
                b.allScripts[entry.Value.id].TimeUp();
            }
        }
    }

    public void logThisText(string text)
    {
        print(text);
    }

    #endregion postZC
}

//------------------ T H I S --- M E A N S --- W A R ------------------------------

public class Begin
{
    #region preB

    public int NOP;
    public int PID;
    public int TID;
    public int COINS;
    public int selectedGid = -1;
    public ZCode baseScript;
    public Transform arrowModel;
    public Transform[] _models;
    public SortedDictionary<int, Transform> allModels;            // Holds ( id , Noviceid ) Relationship
    public SortedDictionary<int, JLife> allScripts;               // Holds ( id , Noviceid ) Relationship
    public SortedDictionary<int, Transform> allProjectileModels;  // Holds ( id , ArrowMesh ) Relationship        
    public string[,] TimeQueue ;                                  // Queuing For Multiplayer 
    public bool[,] isCommandExecuted;                             // isCommandExecuted T/F  
    public int[,] EpochSync;                                      // Autonomous TimeSync
    public int[] AckArray ;                                       // For Acknowledgement storing
    
    private string cBuffer = String.Empty ;    

    public void command(int _sx, int _sy, out string Log, out int n32x, out int n32y)
    {
        Log = "";
        lastClicked2 = 0;
        n32x = -1;
        n32y = -1;
        if (_sx < 0 || _sx >= groundLength || _sy < 0 || _sy >= groundWidth)
        {
            Logger("UCOB x:"+_sx+" y:"+_sy);
            return;// X || Y- Out Of Bounds
        }

        if (ZCode.ZDepButtonToggle)// is ready to deploy
        {
            int _g9x = _sx;
            int _g9y = _sy;                        
            int c9x = -1;
            int c9y = -1;
            bool isDeployManaged = false;
            if (PID == 1)//Tower Points
            {
                c9x = 10;
                c9y = 10;
            }
            if (PID == 2)
            {
                c9x = 30;
                c9y = 10;
            }
            if (PID == 3)
            {
                c9x = 10;
                c9y = 29;
            }
            if (PID == 4)
            {
                c9x = 30;
                c9y = 29;
            }
            int t9x = -1 ;
            int t9y = -1 ;
            int sAreaLimit = 50;// One Less that 25
            int kLinearLimit = 5;
            int triangleLength = 999;            
            if (calculateDistanceSquare( c9x , c9y , _g9x , _g9y ) > sAreaLimit)
            {                
                for ( int i = -kLinearLimit ; i <= kLinearLimit ; i++ )
                {
                    for ( int j = -kLinearLimit ; j <= kLinearLimit ; j++ )
                    {
                        t9x = c9x + i;
                        t9y = c9y + j;                        
                        int tmpTDistance =  calculateDistanceSquare(t9x, t9y, _g9x, _g9y);
                        if (  tmpTDistance < triangleLength )
                        {                            
                            _sx = t9x;
                            _sy = t9y;
                            triangleLength = tmpTDistance;
                        } 
                    }
                }
                isDeployManaged = true;
            }
            //Logger(String.Format("G:{0},{1} S:{2},{3} T:{4}",_g9x,_g9y,_sx,_sy,triangleLength));
            /* 
            int _s9x = -1;
            int _s9y = -1;
            if (PID == 1)// Quadrant for P1
            {                
                if (_sx > groundLength / 2)//&& _sy < groundWidth / 2 )
                {
                    _s9x = groundLength / 2;
                }
                if (_sy > groundWidth / 2)
                {
                    _s9y = groundWidth / 2;
                }
            }
            if (PID == 2)// Quadrant for P2
            {
                if (_sx < groundLength / 2)// && _sy < groundWidth / 2)
                {
                    _s9x = groundLength / 2;
                }
                if (_sy > groundWidth / 2)
                {
                    _s9y = groundWidth / 2;
                }
            }
            if (PID == 3)// Quadrant for P3
            {
                if (_sx > groundLength / 2)//&& _sy > groundWidth / 2)
                {
                    _s9x = groundLength / 2;
                }
                if (_sy < groundWidth / 2)
                {
                    _s9y = groundWidth / 2;
                }
            }
            if (PID == 4)// Quadrant for P4
            {
                if (_sx < groundLength / 2)//&& _sy > groundWidth / 2)
                {
                    _s9x = groundLength / 2;
                }
                if (_sy < groundWidth / 2)
                {
                    _s9y = groundWidth / 2;
                }
            }            
            if (_s9x != -1)
            {
                _sx = _s9x;
                isDeployManaged = true;
            }
            if (_s9y != -1)
            {
                _sy = _s9y;
                isDeployManaged = true;
            }
            */
            /*
            if ( ! deployAreaNear )
            {

                baseScript.SendMessage2GUI("Deploy Near To Base !!!");
                return;
            }//CLAMP AND DEPLOY
            */

            // DEPLOY SCRIPT
            // TEA TIME            
            // The deploy information should be pid , x , y and wid , so we can manage it with server variables
            // For now i made it P1
            // Spawn Function PID , _sx , _sy , Deck            
            int sGid = -1;
            sGid = spawnFunction(PID, ZCode.ZDECK, _sx, _sy, 'Q' );// For now i am using PID later on pid from request                        
            // on ACK from server u should clear the deck
            for (int c34 = 0; c34 < ZCode.ZDECK.Length; c34++)
            {
                ZCode.ZDECK[c34] = 0;
            }
            selectedGid = sGid;// Making it normal for further selection
            n32x = _sx;
            n32y = _sy;
            if (isDeployManaged)
            {
                int is_attack = 0;
                if (grid[_g9x, _g9y, 0] != 0 && allUnits.ContainsKey(grid[_g9x, _g9y, 0]) && allUnits[grid[_g9x, _g9y, 0]].tid != TID)
                {
                    is_attack = 1;
                }
                // The below call should be queued back to spawn like spawn and move or spawn and attack call
                cMove(PID, selectedGid, _g9x, _g9y, is_attack , 'Q' );// Needs Review "P2 so put PID=2"
                // i put -1 bcoz no sound on deploy and move... only horn on standing units move
            }
            ZCode.ZDepButtonToggle = false;
            ZCode.limitSpawnRings[ZCode.ZPID].SetActive(false);
            ZCode.BoundaryLine.SetActive(false);
            Log = "select";
            return;
        }


        int hitID = grid[_sx, _sy, 0];
        if (selectedGid == -1)
        {
            if (hitID != 0)
            {
                if (allUnits.ContainsKey(hitID))// fail safe in case of dead unit selection
                {
                    if (allUnits[hitID].pid == PID)
                    {
                        selectedGid = allUnits[hitID].gid;
                        Log = "select";
                    }
                }
            }
            // else clicked on empty square
        }
        else
        {// SECOND CLICK            
            if (hitID != 0 && allUnits.ContainsKey(hitID))
            {
                if (allUnits[hitID].pid == PID)
                {
                    selectedGid = allUnits[hitID].gid; // Change Selection
                    Log = "select";
                }
                else
                {
                    // Make Enemy                     
                    cMove(PID, selectedGid, _sx, _sy, 1 , 'Q' );//AFTER UPPER REVOLUTIONARY 13MAY                    
                    Log = "hamla";// 1 stands for attacking
                }
            }
            else// Move 
            {
                bool isHumanErgo = false;
                bool isMobileEnemy = false;
                int[] d3x = new int[] { -1, 0, 1, -1, 1, -1, 0, 1 };
                int[] d3y = new int[] { 1, 1, 1, 0, 0, -1, -1, -1 };
                for (int rt3 = 0; rt3 < d3x.Length; rt3++)
                {
                    int n3x = _sx + d3x[rt3];
                    int n3y = _sy + d3y[rt3];
                    if (!(n3x < 0 || n3x >= groundLength || n3y < 0 || n3y >= groundWidth))
                    {
                        int t3id = grid[n3x, n3y, 0];
                        if (t3id != 0 && allUnits[t3id].pid == PID && allUnits[t3id].gid != selectedGid && allUnits[t3id].wid > 0)//To ensure defense ergo
                        {
                            // 27th May = selecting non-empty square instead of moving side to side
                            selectedGid = allUnits[t3id].gid;// Selecting by human feel ,, nearest non-empty our unit                            
                            isHumanErgo = true;
                            Log = "select";
                            n32x = n3x;
                            n32y = n3y;
                            break;
                        }
                        else if (t3id != 0 && allUnits[t3id].tid != TID)// Ergo Attacko
                        {
                            //Logger("mobile enemy attack");
                            cMove(PID, selectedGid, _sx, _sy, 1 , 'Q' );//AFTER UPPER REVOLUTIONARY 13MAY                    
                            isMobileEnemy = true;
                            Log = "hamla";// 1 stands for attacking
                            n32x = n3x;
                            n32y = n3y;
                            break;
                        }

                    }
                }
                if (!isHumanErgo && !isMobileEnemy)
                {
                    cMove(PID, selectedGid, _sx, _sy, 0 , 'Q' );
                    //selectedGid = -1;
                    Log = "move";
                }
            }
        }
    }

    private void aimTheArrow(Transform t2, int _x, int _y, int _dx, int _dy, int _id)
    {
        double radians = System.Math.Atan2(_dy - _y, _dx - _x);
        int degrees = (int)(radians * 180 / System.Math.PI);
        float curAngle = t2.rotation.eulerAngles.y;
        curAngle = -curAngle;
        t2.Rotate(new Vector3(0, curAngle - degrees, 0));
        if (_id > 0) // == 0 means this is called for projectile // > 0 means we are classifying model : Unit : Archer
        {
            allScripts[_id].healthBarLookAtCamera();
            //allModels[_id].SendMessage("healthBarLookAtCamera", SendMessageOptions.DontRequireReceiver);
        }

    }
    
    #endregion preB

    #region Queuable Synaptics    

    public int spawnFunction(int _pid, int[] ddeck, int _sx, int _sy , int doNow )
    {
        // S P A W N
        if (doNow == 'Q')
        {
            if (cBuffer.Equals(String.Empty))
            {
                cBuffer = ZCode.pad(ZCode.GPG_STIME+ZCode.futureFalcon, 3) + ZCode.pad(_pid, 1);//TTTP
            }
            cBuffer += ZCode.pad(8,1) + ZCode.pad(ddeck[0],1) + ZCode.pad(ddeck[1],1) + ZCode.pad(ddeck[2],1) + ZCode.pad(ddeck[3],1)  + ZCode.pad( _sx , 2) + ZCode.pad( _sy , 2) + ZCode.pad(2,1);// CDDDDXXYYB

            int myCost = 0;
            int myUnitsInDeck = 0;
            for (int jui = 0; jui < ddeck.Length; jui++)
            {
                myCost += (ddeck[jui] * ZCode.ZUnitCosts[jui]);
                myUnitsInDeck += ddeck[jui];
            }            
            if ((XStatics.NOP >= 2) || (XStatics.NOP <= 1 && _pid == 1))
            {
                ZCode.ZCOINS -= myCost;
                COINS -= myCost;
                ZCode.ZAlive += myUnitsInDeck;
            }            

            return -1 ;
        }                

        int FirstID = -1;
        int[] PriorityOfUnits = new int[] { 0, 2, 1, 3 };// Sentinal, Archers inside , Pikes , Cavalry
        for (int aftPriority = 1; aftPriority < ddeck.Length; aftPriority++)
        {
            int aft = PriorityOfUnits[aftPriority];
            if (ddeck[aft] > 0)
            {
                int dir = 0;
                int i = 0;
                int j = 0;
                int ci = i, cj = j, di = i, dj = j;
                while (cj <= 20 && ddeck[aft] > 0)// if one limiter is out of bounds so are all limiters
                {
                    if (dir == 0)// right
                    {
                        j++;
                        if (j > cj)
                        {
                            dir = 1;
                            cj = j;
                        }
                    }
                    else if (dir == 1)// down
                    {
                        i++;
                        if (i > ci)
                        {
                            dir = 2;
                            ci = i;
                        }
                    }
                    else if (dir == 2)// left
                    {
                        j--;
                        if (j < dj)
                        {
                            dir = 3;
                            dj = j;
                        }
                    }
                    else // up
                    {
                        i--;
                        if (i < di)
                        {
                            dir = 0;
                            di = i;
                        }
                    }

                    int _qx = _sx + i;
                    int _qy = _sy + j;
                    if (_qx < 0 || _qx >= groundLength || _qy < 0 || _qy >= groundWidth)
                    {
                        continue; // Out Of Bounds
                    }
                    else
                    {
                        if (grid[_qx, _qy, 0] == 0)
                        {
                            grid[_qx, _qy, 0] = Test_generateid(_pid);//_pid
                            Test_addFighter(grid[_qx, _qy, 0], aft, _pid, _qx, _qy);
                            if (FirstID < 0)
                            {
                                FirstID = grid[_qx, _qy, 0];
                            }
                            allUnits[grid[_qx, _qy, 0]].gid = FirstID;
                            // Coin value decrementing moved to top
                            ddeck[aft]--;
                        }
                    }
                }
            }
        }
        if (_pid == PID)
        {
            //sfx.PlayOneClip( sfx.ResultFolder[3] , 0.2f );//SpawnMusic
        }
        return FirstID;// for Ergo deploy and attack
    }

    public void cMove(int _pidCurrent, int _targGID, int targX, int targY, int isAttack , char doNow )
    {
        // M O V E                                
        if (doNow == 'Q')
        {            
            if (cBuffer.Equals(String.Empty))
            {                                
                cBuffer = ZCode.pad(ZCode.GPG_STIME+ZCode.futureFalcon,3) + ZCode.pad(_pidCurrent,1);//TTTP
            }
            cBuffer += ZCode.pad(9,1) + ZCode.pad(_targGID, 4) + ZCode.pad(targX, 2) + ZCode.pad(targY, 2) + ZCode.pad(isAttack, 1);// IGGGGXXYYA
            return;
        }
        

        foreach (KeyValuePair<int, Fighter> entry in allUnits)
        {
            if (entry.Value.gid == _targGID)
            {
                entry.Value.ex = targX;
                entry.Value.ey = targY;
                entry.Value.mode = 1; // This should make the ReGroup Miracle
                // UPPER UPPER REVOLUTIONARY 3 hour bug idi 
                entry.Value.is_relocated = true;
                if (entry.Value.wid % 2 == 0)// For melee it is always move code
                {
                    entry.Value.is_GivenAtkOrdr = isAttack == 1 ? true : false;//AdvncdC#code
                }

            }
        }        
        if ( (!ZCode.isSync) && _pidCurrent == PID)
        {
            sfx.PlayLongClip(0, sfx.DrumsFolder[0], 0.3f);
        }

    }

    #endregion

    #region Valuable Variables

    public int gtime;                                // Game Time
    public int fps;                                  // tmp Frame Number
    public int maxFps;                               // Max FPS
    public int fastFactor;                           // Ex: x 2 times the original speed
    public SortedDictionary<int, Fighter> allUnits;  // Holds ( id , Fighter ) Relationship
    public int groundWidth;                          // Battle Ground Width
    public int groundLength;                         // Battle Ground Length
    public int[,,] grid;                             // 3D Maxtrix ...(x,y,0) = id at x,y and (x,y,1) = gtime at x,y
    public SortedDictionary<int, Arrow> allArrows;   // Holds ( arrowID , arrowObject) Relationship
    public int[] playerUC;                           // Player Unit Counts  
    public int arrowFrameDelay;                      // update frequency of arrow objects
    public float distBtwTroops2;                     // passed form ZCode to maintain same distValue

    #endregion

    #region Testing Data

    public int first_time_print = 0;

    public String board;

    public int UnixTime()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (int)(DateTime.UtcNow - epochStart).Ticks * 100;
    }

    public int Test_generateid2(int _pid)
    {
        System.Random r = new System.Random(UnixTime());
        int tmp_id;
        tmp_id = (_pid * 1000) + r.Next(111, 999);
        while (allUnits.ContainsKey(tmp_id))
        {
            tmp_id = (_pid * 1000) + r.Next(111, 999);
        }
        return tmp_id;
    }

    public int Test_generateid(int _pid)
    {
        int tmp_id;
        int ucount;
        int ulimit = 900;

        ucount = playerUC[_pid];
        tmp_id = (_pid * 1000) + (ucount);

        while (allUnits.ContainsKey(tmp_id))
        {
            ucount += 1;
            ucount %= ulimit;
            tmp_id = (_pid * 1000) + (ucount);
        }

        playerUC[_pid] = (ucount % ulimit);
        return tmp_id;
    }

    public void Test_addFighter(int _id, int _wid, int _pid, int _x, int _y)
    {
        Fighter obj;

        obj = new Fighter(_id, 0, _wid, _pid, _x, _y, 0);
        allUnits.Add(_id, obj);

        ZCode.ZTotalUnitCount[_pid, _wid]++;//Every Spawn is Counted Here
        // SPAWN    
        if (!ZCode.isSync)
        {
            int model_index = _wid;
            if (_wid > 0)
            {
                allModels.Add(_id, GameObject.Instantiate(_models[model_index], new Vector3(-99f, -99f, -99f), Quaternion.identity) as Transform);
                allScripts.Add(_id, allModels[_id].gameObject.GetComponent<JLife>());
                allScripts[_id].setInitialColor(_pid);                
            }
            else
            {//Spawn Tower
                allModels.Add(_id, GameObject.Instantiate(_models[model_index], new Vector3(_x * distBtwTroops2, 0f, _y * distBtwTroops2), Quaternion.identity) as Transform);
                allModels[_id].Find("cage").GetComponent<Renderer>().material.color = castleColors[_pid];
                ZCode.limitSpawnRings[_pid] = allModels[_id].Find("AuraAreaRing").gameObject;
                ZCode.limitTeamRings[_pid] = allModels[_id].Find("InnerRing").gameObject;
            }
        }
    }

    public void Test_populategrid(String _board)
    {
        for (int i = 0; i < groundLength; i++)
        {
            for (int j = 0; j < groundWidth; j++)
            {
                char c = _board[(i * groundWidth) + j];
                switch (c)
                {
                    case 'p':
                        grid[i, j, 0] = Test_generateid(1);//_pid
                        Test_addFighter(grid[i, j, 0], 1, 1, i, j);// _id , _wid = 1 pike , 2 arch , 3 cav , _pid 
                        grid[i, j, 1] = gtime;
                        break;
                    case 'a':
                        grid[i, j, 0] = Test_generateid(1);//_pid
                        Test_addFighter(grid[i, j, 0], 2, 1, i, j);// _id , _wid = 1 pike , 2 arch , 3 cav , _pid 
                        grid[i, j, 1] = gtime;
                        break;
                    case 'c':
                        grid[i, j, 0] = Test_generateid(1);//_pid
                        Test_addFighter(grid[i, j, 0], 3, 1, i, j);// _id , _wid = 1 pike , 2 arch , 3 cav , _pid 
                        grid[i, j, 1] = gtime;
                        break;

                    case 'P':
                        grid[i, j, 0] = Test_generateid(2);//_pid
                        Test_addFighter(grid[i, j, 0], 1, 2, i, j);// _id , _wid = 1 pike , 2 arch , 3 cav , _pid 
                        grid[i, j, 1] = gtime;
                        break;
                    case 'A':
                        grid[i, j, 0] = Test_generateid(2);//_pid
                        Test_addFighter(grid[i, j, 0], 2, 2, i, j);// _id , _wid = 1 pike , 2 arch , 3 cav , _pid 
                        grid[i, j, 1] = gtime;
                        break;
                    case 'C':
                        grid[i, j, 0] = Test_generateid(2);//_pid
                        Test_addFighter(grid[i, j, 0], 3, 2, i, j);// _id , _wid = 1 pike , 2 arch , 3 cav , _pid 
                        grid[i, j, 1] = gtime;
                        break;

                    case '1':
                        grid[i, j, 0] = Test_generateid(1);//_pid
                        Test_addFighter(grid[i, j, 0], 0, 1, i, j);// _id , _wid = 1 pike , 2 arch , 3 cav , _pid 
                        grid[i, j, 1] = gtime;
                        break;
                    case '2':
                        grid[i, j, 0] = Test_generateid(2);//_pid
                        Test_addFighter(grid[i, j, 0], 0, 2, i, j);// _id , _wid = 1 pike , 2 arch , 3 cav , _pid 
                        grid[i, j, 1] = gtime;
                        break;
                    case '3':
                        grid[i, j, 0] = Test_generateid(3);//_pid
                        Test_addFighter(grid[i, j, 0], 0, 3, i, j);// _id , _wid = 1 pike , 2 arch , 3 cav , _pid 
                        grid[i, j, 1] = gtime;
                        break;
                    case '4':
                        grid[i, j, 0] = Test_generateid(4);//_pid
                        Test_addFighter(grid[i, j, 0], 0, 4, i, j);// _id , _wid = 1 pike , 2 arch , 3 cav , _pid 
                        grid[i, j, 1] = gtime;
                        break;


                    default:
                        grid[i, j, 0] = 0;
                        grid[1, j, 1] = 0;
                        break;
                }
            }
        }
    }

    #endregion

    #region Begin Constructors

    public Color[] castleColors;
    public int lastClicked2;
    public int[,] ArtOfWar;
    public SSound sfx;

    private void TeamRings( int _pid )
    {
        if ( _pid % 2 == 1 )// Team 1 
        {
            ZCode.limitTeamRings[1].SetActive(true);
            if (XStatics.NOP > 2) { ZCode.limitTeamRings[3].SetActive(true); }
        }
        else
        {
            ZCode.limitTeamRings[2].SetActive(true);
            if (XStatics.NOP > 2) { ZCode.limitTeamRings[4].SetActive(true); }
        }
    }

    public Begin(ZCode _client, float _distBtwTroops2, Color[] _castleColors)
    {
        sfx = ZCode.MusicPlayer;        
        ZCode.limitSpawnRings = new GameObject[5];
        ZCode.limitTeamRings = new GameObject[5];
        ArtOfWar = new int[,]
        {//   T  P  A  C
            { 1, 1, 1, 1, 1, 1, 1},
     /* P */{ 1, 1, 2, 3, 0, 0, 0},
     /* A */{ 1, 2, 1, 1, 0, 0, 0},
     /* C */{ 1, 1, 3, 1, 0, 0, 0},
            { 1, 0, 0, 0, 0, 0, 0},
            { 1, 0, 0, 0, 0, 0, 0},
            { 1, 0, 0, 0, 0, 0, 0}
        };

        NOP = ZCode.ZNOP;
        PID = ZCode.ZPID;
        TID = ZCode.ZTID;
        COINS = ZCode.ZCOINS;

        baseScript = _client;
        distBtwTroops2 = _distBtwTroops2;
        castleColors = _castleColors;
        _models = _client.Models;
        arrowModel = _client.arrowModelBase;
        allModels = new SortedDictionary<int, UnityEngine.Transform>();
        allScripts = new SortedDictionary<int, JLife>();
        allProjectileModels = new SortedDictionary<int, UnityEngine.Transform>();
        _client.gameObject.SendMessage("chooseSelectionColor", PID, SendMessageOptions.DontRequireReceiver);
        lastClicked2 = 0;

        gtime = 0;
        fps = 0;
        maxFps = 40;
        fastFactor = 8;

        #if UNITY_EDITOR
        fastFactor = 8;// This is must for animation to sync in mobile
        #endif

        arrowFrameDelay = 10;
        // fastFactor is designed now to be 8 Per Sec

        allUnits = new SortedDictionary<int, Fighter>();
        groundWidth =  40;
        groundLength = 41;        
        grid = new int[groundLength, groundWidth, 2];
        allArrows = new SortedDictionary<int, Arrow>();
        playerUC = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };// Slot 4v4 + Padding

        AckArray = new int[(ZCode.ZMINUTES * 60) + 10];
        TimeQueue = new string[(ZCode.ZMINUTES*60)+10,5];// 300 x 4 Natural Number indexing - Offset for future falcon
        isCommandExecuted = new bool[(ZCode.ZMINUTES * 60) + 10, 5];
        EpochSync = new int[(ZCode.ZMINUTES * 60) + 1, 5];
        //TimeQueue[20, 3] = "0203" + "8032104207" ;
        //TimeQueue[20, 4] = "0204" + "8012323200";

        // Here we must add our capital
        // Also add enemy capitals as Static Units
        /*        
        board =
        ".............................." + // Player 1 
        ".......aaaa..................." +
        ".......aaaa..................." +
        ".......aaaa..................." +
        "....1....................3...." +
        ".........cccc................." +
        ".........cccc................." +
        ".........cccc....pppp........." +
        ".................pppp........." + // OLD BOARD
        ".................pppp........." +
        ".............................." +
        ".............................." +
        ".............................." +
        ".............................." +
        ".............................." +
        ".............................." +
        ".............................." + // 17th
        ".............................." +
        ".............................." +
        ".............................." +
        "..........PPPP................" +
        "..........PPPP................" +
        "..........PPPP................" +
        ".............................." + // 24
        "......AAAA........CCCC........" +
        "......AAAA........CCCC........" +
        "....2.AAAA........CCCC...4...." +
        ".............................." +
        ".............................." +
        ".............................." +        
        ".............................." // Player 2 
        ;
        */
        board =
        "........................................" + // Player 1 
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" + // OLD BOARD
        "........................................" +
        "..........1..................3.........." +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" + // 17th
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" + // 24
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" + // 30
        "..........2..................4.........." +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" +
        "........................................" // Player 2 
        ;
        if (ZCode.ZNOP < 4)
        {
            board =
            "........................................" + // Player 1 
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" + // OLD BOARD
            "........................................" +
            "..........1............................." +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" + // 17th
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" + // 24
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" + // 30
            "..........2............................." +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" +
            "........................................" // Player 2 
            ;
        }
        if (XStatics.NOP <= 1)// triggerAI
        {
            // I will do it in Update Cycle
        }

        Bgtime = -99;
        BTLgs = new int[] { 1, 1};
        BplayerUC = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        Bgrid = new int[ groundLength , groundWidth , 2];
        BallUnits = new SortedDictionary<int, Fighter>();        

        Bgtime2 = -99;
        BTLgs2 = new int[] { 1, 1 };
        BplayerUC2 = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        Bgrid2 = new int[groundLength, groundWidth, 2];
        BallUnits2 = new SortedDictionary<int, Fighter>();        

        gtime = 1;
        ZCode.GPG_GTIME = gtime;
        ZCode.GPG_STIME = gtime;
        Test_populategrid(board);
        TeamRings( PID );
    }

    public bool AddGameMoves( string cTmpBuffer )
    {
        int tTmpBuffertime = ZCode.rpad(cTmpBuffer, 0, 2);
        int tTmpBufferpid = ZCode.rpad(cTmpBuffer, 3, 3);
        string chkPrevCommands = TimeQueue[ tTmpBuffertime , tTmpBufferpid ];
        if ( String.IsNullOrEmpty(chkPrevCommands) )
        {
            TimeQueue[ tTmpBuffertime , tTmpBufferpid ] = cTmpBuffer + string.Empty;
            //AckArray[ tTmpBuffertime ] += 1;            
            if ( tTmpBufferpid == PID )
            {
                AckArray[tTmpBuffertime] += 1;
                ZCode.GPG_MsgsSlf++;
            }
            else
            {
                ZCode.GPG_MsgsRec++;
            }
            return true;
        }
        else// We already have a move 
        {            
            return false;
        }        
    }

    #endregion

    public void UpdateCycle()// This is the Master Update
    {        
        if (!ZCode.isSync)
        {            

            #region Calc 1 

            ZCode.ZAlive = 0;// we will count them to put constraint on max units
            ZCode.ZAIUnits = 0;
            foreach (KeyValuePair<int, Fighter> entry in allUnits)
            {
                if (XStatics.NOP <= 1 && entry.Value.pid == 1)
                {
                    if (ZCode.triggerAttack)
                    {
                        if (entry.Value.wid % 2 != 0)
                        {
                            //Logger(entry.Value.id + " mode:" + entry.Value.mode + "ex:" +entry.Value.ex + "ey:" +entry.Value.ey );                        
                            if (!entry.Value.is_marching || entry.Value.mode == 0 || (entry.Value.mode == 1 && entry.Value.is_suddenHalt))
                            {
                                entry.Value.ex = ZCode.TowerX[2];
                                entry.Value.ey = ZCode.TowerY[2];
                                entry.Value.is_marching = true;
                                //entry.Value.is_relocated = true;                        
                            }

                        }
                    }
                    else// Stay where you are for all units
                    {
                        if (entry.Value.is_marching)
                        {
                            entry.Value.ex = -1;
                            entry.Value.ey = -1;
                            entry.Value.eid = -1;
                            entry.Value.egid = -1;
                            entry.Value.mode = 0;
                            entry.Value.is_marching = false;
                        }
                    }
                }
                if (entry.Value.mode == 1 && (!entry.Value.is_suddenHalt) && entry.Value.wid > 0)
                {                    
                    allScripts[entry.Value.id].movementFunction();                                        
                }
                if (entry.Value.wid > 0)
                {
                    if (entry.Value.pid == PID)
                    {
                        ZCode.ZAlive++;
                    }
                    if (entry.Value.pid == 2)//AI
                    {
                        ZCode.ZAIUnits++;
                    }
                }
            }

            #endregion

            #region Calc 2 

            if (fps == 2)
            {
                foreach (KeyValuePair<int, Fighter> entry in allUnits)
                {
                    if (entry.Value.wid == 0)
                    {
                        ZCode.TowerH[entry.Value.pid] = entry.Value.health;
                        ZCode.TowerX[entry.Value.pid] = entry.Value.x;
                        ZCode.TowerY[entry.Value.pid] = entry.Value.y;
                    }
                }
            }

            #endregion

            #region Calc 3 
            
            if ((fps - 1) % ((maxFps * 8) / (fastFactor * arrowFrameDelay * 2)) == 0)// Testing 4
            {
                // 0 1 2 = 7 11 15 pattern ...Arithmetic progression with diff = arrowFrameDelay                        
                foreach (KeyValuePair<int, Arrow> arw in allArrows)
                {
                    int aid = arw.Key;
                    if (arw.Value.step == 0)
                    {
                        allProjectileModels[aid].position = new Vector3(allArrows[aid].ax * distBtwTroops2, arw.Value.height, allArrows[aid].ay * distBtwTroops2);
                        aimTheArrow(allProjectileModels[aid], arw.Value.ax, arw.Value.ay, arw.Value.atX, arw.Value.atY, 0);
                        aimTheArrow(allModels[aid], arw.Value.ax, arw.Value.ay, arw.Value.atX, arw.Value.atY, arw.Value.id);
                        allProjectileModels[aid].Rotate(new Vector3(0f, 0f, 45f));//default up angle
                        arw.Value.step++;
                        //allProjectileModels[aid].Find("Cube").GetComponent<SkinnedMeshRenderer>().material.color = castleColors[allUnits[aid].pid];
                    }
                    else if ((!arw.Value.is_useful) && allProjectileModels[aid].position.y < 1)// make it 1 at convinience
                    {
                        arw.Value.is_useful = true;// arrow is atop of some unit
                        allProjectileModels[aid].position += new Vector3(0f, -999f, 0f);//hit                    
                        if (allUnits.ContainsKey(aid) && allUnits[aid].wid > 0)
                        {
                            int vid;
                            //vid = grid[arw.Value.atX, arw.Value.atY, 0];//Victim ID   
                            vid = arw.Value.neID;
                            if (vid > 0 && allUnits.ContainsKey(vid))
                            {
                                if (allUnits[vid].wid > 0)// towers dont have health bars
                                {
                                    //allUnits[vid].hitCount[arw.Value.atype]++;                                
                                    //int[] tmpHitCountArray = allUnits[vid].hitCount;
                                    int thevinens2 = allUnits[vid].thealth;
                                    thevinens2 -= (allArrows[aid].dmg * ArtOfWar[allArrows[aid].atype, allUnits[vid].wid]);
                                    allUnits[vid].thealth = thevinens2;
                                    allUnits[vid].is_takingArrowDamage = true;
                                    allScripts[vid].showHealthBar(new int[] { thevinens2, allUnits[vid].maxHealth });
                                    if ((allUnits[vid].mode != 1) && thevinens2 < 1)//stable unit
                                    {
                                        allScripts[vid].Die(0);
                                    }
                                }
                            }
                        }
                        continue;
                    }
                    else
                    {
                        if (!arw.Value.is_useful)
                        {
                            float climb = arw.Value.climb;
                            float fwd = arw.Value.fwd;//MEMORY
                            if (arw.Value.releaseTime > 0)
                            {
                                arw.Value.releaseTime--;
                            }
                            else
                            {

                                if (arw.Value.step == 3)
                                {
                                    float ArwVolume = 0.4f;//default                        
                                    if (arw.Value.apid == PID)
                                    {
                                        ArwVolume = 0.7f;
                                    }
                                    sfx.RandomizeSfx(sfx.ArrowFolder, ArwVolume);
                                }
                                int releaseDelay = 2;
                                if (arw.Value.step > releaseDelay - 2 + (arw.Value.maxStep / 2))
                                {
                                    climb = -climb;
                                }
                                if (arw.Value.step > releaseDelay)
                                {
                                    allProjectileModels[aid].position += new Vector3(0f, climb, 0f);
                                    allProjectileModels[aid].Rotate(new Vector3(0f, 0f, -140f / (arrowFrameDelay * 2)));//default up angle                        
                                    allProjectileModels[aid].Translate(new Vector3(fwd, 0f, 0f));
                                }
                                arw.Value.step++;
                            }
                        }
                    }
                }
            }
            
            #endregion        
        }

        if ( ZCode.isSync || fps == ((maxFps * 8) / fastFactor)) // Equivalent to 1 second in gametime
        {
            L2D.LogLine(1, "Opening Second Equivalent at gtime:" + gtime + " isSync:" + ZCode.isSync );

            #region Rain of Arrows            

            int L4 = ZCode.ZNOP <= 1 ? 3 : ZCode.ZNOP + 1;

            for (int i43 = 0; i43 < L4; i43++)
            {
                for (int j43 = 0; j43 < ZCode.ZTUNITS + 1; j43++)
                {
                    ZCode.ZCurrentUnitCount[i43, j43] = 0;
                }
            }

            if (!ZCode.isSync)
            {
                ZCode.SpillScript.incSpriteTime();//To time the sprites

                lastClicked2++;

                if (lastClicked2 > 10)
                {
                    baseScript.hideDestRing();
                    selectedGid = -1;
                    ZCode.ZSelectedGID = -1;
                }

                if (!ZCode.isAudioAllowed)
                {
                    sfx.HaltMoreAudioSources();//For stopping all efx immediately                
                }
            }

            List<int> deadArrows = new List<int>();// BIG TIME CHECK THIS AGAIN

            if (gtime % 2 == 0)//June 5th MRNG AFTN BUG
            {
                foreach (KeyValuePair<int, Arrow> arw in allArrows)//Rain of Arrows Here
                {
                    int ahead = arw.Value.neID;
                    if (allUnits.ContainsKey(ahead))
                    {
                        allUnits[ahead].health -= (arw.Value.dmg * ArtOfWar[arw.Value.atype, allUnits[ahead].wid]);
                    }
                    deadArrows.Add(arw.Key);
                }
            }

            foreach (int deadAID in deadArrows)
            {
                allArrows.Remove(deadAID);
                if (!ZCode.isSync)
                {                    
                    GameObject.Destroy(allProjectileModels[deadAID].gameObject);
                    allProjectileModels.Remove(deadAID);                    
                }
            }            

            for (int zhi = 0; zhi < ZCode.ZHealths.Length; zhi++)
            {
                ZCode.ZHealths[zhi] = 0;
                ZCode.ZMaxHealths[zhi] = 0;
            }

            #endregion            

            #region ReRun And BackUp Points

            if (XStatics.NOP >= 2 && (!ZCode.isSync) && gtime % 2 == 0)
            {
                int WorstHit = -1;
                for (int chkExec = ZCode.timeForLastMemory ; (!ZCode.isSync) && chkExec > 0; chkExec--)// i have put timeforlastmem-1 fail safe
                {
                    for (int PJ = 1; PJ < XStatics.NOP; PJ++)
                    {
                        // && PJ != PID -- i think we are missing our own commands so set the loop back
                        if (((gtime - chkExec) > 0) && (!String.IsNullOrEmpty(TimeQueue[(gtime - chkExec), PJ])) && (!isCommandExecuted[(gtime - chkExec), PJ]))
                        {
                            ZCode.isSync = true;
                            WorstHit = gtime - chkExec;//Where a command is not executed
                            break;
                        }
                    }
                }
                if (ZCode.isSync)// We have some leftOut commands here 
                {
                    int decision = 0;
                    if (Bgtime > Bgtime2)// More Near
                    {
                        if (Bgtime <= WorstHit)
                        {
                            decision = 1;
                        }
                        else
                        {
                            decision = 2;
                        }
                    }
                    else
                    {
                        if (Bgtime2 <= WorstHit)
                        {
                            decision = 2;
                        }
                        else
                        {
                            decision = 1;
                        }
                    }
                    List<int> UnitsBeforeSync = new List<int>();
                    foreach (int key in allUnits.Keys)
                    {
                        UnitsBeforeSync.Add(key);
                    }
                    //ZCode.informationOn12 += "Synced:" + ( gtime - WorstHit ) + " gs\n";
                    L2D.LogLine(1, "Syncing Back " + (gtime - WorstHit) + "gs" );                    
                    Restore(decision);                                        
                    while (gtime <= ZCode.GPG_GTIME)                                 // OSCAR 
                    {
                        UpdateCycle();
                    }                 
                    // the moment we are out of while loop we are like at the end of this method
                    // to continue forward we need to put a rain of arrows here exclusively once
                    // master minded killer : run till odd gtime to avoid rain of arrows ... shabash ra killa
                    // Therefore the loop must end in a odd number gtime
                       
                    foreach (int key in UnitsBeforeSync)                          // Destroy Non-Persistent Units
                    {
                        if (!allUnits.ContainsKey(key))
                        {
                            GameObject.Destroy(allModels[key].gameObject);
                            allModels.Remove(key);
                            allScripts.Remove(key);
                        }
                    }
                    foreach (KeyValuePair<int, Fighter> entry in allUnits)          // Adding New Units
                    {
                        if (!UnitsBeforeSync.Contains(entry.Key))
                        {
                            int _id = entry.Value.id;
                            int _pid = entry.Value.pid;
                            int _wid = entry.Value.wid;
                            int _x = entry.Value.x;
                            int _y = entry.Value.y;
                            int model_index = _wid;
                            if (_wid > 0)
                            {
                                allModels.Add(_id, GameObject.Instantiate(_models[model_index], new Vector3(-99f, -99f, -99f), Quaternion.identity) as Transform);
                                allScripts.Add(_id, allModels[_id].gameObject.GetComponent<JLife>());
                                allScripts[_id].setInitialColor(_pid);
                            }
                            else
                            {//Spawn Tower
                                allModels.Add(_id, GameObject.Instantiate(_models[model_index], new Vector3(_x * distBtwTroops2, 0f, _y * distBtwTroops2), Quaternion.identity) as Transform);
                                allModels[_id].Find("cage").GetComponent<Renderer>().material.color = castleColors[_pid];
                                ZCode.limitSpawnRings[_pid] = allModels[_id].Find("AuraAreaRing").gameObject;
                                ZCode.limitTeamRings[_pid] = allModels[_id].Find("InnerRing").gameObject;
                                if ( _pid == PID) { ZCode.isUnitsPanelVisible = true; }
                            }

                            /*
                            if ( false )// Should be same as UPDATE ONE
                            {
                                int[] _packet = new int[] {//COMMUNICATOR
                                entry.Value.wid,
                                entry.Value.pid,
                                entry.Value.tid,
                                entry.Value.mode,    //3
                                entry.Value.x,
                                entry.Value.y,
                                entry.Value.nx,
                                entry.Value.ny,
                                1,
                                entry.Value.gid,// This is for 1 second coloring
                                entry.Value.is_takingArrowDamage?1:0
                                };
                                // CRITICAL SYNC UPDATE ONE                                                 
                                allScripts[entry.Value.id].ModeUpdate(_packet);
                                allScripts[entry.Value.id].showHealthBar(new int[] { 0, 0 });
                            }
                            */
                        }
                    }                    
                    ZCode.isSync = false;
                }
                if ( XStatics.NOP == 2 )
                {
                    if ( ZCode.TowerH[1] > 0 && ZCode.TowerH[2] < 0 )
                    {
                        if (PID%2==1) { ZCode.gameDecision = 1; }
                        else { ZCode.gameDecision = -1; }
                        ZCode.isGameOver = true;                        
                    }
                    if ( ZCode.TowerH[1] < 0 && ZCode.TowerH[2] > 0 )
                    {
                        if (PID%2==0) { ZCode.gameDecision = 1; }
                        else { ZCode.gameDecision = -1;  }
                        ZCode.isGameOver = true;
                    }
                }
                else
                {
                    if ( (ZCode.TowerH[1] > 0 || ZCode.TowerH[3] > 0) && ( ZCode.TowerH[2] < 0 && ZCode.TowerH[4] < 0 ) )
                    {
                        if (PID % 2 == 1) { ZCode.gameDecision = 1; }
                        else { ZCode.gameDecision = -1; }
                        ZCode.isGameOver = true;
                    }
                    if ((ZCode.TowerH[2] > 0 || ZCode.TowerH[4] > 0) && (ZCode.TowerH[1] < 0 && ZCode.TowerH[3] < 0))
                    {
                        if (PID % 2 == 0) { ZCode.gameDecision = 1; }
                        else { ZCode.gameDecision = -1; }
                        ZCode.isGameOver = true;
                    }
                }                
                if ( ZCode.isGameOver )
                {
                    ZCode.isUnitsPanelVisible = false;
                }                
            }

            if (XStatics.NOP >= 2 && gtime % (ZCode.timeForLastMemory / 2) == 2) // 2 , 8 , 14 , 20 , 26 , 32....
            {
                Backup();
            }

            #endregion

            #region TQueue                   

            if (!ZCode.isSync)
            {
                ZCode.GPG_STIME = gtime+1;// Any UI moves after this point will be scheduled to 
            }

            if (!ZCode.isSync)
            {
                if (!String.IsNullOrEmpty(cBuffer))
                {
                    AddGameMoves(cBuffer);
                    cBuffer = string.Empty;
                }

                if (XStatics.isMultiPlayer)
                {
                    ZCode.EpsilonFunction();
                    string lastNMoves = string.Empty;                    
                    for (int tLMi = gtime + 1; tLMi > 0 && tLMi > gtime - (ZCode.timeForLastMemory + 1); tLMi--)
                    {
                        if (AckArray[tLMi] >= 1 && AckArray[tLMi] < XStatics.NOP)
                        {
                            lastNMoves += TimeQueue[tLMi, PID] + "$";
                        }
                    }
                    if ( !string.IsNullOrEmpty(lastNMoves) )
                    {
                        KID.SendGameMessage(lastNMoves);
                    }                    
                }
            }
            
            for (int i = 1; i < 5; i++)
            {
                int lastDeployedGID = -1;
                string guestCommand = TimeQueue[gtime, i];
                if (!String.IsNullOrEmpty(guestCommand))
                {
                    if (i == PID)
                    {
                        ZCode.GPG_MsgsSExe++;
                    }
                    else
                    {
                        ZCode.GPG_MsgsRExe++;
                    }
                    isCommandExecuted[gtime, i] = true;                    
                    int nLen = (guestCommand.Length - 4) / 10; // 4 , +10 , +10 , +10.....
                    for (int j = 0; j < nLen; j++)
                    {
                        int si = (j * 10) + 4;
                        if (ZCode.rpad(guestCommand, si, si) == 8)
                        {
                            //si+1->5->DDDDXXYYB
                            int[] deck2 = new int[ZCode.ZTUNITS + 1];
                            deck2[0] = 0;
                            deck2[1] = ZCode.rpad(guestCommand, si + 2, si + 2);
                            deck2[2] = ZCode.rpad(guestCommand, si + 3, si + 3);
                            deck2[3] = ZCode.rpad(guestCommand, si + 4, si + 4);
                            int sX2 = ZCode.rpad(guestCommand, si + 5, si + 6);
                            int sY2 = ZCode.rpad(guestCommand, si + 7, si + 8);
                            int sGID = spawnFunction(i, deck2, sX2, sY2, 'Y');
                            lastDeployedGID = sGID;
                            if (i == PID)
                            {
                                selectedGid = sGID;
                                ZCode.ZSelectedGID = sGID;
                            }
                        }
                        else
                        {
                            //si+1->5->GGGGXXYYA
                            int targGID2 = ZCode.rpad(guestCommand, si + 1, si + 4);
                            int targX2 = ZCode.rpad(guestCommand, si + 5, si + 6);
                            int targY2 = ZCode.rpad(guestCommand, si + 7, si + 8);
                            int isAttack2 = ZCode.rpad(guestCommand, si + 9, si + 9);
                            if (targGID2 < 0) { targGID2 = lastDeployedGID; }
                            cMove(i, targGID2, targX2, targY2, isAttack2, 'Y');
                        }
                    }
                }
            }


            #endregion

            #region Cleaning Dead 

            List<int> deadUnits = new List<int>();
            foreach (KeyValuePair<int, Fighter> entry in allUnits)
            {
                if (entry.Value.wid == 0)//For Zhealths and MaxHealths for HUD
                {
                    if (entry.Value.health > 1)
                    {
                        ZCode.ZHealths[entry.Value.pid] = entry.Value.health;
                        ZCode.ZMaxHealths[entry.Value.pid] = entry.Value.maxHealth;
                    }
                    else
                    {
                        ZCode.TowerH[entry.Value.pid] = -1;//Dead Tower
                    }
                }
                if (entry.Value.health < 1)
                {
                    if (entry.Value.pid == PID)
                    {
                        ZCode.GPG_UL++;
                    }
                    deadUnits.Add(entry.Value.id); // Registering Casualities = Dead Units                                        
                    if (entry.Value.wid == 0 && XStatics.NOP < 2 && entry.Value.pid == PID)//?
                    {
                        ZCode.isGameOver = true;
                        ZCode.isUnitsPanelVisible = false;
                    }
                    if (entry.Value.wid == 0 && XStatics.NOP < 2 && entry.Value.pid != PID)
                    {
                        ZCode.isAIdefeated = true;
                        ZCode.isUnitsPanelVisible = false;
                    }
                }
                else
                {
                    ZCode.ZCurrentUnitCount[entry.Value.pid, entry.Value.wid]++;
                    if (ZCode.ZMaxUnitCount[entry.Value.pid, entry.Value.wid] < ZCode.ZCurrentUnitCount[entry.Value.pid, entry.Value.wid])
                    {
                        ZCode.ZMaxUnitCount[entry.Value.pid, entry.Value.wid] = ZCode.ZCurrentUnitCount[entry.Value.pid, entry.Value.wid];
                    }
                    entry.Value.is_processed = false;
                    entry.Value.thealth = entry.Value.health;
                    grid[entry.Value.x, entry.Value.y, 1] = gtime + 1;
                    if (entry.Value.wid > 0)
                    {
                        if (entry.Value.pledgeTime > 0)
                        {
                            entry.Value.pledgeTime--;
                            entry.Value.is_processed = true;
                        }
                        if (!ZCode.isSync)
                        {
                            int[] _packet = new int[] {//COMMUNICATOR
                                entry.Value.wid,
                                entry.Value.pid,
                                entry.Value.tid,
                                entry.Value.mode,    //3
                                entry.Value.x,
                                entry.Value.y,
                                entry.Value.nx,
                                entry.Value.ny,
                                1,
                                entry.Value.gid,// This is for 1 second coloring
                                entry.Value.is_takingArrowDamage?1:0
                            };
                            // CRITICAL UPDATE 1                                                 
                            allScripts[entry.Value.id].ModeUpdate(_packet);
                            allScripts[entry.Value.id].showHealthBar(new int[] { 0, 0 });
                        }
                        entry.Value.is_takingArrowDamage = false;
                    }
                    else
                    {
                        entry.Value.is_processed = true;
                    }
                }
            }

            foreach (int deadUnitID in deadUnits)
            {
                grid[allUnits[deadUnitID].x, allUnits[deadUnitID].y, 0] = 0;
                grid[allUnits[deadUnitID].x, allUnits[deadUnitID].y, 1] = 0;// Freeing grid space

                if (!ZCode.isSync)
                {
                    if (allUnits[deadUnitID].wid > 0)
                    {
                        allScripts[deadUnitID].Die(1);
                    }
                    if (allUnits[deadUnitID].wid == 0)
                    {
                        GameObject.Destroy(allModels[deadUnitID].gameObject);
                        //May be we should add explosion sprite
                    }
                    allModels.Remove(deadUnitID);
                    allScripts.Remove(deadUnitID);
                }

                allUnits.Remove(deadUnitID);
            }

            #endregion

            #region Unlock Achievements

            if (!ZCode.isSync && gtime%5==0 && XStatics.isLoggedIn )// Mavayya told to remove continuous signin asking
            {
                if ( ZCode.ZMaxUnitCount[PID,1] >= 20 )
                {
                    KID.UnlockAchievement("PikeMaster");
                }
                if (ZCode.ZMaxUnitCount[PID, 2] >= 20)
                {
                    KID.UnlockAchievement("BowMaster");
                }
                if (ZCode.ZMaxUnitCount[PID, 3] >= 20)
                {
                    KID.UnlockAchievement("SilverKnight");
                }
                if (ZCode.ZMaxUnitCount[PID, 3] >= 29)
                {
                    KID.UnlockAchievement("GoldenKnight");
                }
            }

            #endregion

            #region Handy Centroids

            // Make movement             
            Dictionary<int, int> counts = new Dictionary<int, int>();  // _gid , number of units belonging to gid
            Dictionary<int, int> centroids_X = new Dictionary<int, int>(); // _gid , _centeroid_XCOR
            Dictionary<int, int> centroids_Y = new Dictionary<int, int>(); // _gid , _centeroid_YCOR 
            Dictionary<int, int> centroids_Z = new Dictionary<int, int>(); // _gid , _centeroid_ZCOR 
            Dictionary<int, int> IdleCounts = new Dictionary<int, int>();  // _gid , number of units belonging to gid
            Dictionary<int, int> ArchCounts = new Dictionary<int, int>();  // _gid , number of units belonging to gid
            int tgid;
            foreach (KeyValuePair<int, Fighter> entry in allUnits)
            {
                tgid = entry.Value.gid;
                if (counts.ContainsKey(tgid))
                {
                    centroids_X[tgid] += entry.Value.x;
                    centroids_Y[tgid] += entry.Value.y;
                    //centroids_Z[ tgid ] = ((centroids_Z[ tgid ]*counts[tgid]) + entry.Value.z )/ (counts[tgid]+1);
                    counts[tgid] += 1;
                }
                else
                {
                    centroids_X.Add(tgid, entry.Value.x);
                    centroids_Y.Add(tgid, entry.Value.y);
                    //centroids_Z.Add( tgid , entry.Value.z ) ;
                    counts.Add(tgid, 1);
                }
                if (entry.Value.mode == 0 || (entry.Value.mode == 1 && entry.Value.is_suddenHalt))
                {
                    if (IdleCounts.ContainsKey(tgid))
                    {
                        IdleCounts[tgid] = IdleCounts[tgid] + 1;
                    }
                    else
                    {
                        IdleCounts.Add(tgid, 1);
                    }
                }
                else
                {
                    if (!IdleCounts.ContainsKey(tgid))
                    {
                        IdleCounts.Add(tgid, 0);
                    }
                }
                if (entry.Value.wid == 2 && entry.Value.pid == 1)//Counting Player Archers
                {
                    if (ArchCounts.ContainsKey(tgid))
                    {
                        ArchCounts[tgid] = ArchCounts[tgid] + 1;
                    }
                    else
                    {
                        ArchCounts.Add(tgid, 1);
                    }
                }
                else
                {
                    if (!ArchCounts.ContainsKey(tgid))
                    {
                        ArchCounts.Add(tgid, 0);
                    }
                }
            }

            foreach (KeyValuePair<int, int> ent in counts)
            {
                int _gid = ent.Key;
                centroids_X[_gid] = centroids_X[_gid] / counts[_gid]; // average
                centroids_Y[_gid] = centroids_Y[_gid] / counts[_gid];
            }

            int currentArrowCount = allArrows.Count;
            int up = 1;// Units Processed , Sentinel to Start = 1 , Later on up = 0 , up++ on processing a unit

            #endregion            

            while (up > 0)// No up means no further possible moves
            {
                #region Genie Init

                List<int>[] Genie = new List<int>[25];
                for (int j = Genie.Length - 1; j >= 0; j--)
                {
                    Genie[j] = new List<int>(); // Initializing the Genie For Strategic Movement
                }

                #endregion

                foreach (KeyValuePair<int, Fighter> entry in allUnits) // Unit Processing
                {
                    #region Regroup and Mode Decision

                    if (entry.Value.is_processed)
                    {
                        continue; // Skip the processed units - Tower Skip
                    }                    
                    
                    // Regroup
                    int cinnamonX = centroids_X[entry.Value.gid];
                    int cinnamonY = (centroids_Y[entry.Value.gid] + entry.Value.y) / 2;//This may keep line formations and awesome if works                                                            
                    if ((!entry.Value.is_marching) && (IdleCounts[entry.Value.gid] == counts[entry.Value.gid]))
                    {
                        int sdist2 = Int32.MaxValue;
                        bool is_homie = false;
                        int cdist2;
                        int sr2x = 0;
                        int sr2y = 0;
                        int u2x = entry.Value.x;
                        int u2y = entry.Value.y;
                        int pdist2 = calculateDistanceSquare(u2x, u2y, cinnamonX, cinnamonY);
                        int d2x, d2y;      // I D L E - R E L O C A T I O N                  
                        int[] duX = new int[] { -1, 0, 1, -1, 1, -1, 0, 1 };
                        int[] duY = new int[] { 1, 1, 1, 0, 0, -1, -1, -1 };
                        for (int isac = 0; isac < duX.Length; isac++)
                        {
                            d2x = u2x + duX[isac];
                            d2y = u2y + duY[isac];
                            if (d2x < 0 || d2x >= groundLength || d2y < 0 || d2y >= groundWidth)
                            {
                                continue;// Out of Bounds - for centering
                            }
                            if (grid[d2x, d2y, 0] == 0)//Empty
                            {
                                cdist2 = calculateDistanceSquare(d2x, d2y, cinnamonX, cinnamonY);
                                if (cdist2 < pdist2 && cdist2 < sdist2)
                                {
                                    sdist2 = cdist2;
                                    sr2x = d2x;
                                    sr2y = d2y;
                                    is_homie = true;
                                }
                            }
                        }
                        if (is_homie)
                        {
                            entry.Value.ex = sr2x;
                            entry.Value.ey = sr2y;
                        }
                    }

                    if (entry.Value.is_relocated)
                    {
                        int rtx = (centroids_X[entry.Value.gid] - entry.Value.x);
                        int rty = (centroids_Y[entry.Value.gid] - entry.Value.y);
                        if (calculateDistanceSquare(entry.Value.x, entry.Value.y, entry.Value.ex - rtx, entry.Value.ey - rty) <
                             calculateDistanceSquare(entry.Value.x, entry.Value.y, centroids_X[entry.Value.gid], centroids_Y[entry.Value.gid]))
                        {
                            entry.Value.ex -= rtx;
                            entry.Value.ey -= rty;
                        }
                        if (entry.Value.ex < 0)
                        {
                            entry.Value.ex = 0;
                        }
                        if (entry.Value.ex >= groundLength)
                        {
                            entry.Value.ex = groundLength - 1;
                        }
                        if (entry.Value.ey < 0)
                        {
                            entry.Value.ey = 0;
                        }
                        if (entry.Value.ey >= groundWidth)
                        {
                            entry.Value.ey = groundWidth - 1;
                        }
                        entry.Value.is_relocated = false;
                    }

                    //int k234 = ((int)Math.Sqrt(calculateDistanceSquare(entry.Value.x, entry.Value.y, centroids_X[entry.Value.gid], centroids_Y[entry.Value.gid])));
                    //if ( k234 > 6 )


                    if (entry.Value.eid > -1)
                    {
                        entry.Value.mode = 2; //attack animation triggered
                        if (allUnits.ContainsKey(entry.Value.eid))
                        {
                            entry.Value.ex = allUnits[entry.Value.eid].x;
                            entry.Value.ey = allUnits[entry.Value.eid].y;
                            entry.Value.ez = allUnits[entry.Value.eid].z;
                        }
                        else
                        {
                            entry.Value.eid = -1; // REVOLUTIONARY @9may 11:41
                            entry.Value.mode = 0;
                        }
                    }
                    else if (entry.Value.ex > -1 && entry.Value.ey > -1)
                    {
                        // unit is assigned to move to ex , ey , ez                        
                        if (entry.Value.ex == entry.Value.x && entry.Value.ey == entry.Value.y)
                        {
                            entry.Value.mode = 0;// Reached -- REVLOUTIONARY @9may 11:56
                        }
                        else
                        {
                            entry.Value.mode = 1;
                        }
                    }
                    else if (entry.Value.egid > -1)
                    {
                        if (centroids_X.ContainsKey(entry.Value.egid))
                        {
                            entry.Value.mode = 1; // walk to ex , ey , ez 
                            entry.Value.ex = centroids_X[entry.Value.egid] - (centroids_X[entry.Value.gid] - entry.Value.x);
                            entry.Value.ey = centroids_Y[entry.Value.egid] - (centroids_Y[entry.Value.gid] - entry.Value.y);
                            if (entry.Value.ex < 0)
                            {
                                entry.Value.ex = 0;
                            }
                            if (entry.Value.ex >= groundLength)
                            {
                                entry.Value.ex = groundLength - 1;
                            }
                            if (entry.Value.ey < 0)
                            {
                                entry.Value.ey = 0;
                            }
                            if (entry.Value.ey >= groundWidth)
                            {
                                entry.Value.ey = groundWidth - 1;
                            }
                        }
                        else
                        {
                            entry.Value.egid = -1; // REVOLUTIONARY @9may 11:56
                            entry.Value.mode = 0; // Enemy GID Killed
                        }
                    }
                    else
                    {
                        entry.Value.mode = 0;// scount or idle mode
                    }



                    if ((entry.Value.mode == 0 || entry.Value.is_suddenHalt)
                        && (entry.Value.wid % 2 != 0 || (entry.Value.wid % 2 == 0 && (!allArrows.ContainsKey(entry.Value.id)) && currentArrowCount == 0))
                        // 26May2016 the archer sync update
                        // To make sure we are not scouting if arrow is released
                        )// In Case Regrouping changes their mode to 1 
                    {
                        int ax, ay; // Archer X , Y
                        int wx, wy; // Watch X , Y
                        ax = entry.Value.x;
                        ay = entry.Value.y;
                        int prange = entry.Value.range; // Generally 12 
                        int nrange = -prange; // -12                         

                        // Advanced Scout * * * * * * * * * * * * * * * * * *                     
                        int dir = 0;
                        int i = 0;
                        int j = 0;
                        bool phase_stop = false;
                        int nearest_enemy = -1;
                        int nearest_distance = int.MaxValue;
                        int ci = i, cj = j, di = i, dj = j;
                        while (cj <= prange)// if one limiter is out of bounds so are all limiters
                        {
                            if (dir == 0)// right
                            {
                                j++;
                                if (j > cj)
                                {
                                    dir = 1;
                                    cj = j;
                                    if (phase_stop)
                                    {
                                        break;
                                    }
                                }
                            }
                            else if (dir == 1)// down
                            {
                                i++;
                                if (i > ci)
                                {
                                    dir = 2;
                                    ci = i;
                                    if (phase_stop)
                                    {
                                        break;
                                    }
                                }
                            }
                            else if (dir == 2)// left
                            {
                                j--;
                                if (j < dj)
                                {
                                    dir = 3;
                                    dj = j;
                                    if (phase_stop)
                                    {
                                        break;
                                    }
                                }
                            }
                            else // up
                            {
                                i--;
                                if (i < di)
                                {
                                    dir = 0;
                                    di = i;
                                    if (phase_stop)
                                    {
                                        break;
                                    }
                                }
                            }

                            wx = ax + i;
                            wy = ay + j;
                            if (wx >= 0 && wx < groundLength && wy >= 0 && wy < groundWidth)
                            {
                                if (grid[wx, wy, 1] >= gtime && allUnits.ContainsKey(grid[wx, wy, 0]) && allUnits[grid[wx, wy, 0]].tid != entry.Value.tid)
                                {
                                    int edist = calculateDistanceSquare(ax, ay, wx, wy);
                                    if (edist > 3 && edist < nearest_distance && allUnits[grid[wx, wy, 0]].thealth > 0)// Health condition to avoid over kill
                                    {
                                        // Edist > 3 assumes second ring and neglect first ring arrows and scouts
                                        nearest_distance = edist;
                                        nearest_enemy = grid[wx, wy, 0]; // enemy id                                         
                                        phase_stop = true; // because one enemy found atleast with some health                                        
                                    }
                                }
                            }

                        }

                        if (phase_stop)
                        {
                            if (entry.Value.wid % 2 == 0) // Ranged Units
                            {
                                if (!allArrows.ContainsKey(entry.Value.id) && gtime % 2 == 0)// Check if i already had an arrow registered on my id
                                {
                                    // new Arrow ( _id , _eid , _dmg )
                                    // PROJSEC = PROJECTILE SECTION (For Find)
                                    entry.Value.mode = 6;
                                    // Dynamic Arrow
                                    Arrow arw;
                                    bool reroute_decision = false;
                                    if (allUnits[nearest_enemy].mode == 1)
                                    {
                                        reroute_decision = true;
                                    }
                                    arw = new Arrow(entry.Value.wid, entry.Value.id, entry.Value.pid, 2 * entry.Value.attack, entry.Value.x, entry.Value.y, allUnits[nearest_enemy].x, allUnits[nearest_enemy].y, arrowFrameDelay, reroute_decision, nearest_enemy);// true for dynamic routing
                                    allArrows.Add(entry.Value.id, arw);                                                                        
                                    allUnits[nearest_enemy].thealth -= (2 * entry.Value.attack * ArtOfWar[entry.Value.wid, allUnits[nearest_enemy].wid]);
                                    if (!ZCode.isSync)
                                    {
                                        allProjectileModels.Add(entry.Value.id, GameObject.Instantiate(arrowModel, new Vector3(-99f, -99f, -99f), Quaternion.identity) as Transform);
                                    }
                                }
                            }
                            else
                            {
                                entry.Value.ex = allUnits[nearest_enemy].x; // Aggressive
                                entry.Value.ey = allUnits[nearest_enemy].y;
                                entry.Value.ez = allUnits[nearest_enemy].z;
                                Console.WriteLine("Aggressive Unit {2} onto:({0},{1})", allUnits[nearest_enemy].x, allUnits[nearest_enemy].y, entry.Value.id);
                            }
                        }
                        // End of Advanced Scout * * * * * * * * * * * * * * 


                    }

                    int free_squares = 0;
                    bool can_move = false;
                    int shortest_distance = int.MaxValue;
                    int current_distance = calculateDistanceSquare(entry.Value.x, entry.Value.y, entry.Value.ex, entry.Value.ey);// if ex|ey =-1 then implication is idle
                    int shortest_x = -1;
                    int shortest_y = -1;

                    #endregion

                    #region Meele Fights

                    // check for meele attack
                    int ux, uy;
                    int dx, dy;
                    ux = entry.Value.x;
                    uy = entry.Value.y;
                    dx = -1; dy = -1;//    N   S  NW  SE  NE  SW   W   E       Below are horse movables
                    int[] DX = new int[] { 1, -1, 1, -1, 1, -1, 0, 0, -1, 0, 1, -2, 2, -2, 2, -2, 2, -1, 0, 1 };
                    int[] DY = new int[] { 0, 0, 1, -1, -1, 1, 1, -1, 2, 2, 2, 1, 1, 0, 0, -1, -1, -2, -2, -2 };
                    //int[] DX = new int[] { -1, 0, 1, -1, 1, -1, 0, 1,   -1, 0, 1, -2, 2, -2, 2, -2, 2, -1, 0, 1 };
                    //int[] DY = new int[] { 1, 1, 1, 0, 0, -1, -1, -1,    2, 2, 2, 1, 1, 0, 0, -1, -1, -2, -2, -2 };


                    for (int i = 0; i < DX.Length; i++)
                    {
                        if (i >= 8 && entry.Value.wid != 3)
                        {
                            break; // >= 8 are cavalry co-ordinates
                        }

                        dx = ux + DX[i];
                        dy = uy + DY[i];
                        if (dx >= 0 && dx < groundLength && dy >= 0 && dy < groundWidth)
                        {
                            if (i < 8 && grid[dx, dy, 1] >= gtime)// Meele Attack
                            {
                                if (allUnits.ContainsKey(grid[dx, dy, 0]) && allUnits[grid[dx, dy, 0]].tid != entry.Value.tid)// Enemy
                                {
                                    // Marking enemy as enemy
                                    entry.Value.mode = 2;
                                    entry.Value.eid = grid[dx, dy, 0];
                                    can_move = false;
                                    break;
                                }
                                else // Friendly
                                {
                                    // Friendly Block ...Ask To Make Way...Version2.0..Not Now	
                                }
                            }
                            else if (grid[dx, dy, 1] < gtime) // Movement
                            {
                                if (entry.Value.mode == 1)
                                {
                                    int distance = calculateDistanceSquare(dx, dy, entry.Value.ex, entry.Value.ey);
                                    if (distance < current_distance)
                                    {
                                        free_squares++;
                                        if (distance < shortest_distance)
                                        {
                                            can_move = true;
                                            shortest_distance = distance;
                                            shortest_x = dx;
                                            shortest_y = dy;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Means i >= 8 and grid [ i , j , 1 ] >= gtime ...occupied far square
                            }
                        }
                    }

                    if (entry.Value.wid % 2 == 0 && entry.Value.is_GivenAtkOrdr)
                    {
                        int reachDistance = calculateDistanceSquare(entry.Value.x, entry.Value.y, entry.Value.ex, entry.Value.ey);
                        if (reachDistance < (entry.Value.range * entry.Value.range))
                        {
                            entry.Value.ex = entry.Value.x;
                            entry.Value.ey = entry.Value.y;// Making the destiny reached
                            entry.Value.is_GivenAtkOrdr = false;
                        }
                    }
                    entry.Value.should_move = can_move;
                    entry.Value.nx = shortest_x;
                    entry.Value.ny = shortest_y;
                    Genie[free_squares].Add(entry.Value.id);

                    #endregion
                }

                #region Unit Battles

                up = 0;
                for (int j = Genie.Length - 1; j >= 0; j--)
                {
                    foreach (int _id in Genie[j])
                    {
                        Fighter entryValue = allUnits[_id];

                        if (entryValue.mode == 0 || entryValue.mode == 6)
                        {
                            entryValue.is_processed = true;
                            up++;
                            if (!ZCode.isSync)
                            {
                                int[] _packet4 = new int[] {//COMMUNICATOR
                                        entryValue.wid,
                                        entryValue.pid,
                                        entryValue.tid,
                                        entryValue.mode,
                                        entryValue.x,
                                        entryValue.y,
                                        entryValue.nx,
                                        entryValue.ny,
                                        4
                                    };
                                // CRITICAL UPDATE 4                            
                                allScripts[entryValue.id].ModeUpdate(_packet4);
                            }
                            entryValue.is_suddenHalt = false;
                            entryValue.mode = 0;// To Reset Archers shooting mode to normal
                        }

                        // Movement 
                        if (entryValue.mode == 1)
                        {
                            if (entryValue.should_move)
                            {
                                if (grid[entryValue.nx, entryValue.ny, 1] >= gtime)
                                {
                                    // Room occupied by quicker unit ... Fail Safe to avoid over writing of grid positions
                                    entryValue.is_suddenHalt = true;
                                }
                                else
                                {
                                    entryValue.is_suddenHalt = false;
                                    bool should_print = false;
                                    //if ( entryValue.wid == 1 && entryValue.pid == 1 ){ 
                                    //should_print = true ;
                                    //}
                                    if (should_print)
                                    {
                                        Console.Write(" {2} moved from ({0},{1})", entryValue.x, entryValue.y, entryValue.id);
                                    }

                                    if (!ZCode.isSync)
                                    {
                                        int[] _packet2 = new int[] {//COMMUNICATOR
                                            entryValue.wid,
                                            entryValue.pid,
                                            entryValue.tid,
                                            entryValue.mode,
                                            entryValue.x,
                                            entryValue.y,
                                            entryValue.nx,
                                            entryValue.ny,
                                            2
                                        };
                                        // CRITICAL UPDATE 2                                     
                                        allScripts[entryValue.id].ModeUpdate(_packet2);
                                    }

                                    grid[entryValue.x, entryValue.y, 0] = 0;
                                    grid[entryValue.x, entryValue.y, 1] = 0; // Making Room For Other Units                                    

                                    entryValue.x = entryValue.nx;
                                    entryValue.y = entryValue.ny;	// New Square - Moved 

                                    grid[entryValue.x, entryValue.y, 0] = entryValue.id;
                                    grid[entryValue.x, entryValue.y, 1] = gtime + 1; // Registering New Place in Grid

                                    entryValue.is_processed = true;
                                    up++;

                                    if (should_print)
                                    {
                                        Console.Write(" to ({0},{1})", entryValue.x, entryValue.y);
                                        Console.WriteLine(" Destiny to ({0},{1}) FreeSquares:{2}", entryValue.ex, entryValue.ey, j);
                                    }
                                }
                            }
                            else
                            {
                                // Should check again for Room
                                //entryValue.nx = entryValue.x;
                                //entryValue.ny = entryValue.y;                                                                
                                entryValue.is_suddenHalt = true;
                            }
                        }

                        // Attack
                        if (entryValue.mode == 2)
                        {
                            if (allUnits.ContainsKey(entryValue.eid)) // Checking if enemy is alive
                            {
                                allUnits[entryValue.eid].health -= (entryValue.attack * ArtOfWar[entryValue.wid, allUnits[entryValue.eid].wid]);

                                if (!ZCode.isSync)
                                {
                                    //SPILLING BLOOD
                                    if (allUnits[entryValue.eid].wid > 0)//Spill Blood only for Units
                                    {
                                        ZCode.SpillScript.deployBlood(allUnits[entryValue.eid].x, allUnits[entryValue.eid].y);
                                    }
                                    //sfx.RandomizeSfx(sfx.SwordFolder , 0.06f );
                                    //Console.WriteLine(entryValue.id + " attacked " + entryValue.eid);
                                    int ahead = entryValue.eid;
                                    if (allUnits[ahead].wid > 0)
                                    {
                                        //allModels[ahead].SendMessage("showHealthBar", new int[] { allUnits[ahead].health, allUnits[ahead].maxHealth }, SendMessageOptions.DontRequireReceiver);
                                        allScripts[ahead].showHealthBar(new int[] { allUnits[ahead].health, allUnits[ahead].maxHealth });
                                    }

                                    int[] _packet3 = new int[] {//COMMUNICATOR
                                        entryValue.wid,
                                        entryValue.pid,
                                        entryValue.tid,
                                        entryValue.mode,
                                        entryValue.x,
                                        entryValue.y,
                                        allUnits[entryValue.eid].x ,
                                        allUnits[entryValue.eid].y ,
                                        3
                                    };
                                    // CRITICAL UPDATE 3                                 
                                    allScripts[entryValue.id].ModeUpdate(_packet3);
                                }
                            }
                            else
                            {
                                entryValue.eid = -1; // Enemy Got Killed
                            }

                            if (!counts.ContainsKey(entryValue.egid))
                            {
                                entryValue.egid = -1; // Enemy Group is dead
                            }

                            entryValue.is_processed = true;
                            up++;
                        }
                    }
                }
                Console.WriteLine(up + " units processed in this loop");

                #endregion

            }

            #region Sound Plan

            if (!ZCode.isSync)
            {
                foreach (KeyValuePair<int, Fighter> entry in allUnits) // Unit Processing
                {
                    if (!entry.Value.is_processed)// these are in mode=1 but unable to move
                    {
                        allScripts[entry.Value.id].suddenModelHalt(1);
                    }
                }

                int maxModes = 10;
                int[,] audioHelpArray = new int[ZCode.ZTUNITS + 1, maxModes];//Assuming 10 modes at max
                foreach (KeyValuePair<int, Fighter> entry in allUnits)
                {
                    entry.Value.thealth = entry.Value.health;
                    if (entry.Value.pid == PID && (!entry.Value.is_suddenHalt))
                    {
                        audioHelpArray[entry.Value.wid, entry.Value.mode]++;
                    }
                }
                // cavalry march
                // soldier march
                // attack
                int mode2Count = 0;
                int cavalryMarch = 0;
                int soldierMarch = 0;
                for (int aut = 0; aut < ZCode.ZTUNITS + 1; aut++)
                {
                    for (int am = 0; am < maxModes; am++)
                    {
                        if (am == 2)
                        {
                            mode2Count += audioHelpArray[aut, am];
                        }
                        if (am == 1)
                        {
                            if (aut == 1 || aut == 2)
                            {
                                soldierMarch += audioHelpArray[aut, am];
                            }
                            if (aut == 3)
                            {
                                cavalryMarch += audioHelpArray[aut, am];
                            }
                        }
                    }
                }
                if (mode2Count > 2)
                {
                    sfx.PlayLongClip(2, sfx.AttackFolder[0], 0.5f);
                }
                if (cavalryMarch > 2)
                {
                    sfx.PlayLongClip(3, sfx.CavalryFolder[0], 0.5f);
                }
                if (soldierMarch > 2)
                {
                    sfx.PlayLongClip(4, sfx.MarchingFolder[0], 0.5f);
                }

                // Arrow Rerouting
                foreach (KeyValuePair<int, Arrow> ark in allArrows)
                {
                    if (ark.Value.should_reroute)
                    {
                        if (allUnits[ark.Value.neID].mode == 1 && (!allUnits[ark.Value.neID].is_suddenHalt) && allUnits[ark.Value.neID].nx > -1 && allUnits[ark.Value.neID].ny > -1)
                        {
                            //Logger(ark.Value.atX + "," + ark.Value.atY +" -> "+ allUnits[ark.Value.neID].x +","+ allUnits[ark.Value.neID].y);
                            ark.Value.atX = allUnits[ark.Value.neID].x;
                            ark.Value.atY = allUnits[ark.Value.neID].y;
                            string info2 = ark.Value.RouteArrow();
                            //Logger( info2 ) ;// This will recalculate all distances
                            //Logger(String.Format("{0}({1},{2}) neID:{3} Grid:{4}", ark.Key, ark.Value.atX, ark.Value.atY, ark.Value.neID, grid[ark.Value.atX, ark.Value.atY, 0]));
                        }
                        //else is_suddenHalt situations i assume - should check if arrows are not rerouting                    
                        ark.Value.should_reroute = false;
                    }
                }
            }
            #endregion

            #region Treasury and Hack Check

            if (!ZCode.isSync)
            {
                if (!ZCode.isGameOver)
                {
                    float adjuster = 1.1f;// for tight coin value 
                    COINS += ((int)adjuster);// The Coin Generation Logic - HACKER PROOF
                    ZCode.ZAIGEMS += ((int)adjuster);// These are used by AI                    
                }
                ZCode.ZCOINS = COINS;
            }

            #endregion

            #region The AI MODULE

            if ( XStatics.NOP <= 1 && !ZCode.isAIdefeated )// switching off AI while debugging
            {
                int _weakF = -1;
                int WeakX = -1;
                int WeakY = -1;
                foreach (KeyValuePair<int, int> _tgid in ArchCounts)
                {
                    if ((ArchCounts[_tgid.Key] > (counts[_tgid.Key] - 1)) && (ArchCounts[_tgid.Key] > _weakF))//P1 Archer Band
                    {
                        WeakX = centroids_X[_tgid.Key];
                        WeakY = centroids_Y[_tgid.Key];
                    }
                }
                int AImaxUnit = XStatics.xArmyCap;
                if (ZCode.ZAIUnits < AImaxUnit)
                {
                    int maxUCO = 0;
                    int favUnit = 1;
                    int[] zRomansUnitCount = new int[ZCode.ZDECK.Length];
                    foreach (KeyValuePair<int, Fighter> entry in allUnits)
                    {
                        if (entry.Value.pid == 1)// Romans
                        {
                            zRomansUnitCount[entry.Value.wid]++;
                        }
                    }
                    for (int ai2 = 1; ai2 < zRomansUnitCount.Length; ai2++)
                    {
                        int cmco = (ZCode.ZDECK[ai2] + zRomansUnitCount[ai2]);
                        if (cmco > maxUCO)
                        {
                            maxUCO = cmco;
                            favUnit = ai2;
                        }
                    }
                    int oppUnit = 0;
                    bool _isDefensive = false;
                    // Force to deploy pikes
                    if (favUnit == 1) { oppUnit = 2; }//2 Pike so using Archers
                    if (favUnit == 2) { oppUnit = 3; }//3 Archers so using Cav
                    if (favUnit == 3) { oppUnit = 1; }//1 Cav so using Pikes
                    if (ZCode.ZAlive == 0)
                    {
                        oppUnit = 1; // choosing pikes and shot when player is awaiting
                    }
                    int aiSpwanPointX = (int)UnityEngine.Random.Range(24, 26);
                    int aiSpwanPointY = (int)UnityEngine.Random.Range(6, 12);
                    int aiMoveX = -1;
                    int aiMoveY = -1;
                    if (ZCode.ZAlive > ZCode.ZAIUnits)
                    {
                        oppUnit = 1;
                        _isDefensive = true;
                        if (_isDefensive)
                        {
                            if (WeakX > -1 && WeakY > -1)
                            {
                                //Logger("Tactics Against Arch:(" + WeakX + "," + WeakY + ")");
                                aiMoveX = WeakX;
                                aiMoveY = WeakY;
                                if (WeakX < groundLength / 2)
                                {
                                    WeakX = (groundLength / 2);
                                }
                                if (WeakY > groundWidth / 2)
                                {
                                    WeakY = (groundWidth / 2);
                                }
                                aiSpwanPointX = WeakX;
                                aiSpwanPointY = WeakY;
                                //check if aispawnPoint Y is free for attack
                                bool isTrap = false;
                                for (int chkLine = aiSpwanPointX; chkLine > aiMoveX; chkLine--)
                                {
                                    int middleman = grid[chkLine, aiSpwanPointY, 0];
                                    if (middleman > 0 && allUnits[middleman].tid != TID && allUnits[middleman].wid % 2 != 0)
                                    {
                                        isTrap = true;
                                        break;
                                    }
                                }
                                if (!isTrap)// is surrounded checking
                                {
                                    int[] dCapX = new int[] { -1, -1, -1, 0, 0, 1, 1, 1 };
                                    int[] dCapY = new int[] { -1, 0, 1, -1, 1, -1, 0, 1 };
                                    for (int i = 0; i < 8; i++)
                                    {
                                        int ddx = aiSpwanPointX + dCapX[i];
                                        int ddy = aiSpwanPointY + dCapY[i];
                                        if (!(ddx < 0 || ddy < 0 || ddx >= groundLength || ddy >= groundWidth))
                                        {
                                            int sideman = grid[ddx, ddy, 0];
                                            if (sideman > 0 && allUnits[sideman].tid != TID && allUnits[sideman].wid % 2 != 0)
                                            {
                                                isTrap = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (isTrap)
                                {
                                    oppUnit = 1;
                                    aiSpwanPointX = 28;
                                    aiSpwanPointY = 9;
                                }
                                else
                                {
                                    oppUnit = 3;
                                }
                            }
                            else
                            {
                                aiSpwanPointX = 28;
                                aiSpwanPointY = 9;
                            }
                        }
                    }
                    //Spwan Function
                    int[] oppDeck = new int[ZCode.ZDECK.Length];
                    if (oppUnit == 2 || oppUnit == 1)
                    {
                        oppDeck[oppUnit] = ZCode.ZAIGEMS / ZCode.ZUnitCosts[oppUnit];
                    }
                    else// for cav
                    {
                        oppDeck[oppUnit] = (ZCode.ZAIGEMS - (2 * ZCode.ZUnitCosts[2])) / ZCode.ZUnitCosts[oppUnit];
                    }
                    //Logger("oppUnit:"+oppUnit);
                    if ((ZCode.ZAIUnits + oppDeck[oppUnit]) > AImaxUnit)
                    {
                        oppDeck[oppUnit] = AImaxUnit - ZCode.ZAIUnits;
                    }
                    if (oppDeck[oppUnit] > 2 || _isDefensive)// Minimum 3 units
                    {
                        bool shoud_arch_spawn = oppUnit == 3 && (!_isDefensive) ? true : false;
                        int spawnedAIS = oppDeck[oppUnit];
                        int eGS = spawnFunction(2, oppDeck, aiSpwanPointX, aiSpwanPointY, 'I' );// For now i am using PID later on pid from request                        
                        if (aiMoveX > -1 && aiMoveY > -1)
                        {
                            //Logger("Made to atck"+eGS);
                            foreach (KeyValuePair<int, Fighter> entry in allUnits)
                            {
                                if (entry.Value.gid == eGS)
                                {
                                    entry.Value.ex = aiMoveX;
                                    entry.Value.ey = aiMoveY;
                                    entry.Value.is_AIMarched = true;
                                }
                            }
                        }

                        if (shoud_arch_spawn)
                        {
                            oppDeck[oppUnit] = 0;
                            oppDeck[2] = 2;
                            aiSpwanPointX = (int)UnityEngine.Random.Range(26, 28);
                            spawnFunction(2, oppDeck, aiSpwanPointX, aiSpwanPointY, 'I' );
                        }
                        ZCode.ZAIGEMS -= (spawnedAIS * ZCode.ZUnitCosts[oppUnit]);
                        if (shoud_arch_spawn)
                        {
                            ZCode.ZAIGEMS -= (2 * ZCode.ZUnitCosts[2]);
                        }
                    }
                }
                if (ZCode.ZAIUnits < ZCode.ZAlive)
                {
                    ZCode.isDrummedByAI = false;
                }
                // IDLE AI MANAGEMENT #sri archers attacking red cavalry 3rd July
                foreach (KeyValuePair<int, Fighter> entry in allUnits)
                {
                    if (entry.Value.pid == 2 && entry.Value.x < groundLength / 2 && (!entry.Value.is_marching) && // P2 IDLE UNITS IN P1's TOWN
                        (entry.Value.wid % 2 != 0 && (entry.Value.mode == 0 || (entry.Value.mode == 1 && entry.Value.is_suddenHalt))))
                    {
                        //Logger("Deploying _wid:"+entry.Value.wid+" at ("+entry.Value.x+","+entry.Value.y+") to ebase");
                        entry.Value.ex = 9;
                        entry.Value.ey = (int)UnityEngine.Random.Range(8, 10);
                        entry.Value.is_AIMarched = true;
                        entry.Value.is_marching = true;
                    }
                }
                // ......................
                if (ZCode.ZAIUnits > AImaxUnit / 2 && ZCode.ZAIUnits > ZCode.ZAlive)
                {// Advance Functions
                    if (!ZCode.isDrummedByAI)
                    {
                        sfx.PlayLongClip(1, sfx.EDrumsFolder[0], 0.5f);
                        ZCode.isDrummedByAI = true;
                    }
                    int dtry = (int)UnityEngine.Random.Range(1, 10);
                    foreach (KeyValuePair<int, Fighter> entry in allUnits)
                    {
                        if (entry.Value.pid == 2 && // Only P2
                            (!entry.Value.is_AIMarched || (entry.Value.is_AIMarched && entry.Value.wid % 2 != 0 && (entry.Value.mode == 0 || (entry.Value.mode == 1 && entry.Value.is_suddenHalt)))))// Make Enemy March
                        {
                            if (entry.Value.wid == 1 || entry.Value.wid == 3)
                            {
                                if (UnityEngine.Random.Range(1, 10) < 5)
                                {
                                    entry.Value.ex = 9;
                                    entry.Value.ey = (int)UnityEngine.Random.Range(8, 10);
                                }
                                else
                                {
                                    entry.Value.ex = 9;
                                    entry.Value.ey = (int)UnityEngine.Random.Range(7, 11);
                                }

                            }
                            else if (entry.Value.wid == 2)
                            {
                                entry.Value.ex = ZCode.TowerX[1];
                                entry.Value.ey = dtry;
                                entry.Value.is_GivenAtkOrdr = true;
                            }

                            entry.Value.is_marching = true;
                            entry.Value.is_AIMarched = true;
                        }
                    }
                }
            }            
            gtime++;                        
            if ( ! ZCode.isSync )
            {                
                ZCode.GPG_GTIME = gtime;
                L2D.LogLine(1, "Ending Second Equivalent at :" + gtime + " isSync:" + ZCode.isSync);
            }            
            ZCode.ZGTIME = gtime ;// this is center time display
            fps = 0;
            
            //Test.gridprint(grid, gtime, allUnits);// helper to be removed later            
            //Logger( gapD.TotalSeconds );

            #endregion
        }
        fps++;

    }

    #region Backup Variables

    public static int Bgtime;
    public static int[] BTLgs;
    public static int[] BplayerUC;
    public static int[,,] Bgrid;
    public static SortedDictionary<int, Fighter> BallUnits;
    
    public static int Bgtime2;
    public static int[] BTLgs2;
    public static int[] BplayerUC2;
    public static int[,,] Bgrid2;
    public static SortedDictionary<int, Fighter> BallUnits2;
    
    public void Backup()
    {
        if ( Bgtime >= gtime )
        {         
            return;
        }
        if ( ( (gtime - Bgtime) > ((ZCode.timeForLastMemory/2)+1) ) )// CHECK THIS STUFF
        {
            Bgtime = gtime;
            BTLgs[0] = ZCode.GPG_MsgsRExe;
            BTLgs[1] = ZCode.GPG_MsgsSExe;
            for (int i = 0; i < playerUC.Length; i++)
            {
                BplayerUC[i] = playerUC[i];
            }            
            for (int i = 0; i < groundLength; i++)
            {
                for (int j = 0; j < groundWidth; j++)
                {
                    Bgrid[i, j, 0] = grid[i, j, 0];
                    Bgrid[i, j, 1] = grid[i, j, 1];
                }
            }
            BallUnits.Clear();
            foreach (KeyValuePair<int, Fighter> entry in allUnits)
            {
                BallUnits.Add(entry.Key, (Fighter)entry.Value.Clone());
            }            
        }
        else
        {
            Bgtime2 = gtime;
            BTLgs2[0] = ZCode.GPG_MsgsRExe;
            BTLgs2[1] = ZCode.GPG_MsgsSExe;
            for (int i = 0; i < playerUC.Length; i++)
            {
                BplayerUC2[i] = playerUC[i];
            }            
            for (int i = 0; i < groundLength; i++)
            {
                for (int j = 0; j < groundWidth; j++)
                {
                    Bgrid2[i, j, 0] = grid[i, j, 0];
                    Bgrid2[i, j, 1] = grid[i, j, 1];
                }
            }
            BallUnits2.Clear();
            foreach (KeyValuePair<int, Fighter> entry in allUnits)
            {
                BallUnits2.Add(entry.Key, (Fighter)entry.Value.Clone());
            }            
        }
    }

    public void Restore( int _decision )
    {
        allArrows.Clear();// This is a certain bug that is debugged on 07-Sep-16
        allProjectileModels.Clear();// To Make Sure the arrow rain will be clean
        if ( _decision == 1 )
        {
            gtime = Bgtime;
            ZCode.GPG_MsgsRExe = BTLgs[0];
            ZCode.GPG_MsgsSExe = BTLgs[1];
            for (int i = 0; i < BplayerUC.Length; i++)
            {
                playerUC[i] = BplayerUC[i];
            }
            for (int i = 0; i < groundLength; i++)
            {
                for (int j = 0; j < groundWidth; j++)
                {
                    grid[i, j, 0] = Bgrid[i, j, 0];
                    grid[i, j, 1] = Bgrid[i, j, 1];
                }
            }
            allUnits.Clear();
            foreach (KeyValuePair<int, Fighter> entry in BallUnits)
            {
                allUnits.Add(entry.Key, (Fighter)entry.Value.Clone());
            }            
        }
        else
        {
            gtime = Bgtime2;
            ZCode.GPG_MsgsRExe = BTLgs2[0];
            ZCode.GPG_MsgsSExe = BTLgs2[1];
            for (int i = 0; i < BplayerUC2.Length; i++)
            {
                playerUC[i] = BplayerUC2[i];
            }
            for (int i = 0; i < groundLength; i++)
            {
                for (int j = 0; j < groundWidth; j++)
                {
                    grid[i, j, 0] = Bgrid2[i, j, 0];
                    grid[i, j, 1] = Bgrid2[i, j, 1];
                }
            }
            allUnits.Clear();
            foreach (KeyValuePair<int, Fighter> entry in BallUnits2)
            {
                allUnits.Add(entry.Key, (Fighter)entry.Value.Clone());
            }            
        }
    }

    #endregion

    #region Helper Methods

    public void Logger(object logtext)
    {
        //baseScript.SendMessage("logThisText", logtext.ToString() , SendMessageOptions.DontRequireReceiver);
        baseScript.logThisText(logtext.ToString());
    }

    public int calculateDistanceSquare(int x1, int y1, int x2, int y2)
    {
        return ((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1));
    }

    #endregion
}

//------------------ D A T A --- M O D E L S --------------------------------------

public class Fighter : ICloneable
{

    #region Fighter Details

    public int id;          // Unique ID
    public int gid;         // Group ID 
    public int wid;         // Weapon ID
    public int pid;         // Player ID
    public int tid;         // Team ID
    public int x;           // Fighters X Cor
    public int y;           // Fighters Y Cor
    public int z;           // Fighters Z Cor
    public int range;       // Fighters Sight Range
    public int health;      // Health or Hitpoints
    public int attack;      // Weapon Attack
    public int handattack;  // Hand Attack
    public int areaattack;  // Area Attack
    public int radius;      // Area Attack Radius
    public int speed;       // Movement Speed
    public int delay;       // Delay betwen attacks
    public int mode;        // Idle[0] , Walk[1] , Attack[2] , Hurt[3] , Die[4]
    public int angle;       // Fighters angle along SkyToEarth Axis
    public int maxHealth;   // maxHealth , diff from current health : health    

    #endregion

    #region Enemy Details

    public int eid;         // enemy whom the fighter is going to attack
    public int egid;        // enemy group which is fighter is going to attack
    public int ex;          // enemy X Cor
    public int ey;          // enemy Y Cor
    public int ez;          // enemy Z Cor

    #endregion

    #region Helper Variables

    public int hitCount;       // for doing arrow stuff
    public bool is_AIMarched;// suddenHalt
    public bool is_suddenHalt;// suddenHalt
    public bool is_relocated; // relocation
    public bool is_GivenAtkOrdr; // given attack order or move order
    public bool is_processed; // Whether processed
    public bool is_marching;  // For Atck SP Mode
    public bool is_takingArrowDamage;// Whether taking Arrow Hits
    public bool should_move;  // Can Unit Move
    public int nx;            // Next X Cor
    public int ny;            // Next Y Cor
    public int thealth;       // Sum up arrow dmg
    public int pledgeTime;    // hold unit before setting it into action

    #endregion

    #region Constructors    

    public Fighter(int _id, int _gid, int _wid, int _pid, int _x, int _y, int _z)
    {
        id = _id;
        gid = _gid;
        wid = _wid;
        pid = _pid;
        x = _x;
        y = _y;
        z = _z;
        mode = 0;
        angle = 0;
        giveWeapon(_wid);
        eid = -1;
        egid = -1;
        ex = -1;
        ey = -1;
        ez = -1;
        tid = 1; // By default Team1
        if (_pid % 2 == 0)
        {
            tid = 2; // All even pids to Team2
        }
        is_AIMarched = false;
        is_suddenHalt = false;
        is_relocated = false;
        is_GivenAtkOrdr = false;
        is_processed = false;
        is_marching = false;
        is_takingArrowDamage = false;
        should_move = false;
        nx = -1;
        ny = -1;
        thealth = health;
        hitCount = 0;
        pledgeTime = 0;
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    #endregion

    #region Managing Methods

    private void giveWeapon(int _wid)
    {
        Constants c = new Constants(_wid);
        range = c.getRange();
        health = c.getHealth();
        attack = c.getAttack();
        handattack = c.getHandattack();
        areaattack = c.getAreaattack();
        radius = c.getRadius();
        speed = c.getSpeed();
        delay = c.getDelay();
        maxHealth = c.getMaxHealth();
    }

    public override String ToString()
    {
        return String.Format(
            " ID : {0}" +
            " GID : {1}" +
            " WID : {2}" +
            " PID : {3}" +
            " + : {4}" +
            " ({5},{6})" +
            " ({8},{9})" +
            " EID : {11}" +
            " EGID : {12}" +
            " M : {13}" +
            " R : {14}" +

        "", id, gid, wid, pid, health, x, y, z, ex, ey, ez, eid, egid, mode, is_relocated ? 1 : 0);
    }

    #endregion

}


public class Constants
{
    #region Properties

    private int wid;
    private int range;
    private int health;
    private int attack;
    private int handattack;
    private int areaattack;
    private int radius;
    private int speed;
    private int delay;
    private int maxHealth;

    #endregion

    #region Constructors

    public Constants(int _wid)
    {
        wid = _wid;
        range = 0;
        health = 0;
        attack = 0;
        handattack = 0;
        areaattack = 0;
        radius = 0;
        speed = 0;
        delay = 0;
        maxHealth = 1;
        if (wid < 0)
        {
            return; // Err Log
            // THE NUMBER THEORY
        }
        else if (wid == 0) // Aura
        {
            range = 10;
            health = 7000;//12000
            attack = 40;
            handattack = attack;
            speed = 1;
            delay = 1;
        }
        else if (wid == 1) // PikeMen
        {
            range = 3;
            health = 490;// 0kill test
            attack = 45;
            handattack = attack;
            speed = 5;
            delay = 1;
        }
        else if (wid == 2) // Archers
        {
            range = 12;
            health = 260;
            attack = 19;
            handattack = attack;
            speed = 7;
            delay = 5;
        }
        else if (wid == 3) // Cavalry
        {
            range = 3;
            health = 900;//850
            attack = 80;
            handattack = attack;
            speed = 11;
            delay = 1;
        }
        else
        {
            return; // Err Log
        }

    }

    #endregion

    #region Getter Methods

    public int getRange()
    {
        return range;
    }

    public int getMaxHealth()
    {
        return health;
    }

    public int getHealth()
    {
        return health;
    }

    public int getAttack()
    {
        return attack;
    }

    public int getHandattack()
    {
        return handattack;
    }

    public int getAreaattack()
    {
        return areaattack;
    }

    public int getRadius()
    {
        return radius;
    }

    public int getSpeed()
    {
        return speed;
    }

    public int getDelay()
    {
        return delay;
    }

    #endregion

}


public class Arrow
{
    #region Properties

    public int id;              // Fighter id who fired Arrow    
    public int apid;             // For Sound
    public int dmg;             // Damage value when Arrow hits target    
    public int ax;              // Arrows X Cor
    public int ay;              // Arrows Y Cor   
    public int atX;             // Target X
    public int atY;             // Target Y
    public int angle;           // Arrows rotation along SkyToEarth Axis ( theta )
    public int upangle;         // Arrows rotation wrt XY Plane and Z ( Phi )    
    public int step;            // No.of.steps made by arrow
    public int maxStep;         // AP with d = arrowDelay 0 1 2 ..7 11 15
    public int releaseTime;     // Realistic random arrow release
    public int waitTime;        // die Time
    public float height;        // height of arrow
    public float climb;         // dClimb
    public float fwd;           // dFwd
    public float realdist;
    public bool is_useful;
    public bool should_reroute;
    public int ADelay;
    public int neID; // Nearest Enemy ID
    public int atype; // wid 2 for normal arch , 4 for crossbow...and 6 for Maze may be

    #endregion

    #region Constructors

    public Arrow(int _atype, int _id, int _apid, int _dmg, int _ax, int _ay, int _atX, int _atY, int adelay, bool _should_reroute, int _neID)
    {
        atype = _atype;
        id = _id;
        apid = _apid;
        dmg = _dmg;
        ax = _ax;
        ay = _ay;
        atX = _atX;
        atY = _atY;
        angle = 0;
        upangle = 0;
        step = 0;
        waitTime = 1;
        height = 2.6f;
        maxStep = 7 + (waitTime * adelay);
        releaseTime = (int)UnityEngine.Random.Range(1, (maxStep - 1));
        climb = 0.7f; // for archer...for crossbow make it small
        ADelay = adelay;
        is_useful = false;
        should_reroute = _should_reroute;
        neID = _neID;

        RouteArrow();
        // This must be in co-relation to the update first if - arrow Frame Delay         
        // need parabolic trajectory managing methods here     
    }    

    #endregion

    #region Managing Methods
    public string RouteArrow()
    {
        string info = "";
        info += "OLD realdist" + realdist;
        realdist = (float)Math.Sqrt(((atX - ax) * (atX - ax)) + ((atY - ay) * (atY - ay)));
        realdist *= 2.6f;
        climb = realdist / 50f;
        fwd = realdist / ADelay;
        info += " realdist" + realdist;
        return info;
    }
    #endregion

}







