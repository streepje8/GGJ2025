using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public GameState GameState { get; private set; } = new GameState();
    public RequestManager RequestManager { get; private set; } = new RequestManager();
    [field: SerializeField] public BubbleWaves WaveContainer { get; private set; }
    [field: SerializeField] public GameObject RequestPrefab { get; private set; }
    [field: SerializeField] public Transform RequestPrefabParent { get; private set; }
    public static GameManager Instance { get; private set; }

    public GameObject GameObj;
    public GameObject LScreen;
    public TMP_Text ScoreText;

    private void Update()
    {
        if (GameState.Lives <= 0)
        {
            ScoreText.text = $"Score: {GameState.Score}";
            GameObj.SetActive(false);
            LScreen.SetActive(true);
        }
    }

    public int playerCount { get; private set; } = 0;
    private void Awake()
    {
        Instance = this;
        PlayerIDs.Enqueue(1);
        PlayerIDs.Enqueue(2);
        PlayerIDs.Enqueue(3);
        PlayerIDs.Enqueue(4);
    }

    public Queue<int> PlayerIDs { get; private set; } = new Queue<int>();
    public void JoinPlayer(PlayerInput player)
    {
        player.GetComponent<PlayerIdentifier>().SetID(PlayerIDs.Dequeue());
        playerCount++;
    }

    public void LeavePlayer(PlayerInput player)
    {
        PlayerIDs.Enqueue(player.GetComponent<PlayerIdentifier>().GetID());
        playerCount--;
    }
    
    public void NewGame()
    {
        GameState.Time = 0;
        GameState.Lives = 3;
        GameState.Difficulty = 0;
        GameState.Score = 0;
        GameState.Wave = 0;
    }

    public void StartGame()
    {
        NextWave();
    }

    public Request InstantiateRequest()
    {
        return Instantiate(RequestPrefab, RequestPrefabParent).GetComponent<Request>();
    }

    private void NextWave()
    {
        GameState.Wave++;
        Debug.Log($"Starting wave: {GameState.Wave}");
        if (WaveContainer.Waves.Count >= GameState.Wave)
        {
            SendWave(WaveContainer.Waves[GameState.Wave - 1]);
        }
    }

    private void SendWave(BubbleWave wave)
    {
        StartCoroutine(SendWaveConcurrent(wave, NextWave));
    }

    public delegate void PostWaveCompletionHandler();
    IEnumerator SendWaveConcurrent(BubbleWave wave, PostWaveCompletionHandler afterWave)
    {
        foreach (var request in wave.Requests.Shuffle(new Random(UnityEngine.Random.Range(0,1000))))
        {
            if (request.Name.Equals("DifficultyUp", StringComparison.OrdinalIgnoreCase))
            {
                GameState.Difficulty++;
            }
            else
            {
                HonourRequest(request);
                var timer = 0f;
                var randomMinimumTime = UnityEngine.Random.Range(2f, 4f);
                var maxTime = GetMaxTime(GameState.Difficulty);
                yield return new WaitUntil(() =>
                {
                    timer += Time.deltaTime;
                    if (timer >= maxTime) return true;
                    if (AllBubblesBeat() && timer >= randomMinimumTime) return true;
                    return false;
                });
            }
        }
        if(afterWave != null) afterWave();
        yield return null;
    }

    private bool AllBubblesBeat()
    {
        return RequestManager.ActiveRequests.Count < 1;
    }

    private float GetMaxTime(int difficulty)
    {
        return 20/(float)(difficulty+1);
    }

    private void HonourRequest(BubbleKind request)
    {
        RequestManager.Request(request);
    }

    public void SubmitBubble(BubbleKind kind)
    {
        RequestManager.ClearRequest(kind);
    }

    public GameObject Game;
    public GameObject MainMenu;
    public void SwitchToGame()
    {
        Game.SetActive(true);
        MainMenu.SetActive(false);
    }
}

public class RequestManager
{
    public List<Request> ActiveRequests = new List<Request>();
    public void Request(BubbleKind request)
    {
        var req = CreateRequest();
        req.BindToRequest(request);
        ActiveRequests.Add(req);
        req.Activate();
    }

    private Request CreateRequest()
    {
        return GameManager.Instance.InstantiateRequest();
    }

    public void ClearRequest(BubbleKind kind)
    {
        Request toDelete = null;
        float earliest = -1;
        for (var i = ActiveRequests.Count - 1; i >= 0; i--)
        {
            var req = ActiveRequests[i];
            if (req.Kind == kind)
            {
                if (req.t > earliest)
                {
                    earliest = req.t;
                    toDelete = req;
                }
            }
        }

        if (toDelete != null)
        {
            ActiveRequests.Remove(toDelete);
            Object.Destroy(toDelete.gameObject);
            Debug.Log("100 Points.");
            GameManager.Instance.GameState.Score += 100;
        }
        else
        {
            Debug.Log("5 Points.");
            GameManager.Instance.GameState.Score += 5;
        }
    }

    public void RequestLost(Request request)
    {
        ActiveRequests.Remove(request);
        Object.Destroy(request.gameObject);
        GameManager.Instance.GameState.Lives--;
    }
}

