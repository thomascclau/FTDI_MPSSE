#define FT232H                // Enable only one of these defines depending on your device type
//#define FT2232H
//#define FT4232H



// Definition

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FTD2XX_NET;
using System.Threading;
using System.Diagnostics;

namespace KeypadLED
{
    public partial class FormMain : Form
    {
        //###################################################################################################################################
        //###################################################################################################################################
        //##################                                      Definitions                                           #####################
        //###################################################################################################################################
        //###################################################################################################################################

        // ###### Driver defines ######
        FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

        // ###### I2C Library defines ######
        const byte I2C_Dir_SDAin_SCLin = 0x00;
        const byte I2C_Dir_SDAin_SCLout = 0x01;
        const byte I2C_Dir_SDAout_SCLout = 0x03;
        const byte I2C_Dir_SDAout_SCLin = 0x02;
        const byte I2C_Data_SDAhi_SCLhi = 0x03;
        const byte I2C_Data_SDAlo_SCLhi = 0x01;
        const byte I2C_Data_SDAlo_SCLlo = 0x00;
        const byte I2C_Data_SDAhi_SCLlo = 0x02;
        // MPSSE clocking commands
        const byte MSB_FALLING_EDGE_CLOCK_BYTE_IN = 0x24;
        const byte MSB_RISING_EDGE_CLOCK_BYTE_IN = 0x20;
        const byte MSB_FALLING_EDGE_CLOCK_BYTE_OUT = 0x11;
        const byte MSB_DOWN_EDGE_CLOCK_BIT_IN = 0x26;
        const byte MSB_UP_EDGE_CLOCK_BYTE_IN = 0x20;
        const byte MSB_UP_EDGE_CLOCK_BYTE_OUT = 0x10;
        const byte MSB_RISING_EDGE_CLOCK_BIT_IN = 0x22;
        const byte MSB_FALLING_EDGE_CLOCK_BIT_OUT = 0x13;
        // Clock
        const uint ClockDivisor = 49;      //          = 199;// for 100KHz
        // Sending and receiving
        static uint NumBytesToSend = 0;
        static uint NumBytesToRead = 0;
        uint NumBytesSent = 0;
        static uint NumBytesRead = 0;
        static byte[] MPSSEbuffer = new byte[500];
        static byte[] InputBuffer = new byte[500];   
        static byte[] InputBuffer2 = new byte[500];
        static uint BytesAvailable = 0;
        static bool I2C_Ack = false;
        static byte AppStatus = 0;
        static bool DeviceInit = false;
        static byte I2C_Status = 0;
        public bool Running = true;
        static bool DeviceOpen = false;
        // GPIO
        static byte GPIO_Low_Dat = 0;
        static byte GPIO_Low_Dir = 0;
        static byte ADbusReadVal = 0;
        static byte ACbusReadVal = 0;
               
        // registers
        public const byte REGISTER_COMMAND = 0x80;
        public const byte REGISTER_ID = 0x81;
        public const byte REGISTER_PROX_RATE = 0x82;
        public const byte REGISTER_PROX_CURRENT = 0x83;
        public const byte REGISTER_AMBI_PARAMETER = 0x84;
        public const byte REGISTER_AMBI_VALUE = 0x85;
        public const byte REGISTER_PROX_VALUE = 0x87;
        public const byte REGISTER_INTERRUPT_CONTROL = 0x89;
        public const byte REGISTER_INTERRUPT_LOW_THRES = 0x8a;
        public const byte REGISTER_INTERRUPT_HIGH_THRES = 0x8c;
        public const byte REGISTER_INTERRUPT_STATUS = 0x8e;
        public const byte REGISTER_PROX_TIMING = 0xf9;
        // Bits in the registers defined above
        public const byte COMMAND_SELFTIMED_MODE_ENABLE = 0x01;
        public const byte COMMAND_PROX_ENABLE = 0x02;
        public const byte COMMAND_AMBI_ENABLE = 0x04;
        public const byte COMMAND_MASK_PROX_DATA_READY = 0x20;
        public const byte PROX_MEASUREMENT_RATE_31 = 0x04;
        public const byte AMBI_PARA_AVERAGE_32 = 0x05; // DEFAULT
        public const byte AMBI_PARA_AUTO_OFFSET_ENABLE = 0x08; // DEFAULT enable
        public const byte AMBI_PARA_MEAS_RATE_2 = 0x10; // DEFAULT
        public const byte INTERRUPT_THRES_SEL_PROX = 0x00;
        public const byte INTERRUPT_THRES_ENABLE = 0x02;
        public const byte INTERRUPT_COUNT_EXCEED_1 = 0x00; // DEFAULT


        static bool StartScan = true;

        uint devcount = 0;

        //###################################################################################################################################
        //###################################################################################################################################
        //##################                          Main Application Layer                                            #####################
        //###################################################################################################################################
        //###################################################################################################################################
                    
        
        // Create new instance of the FTDI device class
        FTDI myFtdiDevice = new FTDI();
                
        public FormMain()
        {
            InitializeComponent();
        }


        //###################################################################################################################################
        // When the form loads...

