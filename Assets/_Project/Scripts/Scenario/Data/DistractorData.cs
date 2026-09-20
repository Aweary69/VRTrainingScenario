using System;
using Project.Core;
using UnityEngine;

namespace Project.Scenario.Data
{
    /// <summary>
    /// Заведомо "неправильное" действие в рамках группы (ложная кнопка, не тот документ,
    /// не та зона), которое дизайнер сценария расставляет намеренно, чтобы проверить
    /// внимательность игрока.
    ///
    /// Вынесено отдельным сериализуемым типом (а не просто ActionKey), потому что
    /// ActionKey — readonly struct и не редактируется в инспекторе Unity напрямую.
    /// </summary>
    [Serializable]
    public class DistractorData
    {
        [SerializeField] private ActionType actionType;
        [SerializeField] private string targetId;

        public ActionType ActionType => actionType;
        public string TargetId => targetId;

        public ActionKey ToKey() => new ActionKey(actionType, targetId);
    }
}
