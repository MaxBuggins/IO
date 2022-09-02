using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Linq;


public class PlayFlowCloud : EditorWindow
{
    private static PlayFlowConfig data;

    private static string defaultPath = PlayFlowBuilder.defaultPath;
    private string serverlUrl;
    private string port = "";
    private string serverArguments = "";
    private string token = "";
    private string ssl = "";

    private string instanceType = "small";
    private int instanceIndex = 0;

    private int selected_server = 0;
    private bool devmode = false;

    private static float t;

    private string playflow_logs = "PlayFlow Logs: ";

    public List<string> active_servers = new List<string>();

    public string[] regionOptions = new string[]
        {"North America", "Europe", "Southeast Asia | Oceanic", "East Asia"};

    private string[] regions = new string[] {"us-east", "eu-west", "sea", "ea"};


    public string[] defaultregionOptions = new string[]
        {"North America", "Europe", "Southeast Asia | Oceanic", "East Asia"};

    private string[] defaultregions = new string[] {"us-east", "eu-west", "sea", "ea"};

    public string[] productionRegionOptions = new string[]
    {
        "North America East (North Virginia)", "North America West (California)", "North America West (Oregon)", "Europe (Stockholm)",
        "Europe (France)", "South Asia (Mumbai)", "South East Asia (Singapore)", "East Asia (Korea)",
        "East Asia (Japan)", "Australia (Sydney)"
    };
    

    private string[] productionRegions = new string[]
        {"us-east", "us-west", "us-west-2", "eu-north", "eu-west", "ap-south", "sea", "ea", "ap-north", "ap-southeast"};

    
    public int index = 0;
    private string region = "us-east";
    
    

    private string path = "";

    private static Texture banner = null;

    private bool isProductionToken = false;
    
    
    Dictionary<string, string> instance_types = new Dictionary<string, string>
    {
        { "small", "Small - 2 VCPU 1GB RAM" },
        { "medium", "Medium - 2 VCPU 2GB RAM" },
        { "large", "Large - 2 VCPU 4GB RAM"},
    };


    Vector2 scroll;


