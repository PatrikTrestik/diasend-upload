using System;
using System.Collections.Generic;
using System.Text;

// See:
// https://www.bluetooth.com/wp-content/uploads/Sitecore-Media-Library/Gatt/Xml/Characteristics/org.bluetooth.characteristic.glucose_measurement.xml
namespace DiasendAPI.GATGlucoseMeasurement
{
    /// <summary>
    /// These flags define which data fields are present in the Characteristic value
    /// </summary>
    [Flags]
    public enum GlucoseMeasurementFlags
    {
        /// <summary>
        /// The time offset present
        /// </summary>
        TimeOffsetPresent = 1,
        /// <summary>
        /// Glucose Concentration, Type and Sample Location Present
        /// </summary>
        GlucoseTypeSampleLocationPresent = 2,
        /// <summary>
        /// Glucose Concentration Units, 0-kg/L, 1-mol/L
        /// </summary>
        GlucoseConcentrationUnits = 4,
        /// <summary>
        /// Sensor Status Annunciation Present
        /// </summary>
        SensorStatusAnnunciationPresent = 8,
        /// <summary>
        /// Context Information Follows
        /// </summary>
        ContextInformationFollows = 16
    }
    /// <summary>
    /// Glucose Sample Type
    /// </summary>
    public enum GlucoseSampleType
    { 
         
        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved1 = 0,
        /// <summary>
        /// The capillary whole blood
        /// </summary>
        CapillaryWholeBlood = 1,
        /// <summary>
        /// The capillary plasma
        /// </summary>
        CapillaryPlasma = 2,
        /// <summary>
        /// The venous whole blood
        /// </summary>
        VenousWholeBlood = 3,
        /// <summary>
        /// The venous plasma
        /// </summary>
        VenousPlasma = 4,
        /// <summary>
        /// The arterial whole blood
        /// </summary>
        ArterialWholeBlood = 5,
        /// <summary>
        /// The arterial plasma
        /// </summary>
        ArterialPlasma = 6,
        /// <summary>
        /// The undetermined whole blood
        /// </summary>
        UndeterminedWholeBlood = 7,
        /// <summary>
        /// The undetermined Plasma
        /// </summary>
        UndeterminedPlasma = 8,
        /// <summary>
        /// The interstitial fluid
        /// </summary>
        InterstitialFluid = 9,
        /// <summary>
        /// The control solution
        /// </summary>
        ControlSolution = 10
    }
    /// <summary>
    /// Glucose Sample Location
    /// </summary>
    public enum GlucoseSampleLocation
    {
        /// <summary>
        /// The reserved1
        /// </summary>
        Reserved1 = 0,
        /// <summary>
        /// The finger
        /// </summary>
        Finger = 1,
        /// <summary>
        /// The alternate site test
        /// </summary>
        AlternateSiteTest = 2,
        /// <summary>
        /// The earlobe
        /// </summary>
        Earlobe = 3,
        /// <summary>
        /// The control solution
        /// </summary>
        ControlSolution = 4,
        /// <summary>
        /// The sample location value not available
        /// </summary>
        SampleLocationValueNotAvailable = 15
    }
}
