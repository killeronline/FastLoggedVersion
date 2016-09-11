using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi.Multiplayer;
using System;
using System.IO;

public class XStatics {

    public static string socialName = "Anonymous";

    public static bool isLoggedIn = false;

    public static int NOP = 2;//This is for mplayer testing

    public static int MultiplayerConnections = 1;// Yourself    

    public static int xArmyCap = 30 ;// 60 when profiler

    public static bool isFullMute = false;

    public static bool isMultiPlayer = false;

    public static List<Participant> participantsList;

    public static KID KidInAction;

    public static string SceneName = "ABattle";

    public static string SceneNameHome = "AHomeScreen";

    public static string RoomSceneName = "ARoom";

    public static void invokeKidInAction()
    {
        KidInAction = new KID();
    }

}

public class KID : GooglePlayGames.BasicApi.Multiplayer.RealTimeMultiplayerListener
{
        
    public static string Achievement_Beginner = "CgkI3p_jiqkeEAIQCA";
    public static string Achievement_PikeMaster = "CgkI3p_jiqkeEAIQCQ";
    public static string Achievement_BowMaster = "CgkI3p_jiqkeEAIQCg";
    public static string Achievement_SilverKnight = "CgkI3p_jiqkeEAIQCw";
    public static string Achievement_GoldenKnight = "CgkI3p_jiqkeEAIQDA";
    public static string Achievement_TheKing = "CgkI3p_jiqkeEAIQDQ";
    public static string LBoardGlobal = "CgkI3p_jiqkeEAIQBw";
    public static bool isUnlockedAchievement_Beginner = false;
    public static bool isUnlockedAchievement_PikeMaster = false;
    public static bool isUnlockedAchievement_BowMaster = false;
    public static bool isUnlockedAchievement_SilverKnight = false;
    public static bool isUnlockedAchievement_GoldenKnight = false;
    public static bool isUnlockedAchievement_TheKing = false;
    public static string[] LocalParticipantIds = new string[5];

    public KID()
    {
        //.....retry login again and again and show error msg if not logged in 
    }
    
    public static void Logout()
    {
        PlayGamesPlatform.Instance.SignOut();
        if ( PlayGamesPlatform.Instance.IsAuthenticated() )
        {
            XStatics.isLoggedIn = true;
        }
        else
        {
            XStatics.isLoggedIn = false;
        }
    }