        private void FormMain_Load(object sender, EventArgs e)
        {
            labelFTDIStatus.Text = "Closed";

            buttonClose.Enabled = true;
            buttonInit.Enabled = false;
            buttonRed.Enabled = false;
            buttonBlue.Enabled = false;
            buttonGreen.Enabled = false;

            // Print device type to remind user of type that it was compiled for
            // could auto detect based on device info instead of setting defines at top of code
#if (FT232H)
            {
                labelFTDIType.Text = "FT232H";
            }
#elif (FT2232H)
            {
                labelFTDIType.Text = "FT2232H";
            }
#elif (FT4232H)
            {
                labelFTDIType.Text = "FT4232H";
            }
#else
            {
                labelFTDIType.Text = "error - no define set";
            }
#endif
            try
            {
                ftStatus = myFtdiDevice.GetNumberOfDevices(ref devcount);
            }
            catch
            {
                labelFTDIStatus.Text = "Driver not loaded";

                buttonInit.Enabled = false;
                buttonClose.Enabled = true;
            }

            // e.g. open a UM232H Module by it's description
            //ftStatus = myFtdiDevice.OpenByDescription("UM232H");  // could replace line below
            ftStatus = myFtdiDevice.OpenByIndex(0);

            // Update the Status text line
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                DeviceOpen = true;
                labelFTDIStatus.Text = "Open";
                Debug.WriteLine(" Open FTDI");
            }
            else
            {
                DeviceOpen = false;
                labelFTDIStatus.Text = "No Device Found";
            }

            Update();
            Application.DoEvents();

            // If the device opened successfully, initialise MPSSE 
            if (DeviceOpen == true)
            {
                DeviceInit = true;

                AppStatus = I2C_ConfigureMpsse();
                if (AppStatus != 0)
                {
                    labelFTDIStatus.Text = "Failed Init";
                    DeviceInit = false;
                }

                if (DeviceInit == true)
                {
                    // allow user to start or exit
                    buttonInit.Enabled = true;
                    buttonClose.Enabled = true;

                    labelFTDIStatus.Text = "Ready";
                }
                else
                {
                    labelFTDIStatus.Text = "Failed Init";
                    myFtdiDevice.Close();
                    // allow re-init or exit
                    buttonInit.Enabled = false;
                    buttonClose.Enabled = true;
                }

                Update();
                Application.DoEvents();
            }
            else
            {
                // allow re-init or exit
                buttonInit.Enabled = false;
                buttonClose.Enabled = true;
            }
        }

        //###################################################################################################################################
        // Code for the "Initialise" button
        
        private void buttonInit_Click(object sender, EventArgs e)
        {
            if (DeviceInit == true)
            {
                AppStatus = LP55281_Config();
                if (AppStatus != 0)
                {
                    labelDUTStatus.Text = "Failed LED Driver Init";
                    DeviceInit = false;
                }
                else
                {
                    labelDUTStatus.Text = "LED Driver Initiated";
                    buttonRed.Enabled = true;
                    buttonBlue.Enabled = true;
                    buttonGreen.Enabled = true;
                }
            }
        }
        

        //###################################################################################################################################
        // Code for the "Close" button
        
        private void buttonClose_Click(object sender, EventArgs e)
        {

            Running = false;
            labelFTDIStatus.Text = "Stopping";
            Update();
            Application.DoEvents();

            // Disable LED Driver
            AppStatus = LP55281_Enable(0x4C, false);
            AppStatus = LP55281_Enable(0x4D, false);

            // Disable GPIOL0
            AppStatus = I2C_SetGPIOValuesLow(0x10, 0x00);       

            // Close the FTDI device and then close the window
            myFtdiDevice.Close();
            Thread.Sleep(1000);
            labelFTDIStatus.Text = "Closed";
            this.Close();
        }

        //###################################################################################################################################
        //###################################################################################################################################
        //##################                          Driver Layer                                                      #####################
        //###################################################################################################################################
        //###################################################################################################################################


        public byte LP55281_Reset(byte Address)
        {
            // Reset LED Driver
            AppStatus = I2C_SetStart();                                                     // I2C START
            if (AppStatus != 0) return 1;

            AppStatus = I2C_SendDeviceAddrAndCheckACK((byte)(Address), false);              // I2C ADDRESS for Controller #1
            if (AppStatus != 0) return 1;
            if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return

            AppStatus = I2C_SendByteAndCheckACK((byte)(0x60));                              // 0x60 Reset Register
            if (AppStatus != 0) return 1;
            if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return

            AppStatus = I2C_SendByteAndCheckACK((byte)(0x00));                                // Write Anything to Reset Register
            if (AppStatus != 0) return 1;
            if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return

            AppStatus = I2C_SetStop();                                                      // I2C STOP
            if (AppStatus != 0) return 1;
          
            return 0;
        }

        public byte LP55281_Enable(byte Address, bool enable)
        {
            // Enable/Disable Device
            AppStatus = I2C_SetStart();                                                     // I2C START
            if (AppStatus != 0) return 1;

            AppStatus = I2C_SendDeviceAddrAndCheckACK((byte)(Address), false);              // I2C ADDRESS 
            if (AppStatus != 0) return 1;
            if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return

            AppStatus = I2C_SendByteAndCheckACK((byte)(0x11));                              // Enables Register
            if (AppStatus != 0) return 1;
            if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return

            if (enable)
            {
                AppStatus = I2C_SendByteAndCheckACK((byte)(0xEF));                               // Enable Everything
                if (AppStatus != 0) return 1;
                if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return
            }
            else
            {
                AppStatus = I2C_SendByteAndCheckACK((byte)(0x00));                               // Disable Everything
                if (AppStatus != 0) return 1;
                if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return
            }

            AppStatus = I2C_SetStop();                                                      // I2C STOP
            if (AppStatus != 0) return 1;

            AppStatus = I2C_SetStart();                                                     // I2C START
            if (AppStatus != 0) return 1;

            return 0;
        }
        
