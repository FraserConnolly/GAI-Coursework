using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int AllyCount;
    public int EnemyCount;

    [SerializeField]
    TextMeshProUGUI _allyCountDisplay;
    
    [SerializeField]
    TextMeshProUGUI _enemyCountDisplay;

    [SerializeField]
    bool reloadOnLevelComplete = false;
    bool levelComplete = false;

    public void ReloadScene()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private GameData data;

    private void Start()
    {
        data = GameData.Instance;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            // reload scene
            ReloadScene();
            return;
        }

        if ( levelComplete  )
        {
            return;
        }

        AllyCount = data.allies.Where(a => a != null).Count();
        EnemyCount = data.enemies.Where(e => e != null).Count();

        UpdateDebugUI();

        if (AllyCount == 0)
        {
            levelComplete = true;

            Debug.Log("Enemies have won!");
            Debug.Break();

            if (reloadOnLevelComplete)
            {
            ReloadScene();
            return;
        }
        }
        else if (EnemyCount == 0)
        {
            levelComplete = true;

            Debug.Log("Allies have won!");
            Debug.Break();
            if (reloadOnLevelComplete)
        {
            Debug.Log("Enemies have won!");
            Debug.Break();
            ReloadScene();
            }
        }
    }

    private void UpdateDebugUI()
    {
        _allyCountDisplay.text = $"Ally Count\n{AllyCount}";
        _enemyCountDisplay.text = $"Enemy Count\n{EnemyCount}";
    }


}
