Imports System
Imports System.IO
Imports System.Text.Json
Imports System.Threading

Module Program
    Private proxies As Proxy()

    Sub Main()
        Console.WriteLine("  _   _       _   ____  _          _ ____   ___ _____ 
 | \ | | ___ | |_|  _ \(_)_  _____| | __ ) / _ \_   _|
 |  \| |/ _ \| __| |_) | \ \/ / _ \ |  _ \| | | || |  
 | |\  | (_) | |_|  __/| |>  <  __/ | |_) | |_| || |  
 |_| \_|\___/ \__|_|   |_/_/\_\___|_|____/ \___/ |_|  
                                                      ")
        Console.WriteLine()
        Console.WriteLine("Github: https://github.com/glad-tidings/NotPixelBot")
        Console.WriteLine()
mainMenu:
        Console.Write("Select an option:
1. Run bot
2. Create session
>> ")
        Dim opt As String = Console.ReadLine()

        Dim queries As NotPixelQuery()
        Dim jsonConfig As String = ""
        Dim jsonProxy As String = ""
        Try
            jsonConfig = File.ReadAllText("data.txt")
        Catch ex As Exception
            Console.WriteLine("file 'data.txt' not found")
            GoTo Get_Error
        End Try
        Try
            jsonProxy = File.ReadAllText("proxy.txt")
        Catch ex As Exception
            Console.WriteLine("file 'proxy.txt' not found")
            GoTo Get_Error
        End Try
        Try
            queries = JsonSerializer.Deserialize(Of NotPixelQuery())(jsonConfig)
            proxies = JsonSerializer.Deserialize(Of Proxy())(jsonConfig)
        Catch ex As Exception
            Console.WriteLine("configuration is wrong")
            GoTo Get_Error
        End Try

        If Not String.IsNullOrEmpty(opt) Then
            Select Case opt
                Case "1"
                    Dim NotPixel As New Thread(
                        Sub()
                            For Each Query In queries.Where(Function(x) x.Active)
                                Dim BotThread As New Thread(Sub() NotPixelThread(Query))
                                BotThread.Start()

                                Thread.Sleep(120000)
                            Next
                        End Sub)
                    NotPixel.Start()
                Case "2"
                    For Each Query In queries
                        If Not File.Exists($"sessions\{Query.Name}.session") Then
                            Console.WriteLine()
                            Console.WriteLine($"Create session for account {Query.Name} ({Query.Phone})")
                            Dim vw As TelegramMiniApp.WebView = New TelegramMiniApp.WebView(Query.API_ID, Query.API_HASH, Query.Name, Query.Phone, "", "")
                            If vw.Save_Session().Result Then
                                Console.WriteLine("Session created")
                            Else
                                Console.WriteLine("Create session failed")
                            End If
                        End If
                    Next

                    Environment.Exit(0)
                Case Else
                    GoTo mainMenu
            End Select
        Else
            GoTo mainMenu
        End If

Get_Error:
        Console.ReadLine()
    End Sub

    Public Async Sub NotPixelThread(Query As NotPixelQuery)
        While True
            Dim RND As New Random()
            Try
                Dim Bot As New NotPixelBot(Query, proxies)
                If Not Bot.HasError Then
                    Log.Show("NotPixel", Query.Name, $"my ip '{Bot.IPAddress}'", ConsoleColor.White)
                    Dim Sync = Await Bot.NotPixelSyncAsync()
                    If Sync IsNot Nothing Then
                        Log.Show("NotPixel", Query.Name, $"synced successfully. B<{Int(Sync.UserBalance)}> C<{Sync.Charges}>", ConsoleColor.Blue)

                        If Query.Farming Then
                            Thread.Sleep(3000)
                            Dim Claim = Await Bot.NotPixelClaimAsync()
                            If Claim IsNot Nothing Then
                                Log.Show("NotPixel", Query.Name, $"coin claimed successfully.", ConsoleColor.Green)
                            Else
                                Log.Show("NotPixel", Query.Name, $"claim coin failed", ConsoleColor.Red)
                            End If
                        End If

                        If Query.Paint And Sync.Charges > 0 Then
                            For I As Integer = 1 To IIf(Sync.Charges > 10, 10, Sync.Charges)
                                Dim eachpaintRND As Integer = RND.Next(Query.PaintSleep(0), Query.PaintSleep(1))
                                Thread.Sleep(eachpaintRND * 1000)

                                Dim Paint = Await Bot.NotPixelPaintAsync()
                                If Paint IsNot Nothing Then
                                    Log.Show("NotPixel", Query.Name, $"{I}/{IIf(Sync.Charges > 10, 10, Sync.Charges)} paint successfully.", ConsoleColor.Green)
                                Else
                                    Log.Show("NotPixel", Query.Name, $"{I}/{IIf(Sync.Charges > 10, 10, Sync.Charges)} paint failed", ConsoleColor.Red)
                                End If
                            Next
                        End If

                        If Query.Upgrade Then
                            Dim eachupgradeRND As Integer = RND.Next(Query.UpgradeSleep(0), Query.UpgradeSleep(1))
                            Thread.Sleep(eachupgradeRND * 1000)

                            Dim Reward = Await Bot.NotPixelUpgradeRewardAsync()
                            If Reward = 2 Then
                                Log.Show("NotPixel", Query.Name, $"reward upgraded successfully.", ConsoleColor.Green)
                            ElseIf Reward = 0 Then
                                Log.Show("NotPixel", Query.Name, $"reward upgrade failed", ConsoleColor.Red)
                            End If

                            eachupgradeRND = RND.Next(Query.UpgradeSleep(0), Query.UpgradeSleep(1))
                            Thread.Sleep(eachupgradeRND * 1000)

                            Dim Speed = Await Bot.NotPixelUpgradeSpeedAsync()
                            If Speed = 2 Then
                                Log.Show("NotPixel", Query.Name, $"speed upgraded successfully.", ConsoleColor.Green)
                            ElseIf Speed = 0 Then
                                Log.Show("NotPixel", Query.Name, $"speed upgrade failed", ConsoleColor.Red)
                            End If

                            eachupgradeRND = RND.Next(Query.UpgradeSleep(0), Query.UpgradeSleep(1))
                            Thread.Sleep(eachupgradeRND * 1000)

                            Dim Limit = Await Bot.NotPixelUpgradeLimitAsync()
                            If Limit = 2 Then
                                Log.Show("NotPixel", Query.Name, $"limit upgraded successfully.", ConsoleColor.Green)
                            ElseIf Limit = 0 Then
                                Log.Show("NotPixel", Query.Name, $"limit upgrade failed", ConsoleColor.Red)
                            End If
                        End If

                        If Query.Secret Then
                            Dim secrets = Await Bot.NotPixelGetSecretAsync()
                            If secrets IsNot Nothing Then
                                For Each secret In secrets.Secrets
                                    Dim secEx As Boolean = False
                                    If Sync.Quests IsNot Nothing Then
                                        If Sync.Quests.Where(Function(x) x.Key = "secretWord:" & secret And x.Value = True).Count = 0 Then secEx = True
                                    Else
                                        secEx = True
                                    End If
                                    If secEx Then
                                        Dim setSecret = Await Bot.NotPixelSecretAsync(secret)
                                        If setSecret Then
                                            Log.Show("NotPixel", Query.Name, $"quest claimed successfully.", ConsoleColor.Green)
                                        Else
                                            Log.Show("NotPixel", Query.Name, $"claim quest failed", ConsoleColor.Red)
                                        End If

                                        Thread.Sleep(3000)
                                    End If
                                Next
                            End If
                        End If
                    Else
                        Log.Show("NotPixel", Query.Name, $"synced failed", ConsoleColor.Red)
                    End If

                    Sync = Await Bot.NotPixelSyncAsync()
                    If Sync IsNot Nothing Then
                        Log.Show("NotPixel", Query.Name, $"B<{Int(Sync.UserBalance)}> C<{Sync.Charges}>", ConsoleColor.Blue)
                    End If
                Else
                    Log.Show("NotPixel", Query.Name, $"{Bot.ErrorMessage}", ConsoleColor.Red)
                End If
            Catch ex As Exception
                Log.Show("NotPixel", Query.Name, $"Error: {ex.Message}", ConsoleColor.Red)
            End Try

            Dim syncRND As Integer = 0
            If Date.Now.Hour < 8 Then
                syncRND = RND.Next(Query.NightSleep(0), Query.NightSleep(1))
            Else
                syncRND = RND.Next(Query.DaySleep(0), Query.DaySleep(1))
            End If
            Log.Show("NotPixel", Query.Name, $"sync sleep '{Int(syncRND / 3600)}h {Int((syncRND Mod 3600) / 60)}m {syncRND Mod 60}s'", ConsoleColor.Yellow)
            Thread.Sleep(syncRND * 1000)
        End While
    End Sub
End Module