        public byte LP55281_LED(byte Address, byte Colour, bool enable)
        {
            int Count = 0;
            int Register = 0;

            // LEDs Register Address
            //
            // Red 1    = 0x00
            // Green 1  = 0x01
            // Blue 1   = 0x02
            
            if (Colour < 3)
            {
                for (Count = 0; Count < 4; Count++)
                {
                    Register = Count * 3 + Colour;

                    AppStatus = I2C_SetStart();                                                     // I2C START
                    if (AppStatus != 0) return 1;

                    AppStatus = I2C_SendDeviceAddrAndCheckACK((byte)(Address), false);              // I2C ADDRESS (for write)
                    if (AppStatus != 0) return 1;
                    if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return

                    AppStatus = I2C_SendByteAndCheckACK((byte)(Register));                          // 0x00 Register Address
                    if (AppStatus != 0) return 1;
                    if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return

                    if (enable)
                    {
                        AppStatus = I2C_SendByteAndCheckACK((byte)(0xFF));                              // Write 0xFF to Register
                        if (AppStatus != 0) return 1;
                        if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return
                    }
                    else
                    {
                        AppStatus = I2C_SendByteAndCheckACK((byte)(0x00));                              // Write 0xFF to Register
                        if (AppStatus != 0) return 1;
                        if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return

                    }

                    AppStatus = I2C_SetStop();                                                      // I2C STOP
                    if (AppStatus != 0) return 1;
                }
            }
            return 0;
        }
                
        public byte LP55281_Config()
        {
            // Write 20 to current setting register
            // VCNL_WrSingle(REGISTER_PROX_CURRENT, 20);

            AppStatus = LP55281_Reset(0x4C);
            if (AppStatus != 0) return 1;

            AppStatus = LP55281_Reset(0x4D);
            if (AppStatus != 0) return 1;

            AppStatus = LP55281_Enable(0x4C, true);
            if (AppStatus != 0) return 1;

            AppStatus = LP55281_Enable(0x4D, true);
            if (AppStatus != 0) return 1;
            
            return 0;
 
        }
        //###################################################################################################################################

        public byte PCA9555_Read()
        {
            byte Port0;
            byte Port1;

            // Read from PCA9555
            AppStatus = I2C_SetStart();                                                     // I2C START
            if (AppStatus != 0) return 1;

            AppStatus = I2C_SendDeviceAddrAndCheckACK((byte)(0x24), false);                 // I2C ADDRESS for ADA I/O Expander for write
            if (AppStatus != 0) return 1;
            if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return

            AppStatus = I2C_SendByteAndCheckACK((byte)(0x00));                              // Input Port 0
            if (AppStatus != 0) return 1;
            if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return

            AppStatus = I2C_SetStart();                                                     // I2C RESTART
            if (AppStatus != 0) return 1;

            AppStatus = I2C_SendDeviceAddrAndCheckACK((byte)(0x24), true);                 // I2C ADDRESS for ADA I/O Expander for read
            if (AppStatus != 0) return 1;
            if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return

            AppStatus = I2C_ReadByte(true);
            if (AppStatus != 0) return 1;
            if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return

            Port0 = InputBuffer2[0];

            AppStatus = I2C_ReadByte(true);
            if (AppStatus != 0) return 1;
            if (I2C_Ack != true) { I2C_SetStop(); return 1; }                               // if NAKs then send stop and return

            Port1 = InputBuffer2[0];

            AppStatus = I2C_SetStop();                                                     // I2C STOP
            if (AppStatus != 0) return 1;

            AppStatus = I2C_SetStop();                                                     // I2C STOP
            if (AppStatus != 0) return 1;                                                  // Not sure why I have to stop it twice, but otherwise it might not stop properly



            if ((byte)(Port0 & 0x01) == 0)
            {
                buttonKey1.BackColor = System.Drawing.Color.Yellow;
            }
            else
            {
                buttonKey1.BackColor = System.Drawing.Color.Gray;
            }

            if ((byte)(Port0 & 0x02) == 0)
            {
                buttonKey2.BackColor = System.Drawing.Color.Yellow;
            }
            else
            {
                buttonKey2.BackColor = System.Drawing.Color.Gray;
            }

            if ((byte)(Port0 & 0x04) == 0)
            {
                buttonKey3.BackColor = System.Drawing.Color.Yellow;
            }
            else
            {
                buttonKey3.BackColor = System.Drawing.Color.Gray;
            }

            if ((byte)(Port0 & 0x08) == 0)
            {
                buttonKey4.BackColor = System.Drawing.Color.Yellow;
            }
            else
            {
                buttonKey4.BackColor = System.Drawing.Color.Gray;
            }


            if ((byte)(Port0 & 0x10) == 0)
            {
                buttonKey5.BackColor = System.Drawing.Color.Yellow;
            }
            else
            {
                buttonKey5.BackColor = System.Drawing.Color.Gray;
            }


            if ((byte)(Port0 & 0x20) == 0)
            {
                buttonKey6.BackColor = System.Drawing.Color.Yellow;
            }
            else
            {
                buttonKey6.BackColor = System.Drawing.Color.Gray;
            }


            if ((byte)(Port0 & 0x40) == 0)
            {
                buttonKey7.BackColor = System.Drawing.Color.Yellow;
            }
            else
            {
                buttonKey7.BackColor = System.Drawing.Color.Gray;
            }

            if ((byte)(Port0 & 0x80) == 0)
            {
                buttonKey8.BackColor = System.Drawing.Color.Yellow;
            }
            else
            {
                buttonKey8.BackColor = System.Drawing.Color.Gray;
            }

            if ((byte)(Port1 & 0x01) == 0)
            {
                buttonKey9.BackColor = System.Drawing.Color.Yellow;
            }
            else
            {
                buttonKey9.BackColor = System.Drawing.Color.Gray;
            }

            return 0;
        }







