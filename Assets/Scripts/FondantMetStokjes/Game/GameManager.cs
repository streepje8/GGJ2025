using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public GameState GameState { get; private set; } = new GameState();
    public RequestManager RequestManager { get; private set; } = new RequestManager();
    [field: SerializeField] public BubbleWaves WaveContainer { get; private set; }
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
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

    private void NextWave()
    {
        GameState.Wave++;
        if (WaveContainer.Waves.Count >= GameState.Wave)
        {
            SendWave(WaveContainer.Waves[GameState.Wave]);
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
        yield return null;
    }

    private bool AllBubblesBeat()
    {
        return false;
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
        Debug.Log($"Submitted: {kind.Name}");
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
        throw new NotImplementedException();
    }
}

public class Request : MonoBehaviour
{
    public BubbleKind Kind { get; set; }

    public void BindToRequest(BubbleKind request)
    {
        Kind = request;
    }

    public void Activate()
    {
        throw new NotImplementedException();
    }
}