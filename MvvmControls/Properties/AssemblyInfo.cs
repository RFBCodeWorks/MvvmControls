using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Markup;

[assembly: System.Windows.ThemeInfo(System.Windows.ResourceDictionaryLocation.None, System.Windows.ResourceDictionaryLocation.SourceAssembly)]

// In SDK-style projects such as this one, several assembly attributes that were historically
// defined in this file are now automatically added during build and populated with
// values defined in project properties. For details of which attributes are included
// and how to customise this process see: https://aka.ms/assembly-info-properties


// Setting ComVisible to false makes the types in this assembly not visible to COM
// components.  If you need to access a type in this assembly from COM, set the ComVisible
// attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.


[assembly: Guid("646cf70c-e2ab-4f1b-b138-fe1f164ae379")]
[assembly: XmlnsPrefix(RFBCodeWorks.XmlnsMarkupDefs.URL, "cb")]
[assembly: XmlnsDefinition(RFBCodeWorks.XmlnsMarkupDefs.SPECIALIZED, "RFBCodeWorks.Mvvm.Specialized")]
[assembly: XmlnsDefinition(RFBCodeWorks.XmlnsMarkupDefs.BEHAVIORS, "RFBCodeWorks.WPF.Behaviors")]
[assembly: XmlnsDefinition(RFBCodeWorks.XmlnsMarkupDefs.CONTROLS, "RFBCodeWorks.WPF.Controls")]
[assembly: XmlnsDefinition(RFBCodeWorks.XmlnsMarkupDefs.CONVERTERS, "RFBCodeWorks.WPF.Converters")]

namespace RFBCodeWorks 
{
    internal static class XmlnsMarkupDefs
    {
        public const string URL = @"https://github.com/RFBCodeWorks/MvvmControls/";
        public const string SPECIALIZED = URL + @"Mvvm/Specialized";
        public const string BEHAVIORS = URL + @"WPF.Behaviors";
        public const string CONTROLS = URL + @"WPF.Controls";
        public const string CONVERTERS = URL + @"WPF.Converters";

    }
}