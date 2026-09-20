namespace Project.Scenario
{
    /// <summary>
    /// Статус шага на момент окончания сценария/группы (см. ТЗ: "выполнен с ошибкой/без",
    /// "пропущенный").
    /// </summary>
    public enum StepStatus
    {
        Pending,   // ещё не начат
        Success,   // выполнен верно
        Failed,    // выполнен с ошибкой (Mismatch)
        Skipped    // не был завершён из-за SequenceBroken в группе
    }
}