        //###################################################################################################################################
        //###################################################################################################################################
        //##################                             I2C Layer                                                      #####################
        //###################################################################################################################################
        //###################################################################################################################################


        public byte I2C_ConfigureMpsse()
        {

            byte ADbusVal = 0;
            byte ADbusDir = 0;
            NumBytesToSend = 0;

            /***** Initial device configuration *****/

            ftStatus = FTDI.FT_STATUS.FT_OK;
            ftStatus |= myFtdiDevice.SetTimeouts(5000, 5000);
            ftStatus |= myFtdiDevice.SetLatency(16);
            ftStatus |= myFtdiDevice.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, 0x00,0x00);
            ftStatus |= myFtdiDevice.SetBitMode(0x00, 0x00);
            ftStatus |= myFtdiDevice.SetBitMode(0x00, 0x02);         // MPSSE mode        

            Thread.Sleep(500);


            if (ftStatus != FTDI.FT_STATUS.FT_OK)
                return 1; // error();

            /***** Flush the buffer *****/
            I2C_Status = FlushBuffer();

            /***** Synchronize the MPSSE interface by sending bad command 0xAA *****/
            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0xAA;
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0) return 1; // error();
            NumBytesToRead = 2;
            I2C_Status = Receive_Data(2);
            if (I2C_Status !=0) return 1; //error();

            if ((InputBuffer2[0] == 0xFA) && (InputBuffer2[1] == 0xAA))
            {
                Console.WriteLine("Bad Command Echo successful");
            }
            else
            {
                return 1;            //error();
            }
            
            /***** Synchronize the MPSSE interface by sending bad command 0xAB *****/
            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0xAB;
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0) return 1; // error();
            NumBytesToRead = 2;
            I2C_Status = Receive_Data(2);
            if (I2C_Status !=0) return 1; //error();

            if ((InputBuffer2[0] == 0xFA) && (InputBuffer2[1] == 0xAB))
            {
                Console.WriteLine("Bad Command Echo successful");
            }
            else
            {
                return 1;            //error();
            }

            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0x8A; 	// Disable clock divide by 5 for 60Mhz master clock
            MPSSEbuffer[NumBytesToSend++] = 0x97;	// Turn off adaptive clocking
            MPSSEbuffer[NumBytesToSend++] = 0x8C; 	// Enable 3 phase data clock, used by I2C to allow data on both clock edges
            // The SK clock frequency can be worked out by below algorithm with divide by 5 set as off
            // SK frequency  = 60MHz /((1 +  [(1 +0xValueH*256) OR 0xValueL])*2)
            MPSSEbuffer[NumBytesToSend++] = 0x86; 	//Command to set clock divisor
            MPSSEbuffer[NumBytesToSend++] = (byte)(ClockDivisor & 0x00FF);	//Set 0xValueL of clock divisor
            MPSSEbuffer[NumBytesToSend++] = (byte)((ClockDivisor >> 8) & 0x00FF);	//Set 0xValueH of clock divisor
            MPSSEbuffer[NumBytesToSend++] = 0x85; 			// loopback off

#if (FT232H)
            MPSSEbuffer[NumBytesToSend++] = 0x9E;       //Enable the FT232H's drive-zero mode with the following enable mask...
            MPSSEbuffer[NumBytesToSend++] = 0x07;       // ... Low byte (ADx) enables - bits 0, 1 and 2 and ... 
            MPSSEbuffer[NumBytesToSend++] = 0x00;       //...High byte (ACx) enables - all off

            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLhi | (GPIO_Low_Dat & 0xF8)); // SDA and SCL both output high (open drain)
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));
#else
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));  	// SDA and SCL set low but as input to mimic open drain
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLin | (GPIO_Low_Dir & 0xF8));	//

