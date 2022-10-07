using UnityEngine;
using Steamworks;
using System.Collections;
using System.Threading;

public class SteamLeaderboard : MonoBehaviour
{
    private const ELeaderboardUploadScoreMethod leaderboardMethod = ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest;

    public static SteamLeaderboard_t currentLeaderboard;
    public static SteamLeaderboardEntries_t currentLeaderboardEntrys;


    private static CallResult<LeaderboardFindResult_t> m_findResult = new CallResult<LeaderboardFindResult_t>();
    private static CallResult<LeaderboardScoreUploaded_t> m_uploadResult = new CallResult<LeaderboardScoreUploaded_t>();
    private static CallResult<LeaderboardScoresDownloaded_t> m_downloadResult = new CallResult<LeaderboardScoresDownloaded_t>();


    public static void UpdateScore(int score)
    {
        if (currentLeaderboard == null)
        {
            UnityEngine.Debug.Log("Can't upload to the leaderboard because isn't loadded yet");
        }
        else
        {
            UnityEngine.Debug.Log("uploading score(" + score + ") to steam leaderboard(" + SteamUserStats.GetLeaderboardName(currentLeaderboard) +")");
            int[] scoreDetail = new int[1];
            //scoreDetail[0] = ;
            SteamAPICall_t hSteamAPICall = SteamUserStats.UploadLeaderboardScore(currentLeaderboard, leaderboardMethod, score, scoreDetail, scoreDetail.Length);
            m_uploadResult.Set(hSteamAPICall, OnLeaderboardUploadResult);
        }
    }

    public static void DownloadLeaderboard()
    {
        SteamAPICall_t hSteamAPICall = SteamUserStats.DownloadLeaderboardEntries(currentLeaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, 10);
        m_downloadResult.Set(hSteamAPICall, OnLeaderboardDownloadResults);
    }

    public static void Init(string leaderboardName)
    {
        SteamAPICall_t hSteamAPICall = SteamUserStats.FindLeaderboard(leaderboardName);
        m_findResult.Set(hSteamAPICall, OnLeaderboardFindResult);
        InitTimer();
    }

    static private void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool failure)
    {
        UnityEngine.Debug.Log("STEAM LEADERBOARDS: Found - " + pCallback.m_bLeaderboardFound + " leaderboardID - " + pCallback.m_hSteamLeaderboard.m_SteamLeaderboard);
        currentLeaderboard = pCallback.m_hSteamLeaderboard;
    }

    static private void OnLeaderboardUploadResult(LeaderboardScoreUploaded_t pCallback, bool failure)
    {
        UnityEngine.Debug.Log("STEAM LEADERBOARDS: failure - " + failure + " Completed - " + pCallback.m_bSuccess + " NewScore: " + pCallback.m_nGlobalRankNew + " Score " + pCallback.m_nScore + " HasChanged - " + pCallback.m_bScoreChanged);
    }


    static private void OnLeaderboardDownloadResults(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
    {
        Debug.Log("[" + LeaderboardScoresDownloaded_t.k_iCallback + " - LeaderboardScoresDownloaded] - " + pCallback.m_hSteamLeaderboard + " -- " + pCallback.m_hSteamLeaderboardEntries + " -- " + pCallback.m_cEntryCount);

        currentLeaderboardEntrys = pCallback.m_hSteamLeaderboardEntries;
    }




    private static Timer timer1;
    public static void InitTimer()
    {
        timer1 = new Timer(timer1_Tick, null, 0, 1000);
    }

    private static void timer1_Tick(object state)
    {
        SteamAPI.RunCallbacks();
    }
}
