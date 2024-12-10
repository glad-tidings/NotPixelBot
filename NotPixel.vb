Imports System.Text.Json.Serialization

Public Class NotPixelQuery
    Public Property Index As Long
    Public Property Name As String
    Public Property API_ID As String
    Public Property API_HASH As String
    Public Property Phone As String
    Public Property Auth As String
    Public Property Active As Boolean
    Public Property Farming As Boolean
    Public Property Paint As Boolean
    Public Property PaintSleep As Integer()
    Public Property Upgrade As Boolean
    Public Property UpgradeSleep As Integer()
    Public Property Secret As Boolean
    Public Property DaySleep As Integer()
    Public Property NightSleep As Integer()
End Class

Public Class NotPixelUserDetailResponse
    <JsonPropertyName("id")>
    Public Property Id As Long
    <JsonPropertyName("firstName")>
    Public Property FirstName As String
    <JsonPropertyName("lastName")>
    Public Property LastName As String
    <JsonPropertyName("balance")>
    Public Property balance As Double
    <JsonPropertyName("repaints")>
    Public Property repaints As Double
    <JsonPropertyName("league")>
    Public Property League As String
End Class

Public Class NotPixelStatusResponse
    <JsonPropertyName("coins")>
    Public Property Coins As Double
    <JsonPropertyName("speedPerSecond")>
    Public Property SpeedPerSecond As Double
    <JsonPropertyName("claimed")>
    Public Property Claimed As Double
    <JsonPropertyName("boosts")>
    Public Property Boosts As NotPixelStatusBoosts
    <JsonPropertyName("repaintsTotal")>
    Public Property RepaintsTotal As Double
    <JsonPropertyName("userBalance")>
    Public Property UserBalance As Double
    <JsonPropertyName("league")>
    Public Property League As String
    <JsonPropertyName("charges")>
    Public Property Charges As Integer
    <JsonPropertyName("quests")>
    Public Property Quests As Dictionary(Of String, Boolean)
End Class

Public Class NotPixelStatusBoosts
    <JsonPropertyName("energyLimit")>
    Public Property EnergyLimit As Integer
    <JsonPropertyName("paintReward")>
    Public Property PaintReward As Integer
    <JsonPropertyName("reChargeSpeed")>
    Public Property ReChargeSpeed As Integer
End Class

Public Class NotPixelPaintRequest
    <JsonPropertyName("pixelId")>
    Public Property PixelId As Integer
    <JsonPropertyName("newColor")>
    Public Property NewColor As String
End Class

Public Class NotPixelPaintResponse
    <JsonPropertyName("balance")>
    Public Property balance As Double
End Class

Public Class NotPixelPaintRewardResponse
    <JsonPropertyName("paintReward")>
    Public Property PaintReward As Boolean
End Class

Public Class NotPixelReChargeSpeedResponse
    <JsonPropertyName("reChargeSpeed")>
    Public Property ReChargeSpeed As Boolean
End Class

Public Class NotPixelEnergyLimitResponse
    <JsonPropertyName("energyLimit")>
    Public Property EnergyLimit As Boolean
End Class

Public Class NotPixel3XPoint
    <JsonPropertyName("data")>
    Public Property Data As List(Of NotPixel3XPointData)
End Class

Public Class NotPixel3XPointData
    <JsonPropertyName("cordinates")>
    Public Property Cordinates As List(Of NotPixel3XPointDataCor)
    <JsonPropertyName("color")>
    Public Property Color As String
End Class

Public Class NotPixel3XPointDataCor
    <JsonPropertyName("start")>
    Public Property [Start] As List(Of Integer)
    <JsonPropertyName("end")>
    Public Property [End] As List(Of Integer)
End Class

Public Class NotPixelSecretRequest
    <JsonPropertyName("secret_word")>
    Public Property SecretWord As String
End Class

Public Class NotPixelSecretResponse
    <JsonPropertyName("secretWord")>
    Public Property SecretWord As NotPixelSecretSecretWord
End Class

Public Class NotPixelSecretSecretWord
    <JsonPropertyName("success")>
    Public Property Success As Boolean
    <JsonPropertyName("reward")>
    Public Property Reward As Integer
End Class

Public Class NotPixelGetSecretResponse
    <JsonPropertyName("secrets")>
    Public Property Secrets As List(Of String)
End Class

Public Class Proxy
    Public Property Index As Integer
    Public Property Proxy As String
End Class

Public Class httpbin
    <JsonPropertyName("origin")>
    Public Property Origin As String
End Class