#endif


            MPSSEbuffer[NumBytesToSend++] = 0x80; 	//Command to set directions of lower 8 pins and force value on bits set as output 
            MPSSEbuffer[NumBytesToSend++] = (byte)(ADbusVal);
            MPSSEbuffer[NumBytesToSend++] = (byte)(ADbusDir);


            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
            {
                return 1;            //error();
            }
            else
            {
                return 0;
            }
        }

        //###################################################################################################################################
        // Reads a byte over I2C 

        public byte I2C_ReadByte(bool ACK)
        {
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            NumBytesToSend = 0;
            
#if (FT232H)
            // Clock in one data byte
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BYTE_IN;      // Clock data byte in
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x00;                               // Data length of 0x0000 means 1 byte data to clock in

            // clock out one bit as ack/nak bit
            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BIT_OUT;     // Clock data bit out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                               // Length of 0 means 1 bit
            if (ACK == true)
                MPSSEbuffer[NumBytesToSend++] = 0x00;                           // Data bit to send is a '0'
            else
                MPSSEbuffer[NumBytesToSend++] = 0xFF;                           // Data bit to send is a '1'

            // I2C lines back to idle state 
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));
            MPSSEbuffer[NumBytesToSend++] = 0x80;                               //       ' Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                            //      ' Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                             //     ' Set the directions
#else          
            // Ensure line is definitely an input since FT2232H and FT4232H don't have open drain
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8)); // make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions
            // Clock one byte of data in from the sensor
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BYTE_IN;      // Clock data byte in
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x00;                               // Data length of 0x0000 means 1 byte data to clock in
            
            // Change direction back to output and clock out one bit. If ACK is true, we send bit as 0 as an acknowledge
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));    // back to output
            MPSSEbuffer[NumBytesToSend++] = 0x80;                               // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                           // set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                           // set the directions

            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BIT_OUT;    // Clock data bit out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                              // Length of 0 means 1 bit
            if (ACK == true)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x00;                          // Data bit to send is a '0'
            }
            else
            {
                MPSSEbuffer[NumBytesToSend++] = 0xFF;                          // Data bit to send is a '1'
            }

            // Put line states back to idle with SDA open drain high (set to input) 
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8));//make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                               //       ' Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                            //      ' Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                             //     ' Set the directions

            
#endif
            // This command then tells the MPSSE to send any results gathered back immediately
            MPSSEbuffer[NumBytesToSend++] = 0x87;                                  //    ' Send answer back immediate command

            // send commands to chip
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
            {
                return 1;
            }

            // get the byte which has been read from the driver's receive buffer
            I2C_Status = Receive_Data(1);
            if (I2C_Status != 0)
            {
                return 1;
            }
            
            // InputBuffer2[0] now contains the results

            return 0;
        }
         

        //###################################################################################################################################
        // Sends I2C address followed by reading 2 bytes
        
        public byte I2C_Read2BytesWithAddr(byte Address)
        {
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            NumBytesToSend = 0;
            
            // ------------------------------------ Address ------------------------------------
            
            Address <<= 1;
            Address |= 0x01;

#if (FT232H)
            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // 
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   //  Data length of 0x0000 means 1 byte data to clock in
            MPSSEbuffer[NumBytesToSend++] = Address;// DataSend[0];          //  Byte to send

            // Put line back to idle (data released, clock pulled low)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data bits in
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit
#else

            // Set directions of clock and data to output in preparation for clocking out a byte
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// back to output
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions
            // clock out one byte
            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // 
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Data length of 0x0000 means 1 byte data to clock in
            MPSSEbuffer[NumBytesToSend++] = Address;                         // Byte to send

            // Put line back to idle (data released, clock pulled low) so that sensor can drive data line
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8)); // make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data byte in
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit

#endif

            // ------------------------------------ Clock in 1st byte and ACK ------------------------------------

#if (FT232H)
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BYTE_IN;      // Clock data byte in
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x00;                               // Data length of 0x0000 means 1 byte data to clock in

            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BIT_OUT;    // Clock data bit out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                              // Length of 0 means 1 bit
            MPSSEbuffer[NumBytesToSend++] = 0x00;                              // Sending 0 here as ACK

            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));

            MPSSEbuffer[NumBytesToSend++] = 0x80;                               //       ' Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                            //      ' Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                             //     ' Set the directions
#else          

            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BYTE_IN;      // Clock data byte in
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x00;                               // Data length of 0x0000 means 1 byte data to clock in
            
            // Send a 0 bit as an acknowledge
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));//back to output
            MPSSEbuffer[NumBytesToSend++] = 0x80;                               //       ' Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                            //      ' Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                             //     ' Set the directions

            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BIT_OUT;    // Clock data bit out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                              // Length of 0 means 1 bit
            MPSSEbuffer[NumBytesToSend++] = 0x00;                              // Sending 0 here as ACK
            
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8));//make data input
            
            MPSSEbuffer[NumBytesToSend++] = 0x80;                               //       ' Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                            //      ' Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                             //     ' Set the directions

            
#endif

            // ------------------------------------ Clock in 2nd byte and NAK ------------------------------------

#if (FT232H)
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BYTE_IN;      // Clock data byte in
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x00;                               // Data length of 0x0000 means 1 byte data to clock in

            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BIT_OUT;    // Clock data bit out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                              // Length of 0 means 1 bit
            MPSSEbuffer[NumBytesToSend++] = 0xFF;                              // Sending 1 here as NAK

            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));

            MPSSEbuffer[NumBytesToSend++] = 0x80;                               //       ' Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                            //      ' Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                             //     ' Set the directions
