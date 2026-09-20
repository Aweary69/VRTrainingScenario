using System.Collections.Generic;
using Project.Scenario.Data;

namespace Project.Scenario
{
    /// <summary>
    /// Runtime-состояние одной группы шагов во время прохождения.
    /// Хранит список StepRuntimeState и построенный на их основе индекс поиска
    /// (GroupRuntimeIndex), который живёт ровно на время активности группы.
    /// </summary>
    public class GroupRuntimeState
    {
        public readonly StepGroupData GroupData;
        public readonly int GroupIndex;
        public readonly IReadOnlyList<StepRuntimeState> Steps;
        public readonly GroupRuntimeIndex Index;

        public GroupRuntimeState(StepGroupData groupData, int groupIndex)
        {
            GroupData = groupData;
            GroupIndex = groupIndex;

            var steps = new List<StepRuntimeState>(groupData.Steps.Length);
            for (int i = 0; i < groupData.Steps.Length; i++)
            {
                steps.Add(new StepRuntimeState(groupData.Steps[i], i));
            }
            Steps = steps;

            Index = new GroupRuntimeIndex(groupData);
        }

        /// <summary>Текущий незавершённый шаг группы, либо null, если все шаги завершены/пропущены.</summary>
        public StepRuntimeState CurrentStep
        {
            get
            {
                foreach (var step in Steps)
                {
                    if (step.Status == StepStatus.Pending)
                        return step;
                }
                return null;
            }
        }

        /// <summary>Помечает все ещё не завершённые шаги как Skipped (используется при SequenceBroken).</summary>
        public void SkipRemainingSteps()
        {
            foreach (var step in Steps)
            {
                if (step.Status == StepStatus.Pending)
                    step.MarkSkipped();
            }
        }
    }
}
