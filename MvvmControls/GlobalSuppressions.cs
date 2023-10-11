// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0090:Use 'new(...)'", Justification = "Its an unnecessary suggestion")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required Delegate", Scope = "member", Target = "~M:RFBCodeWorks.Mvvm.Primitives.ControlBase.ReturnTrue``1(``0)~System.Boolean")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required Delegate", Scope = "member", Target = "~M:RFBCodeWorks.Mvvm.Primitives.AbstractAsyncButtonDefinition`1.ReturnTrue(`0)~System.Boolean")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required Delegate", Scope = "member", Target = "~M:RFBCodeWorks.Mvvm.Primitives.CommandBase.ReturnTrue``1(``0)~System.Boolean")]
[assembly: SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "This is desired message.", Scope = "member", Target = "~M:RFBCodeWorks.Mvvm.Primitives.AbstractCommand`1.ThrowIfInvalidParameter(System.Object)~`0")]
