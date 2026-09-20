using System;
using UnityEngine;

namespace Project.Scenario.Data
{
    /// <summary>
    /// Группа связанных по смыслу шагов (например, "Проверка документов").
    /// Шаги внутри группы выполняются строго линейно — нарушение порядка
    /// закрывает всю группу (см. GroupRuntimeIndex / ScenarioController).
    /// </summary>
    [Serializable]
    public class StepGroupData
    {
        [SerializeField] private string groupName;
        [SerializeField] private StepData[] steps;

        [Tooltip("Заведомо неправильные объекты/кнопки этой группы. " +
                 "Нужны, чтобы дистракторы давали явную ошибку (Failed), " +
                 "а не терялись как случайный шум сцены (Ignore).")]
        [SerializeField] private DistractorData[] knownDistractors;

        public string GroupName => groupName;
        public StepData[] Steps => steps;
        public DistractorData[] KnownDistractors => knownDistractors;
    }
}
