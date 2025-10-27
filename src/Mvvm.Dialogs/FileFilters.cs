using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace RFBCodeWorks.Mvvm.DialogServices
{   
    /// <summary>
    /// Interface for objects that provide a Search Pattern for use when Opening/Saving Files
    /// </summary>
    public interface IFileFilter
    {
        /// <summary>
        /// A search pattern that can be used with methods similar to <see cref="System.IO.Directory.GetFiles(string)"/>
        /// </summary>
        string SearchPattern { get; }

        /// <summary>
        /// A string that can be used by File Dialogs to specify the types of files to search for/save as.
        /// <br/>This should be in the format: '{Name}|{Filter}'  Examples: 
        /// <code>
        /// "Text Files|*.txt"
        /// <br/>"Image Files|*.jpg;*.jpeg"
        /// <br/>"Log Files(*.log)|*.log"
        /// </code>
        /// </summary>
        /// <remarks>Any string compatible with <see cref="Microsoft.Win32.FileDialog.Filter"/> will work.</remarks>
        string DialogFilter { get; }
    }
    
    /// <summary>
    /// Class that represents FileDialog Filters and is implicitly convertable to strings
    /// </summary>
    public class FileFilter : IFileFilter
    {

        #region  < Constructors>

        /// <summary>
        /// Create a new <see cref="FileFilter"/> from a filename.
        /// <br/> The FileName without extension becomes the <see cref="Name"/>
        /// <br/> The File Extension becomes the <see cref="Extension"/>
        /// <br/> A search pattern will be generated via <see cref="GetSearchPattern(string)"/>
        /// </summary>
        /// <param name="filename">The filename to parse.</param>
        /// <param name="useExactFileName">
        /// When <see langword="true"/>: the SearchPattern will be identical to the <paramref name="filename"/> parameter. <br/>
        /// When <see langword="false"/> (default): the SearchPattern will be derived from the file extension. ex: MyFile.txt -> *.txt
        /// </param>
        public FileFilter(string filename, bool useExactFileName = false)
        {
            Name = Path.GetFileNameWithoutExtension(filename);
            Extension = Path.GetExtension(filename);
            SearchPattern = useExactFileName ? filename : GetSearchPattern(Extension);
        }

        /// <summary>
        /// Create a new FileFilter object using a specific <paramref name="name"/> and <paramref name="searchPattern"/>
        /// </summary>
        /// <param name="name"><inheritdoc cref="Name" path="*"/></param>
        /// <param name="searchPattern"><inheritdoc cref="SearchPattern" path="*"/></param>
        public FileFilter(string name, string searchPattern)
        {
            Name = name;
            SearchPattern = SanitizeSearchPattern(searchPattern);
            Extension = GetExtensionsFromSearchPattern(searchPattern);
        }

        /// <summary>
        /// Create a new FileFilter object that specifies a <paramref name="name"/>, a default <paramref name="extension"/>, and a specific <paramref name="searchPattern"/>
        /// </summary>
        /// <param name="name"><inheritdoc cref="Name" path="*"/></param>
        /// <param name="extension"><inheritdoc cref="Extension" path="*"/></param>
        /// <param name="searchPattern"><inheritdoc cref="SearchPattern" path="*"/></param>
        public FileFilter(string name, string extension, string searchPattern)
        {
            Name = name;
            Extension = extension;
            SearchPattern = searchPattern;
        }

        #endregion

        #region < Implicit Conversion >

        /// <summary>
        /// Attempt to implicitly convert the <paramref name="filter"/> into a new <see cref="FileFilter"/> object
        /// </summary>
        /// <inheritdoc cref="FileFilter.TryParse(string, out FileFilter)"/>
        public static implicit operator FileFilter(string filter)
        {
            if (TryParse(filter, out var newFilter))
                return newFilter;
            throw new ArgumentException($"Unable to implicitly convert string '{filter}' into a new {typeof(FileFilter)} object");
        }

        #endregion

        #region < Object Properties >

        /// <summary>
        /// The description of the file type
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The File Extension, such as .txt. <br/>
        /// Multiple extensions are valid here, as long as they are delimited by the ';' character. If this is the case, evaluation should be performed against the <see cref="Extensions"/> property.
        /// </summary>
        /// <remarks>
        /// Examples: <br/>
        /// Single Extension: .jpg <br/>
        /// Multiple Extensions: .jpg ; .jpeg <br/>
        /// </remarks>
        public string Extension { get; }

        /// <summary>
        /// Creates a new String[] from the <see cref="Extension"/> string by splitting it on the ';' character
        /// </summary>
        public string[] Extensions => Extension.Split(';');

        /// <summary>
        /// <inheritdoc cref="Directory.GetFiles(string, string)" path="/param[@name='searchPattern']" /> <br/>
        /// Multiple patterns can be used here for use with <see cref="DialogFilter"/> and <see cref="MvvmDialogFilter"/>. These must be ';' delimited.
        /// </summary>
        /// <remarks>
        /// Examples: <br/>
        /// Single Extension: *.jpg <br/>
        /// Multiple Extensions: *.jpg ; *.jpeg <br/>
        /// </remarks>
        public string SearchPattern { get; }

        /// <summary>
        /// Creates a new String[] from the <see cref="SearchPattern"/> string by splitting it on the ';' character
        /// </summary>
        public string[] SearchPatterns=> SearchPattern.Split(';');

        /// <summary>
        /// The Concatenated File Filter for use with standard windows file browser dialogs in the following format: <br/> All files (*.*)|*.*
        /// </summary>
        /// <remarks>Creates the filter from the <see cref="Name"/> and <see cref="SearchPattern"/></remarks>
        public string DialogFilter
        {
            get
            {
                if (dfilter is null) dfilter = GetDialogFilter(Name, SearchPattern);
                return dfilter;
            }
        }
        private string dfilter;

        /// <summary>
        /// The Concatenated File Filter for use with file browser dialogs in the following format: <br/> All files|*.*
        /// </summary>
        /// <remarks>Creates the filter from the <see cref="Name"/> and <see cref="SearchPattern"/></remarks>
        public string MvvmDialogFilter
        {
            get
            {
                if (mfilter is null) mfilter = GetMvvmDialogFileFilter(Name, SearchPattern);
                return mfilter;
            }
        }
        private string mfilter = null;

        /// <summary>
        /// Return <see cref="MvvmDialogFilter"/>
        /// </summary>
        /// <returns><inheritdoc cref="MvvmDialogFilter" path="*"/></returns>
        public override string ToString()
        {
            return MvvmDialogFilter;
        }

        #endregion

        #region < Static Methods >

        //lang=Regex
        private const string FileFilterRegexString = "([*].*?[.][a-z0-9]+[;]?)";

        /// <summary>^[*].*[.][a-z0-9]+$</summary>
        public static readonly Regex FileFilterRegex = new Regex("^" + FileFilterRegexString + "$", RegexOptions.Compiled);
        /// <summary>^[.][a-z0-9]+$</summary>
        public static readonly Regex FileExtensionRegex = new Regex("^[.][a-z0-9]+$", RegexOptions.Compiled);
        /// <summary>^[*].*[.][a-z0-9]+$</summary>
        private static readonly Regex SanitizeSearchPatternRegex = new Regex("(?<pattern>[*]?[^;]*?[.][a-z0-9]+[^;])+", RegexOptions.Compiled);


        /// <summary>^[*].*[.][a-z0-9]+$</summary>
        //lang=Regex
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "RE0001:Regex issue: {0}", Justification = "Its Valid. Don't worry.")]
        private static readonly Regex TryParseRegex = new Regex("(?<Name>[^|]+)([(](?<extension>.*)[)])?(?<Pattern>" + FileFilterRegexString + "+)", RegexOptions.Compiled);

        /// <summary>
        /// Attempt to parse the string into a new FileFilter object
        /// </summary>
        /// <param name="filter">The filter string to parse. ex:
        /// <br/> All Files (*.*) | *.*
        /// <br/> Text Files | *.txt
        /// <br/> MyFile.txt -> equivalent to TextFiles example above
        /// </param>
        /// <param name="fileFilter">The FileFilter object dervied from the string</param>
        /// <returns>TRUE if successfull, otherwise false</returns>
        public static bool TryParse(string filter, out FileFilter fileFilter)
        {
            fileFilter = null;
            if (filter.Contains(@"\") | filter.Contains(@"/"))
                filter = Path.GetFileName(filter);
            
            if (filter.Contains("|"))
            {
                if (TryParseRegex.IsMatch(filter, out var match))
                {
                    var filename = match.Groups["Name"].Value;
                    var pattern = match.Groups["Pattern"].Value;
                    if (match.Groups["extension"].Success)
                        fileFilter = new FileFilter(filename, match.Groups["extension"].Value, pattern);
                    else
                        fileFilter = new FileFilter(filename, pattern);
                    return true;
                }
            }
            if (filter == Path.GetFileName(filter))
            {
                fileFilter = new FileFilter(filter);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sanitizes the extension into a FileFIlter
        /// </summary>
        /// <param name="extension">the file extension or filename</param>
        /// <returns>
        /// Inputs: file.txt / *.txt / .txt <br/>
        /// Output: *.txt
        /// </returns>
        public static string GetSearchPattern(string extension)
        {
            if (extension is null) throw new ArgumentNullException(nameof(extension));
            if (FileExtensionRegex.IsMatch(extension)) return "*" + extension;
            if (FileFilterRegex.IsMatch(extension)) return extension;
            string ext = Path.GetExtension(extension);
            //if (ext == string.Empty) throw new ArgumentException("No Extension Data Found");
            return "*" + ext;
        }

        /// <summary>
        /// Sanitizes a search pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns>
        /// Input: .txt -- Output: *.txt
        /// Input: .txt;.log -- Output: *.txt;*.log
        /// </returns>
        public static string SanitizeSearchPattern(string pattern)
        {
            string result = null;
            var matches = SanitizeSearchPatternRegex.Matches(pattern);
            bool first = true;
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    if (first)
                    {
                        result = GetSearchPattern(match.Groups["pattern"].Value);
                        first = false;
                    }
                    else
                        result += ";" + GetSearchPattern(match.Groups["pattern"].Value);
                }
            }
            return result ?? pattern;
        }

        /// <summary>
        /// Sanitizes a search pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns>
        /// Input: *.txt -- Output: .txt
        /// Input: *.txt;*.log -- Output: .txt;.log
        /// </returns>
        public static string GetExtensionsFromSearchPattern(string pattern)
        {
            string result = null;
            var matches = SanitizeSearchPatternRegex.Matches(pattern);
            bool first = true;
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    if (first)
                    {
                        result = Path.GetExtension(match.Groups["pattern"].Value);
                        first = false;
                    }
                    else
                        result += ";" + Path.GetExtension(match.Groups["pattern"].Value);
                }
            }
            return result ?? pattern;
        }


        /// <summary>
        /// Generate a new File Filter
        /// </summary>
        /// <param name="name"><inheritdoc cref="Name" path="*"/></param>
        /// <param name="searchPattern"><inheritdoc cref="Exception" path="*"/></param>
        /// <returns><inheritdoc cref="DialogFilter" path="*"/></returns>
        public static string GetDialogFilter(string name, string searchPattern) { return String.Format(@"{0} ({1})|{1}", name, searchPattern); }

        /// <inheritdoc cref="GetDialogFilter(string, string)"/>
        public static string GetDialogFilter(string fileName) => GetDialogFilter(Path.GetFileNameWithoutExtension(fileName), GetSearchPattern(fileName));

        /// <summary>
        /// Generate a new File Filter designed for MVVMDialogs FileDialog window
        /// </summary>
        /// <param name="name"><inheritdoc cref="Name" path="*"/></param>
        /// <param name="searchPattern"><inheritdoc cref="Exception" path="*"/></param>
        /// <returns><inheritdoc cref="MvvmDialogFilter" path="*"/></returns>
        public static string GetMvvmDialogFileFilter(string name, string searchPattern) { return String.Format(@"{0}|{1}", name, searchPattern); }

        /// <inheritdoc cref="GetMvvmDialogFileFilter(string, string)"/>
        public static string GetMvvmDialogFileFilter(string fileName) => GetMvvmDialogFileFilter(Path.GetFileNameWithoutExtension(fileName), GetSearchPattern(fileName));

        #endregion
    }

    /// <summary>
    /// Contains the standard File Filters and extension methods
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.filedialog.filter?view=windowsdesktop-6.0"/>
    public static class FileFilters
    {
        /// <inheritdoc cref="Regex.IsMatch(string)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsMatch(this Regex pattern, string text, out Match match)
        {
            match = pattern.Match(text);
            return match.Success;
        }

        /// <summary>
        /// Get the concatenated filter string
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string GetDialogFilter(this IEnumerable<FileFilter> collection)
        {
            string str = string.Empty;
            bool first = true;
            foreach( var f in collection)
            {
                if (first)
                {
                    str = f.DialogFilter;
                    first = false;
                }
                else
                    str += "|" + f.DialogFilter;
            }
            return str;
        }

        /// <inheritdoc cref="GetDialogFilter(IEnumerable{FileFilter})"/>
        public static string ToDialogFilter(this IEnumerable<FileFilter> collection) => GetDialogFilter(collection);

        /// <summary>
        /// Get the concatenated filter string for use with MvvMDialogs.FileDialog 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string GetMvvmDialogFilter(this IEnumerable<FileFilter> collection)
        {
            string str = string.Empty;
            bool first = true;
            foreach (var f in collection)
            {
                if (first)
                {
                    str = f.MvvmDialogFilter;
                    first = false;
                }
                else
                    str += "|" + f.MvvmDialogFilter;
            }
            return str;
        }
        /// <inheritdoc cref="GetMvvmDialogFilter(IEnumerable{FileFilter})"/>
        public static string ToMvvmDialogFilter(this IEnumerable<FileFilter> collection) => GetMvvmDialogFilter(collection);

        /// <summary>
        /// Attempt to convert the collection of strings to <see cref="FileFilter"/> objects
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static FileFilter[] Parse(IEnumerable<string> collection)
        {
            List<FileFilter> filters = new List<FileFilter>();
            foreach (var s in collection)
                if (FileFilter.TryParse(s, out var filter))
                    filters.Add(filter);
            return filters.ToArray();
        }

        /// <summary>
        /// Any File ( *.* )
        /// </summary>
        public static FileFilter AllFiles => new FileFilter("All Files", "*.*", "*.*");

        /// <summary>
        /// Text Files ( *.txt )
        /// </summary>
        public static FileFilter TextFiles => new FileFilter("Text Files", ".txt", "*.txt");

        /// <summary>
        /// Log Files ( *.log )
        /// </summary>
        public static FileFilter LogFiles => new FileFilter("Log Files", ".log", "*.log");

        /// <summary>
        /// Zip Files ( *.zip )
        /// </summary>
        public static FileFilter ZipFiles => new FileFilter("Archives", ".zip", "*.zip");

        /// <summary>
        /// Executable Files ( *.exe )
        /// </summary>
        public static FileFilter Executable => new FileFilter("Executable", ".exe", "*.exe");

        /// <summary>
        /// Image Files ( *.BMP | *.JPG | *.GIF | *.JPEG )
        /// </summary>
        public static FileFilter ImageFiles => new FileFilter("Image Files", "*.BMP;*.JPG;*.GIF;*.JPEG", "*.BMP;*.JPG;*.GIF;*.JPEG");
    }
}
