#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    private static readonly string CURRENT_LEVEL_ID = "current_level";
    private static readonly string ACTUAL_LEVEL_ID = "actual_level";

    private static readonly string RATE_US_ALREADY_SHOWN = "rate_us_already_shown";

    private static readonly string SAVE_FILE_NAME = "GameSave.dat";

    public static int PLAYER_LAYER;
    public static int ENEMY_LAYER;
    public static int BORDER_LAYER;
    public static int OBSTACLE_LAYER;
    public static int PROJECTILE_LAYER;
    public static int FINISH_LAYER;
    public static int GREEN_ORB_LAYER;

    [SerializeField] LevelDatabase levelDatabase;
    [SerializeField] GameSettings gameSettings;

    public static LevelDatabase LevelDatabase => instance.levelDatabase;
    public static GameSettings GameSettings => instance.gameSettings;

    public static int CurrentLevelId
    {
        get => GameSettingsPrefs.Get<int>(CURRENT_LEVEL_ID);
        set => GameSettingsPrefs.Set(CURRENT_LEVEL_ID, value);
    }

    public static int ActualLevelId
    {
        get => GameSettingsPrefs.Get<int>(ACTUAL_LEVEL_ID);
        set => GameSettingsPrefs.Set(ACTUAL_LEVEL_ID, value);
    }

    public static int LevelToPlay { get; set; }

    public static int CurrentRoomId { get; private set; }

    public static Zone CurrentZone { get; private set; }
    public static Level CurrentLevel { get; private set; }
    public static Level.Room CurrentRoom { get; private set; }

    private static GameSave gameSave;

    public static float Sound => GameSettingsPrefs.Get<float>("sound");

    private static readonly string LAST_OPEN_LEVEL_ID = "last_open_level_id";
    public static int LastOpenLevelId { get => GameSettingsPrefs.Get<int>(LAST_OPEN_LEVEL_ID); set => GameSettingsPrefs.Set(LAST_OPEN_LEVEL_ID, value); }

    public static bool RateUsAlreadyShown {
        get => GameSettingsPrefs.Get<bool>(RATE_US_ALREADY_SHOWN);
        set => GameSettingsPrefs.Set(RATE_US_ALREADY_SHOWN, value);
    }
    public static bool ShowRateUsPanel { get; private set; }

    public static bool HealthRoom { get; private set; }

    public static bool HasResurected { get; set; }


    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }

        ActualLevelId = 0;

        instance = this;

        ENEMY_LAYER = LayerMask.NameToLayer("Enemy");
        PLAYER_LAYER = LayerMask.NameToLayer("Player");
        FINISH_LAYER = LayerMask.NameToLayer("Finish");
        BORDER_LAYER = LayerMask.NameToLayer("Border");
        OBSTACLE_LAYER = LayerMask.NameToLayer("Obstacle");
        PROJECTILE_LAYER = LayerMask.NameToLayer("Projectile");
        GREEN_ORB_LAYER = LayerMask.NameToLayer("GreenOrb");

        HealthRoom = false;

        ShowRateUsPanel = false;

        LoadSave();
    }

    private void Start()
    {
        StartGame();
    }

    public static void StartGame()
    {
        CurrentZone = LevelDatabase.GetZone(0);
        CurrentLevel = CurrentZone.Levels[0];

        CurrentRoomId = 0;
        CurrentRoom = CurrentLevel.GetRoom(0);

        HealthRoom = false;

        //GamePanelBehavior.SetLevelNumber(LevelToPlay);
        
        StartLevel();
    }

    
    public static void FinishRoom(bool success)
    {
        if (success)
        {
            if (CurrentRoomId + 1 == CurrentLevel.RoomsCount)
            {
                LevelCompleteBehavior.Show(true, FinishLevel);
            } else
            {
                GamePanelBehavior.DoHidden(() => {
                    /*if (HealthRoom)
                    {
                        HealthRoom = false;
                    }
                    else
                    {
                        CurrentRoomId++;

                        if (CurrentRoomId == 4)
                        {
                            HealthRoom = true;
                        }
                    }*/

                    CurrentRoomId++;

                    //GamePanelBehavior.SetProgression(0);

                    CurrentRoom = CurrentLevel.GetRoom(CurrentRoomId);
                    LevelController.DiscardLevel();
                    PlayerController.DisablePlayer();

                    AbilitiesController.OnRoomFinished();

                    Tween.NextFrame(() => StartRoom(false));
                    
                });
            }
        } else
        {
            LevelCompleteBehavior.Show(false, ReturnToMap);
        }

        /*GamePanelBehavior.DoHidden(() => {
            if (success)
            {
                if (HealthRoom)
                {
                    HealthRoom = false;
                } else
                {
                    CurrentRoomId++;

                    if(CurrentRoomId == 4)
                    {
                        HealthRoom = true;
                    }
                }

                GamePanelBehavior.SetProgression(((float)CurrentRoomId) / CurrentLevel.RoomsCount);

                if (CurrentRoomId == CurrentLevel.RoomsCount)
                {
                    FinishLevel();
                }
                else
                {
                    CurrentRoom = CurrentLevel.GetRoom(CurrentRoomId);


                    LevelController.DiscardLevel();
                    PlayerController.DisablePlayer();

                    Tween.NextFrame(() => StartRoom(false));
                }
            }
            else
            {
                ReturnToMap();
            }
        });*/


    }


    private static void FinishLevel()
    {
        CurrentLevelId++;
        ActualLevelId = LevelDatabase.GetNextLevelId(ActualLevelId, CurrentLevelId >= LevelDatabase.LevelsCount);

        CurrentZone = LevelDatabase.GetZone(0);
        CurrentLevel = CurrentZone.Levels[ActualLevelId];

        CurrentRoomId = 0;
        CurrentRoom = CurrentLevel.GetRoom(CurrentRoomId);

        ShowRateUsPanel = !RateUsAlreadyShown;

        ReturnToMap();

    }

    public static void ReturnToMap()
    {
        LevelController.DiscardLevel();
        PlayerController.DisablePlayer();

        LevelController.DestroyPools();

        Tween.RemoveAll();

        Tween.NextFrame(() => SceneLoader.LoadScene("Menu"));
    }

    public static void StartRoom(bool resetHP)
    {
        LevelController.InitRoom();
        PlayerController.InitPlayer(resetHP);
        CameraController.InitCamera();
    }

    public static void StartLevel()
    {
        // DEV
        //if (!GameSettingsPrefs.Get<bool>("tutorial"))
        //{
        //    GamePanelBehavior.ShowTutorial();
        //}

        LevelController.InitLevel();

        StartRoom(true);

        HasResurected = false;
    }

    public static void PauseLevel()
    {

    }

    public static void Save()
    {
        Serializer.SerializeToPDP(gameSave, SAVE_FILE_NAME);
    }

    private static void LoadSave()
    {
        gameSave = Serializer.DeserializeFromPDP<GameSave>(SAVE_FILE_NAME, Serializer.SerializeType.Binary, "", false);
    }

    public static SkinSave GetSkinSave(SkinData skinData)
    {
        return gameSave.GetSkinSave(skinData);
    }
}
