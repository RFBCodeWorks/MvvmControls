

# MvvmControls
 This is a Library designed to ease working within an MVVM framework, and is meant to consolidate boiler-plate ViewModel interactions for common WPF controls into pre-built classes. 

## Mvvm.Primitive Classes
These classes are the base classes the main objects are derived from, and follow a strucute similar to System.Windows.Controls namespace.

- **ControlBase : ObservableObject**
	- Properties:- bool IsEnabled { get; set; }
	   - bool IsVisible { get; set; }
	   - Visibility Visibility { get; set; }
   - Events:
	   - VisibilityChanged   
- **ItemSource<T,E> : ControlBase `where E:IList<T>`**
	- Base class for an ItemSource control. 
	- Type Parameters
	   - T : The object that will be housed in the collection of type E
	   - E : The collection type - Must implement `IList<T>`
   - Properties
	   - string DisplayMemberPath - Set which property to display (for example, display the 'Name' in the combobox)
	   - E Items - The collection of items
   - Events:
	   - ItemSourceChanged - occurs when the `Items` property is set to a new collection
- **SelectorDefinition<T,E,V> : ItemSource <T,E> `where E:IList<T>`**
	- Extends the ItemSource<T,E> class for use with Selector controls, such as ComboBox and ListBox
	- Properties:
	   - T SelectedItem -- The currently selected item
	   - string SelectedValuePath -- Optional setting that allows setting a 'ValuePath' for the control to utilize. Ex: "ID" --> SelectedValue = SelectedItem.ID
	   - V SelectedValue -- The currently selected value by the SelectedValuePath - This should be only set by the control, not in code.
   - Events:
	   - SelectedItemChanged - Occurs when a new item is selected
	   - SelectedValueChanged - Occurs when the SelectedValue property changes
- **AbstractCommand,  AbstractCommand`<T>`, AbstractAsyncCommand, AbstractAsyncCommand`<T>`**
   - These are abstract ICommand objects that implement the Community Toolkit's IRelayCommand interface
- **AbstractButtonDefinition, AbstractButtonDefinition`<T>`, AbstractAsyncButtonDefinition, AbstractAsyncButtonDefinition`<T>`**
   - These are abstract classes that implement the IButtonDefinition & ICommand interfaces, for use with to bind with to the button control  
 - **ToggleButtonDefinition**
   - Base class for RadioButton and CheckBox button definitions

## Concrete Classes
- **ButtonDefinition, ButtonDefinition`<T>`, AsyncButtonDefinition, AsyncButtonDefinition`<T>`**
	- These sealed classes are derived from their respective abstract versions.
	- They utilize IRelayCommand objects to implement ICommand. 
- **RelayCommand, RelayCommand`<T>`, AsyncRelayCommand, AsyncRelayCommand`<T>`**
	- These ICommand objects are sealed, but are derived from their respective abstract primitives.
- **ListBoxDefinition, ComboBoxDefinition** - Derived from `Primitives.SelectorDefinition`
   - ListBox- and ComboBox- Definition objects are used to represent a user's interaction with a Listbox or ComboBox
   - Both extend Primitives.SelectorDefinition with properties specific to their associated controls.
   - With the `WPF.Behaviors.MultiItemSelectionBehavior` properties attached to a listbox, a Listbox also supports multi-item selection modes.
- **CheckBoxDefinition, RadioButtonDefinition** - Derived from `Primitives.ToggleButton`
   - CheckBoxDefinition is directly derived from `Primitives.ToggleButton`, and implements `ICheckBox`
   - RadioButton adds the 'GroupName' property, and implements `IRadioButton`. 
   - RadioButton also overrides the `IsThreeState` property to `FALSE`.
   
- **TextControlDefinition** -  Derived from `Primitives.ControlBase`
   - While a textbox would normally just be bound to a string, since this is derived from ControlBase, it allows for the enabling/disabling/visibility control over the textbox.
   Additional members include:
	   - Func<string> GetText - A function that can be executed to set the Text property
	   - string Text - The text to display in the textbox
	   - bool IsReadOnly - Set the textbox to ReadOnly mode
	   - Refresh() - Invokes GetText() to update the Text property
	   - Refresh(object, eventargs) - Provides a way to automatically refresh the Text property by subscribing the object to an external event

## A simplified ComboBox Experience
 
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
  xmlns:Behaviors="clr-namespace:RFBCodeWorks.WPF.Behaviors;assembly=RFBCodeWorks.Mvvm.Controls"

  <ComboBox Behaviors:ControlDefinitions.ComboBoxDefinition="{Binding ComboBoxDefinition}" />
```

### Development Targets

This library utilizes C#9.0 for init setters.
- If a consumer is not using C#9, you can refer to the following guide: https://blog.ndepend.com/using-c9-record-and-init-property-in-your-net-framework-4-x-net-standard-and-net-core-projects/
 - The file 'CSharp9_MissingComponents.cs' is already included within this library, so the file can just be included in your project to provide the necesary components, then include the following in your project file: `<LangVersion>9.0</LangVersion>`

Build Targets:
- .NetFramework `net472`, `net480`
- `.NetCoreApp3.1`
- `.Net5.0-windows`

