using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
	public GameObject GetEnemyAt(int index)
	{
		return enemyUnits[index];
	}

	public int GetEnemyCount()
	{
		return enemyUnits.Count;
	}

	public List<GameObject> GetCopyOfEnemiesList()
	{
		return new List<GameObject>(enemyUnits);
	}

	public GameObject GetAllyAt(int index)
	{
		return allyUnits[index];
	}

	public int GetAllyCount()
	{
		return allyUnits.Count;
	}

	public List<GameObject> GetCopyOfAlliesList()
	{
		return new List<GameObject>(allyUnits);
	}

	public float GetUnitHealth(GameObject gameObject)
	{
		if (unitToHealth.ContainsKey(gameObject))
		{
			return unitToHealth[gameObject];
		}
		return -1.0f;
	}

	#region Private interface

	private Dictionary<GameObject, float> unitToHealth = new Dictionary<GameObject, float>();

    private List<GameObject> enemyUnits = new List<GameObject>();
	private List<GameObject> allyUnits = new List<GameObject>();

    private Sprite unitSprite;

    private GameObject enemiesGO;
    private GameObject alliesGO;

	

	// Start is called before the first frame update
	private void Start()
    {
        enemiesGO = new GameObject("Enemies");
        enemiesGO.transform.parent = transform;

		alliesGO = new GameObject("Allies");
		alliesGO.transform.parent = transform;

		var gameData = GetComponent<GameData>();

        var map = gameData.Map;
        var enemyUnitLocations = map.GetInitialUnitLocations();
        var allyUnitLocations = map.GetInitialPlayerLocations();

        unitSprite = Resources.Load<Sprite>("Unit");

		foreach (var enemyUnitLocation in enemyUnitLocations)
		{
			enemyUnits.Add(CreateUnit(map, map.MapIndexToX(enemyUnitLocation), map.MapIndexToY(enemyUnitLocation), true));
		}
		foreach (var allyUnitLocation in allyUnitLocations)
		{
			allyUnits.Add(CreateUnit(map, map.MapIndexToX(allyUnitLocation), map.MapIndexToY(allyUnitLocation), false));
		}

		CopyUnitsToLists();
	}

    private void Update()
    {
		CopyUnitsToLists();

	}

	private void CopyUnitsToLists()
	{
		var gameData = GameData.Instance;
		if(gameData.allies.Count != allyUnits.Count)
		{
			gameData.allies.Clear();
			foreach (var unit in allyUnits)
			{
				gameData.allies.Add(unit);
			}
		}
		if (gameData.enemies.Count != enemyUnits.Count)
		{
			gameData.enemies.Clear();
			foreach (var unit in enemyUnits)
			{
				gameData.enemies.Add(unit);
			}
		}
	}


    private GameObject CreateUnit(Map map, int mapX, int mapY, bool isEnemy)
    {
		var unit = new GameObject(isEnemy ? "Enemy " + enemyUnits.Count : "Ally " + allyUnits.Count);
        unit.transform.parent = isEnemy ? enemiesGO.transform : alliesGO.transform;
		unit.transform.position = new Vector3(mapX, mapY, 0.0f);

		var spriteRenderer = unit.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = unitSprite;
		spriteRenderer.color = isEnemy ? Color.yellow : Color.magenta;

        var collider = unit.AddComponent<CircleCollider2D>();
        collider.radius = SteeringAgent.CollisionRadius;

		unitToHealth.Add(unit, 1.0f);

		// Ensure this is last as the users entry point into these classes will be called when this happens
		// so everything needs setup before this
		if (isEnemy)
        {
            unit.AddComponent<EnemyAgent>();
        }
        else
        {
			unit.AddComponent<AllyAgent>();
		}

		return unit;
    }

	/// <summary>
	/// Never call this! Anyone calling this function manually will deducted marks from their coursework!
	/// Applies damage to the unit and will Destroy the unit if dead
	/// </summary>
	/// <param name="unit">Unit to apply damage to</param>
	/// <param name="damage">Amount of damage to apply</param>
    public void ApplyDamageToUnit(GameObject unit, float damage)
    {
        if(unitToHealth.ContainsKey(unit))
        {
            unitToHealth[unit] -= damage;

            if(unitToHealth[unit] <= 0.0f)
            {
                unitToHealth.Remove(unit);
                unit.SetActive(false);
                Destroy(unit);
			}
        }
    }
	#endregion
}
