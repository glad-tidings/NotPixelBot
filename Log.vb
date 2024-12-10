Public Class Log
    Public Shared Sub Show(Game As String, Account As String, Message As String, Color As ConsoleColor)
        Console.ForegroundColor = ConsoleColor.White
        Console.Write($"[{Date.Now.ToString("yyyy-MM-dd HH:mm:ss")}] ")
        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.Write($"[{Game}] ")
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.Write($"[{Account}] ")
        Console.ForegroundColor = Color
        Console.WriteLine(Message)
        Console.ResetColor()
    End Sub
End Class