#else
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BYTE_IN;      // Clock data byte in
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x00;                               // Data length of 0x0000 means 1 byte data to clock in
            
            // Send a 1 bit as a Nack
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));//back to output
            MPSSEbuffer[NumBytesToSend++] = 0x80;                               //       ' Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                            //      ' Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                             //     ' Set the directions

            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BIT_OUT;    // Clock data bit out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                              // Length of 0 means 1 bit
            MPSSEbuffer[NumBytesToSend++] = 0xFF;                              // Sending 1 here as NAK
                                   
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8));//make data input
            
            MPSSEbuffer[NumBytesToSend++] = 0x80;                               //       ' Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                            //      ' Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                             //     ' Set the directions
                        
#endif
            // This command then tells the MPSSE to send any results gathered back immediately
            MPSSEbuffer[NumBytesToSend++] = 0x87;                                //  ' Send answer back immediate command

            // Send off the commands
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
            {
                return 1;
            }

            // Read back the ack from the address phase and the 2 bytes read
            I2C_Status = Receive_Data(3);
            if (I2C_Status != 0)
            {
                return 1;
            }
            
            // Check if address phase was acked
            if ((InputBuffer2[0] & 0x01) == 0)
            {
                I2C_Ack = true;
            }
            else
            {
                I2C_Ack = false;
            }
            
            // Get the two data bytes to put back to the calling function - InputBuffer2[0..1] now contains the results
            InputBuffer2[0] = InputBuffer2[1];
            InputBuffer2[1] = InputBuffer2[2];            
                        
            return 0;
            
        }

        //###################################################################################################################################

        public byte I2C_SendDeviceAddrAndCheckACK(byte Address, bool Read)
        {


            byte ADbusVal = 0;
            byte ADbusDir = 0;
            NumBytesToSend = 0;

            Address <<= 1;
            if (Read == true)
                Address |= 0x01;

#if (FT232H)
            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // 
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   //  Data length of 0x0000 means 1 byte data to clock in
            MPSSEbuffer[NumBytesToSend++] = Address;           //  Byte to send

            // Put line back to idle (data released, clock pulled low)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data bits in
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit
#else

            // Set directions of clock and data to output in preparation for clocking out a byte
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// back to output
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions
            // clock out one byte
            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // 
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Data length of 0x0000 means 1 byte data to clock in
            MPSSEbuffer[NumBytesToSend++] = Address;                         // Byte to send

            // Put line back to idle (data released, clock pulled low) so that sensor can drive data line
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8)); // make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data byte in
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit

#endif
            // This command then tells the MPSSE to send any results gathered (in this case the ack bit) back immediately
            MPSSEbuffer[NumBytesToSend++] = 0x87;                                //  ' Send answer back immediate command

            // send commands to chip
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
            {
                return 1;
            }

            // read back byte containing ack
            I2C_Status = Receive_Data(1);
            if (I2C_Status != 0)
            {
                return 1;            // can also check NumBytesRead
            }

            // if ack bit is 0 then sensor acked the transfer, otherwise it nak'd the transfer
            if ((InputBuffer2[0] & 0x01) == 0)
            {
                I2C_Ack = true;
            }
            else
            {
                I2C_Ack = false;
            }

            return 0;

        }

        //###################################################################################################################################
        // Writes one byte to the I2C bus

        public byte I2C_SendByteAndCheckACK(byte DataByteToSend)
        {
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            NumBytesToSend = 0;

#if (FT232H)
            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // 
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   //  Data length of 0x0000 means 1 byte data to clock in
            MPSSEbuffer[NumBytesToSend++] = DataByteToSend;// DataSend[0];          //  Byte to send

            // Put line back to idle (data released, clock pulled low)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data bits in
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit
#else

            // Set directions of clock and data to output in preparation for clocking out a byte
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// back to output
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions
            // clock out one byte
            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // 
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Data length of 0x0000 means 1 byte data to clock in
            MPSSEbuffer[NumBytesToSend++] = DataByteToSend;                         // Byte to send

            // Put line back to idle (data released, clock pulled low) so that sensor can drive data line
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8)); // make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data byte in
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit

#endif
            // This command then tells the MPSSE to send any results gathered (in this case the ack bit) back immediately
            MPSSEbuffer[NumBytesToSend++] = 0x87;                                //  ' Send answer back immediate command

            // send commands to chip
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
            {
                return 1;
            }

            // read back byte containing ack
            I2C_Status = Receive_Data(1);
            if (I2C_Status != 0)
            {
                return 1;            // can also check NumBytesRead
            }
       
            // if ack bit is 0 then sensor acked the transfer, otherwise it nak'd the transfer
            if ((InputBuffer2[0] & 0x01) == 0)
            {
                I2C_Ack = true;
            }
            else
            {
                I2C_Ack = false;
            }

            return 0;
                               
        }
        
        //###################################################################################################################################
        // Sets I2C Start condition

        public byte I2C_SetStart()
        {
            byte Count = 0;
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            NumBytesToSend = 0;


#if (FT232H)
            // SDA high, SCL high
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLhi | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));    // on FT232H lines always output

            for (Count = 0; Count < 6; Count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA lo, SCL high
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLhi | (GPIO_Low_Dat & 0xF8));

            for (Count = 0; Count < 6; Count++)	// Repeat commands to ensure the minimum period of the start setup time ie 600ns is achieved
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA lo, SCL lo
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));

            for (Count = 0; Count < 6; Count++)	// Repeat commands to ensure the minimum period of the start setup time ie 600ns is achieved
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // Release SDA
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));

            MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction


