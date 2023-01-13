# MvvmControls
 This is a Library designed to ease working within an MVVM framework, and is meant to consolidate boiler-plate ViewModel interactions for common WPF controls into pre-built classes. 
 This library also includes interfaces and AttachedProperties that allow a ViewModel to be notified and react to various events of a Window, including:
 - Activated/Deactivated
 - Loaded/ContentRendered
 - Closed/Closing -- Closing can also be cancelled from the ViewModel
 
 These classes can then be directly bound to within the xaml using the 'Behaviors' namespace, potentially greatly reducing the xaml as well.
 
 ## Example - A simplified ComboBox Experience
 
A ComboBox has the following properties that the ViewModel may interact with:
 
- ItemSource (The items within the drop-down)
- SelectedItem (The currently selected item)
- SelectedValue \ SelectedValuePath ( The value of the SelectedItem, as an object, typically a property of the item )
- SelectedItemChanged event - Occurs and a new item is selected
- ItemSourceChanged event - Occurs when the itemsource is updated
- IsEnabled - Enable/Disable a control from within the ViewModel
- Visibilty / IsVisible - Toggle visibility of an item from within the ViewModel
  
  
 This can all be set up within the ViewModel like so:
```
public class MyViewModel {

	public MyViewModel()
	{
		MyComboBox = new ComboBoxDefinition<string>()
		{
			ItemSource = new string[] { "Index0", "Index1", "Index2", "Index3" }
		};
		ComboBoxDefinition.SelectedItemChanged += NewItemSelected;
	}
	public ComboBoxDefinition<string> MyComboBox { get; }

	private void NewItemSelected(object sender, EventArgs e) { /* React to item selection */ } 
}
```	
And the corresponding xaml:
```
	xmlns:MvvmControls="clr-namespace:RFBCodeWorks.Mvvvm.Behaviors;assembly=MvvmControls"

	<ComboBox MvvmControls:ControlDefinitions.ComboBoxDefinition="{Binding ComboBoxDefinition}" />
```

### Development Targets

This library utilizes C#9.0, so all consumers must use atleast C#9.0.
- If a consumer is not using C#9, you can refer to the following guide: https://blog.ndepend.com/using-c9-record-and-init-property-in-your-net-framework-4-x-net-standard-and-net-core-projects/
 - The file 'CSharp9_MissingComponents.cs' is already included within this library, so the file can just be included in your project to provide the necesary components, then include the following in your project file: `<LangVersion>9.0</LangVersion>`

Build Targets:
- .NetFramework `net472`, `net480`
- `.NetCoreApp3.1`
- `.Net5.0-windows`