    public static bool chkLogin()
    {
        try
        {
            return PlayGamesPlatform.Instance.IsAuthenticated();
        }
        catch { return false; }
    }
    public static void Login()
    {
        try
        {
            // PRE-LOGIN
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()               
                .Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            //LOGIN
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("Login Success");
                    XStatics.isLoggedIn = true;                    
                    XStatics.socialName = KID.processPlayerName(Social.localUser.userName);                    
                }
                else
                {
                    Debug.Log("Login Failed");
                    XStatics.isLoggedIn = false;
                    XStatics.socialName = "Anonymous";
                }
            });
        }
        catch{;}
    }

    public static void ShowLBoards()
    {        
        try
        {
            if (!(PlayGamesPlatform.Instance.IsAuthenticated()))
            {
                Login();
            }
            PlayGamesPlatform.Instance.ShowLeaderboardUI(LBoardGlobal);// Triarii
        }
        catch{;}
        //Social.ShowLeaderboardUI();        // To Show All Leader Boards
    }

    public static void PostScoreForLBoardGlobal(int _score)
    {
        if (!XStatics.isLoggedIn) { KID.Login(); }
        try
        {
            if (!(PlayGamesPlatform.Instance.IsAuthenticated()))
            {
                Login();
            }
            Social.ReportScore(_score, LBoardGlobal, (bool success) =>
            {
                if (success)
                {
                    Debug.Log("Submitted Score : " + _score);
                }
                else
                {
                    Debug.Log("Failed Score : " + _score);
                }
            });
        }
        catch {; }

    }


    public static void ShowAchievementsUI()
    {        
        try
        {
            if (!(PlayGamesPlatform.Instance.IsAuthenticated()))
            {
                Login();
            }
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        catch {; }
    }

    public static void UnlockAchievement( string name )
    {
        try
        {
            if (!(PlayGamesPlatform.Instance.IsAuthenticated()))
            {
                Login();
            }
            if (name.Equals("Beginner"))// Currently we are not checking for bool success
            {
                if (!isUnlockedAchievement_Beginner)
                {
                    Social.ReportProgress(Achievement_Beginner, 100.0f, (bool success) => { });
                    isUnlockedAchievement_Beginner = true;
                }
            }
            else if (name.Equals("PikeMaster"))
            {
                if (!isUnlockedAchievement_PikeMaster)
                {
                    Social.ReportProgress(Achievement_PikeMaster, 100.0f, (bool success) => { });
                    isUnlockedAchievement_PikeMaster = true;
                }
            }
            else if (name.Equals("BowMaster"))
            {
                if (!isUnlockedAchievement_BowMaster)
                {
                    Social.ReportProgress(Achievement_BowMaster, 100.0f, (bool success) => { });
                    isUnlockedAchievement_BowMaster = true;
                }
            }
            else if (name.Equals("SilverKnight"))
            {
                if (!isUnlockedAchievement_SilverKnight)
                {
                    Social.ReportProgress(Achievement_SilverKnight, 100.0f, (bool success) => { });
                    isUnlockedAchievement_SilverKnight = true;
                }
            }
            else if (name.Equals("GoldenKnight"))
            {
                if (!isUnlockedAchievement_GoldenKnight)
                {
                    Social.ReportProgress(Achievement_GoldenKnight, 100.0f, (bool success) => { });
                    isUnlockedAchievement_GoldenKnight = true;
                }
            }
            else
            {
                // invalid name
            }
        }
        catch (Exception e)
        {
            L2D.LogLine(0, "Unlock Achievement Exception:" + e.Message);
        }
    }

    public static void IncrementAchievement( string name , int value )
    {
        try
        {
            if (!(PlayGamesPlatform.Instance.IsAuthenticated()))
            {
                Login();
            }
            if (name.Equals("TheKing"))// Currently we are not checking for bool success
            {
                if (!isUnlockedAchievement_TheKing)
                {
                    PlayGamesPlatform.Instance.IncrementAchievement(Achievement_TheKing, value, (bool success) => { });
                    isUnlockedAchievement_TheKing = true;
                }
            }            
            else
            {
                // invalid name
            }
        }
        catch (Exception e)
        {
            L2D.LogLine(0, "Increment Achievement Exception:" + e.Message);
        }
    }

    public static void ShowQuickGameUI( int _opponents )
    {        
        try
        {
            if (!(PlayGamesPlatform.Instance.IsAuthenticated()))
            {
                Login();
            }
            uint MaxOpponents = (uint)_opponents;
            uint MinOpponents = (uint)_opponents;            
            uint GameVariant = 0;
            PlayGamesPlatform.Instance.RealTime.CreateQuickGame(MinOpponents, MaxOpponents,
                        GameVariant, XStatics.KidInAction );
            L2D.LogLine(2, "Show Quick Game UI Succeeded.");
        }
        catch (Exception e)
        {
            L2D.LogLine(0, "Show Quick Game Exception:" + e.Message);
        }
    }

    public static void SendBroadcastChatMessage( int scode , string dataBroadcastMessage )
    {
        if (XStatics.isMultiPlayer)
        {
            try
            {
                dataBroadcastMessage = scode + dataBroadcastMessage;
                if (dataBroadcastMessage.Length < 1100)
                {
                    List<byte> dtSend = new List<byte>();
                    for (int i = 0; i < dataBroadcastMessage.Length; i++)
                    {
                        dtSend.Add((byte)dataBroadcastMessage[i]);
                    }
                    byte[] dataToBeSent = dtSend.ToArray();
                    PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, dataToBeSent);
                    L2D.LogLine(2, "Sent Broadcast Message:" + dataBroadcastMessage + ":" + dataToBeSent.ToString());
                }
                else
                {
                    ZCode.chatMsgList.Add(CbScript.formatChatMessage("Unable to send broadcast data", 1));
                    L2D.LogLine(1, "Unable to send broadcast message due to more size");
                }
            }
            catch (Exception e)
            {
                L2D.LogLine(0, "Broadcast Message Exception:" + e.Message);
            }
        }
    }

    public static void SendGameMessage( string gameMessage )
    {
        if (XStatics.isMultiPlayer)
        {
            try
            {                
                List<byte> dtSend = new List<byte>();
                if (gameMessage.Length < 1399)// Google Play Games Official Limit : Reliable 1400
                {
                    for (int i = 0; i < gameMessage.Length; i++)
                    {
                        dtSend.Add((byte)gameMessage[i]);
                    }
                    byte[] dataToBeSent = dtSend.ToArray();
                    // game message 
                    PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, dataToBeSent);
                    L2D.LogLine(2, "Sent Game Message:" + gameMessage + ":" + dataToBeSent.ToString());
                }
                else
                {
                    ZCode.chatMsgList.Add(CbScript.formatChatMessage("Unable to send game data",1));
                    L2D.LogLine(1, "Unable to send game message due to more size");
                }
            }
            catch (Exception e)
            {
                L2D.LogLine(0, "Game Message Exception:" + e.Message);
            }
        }
    }

    public static void StripRooms()
    {
        try
        {
            PlayGamesPlatform.Instance.RealTime.LeaveRoom();
            L2D.LogLine(2, "Stripped Room Call.");
        }
        catch(Exception e)
        {
            L2D.LogLine(0, "Stripped Room Call Exception:"+ e.Message);
        }
    }

    private bool showingWaitingRoom = false;
    void RealTimeMultiplayerListener.OnRoomSetupProgress(float percent)
    {
        // show the default waiting room.        
        if (!showingWaitingRoom)
        {
            showingWaitingRoom = true;
            PlayGamesPlatform.Instance.RealTime.ShowWaitingRoomUI();
        }
        
    }

    public static string processPlayerName( string prePlayerName )
    {
        if ( string.IsNullOrEmpty(prePlayerName.Trim()) )
        {
            return "PrivatePlayer";
        }
        else
        {
            return prePlayerName;
        }
        
    }

    void RealTimeMultiplayerListener.OnRoomConnected(bool success)
    {
        if ( success )
        {
            XStatics.participantsList = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
            List<Participant> participants = PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();            
            Participant myself = PlayGamesPlatform.Instance.RealTime.GetSelf();
            int pid = 0;
            ZCode.ZPLayerNames = new string[5];
            for ( int i = 0; i < 5; i++)
            {
                ZCode.ZPLayerNames[i] = "Anonymous";
            }
            foreach (Participant p in participants)
            {
                pid++;
                KID.LocalParticipantIds[pid] = p.ParticipantId;
                ZCode.ZPLayerNames[pid] = processPlayerName(p.DisplayName);
                if (myself.ParticipantId == p.ParticipantId)
                {
                    ZCode.ZPID = pid;
                }
            }
            XStatics.MultiplayerConnections = XStatics.NOP;
            SceneManager.LoadScene(XStatics.SceneName);
            L2D.LogLine(2, "Succeeded to create room.");
        }
        else
        {
            SceneManager.LoadScene(XStatics.SceneNameHome);
            L2D.LogLine(2, "Failed to create room.");
        }        
    }

    void RealTimeMultiplayerListener.OnLeftRoom()
    {
        ZCode.chatMsgList.Add(CbScript.formatChatMessage("You left the room.", 1));
        L2D.LogLine(2, "You left the room.");
    }

    void RealTimeMultiplayerListener.OnParticipantLeft(Participant participant)
    {
        L2D.LogLine(2, "Participant Left " + participant.DisplayName );
        return;
    }

    void RealTimeMultiplayerListener.OnPeersConnected(string[] participantIds)
    {
        string tmpPlayerNs = "";
        XStatics.MultiplayerConnections++;
        for ( int i = 0; i < participantIds.Length;i++)
        {
            Participant p = PlayGamesPlatform.Instance.RealTime.GetParticipant(participantIds[i]) ;
            ZCode.chatMsgList.Add( CbScript.formatChatMessage(processPlayerName(p.DisplayName)+" Joined.",1));
            tmpPlayerNs += p.DisplayName + ":";
        }        
        L2D.LogLine(2, "Peers Connected " + tmpPlayerNs + " Joined." );        
    }

    void RealTimeMultiplayerListener.OnPeersDisconnected(string[] participantIds)
    {
        try
        {            
            if (XStatics.MultiplayerConnections > 1)
            {
                string tmpPlayerNs = "";
                XStatics.MultiplayerConnections--;
                for (int i = 0; i < participantIds.Length; i++)
                {
                    Participant p = PlayGamesPlatform.Instance.RealTime.GetParticipant(participantIds[i]);
                    for (int j = 0; j < XStatics.participantsList.Count; j++)
                    {
                        if (participantIds[i].Equals(XStatics.participantsList[j].ParticipantId) && ZCode.GPG_PlayerLastSeen[j] >= 0)
                        {
                            ZCode.chatMsgList.Add(CbScript.formatChatMessage(processPlayerName(p.DisplayName) + " Left.", 1));
                            ZCode.GPG_PlayerLastSeen[j] = -2;//Left Code                        
                            tmpPlayerNs += p.DisplayName + ":";
                            break;
                        }
                    }
                }
                L2D.LogLine(2, "Peers Disconnected " + tmpPlayerNs + " Left.");
            }            
            else
            {
                L2D.LogLine(2, "Peers Disconnected ALL.");
            }
        }
        catch(Exception e){
            L2D.LogLine(0, "Peers Disconnected Exception " + e.Message );
        }       
    }

    public static void ProcessMoves( string Msg )
    {
        string[] lastNCommands = Msg.Split('$');
        for (int i = 0; i < lastNCommands.Length; i++)
        {
            if (lastNCommands[i] != string.Empty)
            {
                bool isAdded = ZCode.b.AddGameMoves(lastNCommands[i]);
                if (isAdded && XStatics.isMultiPlayer)
                {
                    KID.SendBroadcastChatMessage(13, lastNCommands[i].Substring(0, 4));// ACK Message
                    L2D.LogLine(2, "Sending Ack Message: with code:13 " + lastNCommands[i].Substring(0,4));
                }
                else
                {
                    L2D.LogLine(1, "Already Added [Check]: " + lastNCommands[i] );
                }
                //else - we have already received this move earlier                    
            }
        }
    }

    void RealTimeMultiplayerListener.OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        string Msg = string.Empty;
        for (int i = 0; i < data.Length; i++)
        {
            Msg += (char)data[i] + string.Empty ;
        }
        if ( isReliable )//Game Messages
        {
            L2D.LogLine(2, "Reliable Game Message:"+Msg);
            KID.ProcessMoves(Msg);            
        }
        else
        {
            string sms = Msg.Substring(2);
            int scode = ZCode.rpad(Msg, 0, 1);
            if (  scode == 11  )//Chat Message
            {
                ZCode.chatMsgList.Add(sms);
                L2D.LogLine(2, "code:11 "+sms);
            }            
            else if ( scode == 12 )//Zulu Time Check
            {                
                ZCode.b.EpochSync[ZCode.rpad(sms, 0, 2), ZCode.rpad(sms, 3, 3)] = Convert.ToInt32(sms.Substring(4)) ;
                ZCode.GPG_PlayerLastSeen[ZCode.rpad(sms, 3, 3)] = ZCode.rpad(sms, 0, 2);// last epoch 
                L2D.LogLine(2, "code:12 t:" + ZCode.rpad(sms, 0, 2) + " p:" + ZCode.rpad(sms, 3, 3) + " epoch:" + sms.Substring(4) );
            }
            else if ( scode == 13 )// Reading Ack Message
            {
                if ( ZCode.rpad(sms,3,3) == ZCode.ZPID )// Handle only my acks
                {
                    ZCode.b.AckArray[ZCode.rpad(sms, 0, 2)] += 1;// Receiving Acknowledgement                    
                }
                L2D.LogLine(2, "code:13 Ack Message:"+sms);
            }
            else
            {
                L2D.LogLine(0,"Midway Hacker");
                // Midway Hacker
            }
        }
    }
}

