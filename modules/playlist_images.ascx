<%@ Control Language="VB" Inherits="BaseUserControl"%>

<script runat="server">
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim sAppPath As String = Me.AppPath
        litPlayer.Text = "<scr" & "ipt type=""text/javascript"" src=""" & sAppPath & "systems/media/swfobject.js""></scr" & "ipt>" & vbCrLf & _
            "<p id=""" & litPlayer.ClientID & "abc""><a href=""http" & "://www.macromedia.com/go/getflashplayer"">Get the Flash Player</a> to see this player.</p>" & vbCrLf & _
            "<script type=""text/javascript"">" & vbCrLf & _
            "var s3 = new SWFObject(""" & sAppPath & "systems/media/imagerotator.swf"", ""playlist"", ""550"", ""400"", ""7"");" & vbCrLf & _
            "s3.addVariable(""file"",""" & sAppPath & "systems/listing_xspf.aspx?pg=" & Me.ModuleData & """);" & vbCrLf & _
            "s3.addVariable(""transition"",""random"");" & vbCrLf & _
            "s3.addVariable(""backcolor"",""0x00000"");" & vbCrLf & _
            "s3.addVariable(""frontcolor"",""0xD9CFE7"");" & vbCrLf & _
            "s3.addVariable(""lightcolor"",""0xB5A0D3"");" & vbCrLf & _
            "s3.addVariable(""shownavigation"",""true"");" & vbCrLf & _
            "s3.addVariable(""overstretch"",""none"");" & vbCrLf & _
            "s3.addVariable(""linktarget"",""_self"");" & vbCrLf & _
            "s3.addVariable(""width"",""550"");" & vbCrLf & _
            "s3.addVariable(""height"",""400"");" & vbCrLf & _
            "s3.write(""" & litPlayer.ClientID & "abc"");" & vbCrLf & _
            "</scr" & "ipt>"
    End Sub
</script>

<asp:Literal ID="litPlayer" runat="server"></asp:Literal>

