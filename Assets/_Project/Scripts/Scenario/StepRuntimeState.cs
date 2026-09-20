using Project.Core;
using Project.Scenario.Data;

namespace Project.Scenario
{
    /// <summary>
    /// Runtime-состояние шага во время прохождения сценария.
    /// Отделено от StepData (ScriptableObject), чтобы данные сценария оставались
    /// неизменяемым ассетом, а не мутировались во время игры.
    /// </summary>
    public class StepRuntimeState
    {
        public readonly StepData StepData;
        public readonly int StepIndex;

        /// <summary>Индекс следующего ожидаемого действия внутри шага (для multi-action шагов).</summary>
        public int CurrentActionIndex { get; private set; }

        public StepStatus Status { get; private set; } = StepStatus.Pending;

        public StepRuntimeState(StepData stepData, int stepIndex)
        {
            StepData = stepData;
            StepIndex = stepIndex;
        }

        public bool IsLastAction => CurrentActionIndex >= StepData.Actions.Length - 1;

        public ActionData CurrentExpectedAction => StepData.Actions[CurrentActionIndex];

        public void AdvanceAction() => CurrentActionIndex++;

        public void MarkSuccess() => Status = StepStatus.Success;
        public void MarkFailed() => Status = StepStatus.Failed;
        public void MarkSkipped() => Status = StepStatus.Skipped;
    }
}
