using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Text.RegularExpressions;

namespace OjtPortal.Infrastructure
{
    public static class EmailChecker
    {
        public static bool IsEmailValid(string email)
        {
            return Regex.IsMatch(email, @".*cit\.edu$") || Regex.IsMatch(email, @".*noopmail\.org$");
        }
    }
}