# else

            // Both SDA and SCL high (setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLin | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA low, SCL high (setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLin | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)	// Repeat commands to ensure the minimum period of the start setup time
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA low, SCL low
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));//
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));//as above

            for (Count = 0; Count < 6; Count++)	// Repeat commands to ensure the minimum period of the start setup time
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // Release SDA (setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));//
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8));//as above

            MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction



# endif
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
                return 1;
            else
                return 0;

        }
        
        //###################################################################################################################################
        // Sets I2C Stop condition

        public byte I2C_SetStop()
        {
            byte Count = 0;
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            NumBytesToSend = 0;

#if (FT232H)
            // SDA low, SCL low
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));    // on FT232H lines always output

            for (Count = 0; Count < 6; Count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }
           
            // SDA low, SCL high
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLhi | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));    // on FT232H lines always output

            for (Count = 0; Count < 6; Count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA high, SCL high
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLhi | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));        // on FT232H lines always output

            for (Count = 0; Count < 6; Count++)	
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }
           
# else

            // SDA low, SCL low
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }


            // SDA low, SCL high (note: setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLin | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA high, SCL high (note: setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLin | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)	// Repeat commands to hold states for longer time
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }
#endif
           // send the buffer of commands to the chip 
           I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
                return 1;
            else
                return 0;

        }
        

       //###################################################################################################################################
       // Sets GPIO values on low byte and puts I2C lines (bits 0, 1, 2) to idle outwith transaction state
       
        public byte I2C_SetLineStatesIdle()
       {
           byte ADbusVal = 0;
           byte ADbusDir = 0;
           NumBytesToSend = 0;
            
#if (FT232H)
           // '######## Combine the I2C line state for bits 2..0 with the GPIO for bits 7..3 ########
           ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLhi | (GPIO_Low_Dat & 0xF8));
           ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));    // FT232H always output due to open drain capability    
           
# else
           ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
           ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLin | (GPIO_Low_Dir & 0xF8));       // FT2232H/FT4232H use input to mimic open drain
