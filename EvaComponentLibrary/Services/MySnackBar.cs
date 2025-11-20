using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor;
using EvaComponentLibrary.Languages;
using Microsoft.Extensions.Localization;
using MudBlazor;
namespace EvaComponentLibrary.Services
{
    public class MySnackBar(ISnackbar Snackbar, IStringLocalizer<MyStrings> Locals)
    {
       
        public void SnackBarManager(string result)
        {
            switch (result)
    {
                case "Error" or null or "":
                    PoPSnackBar(Locals["_genError"], Severity.Error);
                    break;
    
                case "Unauthorized":
                    PoPSnackBar(Locals["_unauthorized"], Severity.Error);
                    break;

                case "OK":
                    PoPSnackBar(Locals["_addSucc"], Severity.Success);
                    break;

                case "conflict":
                    PoPSnackBar(Locals["_syncEmail"], Severity.Warning);
                    break;
                default:
                    PoPSnackBar(Locals["_genError"], Severity.Error);
                    break;
            }
        }
        public void PoPSnackBar(string message, Severity color, SnackbarDuplicatesBehavior duplic = SnackbarDuplicatesBehavior.Prevent, string additionalText = "")
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(string.Format(Locals[message],additionalText), color, config =>
            {
                config.VisibleStateDuration = 1500;
                config.SnackbarVariant = Variant.Filled;
                config.CloseAfterNavigation = true;
                config.ShowCloseIcon = false;
                config.DuplicatesBehavior = duplic;
            });
        }
    }

}
