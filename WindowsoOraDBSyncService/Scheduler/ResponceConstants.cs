namespace OraDBSyncService.Scheduler
{
    internal static class ResponceConstants
    {
        internal static readonly string CreateSuccess = "Успешное добавление задачи синхронизации.\r\n";
        internal static readonly string DeleteSuccess = "Успешное удаление задачи синхронизации.\r\n";
        internal static readonly string ReplaceSuccess = "Успешная замена задачи синхронизации.\r\n";
        internal static readonly string ExecuteSuccess = "Немедленный запуск задачи синхронизации.\r\n";
        internal static readonly string CheckSuccess = "Информация о задаче синхронизации.\r\n";
        internal static readonly string InterruptSuccess = "Прерывание задачи синхронизации.\r\n";

        internal static readonly string CreateFail = "Неудачное добавление задачи синхронизации.\r\n";
        internal static readonly string CreateFailExists = "Задача синхронизации уже существует.\r\n";
        internal static readonly string DeleteFail = "Неудачное удаление задачи синхронизации.\r\n";
        internal static readonly string ReplaceFail = "Неудачная замена задачи синхронизации.\r\n";
        internal static readonly string ExecuteFail = "Неудачный запуск задачи синхронизации.\r\n";
        internal static readonly string CheckFail = "Задачи синхронизации не существует.\r\n";
        internal static readonly string InterruptFail = "Не удалось прервать выполнение задачи.\r\n";
    }
}
