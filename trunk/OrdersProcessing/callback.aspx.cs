using System;
using System.IO;
using GCheckout.MerchantCalculation;
using GCheckout.Util;
using Orders;

public partial class OrdersProcessing_callback : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
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
            CallbackProcessor P = new CallbackProcessor(new Orders.Callback());
            byte[] ResponseXML = P.Process(RequestXml);

            Log.Debug("Response XML: " + EncodeHelper.Utf8BytesToString(ResponseXML));

            Response.BinaryWrite(ResponseXML);
        }
        catch (Exception ex)
        {
            Log.Debug(ex.ToString());
        }
    }
}