class PSCORE
{
    public static string DetailedText = "";

    public static int GetScore( int decision , int UL , int gtime )
    {
        DetailedText = "";
        int tTime = ZCode.ZMINUTES * 60;
        int result = 0;
        if ( decision > 0 )
        {
            int[] dScores = new int[] { 20000, (int)(10000 * (1 - (UL / (gtime + 1f)))), (int)(10000 * (1 - (gtime / (tTime + 1f)))) };
            result = dScores[0] + dScores[1] + dScores[2] ;
            DetailedText = dScores[0] + "\n" + dScores[1] + "\n" + dScores[2] + "\n\n"+result+" \n ";
        }
        else if ( decision == 0 )
        {
            int[] dScores = new int[] { 10000, (int)(10000 * (1 - (UL / (gtime + 1f)))), (int)(10000 * (1 - (gtime / (tTime + 1f)))) };
            result = dScores[0] + dScores[1] + dScores[2];
            DetailedText = dScores[0] + "\n" + dScores[1] + "\n" + dScores[2] + "\n\n" + result + " \n ";
        }
        else
        {
            int[] dScores = new int[] {  0, (int)(10000 * (1 - (UL / (gtime + 1f)))), (int)(-10000 * (1 - (gtime / (tTime + 1f)))) };
            result = dScores[0] + dScores[1] + dScores[2];
            DetailedText = dScores[0] + "\n" + dScores[1] + "\n" + dScores[2] + "\n\n" + result + " \n ";
        }

        return result;
    }
}


