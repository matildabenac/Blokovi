using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Dropdown spawnRateOptions;
    public int spawnRate;
    public Button startButton;
    public Button exitButton;
    public Button endButton;

    public GameObject menu;
    public GameObject end;
    
    private static UIManager _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        spawnRateOptions.ClearOptions();
        var options = new List<string>();
        for (var i = 1; i <= 15; ++i)
        {
            options.Add(i.ToString());
        }
        spawnRateOptions.AddOptions(options);

        spawnRate = 1;
        
        spawnRateOptions.onValueChanged.AddListener(SetSpawnRate);
        startButton.onClick.AddListener(Load);
        exitButton.onClick.AddListener(Exit);
        endButton.onClick.AddListener(End);
    }

    // Update is called once per frame
    // void Update()
    // {
    //     
    // }

    private void SetSpawnRate(int value)
    {
        spawnRate = value + 1;
    }

    private void Load()
    {
        menu.SetActive(false);
        end.SetActive(true);
        SceneManager.LoadScene("GameScene");
    }

    private static void Exit()
    {
        Application.Quit();
    }

    private void End()
    {
        menu.SetActive(true);
        end.SetActive(false);
        SceneManager.LoadScene("MenuScene");
    }
}
