namespace EmployeeApplicationService.Utilities
{
    public static class ValidationUtility
    {
        public static bool IsValidText(string? value) {  return string.IsNullOrEmpty(value); }
    }
}
