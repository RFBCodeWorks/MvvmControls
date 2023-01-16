## RFBCodeWorks.Mvvm.IViewModel

This dll provides the following:

IViewModel 
- Interface that allows a ViewModel to specify a parent IViewModel object.
- This would be used for consumers that want to potentially have several ViewModels that are loosely associated (such as a window model, and a control/page model), so a caller can get to the window model from the control model

INotifyArgs
- This sealed concrete class is a container to house and implicitly return the default arguments for INotifyPropertyChanged and INotifyPropertyChanging
- Usage would be:

```
static readonly INotifyArgs MyPropertyChangingArgs = new INotifyArgs(nameof(MyProperty));

public void DoSomething() {
    OnPropertyChanging(MyPropertyChangingArgs);
    // Update Property
    OnPropertyChanged(MyPropertyChangingArgs); 
}
```