# endif
            
           MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
           MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
           MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction

           I2C_Status = Send_Data(NumBytesToSend);
           if (I2C_Status != 0)
               return 1;
           else
               return 0;
       }

       //###################################################################################################################################
       // Gets GPIO values from low byte

       public byte I2C_GetGPIOValuesLow()
       {
           NumBytesToSend = 0;

           MPSSEbuffer[NumBytesToSend++] = 0x81;	    // ADbus GPIO command for reading low byte
           MPSSEbuffer[NumBytesToSend++] = 0x87;        // Send answer back immediate command
                      
           I2C_Status = Send_Data(NumBytesToSend);
           if (I2C_Status != 0)
               return 1;

           I2C_Status = Receive_Data(1);
           if (I2C_Status != 0)
           {
               return 1;
           }

           ADbusReadVal = (byte)(InputBuffer2[0] & 0xF8); // mask the returned value to show only 5 GPIO lines (bits 0/1/2 are I2C)
               
           return 0;
       }

        //###################################################################################################################################
        // Gets GPIO values from low byte (ADBus)

        public byte I2C_SetGPIOValuesLow(byte ADbusDir, byte ADbusVal)
        {

            ADbusDir = (byte)(0xF0 & ADbusDir);
            ADbusVal = (byte)(0xF0 & ADbusVal);


#if (FT232H)
            // '######## Combine the I2C line state for bits 2..0 with the GPIO for bits 7..3 ########
            ADbusVal = (byte)(0x00 | ADbusVal | (GPIO_Low_Dat & 0x0F));
            ADbusDir = (byte)(0x00 | ADbusDir | (GPIO_Low_Dir & 0x0F));    // FT232H always output due to open drain capability    

#else
           ADbusVal = (byte)(0x00 | ADbusVal | (GPIO_Low_Dat & 0x0F));
           ADbusDir = (byte)(0x00 | ADbusDir | (GPIO_Low_Dir & 0x0F));       // FT2232H/FT4232H use input to mimic open drain
#endif

            MPSSEbuffer[NumBytesToSend++] = 0x80;       // ADbus GPIO command
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;   // Set direction

            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
                return 1;
            else
                return 0;
        }




        //###################################################################################################################################
        // Sets GPIO values on high byte

        public byte I2C_SetGPIOValuesHigh(byte ACbusDir, byte ACbusVal)
       {
           NumBytesToSend = 0;
           
#if (FT4232H)

           return 1;
           
# else
           MPSSEbuffer[NumBytesToSend++] = 0x82;	    // ACbus GPIO command
           MPSSEbuffer[NumBytesToSend++] = ACbusVal;   // Set data value
           MPSSEbuffer[NumBytesToSend++] = ACbusDir;	// Set direction
                      
           I2C_Status = Send_Data(NumBytesToSend);
           if (I2C_Status != 0)
               return 1;
           else
               return 0;

# endif
       }



        //###################################################################################################################################
        // Gets GPIO values from high byte

       public byte I2C_GetGPIOValuesHigh()
       {
            NumBytesToSend = 0;

#if (FT4232H)
                return 1;       // no high byte on FT4232H
# else

           MPSSEbuffer[NumBytesToSend++] = 0x83;	        // ACbus read GPIO command
           MPSSEbuffer[NumBytesToSend++] = 0x87;            // Send answer back immediate command

           I2C_Status = Send_Data(NumBytesToSend);
           if (I2C_Status != 0)
               return 1;

           I2C_Status = Receive_Data(1);
           if (I2C_Status != 0)
               return 1;

           ACbusReadVal = (byte)(InputBuffer2[0]);      // Return via global variable for calling function to read

           return 0;
# endif
       }

        
        //###################################################################################################################################
        //###################################################################################################################################
        //##################                                          D2xx Layer                                        #####################
        //###################################################################################################################################
        //###################################################################################################################################
                

        // Read a specified number of bytes from the driver receive buffer

        private byte Receive_Data(uint BytesToRead)
        {
            uint NumBytesInQueue = 0;
            uint QueueTimeOut = 0;
            uint Buffer1Index = 0;
            uint Buffer2Index = 0;
            uint TotalBytesRead = 0;
            bool QueueTimeoutFlag = false;
            uint NumBytesRxd = 0;

            // Keep looping until all requested bytes are received or we've tried 5000 times (value can be chosen as required)
            while ((TotalBytesRead < BytesToRead) && (QueueTimeoutFlag == false))
            {
                ftStatus = myFtdiDevice.GetRxBytesAvailable(ref NumBytesInQueue);       // Check bytes available

                if ((NumBytesInQueue > 0) && (ftStatus == FTDI.FT_STATUS.FT_OK))
                {
                    ftStatus = myFtdiDevice.Read(InputBuffer, NumBytesInQueue, ref NumBytesRxd);  // if any available read them

                    if ((NumBytesInQueue == NumBytesRxd) && (ftStatus == FTDI.FT_STATUS.FT_OK))
                    {
                        Buffer1Index = 0;

                        while (Buffer1Index < NumBytesRxd)
                        {
                            InputBuffer2[Buffer2Index] = InputBuffer[Buffer1Index];     // copy into main overall application buffer
                            Buffer1Index++;
                            Buffer2Index++;
                        }
                        TotalBytesRead = TotalBytesRead + NumBytesRxd;                  // Keep track of total
                    }
                    else
                        return 1;

                    QueueTimeOut++;
                    if (QueueTimeOut == 5000)
                        QueueTimeoutFlag = true;
                    else
                        Thread.Sleep(0);                                                // Avoids running Queue status checks back to back
                }
            }
            // returning globals NumBytesRead and the buffer InputBuffer2
            NumBytesRead = TotalBytesRead;

            if (QueueTimeoutFlag == true)
                return 1;
            else
                return 0;
        }


        //###################################################################################################################################
        // Write a buffer of data and check that it got sent without error

        private byte Send_Data(uint BytesToSend)
        {

            NumBytesToSend = BytesToSend;

            // Send data. This will return once all sent or if times out
            ftStatus = myFtdiDevice.Write(MPSSEbuffer, NumBytesToSend, ref NumBytesSent);

            // Ensure that call completed OK and that all bytes sent as requested
            if ((NumBytesSent != NumBytesToSend) || (ftStatus != FTDI.FT_STATUS.FT_OK))
                return 1;   // error   calling function can check NumBytesSent to see how many got sent
            else
                return 0;   // success
        }
  
        //###################################################################################################################################
        // Flush drivers receive buffer - Get queue status and read everything available and discard data

        private byte FlushBuffer()
        {
            ftStatus = myFtdiDevice.GetRxBytesAvailable(ref BytesAvailable);	 // Get the number of bytes in the receive buffer
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
                return 1;
            
            if(BytesAvailable > 0)
            {
                ftStatus = myFtdiDevice.Read(InputBuffer, BytesAvailable, ref NumBytesRead);  	//Read out the data from receive buffer
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                    return 1;       // error
                else
                    return 0;       // all bytes successfully read
            }
            else
            {
                return 0;           // there were no bytes to read
            }
        }


        //###################################################################################################################################
        // User Functions 
        


        private void buttonRed_Click(object sender, EventArgs e)
        {
            AppStatus = LP55281_LED(0x4C, 0, true);
            if (AppStatus != 0) labelDUTStatus.Text = "Error"; 

            AppStatus = LP55281_LED(0x4D, 0, true);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4C, 1, false);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4D, 1, false);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4C, 2, false);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4D, 2, false);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

        }
        private void buttonGreen_Click(object sender, EventArgs e)
        {
            AppStatus = LP55281_LED(0x4C, 0, false);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4D, 0, false);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4C, 1, true);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4D, 1, true);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4C, 2, false);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4D, 2, false);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";
        }
        private void buttonBlue_Click(object sender, EventArgs e)
        {
            AppStatus = LP55281_LED(0x4C, 0, false);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4D, 0, false);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4C, 1, false);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4D, 1, false);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4C, 2, true);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";

            AppStatus = LP55281_LED(0x4D, 2, true);
            if (AppStatus != 0) labelDUTStatus.Text = "Error";
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            StartScan = true;

            while(StartScan)
            {
                AppStatus = PCA9555_Read();
                if (AppStatus != 0)
                {
                    labelDUTStatus.Text = "Failed I/O Expander Init";
                    DeviceInit = false;
                    StartScan = false;
                }
                else
                {
                    labelDUTStatus.Text = "I/O Expander  Initiated";
                }

                Update();
                Application.DoEvents();
                Thread.Sleep(300);
            }


        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            StartScan = false;
        }

    }
}

