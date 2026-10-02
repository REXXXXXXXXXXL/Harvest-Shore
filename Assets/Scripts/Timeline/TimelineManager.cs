using System;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : Singleton<TimelineManager>
{
    public PlayableDirector startDirector;
    private PlayableDirector currentDirector;
    public bool playStartCutscene;

    private bool isDone;
    public bool IsDone { set => isDone = value; }
    private bool isPause;
    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        if (startDirector != null)
        {
            startDirector.played += OnPlayed;
            startDirector.stopped += OnStopped;
        }
    }


    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        if (startDirector != null)
        {
            startDirector.played -= OnPlayed;
            startDirector.stopped -= OnStopped;
        }
    }


    private void Update()
    {
        if (isPause && Input.GetKeyDown(KeyCode.Space) && isDone)
        {
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }
    }

    private void OnStartNewGameEvent(int obj)
    {
        if (playStartCutscene && startDirector != null)
            startDirector.Play();
        else
            EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
    }

    private void OnAfterSceneLoadedEvent()
    {
        // currentDirector = FindObjectOfType<PlayableDirector>();
        // if (currentDirector != null)
        //     currentDirector.Play();
        if (!playStartCutscene || startDirector == null || !startDirector.isActiveAndEnabled)
            EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
    }

    private void OnStopped(PlayableDirector obj)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
    }

    private void OnPlayed(PlayableDirector obj)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);//時間停止
        
    }
    public void PauseTimeline(PlayableDirector director)
    {
        currentDirector = director;

        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        isPause = true;
    }
}
