<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="GCheckout.MerchantCalculation" %>
<%@ Import Namespace="GCheckout.Util" %>
<%@ Import Namespace="Orders" %>
<script runat="server" language="c#">

    void Page_Load(Object sender, EventArgs e)
    {
        try
        {
            // Extract the XML from the request.
            Stream RequestStream = Request.InputStream;
            StreamReader RequestStreamReader = new StreamReader(RequestStream);
            string RequestXml = RequestStreamReader.ReadToEnd();
            RequestStream.Close();

            Log.Debug("Request XML: " + RequestXml);

            // Process the incoming XML.
            string orderId = EncodeHelper.GetElementValue(RequestXml, "MERCHANT_DATA_HIDDEN");
            CallbackProcessor P = new CallbackProcessor(new Callback(orderId));
            byte[] ResponseXML = P.Process(RequestXml);

            Log.Debug("Response XML: " + EncodeHelper.Utf8BytesToString(ResponseXML));

            Response.BinaryWrite(ResponseXML);
        }
        catch (Exception ex)
        {
            Log.Debug(ex.ToString());
        }
    }

</script>