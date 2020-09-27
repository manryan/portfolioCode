using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using System.Net;
using System.IO;

public enum Language { english = 1, spanish };
public class GameManager : MonoBehaviour, IUnityAdsListener
{
    #region Variables

    public int teamQuantity;

    public int playerQuantity;

    #endregion

    #region GameManager Singleton/Awake

    static GameManager _instance = null;

   public RecordedValues values;

    public Language lang;

    public int langValue;


    public void saveLevel()
    {
          string json = JsonUtility.ToJson(values);
        //    PlayerPrefs.SetString("TestSave" + SceneManager.GetActiveScene().name + id.instance.saveIndex, json);
        //   PlayerPrefs.Save();

        PlayerPrefs.SetString("recordedvalues", json);

        PlayerPrefs.Save();
    }

   public List<Text> results;

    #region generic

    /* public static List<T> FindObjectsOfTypeAll<T>()
     {
         List<T> results = new List<T>();

         for (int i = 0; i < SceneManager.sceneCount; i++)
         {
             var s = SceneManager.GetSceneAt(i);
             if (s.isLoaded)
             {
                 var allGameObjects = s.GetRootGameObjects();
                 for (int j = 0; j < allGameObjects.Length; j++)
                 {
                     var go = allGameObjects[j];
                       results.AddRange(go.GetComponentsInChildren<T>(true));
                 }
             }
         }
         return results;
     }*/

    #endregion


    public void FindAllText()
    {
        //List<T> results = new List<T>();
        results= new List<Text>();

                var allGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
                for (int j = 0; j < allGameObjects.Length; j++)
                {
                    var go = allGameObjects[j];
                    //    results.AddRange(go.GetComponentsInChildren<T>(true));
                    results.AddRange(go.GetComponentsInChildren<Text>(true));
                }

        for (int i = results.Count-1; i >0; i--)
        {
            if (results[i].text.Length == 0)
                results.Remove(results[i]);
        }
    }

    //   public Text[] allTexts;

    string[] englishWords;

    public void updateAllText()
    {
        if (SceneManager.GetActiveScene().buildIndex==1 && englishWords==null)
            englishWords = new string[results.Count];


        //update to spanish?
        if(langValue==1)
        for (int i = 0; i < results.Count; i++)
        {

            if (results[i])
            {
                    //  if (results[i].text.Length > 0)
                    //   {
                    //replace the text?
                    if (I18n.langkey.Fields[1].ContainsKey(results[i].text))
                    {
                        englishWords[i] = results[i].text;
                        results[i].text = I18n.langkey.Fields[1][results[i].text];
                    }
              //  }
            }
        }
        else
        {
            for (int i = 0; i < results.Count; i++)
            {

                if (results[i])
                {
                    //  if (results[i].text.Length > 0)
                    //   {
                    //replace the text?
                    if(englishWords[i]!=null)
                        results[i].text = englishWords[i];
                    //  }
                }
            }
        }




        //cc.updatebuttonsText();
    }

    public string GetHtmlFromUri(string resource)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 0 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
               
                        cs[0] = new char();
                        reader.Read(cs, 0, cs.Length);
                        html += cs[0];
                    }
                }
            }
        }
        catch
        {
            return "";
        }
        return html;
    }
    char[] cs = new char[1];

    public bool testinternet()
    {
        //   HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://google.com");
        string HtmlText = GetHtmlFromUri("http://google.com");
        if (HtmlText == "")
        {
            //No connection
            return false;
        }
        else
        {
            Debug.Log("connected");
        }

        return true;
    }


    string gameId = "3667537";
    bool testMode = true;
    string myPlacementId = "rewardedVideo";
    void Awake()
    {
         prod = SaveSystem.LoadPlayer(Application.persistentDataPath + "/gamesave.save");

        //  File.Delete(Application.persistentDataPath + "/gamesave.save");

        string jsonString = PlayerPrefs.GetString("recordedvalues");
        values = JsonUtility.FromJson<RecordedValues>(jsonString);

        if(values==null)
        {
            values = new RecordedValues();
            values.gunLevel = 1;
            values.gunTime = new List<float>();
        }

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

      langValue=  PlayerPrefs.GetInt("language");

       if(langValue!=0)
        {
            lang = Language.spanish;
        }
       

        DontDestroyOnLoad(gameObject);

        //Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
        // Show an ad:
      //  Advertisement.Show();
    }

    public void addlistener()
    {
        Advertisement.AddListener(this);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            Time.timeScale = 1;
            adOpen = false;
            // Reward the user for watching the ad to completion.
        }
        else if (showResult == ShowResult.Skipped)
        {
            Time.timeScale = 1;
            adOpen = false;
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            Time.timeScale = 1;
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("what called this");
        // If the ready Placement is rewarded, show the ad:
        if (placementId == myPlacementId)
        {
       //     Advertisement.Show(myPlacementId);
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }


    public bool adOpen;

    public void showAd(string scenename)
    {
        SceneManager.LoadScene(scenename);
        if(prod==null || !prod.noads)
        if (Advertisement.IsReady())
        {
            adOpen = true;
            Advertisement.Show("video");
        }
    }

    public bool ready()
    {
        if (Advertisement.IsReady())
            return true;

        return false;
    }

    public static GameManager instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    #endregion

    public List<TeamInstance> teamInfos;

    public void purchasedNoAds()
    {
        prod = new products();
        prod.noads = true;
        SaveSystem.SavePlayer(prod,Application.persistentDataPath + "/gamesave.save");
    }

    public products prod;

}

[System.Serializable]
public class TeamInstance
{
    public string[] memberNames;

    public string teamName;

}
[System.Serializable]
public class products
{
    public bool noads;

}