class L2D
{    
    public static bool shouldRecord = true;
    //------------------------------------
    public static bool isLoggable = false;
    public static string LogMsg = "";
    public static string LogFileName = "";
    public static string errorMsg = "";
    public static List<string> allLogs = new List<string>();
    public static StreamWriter sw;
    public static void Close()
    {
        if (isLoggable && shouldRecord )
        {
            //sw.Flush();
            //sw.Close();
        }
    }
    public static void ShareLogs()
    {
        if (isLoggable && shouldRecord)
        {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), LogFileName);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), LogFileName + " Pid:" + ZCode.ZPID);
            string logData = "";
            foreach (string log2 in allLogs)
            {
                logData += log2;
            }
            logData += "End Of Log";
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), logData);
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("startActivity", intentObject);
        }
    }
    public static void LogToDisc()
    {
        if (isLoggable && shouldRecord )
        {
            //sw.Write(LogMsg);
            //sw.Flush();
            allLogs.Add(LogMsg);
        }
        LogMsg = "";
    }
    public static void LogLine( int priority , string tmpLogMsg )
    {
        if ( isLoggable && shouldRecord )
        {
            string prefix;
            if (priority == 0)
            {
                prefix = "ERROR";
            }
            else if (priority == 1)
            {
                prefix = "DEBUG";                
            }
            else if (priority == 2)
            {
                prefix = "NTWRK";
            }
            else
            {
                prefix = "XXXXX";                
            }
            //LogMsg += DateTime.Now.ToString("yy:MM:dd:HH:mm:ss") + " " + prefix + " " + tmpLogMsg + Environment.NewLine;
            LogMsg += DateTime.Now.ToString("yy:MM:dd:HH:mm:ss") + " " + prefix + " " + tmpLogMsg + "#";
        }
    }
    public static void CreateLogFile()
    {
        if (shouldRecord)
        {
            isLoggable = false;            
            LogMsg = "";

            LogFileName = "MrwLogs_" + DateTime.Now.ToString("yy:MM:dd:HH:mm:ss");                      
            /*
            while (File.Exists(Application.persistentDataPath + "/" + LogFileName + ".txt"))
            {
                LogFileName += (new System.Random().Next(1, 9));
            }
            LogFileName = Application.persistentDataPath + "/" + LogFileName + ".txt";            
            */
            try
            {
                //sw = File.CreateText(LogFileName);
                //sw.WriteLine(ZCode.ZPLayerNames[ZCode.ZPID] + "(" + ZCode.ZPID + ")");                
                allLogs.Add(ZCode.ZPLayerNames[ZCode.ZPID] + "(" + ZCode.ZPID + ")"+"#");
                isLoggable = true;
                //errorMsg = "Succeeded " + LogFileName;                
            }
            catch(Exception e)
            {
                isLoggable = false;
                errorMsg = e.Message;
            }
        }
    }
}


