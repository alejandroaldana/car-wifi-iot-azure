using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

class AzureIoTHub
{
    private static void CreateClient()
    {
        if (deviceClient == null)
        {
            // create Azure IoT Hub client from embedded connection string
            deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
        }
    }

    static DeviceClient deviceClient = null;

    //
    // Note: this connection string is specific to the device "w10iot02". To configure other devices,
    // see information on iothub-explorer at http://aka.ms/iothubgetstartedVSCS
    //
    const string deviceConnectionString = "{insert your connection string here}";


    //
    // To monitor messages sent to device "kraaa" use iothub-explorer as follows:
    //    iothub-explorer monitor-events --login HostName=IOT-PUE.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=aYnBCXxI8kI+akXHpQ9IiYXRdIRZA7yyn0zeyGs5R2g= "w10iot02"
    //

    // Refer to http://aka.ms/azure-iot-hub-vs-cs-2017-wiki for more information on Connected Service for Azure IoT Hub

    public static async void SendDeviceToCloudMessageAsync(int time, string method,bool movement)
    {
        try
        {
            CreateClient();
            double timeInSeconds = ((double)time) / 1000.0;
            double distanceInMeters = 0.822 * timeInSeconds;
            if (!movement) distanceInMeters = 0;
            var data = new
            {
                deviceId = "w10iot02",
                messageId = 1,
                timeMiliseconds = time,
                timeSeconds = timeInSeconds,
                distance = distanceInMeters,
                method = method,
                fecha = DateTime.Now,
                hora = DateTime.Now.Hour
            };
            
                //str = "{\"deviceId\":\"w10iot02\",\"messageId\":1,\"timeMiliseconds\":" + time + ",\"timeSeconds\":\"" + timeInSeconds + "\",\"distance\":\"" + distanceInMeters + "\",\"method\":\"" + method + "\",\"fecha\":\"" + DateTime.Today.ToString() + "\",\"hora\":" + DateTime.Now.Hour + "}";


            var ms = new
            {
                deviceId = "w10iot02",
                messageId = 1,
                time = time,
                method = method

            };
            string str = JsonConvert.SerializeObject(data);
            var message = new Message(Encoding.ASCII.GetBytes(str));
            message.Properties.Add("deviceId", "w10iot02");
            await deviceClient.SendEventAsync(message);
            Debug.WriteLine("Envio");
        }
        catch (Exception e)
        {
            Debug.WriteLine("Error: " + e.Message);
        }

    }

    public static async Task<string> ReceiveCloudToDeviceMessageAsync()
    {
        CreateClient();

        while (true)
        {
            var receivedMessage = await deviceClient.ReceiveAsync();

            if (receivedMessage != null)
            {
                var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                await deviceClient.CompleteAsync(receivedMessage);
                return messageData;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }



    public static async Task GetDeviceTwinAsync()
    {
        CreateClient();

        Console.WriteLine("Getting device twin");
        Twin twin = await deviceClient.GetTwinAsync();
        Console.WriteLine(twin.ToJson());
    }




    public static async Task UpdateDeviceTwin()
    {
        CreateClient();

        TwinCollection tc = new TwinCollection();
        tc["SampleProperty1"] = "test value";

        Console.WriteLine("Updating Device Twin reported properties");
        await deviceClient.UpdateReportedPropertiesAsync(tc);
    }
}
