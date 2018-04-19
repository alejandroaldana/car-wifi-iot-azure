
# Car-Wifi Robot C#  (PUEDAY - 2018)
Este es un proyecto para aprender a usar el entorno IoT de Microsoft, con una aplicación UWP Background (preparadas para soluciones IoT).
La idea es controlar un robot a través de una conexión WIFI con una app de Android. Además la aplicación envía datos del movimiento a Azure para graficarlo en Power BI.

El material usado es:
- Una Raspberry Pi 3.
- Windows 10 IoT Core [(Instalar Windows 10 Iot Core en Raspberry Pi 3)](https://www.windowscentral.com/how-install-windows-10-iot-raspberry-pi-3).
- [Un kit de robot Kuman SM9](https://www.amazon.es/Kuman-Inteligente-Profesional-Electrónicos-Controlado/dp/B071J21R3D).
- Android app aportada por el fabricante de Kuman SM9 para controlar el robot.
- [Azure IoT Hub](https://azure.microsoft.com/es-es/services/iot-hub/).
- [Azure Stream Analytics](https://azure.microsoft.com/es-es/services/stream-analytics/).
- [Power BI Online](https://powerbi.microsoft.com/es-es/)
- [Aplicación de Streaming en una UWP APP](https://github.com/SaschaIoT/HttpWebcamLiveStream).

## Esquema de proyecto

![Esquema de proyecto](http://www.bgait.com/img/desarrollo/esquemaproyecto.png)
## Código aportado

El código aportado es una aplicación Universal Windows Platform en Background. El funcionamiento se divide en dos partes. 

Por un lado encontramos el motor del coche, el cual es una clase (Motor()) que se compone de los pines necesarios para mover el robot. Las funcionalidades son mover hacia delante, mover hacia atrás, mover derecha y mover izquierda. 

La otra parte es el receptor (Server()), el cual recibe las peticiones de la aplicación de Android para saber a donde dirigir el robot.
El receptor tiene dos funciones:
1. StartListening(): Se encarga de mantener una socket abierto de peticiones TCP. Una vez se conecta un dispositivo, procesa la información que le llega.
2. Communication_Decode(): Una vez se ha encontrado una cadena en StartListening(), se decodifica para conocer el movimiento que se desea realizar.

El motor del coche envía a un Azure IoT Hub el tiempo que se ha movido en una dirección. Para esto se ha añadido la conexión a Azure a través de 
AzureIoTHub.cs. Se necesita añadir la cadena de conexión al dispositivo de Azure. Si no se desea conectar con Azure comentar las líneas de Motor.cs
```
AzureIoTHub.SendDeviceToCloudMessageAsync(...);
```
## Conexión a Azure
![Conexión a Azure](https://docs.microsoft.com/en-us/azure/iot-hub/media/iot-hub-get-started-e2e-diagram/4.png).
Si se desea conectar con **AzureIoTHub**, aquí os dejo [un tutorial de Microsoft para realizar esta acción](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-getstarted). Una vez tengamos nuestro Hub montado y en funcionamiento necesitaremos agregar la cadena de conexión en el archivo AzureIoTHub.cs:
```
const string deviceConnectionString = "{insert your connection string here}";
```
En este proyecto se realiza una conexión con Azure IoT Hub para transmitir la telemetría del robot en tiempo real a través de **Azure Stream Analytics**. Stream Analytics lo envía a Power BI donde podremos analizar la información y obtener informes personalizados en un cuadro de mando.  
![Cuadro de mando](http://www.bgait.com/img/desarrollo/cuadrodemandocoche.png)

## Instalación 
El despliegue de la aplicación en el robot se puede realizar a través de Visual Studio 2017, conectando la raspberry pi 3 a la misma red que el ordenador que la depura.
Se debe esperar a que los tres  leds azules de la placa del motor del robot se apaguen para poder conectarnos a través de la aplicación Android. 

El dispositivo Android y el coche robot deben estar conectados a la misma red Wifi. Una vez el coche está conectado a la red, obtenemos su IP y la introducimos en el menú de configuración de la app de Android. 
Nos conectamos y podemos empezar a mover el coche. 