    public async void Awake()
    {
        getGlobalValues();
        await get_server_list();
    }

    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        // if no data exists yet create and reference a new instance
        if (!data)
        {
            // as first option check if maybe there is an instance already
            // and only the reference got lost
            // won't work ofcourse if you moved it elsewhere ...
            data = AssetDatabase.LoadAssetAtPath<PlayFlowConfig>("Assets/PlayFlowCloud/PlayFlowConfig.asset");
            // if that was successful we are done
            if (data) return;

            // otherwise create and reference a new instance
            data = CreateInstance<PlayFlowConfig>();

            AssetDatabase.CreateAsset(data, "Assets/PlayFlowCloud/PlayFlowConfig.asset");
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("PlayFlow/PlayFlowCloud Server")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(PlayFlowCloud));
    }

    private static GUISkin _uiStyle;

    public static GUISkin uiStyle
    {
        get
        {
            if (_uiStyle != null)
                return _uiStyle;
            _uiStyle = GetUiStyle();
            return _uiStyle;
        }
    }


    private static GUISkin GetUiStyle()
    {
        var searchRootAssetFolder = Application.dataPath;
        var playFlowGuiPath =
            Directory.GetFiles(searchRootAssetFolder, "PlayFlowSkin.guiskin", SearchOption.AllDirectories);
        foreach (var eachPath in playFlowGuiPath)
        {
            var loadPath = eachPath.Substring(eachPath.LastIndexOf("Assets"));
            return (GUISkin) AssetDatabase.LoadAssetAtPath(loadPath, typeof(GUISkin));
        }

        return null;
    }

    void getGlobalValues()
    {
        var serializedObject = new SerializedObject(data);
        token = serializedObject.FindProperty("token").stringValue;
        serverArguments = serializedObject.FindProperty("serverArguments").stringValue;
        port = serializedObject.FindProperty("playflowUrl").stringValue;
        ssl = serializedObject.FindProperty("enableSSL").boolValue.ToString();
        index = serializedObject.FindProperty("serverLocation").intValue;

        try
        {
            instanceIndex = serializedObject.FindProperty("instanceIndex").intValue;
        }
        catch
        {
            instanceIndex = 0;
        }

        try
        {
            region = regions[index];
        }
        catch
        {
            index = 0;
            region = regions[index];
        }
    }

    void OnGUI()
    {
        if (banner == null)
        {
            banner = (Texture) AssetDatabase.LoadAssetAtPath("Assets/PlayFlowCloud/Resources/playflow.png",
                typeof(Texture));
        }

        if (token.Length > 60)
        {
            isProductionToken = true;
        }
        else
        {
            isProductionToken = false;
        }

        var serializedObject = new SerializedObject(data);
        // fetches the values of the real instance into the serialized one
        serializedObject.Update();
        var configtoken = serializedObject.FindProperty("token");
        var configserverArguments = serializedObject.FindProperty("serverArguments");
        var configport = serializedObject.FindProperty("playflowUrl");
        var configenableSSL = serializedObject.FindProperty("enableSSL");
        var configapiUrl = serializedObject.FindProperty("serverLocation");
        var configinstanceType = serializedObject.FindProperty("instanceIndex");



        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUI.skin = uiStyle;

        GUILayout.Box(banner, uiStyle.box);

        // GUILayout.Label("PlayFlow Cloud Server Deploy Settings");
        // EditorGUILayout.LabelField(
        //     "Use the PlayFlow Server Port number as your game's port for both the clients and server",
        //     uiStyle.GetStyle("labelsmall"));


        configtoken.stringValue =
            EditorGUILayout.TextField("PlayFlow App Token", configtoken.stringValue, uiStyle.textField);


        configserverArguments.stringValue = EditorGUILayout.TextField("Arguments (optional)",
            configserverArguments.stringValue, uiStyle.textField);

        EditorGUILayout.BeginHorizontal();

        configenableSSL.boolValue = EditorGUILayout.Toggle("Enable SSL", configenableSSL.boolValue);


        if (ssl.Equals(true.ToString()) && isProductionToken)
        {
            configport.stringValue =
                EditorGUILayout.TextField("Server Port", configport.stringValue, uiStyle.textField);
        }

        if (isProductionToken)
        {
            regionOptions = productionRegionOptions;
            regions = productionRegions;
        }
        else
        {
            regionOptions = defaultregionOptions;
            regions = defaultregions;
        }

        EditorGUILayout.EndHorizontal();

        devmode = EditorGUILayout.Toggle("Development Build", devmode);


        getGlobalValues();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Server Location (Free Plan)", uiStyle.GetStyle("labelsmall"));
        index = EditorGUILayout.Popup(configapiUrl.intValue, regionOptions);
        configapiUrl.intValue = index;
        EditorGUILayout.EndHorizontal();


        if (isProductionToken)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Instance Type", uiStyle.GetStyle("labelsmall"));
            instanceIndex = EditorGUILayout.Popup(configinstanceType.intValue, instance_types.Values.ToArray());
            configinstanceType.intValue = instanceIndex;
            instanceType = instance_types.Keys.ToList()[instanceIndex]; 
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Get Active Servers"))
        {
            getservers();
        }

        if (GUILayout.Button("Upload Server"))
        {
            BuildAndZip();
            GUIUtility.ExitGUI();
        }

        if (GUILayout.Button("Upload Status"))
        {
            get_upload_status();
        }


        // if (GUILayout.Button("Upload Server Zip"))
        // {
        //     // upload_files_directly();
        //     Debug.Log(EditorUserBuildSettings.activeBuildTarget);
        // }

        if (GUILayout.Button("Start Server"))
        {
            start_server();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();


        EditorGUILayout.BeginVertical();
        GUILayout.Label("Active Servers", uiStyle.GetStyle("labelsmall"));
        selected_server = EditorGUILayout.Popup(selected_server, active_servers.ToArray());
        EditorGUILayout.EndVertical();


        if (GUILayout.Button("Get Server Status"))
        {
            get_status();
        }


        if (GUILayout.Button("Restart Server"))
        {
            restart_server();
        }


        if (GUILayout.Button("Stop Server"))
        {
            stop_server();
        }


        if (GUILayout.Button("Get Logs"))
        {
            get_logs();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Get PlayFlow Token"))
        {
            System.Diagnostics.Process.Start("https://app.playflowcloud.com");
        }

        if (GUILayout.Button("Reset PlayFlow Instance"))
        {
            reset_instance();
        }

        if (GUILayout.Button("Documentation"))
        {
            System.Diagnostics.Process.Start("https://docs.playflowcloud.com");
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextArea(playflow_logs, uiStyle.textArea);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
        serializedObject.ApplyModifiedProperties();
    }

    private async void get_status()
    {
        EditorUtility.ClearProgressBar();
        try
        {
            if (token == null || token == "")
            {
                playflow_logs = "Please provide a PlayFlow App Token to get started from https://app.playflowcloud.com";
                return;
            }

            if (!active_servers.Any())
            {
                playflow_logs = "No server selected";
                return;
            }

            EditorUtility.DisplayProgressBar("PlayFlowCloud", "Getting Server Info", 0.75f);
            string response = await PlayFlowAPI.GetServerStatus(token, active_servers[selected_server]);
            setPlayFlowLogs(response);
        }
        catch (Exception e)
        {
            playflow_logs = "PlayFlow Restart Failed! StackTrace: " + e.StackTrace;
            EditorUtility.ClearProgressBar();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    public void OnInspectorUpdate()
    {
        Repaint();
    }

    private void BuildAndZip()
    {
        BuildTarget standaloneTarget = EditorUserBuildSettings.selectedStandaloneTarget;
        BuildTargetGroup currentBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(standaloneTarget);
#if UNITY_2021_2_OR_NEWER
        StandaloneBuildSubtarget currentSubTarget = EditorUserBuildSettings.standaloneBuildSubtarget;
#endif
        try
        {
            if (token == null || token == "")
            {
                playflow_logs = "Please provide a PlayFlow App Token to get started from https://app.playflowcloud.com";
                return;
            }


            PlayFlowBuilder.BuildServer(devmode);
            EditorUtility.DisplayProgressBar("PlayFlowCloud", "Zipping Files", 0.4f);
            string zipFile = PlayFlowBuilder.ZipServerBuild();

            string directoryToZip = Path.GetDirectoryName(defaultPath);
            string targetfile = Path.Combine(directoryToZip, @".." + Path.DirectorySeparatorChar + "Server.zip");
            EditorUtility.DisplayProgressBar("PlayFlowCloud", "Uploading Files", 0.75f);
            playflow_logs = PlayFlowAPI.Upload(targetfile, token, region);


            //PlayFlowBuilder.cleanUp(zipFile);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            EditorUserBuildSettings.SwitchActiveBuildTarget(currentBuildTargetGroup, standaloneTarget);
#if UNITY_2021_2_OR_NEWER

            EditorUserBuildSettings.standaloneBuildSubtarget = currentSubTarget;
#endif
        }
    }

    private void upload_files_directly()
    {
        try
        {
            path = EditorUtility.OpenFilePanel("Select Server", "", "zip");
            EditorUtility.DisplayProgressBar("PlayFlowCloud", "Uploading Files", 0.75f);
            playflow_logs = PlayFlowAPI.Upload(path, token, region);
        }
        catch (Exception e)
        {
            playflow_logs = "PlayFlow Build & Publish Failed! StackTrace: " + e.StackTrace;
            EditorUtility.ClearProgressBar();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private async void get_upload_status()
    {
        EditorUtility.ClearProgressBar();
        try
        {
            if (token == null || token == "")
            {
                playflow_logs = "Please provide a PlayFlow App Token to get started from https://app.playflowcloud.com";
                return;
            }

            EditorUtility.DisplayProgressBar("PlayFlowCloud", "Resetting PlayFlow Instance", 0.5f);
            string response = await PlayFlowAPI.Get_Upload_Version(token);
            Debug.Log(response);
            setPlayFlowLogs(response);
            Debug.Log(playflow_logs);
        }
        catch (Exception e)
        {
            playflow_logs = "PlayFlow Cloud Failed! StackTrace: " + e.StackTrace;
            EditorUtility.ClearProgressBar();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private async void reset_instance()
    {
        EditorUtility.ClearProgressBar();
        try
        {
            if (token == null || token == "")
            {
                playflow_logs = "Please provide a PlayFlow App Token to get started from https://app.playflowcloud.com";
                return;
            }

            EditorUtility.DisplayProgressBar("PlayFlowCloud", "Resetting PlayFlow Instance", 0.5f);
            string response = await PlayFlowAPI.ResetInstance(token);
            setPlayFlowLogs(response);
        }
        catch (Exception e)
        {
            playflow_logs = "PlayFlow Reset Server Failed! StackTrace: " + e.StackTrace;
            EditorUtility.ClearProgressBar();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private async void start_server()
    {
        EditorUtility.ClearProgressBar();

        MatchInfo matchInfo = null;
        try
        {
            if (token == null || token == "")
            {
                playflow_logs = "Please provide a PlayFlow App Token to get started from https://app.playflowcloud.com";
                return;
            }

            EditorUtility.DisplayProgressBar("PlayFlowCloud", "Starting Server", 0.75f);
            string response = await PlayFlowAPI.StartServer(token, region, serverArguments, ssl, port, instanceType);
            setPlayFlowLogs(response);

            matchInfo = JsonUtility.FromJson<MatchInfo>(response);
        }
        catch (Exception e)
        {
            playflow_logs = "PlayFlow Start Server Failed! StackTrace: " + e.StackTrace;
            EditorUtility.ClearProgressBar();
        }
        finally
        {
            await get_server_list();

            if (matchInfo != null)
            {
                string match = matchInfo.match_id;

                if (matchInfo.ssl_port != null)
                {
                    match = matchInfo.match_id + " -> (SSL) " + matchInfo.ssl_port;
                }

                selected_server = active_servers.IndexOf(match);
            }

            EditorUtility.ClearProgressBar();
        }
    }

    private void setPlayFlowLogs(string json)
    {
        playflow_logs = json;
    }


    private async void restart_server()
    {
        EditorUtility.ClearProgressBar();
        try
        {
            if (token == null || token == "")
            {
                playflow_logs = "Please provide a PlayFlow App Token to get started from https://app.playflowcloud.com";
                return;
            }

            if (!active_servers.Any())
            {
                playflow_logs = "No server selected";
                return;
            }

            EditorUtility.DisplayProgressBar("PlayFlowCloud", "Restarting Server", 0.75f);
            string response =
                await PlayFlowAPI.RestartServer(token, region, serverArguments, ssl, active_servers[selected_server]);
            setPlayFlowLogs(response);
        }
        catch (Exception e)
        {
            playflow_logs = "PlayFlow Restart Failed! StackTrace: " + e.StackTrace;
            EditorUtility.ClearProgressBar();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }


    private async void stop_server()
    {
        EditorUtility.ClearProgressBar();
        try
        {
            if (token == null || token == "")
            {
                playflow_logs = "Please provide a PlayFlow App Token to get started from https://app.playflowcloud.com";
                return;
            }

            if (!active_servers.Any())
            {
                playflow_logs = "No server selected";
                return;
            }

            EditorUtility.DisplayProgressBar("PlayFlowCloud", "Stopping Server", 0.75f);
            playflow_logs = await PlayFlowAPI.StopServer(token, region, active_servers[selected_server]);
        }
        catch (Exception e)
        {
            playflow_logs = "PlayFlow Stop Server Failed! StackTrace: " + e.StackTrace;
            EditorUtility.ClearProgressBar();
        }
        finally
        {
            await get_server_list();
            EditorUtility.ClearProgressBar();
        }
    }

    private async void get_logs()
    {
        EditorUtility.ClearProgressBar();
        try
        {
            if (token == null || token == "")
            {
                playflow_logs = "Please provide a PlayFlow App Token to get started from https://app.playflowcloud.com";
                return;
            }

            if (!active_servers.Any())
            {
                playflow_logs = "No server selected";
                return;
            }

            EditorUtility.DisplayProgressBar("PlayFlowCloud", "Getting Server Logs", 0.75f);
            playflow_logs = await PlayFlowAPI.GetServerLogs(token, region, active_servers[selected_server]);
            string[] split = playflow_logs.Split(new[] {"\\n"}, StringSplitOptions.None);
            playflow_logs = "";
            foreach (string s in split)
                playflow_logs += s + "\n";
        }
        catch (Exception e)
        {
            playflow_logs = "PlayFlow Stop Server Failed! StackTrace: " + e.StackTrace;
            EditorUtility.ClearProgressBar();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private async void getservers()
    {
        EditorUtility.ClearProgressBar();

        try
        {
            if (token == null || token == "")
            {
                playflow_logs = "Please provide a PlayFlow App Token to get started from https://app.playflowcloud.com";
                return;
            }

            EditorUtility.DisplayProgressBar("PlayFlowCloud", "Getting Active Servers", 0.75f);
            await get_server_list();
        }
        catch (Exception e)
        {
            playflow_logs = "PlayFlow Build & Publish Failed! StackTrace: " + e.StackTrace;
            EditorUtility.ClearProgressBar();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private async Task get_server_list()
    {
        try
        {
            if (token == null || token == "")
            {
                playflow_logs = "Please provide a PlayFlow App Token to get started from https://app.playflowcloud.com";
                return;
            }

            EditorUtility.DisplayProgressBar("PlayFlowCloud", "Updating Servers Info", 0.75f);
            string response = await PlayFlowAPI.GetActiveServers(token, region, true);
            Server[] servers = JsonHelper.FromJson<Server>(response);
            active_servers = new List<string>();
            foreach (Server server in servers)
            {
                string serverInfo = server.port;

                if (server.ssl_enabled)
                {
                    serverInfo = server.port + " -> (SSL) " + server.ssl_port;
                }

                active_servers.Add(serverInfo);
            }

            active_servers.Sort();
            selected_server = active_servers.Count - 1;
            setPlayFlowLogs(response);
        }
        catch (Exception e)
        {
            playflow_logs = "PlayFlow Build & Publish Failed! StackTrace: " + e.StackTrace;
            EditorUtility.ClearProgressBar();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }
}

[Serializable]
public class Server
{
    public string ssl_port;
    public bool ssl_enabled;
    public string server_arguments;
    public string status;
    public string port;
}


[Serializable]
public class MatchInfo
{
    public string match_id;
    public string server_url;
    public string ssl_port;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.servers;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.servers = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.servers = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] servers;
    }
}