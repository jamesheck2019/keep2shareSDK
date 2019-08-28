# keep2shareSDK
.NET API Library for k2s.cc / keep2share.cc


`Download:`
[https://github.com/jamesheck2019/keep2shareSDK/releases](https://github.com/jamesheck2019/keep2shareSDK/releases)<br>
[![NuGet version (BlackBeltCoder.Silk)](https://img.shields.io/nuget/v/DeQmaTech.keep2shareSDK.svg?style=plastic)](https://www.nuget.org/packages/DeQmaTech.keep2shareSDK/)<br>
`Help:`
[https://github.com/jamesheck2019/keep2shareSDK/wiki](https://github.com/jamesheck2019/keep2shareSDK/wiki)<br>

# Functions:
[https://github.com/jamesheck2019/keep2shareSDK/blob/master/IClient.cs](https://github.com/jamesheck2019/keep2shareSDK/blob/master/IClient.cs)

# Example:
```vb.net
Dim tkn = Await keep2shareSDK.GetToken.Get_Token(TextBox2.Text, TextBox1.Text)
Dim client As keep2shareSDK.IClient = New keep2shareSDK.KClient("xxxxxxx")
Dim rslt = Await client.AccountInfo
Dim rslt = Await client.ListFiles(foldrID, FleORdir, 10, 0, New keep2shareSDK.KClient.RootListOptions.SortOptions With {.name = keep2shareSDK.KClient.SoRt.ascending}, False)
```
