using System;
using UnityEngine;

namespace Project.Scenario.Data
{
    /// <summary>
    /// Один шаг сценария. Может содержать одно или несколько ожидаемых действий
    /// (см. ТЗ: "подойти в подсвеченную зону и нажать на кнопку" — два действия
    /// в рамках одного шага, выполняются строго по порядку).
    /// </summary>
    [Serializable]
    public class StepData
    {
        [SerializeField] private int id;
        [SerializeField, TextArea] private string description;
        [SerializeField] private ActionData[] actions;

        public int Id => id;
        public string Description => description;
        public ActionData[] Actions => actions;
    }
}
