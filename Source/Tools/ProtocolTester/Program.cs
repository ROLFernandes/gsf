﻿//******************************************************************************************************
//  Program.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/16/2011 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GSF.TimeSeries;
using GSF;
using GSF.IO;
using GSF.PhasorProtocols;
using GSF.PhasorProtocols.Anonymous;

namespace ProtocolTester
{
    /// <summary>
    /// This is a test tool designed to help with protocol development, it is not intended for deployment...
    /// </summary>
    public class Program
    {
        private static bool WriteLogs = false;
        private static bool TestConcentrator = false;

        private static Concentrator concentrator;
        private static MultiProtocolFrameParser parser;
        private static ConcurrentDictionary<string, IMeasurement> m_definedMeasurements;
        private static ConcurrentDictionary<ushort, ConfigurationCell> m_definedDevices;
        private static StreamWriter m_exportFile;
        private static uint measurementID = 0;
        private static long frameCount;

        public static void Main(string[] args)
        {
            m_definedMeasurements = new ConcurrentDictionary<string, IMeasurement>();
            m_definedDevices = new ConcurrentDictionary<ushort, ConfigurationCell>();

            if (WriteLogs)
                m_exportFile = new StreamWriter(FilePath.GetAbsolutePath("InputTimestamps.csv"));

            if (TestConcentrator)
            {
                // Create a new concentrator
                concentrator = new Concentrator(WriteLogs, FilePath.GetAbsolutePath("OutputTimestamps.csv"));
                concentrator.TimeResolution = 333000;
                concentrator.FramesPerSecond = 30;
                concentrator.LagTime = 3.0D;
                concentrator.LeadTime = 9.0D;
                concentrator.PerformTimestampReasonabilityCheck = false;
                concentrator.ProcessByReceivedTimestamp = true;
                concentrator.Start();
            }

            // Create a new protocol parser
            parser = new MultiProtocolFrameParser();
            parser.AllowedParsingExceptions = 500;
            parser.ParsingExceptionWindow = 5;

            // Attach to desired events
            parser.ConnectionAttempt += parser_ConnectionAttempt;
            parser.ConnectionEstablished += parser_ConnectionEstablished;
            parser.ConnectionException += parser_ConnectionException;
            parser.ParsingException += parser_ParsingException;
            parser.ReceivedConfigurationFrame += parser_ReceivedConfigurationFrame;
            parser.ReceivedDataFrame += parser_ReceivedDataFrame;

            // Define the connection string
            //parser.ConnectionString = @"phasorProtocol=Macrodyne; accessID=1; transportProtocol=File; skipDisableRealTimeData = true; file=C:\Users\Ritchie\Desktop\Macrodyne\ING.out; iniFileName=C:\Users\Ritchie\Desktop\Macrodyne\BCH18Aug2011.ini; deviceLabel=ING1; protocolVersion=G";
            parser.ConnectionString = @"phasorProtocol=Iec61850_90_5; accessID=1; transportProtocol=UDP; skipDisableRealTimeData = true; localPort=102; interface=0.0.0.0; commandChannel={transportProtocol=TCP; server=172.21.1.201:4712; interface=0.0.0.0}";

            // When connecting to a file based resource you may want to loop the data
            parser.AutoRepeatCapturedPlayback = true;

            // Start frame parser
            parser.AutoStartDataParsingSequence = true;
            parser.Start();

            // To keep the console open while receiving live data with AutoRepeatCapturedPlayback = false, uncomment the following line of code:
            Console.ReadLine();

            // Stop concentrator
            if (TestConcentrator)
                concentrator.Stop();

            if (WriteLogs)
                m_exportFile.Close();
        }

