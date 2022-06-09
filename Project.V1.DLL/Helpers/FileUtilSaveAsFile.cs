using Microsoft.JSInterop;

namespace Project.V1.DLL.Helpers;

public static class FileUtilSaveAsFile
{
    public static ValueTask<object> SaveAs(this IJSRuntime js, string filename, byte[] data)
       => js.InvokeAsync<object>(
           "saveAsFile",
           filename,
           Convert.ToBase64String(data));
}