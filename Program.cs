// See https://aka.ms/new-console-template for more information
using Figgle;
using System.Diagnostics;

Console.WriteLine(FiggleFonts.Standard.Render("Soda.Clear!"));

var (total, space) = DiskHelper.GetSpace();

WriteInfo($"+-----------------------------------------+");
WriteInfo($"|\t\tC盘清理工具\t\t  |");
WriteInfo($"|总容量:\t{total:0.00}GB\t\t\t  |");
WriteInfo($"|可用空间:\t{space:0.00}GB\t\t\t  |");
WriteInfo($"+-----------------------------------------+");

Process process = new Process();
process.StartInfo.FileName = "cmd.exe";
process.StartInfo.UseShellExecute = false;
process.StartInfo.CreateNoWindow = true;
process.StartInfo.RedirectStandardInput = true;
process.StartInfo.RedirectStandardOutput = true;
process.Start();

process.StandardInput.WriteLine(Bats.Clear);

char[] animation = new char[] { '|', '/', '-', '\\' };
int counter = 0;
while (!process.StandardOutput.EndOfStream)
{
    string line = process.StandardOutput.ReadLine() ?? string.Empty;

    if (line.Contains(Bats.EndFlag)) break;

    Console.SetCursorPosition(0, Console.CursorTop);
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("SODA-CLEAR:\t");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write($"[{animation[counter % 4]}]");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"正在清理...[{counter % 100}%] {(line.Length < Console.WindowWidth / 2 ? line : line[..(Console.WindowWidth / 2)])}");
    counter++;
    Thread.Sleep(20);
}

var (_, newSpace) = DiskHelper.GetSpace();

Console.WriteLine();
WriteInfo($"已清理空间 [{(newSpace - space):0.00}] GB");

process.WaitForExit();
process.Close();

void WriteInfo(string info)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("SODA-CLEAR:\t");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"{info}");
}

internal class Bats
{
    public const string EndFlag = "the soda clear end";

    public const string Clear = @$"
del /f /s /q %systemdrive%\*.tmp &
del /f /s /q %systemdrive%\*._mp &
del /f /s /q %systemdrive%\*.log &
del /f /s /q %systemdrive%\*.gid &
del /f /s /q %systemdrive%\*.chk &
del /f /s /q %systemdrive%\*.old &
del /f /s /q %systemdrive%\recycled\*.* &
del /f /s /q %windir%\*.bak &
del /f /s /q %windir%\prefetch\*.* &
rd /s /q %windir%\temp & md %windir%\temp &
del /f /q %userprofile%\cookies\*.* &
del /f /q %userprofile%\recent\*.* &
del /f /s /q ""%userprofile%\Local Settings\Temporary Internet Files\*.*"" &
del /f /s /q ""%userprofile%\Local Settings\Temp\*.*"" &
del /f /s /q ""%userprofile%\recent\*.*"" &
echo {EndFlag}
";
}

internal class DiskHelper
{
    public static (decimal, decimal) GetSpace()
    {
        // 获取所有逻辑驱动器
        DriveInfo[] drives = DriveInfo.GetDrives();
        foreach (DriveInfo drive in drives)
        {
            // 如果是C盘
            if (drive.Name == "C:\\")
            {
                return (drive.TotalSize / 1024M / 1024M / 1024M, drive.AvailableFreeSpace / 1024M / 1024M / 1024M);
            }
        }
        return (0, 0);
    }
}