        private static void parser_ReceivedDataFrame(object sender, EventArgs<IDataFrame> e)
        {
            IDataFrame dataFrame = e.Argument;

            if (WriteLogs)
                m_exportFile.WriteLine(dataFrame.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff") + string.Concat(dataFrame.Cells.Select(cell => "," + cell.FrequencyValue.Frequency.ToString())));

            // Pass frame measurements to concentrator...
            if (TestConcentrator)
                ExtractFrameMeasurements(dataFrame);

            // Increase the frame count each time a frame is received
            frameCount++;

            //IDataCell device = dataFrame.Cells[0];
            //Console.Write(device.FrequencyValue.Frequency.ToString("0.000Hz "));

            // Print information each time we receive 60 frames (every 2 seconds for 30 frames per second)
            // Also check to assure the DataFrame has at least one Cell
            if ((frameCount % 60 == 0) && (dataFrame.Cells.Count > 0))
            {
                IDataCell device = dataFrame.Cells[0];
                Console.WriteLine("Received {0} data frames so far...", frameCount);
                Console.WriteLine("    Last frequency: {0}Hz", device.FrequencyValue.Frequency);

                for (int x = 0; x < device.PhasorValues.Count; x++)
                {
                    Console.WriteLine("PMU {0} Phasor {1} Angle = {2}", device.IDCode, x, device.PhasorValues[x].Angle);
                    Console.WriteLine("PMU {0} Phasor {1} Magnitude = {2}", device.IDCode, x, device.PhasorValues[x].Magnitude);
                }

                Console.WriteLine("    Last Timestamp: {0}", ((DateTime)device.Timestamp).ToString("yyyy-MM-dd HH:mm:ss.fff"));

                if ((object)concentrator != null)
                    Console.WriteLine(concentrator.Status);
            }
        }

        private static void parser_ReceivedConfigurationFrame(object sender, EventArgs<IConfigurationFrame> e)
        {
            // Notify the user when a configuration frame is received
            Console.WriteLine("Received configuration frame with {0} device(s)", e.Argument.Cells.Count);
        }

        private static void parser_ParsingException(object sender, EventArgs<Exception> e)
        {
            // Output the exception to the user
            Console.WriteLine("Parsing exception: {0}", e.Argument);
        }

        private static void parser_ConnectionException(object sender, EventArgs<Exception, int> e)
        {
            // Display which connection attempt failed and the exception that occurred
            Console.WriteLine("Connection attempt {0} failed due to exception: {1}",
                e.Argument2, e.Argument1);
        }

        private static void parser_ConnectionEstablished(object sender, EventArgs e)
        {
            // Notify the user when the connection is established
            Console.WriteLine("Initiating {0} {1} based connection...",
                parser.PhasorProtocol.GetFormattedProtocolName(),
                parser.TransportProtocol.ToString().ToUpper());
        }

        private static void parser_ConnectionAttempt(object sender, EventArgs e)
        {
            // Let the user know we are attempting to connect
            Console.WriteLine("Attempting connection...");
        }

        private static void ExtractFrameMeasurements(IDataFrame frame)
        {
            const int AngleIndex = (int)CompositePhasorValue.Angle;
            const int MagnitudeIndex = (int)CompositePhasorValue.Magnitude;
            const int FrequencyIndex = (int)CompositeFrequencyValue.Frequency;
            const int DfDtIndex = (int)CompositeFrequencyValue.DfDt;

            List<IMeasurement> mappedMeasurements = new List<IMeasurement>();
            ConfigurationCell definedDevice;
            PhasorValueCollection phasors;
            AnalogValueCollection analogs;
            DigitalValueCollection digitals;
            IMeasurement[] measurements;
            Ticks timestamp;
            int x, count;

            // Get adjusted timestamp of this frame
            timestamp = frame.Timestamp;

            // Loop through each parsed device in the data frame
            foreach (IDataCell parsedDevice in frame.Cells)
            {
                try
                {
                    // Lookup device by its label (if needed), then by its ID code
                    definedDevice = m_definedDevices.GetOrAdd(parsedDevice.IDCode, id =>
                    {
                        return new ConfigurationCell(id)
                        {
                            StationName = parsedDevice.StationName,
                            IDLabel = parsedDevice.IDLabel.ToNonNullNorWhiteSpace(parsedDevice.StationName),
                            IDCode = parsedDevice.IDCode
                        };
                    });

                    // Map status flags (SF) from device data cell itself (IDataCell implements IMeasurement
                    // and exposes the status flags as its value)
                    MapMeasurementAttributes(mappedMeasurements, definedDevice.GetSignalReference(SignalKind.Status), parsedDevice);

                    // Map phase angles (PAn) and magnitudes (PMn)
                    phasors = parsedDevice.PhasorValues;
                    count = phasors.Count;

                    for (x = 0; x < count; x++)
                    {
                        // Get composite phasor measurements
                        measurements = phasors[x].Measurements;

                        // Map angle
                        MapMeasurementAttributes(mappedMeasurements, definedDevice.GetSignalReference(SignalKind.Angle, x, count), measurements[AngleIndex]);

                        // Map magnitude
                        MapMeasurementAttributes(mappedMeasurements, definedDevice.GetSignalReference(SignalKind.Magnitude, x, count), measurements[MagnitudeIndex]);
                    }

                    // Map frequency (FQ) and dF/dt (DF)
                    measurements = parsedDevice.FrequencyValue.Measurements;

                    // Map frequency
                    MapMeasurementAttributes(mappedMeasurements, definedDevice.GetSignalReference(SignalKind.Frequency), measurements[FrequencyIndex]);

                    // Map dF/dt
                    MapMeasurementAttributes(mappedMeasurements, definedDevice.GetSignalReference(SignalKind.DfDt), measurements[DfDtIndex]);

                    // Map analog values (AVn)
                    analogs = parsedDevice.AnalogValues;
                    count = analogs.Count;

                    for (x = 0; x < count; x++)
                    {
                        // Map analog value
                        MapMeasurementAttributes(mappedMeasurements, definedDevice.GetSignalReference(SignalKind.Analog, x, count), analogs[x].Measurements[0]);
                    }

                    // Map digital values (DVn)
                    digitals = parsedDevice.DigitalValues;
                    count = digitals.Count;

                    for (x = 0; x < count; x++)
                    {
                        // Map digital value
                        MapMeasurementAttributes(mappedMeasurements, definedDevice.GetSignalReference(SignalKind.Digital, x, count), digitals[x].Measurements[0]);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception encountered while mapping \"{0}\" data frame cell \"{1}\" elements to measurements: {2}", frame.IDCode, parsedDevice.StationName.ToNonNullString("[undefined]"), ex.Message);
                }
            }

            concentrator.SortMeasurements(mappedMeasurements);
        }

        private static void MapMeasurementAttributes(ICollection<IMeasurement> mappedMeasurements, string signalReference, IMeasurement parsedMeasurement)
        {
            // Coming into this function the parsed measurement value will only have a "value" and a "timestamp";
            // the measurement will not yet be associated with an actual historian measurement ID as the measurement
            // will have come directly out of the parsed phasor protocol data frame.  We take the generated signal
            // reference and use that to lookup the actual historian measurement ID, source, adder and multipler.
            IMeasurement definedMeasurement = m_definedMeasurements.GetOrAdd(signalReference, signal =>
            {
                Guid id = Guid.NewGuid();
                return new Measurement()
                {
                    Key = new MeasurementKey(id, ++measurementID, signal),
                    ID = id
                };
            });

            // Lookup signal reference in defined measurement list
            if (m_definedMeasurements.TryGetValue(signalReference, out definedMeasurement))
            {
                // Assign ID and other relevant attributes to the parsed measurement value
                parsedMeasurement.ID = definedMeasurement.ID;
                parsedMeasurement.Key = definedMeasurement.Key;
                parsedMeasurement.Adder = definedMeasurement.Adder;              // Allows for run-time additive measurement value adjustments
                parsedMeasurement.Multiplier = definedMeasurement.Multiplier;    // Allows for run-time mulplicative measurement value adjustments

                // Add the updated measurement value to the destination measurement collection
                mappedMeasurements.Add(parsedMeasurement);
            }
        }

    }
}