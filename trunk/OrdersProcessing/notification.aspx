<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="GCheckout.Util" %>
<%@ Import Namespace="Orders" %>

<script runat="server" language="c#">

    public string SerialNumber;
  
    public void Page_Load(Object sender, EventArgs e) 
      {
        // Extract the XML from the request.
        string RequestXml = EncodeHelper.Utf8StreamToString(Request.InputStream);
        // Write the XML to file.
        string FileName = string.Format("{0}--{1}--{2}.xml",
          EncodeHelper.GetTopElement(RequestXml),
          EncodeHelper.GetElementValue(RequestXml, "google-order-number"),
          EncodeHelper.GetElementValue(RequestXml, "serial-number"));
        string FullFileName = Server.MapPath("~/ordersprocessing/InboxDir/") + FileName;
        StreamWriter OutputFile = new StreamWriter(FullFileName, false, Encoding.UTF8);
        OutputFile.Write(RequestXml);
        OutputFile.Close();
        SerialNumber = Orders.ProcessNotification(FullFileName);
      }

</script>
<?xml version="1.0" encoding="UTF-8"?>
<notification-acknowledgment xmlns="http://checkout.google.com/schema/2" serial-number="<%=SerialNumber %>" />
