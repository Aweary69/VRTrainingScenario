using UnityEngine;

namespace Project.Scenario.Data
{
    /// <summary>
    /// Корневой asset сценария тренировки. Полностью описывает прохождение как данные:
    /// новый сценарий создаётся через "Create Asset" в редакторе, без правки кода
    /// (см. README, раздел "Гибкость системы сценариев").
    /// </summary>
    [CreateAssetMenu(fileName = "NewScenario", menuName = "Project/Scenario Data")]
    public class ScenarioData : ScriptableObject
    {
        [SerializeField] private StepGroupData[] groups;

        public StepGroupData[] Groups => groups;
    }
}
