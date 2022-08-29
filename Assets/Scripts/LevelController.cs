using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] List<Level> levels;
    [SerializeField] Opponent opponent;
    private Level currentLevelInstance;
    private int currentLevelIndex = 0;

    #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            currentLevelInstance = levels[currentLevelIndex];
            opponent.ChangeCharacteristics(currentLevelInstance.opponentDamage, currentLevelInstance.opponentStandUpChance);
            Debug.Log("Current level :" + currentLevelIndex);
        }
    #endregion

    #region Public Methods

    public void IncreaseLevel() {
        currentLevelInstance = levels[++currentLevelIndex];
        opponent.ChangeCharacteristics(currentLevelInstance.opponentDamage, currentLevelInstance.opponentStandUpChance);
        Debug.Log("Current level :" + currentLevelIndex);
    }

    public void RefreshLevels() {
        currentLevelIndex = 0;
        currentLevelInstance = levels[currentLevelIndex];
        opponent.ChangeCharacteristics(currentLevelInstance.opponentDamage, currentLevelInstance.opponentStandUpChance);
    }

    public bool IsThereMoreLevels() {
        return (levels.Count - 1) - currentLevelIndex > 0;
    }
        
    #endregion

    #region Private Methods
        
    #endregion
}
